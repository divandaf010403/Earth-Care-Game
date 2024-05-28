using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IQuestHandler
{
    Camera QuestCamera { get; }
    Transform QuestPlayerPosition { get; }
    Transform QuestCameraPosition { get; }
    Transform TrashSpawner { get; }
    Transform IsActiveTrigger { get; }

    int GetWaktuQuest();
    int GetScoreQuest();
    Sprite GetImageRequiredQuest();

    // void OnQuestBeforeStart();
    void OnQuestStart();
    void OnQuestFinish();
}

public class QuestEventArgs : EventArgs
{
    public QuestEventArgs(IQuestHandler questHandler)
    {
        IQuestHandler = questHandler;
    }

    public IQuestHandler IQuestHandler { get; }
}
