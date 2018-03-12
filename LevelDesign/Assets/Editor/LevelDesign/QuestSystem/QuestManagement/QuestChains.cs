using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{

    public class QuestChains : BaseQuest
    {
        private static bool[] _questChainFoldout;
        private static bool _retrievedAllQuests;

        public static void Chains()
        {
            _questChainFoldout = new bool[100];
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabaseManager.ClearAll();
                Quest.QuestDatabaseManager.GetAllQuests();
                _retrievedAllQuests = true;
            }
            for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllQuestID().Count; i++)
            {
                if (Quest.QuestDatabaseManager.ReturnAllQuestChain()[i] == QuestChain.Chain)
                {
                    if (Quest.QuestDatabaseManager.ReturnAllQuestChainTypes()[i] == QuestChainType.Start)
                    {
                        if (GUILayout.Button("RESET [ " + Quest.QuestDatabaseManager.ReturnAllQuestTitles()[i] + " ]"))
                        {
                            Quest.QuestDatabaseManager.ResetQuestChain(Quest.QuestDatabaseManager.ReturnAllQuestID()[i]);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("None of the Quests are part of a chain");
                }
            }
        }
    }
}
