using Godot;
using System;

public class Main : Node
{
	[Export]
	public PackedScene NPCs;
	[Export]
	public PackedScene Villains;
	
	const string prologue0 = "When I was a kid, what I wanted more than anything in the world was to be a superhero.";
	const string prologue1 = "That dream got squashed when my mom pointed out that I don’t have any superpowers.";
	const string prologue2 = "When I was a teenager, what I wanted more than anything in the world was to be a superhero’s sidekick.";
	const string prologue3 = "That dream got squashed when I learned that only orphans and circus performers can be sidekicks.";
	const string prologue4 = "In college, I tried to be a scientific genius who would invent things for superheroes.";
	const string prologue5 = "But I failed my physics class because I spent too much time writing on the internet about how great superheroes are.";
	const string prologue6 = "That did lead to my actual career, though. I’ve become a publicist for superheroes.";
	const string prologue7 = "My first job is for a new hero named Captain Hammer.";
	const string prologue8 = "The trouble is, Captain Hammer is an idiot…";

	const string boss0 = "Your boss says: Make sure that hero stops the villains and doesn't hurt the grannies!";
	const string bossPr0 = "Your boss yells: You dolt! It's like the hero hasn't done anything. Are you sleeping on the job?";
	const string bossPrNeg = "Your boss screams: You idiot! You're doing more harm than good! I should fire you!";
	const string bossPrPos = "Your boss says: Not bad. Imagine what you could do if you were really trying...";

	private int[] villainLevels = {1, 1, 3, 5};
	private int[] innocentLevels = {0, 1, 6, 10};

	const int extraVillainsPerLevelAfterMax = 2;
	const int extraInnocentsPerLevelAfterMax = 4;

	private string[] prologueStrings = {prologue0, prologue1, prologue2, prologue3, prologue4, prologue5, prologue6, prologue7, prologue8};	
 
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

	private bool startEarly_ = false;
	
	private static Random _random = new Random();
	
	// FOR TESTING
	[Export]
	public bool CameraFollowsPlayerInsteadOfHero = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCameraLimits();
		Prologue();
	}

	async private void Prologue()
	{
		var messageTimer = GetNode<Timer>("MessageTimer");
		var hud = GetNode<HUD>("HUD");

		// This hides the score
		hud.SetScore(-1, 0, 0);
		
		hud.ShowButton();		

		for (int i = 0; i < prologueStrings.Length; i++)
		{
			if (startEarly_)
			{
				startEarly_ = false;
				break;
			}

			hud.SetMessage(prologueStrings[i]);
			messageTimer.Start();
			await ToSignal(messageTimer, "timeout");
		}
		
		hud.SetMessage("");

		hud.HideButton();

		StartGame();
	}

	private void StartPressed()
	{
		startEarly_ = true;	
		var messageTimer = GetNode<Timer>("MessageTimer");
		var waitTime = messageTimer.GetWaitTime();
		messageTimer.Stop();
		messageTimer.SetWaitTime(0.01f);
		messageTimer.Start();
		messageTimer.SetWaitTime(waitTime);
	}

	private void StartGame()
	{
		SetLevel(0);
		
		CalcPR();
		UpdateScore();		
	}

	async private void SetLevel(int level)
	{		
		var messageTimer = GetNode<Timer>("MessageTimer");
		var hud = GetNode<HUD>("HUD");

		currentLevel_ = level;

		string message = "";

		if (level == 0)
		{
			message = boss0;
		}
		else if (pr_ == 0)
		{
			message = bossPr0;
		}
		else if (pr_ > 0)
		{
			message = bossPrPos;
		}
		else //if (pr_ < 0)
		{
			message = bossPrNeg;
		}


		hud.SetMessage(message);
		messageTimer.Start();
		await ToSignal(messageTimer, "timeout");
		hud.SetMessage("");

		int extraVillains = 0;
		int extraInnocents = 0;

		if (level >= villainLevels.Length)
		{
			level = villainLevels.Length - 1;
			int extraLevels = currentLevel_ - level;
			
			extraVillains = extraLevels * extraVillainsPerLevelAfterMax;
			extraInnocents = extraLevels * extraInnocentsPerLevelAfterMax;
		}

		levelNumVillains_ = villainLevels[level] + extraVillains;
		levelNumInnocents_ = innocentLevels[level] + extraInnocents;
		
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
		GetNode<Hero>("Hero").WhoWeMovingTowards = null;

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
		GetNode<Hero>("Hero").WhoWeMovingTowards = null;
	}
	
	private void SetCameraLimits() {
		// Get the gameplay bounds from our texture size.
		TextureRect texture = GetNode<TextureRect>("TownCenter");
		Vector2 limits = texture.RectSize * texture.RectScale;
		
		// Move the camera to the player if we need to debug.
		Camera2D camera2D = GetNode<Camera2D>($"Hero/Camera2D");
		if (CameraFollowsPlayerInsteadOfHero) {
			GetNode<Node>("Hero").RemoveChild(camera2D);
			GetNode<Node>("Player").AddChild(camera2D);
		}
		
		// Don't let our camera exceed our texture bounds.
		camera2D.LimitLeft = 0;
		camera2D.LimitTop = 0;
		camera2D.LimitRight = (int)limits.x;
		camera2D.LimitBottom = (int)limits.y;
	}

	private void OnMessageTimerTimeout()
	{
		
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


