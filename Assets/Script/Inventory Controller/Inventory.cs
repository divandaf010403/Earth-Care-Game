using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 10;

    [SerializeField]
    public List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    [Header("Pilihan Inventory")]
    public ItemClickHandler[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    private void Start()
    {
        ChangedSelectedSlot(0);

        LoadInventoryItem();
    }

    public void OnApplicationQuit()
    {
        // SaveInventory();
    }
    
    void Update() {
        LoadInventoryItem();
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
        InventoryItemDataList inventoryItemDataList = new InventoryItemDataList();
        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        Debug.Log(inventoryItemDataList.slotData.Count);

        if (inventoryItemDataList.slotData.Count < SLOTS)
        {
            if (collider.enabled)
            {
                collider.enabled = false;

                mItem.Add(item);

                // item.OnPickup();

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }

                SaveSystem.SaveInventory(mItem);

                LoadInventoryItem();

                item.OnPickup();
            }
        }
    }

    public void RemoveItem(IInventoryItem item)
    {
        if (mItem.Contains(item))
        {
            mItem.Remove(item);
            // item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }

            SaveSystem.SaveInventory(mItem);

            LoadInventoryItem();
        }
    }

    void LoadInventoryItem() 
    {
        List<InventoryItemData> loadedItemData = SaveSystem.LoadInventory();

        if (loadedItemData != null)
        {
            int childIndex = 0;
            foreach (InventoryItemData itemData in loadedItemData)
            {
                Transform imageTransform = transform.GetChild(childIndex).GetChild(0).GetChild(0);
                Image image = imageTransform.GetChild(0).GetComponent<Image>();
                InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();

                // if (!image.enabled)
                // {
                    
                // }

                image.enabled = true;

                inventoryVariable.itemName = itemData.itemName;
                inventoryVariable.jenisSampah = itemData.jenisSampah;
                inventoryVariable.totalSampah = itemData.jumlahItem;
                
                image.sprite = itemData.itemImage;

                childIndex++;
            }
        }
    }
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryItemData(
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
public class InventoryItemDataList
{
    public List<InventoryItemData> slotData = new List<InventoryItemData>();
}
