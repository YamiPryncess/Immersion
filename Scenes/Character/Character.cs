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
    public Dictionary<int, ItemCore> inventory = new Dictionary<int, ItemCore>();
    [Export] public string fullName = "LynnCelestine";
    public AnimationTree animationTree;
    public AnimationNodeStateMachinePlayback stateMachine;
    public CharCam camera;
    public Movement movement;
    public Navigate navigate;
    public GAgent GAgent;
    public List<Observation> observations = new List<Observation>();
    public float horizontalFov = 210;
    public float verticalFov = 150;
    public float directView = 30;
    public float peripheralView = 90;
    public int visibleDistance = 6000; //100 in game vector spaces * 20 Blocks * 3 miles
    public float visionRating = 0; //-3 is pitch black and 3 is blinding bright.
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
    public bool maySee(int lightRating, int zone = 0) {
        //Does this character's A.I. care about seeing some one? Maybe this should be done with collision groupings
        //If Yes, can they see them from the zone they're in?
        if(lightRating >= -visionRating){
            return true;
        }
        return false;
    }
    public bool wouldSee(float degreesH, float degreesV, float distance, int moveRating, int contrast) {
        int interest = 1; //Light is already established
        int camoRating = moveRating - contrast; //1-10 grade, (10 move = fast) - (10 contrast = invisible)
        if(degreesH < 30 && degreesV < 30) interest = 10;
        else if(degreesH < 60 && degreesV < 60) interest = 8;
        else if(degreesH < 90 && degreesV < 90) interest = 6;
        else if(degreesH < 180 && degreesV < 150) interest = 4;
        else if(degreesH < 210 && degreesV < 150) interest = 2;
        interest = interest * camoRating * 60; //100 * 60 = 6000 (3 Miles) = 3 * 20 blocks * 100 vector units
        if(interest > distance) return true; //NightVision and Scopes would see everything
        return false;
    }//This is for calculating the info. It won't be needed if it passes.
    public void receiveVisual(Godot.Object obj, float degreesH, float degreesV, float distance, int moveRating, int contrast) {

    }//This is for reacting to the info with A.I. Bright guy overthere! I see movement... Hey! He's there!
    public void receiveDamage() {

    }
}

public class Observation {
    Godot.Object observed;//Through sound, aura, or vision.
    int AlertLevel = 0; //Current Awareness of recent events.
    List<string[]> details = new List<string[]>(); //time, awareness, location, action, what
    //(action wearing, what clothes | knifed sara,
    //Actions should be specific to leave ambiguity of how it happened out,
    //Killed sara wouldn't specify that it was with a knife) 
    //Maybe multiple action and what words should be allowed in the same string though
    //details may be to simple for emotional experiences and long term memory but it'll work for now.

    public void addEncounter(string time, string awareness, string location, string action, string what) {
        details.Add(new string[5]{time, awareness, location, action, what}); //If some one is explaining their past actions
        //The past actions can be understood in action and what this way -> Action: explainedSelf, attack, revenge; What: Joe, Nightclub
    } //Eventually this will be deleted and simplified in long term memory. This is short term memory.
}