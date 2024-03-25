using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public class ShopData
    {
        public List<ShopController.ShopItem> shopItemList;

        public ShopData(List<ShopController.ShopItem> items)
        {
            shopItemList = items;
        }
    }

    [System.Serializable]
    private class InventoryData
    {
        public List<IInventoryItem> items;

        public InventoryData(List<IInventoryItem> items)
        {
            this.items = items;
        }
    }

    private static readonly string playerPath = Application.persistentDataPath + "/save/player.json";
    private static readonly string shopPath = Application.persistentDataPath + "/save/shopData.json";
    private static readonly string inventoryPath = Application.persistentDataPath + "/save/inventoryData.savSn";

    public static void SavePlayer(MainCharMovement mainChar)
    {
        string json = JsonUtility.ToJson(new PlayerData(mainChar));
        File.WriteAllText(playerPath, json);
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(playerPath))
        {
            string json = File.ReadAllText(playerPath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogError("Player save not found at path: " + playerPath);
            return null;
        }
    }

    public static void SaveShop(List<ShopController.ShopItem> shopItemList)
    {
        string directoryPath = Path.GetDirectoryName(shopPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        ShopData shopData = new ShopData(shopItemList);
        string json = JsonUtility.ToJson(shopData);

        File.WriteAllText(shopPath, json);
        Debug.Log("Shop data saved successfully!");
    }

    // Load shop data
    public static List<ShopController.ShopItem> LoadShop()
    {
        Debug.Log(shopPath);
        if (File.Exists(shopPath))
        {
            string json = File.ReadAllText(shopPath);
            ShopData shopData = JsonUtility.FromJson<ShopData>(json);
            return shopData.shopItemList;
        }
        else
        {
            Debug.LogError("Shop data not found at path: " + shopPath);
            return null;
        }
    }

    public static void SaveInventory(List<IInventoryItem> inventory)
    {
        try
        {
            // Create the directory if it doesn't exist
            string directoryPath = Path.GetDirectoryName(inventoryPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Use using statement to ensure proper file closure
            using (FileStream stream = new FileStream(inventoryPath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                InventoryData data = new InventoryData(inventory);
                formatter.Serialize(stream, data);
            }

            Debug.Log("Inventory data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving inventory data: " + e.Message);
        }
    }

    public static List<IInventoryItem> LoadInventory()
    {
        List<IInventoryItem> inventory = new List<IInventoryItem>();

        try
        {
            if (File.Exists(inventoryPath))
            {
                // Use using statement to ensure proper file closure
                using (FileStream stream = new FileStream(inventoryPath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    InventoryData data = formatter.Deserialize(stream) as InventoryData;
                    inventory = data.items; // Retrieving items from data
                }

                Debug.Log("Inventory data loaded successfully.");
            }
            else
            {
                Debug.LogWarning("Inventory data file does not exist.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading inventory data: " + e.Message);
        }

        return inventory;
    }
}
