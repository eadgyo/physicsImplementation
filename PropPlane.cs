using System;
using System.Runtime.Remoting.Channels;
using Godot;

namespace Edge
{
    public class PropPlane : SimpleProjectile
    {
        private double bank; 
        private double alpha; // angle of attack
        private double throttle; 
        private double wingArea; 
        private double wingSpan; 
        private double tailArea; 
        private double clSlope0; // Slope of Cl-alpha curve
        private double cl0; // Intercept of Cl-alpha curve
        private double clSlope1; // Post-stall slope of Cl-alpha curve
        private double cl1; // Post-stall intercept of Cl-alpha curve
        private double alphaClMax; // Alpha when Cl=Clmax
        private double cdp; // Parasite drag coefficient
        private double eff; // Induced drag efficiency coefficient
        private double mass; 
        private double enginePower; 
        private double engineRps; // Revolutions per second
        private double propDiameter; 
        private double a; // Propeller efficiency coefficient
        private double b; // Propeller efficiency coefficient
        private string flap; // Flap Deflection Amount

        public PropPlane(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double wingArea, double wingSpan, double tailArea, double clSlope0, double cl0, double clSlope1, double cl1, double alphaClMax, double cdp, double eff, double mass, double enginePower, double engineRps, double propDiameter, double a, double b) : base(x0, y0, z0, vx0, vy0, vz0, time)
        {
            this.wingArea = wingArea;
            this.wingSpan = wingSpan;
            this.tailArea = tailArea;
            this.clSlope0 = clSlope0;
            this.cl0 = cl0;
            this.clSlope1 = clSlope1;
            this.cl1 = cl1;
            this.alphaClMax = alphaClMax;
            this.cdp = cdp;
            this.eff = eff;
            this.mass = mass;
            this.enginePower = enginePower;
            this.engineRps = engineRps;
            this.propDiameter = propDiameter;
            this.a = a;
            this.b = b;

            this.bank = 0.0;
            this.alpha = 0.0;
            this.throttle = 0.0;
            this.flap = "0";
        }
        
        public new void UpdatePositionAndVelocity(double dt) 
        {
            OdeSolver.RungeKutta(this, dt);
        } 

        public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
        {
            double[] dQ = new double[6];
            double[] newQ = new double[6];

            // Compute intermediate values
            for (int i = 0; i < 6; i++) {
                newQ[i] = q[i] + qScale * deltaQ[i];
            }

            // Compute the intermediaete values of the various velocities
            double vx = newQ[0];
            double vy = newQ[2];
            double vz = newQ[4];
            double x = newQ[1];
            double y = newQ[3];
            double z = newQ[5];

            double vh = Math.Sqrt(vx * vx + vy * vy);
            double vtotal = Math.Sqrt(vx * vx + vy * vy + vz * vz);

            // Compute the air density
            double temperature = 288.15 - 0.0065*z;
            double grp = (1.0 - 0.0065*z/288.15);
            double pressure = 101325.0 * Math.Pow(grp, 5.25);
            double density = 0.00348 * pressure / temperature;

            // Compute power drop-off factor.
            double omega = density / 1.225;
            double factor = (omega - 0.12) / 0.88;

            // Compute thrust
            double avdanceRatio = vtotal / (engineRps * propDiameter);
            double thrust = throttle * factor * enginePower * (a + b * avdanceRatio) / (engineRps * propDiameter);

            // Compute the lift coefficient.
            // The Cl curve is modeled using two straight lines.
            double cl;
            if (alpha < alphaClMax ) {
                cl = clSlope0 * alpha + cl0;
            } else {
                cl = clSlope1 * alpha + cl1;
            }

            // Include effects of flaps and ground effects.
            // Ground effects are present if the plane is
            // within 5 meters of the ground.
            if ( flap == "20" ) {
                cl += 0.25;
            } else if ( flap == "40" ) {
                cl += 0.5;
            }
            if (z < 5.0) {
                cl += 0.25;
            }

            // Compute lift.
            double lift = 0.5 * cl * density * vtotal * vtotal * wingArea;

            // Compute drag coefficient
            double aspectRatio = wingSpan * wingSpan / wingArea;
            double cd = cdp + cl * cl / (Math.PI * aspectRatio * eff);

            // Compute the drag force.
            double drag = 0.5 * cd * density * vtotal * vtotal * wingArea; 

            // Define some shorthand convenience variables
            // for use with the rotation matrix.
            // Compute the sine and cosines of the clim angle.
            // bank angle, and heading angle;
            double cosW = Math.Cos(bank);
            double sinw = Math.Sin(bank);

            double cosP; // Climb angle
            double sinP; // Climb angle
            double cosT; // Heading angle
            double sinT; // Heading angle

            if (vtotal == 0.0) {
                cosP = 1.0;
                sinP = 0.0;
            } else {
                cosP = vh / vtotal;
                sinP = vz / vtotal;
            }

            if (vh == 0.0) {
                cosT = 1.0;
                sinT = 0.0;
            } else {
                cosT = vx / vh;
                sinT = vy / vh;
            }

            // Convert the thrust, drag and lift forces into
            // x-, y- and z-components using the rotation matrix.
            double Fx = cosT * cosP * (thrust - drag) + (sinT * sinw - cosT * sinP * cosW) * lift;
            double Fy = sinT * cosP * (thrust - drag) + (-cosT * sinw - sinT * sinP * cosW) * lift;
            double Fz = sinP * (thrust - drag) + cosP * cosW * lift;

            // Add the gravity force of the z-direction force.
            Fz += mass * G;

            dQ[0] = ds * (Fx / mass);
            dQ[1] = ds * vx;
            dQ[2] = ds * (Fy / mass);
            dQ[3] = ds * vy;
            dQ[4] = ds * (Fz / mass);
            dQ[5] = ds * vz;

            return dQ;
        }
    }
}
