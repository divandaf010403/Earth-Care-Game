using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryExt : MonoBehaviour
{
    private const int SLOTS = 12;
    private List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public InventoryExtItemDataList inventoryExtItemDataList;

    public int myItemIdExt;
    public string ID_KEY = "ID_KEY_EXt";

    private void Start()
    {
        myItemIdExt = PlayerPrefs.GetInt(ID_KEY, 1);

        LoadInventoryItem();
    }

    public void OnApplicationQuit()
    {
        SaveSystem.SaveInventoryExt(inventoryExtItemDataList.slotData);
    }
    
    void Update() 
    {
        LoadInventoryItem();
    }

    public void AddItem(IInventoryItem item)
    {
        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        Debug.Log(inventoryExtItemDataList.slotData.Count);

        if (inventoryExtItemDataList.slotData.Count < SLOTS)
        {
            if (collider.enabled)
            {
                collider.enabled = false;

                bool itemExists = false;

                // Mengecek apakah item dengan jenisSampah yang sama sudah ada dalam inventaris
                foreach (InventoryExtItemData existingItem in inventoryExtItemDataList.slotData)
                {
                    if (existingItem.jenisSampah == item.jenisSampah)
                    {
                        existingItem.jumlahItem += item.jumlahItem; // Menambah jumlahItem
                        itemExists = true;
                        break;
                    }
                }

                // Jika item dengan jenisSampah yang sama tidak ditemukan, tambahkan item baru ke inventaris
                if (!itemExists)
                {
                    mItem.Add(item);
                    InventoryExtItemData inventoryExtItemData = new InventoryExtItemData(myItemIdExt, item.itemName, item.image, item.typeSampah, item.jenisSampah, item.jumlahItem);
                    inventoryExtItemDataList.slotData.Add(inventoryExtItemData);
                }

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }

                SaveSystem.SaveInventoryExt(inventoryExtItemDataList.slotData);

                LoadInventoryItem();

                item.OnPickup();

                IncrementAndSaveItemId();
            }
        }
    }

    // public void RemoveItem(IInventoryItem item)
    // {
    //     if (mItem.Contains(item))
    //     {
    //         mItem.Remove(item);
    //         item.OnDrop();

    //         Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
    //         if (collider != null)
    //         {
    //             collider.enabled = true;
    //         }

    //         if (ItemRemoved != null)
    //         {
    //             ItemRemoved(this, new InventoryEventArgs(item));
    //         }
    //     }
    // }

    void LoadInventoryItem() 
    {
        List<InventoryExtItemData> loadedItemData = SaveSystem.LoadInventoryExt();

        if (loadedItemData != null)
        {
            inventoryExtItemDataList.slotData = loadedItemData;

            int childIndex = 0;
            foreach (InventoryExtItemData itemData in loadedItemData)
            {
                Transform imageTransform = transform.GetChild(0).GetChild(childIndex);
                Image image = imageTransform.GetChild(0).GetComponent<Image>();
                InventoryVariable inventoryVariable = imageTransform.GetChild(0).GetComponent<InventoryVariable>();
                TextMeshProUGUI totalItem = imageTransform.GetChild(1).GetComponent<TextMeshProUGUI>();

                image.enabled = true;

                inventoryVariable.itemId = itemData.itemId;
                inventoryVariable.itemName = itemData.itemName;
                inventoryVariable.jenisSampah = itemData.jenisSampah;
                inventoryVariable.totalSampah = itemData.jumlahItem;
                
                image.sprite = itemData.itemImage;

                totalItem.text = inventoryVariable.totalSampah.ToString();
                childIndex++;
            }
        }
    }

    private void IncrementAndSaveItemId() {
        myItemIdExt++;
        PlayerPrefs.SetInt(ID_KEY, myItemIdExt);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class InventoryExtItemData
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryExtItemData(
        int itemId,
        string itemName,
        Sprite itemImage,
        string typeSampah,
        string jenisSampah,
        int jumlahItem
    )
    {
        this.itemId = itemId;
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
    public List<InventoryExtItemData> slotData;
}