using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPrefabManager : MonoBehaviour
{
    public PrefabManager prefabManager;

    void DestroyObject(GameObject obj)
    {
        prefabManager.UpdateStatus(obj.name, true);
    }
}
