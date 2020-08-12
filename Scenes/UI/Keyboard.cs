using Godot;
using System;

public class Keyboard : Panel {      
    public GridContainer keyGrid;
    public override void _Ready(){
        keyGrid = (GridContainer)GetNode("KeyGrid");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
