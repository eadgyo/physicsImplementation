using System;
using System.CodeDom;

namespace Edge
{
    public class SpinProjectile : WindProjectile
    {
        protected double rx; // spin axis x velocity component
        protected double ry; // spin axis y velocity component
        protected double rz; // spin axis z velocity component
        protected double omega; // angular velocity w, m/s
        protected double radius; // sphere radius, m

        public SpinProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double Cd, double windx, double windy, double rx, double ry, double rz, double omega, double radius) : base(x0, y0, z0, vx0, vy0, vz0, time, mass, area, density, Cd, windx, windy)
        {
            this.rx = rx;
            this.ry = ry;
            this.rz = rz;
            this.omega = omega;
            this.radius = radius;
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

            // Compute the the directional compontent of the drag force
            // separetely. 
            double Fd = 0.5 * density * area * Cd * va * va;
            double Fdx = -Fd * vax / va;
            double Fdy = -Fd * vay / va;
            double Fdz = -Fd * vaz / va;

            double v = Math.Sqrt(vx * vx + vy * vy + vz * vz) + 1e-08;

            // Evaluate the Magnus Force terms
            double Cl = radius * omega / v;
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
