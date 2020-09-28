using Godot;
using System;

public class CharCam : Spatial {
    
    public float cameraXAngle = 0;
    public Camera fpCamera;
    public Character character;
    public override void _Ready() {
        fpCamera = (Camera)GetNode("FirstPerson");
        character = GetParent<Character>();
    }
    public void camera(float delta){
        Vector2 arrowKeys = new Vector2(Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
        Input.GetActionStrength("look_up") - Input.GetActionStrength("look_down")).Normalized();
        character.RotateY(Mathf.Deg2Rad(-arrowKeys.x * 3));
        float change = arrowKeys.y * 3;
        if(change + cameraXAngle < 90 && change + cameraXAngle > -90){
            RotateX(Mathf.Deg2Rad(change));
            cameraXAngle += change;
        }
    }

}
//Temporarily Removed Code (Old Code for mouse rotation)

    /*public override void _Input(InputEvent inputEvent) {
        //if(playState != PLAYSTATE.PHYSICS){
            //return;
        //}
        if(inputEvent is InputEventMouseMotion inputEventMotion) {
            GD.Print(inputEventMotion.Relative);
            character.RotateY(Mathf.Deg2Rad(-inputEventMotion.Relative.x * mouseSensitivity));
            float change = -inputEventMotion.Relative.y * mouseSensitivity;
            if(change + cameraXAngle < 90 && change + cameraXAngle > -90){
                RotateX(Mathf.Deg2Rad(change));
                cameraXAngle += change;
            }
        }
    }*/