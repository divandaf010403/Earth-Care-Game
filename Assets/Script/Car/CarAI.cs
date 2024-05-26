using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public float safeDistance = 2f;
    public float carSpeed = 5f;

    public GameObject currentTrafficRoute;
    public GameObject nextWaypoint;
    public int currentWaypointNumber;

    private void Update() 
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, safeDistance);

        if (hit.transform)
        {
            if (hit.transform.tag == "Car")
            {
                Stop();
            }
        }
        else
        {
            Move();
        }
    }

    void Stop()
    {
        transform.position += new Vector3(0, 0, 0);
    }

    void Move()
    {
        transform.position += new Vector3(0, 0, carSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + safeDistance));
    }
}
