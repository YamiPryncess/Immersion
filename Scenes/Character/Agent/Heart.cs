using Godot;
using System;

public class Heart : Node {
    GAgent agent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        agent = GetParent<GAgent>();
        // Call base Start method
        // base.Start(); // Supposed to be a GAgent Start but it was already cause since it's the parent
        // Set goal so that it can't be removed so the nurse can repeat this action
        SubGoal s1 = new SubGoal("treatPatient", 1, false);
        agent.goals.Add(s1, 3);

        // Resting goal
        SubGoal s2 = new SubGoal("rested", 1, false);
        agent.goals.Add(s2, 1);

        // Call the GetTired() method for the first time
        //Invoke("GetTired", Random.Range(10.0f, 20.0f));
    }

/*    void GetTired() {

        beliefs.ModifyState("exhausted", 0);
        //call the get tired method over and over at random times to make the nurse
        //get tired again
        Invoke("GetTired", Random.Range(0.0f, 20.0f));
    }
*/
}