using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string itemName { get; }
    Sprite image { get; }
    string typeSampah { get; }
    string jenisSampah { get; }
    int jumlahItem { get; }
    void OnPickup();
    void OnDrop();
}

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }

    public IInventoryItem Item;
}

[Serializable]
public class InventoryItemData
{
    public string itemName;
    public string jenisSampah;
    public string typeSampah;
    public int jumlahItem;
    // Add other fields as needed
}