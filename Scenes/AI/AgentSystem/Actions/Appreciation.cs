using Godot;
using System;

public class Appreciation : GAction {
    public override bool PrePerform(){
        return true;
    }
    public override bool PostPerform(){
        return true;
    }
}

//Example Code: 
/*public override bool PrePerform() {

        // Get a free cubicle
        target = inventory.FindItemWithTag("Cubicle");
        // Check that we did indeed get a cubicle
        if (target == null)
            // No cubicle so return false
            return false;
        // All good
        return true;
    }

    public override bool PostPerform() {

        // Add a new state "TreatingPatient"
        GWorld.Instance.GetWorld().ModifyState("TreatingPatient", 1);
        // Give back the cubicle
        GWorld.Instance.AddCubicle(target);
        // Remove the cubicle from the list
        inventory.RemoveItem(target);
        // Give the cubicle back to the world
        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", 1);
        return true;
    }*/