using System;
using UnityEngine;

public interface IQuestHandler
{
    Camera QuestCamera { get; }
    Transform QuestPlayerPosition { get; }
    Transform QuestCameraPosition { get; }
    Transform IsActiveTrigger { get; }

    int GetWaktuQuest();
    int GetScoreQuest();
    Sprite GetImageRequiredQuest();
    void OnQuestStart();
    void OnQuestFinish();
    Transform GetTransform();
}

public class QuestEventArgs : EventArgs
{
    public QuestEventArgs(IQuestHandler questHandler)
    {
        IQuestHandler = questHandler;
    }

    public IQuestHandler IQuestHandler { get; }
}
