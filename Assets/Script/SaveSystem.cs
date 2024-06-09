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

    private static readonly string playerDataPath = Application.persistentDataPath + "/save/player.json";
    private static readonly string shopPath = Application.persistentDataPath + "/save/shopData.json";
    private static readonly string inventoryPath = Application.persistentDataPath + "/save/inventoryData.json";
    private static readonly string inventoryExtPath = Application.persistentDataPath + "/save/inventoryExtData.json";

    public static void SavePlayerData(MainCharMovement mainChar)
    {
        PlayerData data = LoadPlayerData();
        if (data == null)
        {
            data = new PlayerData(mainChar);
        }
        else
        {
            data.position = new float[3];
            data.position[0] = mainChar.transform.position.x;
            data.position[1] = mainChar.transform.position.y;
            data.position[2] = mainChar.transform.position.z;

            data.rotation = new float[3];
            data.rotation[0] = mainChar.transform.rotation.eulerAngles.x;
            data.rotation[1] = mainChar.transform.rotation.eulerAngles.y;
            data.rotation[2] = mainChar.transform.rotation.eulerAngles.z;

            data.playerCoin = mainChar.playerCoin;
            data.questNumber = GameVariable.questNumber;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(playerDataPath, json);
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(playerDataPath))
        {
            string json = File.ReadAllText(playerDataPath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }

    public static void UpdatePlayerTransform(MainCharMovement mainChar)
    {
        PlayerData data = LoadPlayerData() ?? new PlayerData(mainChar);
        data.position[0] = mainChar.transform.position.x;
        data.position[1] = mainChar.transform.position.y;
        data.position[2] = mainChar.transform.position.z;

        data.rotation[0] = mainChar.transform.rotation.eulerAngles.x;
        data.rotation[1] = mainChar.transform.rotation.eulerAngles.y;
        data.rotation[2] = mainChar.transform.rotation.eulerAngles.z;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(playerDataPath, json);
    }

    public static void UpdatePlayerCoin(MainCharMovement mainChar)
    {
        PlayerData data = LoadPlayerData() ?? new PlayerData(mainChar);
        data.playerCoin = mainChar.playerCoin;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(playerDataPath, json);
    }

    public static void UpdatePlayerQuest()
    {
        PlayerData data = LoadPlayerData() ?? new PlayerData(null);
        data.questNumber = GameVariable.questNumber;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(playerDataPath, json);
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

    // public static void SaveInventory(List<IInventoryItem> mItem)
    // {
    //     InventoryItemDataList inventoryItemDataList = new InventoryItemDataList();
    //     string directoryPath = Path.GetDirectoryName(inventoryPath);

    //     if (!Directory.Exists(directoryPath))
    //     {
    //         Directory.CreateDirectory(directoryPath);
    //     }

    //     for(int i = 0; i < mItem.Count; i++) {
    //         InventoryItemData inventoryItemData = new InventoryItemData(mItem[i].itemName, mItem[i].image, mItem[i].typeSampah, mItem[i].jenisSampah, mItem[i].jumlahItem);
    //         inventoryItemDataList.slotData.Add(inventoryItemData);
    //     }

    //     string json = JsonUtility.ToJson(inventoryItemDataList);

    //     File.WriteAllText(inventoryPath, json);
    //     Debug.Log("Inventory data saved successfully!");
    // }

    // Save untuk bottom Inventory
    public static void SaveInventory(List<InventoryItemData> existingInventoryData)
    {
        string directoryPath = Path.GetDirectoryName(inventoryPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        InventoryItemDataList inventoryItemDataList = new InventoryItemDataList();
        inventoryItemDataList.slotData = existingInventoryData;

        string json = JsonUtility.ToJson(inventoryItemDataList);

        File.WriteAllText(inventoryPath, json);
        Debug.Log("Inventory data saved successfully!");
    }

    public static List<InventoryItemData> LoadInventory()
    {
        if (File.Exists(inventoryPath))
        {
            string json = File.ReadAllText(inventoryPath);
            InventoryItemDataList inventoryDataItemList = JsonUtility.FromJson<InventoryItemDataList>(json);
            return inventoryDataItemList.slotData;
        }
        else
        {
            Debug.LogError("Shop data not found at path: " + inventoryPath);
            return null;
        }
    }

    // Save untuk Inventory Extended
    public static void SaveInventoryExt(List<InventoryExtItemData> existingInventoryData)
    {
        string directoryPath = Path.GetDirectoryName(inventoryExtPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        InventoryExtItemDataList inventoryExtItemDataList = new InventoryExtItemDataList();
        inventoryExtItemDataList.slotData = existingInventoryData;

        string json = JsonUtility.ToJson(inventoryExtItemDataList);

        File.WriteAllText(inventoryExtPath, json);
        Debug.Log("Inventory data saved successfully!");
    }

    public static List<InventoryExtItemData> LoadInventoryExt()
    {
        if (File.Exists(inventoryExtPath))
        {
            string json = File.ReadAllText(inventoryExtPath);
            InventoryExtItemDataList inventoryExtDataItemList = JsonUtility.FromJson<InventoryExtItemDataList>(json);
            return inventoryExtDataItemList.slotData;
        }
        else
        {
            Debug.LogError("Shop data not found at path: " + inventoryExtPath);
            return null;
        }
    }
}
