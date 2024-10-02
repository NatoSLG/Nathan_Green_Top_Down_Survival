using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static UnityEditor.Progress;

/// <summary>
/// This class handles the players inventory.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    /*Nested class that is used to represent an inventory slot.
     Inventory can contain an item or weapon and needs to track the UI equivalent(image) of the slot*/
    public class Slot
    {
        public Item item; // Reference to the item in the slot (defined an Item class separately)
        public Image slotImage; // Reference to the UI image representing the item

        // Constructor for a Slot
        public Slot(Item newItem, Image newImage)
        {
            item = newItem;
            slotImage = newImage;
        }

        // Method to update the slot with a new item and its corresponding image
        public void UpdateSlot(Item newItem, Sprite newSprite)
        {
            item = newItem;
            slotImage.sprite = newSprite; // Update the UI image
        }

        // Method to clear the slot
        public void ClearSlot()
        {
            item = null;
            slotImage.sprite = null; // Remove the UI image
        }

        // Optional: Method to check if the slot is empty
        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public Slot[] inventorySlots; // Inventory slots list

    // Initialize inventory slots
    void Start()
    {
        inventorySlots = new Slot[24]; // 24 inventory slots
    }
}
