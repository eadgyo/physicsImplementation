using System;
using Godot;

namespace Edge
{
    public class FountainLightning : Powerboat
    {
        public FountainLightning(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double planingSpeed) : base(x0, y0, z0, vx0, vy0, vz0, time, planingSpeed)
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

            double v = newQ[0];
            double ax;
            if (mode.Equals("accelerating")) {
                // if the velocity is at or above the maximum
                // value, set the acceleration to zero.
                if (v >= 46.1) {
                    ax = 0.0;
                }
                // if the velocity is less than 11.2 m/s, set the acceleration
                else if (v < 11.2) {
                    ax = 2.1;
                } 
                // otherwise, evaluate the acceleration according
                // to the curve fit equation
                else {
                    ax = -4.44e-7 * Math.Pow(v, 4.0) + 2.56e-4 * Math.Pow(v, 3.0) - 0.0216 *v *v + 0.527 *v - 1.51;
                }
            }
            // otherwise, evaluate the acceleration according
            // to the curve fit equation
            else if (mode.Equals("decelerating")) {
                // Only decelerate if the velocity is positive.
                if (newQ[0] > 0.1) {
                    ax = -2.0;
                } else {
                    ax = 0.0;
                }
            }
            // if the mode is "cruising", set the accerlation
            else {
                ax = 0.0;
            }

            // Compute the right hand sides of the six ODEs
            dQ[0] = ds * ax;
            dQ[1] = ds * newQ[0];
            dQ[2] = 0;
            dQ[3] = 0;
            dQ[4] = 0;
            dQ[5] = 0;

            return dQ;
        }
    }
}
