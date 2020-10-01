using Godot;
using System.Collections.Generic;
using System.Linq;

public class Character : KinematicBody {
    //=============LOCAL VARIABLES================
    [Signal]
    public delegate void InventoryChanged();
    public int frame = 0;
    public Master master;
    public Spatial mannequiny;
    public Dictionary<string, ItemCore> inventory = new Dictionary<string, ItemCore>();
    [Export] public string fullName = "LynnCelestine";
    public AnimationTree animationTree;
    public AnimationNodeStateMachinePlayback stateMachine;
    public CharCam camera;
    public Movement movement;
    public Navigate navigate;
    public GAgent GAgent;
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
    public enum PLAYSTATE { NAV, PHYSICS, TEXT }
    [Export] public PLAYSTATE playState = PLAYSTATE.PHYSICS;
    public enum NAVSTATE { TRACKER, FOLLOW }
    [Export] public NAVSTATE navState = NAVSTATE.FOLLOW;

    //=============GODOT VIRTUALS================
    public override void _Ready() {
        master = GetTree().Root.GetNode<Master>("Master");
        camera = GetNode<CharCam>("CharCam");
        movement = GetNode<Movement>("Movement");
        navigate = GetNode<Navigate>("Navigate");
        GAgent = GetNode<GAgent>("GAgent");
        mannequiny = (Spatial)GetNode("Mannequiny");animationTree = (AnimationTree)GetNode("Mannequiny/AnimationTree");
        stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        if(playState == PLAYSTATE.PHYSICS && master.player == null) {
            master.player = this;
        }
    }

    public override void _Process(float delta){
        if(playState == PLAYSTATE.PHYSICS){
            camera.camera(delta);
        }
    }
    
    public override void _PhysicsProcess(float delta){ 
        frame++;
        if(frame > 30){
            frame = 0;
        }
        if (playState == PLAYSTATE.NAV) {
            navigate.navMesh(delta, navState);
        } else if (playState == PLAYSTATE.PHYSICS) {
            movement.inputPhysics(delta);
        } else {
            MoveAndSlide(new Vector3(0, -gravity*delta, 0));
        }
    }
}