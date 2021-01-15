using Godot;
using System;

public class Vision : RayCast {
    
    public float cameraXAngle = 0;
    public Camera fpCamera;
    public Character character;
    public override void _Ready() {
        fpCamera = (Camera)GetNode("FirstPerson");
        character = GetParent<Character>();
        ExcludeParent = true;
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
    public bool look(Character target) { //May be updated to return a limb count in the future.
        CastTo = target.GlobalTransform.origin - GlobalTransform.origin;
        ForceRaycastUpdate();
        if(IsColliding() && GetCollider() is Character colTarget) {
            if(colTarget.fullName == target.fullName) {
                return true;
            }
        }
        return false;
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