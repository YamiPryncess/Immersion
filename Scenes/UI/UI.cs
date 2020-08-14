using Godot;
using System.Linq;
using System.Collections.Generic;

public class UI : Control {
    [Signal]
    public delegate void WordSelected();
    public enum HELPER{ LONETRIE, EMPTY, KEYBOARD }
    public enum ACTION{ UP, DOWN, LEFT, RIGHT, UNKNOWN }
    public enum EFFECT{ CHANGETEXT, JUSTSTORE, UNKNOWN}
    HELPER helperState = HELPER.EMPTY;

    //Objects (Functionality from outside this class)
    Master master; //Changes player control state
    PlayerLine playerLine; //To change the Line Edit Text
    ItemList wordList; //To display predictive text
    WordTrie wordTrie; //To predict text (Can also store new words)
    Panel keyboard;
    
    //Event Signal Variables, Only mutated when a particular event happens
    List<List<int>> detales;
    List<string> currentLine; //We store the words the player typed in here
    string searchWord = "";

    //Procedural Variables, Mutated/Used in events/input/process, not for FP Code.
    string selectedWord = ""; //Only used/mutated when _Input() needs it.

    //Mutated in Non-Top Level Function Code. This should be cleaned up eventually.
    //Top Level would be the process functions and events.
    //Since they always run procedurally.
    List<string> predictedText; //This variable probably isn't needed if I can just get the items from itemlist instead
    bool selectedIncomplete = false;//Both these variables only change when they need to so I'm leaving them for now.

    //Class Utility
    [Export] bool debug = false;

    //==================GODOT VIRTUALS======================
    public override void _Ready(){
        master = GetTree().Root.GetNode<Master>("Master");
        playerLine = GetNode<PlayerLine>("PlayerLine");
        wordList = (ItemList)GetNode("PlayerLine/WordList");
        keyboard = (Panel)GetNode("PlayerLine/Keyboard");
        //wordList.Hide();
        wordList.Visible = false;
        keyboard.Visible = false;
        wordTrie = GetNode<WordTrie>("WordTrie");
        string[] words = new string[]{"I", "A", "May", "Pizza", "Food", "Love", "Lose",
        "Let", "Lets", "Loves", "Lover", "Give", "Me", "Have", "No", "Yes", "Want", 
        "Need", "Desire", "Never", "Look", "Loss", "Loser", "Loveliest", "Else", "Tell", "Me",
        "Know", "Feel", "Hunger", "Table", "Worship", "Stay", "With", "My", "Loveliest", "Lovely",
        "Kill", "Killing", "Kills", "Harsh", "Kind", "Great", "Create", "Move", "Sister", "Brother",
        "Mom", "Dad", "Heal", "Cherish", "Nevermind", "Master", "Lord", "Break", "Romance", "Happy",
        "Sad", "Fear", "Up", "Down", "Depressed", "Angry", "Worried", "Anxious", "Scared", "At", "You",
        "Go", "Get", "Know", "Forever", "Cat", "Hate", "Worship", "Pray", "Ritual", "Culture",
        "Nice", "Fuel", "Fuse", "Forget", "Remember", "Memory", "Open", "Petty", "Pity", "Observation" };
        for(int i = 0; i < words.Length; i++){
            wordTrie.insert(words[i]);
        }
    }

    public override void _Process(float delta){
        input();
        /*if(helperState == HELPER.KEYBOARD && (!keyboard.HasFocus() || !wordList.HasFocus())) {
            wordList.Clear();
            wordList.Visible = false;
            keyboard.Visible = false;
            selectedWord = "";
            helperState = HELPER.EMPTY;
        }*/ //This if statement didn't work cause it's the individual keys that
        //have focus. Instead I'll just keep the keyboard to allow for caret movement
        //The player can just disable it manually.
    }
    //Entry Input Handler
    public override void _Input(InputEvent @event){
        if (@event is InputEventKey key){
            if(wordList.Visible && helperState == HELPER.LONETRIE) {

                if(key.IsActionPressed("ui_up")) {
                    selection(predictedText, findSelected(predictedText),
                                ACTION.UP, EFFECT.CHANGETEXT);
                } else if(key.IsActionPressed("ui_down")) {
                    selection(predictedText, findSelected(predictedText),
                                ACTION.DOWN, EFFECT.CHANGETEXT);
                }
                if(key.IsActionPressed("ui_left")) {
                    if(playerLine.CaretPosition-1 < detales[0][5]) {
                        wordList.Clear();
                        wordList.Visible = false;
                        selectedWord = "";
                        helperState = HELPER.EMPTY;
                    }
                } else if(key.IsActionPressed("ui_right")) {
                    if(playerLine.CaretPosition > 
                        detales[0][5] + selectedWord.Length - 1){
                            if(selectedIncomplete){
                                searchTrie(selectedWord);
                                GetTree().SetInputAsHandled();
                            } else {                    
                                wordList.Clear();
                                wordList.Visible = false;
                                selectedWord = "";
                                helperState = HELPER.EMPTY;
                            }
                    }
                } else if(key.IsActionPressed("ui_select")) {
                    wordList.Clear();
                    wordList.Visible = false;
                    selectedWord = "";
                    helperState = HELPER.EMPTY;
                }
            } else if(wordList.Visible && helperState == HELPER.KEYBOARD){
                /*if(key.IsActionPressed("ui_left")) { //Copy pasted but not editted yet
                    if(playerLine.CaretPosition-1 < beforeWord) {
                        wordList.Clear();
                        wordList.Visible = false;
                        selectedWord = "";
                        helperState = HELPER.EMPTY;
                    }
                } else if(key.IsActionPressed("ui_right")) {
                    if(playerLine.CaretPosition+1 > 
                        beforeWord + selectedWord.Length){
                            if(selectedIncomplete){
                                searchTrie(selectedWord);
                                GetTree().SetInputAsHandled();
                            } else {                    
                                wordList.Clear();
                                wordList.Visible = false;
                                selectedWord = "";
                                helperState = HELPER.EMPTY;
                            }
                    }
                }*/
            } else if(!wordList.Visible) {
                if(key.IsActionPressed("ui_up")) {
                    if(playerLine.Text.Length > 0 && 
                        playerLine.CaretPosition != 0 &&
                            playerLine.Text[playerLine.CaretPosition-1] != ' '){
                        newSearch(playerLine.Text, playerLine.CaretPosition);
                        GetTree().SetInputAsHandled();
                    }
                } else if(key.IsActionPressed("ui_end")){
                    newKeyboard(playerLine.Text, playerLine.CaretPosition);
                    GetTree().SetInputAsHandled();
                }
            }
        }
    }
    //Process Input Map -> Finishing Input Handler
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
    //Entry Input Handler Methods
    public int findSelected(List<string> list){
        int selected = list.Count - 1; 
        //Store current selection
        for(var i = 0; i < list.Count; i++){
            if(wordList.IsSelected(i)){
                selected = i;
            }
        }
        return selected;
    }
    public void selection(List<string> list, int selected, ACTION action = ACTION.UNKNOWN, EFFECT effect = EFFECT.UNKNOWN) {
        List<string> lineWords = currentLine;
        string prevSelection = selectedWord;
        if(action == ACTION.UP || action == ACTION.DOWN){
            //Spin Selection by 1
            if(action == ACTION.UP){
                if(selected - 1 < 0){
                    wordList.Select(list.Count - 1);
                    selected = list.Count - 1;
                } else {
                    wordList.Select(selected - 1);
                    selected += -1;
                }
                GetTree().SetInputAsHandled();
            } else if(action == ACTION.DOWN){
                if(selected + 1 >= list.Count){
                    wordList.Select(0);
                    selected = 0;
                } else {
                    wordList.Select(selected + 1);
                    selected += 1;
                }
                GetTree().SetInputAsHandled();
            }
            if(effect == EFFECT.JUSTSTORE) {
                selectedWord = list[selected];
            } else if(effect == EFFECT.CHANGETEXT){
                selectedWord = list[selected];
                //cleanWord is currently needed for selectionIncomplete bool
                selectedWord = cleanWord(selectedWord);
                if(selectedWord != ""){//Root node is the empty one
                    lineWords[detales[0][4]] = selectedWord;
                } else {//If it bugs and root node becomes selectable.
                    selectedWord = cleanWord(prevSelection);
                }//singleSpace() will crash upon reading it.
                string nextText = playerLine.singleSpace(lineWords);
                playerLine.setText(nextText, detales[0][5] + selectedWord.Length);
            }
            wordList.EnsureCurrentIsVisible();
        }
    }

    //=================Signal Methods=================
    public void _on_PlayerLine_text_changed(string text){
        int caret = playerLine.CaretPosition;
        if(helperState == HELPER.KEYBOARD){
            playerLine.GrabFocus();
            helperState = HELPER.EMPTY;
            keyboard.Visible = false;
            wordList.Visible = false;
            wordList.Clear();
        }
        if(text.Length > 0){
            if((caret > 0 && text[caret-1] == ' ') || caret == 0){
                return;
            }
            //if(helperState == HELPER.KEYBOARD) { //Might include alternating 
            //typing to keep the kb helper when you switch to the 
            //computer keyboard. Might not, the problem is that then I'd have 
            //to make a lot more calculations for where the caret goes and how 
            //space and backspace should behave.
                //newKeyboard(text, caret, true);
                //return;
            //}
            
            newSearch(text, caret);
        }
    }

    public void _on_UI_WordSelected() {
        detales = playerLine.detailedCounter();
        currentLine = playerLine.Text.Split(" ").ToList();
        searchWord = "";
        if(currentLine.Count > 0){
            searchWord = currentLine[detales[0][4]];
            selectedWord = searchWord;
        }
    }

    public void _on_PlayerLine_text_entered(string text) {
        if(debug == true) {
            debugTrie(text);
        }
    }

    public void _on_PlayerLine_text_change_rejected() {

    }
    //Changed Text Signal
    public void newSearch(string text, int caret){
        List<string> lineWords = text.Split(" ").ToList();
        List<List<int>> details = playerLine.detailedCounter();
        playerLine.setText(playerLine.singleSpace(lineWords), caret, details[0][9]);
        EmitSignal(nameof(WordSelected));
        searchTrie(searchWord);
        setHelperPos(playerLine.getStringSize(detales[0][5]), HELPER.LONETRIE);
    }
    public void newKeyboard(string text, int caret, bool oldKeyboard = false){
        List<string> lineWords = text.Split(" ").ToList();
        int exactCaret = 0;
        List<List<int>> details = playerLine.detailedCounter();

        if(text.Length > 0){
            exactCaret = playerLine.setText(playerLine.singleSpace(lineWords), caret,
                                    details[0][9]);
        }
        EmitSignal(nameof(WordSelected));
        keyboard.Visible = true;
        string pText = playerLine.Text;
        if((exactCaret > 0 && pText[exactCaret-1] == ' ')){//> 0 is a error handler            
            searchTrie(searchWord, HELPER.KEYBOARD, true);
            setHelperPos(playerLine.getStringSize(exactCaret), HELPER.KEYBOARD, oldKeyboard);
        } else if(exactCaret > 0 && pText[exactCaret-1] != ' '){
            searchTrie(searchWord, HELPER.KEYBOARD, false);            
            setHelperPos(playerLine.getStringSize(detales[0][5]), HELPER.KEYBOARD, oldKeyboard);
        } else if(exactCaret == 0) {
            searchTrie(searchWord, HELPER.KEYBOARD, true);
            setHelperPos(playerLine.getStringSize(detales[0][5]), HELPER.KEYBOARD, oldKeyboard);
        }
        helperState = HELPER.KEYBOARD;
        if(!oldKeyboard) {
            Button aKey = (Button)keyboard.GetNode("KeyGrid/Aa");
            aKey.GrabFocus();
        }
    }

    public string cleanWord(string word){ //Also marks whether word is complete
        selectedIncomplete = false;
        if(word[word.Length - 1] == '-') {
            selectedIncomplete = true;
            return word.Substr(0, word.Length - 1);
        } else if(word[word.Length - 1] == '~'){
            return word.Substr(0, word.Length - 1);
        }
        return word;
    }//In order to avoid this mutation pattern I could just use WordTrie.find()
    //on the word and also check that the incomplete word is not the last word
    //on the item list (as that would be the origin word that was used to search
    //up the whole list. Incomplete words are at the top and you press right on
    //them to search for them after searching for them from the origin word that
    //was typed.)

    public void setHelperPos(Vector2 textToCaretSize, HELPER helperState, bool oldKeyboard = false) {
        if(helperState == HELPER.LONETRIE){
            wordList.RectPosition = new Vector2(textToCaretSize.x + 1,
                                                wordList.RectPosition.y);
        } else if(helperState == HELPER.KEYBOARD){
            wordList.RectPosition = new Vector2(textToCaretSize.x + 1,
                                    keyboard.RectPosition.y);
            keyboard.RectPosition = new Vector2(textToCaretSize.x +
                                                    wordList.RectSize.x,
                                                    keyboard.RectPosition.y);
        }
    }

    public void searchTrie(string searchWord, HELPER argHelper = HELPER.LONETRIE, bool kbSpace = false){
        //Search Trie for the word the user is currently changing text on.
        predictedText = wordTrie.search(searchWord);
        wordList.Clear();
        if(kbSpace == true || (predictedText == null && argHelper == HELPER.KEYBOARD)){
            GD.Print("No searchable word follows from player's caret pos.");
            wordList.Visible = true;
            wordList.AddItem("~");
            wordList.Select(wordList.GetItemCount() - 1);
            wordList.EnsureCurrentIsVisible();
        } else if(predictedText == null) {//Need to present error handling eventually.
            GD.Print("No searchable word follows from player's text.");
            //wordList.Hide();
            wordList.Visible = false;
        } else {
            //GD.Print("Search: ", searchWord);
            List<string> reversedText = new List<string>();
            for(var i = predictedText.Count - 1; i >= 0; i--){
                //GD.Print("Searched: ", predictedText[i]);
                //wordList.Show();
                reversedText.Add(predictedText[i]);
                wordList.AddItem(predictedText[i], null, true);
            }
            if(reversedText.Count > 0 && //If it's not a complete word
                searchWord.ToLowerInvariant() != reversedText[0].ToLowerInvariant()) {
                    wordList.AddItem(searchWord + "~");
                    reversedText.Add(searchWord + "~");
            } else if(reversedText.Count == 0){ //If it's empty
                wordList.AddItem(searchWord + "~");
                reversedText.Add(searchWord + "~");
            }
            wordList.Visible = true;
            if(argHelper == HELPER.LONETRIE){
                helperState = HELPER.LONETRIE;
            }
            predictedText = reversedText; //Same order for predicted and Item List
            wordList.Select(wordList.GetItemCount() - 1);
            wordList.EnsureCurrentIsVisible();
        }
    }

    //Entered Text Signal
    public void debugTrie(string text) {
        if(text.Length > 0){
            currentLine = text.Split(" ").OfType<string>().ToList();
            if(currentLine.Count > 1){
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
                            wordList.AddItem(predictedText[i], null, true);
                        }
                    }
                }
            }
        }
    }
}

/*

//Old Clean Word Comments 
//Exclusively set by cleanWord()
    //So you can never select a word without using cleanWord() I guess that's
    //Another mutation difficulty but I'm fine with it since it's needed really.    

This goes in _Input() it was for a text helper that initially held
    An alphabet but then I realized I should just use a keyboard instaed
    Then I was planning to use it to store pre set sentences with emojis
    I might still use that idea but for now I don't want to mess with this
    anymore lol If I decide to come back to it the code is here!!!

else if(wordList.Visible && helperState == HELPER.HELPER){
                if(key.IsActionPressed("ui_up")) {
                    selection(textHelper, findSelected(textHelper),
                                        ACTION.UP);
                } else if(key.IsActionPressed("ui_down")) {
                    selection(textHelper, findSelected(textHelper),
                                    ACTION.DOWN);
                }
                if(key.IsActionPressed("ui_left")) {
                    wordList.Clear();
                    wordList.Visible = false;
                    helperState = HELPER.EMPTY;
                } else if(key.IsActionPressed("ui_right")) {
                    getHelper(findSelected(textHelper));
                    GetTree().SetInputAsHandled();
                } else if(key.IsActionPressed("ui_select")) {
                    wordList.Clear();
                    wordList.Visible = false;
                    helperState = HELPER.EMPTY;
                }
            } else if(wordList.Visible && helperState == HELPER.DEEPHELPER){
                int selected = 0;
                for(var i = 0; i < textHelper[deepInx].Length; i++){
                    if(wordList.IsSelected(i)){
                        selected = i;
                    }
                }
                if(key.IsActionPressed("ui_up")) {
                    if(selected - 1 < 0){
                        wordList.Select(textHelper[deepInx].Length - 1);
                        selected = textHelper[deepInx].Length - 1;
                    } else {
                        wordList.Select(selected - 1);
                        selected += -1;
                    }
                    selectedChar = textHelper[deepInx][selected];
                    GetTree().SetInputAsHandled();
                } else if(key.IsActionPressed("ui_down")) {
                    if(selected + 1 >= textHelper[deepInx].Length){
                        wordList.Select(0);
                        selected = 0;
                    } else {
                        wordList.Select(selected + 1);
                        selected += 1;
                    }
                    selectedChar = textHelper[deepInx][selected];
                    
                    string nextText = playerLine.Text;
                    int charValue = 0;
                    if(selectedChar != ' ' && !charAdded) {
                        charAdded = true;
                        charValue = charAdded ? 1 : 0;
                        if(playerLine.Text.Length > 0){
                            nextText = nextText.Substr(0,
                                playerLine.CaretPosition-1) +
                                selectedChar.ToString() +
                                nextText.Substr(playerLine.CaretPosition,
                                    nextText.Length-1);
                        } else {
                            nextText = selectedChar.ToString();
                        }
                    } else if(selectedChar != ' ' && charAdded){
                        charValue = 0;
                        if(playerLine.Text.Length > 0){
                            nextText = nextText.Substr(0,
                                playerLine.CaretPosition-2) +
                                selectedChar.ToString() +
                                nextText.Substr(playerLine.CaretPosition,
                                    nextText.Length-1);
                        } else {//Text length can't be zero after char is added
                            nextText = selectedChar.ToString();
                        }//So this part is unnecessary.
                    } else if(selectedChar == ' ' && charAdded){
                        charAdded = false;
                        charValue = -1; //This part of the code doesn't work yet but I've decided to exclude the text helper for now lol what'dyouknow
                        if(playerLine.Text.Length > 0){//idk if this would work
                            nextText = nextText.Substr(0,//if you're deleting to
                                playerLine.CaretPosition-2) + //Zero
                                nextText.Substr(playerLine.CaretPosition,
                                    nextText.Length-1);
                        } else {//Text length can't be zero after char is added
                            nextText = selectedChar.ToString();
                        }//So this part is unnecessary.
                    }else if(selectedChar == ' ' && !charAdded) {
                        charAdded = false;
                        charValue = 0;
                    }

                    List<string> newWords = nextText.Split(" ").ToList();
                    setText(singleSpace(newWords),
                            globalCaret + charValue,
                            spaceCounter(newWords, globalCaret + charValue));
                    GetTree().SetInputAsHandled();
                }
                wordList.EnsureCurrentIsVisible();

                if(key.IsActionPressed("ui_left")) {
                    getHelper(deepInx);
                    GetTree().SetInputAsHandled();
                } else if(key.IsActionPressed("ui_right")) {
                    wordList.Clear();
                    wordList.Visible = false;
                    helperState = HELPER.EMPTY;
                    GetTree().SetInputAsHandled();
                } else if(key.IsActionPressed("ui_select")) {
                    wordList.Clear();
                    wordList.Visible = false;
                    helperState = HELPER.EMPTY;
                }            
            } else if(key.IsActionPressed("ui_down")){
                    newHelper(playerLine.Text, playerLine.CaretPosition);
                    GetTree().SetInputAsHandled();
                }


                public void getHelper(int selection = -1){ //Not being used for now but maybe it'd be added again later
        wordList.Clear();
        if(selection == -1) {
            for(var i = 0; i < textHelper.Count; i++){
                //wordList.Show();
                wordList.Visible = true;
                wordList.AddItem(textHelper[i], null, true);
            }
            helperState = HELPER.HELPER;
            wordList.Select(textHelper.Count - 1);
        } else if(selection >= 0){
            int storeI = 0;
            for(var i = textHelper.Count - 1; i >= 0; i--){
                if(i == selection){//I was storing strings of ABCDE here before but I decided to add a better Keyboard lol
                    for(var j = textHelper[i].Length - 1; j >= 0; j--){
                        wordList.Visible = true;
                        wordList.AddItem(textHelper[i][j].ToString());
                        storeI = i;
                    }
                }
            }
            deepInx = storeI;
            helperState = HELPER.DEEPHELPER;
            wordList.Select(textHelper[storeI].Length - 1);
        }
        globalCaret = playerLine.CaretPosition;
        wordList.EnsureCurrentIsVisible();
    }

        public void newHelper(string text, int caret){//I meed to read through this
        currentLine = text.Split(" ").ToList();//Thoroughly eventually.
        int exactCaret = setText(singleSpace(currentLine), caret,//To make sure
                spaceCounter(currentLine, caret));//the logic is Flawless 
        getHelper();//and works just the same as Trie
        setwordListPos(exactCaret);
    }
            */