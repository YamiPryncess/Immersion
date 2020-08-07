using Godot;
using System.Collections.Generic;
using System.Linq;

public class OldMaster : Spatial
{
    // Our world states
    //private static WorldStates world;
    // Queue of cubicles
    //private static List<Item> food;

    public Object player;
    //public Npc npc;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){

    }

    public override void _Process(float delta){

    }

    public void initPlayer(){
        //player = GetTree().Root.GetNode("/root/GameScene/Player");
        //npc = (Npc)GetTree().Root.GetNode("/root/GameScene/Navigation/Npc");
        //if(player == null || npc == null) return;
        //Object pInventory = (Object)player.Get("inventory");

        /*EmitSignal(nameof(player_initialised));
        pInventory.Connect("inventory_changed", this, nameof(_on_Inventory_changed));
        Object existing_pInv = GD.Load("user://pInv.tres");
        loadInv(existing_pInv, pInventory);*/
    }
}

        // Create our world
        //world = new WorldStates();
        // Create cubicles array
        //food = GetTree();
        // Find all GameObjects that are tagged "Cubicle"
        //Item[] item = GetTree().GetNodesInGroup("items").OfType<Item>().ToArray();
        // Then add them to the cubicles Queue
        //foreach (Item f in food) {

            //cubicles.Enqueue(c);
        //}

        // Inform the state
        //if (cubes.Length > 0) {
          //  world.ModifyState("FreeCubicle", cubes.Length);
        //}

        // Set the time scale in Unity
        //Time.timeScale = 5.0f;
    //}

//    private GWorld() {

  //  }

    // Add patient
    //public void AddPatient(GameObject p) {

       // // Add the patient to the patients Queue
      //  patients.Enqueue(p);
    //}

    // Remove patient
/*    public GameObject RemovePatient() {

        if (patients.Count == 0) return null;
        return patients.Dequeue();
    }

    // Add cubicle
    public void AddCubicle(GameObject p) {

        // Add the patient to the patients Queue
        cubicles.Enqueue(p);
    }

    // Remove cubicle
    public GameObject RemoveCubicle() {

        // Check we have something to remove
        if (cubicles.Count == 0) return null;
        return cubicles.Dequeue();
    }

    public static GWorld Instance {

        get { return instance; }
    }

    public WorldStates GetWorld() {

        return world;
    }
	
	//All Game Lists: Person, Action, Item, Concept, Checkpoint
	public Dictionary<string, Person> persons; //type-Id, Person
	public Dictionary<string, Item> items; //type-Id, Item
	public Dictionary<string, Action> actions; //type-Id, Action
	public Dictionary<string, Concept> concepts; //type-Id, Concept
	public Dictionary<string, Checkpoint> checkpoints; //type-Id, Checkpoint

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	public Master() {
		
		persons = new Dictionary<string, Person>(); //type-Id, Person
		items = new Dictionary<string, Item>(); //type-Id, Item
		actions = new Dictionary<string, Action>(); //type-Id, Action
		concepts = new Dictionary<string, Concept>(); //type-Id, Concept
		checkpoints = new Dictionary<string, Checkpoint>(); //type-Id, Checkpoint
	}

	public virtual void loadLog(EventLog myLog, string subjectId, string subjectType) {
		myLog = new EventLog(subjectId, subjectType); //Personal logging for facts of things that happened to individual.
										   //When game loads new myLog is retrieved.
										  //When new info is logged it goes here first then eventLog.
	}
	public void unloadLog(EventLog myLog) {
		if (myLog != null) {
			myLog = null;
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}*/