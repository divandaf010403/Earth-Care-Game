using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour, Interactable, IInventoryItem
{
    private List<string> jenisSampahList = new List<string> { "Organik", "Anorganik" };
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public Sprite _image = null;
    public string jenisSampahNama;

    public bool Interact(Interactions interactions)
    {
        var interactKey = interactions.GetComponent<InteractKey>();

        if (interactKey == null) return false;

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

    public string name
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
