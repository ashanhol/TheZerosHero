using Godot;
using System;

public class NPC : RigidBody2D
{
	[Export]
	public int Speed = 75; // How fast the player will move (pixels/sec).

	private Vector2 _screenSize; // Size of the game window.
	
	private int _travelDistance; //How many pixels left before the granny turns
	
	private Vector2 _velocity;
	
	static private Random _random = new Random();
	
	static private int _minTravelDistance;
	static private int _maxTravelDistance;
	
	private int RandRange(int min, int max)
	{
		return _random.Next(min, max + 1);
	}
	
	private bool isGoingOffScreen()
	{
		bool offScreenLeft = Position.x <= 0 && _velocity.x < 0;
		bool offScreenRight = Position.x >= _screenSize.x && _velocity.x > 0;
		bool offScreenTop = Position.y <= 0 && _velocity.y < 0;
		bool offScreenBottom = Position.y >= _screenSize.y && _velocity.y > 0;
		return offScreenLeft || offScreenRight || offScreenTop || offScreenBottom;
	}
	
	private void ChangeDirection()
	{
		int isTurningClockwise = RandRange(0, 1);
		_velocity = isTurningClockwise == 1? new Vector2(-_velocity.y, _velocity.x) :
											 new Vector2(_velocity.y, -_velocity.x) ;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = new Vector2((int)ProjectSettings.GetSetting("display/window/size/width"), (int)ProjectSettings.GetSetting("display/window/size/height"));
		int startDirection = _random.Next(4);
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
		_minTravelDistance = 10;
		_maxTravelDistance = (int)Math.Min(_screenSize.x, _screenSize.y)/2;
		_travelDistance = _random.Next(_minTravelDistance, _maxTravelDistance);
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		Position += _velocity * delta * Speed;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, _screenSize.x),
			y: Mathf.Clamp(Position.y, 0, _screenSize.y)
			);
		//Subtract distance traveled since last frame
		_travelDistance -= (int)(Speed * delta);
		
		//Once granny has traveled as far as she should,
		//make a 90 degree turn and choose a random 
		//distance to travel in the new direction 
		if (_travelDistance <= 0)
		{
			ChangeDirection();
			_travelDistance = RandRange(_minTravelDistance, _maxTravelDistance);
		}
		
		if (isGoingOffScreen()){
			//turn 180 degrees to go back where you came from
			_velocity = new Vector2(-_velocity.x, -_velocity.y);
			_travelDistance = RandRange(_minTravelDistance, _maxTravelDistance);
		}
	}
	
	
}
