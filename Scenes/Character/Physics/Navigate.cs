using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Navigate : Node {
    public Character character;
    public Navigation nav;
    public List<Position3D> checkPoints;
    public MeshInstance tracker;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    public Vector3 target = Vector3.Zero;
    public Godot.Collections.Array navPath;
    public int pathInd = 0;
    public bool hasPath = false;
    
    public override void _Ready(){    
        character = GetParent<Character>();
        nav = (Navigation)GetParent().GetParent();
        tracker = (MeshInstance)GetNode("Tracker");
        rng.Randomize();
        checkPoints = GetTree().GetNodesInGroup("Checkpoints").OfType<Position3D>().ToList();
    }

    public override void _PhysicsProcess(float delta) {
        if(hasPath == true && pathInd < navPath.Count) {
            navFollow(delta);
        } else {
            hasPath = false;
            character.stateMachine.Travel("idle");
        }
    }

    public void traverse(Vector3 position){
        target = position;
        navPath = new Godot.Collections.Array(nav.GetSimplePath(character.GlobalTransform.origin, target, true));
        pathInd = 0;
        hasPath = true;
        character.stateMachine.Travel("move_ground");
    }

    public void navMesh(float delta, Character.NAVSTATE navState) {
        if(hasPath == false){
            if((target == Vector3.Zero) || (pathInd >= navPath.Count)) {
                int ranNum = rng.RandiRange(0, 3);
                target = checkPoints[ranNum].GlobalTransform.origin;
                navPath = new Godot.Collections.Array(nav.GetSimplePath(character.GlobalTransform.origin, target, true));
                pathInd = 0;
                hasPath = true;
                //Transform.Orthonormalized();
                character.stateMachine.Travel("move_ground");
            }
            //character.stateMachine.Travel("idle");
            //hasPath = false;

            if (navState == Character.NAVSTATE.FOLLOW) {
                navFollow(delta);
            } else if(navState == Character.NAVSTATE.TRACKER) {
                //navTracker(delta);
            }
        }
    }

    public void navFollow(float delta) {
        if(pathInd < navPath.Count) {
            Vector3 pathPoint = (Vector3)navPath[pathInd];
            Vector3 relativePoint = pathPoint;
            relativePoint.y = 0;
            Vector3 origin = character.GlobalTransform.origin;
            Vector3 relativeOrigin = origin;
            relativeOrigin.y = 0;
            Vector3 pathDir = (relativePoint - relativeOrigin).Normalized();

            float distance = relativeOrigin.DistanceTo(relativePoint);
            float frameDistance = character.runSpeed * delta; //How far the npc should've moved this frame
            float newRunSpeed = character.runSpeed; //No delta since MoveAndSlide already uses it.
            if (distance <= 1) {
                pathInd += 1; //Will be processed next frame.
                //if(pathInd == navPath.Count-1) {
                    //newRunSpeed -= distance; //Npc is slowed to not skip over the point.
                //}
            }
            Vector3 newPathDir = pathDir * newRunSpeed; //For some reason the magnitude gets smaller
            //Vector3 newGravity = new Vector3(0, velocity.y - gravity, 0);
            newPathDir.y = character.velocity.y - character.gravity * delta;
            
            //GD.Print("normalized-pathDir: ", pathDir, " gravSpeed-newPathDir: ", newPathDir, " magnitude: ", newPathDir.Length(), " distance: ", distance);
            
            Vector3 pathLook = new Vector3(pathPoint.x, origin.y, pathPoint.z);
            if(origin != pathLook){
                character.LookAt(pathLook, Vector3.Up);
            }
            character.velocity = character.MoveAndSlide(newPathDir, Vector3.Up);// (-0.0004894423, -0.1633433, -5.997704) spdgrv: 5.999928 -> 4FramesLater (-0.0004975397, -0.1633333, -5.997662) spdgrv: 5.999885
            // Vector3 interpolation = yLessOrigin.LinearInterpolate(yLessPathPoint, newRunSpeed * delta);
            // Transform = Transform.LookingAt(interpolation + yLessPathDir, Vector3.Up);

            character.animationTree.Set("parameters/move_ground/blend_position", pathDir.Length());
        }
    }

    /*public void navTracker(float delta) { //The movement section of this code does not work yet.
         if(pathInd < navPath.Count) {   
            Vector3 pathPoint = (Vector3)navPath[pathInd];
            Vector3 origin = character.GlobalTransform.origin;
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
    }*/
}


//Removed Code (And possibly outdated code)

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