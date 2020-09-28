using Godot;
using System.Linq;
public class Checkpoints : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach(var child in GetChildren().OfType<Position3D>()){
            child.AddToGroup("Checkpoints");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

    // Position3D[] children;

    // // Called when the node enters the scene tree for the first time.
    // public override void _Ready()
    // {
    //     children = GetChildren().OfType<Position3D>().ToArray();
    //     for(int i = 0; i < children.Length; i++) {
    //         if(children[i] is Position3D) {
    //             children[i].AddToGroup("Checkpoints");
    //         }
    //     }
    // }