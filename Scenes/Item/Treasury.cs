using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

public class Treasury : Node {
    [Signal]
    public delegate void IdsChanged();
    Dictionary<string, ItemCore> items = new Dictionary<string, ItemCore>();//Inventory is by id but creating items is by type
    List<bool> itemIds = new List<bool>();
    string path = @"res://Scenes/Item/Data/ItemTypes.txt";
    string idPath = "user://ItemIds.txt";
    public override void _Ready() {
        Connect(nameof(IdsChanged),this,"_on_Ids_Changed");
        newItemType("Pizza", "ewn-07889783-n");
        newItemType("Currency", "ewn-13407086-n");
        saveTreasury();
        loadTreasury();
        loadIds();

        foreach(var child in GetChildren().OfType<Item>()){
            child.info = createItem(child.tempType);
        }
    }
    public void _on_Ids_Changed(){
        saveIds();
        loadIds();
        string printMe = "";
        for(int i = 0; i < itemIds.Count; i++) {
            printMe += itemIds[i] + " ";
        }
        GD.Print("Ids saved & loaded: ", printMe);
    }
    public void newItemType(string type, string synset) {
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
    public ItemCore createItem(string type) {
        ItemCore itemCore = new ItemCore(items[type]);
        generateId(itemCore);
        return itemCore;
    }
    public void generateId(ItemCore itemCore) {
        bool idFound = false;
        for(var i = 0; i < itemIds.Count; i++){
            if(!itemIds[i]) { //Currently a true or false system but it should probably store the iCore ref
                itemCore.id = i;
                itemIds[i] = true;
                idFound = true;
                break;
            }
        }
        if(!idFound) {
            itemCore.id = itemIds.Count;
            itemIds.Add(true);
        }
        EmitSignal(nameof(IdsChanged));
    }
    public void removeId(ItemCore itemCore) {
        itemIds[itemCore.id] = false;
        itemCore.id = -1;
        EmitSignal(nameof(IdsChanged));
    }

    public void loadIds(){
        File file = new File();
        bool exists = file.FileExists(idPath);
        
        if(exists){
            Error err = file.Open(idPath, File.ModeFlags.Read);
            GD.Print("Item Ids File Open Status: ", err);
            string itemJson = file.GetAsText();
            file.Close();
            itemIds = JsonConvert.DeserializeObject<List<bool>>(itemJson);
        }
    }
    public void saveIds() {
        string itemJson = JsonConvert.SerializeObject(itemIds, Formatting.Indented);
        File file = new File();
        Error err = file.Open(idPath, File.ModeFlags.Write);
        GD.Print("Item Ids File Open Status: ", err);
        file.StoreString(itemJson);
        file.Close();
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