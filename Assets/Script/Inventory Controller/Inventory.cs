using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 10;
    private List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    [Header("Pilihan Inventory")]
    public ItemClickHandler[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    [Header("Save/Load")]
    public List<GameObject> allItemInventory = new List<GameObject>();

    [System.Serializable] public class inventoryData
    {
        public Sprite imgInv;
        public string jenisSampahInv;
        public int slotIndex;

        public inventoryData(Sprite imgInv, string jenisSampahInv, int totalItem, int slotIndex)
        {
            this.imgInv = imgInv;
            this.jenisSampahInv = jenisSampahInv;
            this.slotIndex = slotIndex;
        }
    }

    private void Start()
    {
        ChangedSelectedSlot(0);

        // SaveSystem.loadInventory();
    }

    public void OnApplicationQuit()
    {
        // SaveSystem.SaveInventory();
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
