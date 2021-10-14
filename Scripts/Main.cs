using Godot;
using System;

public class Main : Node
{
    private int[] villainLevels = {1, 1, 3};
    private int[] innocentLevels = {0, 1, 6};
    
    // Values for the current level
    // Villains are how many bad guys we spawned
    // VillainsHit are how many we dispatched
    // Innocents are how many people who need to be protected we spawned
    // InnocentsHit are how many we let be dispatched (bad) 
    private int levelNumVillains_ = 0;
    private int levelNumVillainsHit_ = 0;
    private int levelNumInnocents_ = 0;
    private int levelNumInnocentsHit_ = 0;

    // Totals for the game
    private int totalNumVillainsHit_ = 0;
    private int totalNumInnocentsHit_ = 0;

    private int pr_ = 0;

    private int currentLevel_ = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Temporary for testing.
        SetLevel(0);
    }

    private void SetLevel(int level)
    {
        currentLevel_ = level;
        levelNumVillains_ = villainLevels[level];
        levelNumVillainsHit_ = 0;
        levelNumInnocents_ = innocentLevels[level];
        levelNumInnocentsHit_ = 0;

        CalcPR();
        UpdateScore();
    }

    private void CalcPR()
    {
        pr_ = totalNumVillainsHit_ - totalNumInnocentsHit_;
    }

    private void UpdateScore()
    {        
        int remainingVillains = levelNumVillains_ - levelNumVillainsHit_;
        GetNode<HUD>("HUD").SetScore(currentLevel_ + 1, remainingVillains, pr_);
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
