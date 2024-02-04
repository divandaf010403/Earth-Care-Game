using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractKey : MonoBehaviour
{
    public bool HasKey = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) HasKey = !HasKey;
    }
}
