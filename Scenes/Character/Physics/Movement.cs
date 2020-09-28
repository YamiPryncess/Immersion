using Godot;
using System;

public class Movement : Node {
    Camera fpCamera;
    Character character;
    public override void _Ready() {
        fpCamera = GetNode<Camera>("../CharCam/FirstPerson");
        character = GetParent<Character>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void inputPhysics (float delta) {
        // Vector3 inputDirection = new Vector3(
		// 	Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
		// 	0,
		// 	Input.GetActionStrength("move_back") - Input.GetActionStrength("move_front")
		// );

        Vector3 inputDir = new Vector3();
        Transform transform = fpCamera.GlobalTransform;
        //Bug: Can't walk at usual pace when looking up or down. Walking becomes slow because of the basis.

        //Calculate a move direction vector relative to the camera
	    //The basis stores the (right, up, -forwards) vectors of our camera.
        if (Input.IsActionPressed("move_front")) inputDir -= transform.basis.z;
        if (Input.IsActionPressed("move_back")) inputDir += transform.basis.z;
        if (Input.IsActionPressed("move_left")) inputDir -= transform.basis.x;
        if (Input.IsActionPressed("move_right")) inputDir += transform.basis.x;

        if (inputDir.Length() > 1.0) {
            inputDir = inputDir.Normalized();
        }
        inputDir.y = 0;

        character.animationTree.Set("parameters/move_ground/blend_position", inputDir.Length());

        //Rotation
        /*if (inputDir != Vector3.Zero) {
            Transform targetDirection = Transform.LookingAt(GlobalTransform.origin + inputDir, Vector3.Up);
            Transform = Transform.InterpolateWith(targetDirection, rotSpeed * delta);
        }*/
    
        //Movement
        Vector3 velocityNew = inputDir * character.runSpeed;
        //velocity New??? = velocity.LinearInterpolate(velocityNew, acceleration * delta);
		if(velocityNew.Length() > character.maxSpeed){
			velocityNew = velocityNew.Normalized() * character.maxSpeed;
        }
        if (Input.IsActionJustPressed("jump") && character.IsOnFloor()) {
            velocityNew.y += 5;//Bug: For some reason the jump is exagerrated. Maybe when I add linear interpolate velocity it'll be less sensitive like how it is in tutorials.
        }
		velocityNew.y += character.velocity.y - character.gravity * delta;
        character.velocity = character.MoveAndSlide(velocityNew, Vector3.Up);
    }
}
//Removed Code

// public override void _UnhandledInput(InputEvent @event) {
//         // Called when an Godot.InputEvent hasn't been consumed by Godot.Node._Input(Godot.InputEvent)
//         // or any GUI. The input event propagates up through the node tree until a node consumes it.
        
//         //    It is only called if unhandled input processing is enabled, which is done automatically
//         // if this method is overridden, and can be toggled with Godot.Node.SetProcessUnhandledInput(System.Boolean) .
        
//         //    To consume the input event and stop it propagating further to other nodes,
//         // Godot.SceneTree.SetInputAsHandled can be called.

//         //    For gameplay input, this and Godot.Node._UnhandledKeyInput(Godot.InputEventKey) are 
//         // usually a better fit than Godot.Node._Input(Godot.InputEvent) as they allow
//         // the GUI to intercept the events first.
        
//         // Note: This method is only called if the node is present in the scene tree 
//         // (i.e. if it's not orphan).
//     }