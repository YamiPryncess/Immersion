// using Godot;
// using System.Collections.Generic;

// public class Person : KinematicBody {//So other characters need to form an opinion about other characters.
// 	public string name = ""; //Fullnames should be unique.
// 	public string personType = "";
//     public Resource inventory;
//     Dictionary<string, string> facts = new Dictionary<string, string>();
//     //Details of appearence, personality, hobbies for others to talk about.
//     public bool resolved = false;

// 	//public EventLog myLog;
//     //public Dictionary<string, EventLog> feelings;//Feelings, beliefs, & opinions about other things.
// 	public Dictionary<string, int> needs; //I need to write code to form and permeate opinions.

//     private void addNeeds() {
//         needs.Add("Consistency", 0);
//         needs.Add("Socialization", -50);
//         needs.Add("Appetite", 0);
//     }

//     public enum STATE {
//         JOYOUS, SAD
//     }

//     public enum SUBJECT {
//         PERSON, BEHAVIOR, OBJECT, CONCEPT, SELF
//     }
//     public STATE emotion; 
//     protected SUBJECT focus;

//     public Person(){//GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player
//         //stage = EVENT.ENTER;
//         needs = new Dictionary<string, int>();

//         // feelings = new Dictionary<string, EventLog>();//Feeling || class-type-id, new Believed Events
// 	   //Instead of preloading a long dictionary for every log I could use SQLite to fetch info
// 	  //So EventLog would be the only SQLite database file.
// 	 //Sqlite will capture changes like hair color change. Classes here will store changes.
//     }

//     public override void _Ready() {
//         addNeeds();
        
// 	    inventory = ResourceLoader.Load("res://Scenes/Character/Essentials/Inventory.gd", "Resource", true);
//         GD.Print(inventory);
//         }

// 	public override void _Process(float delta) {
//         if(!resolved) {
//             //Goap Actions?
//             Resolve();
//         } else {
//             //Store information associated with resolution
//         }
//     }
//     public virtual void Resolve() { //Will call the Goap Agent.
//         //Will probably recieve certain types of actions from each emotion type.

//     }
// }

// // public class Profile {
// //     protected string believedName = "";
// //     public Person trueId;
// //     protected Type type;
// //     protected string subtype = "";
// //     Dictionary<string, string> body = new Dictionary<string, string>(); 
// //     public enum Type {
// //         PERSON, OBJECT, CONCEPT, CHECKPOINT, ACTION, SELF
// //     }
// //     Profile(string _name, string _type, string _subtype) {
// //         body.Add("1", "2");
// //     }

// // }