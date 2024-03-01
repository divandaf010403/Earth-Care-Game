using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilahSampah : MonoBehaviour, IQuestHandler
{
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    [SerializeField] public Transform trashSpawner;

    [Header("Component")]
    public GameController gc;


    public void OnQuestStart() {
        gc.mainCharacter.transform.position = questPlayerPosition.position;
        gc.mainCharacter.transform.rotation = questPlayerPosition.rotation;
        
    }

    public void OnQuestFinish() {

    }
    
}
