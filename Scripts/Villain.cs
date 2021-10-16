using Godot;
using Godot.Collections;
using System;

public class Villain : Area2D
{
	[Signal]
	public delegate void VillainHit(); // Will be sent to trigger penalty when players hit in disguise.
	[Signal]
	public delegate void VillainOffscren();
	
	// Variables
	public int Speed = 200; // No export since we're using it in multiple places in the code.
	
	private bool isRight_ = true; // keep track of bounce direction
	private bool isDown_ = true; // keep track of bounce direction
	private Vector2 screenSize_;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector2();
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		animatedSprite.Play();
		screenSize_ = GetViewport().Size;
		GetNode<AudioStreamPlayer2D>("Laugh").Play();
	}

	public override void _Process(float delta) 
	{
		var velocity = new Vector2();
		velocity.y = isDown_ ? 0.2F : -0.2F;
		if(isRight_)
		{
			velocity.x += 1;
		}
		else 
		{
			velocity.x -= 1;
		}			
		velocity = velocity.Normalized() * Speed;
		Position += velocity * delta;
		
		if (Position.x < 0)
		{
			isRight_ = true;
		} 
		else if (Position.x > screenSize_.x) 
		{
			isRight_ = false;
		}
		
		// Bounce the villain within the screen bounds
		if (Position.y > screenSize_.y)
		{
			isDown_ = false;
			// Hero missed his chance to get him.
//			EmitSignal("VillainOffscren");
//			Hide();
		}
		else if (Position.y < 0) {
			isDown_ = true;
		}
	}

	// Collision
	private void OnVillainBodyEntered(Node body)
	{
		// Only matters if Hero collides
		if(body.Name == "Hero") 
		{
			EmitSignal("VillainHit");
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
			Hide();
		}
	}

}


