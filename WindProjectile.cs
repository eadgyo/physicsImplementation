using System;

namespace Edge
{
    public class WindProjectile : ProjectileDrag
    {
        protected double windx;

        protected double windy;
        public WindProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double Cd, double windx, double windy) : base(x0, y0, z0, vx0, vy0, vz0, time, mass, area, density, Cd)
        {
            this.windx = windx;
            this.windy = windy;
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

            // Compute the total drag force
            double Fd = 0.5 * density * area * Cd * va * va;

            // Compute the right hand sides of the six ODEs
            dQ[0] = -ds * Fd * vax / (mass * va);
            dQ[1] = ds * vx;
            dQ[2] = -ds * Fd * vay / (mass * va);
            dQ[3] = ds * vy;
            dQ[4] = ds * (-G - Fd * vaz / (mass * va));
            dQ[5] = ds * vz;

            return dQ;
        }
    }
}
