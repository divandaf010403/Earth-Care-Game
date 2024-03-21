using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

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

    private static readonly string playerPath = Application.persistentDataPath + "/save/player.json";
    private static readonly string shopPath = Application.persistentDataPath + "/save/shopData.json";
    private static readonly string inventoryPath = Application.persistentDataPath + "/save/inventoryData.json";

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
        ShopData shopData = new ShopData(shopItemList);
        string json = JsonUtility.ToJson(shopData);
        File.WriteAllText(shopPath, json);
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

    public static void SaveInventory(List<Inventory.inventoryData> inventoryData)
    {
        
    }
}
