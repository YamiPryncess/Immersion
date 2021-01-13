using Godot;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Heart : Node {
    GAgent agent;
    //Dictionary<string, int> physiology = new Dictionary<string, int>();
    //Mentality mentality = new Mentality();
    public override void _Ready() {
        agent = GetParent<GAgent>();
        // Call base Start method
        // base.Start(); // Supposed to be a GAgent Start but it was already cause since it's the parent
        // Set goal so that it can't be removed so the nurse can repeat this action

        /*physiology.Add("Hunger", 100);
        physiology.Add("Pain", 90);
        physiology.Add("Excretion", 50);

        physiology.Add("Comfort", 80);*/
        //physiology.Add("Empathy", 60);
        /*physiology.Add("Sexuality", 30);

        physiology.Add("Interest", 20);
        physiology.Add("Temperature", 20);
        physiology.Add("Mobility", 20);*/

        SubGoal s1 = new SubGoal("Exploration", 1, false);
        agent.goals.Add(s1, 1);

        SubGoal s2 = new SubGoal("Empathy", 1, false);
        agent.goals.Add(s2, 2);
    }
}
/*
public class Mentality { //: ISerializable {//So far I'm confident in how I made morality but not so confident with Rapport working under the same rules... Let's just keep working and test until we figure out an answer. I need rapport to actually record superficial information rather than a complex chain of desires. Such information would be like if I yell william might be sad. Etc lol Or More importantly info that compares you're most treasured desires to what you think of them. Sooo maybe that could be a calculated desire or a list of desires that you find super valuable.
    List<Memory> experiences = new List<Memory>();
    Dictionary<string, Desire> appraisal = new Dictionary<string, Desire>(); //Preferences, not necessarily fitness in specific situations/This identifies the npc's ideal world.
    Dictionary<string, List<string>> desires = new Dictionary<string, List<string>>(); //Key = Emotional Function; Value = Rewarding Belief for Emotional Function.
    public Mentality(){
        desires.Add("Pride");
        desires.Add("Sexuality");
    }
    //Dictionary<string, Dictionary<string, List<Desire>>> prevalance = new Dictionary<string, Dictionary<string, List<Desire>>>();
    //Dictionary<string, Intention> ideals = new Dictionary<string, Intention>();
    //Dictionary<string, List<Rapport>> rapport = new Dictionary<string, List<Rapport>>();
    //mentality.morality["Honest Interaction"].associations["Positive Response"];
    //mentality.rapport["Lynn"].morality["Positive Response"].associations["Honest Interaction"];
    //public Mentality(){
        //experiences.Add("", new List<Memory>());
    //}
}
public class Memory {
    string actor = "";
    string affected = "";
    string understanding = "";//These could be Lists but to save space for now it's just strings.
}

public class Desire {//Beliefs are both conceptual and physical desires. So some BDI actions are beliefs but not all beliefs are actions.
    string desireName = "";
    float motivation = 0; //How will we calculate which desires are more treasured?
    float belief = 0;
    Dictionary<string, float> associations = new Dictionary<string, float>(); //If this desire is true then what that means for other desires is written here. <- This is working forwards towards the goal/desire. Actually I may want to work backwards from the Desire.
    //An Example of working backwards, if understanding is true then, the desire to work together might be successful. However understanding has info that says otherwise in the face of cultural differences. So work together -> Understanding = 10 points and Understanding -> Cultural DIfferences -19 points etc lol
    Dictionary<string, List<int>> helpTowards = new Dictionary<string, List<int>>();
    Dictionary<string, List<int>> harmTowards = new Dictionary<string, List<int>>();
    Dictionary<string, List<int>> nullTowards = new Dictionary<string, List<int>>();

    public void influence(string desire, float amount) {
        associations[desire] += amount;
    }
    public void storeMemoryId(string subject, float outcome, int id) {
        if(outcome > 0){
            helpTowards[subject].Add(id);
        }
        if(outcome < 0){
            harmTowards[subject].Add(id);
        }
        if(outcome == 0){
            nullTowards[subject].Add(id);
        }
    }
}

public class Intention {
    List<Desire> intent = new List<Desire>();
    List<int> order = new List<int>();//1 2 3 5 8 means 1 goes first, then 2, then 3 or 4, then 5, then 8. Negative numbers may indicate that something is optional
}

public class intent {
    
}

public class Rapport {
    string subject = "";
    Dictionary<string, Desire> morality = new Dictionary<string, Desire>();

}*/

//Removed Code
// Dictionary<string, Dictionary<string, float>> bias = 
    //     new Dictionary<string, Dictionary<string, float>>();
    //     //A rating of (+/-)1 means that they're aware of something
    //     //A rating of (+/-)10 means that they love it
    //List<List<List<float>>> beliefs = new List<List<List<float>>>();
    
    // List<List<List<float>>> morality = new List<List<List<float>>>();
    // List<string> experience = new List<string>();
    
        // Call the GetTired() method for the first time
        //Invoke("GetTired", Random.Range(10.0f, 20.0f));

        // bias.Add("Food", new Dictionary<string, float>());
        // bias.Add("Play", new Dictionary<string, float>());
        // bias.Add("Philosophy", new Dictionary<string, float>());
        // bias["Food"].Add("Pizza", 6);
    /*public void updateMorality(int act, int context, float diff){
        fillMorality(act, context);
        List<List<List<float>>> test= new List<List<List<float>>>(morality);
        morality[act][context] += diff;
        if(context == 0 && morality[act][context] == 0 && diff != 0) {
            morality[act][context] = diff > 0 ? 0.1f : -0.1f;q
        }//0 may be used to represent not knowing. Context[0] is General Context. 
    }//This "may" be updated later to check between 1 & -1 instead.
    public void fillMorality(int act, int context){
        for(var i = morality.Count; morality.Count - 1 < act; i++){
            morality.Add(new List<float>());
        }
        for(var i = morality[act].Count; morality[act].Count - 1 < context; i++){
            morality[act].Add(0);
        }
    }
    public void addExperience(string action, string context) {
        experience.Add(action + "|" + context + "|" + "time of incident");
    }*/
    

/*    void GetTired() {

        beliefs.ModifyState("exhausted", 0);
        //call the get tired method over and over at random times to make the nurse
        //get tired again
        Invoke("GetTired", Random.Range(0.0f, 20.0f));
    }
*/

// SubGoal s1 = new SubGoal("treatPatient", 1, false);
//         agent.goals.Add(s1, 3);

//         // Resting goal
//         SubGoal s2 = new SubGoal("rested", 1, false);
//         agent.goals.Add(s2, 1);
/*Dictionary<string, float> helped = new Dictionary<string, float>();
    Dictionary<string, float> harmed = new Dictionary<string, float>();
    Dictionary<string, float> nulled = new Dictionary<string, float>();*/
    
    /*public void updateDependents() {
        if(dependentsUpdated == false) {
            helped = new Dictionary<string, float>();
            harmed = new Dictionary<string, float>();
            foreach(KeyValuePair<string, float> desire in associations) {
                if(desire.Value > 1) {
                    helped.Add(desire.Key, desire.Value);
                } else if(desire.Value < -1) {
                    harmed.Add(desire.Key, desire.Value);
                } else if(desire.Value > -1 && desire.Value < 1) {
                    nulled.Add(desire.Key, desire.Value);
                }
            }
            dependentsUpdated = true;
        }
    }*/
    