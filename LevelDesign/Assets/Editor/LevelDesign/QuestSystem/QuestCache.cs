using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest.QuestItem))]
public class QuestCache : Editor {

    public override void OnInspectorGUI()
    {
        Quest.QuestItem _quest = (Quest.QuestItem)target;

        base.OnInspectorGUI();
        if(GUILayout.Button("CLEAR CACHE"))
        {
            _quest.ClearCache();
        }
    }
}
