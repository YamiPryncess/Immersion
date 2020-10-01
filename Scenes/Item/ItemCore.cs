using Godot;
using System;

public class ItemCore {
    [Export] public int id = 0;
    [Export] public Transform transform;
    [Export] public string type = "";
    [Export] public string synset = "";
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
}