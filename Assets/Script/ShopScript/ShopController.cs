using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    [System.Serializable] public class ShopItem
    {
        public Sprite imageItem;
        public string nameItem;
        public int priceItem;
        public bool isPurchased = false;
        public string jenisSampahNama;
        public string typeSampahTxt;
        public int totalSampahTxt = 1;

        public ShopItem(ShopItem shopItem)
        {
            this.imageItem = shopItem.imageItem;
            this.nameItem = shopItem.nameItem;
            this.priceItem = shopItem.priceItem;
            this.isPurchased = shopItem.isPurchased;
            this.jenisSampahNama = shopItem.jenisSampahNama;
            this.typeSampahTxt = shopItem.typeSampahTxt;
            this.totalSampahTxt = shopItem.totalSampahTxt;
        }
    }

    [SerializeField] public List<ShopItem> shopItemList;
    [SerializeField] public GameObject ItemTemplate;
    GameObject g;
    [SerializeField] public Transform ShopScrollView;
    public Transform inventoryExtTransform;
    public Transform mainChar;

    void Start()
    {
        
    }

    void PurchaseItem(ShopItem item)
    {
        InventoryExt inventoryExt = inventoryExtTransform.GetComponent<InventoryExt>();
        MainCharMovement mainCharComponent = mainChar.GetComponent<MainCharMovement>();

        Debug.Log("Uang Saya adalah : " + mainCharComponent.playerCoin + " dan saya akan membeli item dengan harga : " + item.priceItem);
        
        if (mainCharComponent.playerCoin > item.priceItem)
        {
            bool itemExists = false;

            // Mengecek apakah item dengan jenisSampah yang sama sudah ada dalam inventaris
            foreach (InventoryExtItemData existingItem in inventoryExt.inventoryExtItemDataList.slotData)
            {
                if (existingItem.jenisSampah == item.jenisSampahNama)
                {
                    existingItem.jumlahItem += item.totalSampahTxt; // Menambah jumlahItem
                    itemExists = true;
                    break;
                }
            }

            // Jika item dengan jenisSampah yang sama tidak ditemukan, tambahkan item baru ke inventaris
            if (!itemExists)
            {
                InventoryExtItemData inventoryExtItemData = new InventoryExtItemData(inventoryExt.myItemIdExt, item.nameItem, item.imageItem, item.typeSampahTxt, item.jenisSampahNama, item.totalSampahTxt);
                inventoryExt.inventoryExtItemDataList.slotData.Add(inventoryExtItemData);
            }

            SaveSystem.SaveInventoryExt(inventoryExt.inventoryExtItemDataList.slotData);

            inventoryExt.LoadInventoryItem();

            inventoryExt.IncrementAndSaveItemId();

            mainCharComponent.playerCoin -= item.priceItem;

            mainCharComponent.SavePlayerData();
        }
        else
        {
            Debug.LogError("Uang Kurang");
        }
    }

    void SaveShopData()
    {
        SaveSystem.SaveShop(shopItemList);
    }

    public void LoadShopData()
    {
        List<ShopItem> loadedShopData = SaveSystem.LoadShop();

        if (loadedShopData != null)
        {
            foreach (ShopController.ShopItem item in loadedShopData)
            {
                GameObject newItem = Instantiate(ItemTemplate, ShopScrollView);
                ShopVariable shopVariable = newItem.transform.GetComponent<ShopVariable>();
                newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.nameItem;
                newItem.transform.GetChild(1).GetComponent<Image>().sprite = item.imageItem;

                shopVariable.jenisSampahNama = item.jenisSampahNama;
                shopVariable.typeSampahTxt = item.typeSampahTxt;
                shopVariable.totalSampahTxt = item.totalSampahTxt;

                Button button = newItem.transform.GetChild(2).GetComponent<Button>();
                button.onClick.AddListener(() => PurchaseItem(item));

                if (item.isPurchased == true)
                {
                    newItem.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    newItem.transform.GetChild(2).GetComponent<Button>().interactable = false;
                } 
                else
                {
                    newItem.transform.GetChild(2).GetComponent<Button>().interactable = true;
                    newItem.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    newItem.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.priceItem.ToString();
                }
            }
        }
        else
        {
            Debug.LogWarning("Creating new shop data.");

            int lengthItem = shopItemList.Count;
            for (int i = 0; i < lengthItem; i++)
            {
                g = Instantiate(ItemTemplate, ShopScrollView);
                g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItemList[i].nameItem;
                g.transform.GetChild(1).GetComponent<Image>().sprite = shopItemList[i].imageItem;

                ShopVariable shopVariable = g.transform.GetComponent<ShopVariable>();
                shopVariable.jenisSampahNama = shopItemList[i].jenisSampahNama;
                shopVariable.typeSampahTxt = shopItemList[i].typeSampahTxt;
                shopVariable.totalSampahTxt = shopItemList[i].totalSampahTxt;

                Button button = g.transform.GetChild(2).GetComponent<Button>();
                int index = i;
                button.onClick.AddListener(() => PurchaseItem(shopItemList[index]));

                if (shopItemList[i].isPurchased == true)
                {
                    g.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    g.transform.GetChild(2).GetComponent<Button>().interactable = false;
                }
                else
                {
                    g.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    g.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItemList[i].priceItem.ToString();
                    g.transform.GetChild(2).GetComponent<Button>().interactable = true;
                }
            }

            SaveShopData(); // Save the default shop data to the file
        }
    }
}