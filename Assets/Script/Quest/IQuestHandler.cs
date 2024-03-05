using System;
using UnityEngine;

public interface IQuestHandler
{
    Camera QuestCamera { get; }
    Transform QuestPlayerPosition { get; }
    Transform QuestCameraPosition { get; }
    Transform TrashSpawner { get; }
    Transform IsActiveTrigger { get; }

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
