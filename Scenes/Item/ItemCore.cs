using Godot;
using System.Collections.Generic;

public class ItemCore {//ItemCores can hold other ItemCores as containers or just sets of items
    [Export] public int id = -1;//Only the container id is stored in inventory
    public List<ItemCore> container = new List<ItemCore>(); //The rest would be in this list here
    [Export] public Transform transform;
    [Export] public string type = "";
    [Export] public string synset = "";
    public bool packed = false;
    [Export] public float weight = 0;
    [Export] public float energyLvl = 0;
    [Export] public float toxicLvl = 0;
    [Export] public float digestiveLvl = 0;
    [Export] public float forceLvl = 0;
    [Export] public float magneticLvl = 0;
    [Export] Texture texture;
    [Export] Mesh mesh;
    public ItemCore() {}
    public ItemCore(string _type, string _synset) {
        type = _type;
        synset = _synset;
    }

    public ItemCore(ItemCore itemCore) {
        type = itemCore.type;
        synset = itemCore.synset;
        packed = itemCore.packed;
        weight = itemCore.weight;
        energyLvl = itemCore.energyLvl;
        toxicLvl = itemCore.toxicLvl;
        digestiveLvl = itemCore.digestiveLvl;
        forceLvl = itemCore.forceLvl;
        magneticLvl = itemCore.magneticLvl;
        texture = itemCore.texture;
        mesh = itemCore.mesh;
    }
}