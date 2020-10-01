using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

public class Treasury : Node {
    Dictionary<string, ItemCore> items = new Dictionary<string, ItemCore>();
    string path = @"res://Scenes/Item/Data/ItemTypes.txt";
    public override void _Ready() {
        newTreasure("Pizza", "ewn-07889783-n");
        newTreasure("Currency", "ewn-13407086-n");
        saveTreasury();
        loadTreasury();
    }
    public void newTreasure(string type, string synset) {
        items.Add(type, new ItemCore(type, synset));
    }
    public void saveTreasury() {
        string itemJson = JsonConvert.SerializeObject(items, Formatting.Indented);
        File file = new File();
        Error err = file.Open(path, File.ModeFlags.Write);
        GD.Print("Treasury File Open Status: ", err);
        file.StoreString(itemJson);
        file.Close();
    }
    public void loadTreasury() {
        File file = new File();
        bool exists = file.FileExists(path);
        Debug.Assert(exists);

        if(exists){
            Error err = file.Open(path, File.ModeFlags.Read);
            GD.Print("Treasury File Open Status: ", err);
            string itemJson = file.GetAsText();
            file.Close();
            items = JsonConvert.DeserializeObject<Dictionary<string, ItemCore>>(itemJson);
            GD.Print("Pizza Deserialized: ",items["Pizza"].type,"-",items["Pizza"].synset);    
        }
    }
    public ItemCore getItem(string name) {
        return items[name];
    }
}
//Removed Code
    //public void saveTreasure(string type, string synset) {
        // ResItem item = new ResItem(type, synset);
        // ResourceSaver.Save("res://Scenes/Item/Data/"+type+".tres", item);
        // items.Add(type,
        //     ResourceLoader.Load<ResItem>("res://Scenes/Item/Data/"+type+".tres"));
        // GD.Print("Pizza: ", items.ContainsKey("Pizza"));
    //}
    //     public void loadTreasury() {
    //     Directory directory = new Directory();
    //     directory.Open("res://Scenes/Item/Data");
    //     directory.ListDirBegin();
    //     bool process = true;
    //     while(process){
    //         string type = directory.GetNext();
    //         if(type == "") process = false;
    //         if(!directory.CurrentIsDir() && process != false){
    //             items.Add(type.Substring(0, type.Find(".")),
    //                 ResourceLoader.Load<ResItem>("res://Scenes/Item/Data/" + type));
    //         }
    //     }
    // }