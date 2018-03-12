using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quest
{
    public class QuestDelete : BaseQuest
    {
        private static bool _retrievedAllQuests = false;
        private static bool[] _activeQuestActive;
        private static List<int> _activeID = new List<int>();

        public static void Delete()
        {
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabaseManager.ClearAll();
                Quest.QuestDatabaseManager.GetAllQuests();
                _activeQuestActive = new bool[Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count];
                for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count; i++)
                {
                    _activeID.Add(Quest.QuestDatabaseManager.ReturnAllQuestID()[i]);
                }
                _retrievedAllQuests = true;
            }
            if (Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count > 0)
            {
                for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(550));
                    GUILayout.Label("Quest ID: " + Quest.QuestDatabaseManager.ReturnAllQuestID()[i], GUILayout.Width(150));
                    GUILayout.Label("Quest Title: " + Quest.QuestDatabaseManager.ReturnAllQuestTitles()[i], GUILayout.Width(200));
                    GUILayout.Label("Select: ");
                    _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("DELETE SELECTED"))
                {
                    for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count; i++)
                    { 
                        Quest.QuestDatabaseManager.DeleteQuests(_activeID[i], _activeQuestActive[i]);
                        _retrievedAllQuests = false;
                    }
                }
            }
        }
    }
}
