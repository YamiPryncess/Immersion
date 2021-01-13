using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class GWorld : Node {
    // Our world states
    public WorldStates world;
    //public Dictionary<string, int> actions = new Dictionary<string, int>();
    //public Dictionary<string, int> contexts = new Dictionary<string, int>();
    //Dictionary<string, Dictionary<string, int>> entities = new Dictionary<string, Dictionary<string, int>>();//All people and groups in the game. Ideas of types of people count to: Friends, Enemies, Etc.
    //Dictionary<string, Dictionary<string, int>> actions = new Dictionary<string, Dictionary<string, int>>();//Manner in which the entity-concept relationship is handled/has been handled in the past. Value 0 refers to general bias of the concept then

    public override void _Ready() {
        // Create our world
        world = (WorldStates)GetNode("../WorldStates");
        //classifyWorld();
    }
    public WorldStates GetWorld() {

        return world;
    }
}
    /*public void classifyWorld(){
        addAction("Let Be"); //Actions must be unambigous and related to goals
        addAction("Pressure"); //So regardless whether Agent understand what stealing is, Borrowing a pen and Stealing a pen are the same action/goal.
                                //These behaviors may be dynamically built rather than pre-written: "How to take pen without consequences, Solution = Don't tell."
        addContext("General");//I feel context is lacking... without being able to take into account the specific state of the subject in context.
        addContext("Player");//Like don't be honest with person when they're sad for example.
                            //In that case a context can be changed for subject and a third array can be added for state? lol... But that doesn't record any reasoning as to why, just pre-wired preference.
    }
    public void addAction(string act){ 
        actions.Add(act, actions.Count);
    }
    public void addContext(string context){ 
        contexts.Add(context, contexts.Count);
    }*/
    // public List<Dictionary<string,object>> filter(List<Dictionary<string, object>> entity, List<string> criteria) {
    //     List<string>[] orderedEnts = new List<string>()[criteria.Count];
    //     List<Dictionary<string,object>> orderedEntities = new List<List<Dictionary<string, object>>>(); 
    //     for(var i = 0; i < entity.Count; i++) {
    //         int match = 0;
    //         for(var j = 0; j < criteria.Count; j++) {
    //             if(entity[i].ContainsKey(criteria[j])){
    //                 match++;
    //             }
    //         }
    //         orderedEntities
    //     }
    // }
    /*public void classifyWorld() {
        entities.Add("People", new Dictionary<string, int>());
        entities.Add("Groups", new Dictionary<string, int>());
        entities.Add("Archetypes", new Dictionary<string, int>());
        entities.Add("Food", new Dictionary<string, int>());
        entities.Add("Fungi", new Dictionary<string, int>());
        entities.Add("Play", new Dictionary<string, int>());
        entities.Add("Philosophy", new Dictionary<string, int>());
        entities.Add("Color", new Dictionary<string, int>());
        entities["People"].Add("Lynn", 0);
        entities["People"].Add("Alexia", 0);
        entities["Groups"].Add("YamiRakuen", 0);
        entities["Archetypes"].Add("Friend", 0);
        entities["Archetypes"].Add("Enemy", 0);
        entities["Archetypes"].Add("Father", 0);
        entities["Archetypes"].Add("Mother", 0);
        entities["Food"].Add("Pizza", 0);
        entities["Food"].Add("Plantains", 0);
        entities["Play"].Add("Video Games", 0);
        entities["Play"].Add("Swings", 0);
        entities["Play"].Add("Dance", 0);
        entities["Philosophy"].Add("Humanitarianism", 0);
        entities["Philosophy"].Add("Unus Mundus", 0);
        entities["Color"].Add("Yellow", 0);

        actions.Add("Bias", new Dictionary<string, int>());
        actions.Add("Happy", new Dictionary<string, int>());
        actions.Add("Sad", new Dictionary<string, int>());
        actions["Bias"].Add("Bias", 0);
        actions["Happy"].Add("Alexia", 0);
        actions["Happy"].Add("YamiRakuen", 0);
        actions["Happy"].Add("Friend", 0);
        actions["Sad"].Add("Enemy", 0);
        actions["Sad"].Add("Father", 0);
        actions["Sad"].Add("Mother", 0);

    }*/

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