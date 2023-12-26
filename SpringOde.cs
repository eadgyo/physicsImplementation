
namespace Edge {
public class SpringOde : Ode
{
    private double mass; // Mass of the end of the spring
    private double mu; // drag coef

    private double k; // spring constant

    private double x0; // initial spring deflection

    private double time; // independant variable


    public double Mass { get => mass; set => mass = value; }
    public double Mu { get => mu; set => mu = value; }
    public double K { get => k; set => k = value; }
    public double X0 { get => x0; set => x0 = value; }
    public double Time { get => time; set => time = value; }


    public SpringOde(double mass, double mu, double k, double x0) : base(2)
    {
        this.mass = mass;
        this.mu = mu;
        this.k = k;
        this.x0 = x0;
        Q[0] = 0.0; // vx
        Q[1] = 1; // X0
    }

    public void UpdatePositionAndVelocity(double dt) 
    {
        OdeSolver.RungeKutta(this, dt);
    }

    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        double[] dq = new double[2];
        double[] newQ = new double[2];

        // compute the intermediate dependant variables
        for (int i = 0; i < 2; i++)
        {
            newQ[i] = q[i] + qScale * deltaQ[i];
        }

        // compute right hand side values
        dq[0] = -ds * (mu * newQ[0] + K*newQ[1]) / mass;
        dq[1] = ds * newQ[0];

        return dq;
    }
} 

}