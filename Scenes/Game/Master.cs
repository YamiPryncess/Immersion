using Godot;
using System;
using System.Collections.Generic;

public class Master : Node {
    // Called when the node enters the scene tree for the first time.
    // Our world states
    public Character player;
    public override void _Ready() {//I'm going to use this function as an auto load for now.
        Input.SetMouseMode(Input.MouseMode.Confined);
	    //Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
    }
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