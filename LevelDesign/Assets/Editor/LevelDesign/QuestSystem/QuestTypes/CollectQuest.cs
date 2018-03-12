using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quest
{
    public class CollectQuest : BaseQuest
    {
        public static bool _noObjectsFound;
        public static void Collect()
        {
            // Query the Database to get all QuestItems
            Quest.QuestDatabaseManager.GetQuestItems();
            Quest.QuestDatabaseManager.GetAllQuests();

            // _tmpID = temp int to store the QuestID
            int _tmpID = 0;

            _questItemIndex = EditorGUILayout.Popup("Which Item:", _questItemIndex, Quest.QuestDatabaseManager.ReturnAllQuestItemNames().ToArray());

            _qItemAmount = (QuestItemAmount)EditorGUILayout.EnumPopup("Amount to Collect: ", _qItemAmount);


            // If the player has to collect multiple objects

            if (_qItemAmount == QuestItemAmount.Multiple)
            {
                _questItemCollectAmount = EditorGUILayout.IntField("How many to collect:", _questItemCollectAmount);
                GUILayout.Space(20);
                if (_questItemCollectAmount > 0)
                {
                    for (int i = 0; i < _questItemCollectAmount; i++)
                    {
                        if (GameObject.Find("QuestItem_" + Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex] + "_" + i + "") == null)
                        {
                            if (!_noObjectsFound)
                            {
                                Debug.Log("NOTHING FOUND");
                                _noObjectsFound = true;
                            }
                        }
                    }
                    if (_noObjectsFound)
                    {
                        if (GUILayout.Button("Add Items to the Game"))
                        {
                            Quest.QuestSystem.AddQuestItems(Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], _questItemCollectAmount, false, 0);
                        }
                    }
                }
            }

            // Else if the player only has to collect one

            if (_qItemAmount == QuestItemAmount.Single)
            {
                GUILayout.Space(20);
                if (_questItemCollectAmount == 0)
                {
                    if (GUILayout.Button("Add Item to the Game"))
                    {
                        _questItemCollectAmount = 1;
                        Quest.QuestSystem.AddQuestItems(Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], 1, false, 0);
                    }
                }
            }

            // NPC selection
            if (_qItemAmount != QuestItemAmount.None)
            {

                GUILayout.Space(20);
                EditorGUILayout.Separator();

                GUILayout.Space(20);

                GUILayout.Label("Quest Title");
                _questTitle = EditorGUILayout.TextField(_questTitle);

                GUILayout.Label("Quest Dialogue");
                _questText = EditorGUILayout.TextArea(_questText, GUILayout.Height(100));

                GUILayout.Label("Quest Complete Dialogue");
                _questComplete = EditorGUILayout.TextArea(_questComplete, GUILayout.Height(100));

                EditorGUILayout.Separator();

                GUILayout.Label("Quest Reward", EditorStyles.boldLabel);
                _questReward = (QuestReward)EditorGUILayout.EnumPopup("Quest Reward", _questReward);

                if (_questReward != QuestReward.None)
                {
                    if (_questReward == QuestReward.Gold)
                    {
                        _goldAmount = EditorGUILayout.IntField("How much gold: ", _goldAmount);
                    }
                    if (_questReward == QuestReward.Experience)
                    {
                        _expAmount = EditorGUILayout.IntField("How much Exp: ", _expAmount);
                    }
                    if (_questReward == QuestReward.Both)
                    {
                        _goldAmount = EditorGUILayout.IntField("How much gold: ", _goldAmount);
                        _expAmount = EditorGUILayout.IntField("How much Exp: ", _expAmount);
                    }
                }
                if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                {
                    if (GUILayout.Button("SAVE QUEST"))
                    {
                        if (!_questGameStart)
                        {
                            Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Collect, Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], _questItemCollectAmount, "", false, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        }
                        if (_questGameStart)
                        {
                            Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Collect, Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], _questItemCollectAmount, "", true, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        }
                        if (_createdQuestItems.Count > 0)
                        {
                            for (int i = 0; i < _createdQuestItems.Count; i++)
                            {
                                _createdQuestItems[i].GetComponent<Quest.QuestItem>().SetQuestID(Quest.QuestDatabaseManager.ReturnLastQuestID());
                            }
                        }

                        for (int i = 0; i < _questItemCollectAmount; i++)
                        {
                            QuestObject.name = "QuestItem_" + Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex] + "_" + _questTitle + "_" + i;
                        }


                        Quest.QuestSystem.ClearAll();
                    }
                }
            }
        }
    }
}