using Godot;
using System.Collections.Generic;

public class UI : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    LineEdit playerLine;
    ItemList itemList;
    WordTrie wordTrie;
    string[] currentLine;
    Master master;
    bool debug = false;
    public override void _Ready(){
        master = GetTree().Root.GetNode<Master>("Master");
        playerLine = (LineEdit)GetNode("PlayerLine");
        itemList = (ItemList)GetNode("PlayerLine/ItemList");
        wordTrie = GetNode<WordTrie>("WordTrie");
        string[] words = new string[]{"i", "a", "may", "pizza", "food", "love", "lose",
        "let", "lets", "loves", "lover", "give", "me", "have", "no", "yes", "want", 
        "need", "desire", "never", "look", "loss", "loser", "loveliest", "else", "tell", "me",
        "know", "feel", "hunger", "table", "worship", "stay", "with", "my", "loveliest", "lovely",
        "kill", "killing", "kills", "harsh", "kind", "great", "create", "move", "sister", "brother",
        "mom", "dad", "heal", "cherish", "nevermind", "master", "lord", "break", "romance", "happy",
        "sad", "fear", "up", "down", "depressed", "angry", "worried", "anxious", "scared", "at", "you",
        "go", "get", "know", "forever", "cat", "hate", "worship", "pray", "ritual", "culture",
        "nice", "fuel", "fuse", "forget", "remember", "memory" };
        for(int i = 0; i < words.Length; i++){
            wordTrie.insert(words[i]);
        }
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
        Font playerFont = playerLine.GetFont("");
        Vector2 stringSize = new Vector2();
        List<int> wordCount = new List<int>();
        int countInd = 0;
        string newText = "";
        bool space = true;
        bool startCounting = true; //Skips the first space after each word
        int caret = playerLine.CaretPosition;
        int spacesTillCaret = 0;
        int sentenceLengthTillWordAtCaret = 0;
        string currentWord = "";

        if(text.Length > 0){
            currentLine = text.Split(" ");
            if(currentLine.Length > 0){
                //Count the text
                for(var i = 0; i < text.Length; i++){
                    if(text[i] != ' ' && space == true){
                        //wordCount[countInd] = i;
                        space = false;
                    } else if(text[i] == ' ' && space == false) {
                        space = true;
                        startCounting = true;//Skip the first space after each word
                        countInd++;
                    } else if(startCounting == true) { //Only count beginning spaces and 2 spaces after each word.
                        if(space == true && i < caret) {
                            spacesTillCaret++;
                        }
                    }
                }
                //Space the Text
                for(var i = 0; i < currentLine.Length; i++){
                    if(i == 0){
                        newText += currentLine[i];
                    } else {
                        newText += " " + currentLine[i];
                    }
                    
                }
                if(text[text.Length - 1] == ' '){
                    newText += " ";
                }
                //Reset the text
                playerLine.Text = newText;
                playerLine.CaretPosition = caret - spacesTillCaret;
                caret = playerLine.CaretPosition;
                //Find the caret position and check what word it's on.
                for(var i = 0; i < currentLine.Length; i++){
                    if(sentenceLengthTillWordAtCaret + currentLine[i].Length + 1 < caret){
                        sentenceLengthTillWordAtCaret += currentLine[i].Length + 1;
                    } else {
                        currentWord = currentLine[i];
                    }
                }
                //Move Item List to the word that the caret is on.
                stringSize = playerFont.GetStringSize(playerLine.Text.Substr(0, sentenceLengthTillWordAtCaret));
                itemList.RectPosition = new Vector2(stringSize.x + 1, itemList.RectPosition.y);

                //Search Trie for the word the user is currently changing text on.
                List<string> predictedText = wordTrie.search(currentWord);
                itemList.Clear();
                if(predictedText == null) {
                    GD.Print("No searchable word follows from player's text.");
                    itemList.Hide();
                } else {
                    GD.Print("Search: ", currentWord);
                    for(var i = predictedText.Count - 1; i >= 0; i--){
                        GD.Print("Searched: ", predictedText[i]);
                        itemList.Show();
                        itemList.AddItem(predictedText[i], null, true);
                    }
                    itemList.Select(predictedText.Count-1);
                    itemList.EnsureCurrentIsVisible();
                }
            }
        }
    }

    public void _on_PlayerLine_text_entered(string text) {
        if(debug == true) {
            debugTrie(text);
        }
    }

    public void _on_PlayerLine_text_change_rejected() {

    }

    public void debugTrie(string text) {
        if(text.Length > 0){
            currentLine = text.Split(" ");
            if(currentLine.Length > 1){
                if(currentLine[0] == "Insert") {
                    wordTrie.insert(currentLine[1]);
                    GD.Print("Inserted: ", currentLine[1]);
                }
                if(currentLine[0] == "Find") {
                    GD.Print("Found? ", currentLine[1], " = ", wordTrie.find( currentLine[1]));
                }
                if(currentLine[0] == "Delete") {
                    wordTrie.remove(currentLine[1]);
                }
                if(currentLine[0] == "Search") {
                    List<string> predictedText = wordTrie.search(currentLine[1]);
                    if(predictedText == null) {
                        GD.Print("No searchable word follows from player's text.");
                    } else {
                        GD.Print("Search: ", currentLine[1]);
                        for(var i = predictedText.Count - 1; i >= 0; i--){
                            GD.Print("Searched: ", predictedText[i]);
                            itemList.AddItem(predictedText[i], null, true);
                        }
                    }
                }
            }
        }
    }

}