using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSystem : MonoBehaviour
{
    //public variables
    public LayerMask ground;
    public FootSystem otherFoot;
    public float stepDistance, stepHeight, stepLenght, footSpacing, speed;
    public Transform body;
    public Vector3 footOffset;

    //private variables
    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNornal, newNormal, currentNormal;
    float lerp;

    // Start is called before the first frame update
    void Start()
    {
        //inntialize value
        footSpacing = transform.localPosition.x;
        oldPosition = newPosition = currentPosition = transform.position;
        oldNornal = newNormal = newNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //updating position and normal
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, ground.value))
        {
            if(Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.isMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLenght * direction) + footOffset;
                newNormal = hit.normal;
            }
        }

        if(lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPosition = tempPos;
            currentNormal = Vector3.Lerp(oldNornal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNornal = newNormal;
        }
    }

    public bool isMoving()
    {
        return lerp < 1;
    }
}
