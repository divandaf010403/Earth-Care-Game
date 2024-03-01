using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestHandler {
    void OnQuestStart();
    void OnQuestFinish();
}

public class QuestEventArgs : EventArgs
{
    public QuestEventArgs(IQuestHandler questHandler)
    {
        iQuestHandler = questHandler;
    }
    public IQuestHandler iQuestHandler;
}