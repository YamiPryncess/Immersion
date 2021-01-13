using Godot;
using System;
//using System.IO;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using JsonLD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class WordNet : Node {
    WnJson ewnJson;
    Master master;
    UI ui;
    WordTrie wordTrie;
    JsonSoft jsonSoft;
    Thread thread;
    string folderPath = @"res://Database/EwnData";
    string pathWn;
    string pathEnt;
    string pathSyn;
    string pathWt;
    File file = new File();
    #region Godot WordNet
    public string label { get; set; }
    public string version { get; set; }
    public string language { get; set; }
    public int procCount = System.Environment.ProcessorCount;
    public ConcurrentDictionary<string, Entry> entry;
    public ConcurrentDictionary<string, Synset> synset;
    #endregion

    public override void _Ready() {
        master = GetTree().Root.GetNode<Master>("Master");
        ui = master.GetNode<UI>("UI");
        wordTrie = ui.GetNode<WordTrie>("WordTrie");
        jsonSoft = new JsonSoft();
        entry = new ConcurrentDictionary<string, Entry>(procCount * 4, 200000);
        synset = new ConcurrentDictionary<string, Synset>(procCount * 4, 200000);
        pathWn = folderPath + @"Ewn-RootInfo.txt";
        pathEnt = folderPath + @"/Ewn-Entries.txt";
        pathSyn = folderPath + @"/Ewn-Synsets.txt";
        pathWt = folderPath + @"/Ewn-Trie.txt";
        //loadWordNet();
    }

    public void createWordNet() {
        assemble();//To create Ent/Syn Dictionaries
        serialize();//Stores those dictionaries
        deserialize();//Deserializes them
        
        fillTrie();
        serializeTrie();
        DeserializeTrie();
    }
    public void createTrie() {//If Ent/Syn Dictionaries exist
        deserialize();//Just deserialize those
        fillTrie();//And go straight to processing the trie
        serializeTrie();
        DeserializeTrie();
    }
    public void loadWordNet() {
        deserialize();
        DeserializeTrie();
    }

    public void assemble() {
        ewnJson = jsonSoft.parseJson();
        if(ewnJson != null) {
            label = ewnJson.label;
            version = ewnJson.version;
            language = ewnJson.language;
            //GD.Print(ewnJson.entry[0]);
            for(var i = 0; i < ewnJson.entry.Length; i++){
                entry.TryAdd(ewnJson.entry[i].id, ewnJson.entry[i]);
            }
            for(var i = 0; i < ewnJson.synset.Length; i++){
                synset.TryAdd(ewnJson.synset[i].id, ewnJson.synset[i]);
            }
        }
    }
    public void fillTrie() {
        foreach(KeyValuePair<string, Entry> child in entry){
            List<string> synsetIds = new List<string>();
            string lemma = child.Value.lemma.writtenForm;
            Form[] forms = child.Value.form;
            string entryId = child.Key;
            for(var i = 0; i < child.Value.sense.Length; i++){
                synsetIds.Add(child.Value.sense[i].synsetRef);
            }
            if(lemma.Contains(" ")) {
                List<string> lemmies = lemma.Split(" ").ToList();
                lemma = "";
                for(var i = 0; i < lemmies.Count; i++){
                    if(i > 0) {
                        lemmies[i] = "_" + lemmies[i];
                    }
                    lemma += lemmies[i];
                }
            }
            if(forms != null){
                for(var i = 0; i < forms.Length; i++) {
                    if(forms[i].writtenForm.Contains(" ")){
                        List<string> formies = forms[i].writtenForm.Split(" ").ToList();
                        forms[i].writtenForm = "";
                        for(var j = 0; j < formies.Count; j++){
                            if(i > 0) {
                                formies[j] = "_" + formies[j];
                            }
                            forms[i].writtenForm += formies[j];
                        }
                    }
                }
            }
            wordTrie.insert(lemma, entryId, synsetIds);
            if(forms != null) {
                for(var i = 0; i < forms.Length; i++) {
                    wordTrie.insert(forms[i].writtenForm, entryId, synsetIds, lemma);
                }
            }
        }
    }

    public void serialize() {
        string entJson = JsonConvert.SerializeObject(entry,
                            Newtonsoft.Json.Formatting.Indented); 
        string synJson = JsonConvert.SerializeObject(synset,
                            Newtonsoft.Json.Formatting.Indented);

        Error entErr = file.Open(pathEnt, File.ModeFlags.Write);
        GD.Print("Ent File Open Status: ", entErr);
        file.StoreString(entJson);
        file.Close();

        Error synErr = file.Open(pathSyn, File.ModeFlags.Write);
        GD.Print("Syn File Open Status: ", synErr);
        file.StoreString(synJson);
        file.Close();
    }

    public void serializeTrie() {
        string trieJson = JsonConvert.SerializeObject(wordTrie.rootNode,
                            Newtonsoft.Json.Formatting.Indented); 
        
        Error trieErr = file.Open(pathWt, File.ModeFlags.Write);
        GD.Print("Trie File Open Status: ", trieErr);
        file.StoreString(trieJson);
        file.Close();
    }

    public void deserialize() {
        bool entExists = file.FileExists(pathEnt);
        Debug.Assert(entExists);
        bool synExists = file.FileExists(pathSyn);
        Debug.Assert(synExists);

        if(entExists && synExists){
            Error entErr = file.Open(pathEnt, File.ModeFlags.Read);
            GD.Print("Ent File Open Status: ", entErr);
            string entJson = file.GetAsText();
            file.Close();

            Error synErr = file.Open(pathSyn, File.ModeFlags.Read);
            GD.Print("Syn File Open Status: ", synErr);
            string synJson = file.GetAsText();
            file.Close();

            entry = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Entry>>(entJson);
            synset = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Synset>>(synJson);

            GD.Print("Test Entry Occultism: ", entry["ewn-occultism-n"].lemma.writtenForm);
            GD.Print("Test Synset Occultism: ", synset["ewn-05977317-n"].definition[0].gloss);
        }
    }
    public void DeserializeTrie() {
        bool trieExists = file.FileExists(pathWt);
        Debug.Assert(trieExists);

        if(trieExists){
            Error trieErr = file.Open(pathWt, File.ModeFlags.Read);
            GD.Print("Trie File Open Status: ", trieErr);
            string trieJson = file.GetAsText();
            file.Close();
            wordTrie.rootNode = JsonConvert.DeserializeObject<TrieNode>(trieJson);
        }
    }
}

class JsonSoft : Reference {
    string folderPath = @"res://Database/WordNet";
    string pathXml;
    string pathJson;
    File file = new File();
    XmlDocument doc = new XmlDocument();
    
    public JsonSoft() {
        pathJson = folderPath + @"/En-WordNet.Json";
        pathXml = folderPath + @"/Ewn-2020.xml";
        // convertXml(pathXml, pathJson);
        // formatJson(pathJson);
    }

    public WnJson parseJson() {
        bool exists = file.FileExists(pathJson);
        Debug.Assert(exists);

        if(exists){
            Error err = file.Open(pathJson, File.ModeFlags.Read);
            GD.Print("File Open Status: ", err);
            string json = file.GetAsText();
            //GD.Print(json);
            //fileIO.Close();
            return JsonConvert.DeserializeObject<WnJson>(json);
            //JObject linqJson = JObject.Parse(json);
            //return linqJson.ToObject<WnJson>();
            //return (WnJson)Godot.JSON.Parse(json).Result;
            
        }
        return null;
    }

    public void convertXml() {
        bool exists = file.FileExists(pathXml);
        Debug.Assert(exists);

        if(exists){
            Error err = file.Open(pathXml, File.ModeFlags.Read);
            GD.Print("File Open Status: ", err);
            string fileStr = file.GetAsText();
            doc.LoadXml(fileStr);
            string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.AppendAllText(pathJson, json);
            //file.StoreString(json);
        }
    }

    public void formatJson() {//Needs to be tested. Idk how to re-write file yet also so I choose to delete it.
        bool exists = System.IO.File.Exists(pathJson);
        Debug.Assert(exists);

        if(exists){
            System.IO.FileStream fileIO =
                System.IO.File.Open(pathJson, System.IO.FileMode.Open);
            string json = fileIO.ToString();
            fileIO.Close();
            JsonConvert.SerializeObject(json,
                Newtonsoft.Json.Formatting.Indented);
            System.IO.File.Delete(pathJson);
            System.IO.File.AppendAllText(pathJson, json);
        }
    }
}  

//Removed Code
    //Old Trie Serialization with Formatter
    // System.IO.FileStream fs = new System.IO.FileStream(pathWt, System.IO.FileMode.Create);
    // try {
    //     BinaryFormatter formatter = new BinaryFormatter();
    //     formatter.Serialize(fs, wordTrie.rootNode);
    // }
    // catch (SerializationException e) {
    //     GD.Print("Failed to deserialize. Reason: " + e.Message);
    //     //throw;
    // }
    // finally {
    //     fs.Close();
    // }
    // System.IO.FileStream fs = new System.IO.FileStream(pathWt, System.IO.FileMode.Open);
    // TrieNode tempTrie = null;
    // try {
    //     BinaryFormatter formatter = new BinaryFormatter();
    //     tempTrie = (TrieNode) formatter.Deserialize(fs);
    //     wordTrie.rootNode = tempTrie;
    // }
    // catch (SerializationException e) {
    //     GD.Print("Failed to deserialize. Reason: " + e.Message);
    //     //throw;
    // }
    // finally {
    //     fs.Close();
    // }
// class StoreWN {
//     public string label { get; set; }
//     public string version { get; set; }
//     public string language { get; set; }
//     public Dictionary<string, Entry> entry = new Dictionary<string, Entry>();
//     public Dictionary<string, Synset> synset = new Dictionary<string, Synset>();

//     public StoreWN(string _label, string _version, string _language, Dictionary<string, Entry> _entry, Dictionary<string, Synset> _synset) {
//         label = _label;
//         version = _version;
//         language = _language;
//         entry = _entry;
//         synset = _synset;
//     }
// }

//Old Serialize/Deserialize Code with Formatter
/*System.IO.FileStream fs = new System.IO.FileStream(pathWn, System.IO.FileMode.Create);
        try {   
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, store);
        }
        catch (SerializationException e) {
            GD.Print("Failed to serialize. Reason: " + e.Message);
            //throw;
        }
        finally {
            fs.Close();
        }*/
    /*System.IO.FileStream fs = new System.IO.FileStream(pathWn, System.IO.FileMode.Open);
        try {
            BinaryFormatter formatter = new BinaryFormatter(); 
            store = (StoreWN) formatter.Deserialize(fs);
            label = store.label;
            version = store.version;
            language = store.language;
            entry = store.entry;
            synset = store.synset;
        }
        catch (SerializationException e) {
            GD.Print("Failed to deserialize. Reason: " + e.Message);
            //throw;
        }
        finally {
            fs.Close();
        }*/

    // public StoreWN(SerializationInfo info, StreamingContext ctxt) {
    //     label = (string)info.GetValue("label", typeof(string));
    //     version = (string)info.GetValue("version", typeof(string));
    //     language = (string)info.GetValue("language", typeof(string));
    //     entry = (Dictionary<string, iEntry>)info.GetValue("entry", typeof(Dictionary<string, iEntry>));
    //     synset = (Dictionary<string, iSynset>)info.GetValue("synset", typeof(Dictionary<string, iSynset>));
    // }
        
    // public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
    //     info.AddValue("label", label);
    //     info.AddValue("version", version);
    //     info.AddValue("language", language);
    //     info.AddValue("entry", entry, typeof(Dictionary<string, iEntry>));
    //     info.AddValue("synset", synset, typeof(Dictionary<string, iSynset>));
    // }
        //Deserialize JSON with Stream
        //         System.IO.FileStream fs = new System.IO.FileStream(pathJson, System.IO.FileMode.Open);
        // try {
        //     return JsonConvert.DeserializeObject<WnJson>(fs.ToString());
        // }
        // catch (SerializationException e) {
        //     GD.Print("Failed to deserialize. Reason: " + e.Message);
        //     return null;
        //     //throw;
        // }
        // finally {
        //     fs.Close();
        // }
        //Serielize WordNet
        // Error err = file.Open(pathWn, File.ModeFlags.Write);
        // GD.Print("File Open Status: ", err);
        // file.StoreVar(store, true);
        // file.Close();
        //Deserialize WordNet
        // Error err = file.Open(pathWn, File.ModeFlags.Read);
        // GD.Print("File Open Status: ", err);
        // StoreWN store = (StoreWN)file.GetVar(true);
        // file.Close();
        //Serialize WordTrie
        // Error err = file.Open(pathWt, File.ModeFlags.Write);
        // GD.Print("File Open Status: ", err);
        // file.StoreVar(wordTrie, true);
        // file.Close();