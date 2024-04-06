using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryExt : MonoBehaviour
{
    private const int SLOTS = 12;
    private List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    private void Start()
    {
        
    }

    public void OnApplicationQuit()
    {
        
    }

    public void AddItem(IInventoryItem item)
    {
        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        if (mItem.Count < SLOTS)
        {
            if (collider.enabled)
            {
                collider.enabled = false;

                mItem.Add(item);

                item.OnPickup();

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }
            }
        }
    }

    public void RemoveItem(IInventoryItem item)
    {
        if (mItem.Contains(item))
        {
            mItem.Remove(item);
            item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
        }
    }
}

[System.Serializable]
public class InventoryExtItemData
{
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryExtItemData(
        string itemName,
        Sprite itemImage,
        string typeSampah,
        string jenisSampah,
        int jumlahItem
    )
    {
        this.itemName = itemName;
        this.itemImage = itemImage;
        this.typeSampah = typeSampah;
        this.jenisSampah = jenisSampah;
        this.jumlahItem = jumlahItem;
    }
}

[System.Serializable]
public class InventoryExtItemDataList
{
    public List<InventoryExtItemData> slotData = new List<InventoryExtItemData>();
}