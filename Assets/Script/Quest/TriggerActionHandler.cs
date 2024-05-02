using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.Play("AmbilItem");
    }
}
