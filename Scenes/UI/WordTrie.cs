using Godot;
using System.Collections.Generic;

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
                TrieNode newNode = new TrieNode(currentLetter);
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
        if(node.completeString){
            predictWords.Add(subword);
        }
        return recursion(node, subword, predictWords);
    }

    public List<string> recursion(TrieNode node, string word, List<string> predictWords) {
        foreach(KeyValuePair<char, TrieNode> child in node.children) {
            if(child.Value.completeString) {
                predictWords.Add(word + child.Value.val);
            }
            recursion(child.Value, word + child.Value.val.ToString(), predictWords);
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

    public TrieNode(char letter = ' '){
        val = letter;
        children = new Dictionary<char, TrieNode>();
        completeString = false;
    }
}