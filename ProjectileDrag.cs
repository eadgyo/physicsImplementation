using System;
using Godot;

namespace Edge
{
    public class ProjectileDrag : SimpleProjectile
    {
        protected double mass;
        protected double area;

        protected double density;

        protected double Cd;

        public ProjectileDrag(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double Cd) : base(x0, y0, z0, vx0, vy0, vz0, time)
        {
            this.mass = mass;

            this.area = area;

            this.density = density;

            this.Cd = Cd;
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

            // Declare some convenience variable representing
            // the intermediate values of the velocity.

            double vx = newQ[0];
            double vy = newQ[2];
            double vz = newQ[4];

            // Compute the velocity magnitude and add 1+e-08
            // to make sure no there is no division by zero.
            double v = Math.Sqrt(vx * vx + vy * vy + vz * vz) + 1e-08;

            // Compute the total drag force
            double Fd = 0.5 * density * area * Cd * v * v;

            // Compute the right hand sides of the six ODEs
            dQ[0] = -ds * Fd * vx / (mass * v);
            dQ[1] = ds * vx;
            dQ[2] = -ds * Fd * vy / (mass * v);
            dQ[3] = ds * vy;
            dQ[4] = ds * (-G - Fd * vz / (mass * v));
            dQ[5] = ds * vz;

            return dQ;
        }
    }
}
