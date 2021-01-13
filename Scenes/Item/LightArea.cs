using Godot;
using System;

public class LightArea : Area
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    int tempId = 0; //Id will be obtained by the object that holds it in the future.
    int rating = 8;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        
    }

    public void _on_Body_Entered(Node entrie) {
        if(entrie is Character character) {
            character.GetNode<Reflection>("Interaction/Reflection").addRating(tempId, rating);
        }
    }

    public void _on_Body_Exited(Node exiter) {
        if(exiter is Character character) {
            character.GetNode<Reflection>("Interaction/Reflection").removeRating(tempId);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
