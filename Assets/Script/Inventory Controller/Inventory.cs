using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 10;
    [SerializeField] public List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    [Header("Pilihan Inventory")]
    public ItemClickHandler[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    private void Start()
    {
        ChangedSelectedSlot(0);

        // LoadInventory();
    }

    public void OnApplicationQuit()
    {
        // SaveInventory();
    }

    public void ChangedSelectedSlot(int newValue)
    {
        if (defaultSelectedItemIndex >= 0)
        {
            itemSelected[defaultSelectedItemIndex].Deselected();
        }

        itemSelected[newValue].Selected();
        defaultSelectedItemIndex = newValue;
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

                // SaveSystem.SaveInventory(item.itemName, item.image, item.typeSampah, item.jenisSampah, item.jumlahItem);
                SaveSystem.SaveInventory(mItem);
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

            // SaveInventory();

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
        }
    }

    private void LoadInventory()
    {
        mItem = SaveSystem.LoadInventory();
    }
}

[System.Serializable] public class InventoryItemData
{
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;
    public InventoryItemData(string itemName, Sprite itemImage, string typeSampah, string jenisSampah, int jumlahItem)
    {
        this.itemName = itemName;
        this.itemImage = itemImage;
        this.typeSampah = typeSampah;
        this.jenisSampah = jenisSampah;
        this.jumlahItem = jumlahItem;
    }
}

[System.Serializable]
public class InventoryItemDataList
{
    public List<InventoryItemData> slotData = new List<InventoryItemData>();
}
