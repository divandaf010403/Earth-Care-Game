using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [System.Serializable] class ShopItem
    {
        public Sprite imageItem;
        public string nameItem;
        public int priceItem;
        public bool isPurchased = false;
    }

    [SerializeField] List<ShopItem> shopItemList;

    GameObject ItemTemplate;
    GameObject g;
    [SerializeField] Transform ShopScrollView;

    void Start()
    {
        ItemTemplate = ShopScrollView.GetChild(0).gameObject;

        int lenghtItem = shopItemList.Count;
        for (int i = 0; i < lenghtItem;  i++)
        {
            g = Instantiate(ItemTemplate, ShopScrollView);
            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItemList[i].nameItem;
            //g.transform.GetChild(0).GetComponent<Image>().sprite = shopItemList[i].imageItem;
            g.transform.GetChild(2).GetComponent<Button>().interactable = shopItemList[i].isPurchased;
            g.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItemList[i].priceItem.ToString();
        }

        //Destroy(ItemTemplate);
    }
}
