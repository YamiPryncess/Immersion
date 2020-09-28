using Godot;
using System;
using System.Collections.Generic;

public abstract class GAction : Reference {
    // Name of the action
    public string actionName = "Action";
    // Cost of the action
    public float cost = 1.0f;
    // Target where the action is going to take place
    public Vector3 target; //Used to be game object
    // Store the tag
    public string targetTag;
    // Duration the action should take
    public float duration = 0.0f;
    // An array of WorldStates of preconditions
    public WorldState[] preConditions;
    // An array of WorldStates of afterEffects
    public WorldState[] afterEffects;
    // The NavMEshAgent attached to the agent
    public Navigate nav;
    // Dictionary of preconditions
    public Dictionary<string, int> preconditions;
    // Dictionary of effects
    public Dictionary<string, int> effects;
    
    public Character agent;
    // State of the agent
    public WorldStates agentBeliefs;
    // Access our inventory
    public GInventory inventory;
    public WorldStates beliefs;
    // Are we currently performing an action?
    public bool running = false;
    
    // Constructor
    public GAction() {

        // Set up the preconditions and effects
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void awake(Character _agent, Navigate _nav){ //Supposed to be Private Void Awake
        
        // Get hold of the agents NavMeshAgent
        //nav = GetNode<Navigate>("../../Navigate");

        // Check if there are any preConditions in the Inspector
        // and add to the dictionary
        if (preConditions != null) {

            foreach (WorldState w in preConditions) {

                // Add each item to our Dictionary
                preconditions.Add(w.key, w.value);
            }
        }

        // Check if there are any afterEffects in the Inspector
        // and add to the dictionary
        if (afterEffects != null) {

            foreach (WorldState w in afterEffects) {

                // Add each item to our Dictionary
                effects.Add(w.key, w.value);
            }
        }
        agent = _agent;
        nav = _nav;
        // Populate our inventory
        inventory = (GInventory)agent.GetNode("GAgent/GInventory");
        // Get our agents beliefs
        beliefs = agent.GetNode<GAgent>("GAgent").beliefs;
    }

    public bool IsAchievable() {

        return true;
    }

    //check if the action is achievable given the condition of the
    //world and trying to match with the actions preconditions
    public bool IsAchievableGiven(Dictionary<string, int> conditions) {

        foreach (KeyValuePair<string, int> p in preconditions) {

            if (!conditions.ContainsKey(p.Key)) {
                return false;
            }
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}

