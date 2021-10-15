using Godot;
using System;

public class Hero : KinematicBody2D
{
	// Variables
	public bool IsPlayerYelling = false;
	public int Speed = 200;
	public int HearingDistance = 150; // Max distance from which the hero can hear the player yelling.
	[Export]
	public int SpriteMargin = 25; // Margin to help with clamping so hero stays on screen.
	public Node2D WhoWeMovingTowards = null; // Who are we moving towards atm.

	private Vector2 screenSize_;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenSize_ = GetViewport().Size;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		var player = GetParent().GetNode<Player>("Player");
		if (WhoWeMovingTowards != player && IsPlayerYelling && Position.DistanceTo(player.Position) < HearingDistance)
		{
			GD.Print("I hear something");
			IsPlayerYelling = false;
			Rotation = (player.Position - Position).Angle();
			WhoWeMovingTowards = null;
		}
		if(WhoWeMovingTowards == null) 
		{
			Node2D collider = (Node2D)GetNode<RayCast2D>("RayCast2D").GetCollider();
			// Recognize a collision if we hit something, but if it's a Player,
			// also check that they're not disguised.
			if (collider != null && (!(collider is Player) || ((Player)collider).IsDisguised))
			{
				// If the raycast collides with something, move towards it.
				WhoWeMovingTowards = collider;
			}
			else
			{
				// Rotate the hero until the raycast collides with something
				Rotation += 1 * delta;
			}
		}
		else
		{
			if (WhoWeMovingTowards == player && !player.IsDisguised)
			{
				WhoWeMovingTowards = null;
				return;
			}
			// If we're already locked to something, move towards it.
			var direction = Speed * (WhoWeMovingTowards.Position - Position).Normalized();
			Position += direction * delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.x, 0 + SpriteMargin, screenSize_.x - SpriteMargin),
				y: Mathf.Clamp(Position.y, 0 + SpriteMargin, screenSize_.y - SpriteMargin)
			);
			Rotation = (WhoWeMovingTowards.Position - Position).Angle();
		}
	}
	
	public void OnPlayerYell()
	{
			IsPlayerYelling = true;
	}
}
