using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryVariable : MonoBehaviour
{
    public IInventoryItem Item { get; set; }
    [SerializeField] public int itemId;
    [SerializeField] public string itemName;
    [SerializeField] public string jenisSampah;
    [SerializeField] public int totalSampah;
}
