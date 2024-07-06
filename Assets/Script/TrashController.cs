using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour, Interactable, IInventoryItem
{
    [SerializeField]
    private string _prompt;
    public string InteractionPrompt => _prompt;

    [SerializeField]
    private string _imagePath; // Menyimpan path dari sprite
    public string itemImagePath => _imagePath;

    public string jenisSampahNama;
    public string typeSampahTxt;
    public int totalSampahTxt = 1;

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

    public string itemName
    {
        get { return gameObject.name; }
    }

    public Sprite image
    {
        get { return GetImage(); } // Gunakan method GetImage untuk memuat sprite
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

    // Implementasi method GetImage untuk memuat sprite dari path
    public Sprite GetImage()
    {
        return Resources.Load<Sprite>(itemImagePath);
    }

    public void OnPickup()
    {
        Debug.Log("Item picked up!");
        gameObject.SetActive(false);
        // Destroy(gameObject);
    }

    public void OnPickupDestroy()
    {
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

    private void DestroyAfterDelay()
    {
        Destroy(gameObject);
    }
}
