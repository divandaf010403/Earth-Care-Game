using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player, cameraTrans;

    void Update()
    {
        cameraTrans.LookAt(player);
    }
}
