using Godot;
using System;
namespace Edge {
public abstract class Ode
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private int numEqns;
	private double[] q;
	private double s;
	
	
	public Ode(int numEqns) 
	{
		this.NumEqns = numEqns;
		q = new double[numEqns];
	}

	public int NumEqns { get => numEqns; set => numEqns = value; }
	public double[] Q { get => q; set => q = value; }
	public double S { get => s; set => s = value; }

	public abstract double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale); 

	
}

}
