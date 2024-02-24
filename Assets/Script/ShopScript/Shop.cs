using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class Shop : MonoBehaviour
{
    [System.Serializable] public class ShopItem
    {
        public Sprite imageItem;
        public string nameItem;
        public int priceItem;
        public bool isPurchased = false;

        public ShopItem(ShopItem shopItem)
        {
            this.imageItem = shopItem.imageItem;
            this.nameItem = shopItem.nameItem;
            this.priceItem = shopItem.priceItem;
            this.isPurchased = shopItem.isPurchased;
        }
    }

    [SerializeField] public List<ShopItem> shopItemList;

    [SerializeField] public GameObject ItemTemplate;
    GameObject g;
    [SerializeField] public Transform ShopScrollView;

    void Start()
    {
        
    }

    void PurchaseItem(int index)
    {
        shopItemList[index].isPurchased = true;
        SaveShopData();
        ShopScrollView.GetChild(index).GetChild(2).GetComponent<Button>().interactable = false;
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
            foreach (Shop.ShopItem item in loadedShopData)
            {
                GameObject newItem = Instantiate(ItemTemplate, ShopScrollView);
                newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.nameItem;
                newItem.transform.GetChild(1).GetComponent<Image>().sprite = item.imageItem;
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
                g.transform.GetChild(0).GetComponent<Image>().sprite = shopItemList[i].imageItem;
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
