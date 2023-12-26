using System;
using System.Security.Cryptography.X509Certificates;
using Godot;

namespace Edge
{
    public class GolfBall : SpinProjectile
    {
        public GolfBall(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double density, double windx, double windy, double rx, double ry, double rz) : base(x0, y0, z0, vx0, vy0, vz0, time, 0.0459, Math.PI*0.02135*0.02135, density, 0.22, windx, windy, rx, ry, rz, 0.0, 0.02135)
        {
                    
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
            double Cl = -0.05 + Math.Sqrt(0.0025 + 0.36 * Math.Abs(radius * omega / v));
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

        public void ComputePostCollisionVelocity(double velocity) 
        {
            // 9 iron
            double clubMass = 0.285;
            double loft = 43.0;
            double Crest = 0.78; // Coeff of restitution
 
            // Convert the lof angle from degrees to radians and 
            // assign values to some convenience variables.
            loft = loft * Math.PI /180.0;
            double cosL = Math.Cos(loft);
            double sinL = Math.Sin(loft);

            // Calculate the pre-collision velocities normal
            // and parallel to the line of action
            double vcp = cosL * velocity;
            double vcn = -sinL * velocity;

            // Compute the post-collision velocity of the ball
            // along the line of action
            double vbp = (1.0 + Crest) * clubMass * vcp / (clubMass + mass);

            // Compute the post-collision velocity of the ball
            // along the perpendicular to the line of action
            double vbn = ( 2.0 / 7.0 ) * clubMass * vcn / (clubMass + mass);

            // Compute the initial spin rate assumin  ball is rolling without sliding
            omega = (5.0/7.0) * vcn / radius;

            // Rotate the post-collision ball velocities back into
            // standard Cartesian frame of reference. Because the line
            // of action was in the xy plan, the z-veloctiy is zero.
            Q[0] = cosL * vbp - sinL * vbn; // vx0
            Q[2] = 0.0; // vy0
            Q[4] = sinL * vbp + cosL * vbn; // vz0
        }
    }


}
