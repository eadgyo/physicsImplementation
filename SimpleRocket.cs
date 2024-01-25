using System;

namespace Edge
{
    public class SimpleRocket : Ode
    {
        protected int numberOfEngines;
        protected double seaLevelThrustPerEngines; 
        protected double vacuumThrustPerEngine; 
        protected double rocketDiameter; 
        protected double cd; 
        protected double initialMass; 
        protected double burnTime; 
        protected USatm76 air;

        public SimpleRocket(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double initialMass, double massFlowRate, int numberOfEngines,
        double seaLevelThrustPerEngines, double vacuumThrustPerEngine, double rocketDiameter, double cd, double theta, double omega, double burnTime) : base(10)
        {
            // Load the initial values into the s field
            // and q array from the ODE class
            S = time;
            Q[0] = vx0;
            Q[1] = x0;
            Q[2] = vy0;
            Q[3] = y0;
            Q[4] = vz0;
            Q[5] = z0;
            Q[6] = massFlowRate;
            Q[7] = initialMass;
            Q[8] = omega;
            Q[9] = theta;

            // Initialize the values of the fields declared
            // In the SimpleRocket class.
            this.numberOfEngines = numberOfEngines;
            this.seaLevelThrustPerEngines = seaLevelThrustPerEngines;
            this.vacuumThrustPerEngine = vacuumThrustPerEngine;
            this.rocketDiameter = rocketDiameter;
            this.cd = cd;
            this.initialMass = initialMass;
            this.burnTime = burnTime;

            air = new USatm76(z0);
        }

        public void UpdatePositionAndVelocity(double dt) 
        {
            OdeSolver.RungeKutta(this, dt);
        }

        public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
        {
            double[] dQ = new double[10];
            double[] newQ = new double[10];

            // Compute the intermediate values of the location and velocity components.
            for (int i=0; i < 10; i++) {
                newQ[i] = q[i] + qScale * deltaQ[i];
            }

            // Assign convenience variables to the intermediate
            // values of the locations and velocities
            double vx = newQ[0];
            double vy = newQ[2];
            double vz = newQ[4];
            double vtotal = Math.Sqrt(vx * vx + vy * vy + vz * vz);
            double x = newQ[1];
            double y = newQ[3];
            double z = newQ[5];
            double massFlowRate = newQ[6];
            double mass = newQ[7];
            double omega = newQ[8];
            double theta = newQ[9];

            // Update the values of pressure, density and 
            // temperature based on the current altitude.
            air.updateConditions(z);
            double pressure = air.GetPressure();
            double density = air.GetDensity();

            // Compute the thrust per engine and total thrust
            double pressureRatio = pressure / 101325.0;
            double thrustPerEngine = vacuumThrustPerEngine - (vacuumThrustPerEngine - seaLevelThrustPerEngines) * pressureRatio;
            double thrust = numberOfEngines * thrustPerEngine;

            // Compute the drag force based on the frontal area
            // of the rocket.
            double area = 0.25 * Math.PI * rocketDiameter * rocketDiameter;
            double drag = 0.5 * cd * density * vtotal * vtotal * area;

            // Compute the gravitational acceleration as a function of altitude.
            double re = 6356766.0;
            double g = 9.80665 * re * re / Math.Pow(re + z, 2.0);

            // For the simulation, lift will be assumed to be zero.
            double lift = 0.0;

            // Compute the force components in the x- and z-directions.
            // The rocket will be assumed to be traveling in the x-z plane.
            double Fx = (thrust - drag) * Math.Cos(theta) - lift * Math.Sin(theta);
            double Fz = (thrust - drag) * Math.Sin(theta) + lift * Math.Cos(theta) - mass * g;

            // Load the right-hand side of the ODEs
            dQ[0] = ds * (Fx / mass);
            dQ[1] = ds * vx;
            dQ[2] = 0.0;
            dQ[3] = 0.0;
            dQ[4] = ds * (Fz / mass);
            dQ[5] = ds * vz;
            dQ[6] = 0.0; // Mass flow rate is constant
            dQ[7] = - ds * (massFlowRate * numberOfEngines);
            dQ[8] = 0.0; // d(theta)/dt is constant
            dQ[9] = ds * omega;

            return dQ; 
        }
    }
}
