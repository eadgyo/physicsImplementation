namespace Edge {

public class OdeSolver
{
    public static void RungeKutta(Ode ode, double ds)
    {
        double s;
        int numEqns = ode.NumEqns;
        double[] q;
        double[] dq1 = new double[numEqns];
        double[] dq2 = new double[numEqns];
        double[] dq3 = new double[numEqns];
        double[] dq4 = new double[numEqns];

        // Retrieve the current values of the dependant and independant variables
        s = ode.S;
        q = ode.Q;

        // Compute the four rangekuttas step and get the return value of the right
        // hand side of the method of the delat q values of the steps
        dq1 = ode.GetRightHandSide(s, q, q, ds, 0.0);
        dq2 = ode.GetRightHandSide(s + 0.5 * ds, q, dq1, ds, 0.5);
        dq3 = ode.GetRightHandSide(s + 0.5 * ds, q, dq2, ds, 0.5);
        dq4 = ode.GetRightHandSide(s + ds, q, dq3, ds, 1.0);

        // Update the dependant and independant variables
        // at the new variable location 
        ode.S = s + ds;

        for (int j = 0; j < numEqns; ++j) {
            q[j] = q[j] + (dq1[j] + 2*dq2[j] + 2*dq3[j] + dq4[j])/6.0;
        }

        ode.Q = q;
    }

}

}