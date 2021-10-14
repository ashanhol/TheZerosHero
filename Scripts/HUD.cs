using Godot;
using System;

public class HUD : CanvasLayer
{
	const string c_level = "Level: ";
	const string c_villains = "Villains: ";
	const string c_pr = "PR: ";
	const string c_separator = "    ";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// level is one based
	// villains is number of villains remaining in the level
	// pr is villains hit - innocents hit 
	public void SetScore(int level, int villains, int pr)
	{
		string score = c_level + level + c_separator + c_villains + villains + c_separator + c_pr + pr;
		GetNode<Label>("Score").Text = score;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
