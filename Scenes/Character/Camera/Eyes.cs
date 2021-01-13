using Godot;
using System;

public class Eyes : RayCast
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        
    }

    public bool look(Character target) { //May be updated to return a limb count in the future.
        Enabled = true;
        CastTo = target.GlobalTransform.origin - GlobalTransform.origin;
        ForceRaycastUpdate();
        Enabled = false;
        if(IsColliding() && GetCollider() is Character colTarget) {
            if(colTarget.fullName == target.fullName) {
                return true;
            }
        }
        return false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta) {
//      
//  }
}
