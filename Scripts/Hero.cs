using Godot;
using System;

public class Hero : KinematicBody2D
{
		// Variables
		public bool IsPlayerDisguised = false;
		public bool IsPlayerDefending = false;
		public int Speed = 200;
		[Export]
		public int SpriteMargin = 25; // Margin to help with clamping so hero stays on screen.
	
		private Vector2 screenSize_;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
				screenSize_ = GetViewport().Size;
		}
		
		public override void _PhysicsProcess(float delta)
		{
			var spaceState = GetWorld2d().DirectSpaceState;
			var rayCast = GetNode<RayCast2D>("RayCast2D");
			if (rayCast.IsColliding())
			// If the raycast collides with something, move towards it
			{
				var direction = (GlobalPosition - rayCast.GetCollisionPoint()).Normalized();
				direction = new Vector2(
					x: Mathf.Clamp(direction.x, 0 + SpriteMargin, screenSize_.x - SpriteMargin),
					y: Mathf.Clamp(direction.y, 0 + SpriteMargin, screenSize_.y - SpriteMargin)
				);
				var collision = MoveAndCollide(direction * delta);
				if (collision != null)
				{
					GD.Print("Hero hit " + collision.Collider);
				}
			}
			else
			// Rotate the hero until the raycast collides with something
			{
				Rotation += 1 * delta;
			}
		} 
		
		public void FlipIsPlayerDisguised()
		{
			IsPlayerDisguised = !IsPlayerDisguised;
		}
		
		public void FlipIsPlayerDefending()
		{
			IsPlayerDefending = !IsPlayerDefending;
		}
		
		public void EditRayCastExceptions(Node obj)
		{
			var rayCast = GetNode<RayCast2D>("RayCast2D");
			if (IsPlayerDisguised)
			{
				rayCast.RemoveException(obj);
			} else {
				rayCast.AddException(obj);
			}
			
			if (IsPlayerDefending)
			{
				rayCast.Enabled = false;
			}
			else
			{
				rayCast.Enabled = true;
			}
		}
		
		public void AddToRayCastExceptions(Node node)
		{
			GetNode<RayCast2D>("RayCast2D").AddException(node);
		}
}
