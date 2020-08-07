using Godot;
using System;

public class Concept : Node2D
{
	public string conceptId = "";
	public string conceptType = "";
	public int practioners = 0;
	public string prerequisites = ""; //What state should characters be in to start believing?
	public EventLog myLog;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

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
