using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Godot;

namespace Edge
{
    public class Car : ProjectileDrag 
    {
        private double muR;
        private double omegaE;
        private double redline;
        private double finalDriveRatio;
        private double wheelRatio;
        private int gearNumber;
        private string mode;
        private double[] gearRatio;
        private int numberOfGears;

        public Car(double x, double y, double z, double vx, double vy, double vz, double time, double mass, double area, double density, double Cd, double redline, double finalDriveRatio, double wheelRatio, int numberOfGears) : base(x, y, z, vx, vy, vz, time, mass, area, density, Cd)
        {
            this.Redline = redline;
            this.FinalDriveRatio = finalDriveRatio;
            this.WheelRatio = wheelRatio;
            this.NumberOfGears = numberOfGears;

            GearRatio = new double[numberOfGears + 1];
            GearRatio[0] = 0.0;
            for (int i = 1; i < numberOfGears; ++i) {
                GearRatio[i] = 1.0;
            }

            MuR = 0.015; // Coefficient of rolling friction
            OmegaE = 1000.0; // Engine rpm
            GearNumber = 1; // Gear the car is in
            Mode = "accelerating"; // Accelerating, cruising or braking
        }

        public double MuR { get => muR; set => muR = value; }
        public double OmegaE { get => omegaE; set => omegaE = value; }
        public double Redline { get => redline; set => redline = value; }
        public double FinalDriveRatio { get => finalDriveRatio; set => finalDriveRatio = value; }
        public double WheelRatio { get => wheelRatio; set => wheelRatio = value; }
        public int GearNumber { get => gearNumber; set => gearNumber = value; }
        public string Mode { get => mode; set => mode = value; }
        public double[] GearRatio { get => gearRatio; set => gearRatio = value; }
        public int NumberOfGears { get => numberOfGears; set => numberOfGears = value; }

        public double CurrentGearRatio { get => gearRatio[gearNumber]; set => gearRatio[gearNumber] = value;}

        // Simulate a gear shift
        public void shiftGear(int shift) {
            // If the car will shift beyon highes gear, return.
            if ( shift + GearNumber > NumberOfGears) {
                return;
            }
            // If the car shift below 1st gear, return.
            else if (shift + GearNumber < 1) {
                return;
            }
            // Otherwise, change the gear and recompute
            // the engine rpm value.
            else {
                double oldGearRatio = GearRatio[gearNumber];
                GearNumber += shift;
                double newGearRatio = CurrentGearRatio;
                OmegaE *= newGearRatio / oldGearRatio;
            }
            return;
        }

        public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
        {
            double[] dQ = new double[6];
            double[] newQ = new double[6];

            // Compute intermediate values
            for (int i = 0; i < 6; i++) {
                newQ[i] = q[i] + qScale * deltaQ[i];
            }

            // Compute the constants that define the torque curve line.
            double b,d;
            if (OmegaE <= 1000.0) {
                b = 0.0;
                d = 220.0;
            }
            else if (OmegaE < 4600.0) {
                b = 0.025;
                d = 195.0;
            }
            else {
                b = -0.032;
                d = 457.2;
            }

            // Declare some convenience variable representing
            // the intermediate values of the velocity.
            double vx = newQ[0];
            double vy = newQ[2];
            double vz = newQ[4];

            // Compute the velocity magnitude and add 1+e-08
            // to make sure no there is no division by zero.
            double v = Math.Sqrt(vx * vx + vy * vy + vz * vz) + 1e-08;

            // Compute the the directional compontent of the drag force
            // separetely. 
            double Fd = 0.5 * density * area * Cd * v * v;

            // Compute the force of rolling friction. Because
            // The G constant defined in the ProjectileSimple
            // class has negative sign, the value computed here
            // will be negative.
            double Fr = muR * mass * G;

            // Compute the right-hand sides of the six ODEs
            // newQ[0] is the intermediate value of velocity
            // wether the car is accelerating, cruising, or
            // braking. The braking acceleration is assumed
            // to be a constant -5.0 ms/^2
            if (mode.Equals("accelerating")) {
                double c1 = -Fd/mass;
                double tmp = CurrentGearRatio * FinalDriveRatio / wheelRatio;
                double c2 = 60.0 * tmp * tmp * b * v / (2.0 * Math.PI * mass);
                double c3 = (tmp *d + Fr) / mass;
                dQ[0] = ds * (c1 + c2 + c3);
            }
            else if ( mode.Equals("braking")) {
                // Only brake if the velocity is positive
                if ( newQ[0] > 0.1 ) {
                    dQ[0] = ds * (-5.0);
                }
                else {
                    dQ[0] = 0.0;
                }
            }
            else {
                dQ[0] = 0.0;
            }
            
            // Compute the right hand sides of the six ODEs
            dQ[1] = ds * newQ[0];
            dQ[2] = 0;
            dQ[3] = 0;
            dQ[4] = 0;
            dQ[5] = 0;

            return dQ;
        }
    }
}
