using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private Inventory _Inventory;
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            IInventoryItem item = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>().Item;
            if (item != null)
            {
                _Inventory.RemoveItem(item);
                item.OnDrop();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _Inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
