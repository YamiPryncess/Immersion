using Godot;
using System.Linq;

public class NotifSprite : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public Sprite3D[] children;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        children = new Sprite3D[GetChildCount()];
        int i = 0;
        foreach(var child in GetChildren().OfType<Sprite3D>()){
            children[i] = child;
            i++;
        }
    }

    public void presentNotif(string name) {
        for(int i = 0; i < children.Length; i++) {
            if(children[i].Name == name) {
                children[i].Visible = true;
            } else {
                children[i].Visible = false;
            }
        }
    }

    public void removeNotif() {
        for(int i = 0; i < children.Length; i++) {
            children[i].Visible = false;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
