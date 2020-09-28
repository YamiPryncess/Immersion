using Godot;
using System;

//WnJson uses entry array and synset which isn't useful.
//That's why StoreWn was made with a dictionary in place of it.
public class WnJson { //This class is only used for JSON.
    public string email { get; set; }
    public string url { get; set; }
    public Entry[] entry { get; set; }
    public string label { get; set; }
    public string confidenceScore { get; set; }
    public string license { get; set; }
    public Synset[] synset { get; set; }
    public string version { get; set; }
    public string language { get; set; }
    public string type { get; set; }
    public Context context { get; set; }
    public string id { get; set; }
}
public class Context { //This is only used in the initial JSON
    public string language { get; set; }
}
public class Entry { //StoreWN, WordNet, WnJson instance this class
    public Lemma lemma { get; set; }
    public string partOfSpeech { get; set; }
    public string id { get; set; }
    public Sense[] sense { get; set; }
    public Form[] form { get; set; }
    public Synbehavior[] synBehavior { get; set; }
}
public class Lemma {
    public string writtenForm { get; set; }
}
public class Sense {
    public string id { get; set; }
    public string synsetRef { get; set; }
    public Relation[] relations { get; set; }
    public string identifier { get; set; }
}
public class Relation {
    public string relType { get; set; }
    public string target { get; set; }
}
public class Form {
    public string writtenForm { get; set; }
}
public class Synbehavior {
    public string label { get; set; }
    public string[] senses { get; set; }
}
public class Synset { //StoreWN, WordNet, WnJson instance this class
    public string subject { get; set; }
    public Relation[] relations { get; set; }
    public Definition[] definition { get; set; }
    public Example[] example { get; set; }
    public string ili { get; set; }
    public string partOfSpeech { get; set; }
    public string id { get; set; }
    public Ilidefinition iliDefinition { get; set; }
    public string source { get; set; }
}
public class Ilidefinition {
    public string gloss { get; set; }
}
public class Definition {
    public string gloss { get; set; }
}
public class Example {
    public string value { get; set; }
    public string source { get; set; }
}


//Removed Code
// //using System.Runtime.Serialization;

// [Serializable()]
// public class iEntry : ISerializable {
//     public iLemma lemma { get; set; }
//     public string partOfSpeech { get; set; }
//     public string id { get; set; }
//     public iSense[] sense { get; set; }
//     public iForm[] form { get; set; }
//     public iSynbehavior[] synBehavior { get; set; }
//     public iEntry(SerializationInfo info, StreamingContext ctxt) {
//         lemma = (iLemma)info.GetValue("lemma", typeof(iLemma));
//         partOfSpeech = (string)info.GetValue("partOfSpeech", typeof(string));
//         id = (string)info.GetValue("id", typeof(string));
//         sense = (iSense[])info.GetValue("sense", typeof(iSense[]));
//         form = (iForm[])info.GetValue("form", typeof(iForm[]));
//         synBehavior = (iSynbehavior[])info.GetValue("synBehavior", typeof(iSynbehavior[])); 
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("lemma", lemma, typeof(iLemma));
//         info.AddValue("partOfSpeech", partOfSpeech);
//         info.AddValue("id", id);
//         info.AddValue("sense", sense, typeof(Sense[]));
//         info.AddValue("form", form, typeof(Form[]));
//         info.AddValue("synBehavior", synBehavior, typeof(Synbehavior[])); 
//     }
// }

// [Serializable()]
// public class iLemma : ISerializable {
//     public string writtenForm { get; set; }
//     public iLemma(SerializationInfo info, StreamingContext ctxt) {
//         writtenForm = (string)info.GetValue("writtenForm", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("writtenForm", writtenForm); 
//     }
// }

// [Serializable()]
// public class iSense : ISerializable {
//     public string id { get; set; }
//     public string synsetRef { get; set; }
//     public iRelation[] relations { get; set; }
//     public string identifier { get; set; }

//     public iSense(SerializationInfo info, StreamingContext ctxt) {
//         id = (string)info.GetValue("id", typeof(string));
//         synsetRef = (string)info.GetValue("synsetRef", typeof(string));
//         relations = (iRelation[])info.GetValue("relations", typeof(iRelation[]));
//         identifier = (string)info.GetValue("identifier", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("id", id);
//         info.AddValue("synsetRef", synsetRef);
//         info.AddValue("relations", relations, typeof(Relation[]));
//         info.AddValue("identifier", identifier);
//     }
// }

// [Serializable()]
// public class iRelation : ISerializable {
//     public string relType { get; set; }
//     public string target { get; set; }
//     public iRelation(SerializationInfo info, StreamingContext ctxt) {
//         relType = (string)info.GetValue("relType", typeof(string));
//         target = (string)info.GetValue("target", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("relType", relType);
//         info.AddValue("target", target);
//     }
// }

// [Serializable()]
// public class iForm : ISerializable {
//     public string writtenForm { get; set; }
//     public iForm(SerializationInfo info, StreamingContext ctxt) {
//         writtenForm = (string)info.GetValue("writtenForm", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("writtenForm", writtenForm);
//     }
// }

// [Serializable()]
// public class iSynbehavior : ISerializable {
//     public string label { get; set; }
//     public string[] senses { get; set; }
//     public iSynbehavior(SerializationInfo info, StreamingContext ctxt) {
//         label = (string)info.GetValue("label", typeof(string));
//         senses = (string[])info.GetValue("senses", typeof(string[]));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("label", label);
//         info.AddValue("senses", senses);
//     }
// }

// [Serializable()]
// public class iSynset : ISerializable {
//     public string subject { get; set; }
//     public iRelation[] relations { get; set; }
//     public iDefinition[] definition { get; set; }
//     public iExample[] example { get; set; }
//     public string ili { get; set; }
//     public string partOfSpeech { get; set; }
//     public string id { get; set; }
//     public iIlidefinition iliDefinition { get; set; }
//     public string source { get; set; }
//     public iSynset(SerializationInfo info, StreamingContext ctxt) {
//         subject = (string)info.GetValue("subject", typeof(string));
//         relations = (iRelation[])info.GetValue("relations", typeof(iRelation[]));
//         definition = (iDefinition[])info.GetValue("definition", typeof(iDefinition[]));
//         example = (iExample[])info.GetValue("example", typeof(iExample[]));
//         ili = (string)info.GetValue("ili", typeof(string));
//         partOfSpeech = (string)info.GetValue("partOfSpeech", typeof(string));
//         id = (string)info.GetValue("id", typeof(string));
//         iliDefinition = (iIlidefinition)info.GetValue("iliDefinition", typeof(iIlidefinition));
//         source = (string)info.GetValue("source", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("subject", subject);
//         info.AddValue("relations", relations, typeof(Relation[]));
//         info.AddValue("definition", definition, typeof(Definition[]));
//         info.AddValue("example", example, typeof(Example[]));
//         info.AddValue("ili", ili);
//         info.AddValue("partOfSpeech", partOfSpeech);
//         info.AddValue("id", id);
//         info.AddValue("iliDefinition", iliDefinition, typeof(Ilidefinition));
//         info.AddValue("source", source);
//     }
// }

// [Serializable()]
// public class iIlidefinition : ISerializable {
//     public string gloss { get; set; }
//     public iIlidefinition(SerializationInfo info, StreamingContext ctxt) {
//         gloss = (string)info.GetValue("gloss", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("gloss", gloss);
//     }
// }


// [Serializable()]
// public class iDefinition : ISerializable {
//     public string gloss { get; set; }
//     public iDefinition(SerializationInfo info, StreamingContext ctxt) {
//         gloss = (string)info.GetValue("gloss", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("gloss", gloss);
//     }
// }


// [Serializable()]
// public class iExample : ISerializable {
//     public string value { get; set; }
//     public string source { get; set; }

//     public iExample(SerializationInfo info, StreamingContext ctxt) {
//         value = (string)info.GetValue("value", typeof(string));
//         source = (string)info.GetValue("source", typeof(string));
//     }
        
//     public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
//         info.AddValue("value", value);
//         info.AddValue("source", source);
//     }
// }
/*[Serializable()]
public class WnJson : ISerializable {
    public string email { get; set; }
    public string url { get; set; }
    public Entry[] entry { get; set; }
    public string label { get; set; }
    public string confidenceScore { get; set; }
    public string license { get; set; }
    public Synset[] synset { get; set; }
    public string version { get; set; }
    public string language { get; set; }
    public string type { get; set; }
    public Context context { get; set; }
    public string id { get; set; }
    public WnJson(SerializationInfo info, StreamingContext ctxt) {
        email = (string)info.GetValue("email", typeof(string));
        url = (string)info.GetValue("url", typeof(string));
        entry = (Entry[])info.GetValue("entry", typeof(Entry[]));
        label = (string)info.GetValue("label", typeof(string));
        confidenceScore = (string)info.GetValue("confidenceScore", typeof(string));
        license = (string)info.GetValue("license", typeof(string));
        synset = (Synset[])info.GetValue("synset", typeof(Synset[]));
        version = (string)info.GetValue("version", typeof(string));
        language = (string)info.GetValue("language", typeof(string));
        type = (string)info.GetValue("type", typeof(string));
        context = (Context)info.GetValue("context", typeof(Context));
        id = (string)info.GetValue("id", typeof(string));
    }
        
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
        info.AddValue("email", email);
        info.AddValue("url", url);
        info.AddValue("entry", entry, typeof(Entry[]));
        info.AddValue("label", label);
        info.AddValue("confidenceScore", confidenceScore);
        info.AddValue("license", license);
        info.AddValue("synset", synset, typeof(Synset[]));
        info.AddValue("version", version);
        info.AddValue("language", language);
        info.AddValue("type", type);
        info.AddValue("context", context, typeof(Context));
        info.AddValue("id", id, typeof(string));
    }
}*/
/*public class RootObject : Reference {
    public string context { get; set; }
    public Graph[] graph { get; set; }
}
public class Graph : Reference {
    public string email { get; set; }
    public string url { get; set; }
    public Entry[] entry { get; set; }
    public string label { get; set; }
    public string confidenceScore { get; set; }
    public string license { get; set; }
    public Synset[] synset { get; set; }
    public string version { get; set; }
    public string language { get; set; }
    public string type { get; set; }
    public Context context { get; set; }
    public string id { get; set; }
}
*/
/*[Serializable()]
public class Context : ISerializable {
    public string language { get; set; }
    public Context(SerializationInfo info, StreamingContext ctxt) {
        language = (string)info.GetValue("language", typeof(string));
    }
        
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
        info.AddValue("language", language); 
    }
}*/