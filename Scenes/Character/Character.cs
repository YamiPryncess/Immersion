using Godot;
using System.Collections.Generic;
using System.Linq;

public class Character : KinematicBody
{
    //=============LOCAL VARIABLES================
        // Store our list of actions
    //public List<Action> actions = new List<Action>();
    // Dictionary of subgoals
    //public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    // Our inventory
    //public Inventory inventory = new Inventory();
    // Our beliefs
    //public WorldStates beliefs = new WorldStates();

    // Access the planner
    //GPlanner planner; // Will Use.
    // Action Queue
    //Queue<Action> actionQueue;
    // Our current action
    //public Action currentAction;
    // Our subgoal
    //SubGoal currentGoal;
        
    bool invoked = false;
    //an invoked method to allow an agent to be performing a task
    //for a set location
    public int frame = 0;
    public Master master;
    public Spatial mannequiny;
    public Camera fpCamera;
    public Spatial head;
    public Object inventory;
    public MeshInstance tracker;
    [Export] private string fullName = "LynnCelestine";
    private string savePath;

    public List<Position3D> checkPoints;
    public Navigation nav;
    public AnimationTree animationTree;
    public AnimationNodeStateMachinePlayback stateMachine;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    public Vector3 target = Vector3.Zero;
    public Godot.Collections.Array navPath;
    public int pathInd = 0;
    public float cameraXAngle = 0;
    [Export] public float runSpeed = 5f;
    public float trackSpeedAddend = 1f;
    public float maxSpeed = 22;
    public float acceleration = 4;
    [Export] public float rotSpeed = 10;
    public float gravity = 9.8f;
    public bool jump = false;
    public Vector3 velocity = Vector3.Zero;
    //public float mouseSensitivity = 0.3f;
    public bool isMoving = false;
    public Timer timer;
    public enum PLAYSTATE { NAV, PHYSICS, TEXT }
    [Export] public PLAYSTATE playState = PLAYSTATE.PHYSICS;
    public enum NAVSTATE { TRACKER, FOLLOW }
    [Export] public NAVSTATE navState = NAVSTATE.FOLLOW;

    //=============GODOT VIRTUALS================
    public override void _Ready()
    {
        master = GetTree().Root.GetNode<Master>("Master");
        mannequiny = (Spatial)GetNode("Mannequiny");
        fpCamera = (Camera)GetNode("Camera/FirstPerson");
        head = (Spatial)GetNode("Camera");
        animationTree = (AnimationTree)GetNode("Mannequiny/AnimationTree");
        stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
	    timer = (Timer)GetNode("Timer");
        initInventory();
        initNavMesh();

        if(playState == PLAYSTATE.PHYSICS && master.player == null) {
            master.player = this;
        }
        
        //Action[] acts = this.Get<Action>();
        //foreach (GAction a in acts) {

          //  actions.Add(a);
        //}
    }

    public override void _Process(float delta){
        if(playState == PLAYSTATE.PHYSICS){
            camera(delta);
        }
    }
    
    public override void _PhysicsProcess(float delta){ 
        frame++;
        if(frame > 30){
            frame = 0;
        }
        if (playState == PLAYSTATE.NAV) {
            navMesh(delta);
        } else if (playState == PLAYSTATE.PHYSICS) {
            inputPhysics(delta);
        } else {
            MoveAndSlide(new Vector3(0, -gravity*delta, 0));
        }
    }

        /*public override void _Input(InputEvent inputEvent) {
        //if(playState != PLAYSTATE.PHYSICS){
            //return;
        //}
        if(inputEvent is InputEventMouseMotion inputEventMotion) {
            GD.Print(inputEventMotion.Relative);
            RotateY(Mathf.Deg2Rad(-inputEventMotion.Relative.x * mouseSensitivity));
            float change = -inputEventMotion.Relative.y * mouseSensitivity;
            if(change + cameraXAngle < 90 && change + cameraXAngle > -90){
                head.RotateX(Mathf.Deg2Rad(change));
                cameraXAngle += change;
            }
        }
    }*/
    //=============SIGNALS================
    public void _on_Inventory_changed(Object inventory){
        ResourceSaver.Save(savePath, (Resource)inventory);
    }

    public void _on_Timer_timeout(){
        isMoving = false;
    }
    //=============INITIALIZATION FUNCTIONS================
    public void initNavMesh() {
        nav = (Navigation)GetParent();
        tracker = (MeshInstance)GetNode("Tracker");
        rng.Randomize();
        checkPoints = GetTree().GetNodesInGroup("Checkpoints").OfType<Position3D>().ToList();
    }
    public void initInventory() {
        savePath = "user://" + fullName + "Inventory.tres";
        GDScript invResource = (GDScript) GD.Load("res://Scenes/Character/Features/Inventory.gd");
        inventory = (Godot.Object) invResource.New(); // This is a Godot.Object

        if(inventory == null) GD.Print("No Inventory for NPC");
        
        inventory.Connect("inventory_changed", this, nameof(_on_Inventory_changed));
        
        bool invExists = ResourceLoader.Exists(savePath);

        if (invExists){
            Resource existingInv = (Resource) GD.Load(savePath);
            inventory.Call("set_items", existingInv.Call("get_items"));
            GD.Print(inventory, existingInv, "Existing Inventory loaded");
        } else {
            GD.Print(inventory, "New Inventory created");
        }
    }

    //=============UPDATE FUNCTIONS================

    //-------Camera---------
    public void camera(float delta){
        Vector2 arrowKeys = new Vector2(Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
        Input.GetActionStrength("look_up") - Input.GetActionStrength("look_down")).Normalized();
        RotateY(Mathf.Deg2Rad(-arrowKeys.x * 3));
        float change = arrowKeys.y * 3;
        if(change + cameraXAngle < 90 && change + cameraXAngle > -90){
            head.RotateX(Mathf.Deg2Rad(change));
            cameraXAngle += change;
        }
    }
    
    //-------Movement--------
    public void navMesh(float delta) {
        if(((target == Vector3.Zero) || (pathInd >= navPath.Count))) {
            int ranNum = rng.RandiRange(0, 3);
            target = checkPoints[ranNum].GlobalTransform.origin;
            navPath = new Godot.Collections.Array(nav.GetSimplePath(GlobalTransform.origin, target, true));
            pathInd = 0;
            //Transform.Orthonormalized();
            stateMachine.Travel("move_ground");
        } else if (pathInd >= navPath.Count){
            stateMachine.Travel("idle");
        }

        if (navState == NAVSTATE.FOLLOW) {
            navFollow(delta);
        } else if(navState == NAVSTATE.TRACKER) {
            navTracker(delta);
        }
    }

    public void navTracker(float delta) { //The movement section of this code does not work yet.
         if(pathInd < navPath.Count) {   
            Vector3 pathPoint = (Vector3)navPath[pathInd];
            Vector3 origin = GlobalTransform.origin;
            Vector3 trackerOrigin = tracker.GlobalTransform.origin;
            Vector3 agentTargetDir = (target - origin).Normalized();
            Vector3 agentPathDir = (pathPoint - origin).Normalized();
            Vector3 agentTrackerDir = (trackerOrigin - origin).Normalized();
            Vector3 pathTrackerDir = (pathPoint - trackerOrigin).Normalized();
            Vector3 yLessPTDir = pathTrackerDir;
            yLessPTDir.y = 0;

            Vector3 relativePoint = pathPoint; //Height Agnostic, So if the path was on a 
            relativePoint.y = trackerOrigin.y; //different floor it'd still check.
            float trackerDistance = trackerOrigin.DistanceTo(relativePoint);
            float trackerFrameDistance = (runSpeed + trackSpeedAddend) * delta; //How far the tracker should've moved this frame
            relativePoint.y = origin.y;
            float agentDistance = origin.DistanceTo(relativePoint);
            float frameDistance = runSpeed * delta; //How far the tracker should've moved this frame
            float newRunSpeed = runSpeed; //No delta since MoveAndSlide already uses it.
            if (trackerDistance <= trackerFrameDistance || agentDistance <= frameDistance) {
                pathInd += 1; //Will be processed next frame.
                if(pathInd == navPath.Count-1) {
                    newRunSpeed -= trackerDistance;
                }   //Tracker & Agent are slowed to not skip over the point.
            }
            Vector3 newTrackerDir = agentTrackerDir * newRunSpeed;
            newTrackerDir.y = velocity.y - gravity * delta;
            
            //GD.Print(newTrackerDir, " spdgrv: ", newTrackerDir.Length());
            float agentTrackerDistance = origin.DistanceTo(trackerOrigin); //You may also want to check if the tracker went behind a wall/sharp corner.
            if(agentTrackerDistance < 4){
                //Tracker Speed ust be greater than Player Speed &
                //also be long enough to not shorten the move vector's length
                //when it's affected by gravity. I believe that's what's causing
                //the lost of length in my navApproxSpeed function.
                //If tracker is to long the agent will turn too early on corners also.
                //To prevent this the agent could pursue a point between the path and the tracker
                //but only after the tracker met the goal themselves. -This has not been implemented.
                
                Vector3 pathLook = new Vector3(pathPoint.x, trackerOrigin.y, pathPoint.z);
                //tracker.LookAt(pathLook, Vector3.Up);//Agent looks at the tracker this time.
                //tracker.Translate(pathLook);
                //Transform transform = tracker.GlobalTransform;
                //transform.origin.z -= newRunSpeed + trackSpeedAddend;
                //tracker.GlobalTransform = transform;
                //Vector3 interpolate = trackerOrigin.LinearInterpolate(pathPoint, (newRunSpeed + trackSpeedAddend));
                //Transform trackerTransform = new Transform();
                //trackerTransform.origin = interpolate;
                //tracker.Transform = trackerTransform; 
                //tracker.Transform = tracker.Transform.LookingAt(interpolate + yLessPTDir, Vector3.Up); //Interpolate plus direction?
                // tracker.Translation = new Vector3(
                //     tracker.Translation.x + .0001f,
                //     tracker.Translation.y,
                //     tracker.Translation.z + .0001f
                // );
            }

            Vector3 trackerLook = new Vector3(trackerOrigin.x, origin.y, trackerOrigin.z);
            LookAt(trackerLook, Vector3.Up);//Agent looks at the tracker this time.
            velocity = MoveAndSlide(newTrackerDir, Vector3.Up); //(0, -0.1633333, -5.970223) spdgrv: 5.972457
            //I think move and slide needs velocity to slide past walls
            //but that wouldn't look like realistic behaviour unless
            //ythe collider as bigger than the player.
            
            // Vector3 interpolation = yLessOrigin.LinearInterpolate(yLessPathPoint, newRunSpeed * delta);
            // Transform = Transform.LookingAt(interpolation + yLessPathDir, Vector3.Up);
         }
    }

    public void navFollow(float delta) {
        if(pathInd < navPath.Count) {
            Vector3 pathPoint = (Vector3)navPath[pathInd];
            Vector3 relativePoint = pathPoint;
            relativePoint.y = 0;
            Vector3 origin = GlobalTransform.origin;
            Vector3 relativeOrigin = origin;
            relativeOrigin.y = 0;
            Vector3 pathDir = (relativePoint - relativeOrigin).Normalized();

            float distance = relativeOrigin.DistanceTo(relativePoint);
            float frameDistance = runSpeed * delta; //How far the npc should've moved this frame
            float newRunSpeed = runSpeed; //No delta since MoveAndSlide already uses it.
            if (distance <= 1) {
                pathInd += 1; //Will be processed next frame.
                //if(pathInd == navPath.Count-1) {
                    //newRunSpeed -= distance; //Npc is slowed to not skip over the point.
                //}
            }
            Vector3 newPathDir = pathDir * newRunSpeed; //For some reason the magnitude gets smaller
            //Vector3 newGravity = new Vector3(0, velocity.y - gravity, 0);
            newPathDir.y = velocity.y - gravity * delta;
            
            //GD.Print("normalized-pathDir: ", pathDir, " gravSpeed-newPathDir: ", newPathDir, " magnitude: ", newPathDir.Length(), " distance: ", distance);
            
            Vector3 pathLook = new Vector3(pathPoint.x, origin.y, pathPoint.z);
            if(origin != pathLook){
                LookAt(pathLook, Vector3.Up);
            }
            velocity = MoveAndSlide(newPathDir, Vector3.Up);// (-0.0004894423, -0.1633433, -5.997704) spdgrv: 5.999928 -> 4FramesLater (-0.0004975397, -0.1633333, -5.997662) spdgrv: 5.999885
            // Vector3 interpolation = yLessOrigin.LinearInterpolate(yLessPathPoint, newRunSpeed * delta);
            // Transform = Transform.LookingAt(interpolation + yLessPathDir, Vector3.Up);

            animationTree.Set("parameters/move_ground/blend_position", pathDir.Length());
        }
    }

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

        animationTree.Set("parameters/move_ground/blend_position", inputDir.Length());

        //Rotation
        /*if (inputDir != Vector3.Zero) {
            Transform targetDirection = Transform.LookingAt(GlobalTransform.origin + inputDir, Vector3.Up);
            Transform = Transform.InterpolateWith(targetDirection, rotSpeed * delta);
        }*/
    
        //Movement
        Vector3 velocityNew = inputDir * runSpeed;
        //velocity New??? = velocity.LinearInterpolate(velocityNew, acceleration * delta);
		if(velocityNew.Length() > maxSpeed){
			velocityNew = velocityNew.Normalized() * maxSpeed;
        }
        if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
            velocityNew.y += 5;//Bug: For some reason the jump is exagerrated. Maybe when I add linear interpolate velocity it'll be less sensitive like how it is in tutorials.
        }
		velocityNew.y += velocity.y - gravity * delta;
        velocity = MoveAndSlide(velocityNew, Vector3.Up);
    }

    //---------GOAP----------

    public void CompleteAction() {

        //currentAction.running = false;
        //currentAction.PostPerform();
        //invoked = false;
    }

    public void goap(float delta) {
        
        // Is supposed to be a Late Update.
        //         //if there's a current action and it is still running
        // if (currentAction != null && currentAction.running) {

        //     // Find the distance to the target
        //     float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
        //     // Check the agent has a goal and has reached that goal
        //     if (currentAction.agent.hasPath && distanceToTarget < 2.0f) { // currentAction.agent.remainingDistance < 1.0f) 

        //         if (!invoked) {

        //             //if the action movement is complete wait
        //             //a certain duration for it to be completed
        //             Invoke("CompleteAction", currentAction.duration);
        //             invoked = true;
        //         }
        //     }
        //     return;
        // }

        // // Check we have a planner and an actionQueue
        // if (planner == null || actionQueue == null) {

        //     // If planner is null then create a new one
        //     planner = new GPlanner();

        //     // Sort the goals in descending order and store them in sortedGoals
        //     var sortedGoals = from entry in goals orderby entry.Value descending select entry;

        //     //look through each goal to find one that has an achievable plan
        //     foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {

        //         actionQueue = planner.plan(actions, sg.Key.sGoals, beliefs);
        //         // If actionQueue is not = null then we must have a plan
        //         if (actionQueue != null) {

        //             // Set the current goal
        //             currentGoal = sg.Key;
        //             break;
        //         }
        //     }
        // }

        // // Have we an actionQueue
        // if (actionQueue != null && actionQueue.Count == 0) {

        //     // Check if currentGoal is removable
        //     if (currentGoal.remove) {

        //         // Remove it
        //         goals.Remove(currentGoal);
        //     }
        //     // Set planner = null so it will trigger a new one
        //     planner = null;
        // }

        // // Do we still have actions
        // if (actionQueue != null && actionQueue.Count > 0) {

        //     // Remove the top action of the queue and put it in currentAction
        //     currentAction = actionQueue.Dequeue();

        //     if (currentAction.PrePerform()) {

        //         // Get our current object
        //         if (currentAction.target == null && currentAction.targetTag != "") {

        //             currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
        //         }

        //         if (currentAction.target != null) {

        //             // Activate the current action
        //             currentAction.running = true;
        //             // Pass Unities AI the destination for the agent
        //             currentAction.agent.SetDestination(currentAction.target.transform.position);
        //         }
        //     } else {

        //         // Force a new plan
        //         actionQueue = null;
        //     }
        // }
    }
}
/*public class SubGoal {

    // Dictionary to store our goals
    public Dictionary<string, int> sGoals;
    // Bool to store if goal should be removed after it has been achieved
    public bool remove;

    // Constructor
    public SubGoal(string s, int i, bool r) {

        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}*/

//Removed Code

//Gravity Fix attempts
//When gravity is added. I think it's because 
            //If I comment out gravity but leave the divisor code this error happens at the start of a second target path array I think:
            //E 0:00:19.642   instance_set_transform: Condition "Math::is_nan(v.x)" is true.
  //<C++ Source>  servers/visual/visual_server_scene.cpp:601 @ instance_set_transform()
            //newPathDir = newPathDir.MoveToward(newGravity, delta);
            /*if(newPathDir.Length() < newRunSpeed) { //as the pathPoints get closer The slope gets more
                float gravityDivisor = newRunSpeed / newPathDir.Length(); //influenced by gravity so
                float remainder = newRunSpeed % newPathDir.Length(); //multiplying it by speed doesn't
                newPathDir.x = newPathDir.x * gravityDivisor; //doesn't affect Length() like it would 
                newPathDir.z = newPathDir.z * gravityDivisor; //original slope. The character pauses.
                //Giving it remainder makes it glitchy when it goes over run speed.
                //So no remainder.
            }*/

//Rotation Attempts

            //Transform turnTransform = Transform.LookingAt(yLessOrigin + yLessPathDir, Vector3.Up);
            //turnTransform = turnTransform.Orthonormalized();
            //Transform = Transform.InterpolateWith(turnTransform, rotSpeed * delta);


//Vector3 interpolation = GlobalTransform.origin.LinearInterpolate(relativePoint, newRotSpeed);
                //Transform = Transform.LookingAt(interpolation + yLessPathDir, Vector3.Up);
                //RotateY(Mathf.Deg2Rad(GlobalTransform.basis.z.Dot(pathDir) * 0.3f));
                // Vector3 yLessVelocity = velocity;
                // yLessVelocity.y = 0;
                // Vector3 interpolation = yLessVelocity.LinearInterpolate(yLessPathDir * newRunSpeed, newRunSpeed * delta);
                // float angle = Mathf.Atan2(interpolation.x, interpolation.z);
                // Vector3 myRotate = Rotate;
                // myRotate.y = angle;
                // Rotate
                
                //RotateY(velocity);
                //LookAt(pathPoint, Vector3.Up);
                // Transform rotate = Transform.LookingAt(yLessPathDir, Vector3.Up);
		        // Transform = rotate.InterpolateWith(rotate, newRotSpeed);

//float pathSlope = newPathDir.x/newPathDir.z;
//lerpVelocity = Mathf.Lerp(lerpVelocity, pathSlope, delta * acceleration);
//Vector3 lerpedVector = new Vector3();
//mannequiny.Rotation.y = lerp_angle($Mesh.rotation.y, $Camroot/h.rotation.y, delta * angular_acceleration)

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

    /*      //Basis Transform Reset and Linear Interpolate attempt.
    if(isMoving == true) {                
                Transform transform = Transform;
                transform.basis = Basis.Identity;
                Transform = transform;

                Vector3 yLessTargetDir = targetDir;
                yLessTargetDir.y = 0;  
                //Vector3 interpolation = GlobalTransform.origin.LinearInterpolate(pathDir, newRotSpeed);
                //interpolation.y = 0;
                Transform = LookAt(pathDir, Vector3.Up);
            }
            //Newer but useless except with extra features like timer.
    		// timer.WaitTime = 5f;
            // timer.OneShot = true;
            // timer.Start();
            //Transform targetDir = Transform.LookingAt(pathVec.Normalized(), Vector3.Up);

            //Transform = Transform.InterpolateWith(targetDir, rotSpeed * delta);
            //Transform = Transform.LookingAt(pathVec, Vector3.Up);
            //Linear in that the speed never changes each frame. n + speed.
            // if(IsOnFloor() == false){
            //     gravityForce.y -= gravity;
            //     linearVec.y = gravityForce.y;//Makes it a constant n * -gravity vector in y.
            // }

            //gravityForce.y = 0;
            //Vector3 fVelocity = (prevOrigin - GlobalTransform.origin) * delta;
            //Rotate(Vector3.Up, GlobalTransform.origin.AngleTo(velocity));
            //Vector3 interpolation = GlobalTransform.origin.LinearInterpolate(pathVec, delta * rotSpeed);
            //Transform = Transform.LookingAt(interpolation + dirVec.Normalized(), Vector3.Up);
            //Transform.origin.DirectionTo(target);
            //Transform = Transform.InterpolateWith(rotary, rotSpeed * delta);
            //GD.Print(rotary);
    //Oldest
                MoveAndSlide(moveVec.Normalized() * runSpeed, Vector3.Up);
                if(gravityForce.y > 0) {//I'll change GetFloorVelocity and this no rotate in air if statement logic later.
                        gravityForce.y = 0;//I did this because I want to wait a frame before turning since I don't want them to turn the air velocity. Maybe it would only calculate floor velocity though. 
                } else if (isMoving) { //Makes it so that the player can't turn in air.
                    Vector3 fVelocity = GetFloorVelocity(); //This method requires my chara to be on the floor.
                    float angle = -Mathf.Atan2(fVelocity.z, fVelocity.x);
                    RotateY(angle);
                }*/