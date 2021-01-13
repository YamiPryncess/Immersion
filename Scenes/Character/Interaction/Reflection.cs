using Godot;
using System;
using System.Collections.Generic;

public class Reflection : Area {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private int lightRating = 0; //Light Rating is separate from Movement and Camo
    private int moveRating = 0;
    private int camoRating = 0;
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
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        
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

    public void isLookingAtMe() {
        foreach(KeyValuePair<string, Character> character in colliding) {
            if(!character.Value.maySee(lightRating)) continue;
            Vector3 charEyes = character.Value.GetNode<RayCast>("CharCam/Eyes").GlobalTransform.origin;
            Vector3 charFacing = -character.Value.GetNode<CharCam>("CharCam").GlobalTransform.basis.z;
            Vector3 selfOrigin = GlobalTransform.origin;
            float[] fov = Math.fieldOfVision(charEyes, charFacing, selfOrigin);
            float distance = charEyes.DistanceTo(selfOrigin);
            
            if(character.Value.wouldSee(fov[0], fov[1], distance, moveRating, camoRating)){
                bool result = character.Value.GetNode<Eyes>("CharCam/Eyes").look(self); //RayCast
                if(result) character.Value.receiveVisual(self, fov[0], fov[1], distance, moveRating, camoRating);
            }
            
        }
    }

    public void _on_Body_Entered(Node entrie) {
        if(entrie is Character character){
            colliding.Add(character.fullName, character);
        }
    }
    public void _on_Body_Exited(Node exiter) {
        if(exiter is Character character){
            colliding.Remove(character.fullName);
        }
    }
}
