using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quest
{
    public class QuestActives : BaseQuest
    {
        private static bool _retrievedAllQuests;
        private static bool[] _activeQuestActive;
        private static List<int> _activeID = new List<int>();

        public static void ActiveQuests()
        {
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabaseManager.ClearAll();
                Quest.QuestDatabaseManager.GetActiveQuests();
                _activeQuestActive = new bool[Quest.QuestDatabaseManager.ReturnAllActiveQuestID().Count];

                for (int i = 0; i < _activeQuestActive.Length; i++)
                {
                    _activeQuestActive[i] = true;
                    _activeID.Add(Quest.QuestDatabaseManager.ReturnAllActiveQuestID()[i]);
                }
                _retrievedAllQuests = true;
            }

            for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllActiveQuestID().Count; i++)
            {

                GUILayout.BeginHorizontal(GUILayout.Width(250));
                GUILayout.Label("Quest ID: " + Quest.QuestDatabaseManager.ReturnAllActiveQuestID()[i]);
                GUILayout.Label("Quest Title: " + Quest.QuestDatabaseManager.ReturnAllActiveQuestTitles()[i]);
                GUILayout.Space(50);
                GUILayout.Label("Active: ");
                _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("SAVE CHANGES"))
            {
                for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllActiveQuestID().Count; i++)
                {
                    Quest.QuestDatabaseManager.UpdateActiveQuests(_activeID[i], _activeQuestActive[i]);
                }
            }

            if (GUILayout.Button("BACK"))
            {
                _retrievedAllQuests = false;
            }
        }
    }
}