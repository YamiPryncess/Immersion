using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

public class Interaction : Node {
    public Character character;
    public Treasury treasury;
    private string savePath;
    public override void _Ready() {
        character = GetParent<Character>();
        treasury = GetTree().Root.GetNode<Treasury>("Master/NavWorld/Treasury");
        savePath = "user://" + character.fullName + "-Inventory.txt";
        character.Connect(nameof(Character.InventoryChanged),this,"_on_Inventory_Changed");      
        File file = new File();
        bool exists = file.FileExists(savePath);
        if (exists){
            Error Err = file.Open(savePath, File.ModeFlags.Read);
            GD.Print("Inventory File Open Status: ", Err);
            string itemJson = file.GetAsText();
            file.Close();
            character.inventory = JsonConvert.DeserializeObject<Dictionary<string,ItemCore>>(itemJson);
        } else {
            character.inventory.Add("Currency", treasury.getItem("Currency"));
            character.EmitSignal(nameof(Character.InventoryChanged));
        }
        GD.Print(character.inventory["Currency"].synset);
    }
    public void _on_Inventory_Changed(){
        string invJson = JsonConvert.SerializeObject(character.inventory, Formatting.Indented);
        File file = new File();
        Error err = file.Open(savePath, File.ModeFlags.Write);
        GD.Print("Inventory File Open Status: ", err);
        file.StoreString(invJson);
        file.Close();
    }
}

//Removed Code
// List<ItemCore> itemCores = new List<ItemCore>();
// foreach(KeyValuePair<string, Item> core in inventory){
//     itemCores.Add(core.Value.info);
// }
// string itemJson = JsonConvert.SerializeObject(itemCores, Formatting.Indented);
// List<ItemCore> itemCores = JsonConvert.DeserializeObject<List<ItemCore>>(itemJson);
//             for(var i = 0; i < itemCores.Count; i++){
//                 character.inventory.Add(itemCores[i].itemType, new Item(itemCores[i]));
//             }
//public void _on_Inventory_Changed(Inventory inventory){
    //ResourceSaver.Save(savePath, inventory);
//}
// public override void _Ready() {
//         character = GetParent<Character>();
//         treasury = GetTree().Root.GetNode<Treasury>("Master/NavWorld/Treasury");
//         savePath = "user://" + character.fullName + "-Inventory.tres";
//         bool exists = ResourceLoader.Exists(savePath);

//         if (exists){
//             character.inventory = ResourceLoader.Load<Inventory>(savePath);
//         } else {
//             character.inventory = new Inventory();
//             character.inventory.items.Add("Currency", new Item(treasury.getItem("Currency")));
//             EmitSignal(nameof(Character.InventoryChanged), character.inventory);
//         }
//         GD.Print(character.inventory.items["Currency"].synset);
//     }
