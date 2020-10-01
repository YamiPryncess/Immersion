using Godot;
using System;

public class Item : Area {
    Treasury treasury;
    public ItemCore info;
    [Export] public string tempType = "Pizza";
    public Item() {}
    public Item(ItemCore coreInfo) {
        info = coreInfo;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {//Deserialization code should go here based on item locations
        treasury = GetParent<Treasury>();//NavWorld should keep track of what worlds are loaded maybe?
        //For now this code works with items set up in the editor
    }

    public override void _Process(float delta) {
        if(info != null) {
            info = treasury.getItem(tempType);
            SetProcess(false);
        } else {
            SetProcess(false);
        }
    }
}