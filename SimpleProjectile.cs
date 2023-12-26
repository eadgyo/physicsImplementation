
namespace Edge {
public class SimpleProjectile : Ode
{
    public static double G = 9.81;

    public SimpleProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time) : base(6)
    {
        Q[0] = vx0;
        Q[1] = x0;
        Q[2] = vy0;
        Q[3] = y0;
        Q[4] = vz0;
        Q[5] = z0;

        S = time;
    }

    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {

        return new double[]{Q[0], Q[1], Q[2], Q[3], Q[4], Q[5]};
    }

    public void UpdatePositionAndVelocity(double dt) 
    {
        double time = S;
        double x = Q[1];
        double y = Q[3];
        double z = Q[5];
        double vx = Q[0];
        double vy = Q[2];
        double vz = Q[4];

        
        // Update the xyz location and the z-component of the velocity
        // The x and y velicities don't change.
        Q[1] = vx * dt + x;
        Q[3] = vy * dt + y;
        Q[4] = - (G * dt) + vz;
        Q[5] = vz * dt - (G * 1/2 * dt * dt) + z;

        S += dt;
    }
}
}