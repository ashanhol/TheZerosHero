using Godot;
using System;

public class Player : Area2D
{
	// Signals
	[Signal]
	public delegate void PlayerHit(); // Will be sent to trigger penalty when players hit in disguise.
	[Signal]
	public delegate void Yell(); // Fired when we're in yell mode.

	public bool IsDisguised { get; set; }
	
	// Variables
	public int Speed = 300; // No export since we're using it in multiple places in the code.
	[Export]
	public int SpriteMargin = 25; // Margin to help with clamping so player stays on screen.
	
	private Vector2 screenSize_;
	private bool IsStunned = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Don't let our player exceed our texture bounds.
		TextureRect texture = GetParent().GetNode<TextureRect>("TownCenter");
		screenSize_ = texture.RectSize * texture.RectScale;
	}
	
	public override void _Process(float delta) 
	{
		if(!IsStunned) 
		{
			var velocity = new Vector2();
			var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
			animatedSprite.Play();

			// Determine input.		
			if(Input.IsActionPressed("ui_right"))
			{
				velocity.x += 1;
			}	
			if(Input.IsActionPressed("ui_left")) 
			{
				velocity.x -= 1;
			}
			if (Input.IsActionPressed("ui_down"))
			{
				velocity.y += 1;
			}
			if (Input.IsActionPressed("ui_up"))
			{
				velocity.y -= 1;
			}
			
			// Can't yell if we're in disguise
			if (animatedSprite.Animation != "disguise" && 
				Input.IsKeyPressed((int)KeyList.D) || Input.IsKeyPressed((int)KeyList.Shift))
			{
				// Change sprite to yell mode.
				animatedSprite.Animation = "yell";
				var yoohoo = GetNode<AudioStreamPlayer2D>("Yoohoo");
				if(!yoohoo.IsPlaying()){
					yoohoo.Play();
					EmitSignal("Yell");			
				}
				Speed = 150; // halve speed			
			}
			else if (Input.IsKeyPressed((int)KeyList.C) || Input.IsKeyPressed((int)KeyList.Control))
			{
				// Change sprite to costume.
				animatedSprite.Animation = "disguise";
				IsDisguised = true;
				Speed = 350;			
			}
			else
			{
				// Check to see if we changed from disguise.
				if(animatedSprite.Animation == "disguise")
				{
					IsDisguised = false;
					Speed = 300;
				}
				// Check to see if we stopped defending.
				else if(animatedSprite.Animation == "yell") 
				{
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
			EmitSignal("PlayerHit");
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
			GetNode<Timer>("StunTimer").Start(); // start stun timer
			IsStunned = true;
			animatedSprite.Animation = "plain";
		}
	}
	
	private void OnStunTimerTimeout()
	{
		// Unstun the player to move again
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);		
		IsStunned = false;
	}
	
	// Reset player at game start.
	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

}
