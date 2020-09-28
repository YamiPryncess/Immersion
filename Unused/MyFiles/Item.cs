using Godot;

public class Item : Area
{
    public enum states {Perfect, Broken, Stained, BrokenStained};
	public string itemId = "";
	public string itemType = "";
    public string itemName;
	public int location = 0;
	public string state = "Perfect";
	//public EventLog myLog;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //itemId = itemCounter(itemType)
        itemName = itemId + itemType;
    }

    /*
    func _on_MagickOrb_body_entered(body):
        if body == GameManager.player:
            GameManager.player.inventory.add_item("Magick Orb", 1);
            queue_free();
    */

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}