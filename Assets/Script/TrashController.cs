using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour, Interactable, IInventoryItem
{
    [SerializeField]
    private string _prompt;
    public string InteractionPrompt => _prompt;
    public Sprite _image = null;
    public string jenisSampahNama;
    public string typeSampahTxt;
    public int totalSampahTxt = 1;

    public InventoryItemData GetItemData()
    {
        InventoryItemData itemData = new InventoryItemData();
        itemData.itemName = itemName;
        itemData.jenisSampah = jenisSampahNama;
        itemData.typeSampah = typeSampahTxt;
        itemData.jumlahItem = totalSampahTxt;
        // Populate other fields as needed
        return itemData;
    }

    public bool Interact(Interactions interactions)
    {
        var interactKey = interactions.GetComponent<InteractKey>();

        if (interactKey == null)
            return false;

        if (interactKey.HasKey)
        {
            Debug.Log("Mengambil Sampah");
            return true;
        }

        Debug.Log("No Key");
        return false;
    }

    public void Interact(GameObject go)
    {
        Debug.Log("Item picked up!");
        Destroy(go);
        Debug.Log(go.transform.parent.gameObject.name);
    }

    public string itemName
    {
        get { return gameObject.name; }
    }

    public Sprite image
    {
        get { return _image; }
    }

    public string jenisSampah
    {
        get { return jenisSampahNama; }
    }

    public string typeSampah
    {
        get { return typeSampahTxt; }
    }

    public int jumlahItem
    {
        get { return totalSampahTxt; }
    }

    public void OnPickup()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void OnDrop()
    {
        //RaycastHit hit = new RaycastHit();
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Physics.Raycast(ray, out hit, 1000))
        //{
        //    gameObject.SetActive(true);
        //    gameObject.transform.position = hit.point;
        //}

        GameObject mainCharacter = GameObject.Find("MainCharacter");

        if (mainCharacter != null)
        {
            float radius = 2f;
            Vector3 randomOffset = Random.insideUnitSphere * radius;
            randomOffset.y = 0f; // Untuk memastikan objek tetap di tingkat yang sama dengan karakter utama

            Vector3 dropPosition = mainCharacter.transform.position + randomOffset;

            gameObject.SetActive(true);
            gameObject.transform.position = dropPosition;
        }
        else
        {
            Debug.LogError("MainCharacter not found!");
        }
    }
}
