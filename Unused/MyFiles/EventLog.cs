using Godot;
using System.Collections.Generic;
using System.Data.SQLite;

public class EventLog : Node {//Might do EventId -> Arcade32Played0 : 43 playing events. To allow for looping.
	public Dictionary<string, string> events;
	//SubjectIdStateId, Event
	//Mindless information that happens multiple times a day will only recorded for the day
	//and update the game objects personal information. Lucy -> Details -> Did a lot of arcade yesterday
	
	public EventLog() {

	}
	public EventLog(string subjectId, string subjectType) {
		//Facts about things that have happened to a particular subject.
        
		getFactEvents(subjectId, subjectType);
	}
	public EventLog(string subjectId, string subjectType, string withId, string withType) {
		//For opinions/feelings about events. There will be a reality and a subjects beliefs.
		getPairEvents(subjectId, subjectType, withId, withType);
	}
	public EventLog(string subjectId, string subjectType, string feeling) {//Gene altered humans would be other types
		//Same as previous but All events related to a feeling will be loaded
		getFeelingEvents(subjectId, subjectType, feeling);
	}
	//Events represent change. They are not intended to communicate current facts about something.
	//Current facts can be found on the object in question itself.
	//Beliefs are the 1 human to many objects association table.

    public override void _Ready() {
        events = new Dictionary<string, string>();
    }
	public void addEvent(string subjectId, string newEvent) {

	}

	/*public void getSelfBeliefs(string subjectId) {
	//Unsure if a human will record beliefs about themselves? "I'm a monster" isn't a fact. We'll see.
	}*/
	public void getFactEvents(string subjectId, string subjectType) {
		//No human to object association beliefs. Only facts.
        
	}
	public void getPairEvents(string subjectId, string subjectType, string withId, string withType) {
		//This will require 2 queries. Facts will be added at the top.
		//Personal beliefs and feelings will be added at the bottom of the event.
	}
	public void getFeelingEvents(string subjectId, string subjectType, string feeling) {
		//This will require 2 queries. Facts will be added at the top.
		//Personal beliefs and feelings will be added at the bottom of the event.
	}

	public void updateBelief(string eventId, string modEvent) {

	}

	public void deleteOldEvents(string subjectId, string subjectType, string subjectState) {

	}
}
