using System;

namespace Edge
{
    public class F1Rocket : SimpleRocket
    {
        public F1Rocket(double x0, double y0, double z0, double vx0, double vy0, double vz0) : base(x0, y0, z0, vx0, vy0, vz0, 0.0, 0.0, 2616.0, 1, 6.67e+6, 7.86e+6, 10.0, 0.5, 0.5 * Math.PI, -80 * Math.PI / (180.0 * 150.0), 150.0)
        {
    
            double engineMass = 8371.0;
            double propellantMass =  Q[6] * burnTime;
            double structureMass = 20000.0 + 1*4000.0;

            initialMass = 1*(engineMass + propellantMass) +
                         0.0 + structureMass;
             Q[7] = initialMass;
        }
    }
}
