using Godot;
using System.Collections.Generic;

public class Action : Node2D
{
	//public int valued = 0;
	//public int shadowed = 0;
	//public EventLog myLog;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Name of the action
    public string actionName = "Action";
    public string actionType = "";
    // Cost of the action
    public float cost = 1.0f;
    // Target where the action is going to take place
    public Node target;
    // Store the tag
    public string targetTag;
    // Duration the action should take
    public float duration = 0.0f;
    // An array of WorldStates of preconditions
    //public WorldState[] preConditions;
    // An array of WorldStates of afterEffects
    //public WorldState[] afterEffects;
    // The NavMEshAgent attached to the agent
    public Navigation navAgent;
    // Dictionary of preconditions
    public Dictionary<string, int> preconditions;
    // Dictionary of effects
    public Dictionary<string, int> effects;
    // State of the agent
    //public WorldStates agentBeliefs;
    // Access our inventory
    //public GInventory inventory;
    //public WorldStates beliefs;
    // Are we currently performing an action?
    public bool running = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get hold of the agents NavMeshAgent
        navAgent = (Navigation)GetParent();
        

        // Check if there are any preConditions in the Inspector
        // and add to the dictionary
        // if (preConditions != null) {

        //     foreach (WorldState w in preConditions) {

        //         // Add each item to our Dictionary
        //         preconditions.Add(w.key, w.value);
        //     }
        // }

        // // Check if there are any afterEffects in the Inspector
        // // and add to the dictionary
        // if (afterEffects != null) {

        //     foreach (WorldState w in afterEffects) {

        //         // Add each item to our Dictionary
        //         effects.Add(w.key, w.value);
        //     }
        // }
        // Populate our inventory
        //inventory = this.GetComponent<GAgent>().inventory;
        // Get our agents beliefs
        //beliefs = this.GetComponent<GAgent>().beliefs;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

// public abstract class GAction : MonoBehaviour {

  

//     // Constructor
//     public GAction() {

//         // Set up the preconditions and effects
//         preconditions = new Dictionary<string, int>();
//         effects = new Dictionary<string, int>();
//     }

//     public bool IsAchievable() {

//         return true;
//     }

//     //check if the action is achievable given the condition of the
//     //world and trying to match with the actions preconditions
//     public bool IsAhievableGiven(Dictionary<string, int> conditions) {

//         foreach (KeyValuePair<string, int> p in preconditions) {

//             if (!conditions.ContainsKey(p.Key)) {

//                 return false;
//             }
//         }
//         return true;
//     }

//     public abstract bool PrePerform();
//     public abstract bool PostPerform();
// }
