using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNextWaypoint : MonoBehaviour
{
    public GameObject[] waypoints;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Car")
        {
            int rand = Random.Range(0, waypoints.Length);
            other.gameObject.GetComponent<CarAI>().nextWaypoint = waypoints[rand];
        }
    }
}
