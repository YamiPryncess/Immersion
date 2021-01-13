using Godot;
using System.Collections.Generic;

public class PersonalSpace : Area
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public Dictionary<int,Item> collided = new Dictionary<int,Item>();    
    Character character;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        character = GetParent().GetParent<Character>();
    }

    
    public void _on_Body_Entered(PhysicsBody _item){
        if(_item is Item item) {
            collided.Add(item.info.id, item);
            GD.Print("Colliding with item: ", collided[item.info.id].info.type);
            //For some reason it collides twice with pizza and then never exits or allows me to interact.
        }//Will finish working on this tomorrow.
    }
    public void _on_Body_Exited(PhysicsBody _item){
        if(_item is Item item) {
            GD.Print("Exiting Collision of: ", collided[item.info.id].info.type);
            collided.Remove(item.info.id);
        }
    }
    public void input(float delta){
        if(Input.IsActionJustPressed("interact") && collided.Count > 0) {
            foreach(KeyValuePair<int,Item> col in collided){
                GD.Print("Interacting with item: ", collided[col.Value.info.id].info.type);
                character.inventory.Add(col.Key, col.Value.info);
                col.Value.QueueFree();
                character.EmitSignal(nameof(Character.InventoryChanged));
                break;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        input(delta);
    }
}
