using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActionHandler : MonoBehaviour
{
    public int pointValue = 10;
    private void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);

        Animator anim = transform.parent.GetComponent<Animator>();
        
        StartCoroutine(HandleAnimation(anim));
    }

    private IEnumerator HandleAnimation( Animator anim)
    {
        GameVariable.speed = 0;
        anim.Play("AmbilItem");

        // Wait until the current animation state matches "AmbilItem"
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("AmbilItem"))
        {
            yield return null;
        }

        // Get the length of the current animation
        float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        GameVariable.speed = 5;
        BersihSungai.Instance.AddPoints(pointValue);
    }
}
