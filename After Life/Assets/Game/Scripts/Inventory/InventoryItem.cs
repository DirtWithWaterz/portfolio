using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{
    public ItemData itemData;

    public InventoryItem(ItemData item)
    {
        itemData = item;
    }
}
