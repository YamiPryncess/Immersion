using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GAgent : Node {
    // Store our list of actions
    public List<GAction> actions = new List<GAction>();
    // Dictionary of subgoals
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    // Our inventory
    public GInventory inventory;
    // Our beliefs
    public WorldStates beliefs = new WorldStates();
    // Access the planner
    GPlanner planner;
    // Action Queue
    Queue<GAction> actionQueue;
    // Our current action
    public GAction currentAction;
    // Our subgoal
    SubGoal currentGoal;
    
    public Timer timer;
    bool invoked = false;
    //an invoked method to allow an agent to be performing a task
    //for a set location


    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        actions.Add((GAction)new Appreciation());
        foreach (GAction a in actions) {
            a.awake(GetParent<Character>(), GetNode<Navigate>("../Navigate"));
        }
        planner = GetTree().Root.GetNode<GPlanner>("Master/AI/GPlanner");
        inventory = GetNode<GInventory>("GInventory");
        timer = (Timer)GetNode("../Timer");
    }
       
    public void _on_Timer_timeout(){
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {//Supposed to be Late Update

        //if there's a current action and it is still running
        if (currentAction != null && currentAction.running) {

            // Find the distance to the target
            float distanceToTarget = GetParent<Character>().
                GlobalTransform.origin.DistanceTo(currentAction.target);
            // Check the agent has a goal and has reached that goal
            if (currentAction.nav.hasPath && distanceToTarget < 2.0f) { // currentAction.agent.remainingDistance < 1.0f) 

                if (!invoked) {

                    //if the action movement is complete wait
                    //a certain duration for it to be completed
                    
                    timer.Start(currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        // Check we have a planner and an actionQueue
        if (planner == null || actionQueue == null) {

            // If planner is null then create a new one
            //planner = new GPlanner();

            // Sort the goals in descending order and store them in sortedGoals
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            //look through each goal to find one that has an achievable plan
            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {

                actionQueue = planner.plan(actions, sg.Key.sGoals, beliefs);
                // If actionQueue is not = null then we must have a plan
                if (actionQueue != null) {

                    // Set the current goal
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        // Have we an actionQueue
        if (actionQueue != null && actionQueue.Count == 0) {

            // Check if currentGoal is removable
            if (currentGoal.remove) {

                // Remove it
                goals.Remove(currentGoal);
            }
            // Set planner = null so it will trigger a new one
            planner = null;
        }

        // Do we still have actions
        if (actionQueue != null && actionQueue.Count > 0) {

            // Remove the top action of the queue and put it in currentAction
            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform()) {

                // An optional way to get the target position, Otherwise target is set in the inheriting action
                /*if (currentAction.target == null && currentAction.targetTag != "") {

                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }*/ //Not used for now, will be refactored in later!!

                if (currentAction.target != null) {

                    // Activate the current action
                    currentAction.running = true;
                    // Pass Unities AI the destination for the agent
                    currentAction.agent.navigate.traverse(currentAction.target);
                }
            } else {

                // Force a new plan
                actionQueue = null;
            }
        }
    }
}


public class SubGoal {

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
}