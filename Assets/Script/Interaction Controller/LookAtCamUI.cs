using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamUI : MonoBehaviour
{
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        var rotationUi = _cam.transform.rotation;
        transform.LookAt(transform.position + rotationUi * Vector3.forward, rotationUi * Vector3.up);
    }
}
