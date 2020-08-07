using Godot;
using System.Collections.Generic;

public class UI : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    LineEdit playerLine;
    WordTrie wordTrie;
    string[] currentLine;
    Master master;
    public override void _Ready(){
        master = GetTree().Root.GetNode<Master>("Master");
        playerLine = (LineEdit)GetNode("PlayerLine");
        wordTrie = GetNode<WordTrie>("WordTrie");
    }

    public override void _Process(float delta){
        input();
        // if(Master.player != null && Master.player.playState == Character.PLAYSTATE.TEXT) {
        //     debug();
        // }
    }

  
    public void input(){
        if (Input.IsActionJustPressed("interact")){
            playerLine.GrabFocus();
            if(master.player.playState != Character.PLAYSTATE.TEXT) {
                master.player.playState = Character.PLAYSTATE.TEXT;
            }
        }
        if(Input.IsActionJustPressed("ui_cancel")){
            playerLine.ReleaseFocus();
            if(master.player.playState != Character.PLAYSTATE.PHYSICS) {
                master.player.playState = Character.PLAYSTATE.PHYSICS;
            }
        }
    }

    public void _on_PlayerLine_text_changed(string text){
        
    }

    public void _on_PlayerLine_text_entered(string text) {
        if(text.Length > 0){
            currentLine = text.Split(" ");
            if(currentLine.Length > 1){
                if(currentLine[0] == "Insert") {
                    wordTrie.insert(currentLine[1]);
                    GD.Print("Inserted: ", currentLine[1]);
                }
                if(currentLine[0] == "Find") {
                    GD.Print("Found? ", wordTrie.find( currentLine[1]), ": ", currentLine[1]);
                }
                if(currentLine[0] == "Delete") {
                    wordTrie.remove(currentLine[1]);
                }
                if(currentLine[0] == "Search") {
                    List<string> predictedText = wordTrie.search(currentLine[1]);
                    for(var i = 0; i < predictedText.Count; i++){
                        GD.Print("Searched: ", predictedText[i]);
                    }
                }
            }
        }
    }

    public void _on_PlayerLine_text_change_rejected() {
        //var string_size = font.get_string_size(text.substr(0, get_caret_position()))
    }

    public void debug() {

    }

}