using Godot;
using System;

public class Hero : KinematicBody2D
{
		// Variables
		public bool IsPlayerDisguised = false;
		public int Speed = 200;
		[Export]
		public int SpriteMargin = 25; // Margin to help with clamping so hero stays on screen.
		[Export]
		public Node2D Player; // We can set this in editor or the game controller can set.
		public Node2D WhoWeMovingTowards = null; // Who are we moving towards atm.
	
		private Vector2 screenSize_;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
				screenSize_ = GetViewport().Size;
		}
		
		public override void _PhysicsProcess(float delta)
		{
			var spaceState = GetWorld2d().DirectSpaceState;
			if(WhoWeMovingTowards == null) 
			{
				var rayCast = GetNode<RayCast2D>("RayCast2D");
				rayCast.CollideWithAreas = true;
				var node_ = (Node2D)rayCast.GetCollider();				
				if (rayCast.IsColliding())
				// If the raycast collides with something, move towards it
				{
					if(node_.Name == "Player" && !IsPlayerDisguised) {
						Rotation += 1 * delta;
						return;
					}
					WhoWeMovingTowards = node_;
				}
				else
				{
					// Rotate the hero until the raycast collides with something				
					Rotation += 1 * delta;
				}
			}
			else 
			{
				var direction = (GlobalPosition - WhoWeMovingTowards.Position);
				direction = new Vector2(
						x: Mathf.Clamp(direction.x, 0 + SpriteMargin, screenSize_.x - SpriteMargin),
						y: Mathf.Clamp(direction.y, 0 + SpriteMargin, screenSize_.y - SpriteMargin)
					);
					//var collision = MoveAndCollide(direction * delta);
					//var collision = MoveAndCollide(WhoWeMovingTowards.Position *-1 * delta);
	//				if (collision != null)
	//				{
	//					GD.Print("Hero hit " + collision.Collider);
	//				}
					Position += direction * delta;
			}

		} 
		
		public void FlipIsPlayerDisguised()
		{
			IsPlayerDisguised = !IsPlayerDisguised;
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
		}
		
		public void AddToRayCastExceptions(Node node)
		{
			GetNode<RayCast2D>("RayCast2D").AddException(node);
		}
}
