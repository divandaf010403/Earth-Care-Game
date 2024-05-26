using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    public GameObject[] cars;

    private void Start() 
    {
        Spawn();
    }

    void Spawn()
    {
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            int rand = Random.Range(0, cars.Length);
            cars[rand].GetComponent<CarAI>().currentTrafficRoute = this.gameObject;
            cars[rand].GetComponent<CarAI>().currentWaypointNumber = i;
            Instantiate(cars[rand], transform.GetChild(i).position, transform.GetChild(i).rotation);
        }
    }
}
