using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle all items and information about them.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public abstract class Item : ScriptableObject
{
    public string itemName; // Name of the item
    public string description; // Description of the item
    public Sprite icon; // Icon to display in the UI
    public bool isStackable = true; // stackable by default
    public int maxStackSize = 99; // Default is 1, but you can adjust for stackable items

    // Optional: Method to use the item (can be overridden in child classes for specific behavior)
    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
}
