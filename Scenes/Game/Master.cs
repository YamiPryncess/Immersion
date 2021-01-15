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

public static class Math {
    public static float[] fieldOfVision(Vector3 observer, Vector3 facing, Vector3 observed) {
            Vector2 eyesHFOV = new Vector2(observer.x, observer.z);
            Vector2 eyesVFOV = new Vector2(observer.z, observer.y);
            
            Vector2 facingH = new Vector2(facing.x, facing.z);
            Vector2 facingV = new Vector2(facing.z, facing.y);
            
            Vector2 observedH = new Vector2(observed.x, observed.z);
            Vector2 observedV = new Vector2(observed.z, observed.y);

            float horizontal = Mathf.Acos((observedH - eyesHFOV).Normalized().Dot(facingH));
            float vertical = Mathf.Acos((observedV - eyesVFOV).Normalized().Dot(facingV));
            
            return new float[]{Mathf.Rad2Deg(horizontal), Mathf.Rad2Deg(vertical)};
    }
}