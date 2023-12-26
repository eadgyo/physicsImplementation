using System;

namespace Edge
{
    public class SoccerBall : SpinProjectile
    {
        protected double temperature;
        public SoccerBall(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double Cd, double windx, double windy, double rx, double ry, double rz, double omega, double radius, double temperature) : base(x0, y0, z0, vx0, vy0, vz0, time, mass, area, density, Cd, windx, windy, rx, ry, rz, omega, radius)
        {
            this.temperature = temperature;
        }

        public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
        {
            double[] dQ = new double[6];
            double[] newQ = new double[6];

            // Compute intermediate values
            for (int i = 0; i < 6; i++) {
                newQ[i] = q[i] + qScale * deltaQ[i];
            }

            // Declare some convenience variable representing
            // the intermediate values of the velocity.
            double vx = newQ[0];
            double vy = newQ[2];
            double vz = newQ[4];

            // Compute the apparent velocity by substracting
            // the wind velocity component from the projectile 
            // velocity component
            double vax = vx - windx;
            double vay = vy - windy;
            double vaz = vz;

            // Compute the apparent velocity magnitude and add 1+e-08
            // to make sure no there is no division by zero.
            double va = Math.Sqrt(vax * vax + vay * vay + vaz * vaz) + 1e-08;

            // Compute the drag coeff which dependends on the reynolds number.
            double viscosity = 1.458e-6 * Math.Pow(temperature, 1.5) / (temperature + 110.4);
            double Re = density * va * 2.0 * radius / viscosity;

            if ( Re < 1.0e+5 ) {
                Cd = 0.47;
            } else if ( Re > 1.35e+5 ) {
                Cd = 0.22;
            } else {
                Cd = 0.47 - 0.25 * (Re - 1.0e+5) / 35000.0;
            }

            // Compute the the directional compontent of the drag force
            // separetely. 
            double Fd = 0.5 * density * area * Cd * va * va;
            double Fdx = -Fd * vax / va;
            double Fdy = -Fd * vay / va;
            double Fdz = -Fd * vaz / va;

            double v = Math.Sqrt(vx * vx + vy * vy + vz * vz) + 1e-08;

            // Evaluate the Magnus Force terms
            double rotSpinRatio = Math.Abs(radius * omega / v);
            double Cl = 0.385 + Math.Pow(rotSpinRatio, 0.25);
            double Fm = 0.5 * density * area * Cl * v*v;
            double Fmx = + (vy * rz - ry * vz) * Fm / v;
            double Fmy = - (vx * rz - rx * vz) * Fm / v;
            double Fmz = + (vx * ry - rx * vy) * Fm / v;

            // Compute the right hand sides of the six ODEs
            dQ[0] = ds * (Fdx + Fmx) / mass;
            dQ[1] = ds * vx;
            dQ[2] = ds * (Fdy + Fmy) / mass;
            dQ[3] = ds * vy;
            dQ[4] = ds * (-G + (Fdz + Fmz) / mass);
            dQ[5] = ds * vz;

            return dQ;
        }
    }
}
