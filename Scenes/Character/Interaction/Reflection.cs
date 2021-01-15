using Godot;
using System;
using System.Collections.Generic;

public class Reflection : Area {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export] private int lightRating = 0; //Light Rating is separate from Movement and Camo
    [Export] private int moveRating = 0;
    [Export] private int camoRating = 0;
    SphereShape lightShape;
    AI ai;
    Character self;
    Dictionary<string, Character> colliding = new Dictionary<string, Character>();
    Dictionary<int, int> recentRatings = new Dictionary<int, int>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        ai = GetTree().Root.GetNode<AI>("Master/AI");
        self = GetParent(/*Interaction*/).GetParent<Character>();
        lightShape = (SphereShape)GetNode<CollisionShape>("CollisionShape").Shape;
        updateRadius();
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta) {
        distributeLight();
    }

    public void distributeLight() {
        string[] confirmed = new string[colliding.Count];
        int z = 0;
        foreach(KeyValuePair<string, Character> collided in colliding) {
            if(reflectLight(collided.Value)) {
                confirmed[z] = collided.Key;
                z++;
            }
        } 
        for(int i = 0; i < z; i++) {
            colliding.Remove(confirmed[i]);
        }
    }

    public void addRating(int id, int newRating) {
        recentRatings.Add(id, newRating);//All light sources will be items with ids.
        if(newRating < lightRating) {
            lightRating = newRating;
            updateRadius();
        }
    }

    public void removeRating(int id) {
        recentRatings.Remove(id);
        updateRatings();
    }

    public void updateRatings() {//Probably should double check if a light source is no longer in range here too.
        lightRating = 1;
        foreach(KeyValuePair<int, int> rating in recentRatings) {
            if(lightRating < rating.Value) {
                lightRating = rating.Value;
            }
        }
        updateRadius();
    }

    public void updateRadius() {
        lightShape.Radius = lightRating * 6;
    }

    public bool reflectLight(Character character) {
        bool result = false;
        if(!character.maySee(lightRating)) return result;
        Vector3 charEyes = character.GetNode<RayCast>("Vision").GlobalTransform.origin;
        Vector3 charFacing = -character.GetNode<Vision>("Vision").GlobalTransform.basis.z;
        Vector3 selfOrigin = self.GetNode<Vision>("Vision").GlobalTransform.origin;
        float[] fov = Math.fieldOfVision(charEyes, charFacing, selfOrigin);
        float distance = charEyes.DistanceTo(selfOrigin);
        
        if(character.wouldSee(fov[0], fov[1], distance, moveRating, camoRating)){
            result = character.GetNode<Vision>("Vision").look(self); //RayCast
            GD.Print(fov[0], " ", fov[1]); //Degrees needs more testing but light reflection is working.
            if(result) character.receiveVisual(self, fov[0], fov[1], distance, moveRating, camoRating);
        }
        return result;
    }

    public void _on_Body_Entered(Node entrie) {
        if(entrie is Character character){
            if(character.fullName != self.fullName) {
                colliding.Add(character.fullName, character);
            }
        }
    }
    public void _on_Body_Exited(Node exiter) {
        if(exiter is Character character){
            if(colliding.ContainsKey(character.fullName)) {
                colliding.Remove(character.fullName);
            }
        }
    }
}
