using Godot;
using System;

public class Player : Area2D
{
	// Signals
	[Signal]
	public delegate void Hit(); // Will be sent to trigger gameover when players hit in disguise.
	[Signal]
	public delegate void DisguiseChange(); // Signal sent to add/remove player from hero's raycasting.
	[Signal]
	public delegate void Defending(); // Signal sent to add/remove NPC from hero's raycasting
	
	// Variables
	public int Speed = 300; // No export since we're using it in multiple places in the code.
	[Export]
	public int SpriteMargin = 25; // Margin to help with clamping so player stays on screen.
	
	private Vector2 screenSize_;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 screenSize_ = GetViewport().Size;
	}
	
	public override void _Process(float delta) 
	{
		var velocity = new Vector2();
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		animatedSprite.Play();

		// Determine input.		
		if(Input.IsActionPressed("ui_right"))
		{
			velocity.x += 1;
		}	
		else if(Input.IsActionPressed("ui_left")) 
		{
			velocity.x -= 1;
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			velocity.y += 1;
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			velocity.y -= 1;
		}
		
		if (Input.IsKeyPressed((int)KeyList.D))
		{
			// Change sprite to defend mode.
			animatedSprite.Animation = "shield";
			EmitSignal("Defending");
			Speed = 150; // halve speed			
		}
		else if (Input.IsKeyPressed((int)KeyList.C))
		{
			// Change sprite to costume.
			animatedSprite.Animation = "disguise";
			EmitSignal("DisguiseChange");
		}
		else
		{
			// Check to see if we changed from disguise.
			if(animatedSprite.Animation == "disguise")
			{
				EmitSignal("DisguiseChange");
			}
			// Check to see if we stopped defending.
			else if(animatedSprite.Animation == "shield") 
			{
				EmitSignal("Defending");
				Speed = 300; // reset speed
			}
			// Default to normal person mode
			animatedSprite.Animation = "plain";
		}
		
		// Move player
		if (velocity.Length() > 0) 
		{
			velocity = velocity.Normalized() * Speed;
		}
		Position += velocity * delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0 + SpriteMargin, screenSize_.x - SpriteMargin),
			y: Mathf.Clamp(Position.y, 0 + SpriteMargin, screenSize_.y - SpriteMargin)
		);
	}
	
	// Check for hero collision while disguised
	private void OnPlayerBodyEntered(Node body)
	{
		// Only worry about hero colliding while in disguise mode.
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		if (animatedSprite.Animation != "disguise") {
			return;
		}
		// Check if |body| is Hero.
		if(body.Name == "Hero") 
		{
			GD.Print("Hero collide while disguised"); // debug print statement to make sure this works
			EmitSignal("Hit");
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		}
	}
	
	// Reset player at game start.
	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

}


