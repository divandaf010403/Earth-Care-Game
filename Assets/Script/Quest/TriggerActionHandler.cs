using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActionHandler : MonoBehaviour
{
    public int pointValue = 10;
    private void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.Play("AmbilItem");

        BersihSungai.Instance.AddPoints(pointValue);
    }
}
