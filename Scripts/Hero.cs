using Godot;
using System;

public class Hero : RigidBody2D
{

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
				
		}
		
		public override void _PhysicsProcess(float delta)
		{
			var spaceState = GetWorld2d().DirectSpaceState;
			var rayCast = GetNode<RayCast2D>("RayCast2D");
			if (rayCast.IsColliding())
			{
				GD.Print("Hit " + rayCast.GetCollider() + " at point: " + rayCast.GetCollisionPoint());
			}
		}
		

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
