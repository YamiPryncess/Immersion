using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GInventory : Node
{
    // Store our items in a List
    public List<Item> items = new List<Item>();

    // Method to add items to our list 
    public void AddItem(Item i) {

        items.Add(i);
    }

    // Method to search for a particular item
    public RigidBody FindItemWithTag(string type) {

        // Iterate through all the items
        foreach (Item i in items) {

            // Found a match
            if (i.info.type == type) {

                return i;
            }
        }
        // Nothing found
        return null;
    }

    // Remove an item from our list
    public void RemoveItem(Item i) {

        int indexToRemove = -1;

        // Search through the list to see if it exists
        foreach (Item g in items) {

            // Initially set indexToRemove to 0. The first item in the List
            indexToRemove++;
            // Have we found it?
            if (g == i) {

                break;
            }
        }
        // Do we have something to remove?
        if (indexToRemove >= 1) {

            // Yes we do.  So remove the item at indexToRemove
            items.RemoveAt(indexToRemove);
        }
    }
}
