using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string itemName { get; }
    string itemImagePath { get; } // Path untuk menyimpan sprite
    string typeSampah { get; }
    string jenisSampah { get; }
    int jumlahItem { get; }
    Sprite GetImage(); // Method untuk mengambil sprite dari path
    void OnPickup();
    void OnPickupDestroy();
    void OnDrop();
}

[System.Serializable]
public class InventoryEventArgs : EventArgs
{
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryEventArgs(IInventoryItem item)
    {
        this.itemName = item.itemName;
        this.itemImage = item.GetImage(); // Gunakan method GetImage untuk memuat sprite
        this.typeSampah = item.typeSampah;
        this.jenisSampah = item.jenisSampah;
        this.jumlahItem = item.jumlahItem;
    }
}
