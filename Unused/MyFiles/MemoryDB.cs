using Godot;
using System;
/*
public class MemoryDB: Node2D {
     public MemoryDB(object document) { 
        // create document 
        this.db = document;  

        Object.keys(document).forEach((prop) => { 
            configuration[prop] = { 
                get() { 
                    const value = db[name][document._id][prop]; 
                    // return an instance or a value
                    return value.indexOf('@') !== -1 ? instances[value.replace('@', '')] : value;
                }, 
                set(value) {
                    if (classes[value.constructor.name]) { 
                        // store the id of the instance 
                        db[name][document._id][prop] = value._id; 
                    } else { 
                        db[name][document._id][prop] = value; 
                    } 
                } 
            };
        });
        // add it to the list of instances 
        //instances[document._id] = this;
    }
}*/