using Godot;
using System;

public class Checkpoint : Node2D {
    public enum atmospheres {Party, Spooky, Beautiful};
	public string checkpointId = "";
	public string checkpointType = "";
	public string location = "";
	public string atmosphere = "";
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
