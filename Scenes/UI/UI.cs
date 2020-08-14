using Godot;
using System.Linq;
using System.Collections.Generic;

public class UI : Control
{   //Objects (Functionality from outside this class)
    

    Master master; //Changes player control state
    LineEdit playerLine; //To change the Line Edit Text
    Font playerFont; //To measure font, requires currently used font
    ItemList wordList; //To display predictive text
    WordTrie wordTrie; //To predict text (Can also store new words)
    //Unchanging Structures (Internal Functionality)
    Panel keyboard;
    public enum HELPER{ LONETRIE, EMPTY, KEYBOARD }
    HELPER helperState = HELPER.EMPTY;
    public enum ACTION{ UP, DOWN, LEFT, RIGHT, UNKNOWN }
    public enum EFFECT{ CHANGETEXT, JUSTSTORE, UNKNOWN}
    List<string> textHelper;//Not being used anymore

    //In Godot there are multiple processes that code is transfered between.
    //I took a Functional Programming Approach for this script.
    //Caution 1
    //Because functions are reusable if I were to mutate these variables in them
    //Then use the functions in the wrong order the code would break.
    //Since variables in 1 function depend on variables from another.

    //Caution 2
    //If a variable gets set at a particular event. It shouldn't change until
    //That event happens again. If you reuse the function that sets it
    //In a place other than than where it's set in the event the code will also
    //break. Regardless if it's in the same process or another process.
    
    //Caution 3
    //Upon making the selectedIncomplete Variable I realized that
    //If I don't use the function that sets the variable every time the
    //necessary event happens. Then I won't have the variable set as it
    //needs to be.

    //To prevent that I made a mutation rules for functions that:

    //1 Are self contained with the event where mutation should happen
    //For example setText() should change caret position every single time.
    //By doing that you can record the recent caret position even if the player
    //Moves it without typing.

    //2 Aren't self contained and could be reused and changed by mistake.
    //In this case there is a Mutation Option on those functions that can be
    //Switched on and off to say whether this was the intended call to set those
    //Variables or if it wasn't. The only time originWord should be changed is
    //When something new is typed for example. findCurWord() changes it upon
    //text change which is cool but when findCurWord is called in _Input()
    //It should not be changed so you switch the MO to off.
    
    //Central Structures (No M.O. because they change only when they should.
    List<string> predictedText; //By cycle I mean the next time the player types
    List<string> currentLine; //We store what the player types in here
    int globalCaret; //I made the newSearch code not depend on this anymore.
    //int deepInx; //I was using this to access contents inside of text helper
    bool charAdded = false; //Stored to check whether value is already 1 or not
    string selectedWord = ""; //Only used when _Input() needs it.
    char selectedChar = ' ';//Because calling char's words bothers me lol
    bool selectedIncomplete = false; //Exclusively set by cleanWord()
    //So you can never select a word without using cleanWord() I guess that's
    //Another mutation difficulty but I'm fine with it since it's needed really.

    //Mutating Variables (M.O. because they could change when they shouldn't)
    List<List<int>> detales;

    //Class Utility
    [Export] bool debug = false;

    //==================GODOT VIRTUALS======================
    public override void _Ready(){
        master = GetTree().Root.GetNode<Master>("Master");
        playerLine = (LineEdit)GetNode("PlayerLine");
        playerFont = playerLine.GetFont(""); //Needs actual current font
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

        textHelper = new List<string>(){
            "Shortcuts2",
            "Shortcuts1",
            "Fear",
            "Anger",
            "Concern",
            "Sadness",
            "Happiness",
            "Assertions",
            "Questions",
            "Commands",
            "Judgement",
            "Feelings"
        };

        /*feelings = new List<string>(){
            "Shortcuts3",
            "Shortcuts2",
            "Shortcuts1",
            "Assertions",
            "Questions",
            "Commands",
            "Judgement",
            "I'm glad. :)"
        };*/

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
                string nextText = singleSpace(lineWords);
                setText(nextText, detales[0][5] + selectedWord.Length);
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

    public void _on_PlayerLine_CaretChanged() {
        /*List<string> lineWords = playerLine.Text.Split(" ").ToList();
        originWord = cleanWord(lineWords[i]);
        selectedWord = cleanWord(lineWords[i]);*/
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
        currentLine = text.Split(" ").ToList();
        List<List<int>> details = detailedCounter();
        setText(singleSpace(currentLine), caret, details[0][9]);
        details = detailedCounter();
        currentLine = playerLine.Text.Split(" ").ToList();
        string currentWord = "";
        if(currentLine.Count > 0){
            currentWord = currentLine[details[0][4]];
            selectedWord = currentLine[details[0][4]];
        }
        searchTrie(currentWord);
        setHelperPos(getStringSize(details[0][5]), HELPER.LONETRIE);
        detales = details;
    }
    public void newKeyboard(string text, int caret, bool oldKeyboard = false){
        currentLine = text.Split(" ").ToList();
        int exactCaret = 0;
        List<List<int>> details = detailedCounter();

        if(text.Length > 0){
            exactCaret = setText(singleSpace(currentLine), caret,
                                    details[0][9]);
        }
        keyboard.Visible = true;
        string pText = playerLine.Text;
        currentLine = playerLine.Text.Split(" ").ToList();
        string currentWord = "";
        if(currentLine.Count > 0){
            currentWord = currentLine[details[0][4]];
            selectedWord = currentLine[details[0][4]];
        }
        if((exactCaret > 0 && pText[exactCaret-1] == ' ')){//> 0 is a error handler            
            searchTrie(currentWord, HELPER.KEYBOARD, true);
            setHelperPos(getStringSize(exactCaret), HELPER.KEYBOARD, oldKeyboard);
        } else if(exactCaret > 0 && pText[exactCaret-1] != ' '){
            searchTrie(currentWord, HELPER.KEYBOARD, false);            
            setHelperPos(getStringSize(details[0][5]), HELPER.KEYBOARD, oldKeyboard);
        } else if(exactCaret == 0) {
            searchTrie(currentWord, HELPER.KEYBOARD, true);
            setHelperPos(getStringSize(details[0][5]), HELPER.KEYBOARD, oldKeyboard);
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

    public List<List<int>> detailedCounter(bool selectBehindWord = true){        
        int b4CaretSpaces = 0; //return value 0
        
        int introSpaces = 0; //return value 1
        int middleSpaces = 0; //return value 2
        int endSpaces = 0; //return value 3

        bool wordInxBool = false;
        int wordInx = 0; //return value 4
        int tillWord = 0; //return value 5
        int b4CaretLetters = 0; //return value 6
        int allLetters = 0; //return value 7

        bool b4CaretCleanSpaced = true; //return value 8
        int b4CaretSingles = 0;

        int inbetweenSpaces = 0;
        int spacesToRemove = 0; //return value 9

        List<int> results;
        List<int> wordPos = new List<int>();
        bool wordPosBool = false; //Does the same thing as wordInx bool but just to be safe.

        string text = playerLine.Text;
        int caret = playerLine.CaretPosition;

        if(text.Length > 0){
            for(var i = 0; i < text.Length; i++){
                if(i == introSpaces && text[i] == ' '){
                    introSpaces++;//Count Intro Spaces
                    b4CaretCleanSpaced = false;//Opening spaces means text isn't cleanly spaced
                    if(text.Length - 1 == i){//There are no letters till end
                        endSpaces = -1;//End & Start spaces shouldn't be the same
                    }
                }
                
                if(text[i] == ' ') {
                    endSpaces++;//End Spaces will be sent to middle spaces
                    b4CaretSingles++;//Is counting if words are cleanly spaced

                    if(i + 1 == caret) {
                        if(b4CaretSingles > 1){//Found caret in spaces & unclean
                            b4CaretCleanSpaced = false;
                        }//Double Space != Cleanly Spaced
                        b4CaretSpaces += b4CaretSingles;//Add to total b4Caret
                        b4CaretSingles = 0;//Keep counting until before caret
                    }
                    if(i < caret && wordInxBool == true) {
                        wordInxBool = false;//You're not on a word anymore
                    }
                    if(wordPosBool == true) {
                        wordPosBool = false;//Same as wordInx, working separately
                    }
                }
                
                if(text[i] != ' ') {
                    if(introSpaces != i) {//Middle should not get intro spaces
                        middleSpaces += endSpaces;
                    }
                    endSpaces = 0;//Keep looking for the end or give to middle

                    if(i < caret) { //While still behind caret
                        if(b4CaretSingles > 1){//Just came from spaces & unclean
                            b4CaretCleanSpaced = false;
                        }
                        b4CaretSpaces += b4CaretSingles;
                        b4CaretSingles = 0;//Add to before spaces and continue
                    }
                    if(i < caret && wordInxBool == false) {
                        wordInxBool = true;
                        wordInx++;//If caret is in or right after current word
                    } else if((i == caret && selectBehindWord) &&
                                        wordInxBool == false){
                        wordInxBool = true;
                        wordInx++;//If caret is right behind 1st letter
                    }
                    if(i < caret) {//Count all letters
                        b4CaretLetters++;
                        allLetters++;
                    } else {
                        allLetters++;
                    }
                    if(wordPosBool == false) {
                        wordPos.Add(i);//If on first letter
                        wordPosBool = true;
                    }
                }
            }
        }
        wordInx = wordInx == 0 ? 0 : wordInx-1;
        tillWord = wordPos.Count > 0 ? (wordPos[wordInx] > 0 ? wordPos[wordInx] : 0) : 0;
        inbetweenSpaces = (wordInx > 1) ? wordInx : wordInx;
        spacesToRemove = b4CaretSpaces - inbetweenSpaces;
        results = new List<int>(){b4CaretSpaces, introSpaces, middleSpaces, endSpaces, wordInx, tillWord, b4CaretLetters, allLetters, b4CaretCleanSpaced ? 1 : 0, spacesToRemove};
        /*GD.Print("Details-\n", "\nb4CaretSpaces: ", b4CaretSpaces,
        "\nintroSpaces: ", introSpaces, "\nmiddleSpaces: ", middleSpaces,
        "\nendSpaces: ", endSpaces, "\nwordInx: ", wordInx,
        "\ntillWord: ", tillWord, "\nb4CaretLetters: ", b4CaretLetters,
        "\nallLetters: ", allLetters, "\nb4CaretCleanSpaced: ", b4CaretCleanSpaced ? 1 : 0,
        "\nSpacesToRemove: ", spacesToRemove);*/
        /*GD.Print("Word Positions");
        for(var i = 0; i < wordPos.Count; i++) {
            GD.Print(wordPos[i]);
        }*/

        return new List<List<int>>(){results, wordPos};
    }

    public string singleSpace(List<string> lineWords) {
        string text = "";//if I remove singleSpace then I'll have to make more
        string lineText = playerLine.Text;//Code to handle backspace behavior
        if(lineWords.Count > 0 && lineText.Length > 0) {//backspace at the
            for(var i = 0; i < lineWords.Count; i++){//beginning of a word
                if(i == 0 && lineWords[i].Length > 0){//won't reset the listPos
                    text += lineWords[i];
                } else if(lineWords[i].Length > 0){
                    text += " " + lineWords[i];
                } else {
                    //handle the error? abort everything???
                    //for now let's just leave it like this
                    //I think it'll count for the previous word
                    //If there is no previous word idk what
                    //would happen though
                }
            }
            if(lineText[lineText.Length - 1] == ' '){
                text += " ";
            }
        }
        return text;
    }

    public int setText(string nextText, int caret, int removedSpaces = 0){
        //Reset the text
        playerLine.Text = nextText;
        playerLine.CaretPosition = caret - removedSpaces;
        //EmitSignal(nameof(CaretChanged));
        return playerLine.CaretPosition;
    }
    public Vector2 getStringSize(int tillWordAtCaret) {
        Vector2 textToCaretSize = Vector2.Zero;
        //Move Item List to the word that the caret is on.
        if(playerLine.Text.Length > 0){
            textToCaretSize = playerFont.GetStringSize(playerLine.Text.Substr(0,
                                                tillWordAtCaret));
        }
        return textToCaretSize;
    }

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

/*     public int spaceCounter(List<string> lineWords, int caret){//For deleting spaces to make a new caret position
        int spacesTillCaret = 0;
        string lineText = playerLine.Text;
        bool space = true;
        bool startCounting = true; //Skips the first space after each word
        int countInd = 0;

        if(lineWords.Count > 0){//Depends on currentline not being empty
            //Count the text
            for(var i = 0; i < lineText.Length; i++){
                if(lineText[i] != ' ' && space == true){
                    //wordCount[countInd] = i;
                    space = false;
                } else if(lineText[i] == ' ' && space == false) {
                    space = true;
                    startCounting = true;//Skip the first space after each word
                    countInd++;
                } else if(startCounting == true) { //Only count beginning spaces and 2 spaces after each word.
                    if(space == true && i < caret) {
                        spacesTillCaret++;
                    }
                }
            }
        }
        return spacesTillCaret;
    }    

public string[] findCurWord(bool certainSpacing = false) { //if certain spacing is true that means we're certain it's single spaced.
        List<string> lineWords = playerLine.Text.Split(" ").ToList();
        int caret = playerLine.CaretPosition;
        string currentWord = ""; //This method is dependent on proper spacing at the moment. Idk if I'll keep it like that.
        List<int> wordCount = new List<int>();
        int tillWordAtCaret = 0;
        int lastIndex = 0;
        
        if(lineWords.Count > 0){//Depends on lineWords not being empty
            //Find the caret position and check what word it's on.
            for(var i = 0; i < lineWords.Count; i++){
                if(tillWordAtCaret + lineWords[i].Length + 1 < caret){
                    tillWordAtCaret += lineWords[i].Length + 1;
                } else {
                    lastIndex = i;
                    currentWord = lineWords[i];
                    break;
                }
            }
        }
        return new string[]{currentWord, tillWordAtCaret.ToString(), lastIndex.ToString()};
    }

comments from find cur word

                //Caution, Spaces in front of words count as previous word
                //and not following word even if the caret was touching the
                //following word from behind. + 1 counts for 1 separating space.
                //I might change this later to count the space behind instead of in front.

//To return to other functions
                    if(mutate) { //If I called this function for a word that
                    //wasn't the original I'd mess up the originWord variable.
                    //That's why not mututing external stuff in functions is
                    //so important. We only store it for the whole class when
                    //A new Item List is to be created from this info. 


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

/*//Vector2 textSize = Vector2.Zero; //Originally intended to limit
        //How much the user can type by checking via the StringSize
        //Rather than by the amount of characters.

        //textSize = playerFont.GetStringSize(playerLine.Text);
        
        //Yikes we might have to word count how many letters to
        //delete by calling the size over and over again. Deleting
        from the end wouldn't be cool though. I need a new algorithm that
        //doesn't limit spaces and instead checks for the spot where the
        //user last typed at and instead deletes that character. Idk if
        //there is some sort of glitch they can do to make it multiple
        //characters past size though lol */


/*                    selectedWord = predictedText[selected];
                    string selectedText = newText.Substr(0, sentenceLengthTillWordAtCaret-1);
                    selectedText += selectedWord;
                    selectedText += newText.Substr(sentenceLengthTillWordAtCaret +
                                    selectedWord.Length - 1, newText.Length - 1);
                    newText = selectedText;
                    playerLine.Text = selectedText;
                    playerLine.CaretPosition = sentenceLengthTillWordAtCaret + selectedWord.Length - 1;
                    
                    selectedWord = predictedText[selected];
                    string selectedText = newText.Substr(0, sentenceLengthTillWordAtCaret-1);
                    selectedText += selectedWord;
                    selectedText += newText.Substr(sentenceLengthTillWordAtCaret +
                                    selectedWord.Length - 1, newText.Length - 1);
                    newText = selectedText;
                    playerLine.Text = selectedText;
                    playerLine.CaretPosition = sentenceLengthTillWordAtCaret + selectedWord.Length - 1;*/

                    
    /*  string newText = nextText;
        int caretSpace = 0;
            public string includePrecedingSpace(string nextText, int prevCaret) {
        string lineText = playerLine.Text;
        string newText = "";
        // if(prevCaret != 1 && lineText[0] != ' '){
            
        // }
        
        // if((nextText.Length == prevCaret &&
        //             lineText[nextText.Length - 1] == ' ') ||
        //                 (nextText.Length + 1 == prevCaret) &&
        //                     lineText[nextText.Length] == ' ') {
                                
        // }
        return newText;
    }
        if(allowCaretSpace == true) {
            newText = includePrecedingSpace(nextText, prevCaret);
            if(newText.Length > nextText.Length) {
                caretSpace++;
            } 
        } 
    
    
    includeStartingSpace(lineText, prevCaret)
        
        else if(i != lineWords.Count - 1 &&
                includePrecedingSpace(text, lineText, prevCaret)) { 
                    text += "  " + lineWords[i];
            }*/