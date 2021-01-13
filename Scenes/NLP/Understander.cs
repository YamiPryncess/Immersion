using Godot;
using System.Collections.Generic;
public class Understander : Node {
    //List<string> entries = new List<string>();
    Character player;
    Interaction interaction;
    public override void _Ready(){
        player = (Character)GetTree().Root.GetNode<Master>("Master").player;
        interaction = player.GetNode<Interaction>("Interaction");
    }
    //public override void _Process(float delta) {
        //for(var i = 0; i < entries.Count; i++) {
            
        //}
    //}
    public void newEntry(string text) {
        //entries.Add(text);
        /*string content = "";
        if(text.ToLowerInvariant() == "yes".ToLowerInvariant()) {
            content = "yes";
        } if(text.ToLowerInvariant() == "no".ToLowerInvariant()) {
            content = "no";
        } if(text.ToLowerInvariant() == "pizza please".ToLowerInvariant()) {
            content = "polite request pizza";
        }
        if(content.Length > 0) {
            interaction.informCharacters(content);
        }*/
    }
}