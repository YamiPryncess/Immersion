using Godot;
using System;
using System.Collections.Generic;


public class GWorld : Node {
    // Our world states
    public WorldStates world;
    public override void _Ready() {
        // Create our world
        world = (WorldStates)GetNode("../WorldStates");
    }
    public WorldStates GetWorld() {

        return world;
    }
}

//Removed Code
/*      

    // Queue of patients
    private static Queue<Node> patients;
    // Queue of cubicles
    private static Queue<Node> cubicles;
        GWorld/_ready() {// Create patients array
        //patients = new Queue<Node>();
        // Create cubicles array
        //cubicles = new Queue<Node>();
        // Find all GameObjects that are tagged "Cubicle"
        //Node[] cubes = (Node)("Cubicle");
        // Then add them to the cubicles Queue
        /*foreach (Node c in cubes) {

            cubicles.Enqueue(c);
        }

        // Inform the state
        if (cubes.Length > 0) {
            world.ModifyState("FreeCubicle", cubes.Length);
        }

        // Set the time scale in Unity
        Time.timeScale = 5.0f;
        }

    // Add patient
    public void AddPatient(GameObject p) {

        // Add the patient to the patients Queue
        patients.Enqueue(p);
    }

    // Remove patient
    public GameObject RemovePatient() {

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
    
    */