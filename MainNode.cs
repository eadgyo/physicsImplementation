using System;
using System.Collections;
using System.Xml;
using Godot;
namespace Edge {
public class MainNode : Node2D
{
    double mass = 1.0;
    double mu = 1.5;
    double k = 20.0;
    double x0 = -0.2;

    SpringOde springOde;

    public MainNode()
    {
       
    }

    public void ReadySpring() 
    {
        springOde = new SpringOde(mass, mu, k, x0);
        
        GD.Print("test");
        double dt = 0.1;
        double lastElement = springOde.Q[1];
        Line2D line = new Line2D
        {
            Width = 5.0f
        };
        while (springOde.S < 14.0) {
            GD.Print(new Vector2((float)springOde.S, (float)springOde.Q[1]));
            line.AddPoint(new Vector2((float)springOde.S + 1, ((float)springOde.Q[1] + 1) * 4) * 50);
            springOde.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyProjectile() 
    {
        SimpleProjectile projectile = new SimpleProjectile(0, 0, 0.1, 50*Math.Cos(Math.PI/4), 0, 50*Math.Sin(Math.PI/4), 0);
        double dt = 0.1;
        Line2D line = new Line2D
        {
            Width = 5.0f
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.S, (float)projectile.Q[5]));
            line.AddPoint(new Vector2((float)projectile.S * 7 + 10, 70 - ((float)projectile.Q[5])) * 8);
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyProjectileWithDrag() 
    {
        ProjectileDrag projectile = new ProjectileDrag(0, 0, 0.1, 50*Math.Cos(Math.PI/4), 0, 50*Math.Sin(Math.PI/4), 0, 20, Math.PI * 0.10, 1.293, 0.25);
        double dt = 0.1;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 0, 0)
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.S, (float)projectile.Q[5]));
            line.AddPoint(new Vector2((float)projectile.S * 7 + 10, 70 - ((float)projectile.Q[5])) * 8);
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyProjectileWithWind() 
    {
        WindProjectile projectile = new WindProjectile(0, 0, 0.1, 50*Math.Cos(Math.PI/4), 0, 50*Math.Sin(Math.PI/4), 0, 20, Math.PI * 0.10, 1.293, 0.25, 20, -10);
        double dt = 0.1;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(0, 255, 0)
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.S, (float)projectile.Q[5]));
            line.AddPoint(new Vector2((float)projectile.S * 7 + 10, 70 - ((float)projectile.Q[5])) * 8);
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyProjectileWithSpin() 
    {
        SpinProjectile projectile = new SpinProjectile(0, 0, 0.1, 50*Math.Cos(Math.PI/4), 0, 50*Math.Sin(Math.PI/4), 0, 20, Math.PI * 0.10, 1.293, 0.25, 5, 10, 0, 1, 0, 250, 0.02);
        double dt = 0.1;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.S, (float)projectile.Q[5]));
            line.AddPoint(new Vector2((float)projectile.S * 7 + 10, 70 - ((float)projectile.Q[5])) * 8);
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyGolfBall() 
    {
        GolfBall projectile = new GolfBall(0, 0, 0.1, 0, 0, 0, 0, 1.225, 0, 0, 0, 1.0, 0);
        // Create impact
        projectile.ComputePostCollisionVelocity(50.0);

        double dt = 0.01;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.S, (float)projectile.Q[5]));
            line.AddPoint(new Vector2((float)projectile.Q[1] / 2 + 10, 100 - ((float)projectile.Q[5])) * 5);
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadySoccer() 
    {
        double spinRate = 10.0;
        double omega = spinRate * 2.0 * Math.PI;
        SoccerBall projectile = new SoccerBall(50, 0, 0.1, -28.0, 9.0, 4.0, 0, 0.43, Math.PI * 0.10 * 0.10, 1.2, 0.25, 0, 0, 0, 0, -1.0, omega, 0.10, 294.0);
        double dt = 0.01;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        while (projectile.Q[5] > 0) {
            GD.Print(new Vector2((float)projectile.Q[1] * 5, 200 - ((float)projectile.Q[3]))  );
            line.AddPoint(new Vector2((float)projectile.Q[1] * 20 - 200, 200 - ((float)projectile.Q[3]) * 20) );
            projectile.UpdatePositionAndVelocity(dt);
        }
        AddChild(line);
    }

    public void ReadyCar() {
        Boxter boxter = new Boxter(50, 0, 0, 0, 0, 0, 0, 1.0);
        double dt = 0.06;
        Line2D line = new Line2D
        {
            Width = 4.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        double engineRPM = 0.0;
        double time = 0.0;
        for (int i = 0; i < boxter.NumberOfGears; i++) {
            engineRPM = boxter.OmegaE;
            double startTime = time;
            while (engineRPM < boxter.Redline) {
                boxter.UpdatePositionAndVelocity(dt);
                time += dt;
                engineRPM = boxter.VX * 60.0 * boxter.CurrentGearRatio * boxter.FinalDriveRatio / (2.0 * Math.PI * boxter.WheelRatio);
                boxter.OmegaE = engineRPM;
                //GD.Print(new Vector2((float)time, (float)engineRPM));
                line.AddPoint(new Vector2(10 + (float)time * 10 , 500 -(float)engineRPM / 20));
                //GD.Print(new Vector2(10 +(float)time * 10 , 500 -(float)engineRPM / 20));
            }
            GD.Print("Current gear : " + boxter.GearNumber);
            GD.Print(new Vector2((float)(time - startTime), (float)boxter.VX));
            boxter.shiftGear(1);
        }
        AddChild(line);
        Line2D RedLine = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 0, 0)
        };
        RedLine.AddPoint(new Vector2(0 , 500 -(float)7200 / 20));
        RedLine.AddPoint(new Vector2(OS.GetScreenSize().x, 500 -(float)7200 / 20));
        AddChild(RedLine);
    }

    public void ReadyBoat() 
    {
        FountainLightning boat = new FountainLightning(50, 0, 0, 0, 0, 0, 0, 8.4);
        double dt = 0.1;
        double time = 0;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        while (boat.Q[1] < 1300) {
            GD.Print(new Vector2(new Vector2(10 + (float)time * 10, (float)boat.Q[0] * 5)));
            line.AddPoint(new Vector2(10 + (float)time * 20, 500 - (float)boat.Q[0] * 10) );
            boat.UpdatePositionAndVelocity(dt);
            time += dt;
        }
        AddChild(line);
    }

    public void DisplayProjectiles() 
    {
        ReadyProjectile();
        ReadyProjectileWithDrag();
        ReadyProjectileWithWind();
        ReadyProjectileWithSpin();
    }

    public void ReadyPlane() {
        Skyhawk plane = new Skyhawk(50, 0, 0, 0, 0, 0, 0);
        double dt = 0.1;
        double time = 0;
        Line2D line = new Line2D
        {
            Width = 5.0f,
            DefaultColor = Color.Color8(255, 255, 0)
        };
        while (plane.Q[5] < 1300) {
            GD.Print(new Vector2(new Vector2(10 + (float)time * 10, (float)plane.Q[0] * 5)));
            line.AddPoint(new Vector2(10 + (float)time * 20, 500 - (float)plane.Q[0] * 10) );
            plane.UpdatePositionAndVelocity(dt);
            time += dt;
        }
        AddChild(line);
    }

    public override void _Ready()
    {
        base._Ready();
        ReadyBoat();
    }

    public override void _Draw()
    {

    }
}
}