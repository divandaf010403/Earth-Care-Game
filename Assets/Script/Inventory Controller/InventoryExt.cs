using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryExt : MonoBehaviour
{
    public static InventoryExt Instance;
    private const int SLOTS = 12;
    private List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public InventoryExtItemDataList inventoryExtItemDataList;

    public int myItemIdExt;
    public string ID_KEY = "ID_KEY_EXt";

    [Header("Crafting")]
    public List<NewRecipe> itemRecipes = new List<NewRecipe>();

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

                    IncrementAndSaveItemId();
                }

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }

                SaveSystem.SaveInventoryExt(inventoryExtItemDataList.slotData);

                LoadInventoryItem();

                item.OnPickup();
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

    public void LoadInventoryItem() 
    {
        List<InventoryExtItemData> loadedItemData = SaveSystem.LoadInventoryExt();

        if (loadedItemData != null)
        {
            inventoryExtItemDataList.slotData = loadedItemData;

            int childIndex = 0;
            foreach (InventoryExtItemData itemData in loadedItemData)
            {
                Transform imageTransform = transform.GetChild(0).GetChild(0).GetChild(childIndex);
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

    public void IncrementAndSaveItemId() {
        myItemIdExt++;
        PlayerPrefs.SetInt(ID_KEY, myItemIdExt);
        PlayerPrefs.Save();
    }

    public void craftItem(string itemName)
    {
        foreach (NewRecipe recipe in itemRecipes)
        {
            if (recipe.createdItemPrefab.GetComponent<NewItem>().name == itemName)
            {
                bool haveAllIngredients = true;
                for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                {
                    if (haveAllIngredients)
                        haveAllIngredients = haveIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                }

                if (haveAllIngredients)
                {
                    for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                    {
                        removeIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }

                    // GameObject craftedItem = Instantiate(recipe.createdItemPrefab, dropLocation.position, Quaternion.identity);
                    // craftedItem.GetComponent<NewItem>().currentQuantity = recipe.quantityProduced;

                    // addItemToInventory(craftedItem.GetComponent<NewItem>());

                    Debug.Log("Buat Barang");
                }

                break;
            }
        }
    }

    private void removeIngredient(string itemName, int quantity)
    {
        int remainingQuantity = quantity;

        foreach (InventoryExtItemData itemData in inventoryExtItemDataList.slotData)
        {
            if (itemData.itemName == itemName)
            {
                if (itemData.jumlahItem >= remainingQuantity)
                {
                    itemData.jumlahItem -= remainingQuantity;
                    remainingQuantity = 0;
                    break; // Exit the loop since we have removed the required quantity
                }
                else
                {
                    remainingQuantity -= itemData.jumlahItem;
                    itemData.jumlahItem = 0;
                }
            }
        }
    }

    private bool haveIngredient(string itemName, int quantity)
    {
        int foundQuantity = 0;
        foreach (InventoryExtItemData itemData in inventoryExtItemDataList.slotData)
        {
            if (itemData.itemName == itemName)
            {
                foundQuantity += itemData.jumlahItem;

                if (foundQuantity >= quantity)
                    return true;
            }
        }
        return false;
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

[CreateAssetMenu(fileName = "NewInventoryExtItem", menuName = "Inventory/InventoryExtItem")]
public class InventoryExtItemDataSO : ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;
}