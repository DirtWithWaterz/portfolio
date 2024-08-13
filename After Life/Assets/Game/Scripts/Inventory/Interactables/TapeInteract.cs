using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeInteract : MonoBehaviour
{
    public static event HandleTapeCollected OnTapeCollected;
    public delegate void HandleTapeCollected(ItemData itemData);
    public ItemData tapeData;
    void OnMouseDown(){
        Debug.Log(this.name + " has been added to your inventory.");
        Destroy(gameObject);
        OnTapeCollected?.Invoke(tapeData);
    }
}
