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

[System.Serializable] public class InventoryEventArgs : EventArgs
{
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;
    public InventoryEventArgs(IInventoryItem item)
    {
        this.itemName = item.itemName;
        this.itemImage = item.image;
        this.typeSampah = item.typeSampah;
        this.jenisSampah = item.jenisSampah;
        this.jumlahItem = item.jumlahItem;
    }
}