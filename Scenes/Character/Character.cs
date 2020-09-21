using Godot;
using System.Collections.Generic;
using System.Linq;

public class Character : KinematicBody {
    //=============LOCAL VARIABLES================
    public int frame = 0;
    public Master master;
    public Spatial mannequiny;
    public Object inventory;
    [Export] private string fullName = "LynnCelestine";
    private string savePath;
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
        initInventory();
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
    //=============SIGNALS================
    public void _on_Inventory_changed(Object inventory){
        ResourceSaver.Save(savePath, (Resource)inventory);
    }

    //=============INITIALIZATION FUNCTIONS================
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
}