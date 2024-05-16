using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Transform mainCharacter;
    public Transform mainCharacterRiverQuest;
    public Transform mainCamera;
    public Transform camera2;

    [Header("UI Controller")]
    public GameObject mainUI;
    public GameObject pilahSampahUI;
    public GameObject bersihSungaiUI;

    [Header("Inventory")]
    public Transform inventory;
    public Transform inventoryExt;

    [Header("Shop Panel")]
    public Transform shopPanel;
    public ShopController shopController;

    private void Awake() 
    {
        // Pastikan hanya ada satu instance QuestManager yang ada
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void openCloseinventoryExtMerchant(bool isActive) 
    {
        inventoryExt.gameObject.SetActive(isActive);
        inventoryExt.localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(0).localPosition += new Vector3(50f, 0f, 0f);
        inventoryExt.GetChild(2).gameObject.SetActive(true);
        if (isActive) 
        {
            inventory.gameObject.SetActive(false);
        }
        else
        {
            inventory.gameObject.SetActive(true);
            inventoryExt.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void openCloseInventoryBag(bool isActive) 
    {
        inventoryExt.gameObject.SetActive(isActive);
        inventoryExt.localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(2).gameObject.SetActive(false);
        inventory.gameObject.SetActive(true);
    }

    public void ShowShopPanel()
    {
        shopPanel.gameObject.SetActive(true);
        shopPanel.localPosition = new Vector3(0f, 0f, 0f);
        shopController.LoadShopData();
    }

    public void CloseShopPanel()
    {
        shopPanel.gameObject.SetActive(false);
        if (shopController != null && shopController.ShopScrollView != null)
        {
            foreach (Transform child in shopController.ShopScrollView)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("Invalid shop or ShopScrollView reference.");
        }
    }
}
