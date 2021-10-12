using Godot;
using System;

public class Player : Area2D
{
	// Variables
	[Export]
	public int Speed = 400;
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
		}
		else if (Input.IsKeyPressed((int)KeyList.C))
		{
			// Change sprite to costume.
			animatedSprite.Animation = "disguise";
		}
		else
		{
			// Default to normal person mode
			animatedSprite.Animation = "plain";
		}
		
		// TODO: add clamp to screen bounds after it's gotten from game controller scene.
		

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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
