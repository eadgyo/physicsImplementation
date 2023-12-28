using System;

namespace Edge
{
    public class Boxter : Car
    {   
        // Mass = 1393.0 kg
        // area = 1.94m^2
        // Cd = 0.31
        // redline = 7200 rpm
        // finalDriveRatio = 3.44
        // wheelRadius = 0.3186
        // numberOfGears = 6
        public Boxter(double x, double y, double z, double vx, double vy, double vz, double time, double density) : base(x, y, z, vx, vy, vz, time, 1393.0, 1.94, density, 0.31, 7200, 3.44, 0.3186, 6)
        {
            // Init gear ratios
            GearRatio[1] = 3.82;
            GearRatio[2] = 2.20;
            GearRatio[3] = 1.52;
            GearRatio[4] = 1.22;
            GearRatio[5] = 1.02;
            GearRatio[6] = 0.82; 
        }
    }
}
