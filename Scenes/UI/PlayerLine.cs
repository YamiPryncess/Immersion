using Godot;
using System;
using System.Collections.Generic;

public class PlayerLine : LineEdit
{
    Font playerFont; //To measure font, requires currently used font
    public override void _Ready()
    {
        playerFont = GetFont(""); //Needs actual current font        
    }

//  public override void _Process(float delta)
//  {
//      
//  }

    public int setText(string nextText, int caret, int removedSpaces = 0){
        //Reset the text
        Text = nextText;
        CaretPosition = caret - removedSpaces;
        return CaretPosition;
    }

    public Vector2 getStringSize(int tillWordAtCaret) {
        Vector2 textToCaretSize = Vector2.Zero;
        //Move Item List to the word that the caret is on.
        if(Text.Length > 0){
            textToCaretSize = playerFont.GetStringSize(Text.Substr(0,
                                                tillWordAtCaret));
        }
        return textToCaretSize;
    }

    public string singleSpace(List<string> lineWords) {
        string text = "";//if I remove singleSpace then I'll have to make more
        string lineText = Text;//Code to handle backspace behavior
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

        string text = Text;
        int caret = CaretPosition;

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
}

//Old Comments and work that aren't needed anymore.

/*public int spaceCounter(List<string> lineWords, int caret){//For deleting spaces to make a new caret position
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
                    //A new Item List is to be created from this info. */

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