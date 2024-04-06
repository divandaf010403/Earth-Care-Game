using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform mainCharacter;
    public Transform mainCamera;
    public Transform camera2;

    [Header("UI Controller")]
    public GameObject mainUI;
    public GameObject pilahSampahUI;
    public GameObject bersihSungaiUI;

    [Header("Inventory")]
    public Transform inventory;
    public Transform inventoryExt;

    private void Start() {
        // inventoryExt.gameObject.SetActive(false);
    }

    public void openCloseinventoryExt(bool isActive) {
        inventoryExt.gameObject.SetActive(isActive);
        inventoryExt.localPosition = new Vector3(0f, 0f, 0f);
    }
}
