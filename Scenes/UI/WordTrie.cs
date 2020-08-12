using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

public class WordTrie : Node{
    TrieNode rootNode;

    WordTrie() {
        rootNode = new TrieNode();
    }

    public void insert(string word) {
        TrieNode node = rootNode;

        for(int i = 0; i < word.Length; i++) {
            char currentLetter = word[i];

            if(node.children.ContainsKey(currentLetter)){
                node = node.children[currentLetter];
            } else {
                TrieNode newNode = new TrieNode(currentLetter, i + 1, word.Substr(0, i + 1));
                node.children.Add(currentLetter, newNode);
                node = newNode;
            }
        }
        node.completeString = true;
    }

    public bool find(string word) {
        TrieNode node = rootNode;

        for(int i = 0; i < word.Length; i++) {
            char currentLetter = word[i];
            if(node.children.ContainsKey(currentLetter)){
                node = node.children[currentLetter];
            } else {
                return false;
            }
        }
        return true;
    }

    public TrieNode findWord(string word) {
        TrieNode node = rootNode;

        for(int i = 0; i < word.Length; i++) {
            char currentLetter = word[i];
            if(node.children.ContainsKey(currentLetter)){
                node = node.children[currentLetter];
                if(i == word.Length-1 && node.completeString){
                    return node;
                }
            }
        }
        return rootNode;
    }
    public List<string> search(string subword) {
        TrieNode node = rootNode;
        List<string> predictWords = new List<string>();

        for(int i = 0; i < subword.Length; i++) {
            char currentLetter = subword[i];
            if(node.children.ContainsKey(currentLetter)){
                node = node.children[currentLetter];
            } else {//If the player did not type anything that's familiar to the tree
                return null;//Return null. This may be less strict eventually but for now it must be
            }//perfect spelling. Aside from that only words that the tree knows about can be used.
        }//After getting up to the end of the subword we're going to get ALL words that follow.
        if(node.isRootNode){//This method is not for getting all words.
            return predictWords; //Which is empty.
        }
        
        if(node.completeString){
            predictWords.Add(node.word);
        }
        List<TrieNode> queue = new List<TrieNode>();
        return bFRecursion(node, predictWords, queue);
    }
    public List<string> dFRecursion(TrieNode node, List<string> predictWords) {
        foreach(KeyValuePair<char, TrieNode> child in node.children) { //Idk how consistent dictionary loops are.
            if(child.Value.completeString) { //This is probably unnecessary if I had an actual literal dictionary in the game that the player can just reference for what they currently typed.
                predictWords.Add(child.Value.word); //Could be useful for finding all possible verbs or adjectives or nouns no matter what. I could have that pre-organized in a dictionary though.
            }
            dFRecursion(child.Value, predictWords);
        }
        return predictWords;
    }

    public List<string> bFRecursion(TrieNode node, List<string> predictWords, List<TrieNode> queue, int depthLimit = 3, bool dip = false, List<string> unfinishedWords = null) {
        if(unfinishedWords == null) {
            unfinishedWords = new List<string>();
        }
        if((depthLimit > 0 || depthLimit == -1)){ //-1 for infinite depth
            foreach(KeyValuePair<char, TrieNode> child in node.children) {//Add ALL breadth of children in this depth
                if(child.Value.completeString) {
                    predictWords.Add(child.Value.word);
                } else if(depthLimit == 1) { //We can store unfinished paths in the last depth.
                    unfinishedWords.Add(child.Value.word + "-");
                }
                
                if(child.Value.children.Count > 0){
                    queue.Add(child.Value);
                }
            }
            if(!dip) {
                for(var i = 0; i < queue.Count; i++) {
                    bFRecursion(queue[i], predictWords, queue, Mathf.Max(0, depthLimit - (queue[i].depth - node.depth)), true, unfinishedWords);
                }
                if (unfinishedWords.Count > 0){
                    for(var i = 0; i < unfinishedWords.Count; i++){
                        predictWords.Add(unfinishedWords[i]);
                    }
                }
            } 
        }
        return predictWords;
    }

    public string remove(string word) {
        TrieNode node = rootNode;
        List<TrieNode> suffixes = new List<TrieNode>();

        for(int i = 0; i < word.Length; i++) {
            char currentLetter = word[i];
            if(node.children.ContainsKey(currentLetter)){
                node = node.children[currentLetter];
                suffixes.Insert(0, node);
            }
            if(i == word.Length - 1 && node.children.Count > 0){
                GD.Print("This word \"" + word + "\" has suffixes that depend on it.");
               return ""; //Supposed to throw an error here but I removed it.
            }
        }
        for (int k = 1; k < suffixes.Count; k++){ //Apples, suffixes[s,e,l,p,p,A]
            TrieNode parent = suffixes[k];  //k starts at "e", parent.children will contain "s"
            char childLetter = word[suffixes.Count - k];
            if(parent.children.ContainsKey(childLetter)){
                parent.children.Remove(childLetter);//parent.children will delete s then e in next loop
            }
            if(parent.completeString || parent.children.Count > 0){ //Apple is the end of a word so it'd stop here
                GD.Print("some suffixes of \""+word+"\" removed from trie."); //If Apple vs Apples didn't count then
                return "";
            }//it'd stop at the letter l if you have a child branch for the word :Application" and so on.
        }//I have to store conjugations in here unfortunately.
        if(rootNode.children.ContainsKey(word[0])){//Root never gets in the parent variable
                    rootNode.children.Remove(word[0]); //The first letter is the last parent instead
        }//Since the last parent's children was already checked. We can just delete the lone wolf.
        GD.Print("removed \"" + word + "\" node other conjugations of \"" + word + "\" words may remain."); 
        return "";
    }
}

public class TrieNode {
    public char val;
    public Dictionary<char, TrieNode> children;
    public bool completeString;
    public int depth = 0;
    public string word = "";
    public bool isRootNode = false;

    public TrieNode(char letter = ' ', int _depth = 0, string _word = ""){
        val = letter;
        depth = _depth;
        word = _word;
        completeString = false;

        if(letter == ' ' && _depth == 0 && _word == ""){
            isRootNode = true;
            children = new Dictionary<char, TrieNode>(new RootComparer());
        } else {
            children = new Dictionary<char, TrieNode>(new CharComparer());
        }
    }
}

public class CharComparer : IEqualityComparer<char>
{
     public bool Equals(char c1, char c2)
     {
          return char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);
     }
     public int GetHashCode(char c1)
     {
          return char.ToLowerInvariant(c1).GetHashCode();
     }

}

public class RootComparer : IEqualityComparer<char>
{
     public bool Equals(char c1, char c2)
     {
          return char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2);
     }
     public int GetHashCode(char c1)
     {
          return char.ToUpperInvariant(c1).GetHashCode();
     }

}