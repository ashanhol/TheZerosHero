using Godot;
using System;

public class NPC : Area2D
{
	[Export]
	public int Speed = 30; // How fast the player will move (pixels/sec).
	
	[Signal]
	public delegate void Hit(bool isVillain);

	private Vector2 _screenSize; // Size of the game window.
	
	private float _travelDistance; //How many pixels left before the granny turns
	
	private Vector2 _velocity;
	
	static private Random _random = new Random();
	
	
	static private int _minTravelDistance;
	static private int _maxTravelDistance;
	
	AudioStreamPlayer2D _ohMySound;
	AudioStreamPlayer2D _grannyNoSound;
	
	private int RandRange(int min, int max)
	{
		return _random.Next(min, max + 1);
	}
	
	// returns true if next pixel moved in current direction will put NPC outside
	// the screen bounds
	private bool isGoingOffScreen()
	{
		bool offScreenLeft = Position.x <= 0 && _velocity.x < 0;
		bool offScreenRight = Position.x >= _screenSize.x && _velocity.x > 0;
		bool offScreenTop = Position.y <= 0 && _velocity.y < 0;
		bool offScreenBottom = Position.y >= _screenSize.y && _velocity.y > 0;
		return offScreenLeft || offScreenRight || offScreenTop || offScreenBottom;
	}
	
	private void ChangeDirection90Degrees()
	{
		int isTurningClockwise = RandRange(0, 1);
		_velocity = isTurningClockwise == 1? new Vector2(-_velocity.y, _velocity.x) :
											 new Vector2(_velocity.y, -_velocity.x) ;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ohMySound = GetNode<AudioStreamPlayer2D>("GrannyOhMy");
		_grannyNoSound = GetNode<AudioStreamPlayer2D>("GrannyNo");
		Show();
		
		_screenSize = new Vector2((int)ProjectSettings.GetSetting("display/window/size/width"), (int)ProjectSettings.GetSetting("display/window/size/height"));
		
		int startDirection = RandRange(0, 3);
		switch (startDirection)
		{
			// randomly choose what direction to start in
			case 0: _velocity = new Vector2(1, 0);
				break;
			case 1: _velocity = new Vector2(0, 1);
				break;
			case 2: _velocity = new Vector2(-1, 0);
				break;
			case 3: _velocity = new Vector2(0, -1);
				break;
			default:
				_velocity = new Vector2(1, 1);
				break;
		}
		_velocity = _velocity.Normalized();
		
		// I made up these numbers so she wouldn't turn too often or 
		// walk straight for way too long, idk if they're ideal
		_minTravelDistance = 10;
		_maxTravelDistance = (int)Math.Min(_screenSize.x, _screenSize.y)/2;
		_travelDistance = RandRange(_minTravelDistance, _maxTravelDistance);
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		//Move 
		Position += _velocity * delta * Speed;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, _screenSize.x),
			y: Mathf.Clamp(Position.y, 0, _screenSize.y)
			);
		
		//Subtract distance traveled since last frame from 
		//how far to travel before NPC turns
		_travelDistance -= (Speed * delta);
		
		//If NPC has traveled as far as it should,
		//make a 90 degree turn and choose a random 
		//distance to travel in the new direction 
		if (_travelDistance <= 0)
		{
			ChangeDirection90Degrees();
			_travelDistance = RandRange(_minTravelDistance, _maxTravelDistance);
		}
		
		if (isGoingOffScreen()){
			//turn NPC 180 degrees to go back where it came from
			_velocity = new Vector2(-_velocity.x, -_velocity.y);
			_travelDistance = RandRange(_minTravelDistance, _maxTravelDistance);
		}
	}

	
	private void OnNPCBodyEntered(Node body)
	{
		//We only care about collisions with the hero
		if (body.Name == "Hero")
		{
			if(!_grannyNoSound.Playing)
			{
				_grannyNoSound.Play();
			}
			
			EmitSignal("Hit", false);
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
			Hide();
		}
	}
	
	
	
}
