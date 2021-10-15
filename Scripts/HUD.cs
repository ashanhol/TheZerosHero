using Godot;
using System;

public class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartPressed();

	const string c_level = "Level: ";
	const string c_villains = "Villains: ";
	const string c_pr = "PR: ";
	const string c_separator = "    ";

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// level is one based. Negative values hide the score.
	// villains is number of villains remaining in the level
	// pr is villains hit - innocents hit 
	public void SetScore(int level, int villains, int pr)
	{
		string score = "";

		if (level >= 0)
		{
			score = c_level + level + c_separator + c_villains + villains + c_separator + c_pr + pr;	
		}
		 
		GetNode<Label>("Score").Text = score;
	}

	public void SetMessage(string text)
	{
		GetNode<Label>("Message").Text = text;
	}

	public void ShowButton()
	{
		GetNode<Button>("StartButton").Show();
	}

	public void HideButton()
	{
		GetNode<Button>("StartButton").Hide();
	}

	private void OnStartButtonPressed()
	{
		EmitSignal("StartPressed");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


