using Godot;
using System;

public class Main : Node
{
	[Export]
	public PackedScene NPCs;
	[Export]
	public PackedScene Villains;
	
	private int[] villainLevels = {1, 1, 3};
	private int[] innocentLevels = {0, 1, 6};
	
 
	// Villains are how many bad guys are left
	// VillainsHit are how many we dispatched
	// Innocents are how many people who need to be protected left
	// InnocentsHit are how many we let be dispatched (bad) 
	private int levelNumVillains_ = 0;
	private int levelNumInnocents_ = 0;

	private int totalNumVillainsHit_ = 0;
	private int totalNumInnocentsHit_ = 0;

	private int pr_ = 0;

	private int currentLevel_ = 0;
	
	private static Random _random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Temporary for testing.
		SetLevel(0);
		
		CalcPR();
		UpdateScore();
	}

	private void SetLevel(int level)
	{
		currentLevel_ = level;
		levelNumVillains_ = villainLevels[level];
		levelNumInnocents_ = innocentLevels[level];
		
		SpawnCharacters(Villains, levelNumVillains_);
		SpawnCharacters(NPCs, levelNumInnocents_);
	}

	private void CalcPR()
	{
		pr_ = totalNumVillainsHit_ - totalNumInnocentsHit_;
	}

	private void UpdateScore()
	{        
		GetNode<HUD>("HUD").SetScore(currentLevel_ + 1, levelNumVillains_, pr_);
	}

	private void SpawnCharacters(PackedScene objClass, int amount) {
		while (amount --> 0) {
			// Create a single object instance.
			Node2D instance = (Node2D)objClass.Instance();

			// Pick a random location on our path. Set the mob's position.
			PathFollow2D spawnLocation =
				GetNode<PathFollow2D>("OuterPath/SpawnLocation");
			spawnLocation.Offset = _random.Next();
			instance.Position = spawnLocation.Position;
			
			// Finally, add our mob instance.
			AddChild(instance);
		}
	}

	private void HitVillain()
	{
		totalNumVillainsHit_++;
		levelNumVillains_--;
		CalcPR();
		UpdateScore();

		if (levelNumVillains_ <= 0)
		{
			SetLevel(currentLevel_ + 1);
		}

	}

	private void HitInnocent()
	{
		totalNumInnocentsHit_++;
		levelNumInnocents_--;

		CalcPR();
		UpdateScore();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
