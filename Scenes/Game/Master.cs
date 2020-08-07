using Godot;
using System;

public class Master : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public Character player;
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Confined);
	    //Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public override void _Input(InputEvent inputEvent){
        //if event.is_action_pressed("click"):
        //if Input.get_mouse_mode() != Input.MOUSE_MODE_CAPTURED:
        //		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
        if(inputEvent.IsActionPressed("toggle_mouse_confined")){
            if(Input.GetMouseMode() == Input.MouseMode.Confined){
                Input.SetMouseMode(Input.MouseMode.Visible);
            } else {
                Input.SetMouseMode(Input.MouseMode.Confined);
                GetTree().SetInputAsHandled();
            }
        }

        if (inputEvent.IsActionPressed("toggle_fullscreen")){
            OS.WindowFullscreen = !OS.WindowFullscreen;
            GetTree().SetInputAsHandled();
        }
    }
}