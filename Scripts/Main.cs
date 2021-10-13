using Godot;
using System;

public class Main : Node
{
    // Totals for the game
    // Villains are how many bad guys we spawned
    // VillainsHit are how many we dispatched
    // Innocents are how many people who need to be protected we spawned
    // InnocentsHit are how many we let be dispatched (bad) 
    public int totalNumVillains_ = 0;
    public int totalNumVillainsHit_ = 0;
    public int totalNumInnocents_ = 0;
    public int totalNumInnocentsHit_ = 0;

    // Values for the current level
    public int levelNumVillains_ = 0;
    public int levelNumVillainsHit_ = 0;
    public int levelNumInnocents_ = 0;
    public int levelNumInnocentsHit_ = 0;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
