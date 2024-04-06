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
    public InventoryItemDataList inventoryItemDataList;

    [Header("Pilihan Inventory")]
    public ItemClickHandler[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    public int myItemId;
    public string ID_KEY = "ID_KEY";

    private void Start()
    {
        myItemId = PlayerPrefs.GetInt(ID_KEY, 1);

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
        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        Debug.Log(inventoryItemDataList.slotData.Count);

        if (inventoryItemDataList.slotData.Count < SLOTS)
        {
            if (collider.enabled)
            {
                collider.enabled = false;

                mItem.Add(item);
                InventoryItemData inventoryItemData = new InventoryItemData(myItemId, item.itemName, item.image, item.typeSampah, item.jenisSampah, item.jumlahItem);
                inventoryItemDataList.slotData.Add(inventoryItemData);

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }

                SaveSystem.SaveInventory(inventoryItemDataList.slotData);

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
    //         // item.OnDrop();

    //         Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
    //         if (collider != null)
    //         {
    //             collider.enabled = true;
    //         }

    //         if (ItemRemoved != null)
    //         {
    //             ItemRemoved(this, new InventoryEventArgs(item));
    //         }

    //         // SaveSystem.SaveInventory(myItemId, item);

    //         LoadInventoryItem();
    //     }
    // }

    public void RemoveItem(TrashcanController trashcanController)
    {
        Transform imageTransform = transform.GetChild(defaultSelectedItemIndex).GetChild(0);
        InventoryVariable inventoryVariable = imageTransform.GetChild(0).GetComponent<InventoryVariable>();
        
        int indexToRemove = inventoryItemDataList.slotData.FindIndex(item => {            
            return item.itemId == inventoryVariable.itemId && item.jenisSampah == trashcanController.jenisTempatSampah;
        });

        Debug.Log("Index yang dihapus : " + indexToRemove);

        if (indexToRemove != -1)
        {
            inventoryItemDataList.slotData.RemoveAt(indexToRemove);

            SaveSystem.SaveInventory(inventoryItemDataList.slotData);
            LoadInventoryItem();
            Debug.Log("Buang Sampah Berhasil");
        }
        else
        {
            // Item with the specified itemId not found
            Debug.Log("Gagal Buang Sampah");
        }
    }

    void LoadInventoryItem() 
    {
        List<InventoryItemData> loadedItemData = SaveSystem.LoadInventory();

        if (loadedItemData != null)
        {
            inventoryItemDataList.slotData = loadedItemData;

            int childIndex = 0;
            foreach (InventoryItemData itemData in loadedItemData)
            {
                Transform imageTransform = transform.GetChild(childIndex).GetChild(0).GetChild(0);
                Image image = imageTransform.GetChild(0).GetComponent<Image>();
                InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();

                image.enabled = true;

                inventoryVariable.itemId = itemData.itemId;
                inventoryVariable.itemName = itemData.itemName;
                inventoryVariable.jenisSampah = itemData.jenisSampah;
                inventoryVariable.totalSampah = itemData.jumlahItem;
                
                image.sprite = itemData.itemImage;

                childIndex++;
            }
        }
    }

    private void IncrementAndSaveItemId() {
        myItemId++;
        PlayerPrefs.SetInt(ID_KEY, myItemId);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class InventoryItemData
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryItemData(
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
public class InventoryItemDataList
{
    public List<InventoryItemData> slotData;
}
