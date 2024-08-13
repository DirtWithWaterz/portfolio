using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();

    private void OnEnable() {
        TapeInteract.OnTapeCollected += Add;
    }

    private void OnDisable() {
        TapeInteract.OnTapeCollected -= Add;
    }

    public void Add(ItemData itemData){
        InventoryItem newItem = new InventoryItem(itemData);
        inventory.Add(newItem);
        Debug.Log($"Added {itemData.displayName} to the inventory.");
    }

    public void Remove(InventoryItem item){
        inventory.Remove(item);
    }
}
