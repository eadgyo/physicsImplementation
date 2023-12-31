using System;
using System.Security.Cryptography.X509Certificates;

namespace Edge
{
    public class Powerboat : SimpleProjectile
    {
        protected string mode;
        protected double planingSpeed;
        public Powerboat(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double planingSpeed) : base(x0, y0, z0, vx0, vy0, vz0, time)
        {
            this.planingSpeed = planingSpeed;
            mode = "accelerating";
        }

        public new void UpdatePositionAndVelocity(double dt) 
        {
            OdeSolver.RungeKutta(this, dt);
        } 
    }
}
