using Godot;
using System;
using System.Collections.Generic;

public class InventoryUI : Node {
    Character character;
    GridContainer grid;
    public override void _Ready() {
        character = GetTree().Root.GetNode<Character>("Master/NavWorld/Lynn");
        grid = GetNode<GridContainer>("GridContainer");
        character.Connect(nameof(Character.InventoryChanged), this, "_on_Inventory_Changed");
        invChanged();
    }
    public void _on_Inventory_Changed(){
        invChanged();
    }
    public void invChanged() {
        foreach(Label child in grid.GetChildren()){
            child.QueueFree();
        }
        foreach(KeyValuePair<string, ItemCore> item in character.inventory) {
            Label label = new Label();
            label.Text = item.Value.type;
            AddChild(label);
        }
        /*foreach(Label child in grid.GetChildren()){
            child.GetGlobalTransform();
        }*/
    }
}