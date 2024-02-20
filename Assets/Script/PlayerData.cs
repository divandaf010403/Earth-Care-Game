using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] position;
    public float[] rotation;

    public PlayerData(MainCharMovement mainChar)
    {
        position = new float[3];
        position[0] = mainChar.transform.position.x;
        position[1] = mainChar.transform.position.y;
        position[2] = mainChar.transform.position.z;

        rotation = new float[3];
        rotation[0] = mainChar.transform.rotation.eulerAngles.x;
        rotation[1] = mainChar.transform.rotation.eulerAngles.y;
        rotation[2] = mainChar.transform.rotation.eulerAngles.z;
    }
}
