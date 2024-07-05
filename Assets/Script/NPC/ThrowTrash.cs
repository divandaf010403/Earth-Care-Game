using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrash : MonoBehaviour
{
    [SerializeField] Transform spawnPos;

    public void ThrowTrashTimer(GameObject objToSpawn, Transform trashParent, int siblingIndex)
    {
        // Instantiate the object at the spawn position with no rotation
        GameObject spawnedObject = Instantiate(objToSpawn, spawnPos.position, Quaternion.identity);
        
        // Set the parent of the instantiated object to trashParent
        spawnedObject.transform.SetParent(trashParent);

        // Save spawned item data
        SpwnNPC.Instance.SaveSpawnedItem(spawnedObject, trashParent, siblingIndex);

        // Activate TrashManager
        spawnedObject.transform.GetComponent<TrashManager>().enabled = true;
    }
}
