using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Quest
{
    public class QuestEdit : BaseQuest
    {
        private static Vector2 _scrollPos;
        private static bool _retrievedAllQuests;
        private static int _oldNpcID;
        private static string[] _allItemNames;
        private static string _questZone;

        private static bool _noObjectsFound;
        private static int _selectedQuestIndex;

        public static void EditQuest()
        {
            int _tmpID = 0;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabaseManager.ClearAll();
                Quest.QuestDatabaseManager.GetAllQuests();
                Quest.QuestDatabaseManager.GetQuestItems(); 

                if (Quest.QuestDatabaseManager.ReturnAllQuestAmount().Count > 0)
                {
                    // SET ALL VALUES ONCE SO WE CAN EDIT THEM
                    _questTitle = Quest.QuestDatabaseManager.ReturnAllQuestTitles()[_selectedQuestIndex];
                    _questText = Quest.QuestDatabaseManager.ReturnAllQuestTexts()[_selectedQuestIndex];
                    _questType = Quest.QuestDatabaseManager.ReturnAllQuestTypes()[_selectedQuestIndex];
                    _questComplete = Quest.QuestDatabaseManager.ReturnAllQuestCompleteTexts()[_selectedQuestIndex];
                    _allItemNames = Quest.QuestDatabaseManager.ReturnAllQuestItemNames().ToArray();
                    _questZone = Quest.QuestDatabaseManager.ReturnAllQuestZones()[_selectedQuestIndex];
                    _questAutoComplete = Quest.QuestDatabaseManager.ReturnAllQuestZoneAutoComplete()[_selectedQuestIndex];
                    _qItemAmount = Quest.QuestDatabaseManager.ReturnAllQuestItemAmount()[_selectedQuestIndex];
                    _questItemCollectAmount = Quest.QuestDatabaseManager.ReturnAllQuestCollected()[_selectedQuestIndex];
                    _questReward = Quest.QuestDatabaseManager.ReturnAllQuestRewards()[_selectedQuestIndex];
                    _goldAmount = Quest.QuestDatabaseManager.ReturnAllQuestGold()[_selectedQuestIndex];
                    _expAmount = Quest.QuestDatabaseManager.ReturnAllQuestExperience()[_selectedQuestIndex];
                    _questEnabled = Quest.QuestDatabaseManager.ReturnAllQuestEnabled()[_selectedQuestIndex];
                    _oldNpcID = Quest.QuestDatabaseManager.ReturnAllQuestNpcID()[_selectedQuestIndex];
                    _chain = Quest.QuestDatabaseManager.ReturnAllQuestChain()[_selectedQuestIndex];
                    _chainType = Quest.QuestDatabaseManager.ReturnAllQuestChainTypes()[_selectedQuestIndex];
                    _questChainSelectIndex = Quest.QuestDatabaseManager.ReturnAllQuestChainFollowupID()[_selectedQuestIndex];
                    _killAmount = _questItemCollectAmount;
                    _killSelect = KillQuestSelection.InGame;
                    if (_oldNpcID == -1)
                    {
                        _questGameStart = true;
                    }
                }
                _retrievedAllQuests = true;
            }

            if (Quest.QuestDatabaseManager.ReturnAllQuestAmount().Count > 0)
            {
                _selectedQuestIndex = EditorGUILayout.Popup("Edit Quest: ", _selectedQuestIndex, Quest.QuestDatabaseManager.ReturnAllQuestTitles().ToArray());

                if (GUI.changed)
                {
                    _retrievedAllQuests = false;
                }

                GUILayout.Space(20);

                EditorGUILayout.Separator();

                _questGameStart = EditorGUILayout.Toggle("Quest Game Start? ", _questGameStart);

                GUILayout.Space(20);

                GUILayout.Label("Quest Title");
                _questTitle = EditorGUILayout.TextField(_questTitle);

                GUILayout.Label("Quest Dialogue");
                _questText = EditorGUILayout.TextArea(_questText, GUILayout.Height(100));

                GUILayout.Label("Quest Complete Dialogue");
                _questComplete = EditorGUILayout.TextArea(_questComplete, GUILayout.Height(100));

                _questType = (QuestType)EditorGUILayout.EnumPopup("Type of Quest: ", _questType);

                #region COLLECT
                if (_questType == QuestType.Collect)
                {

                    _qItemAmount = (QuestItemAmount)EditorGUILayout.EnumPopup("Amount to Collect: ", _qItemAmount);

                    if (_qItemAmount == QuestItemAmount.Single)
                    {
                        _questItemIndex = EditorGUILayout.Popup("Which Item: ", _questItemIndex, _allItemNames);
                        if (GameObject.Find("QuestItem_" + Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex] + "_" + _questTitle + "_" + 0) == null)
                        {
                            GUILayout.Space(20);
                            if (GUILayout.Button("Add Item to the Game"))
                            {
                                _questItemCollectAmount = 1;
                                Quest.QuestSystem.AddQuestItems(Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], 1, true, Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex]);
                            }
                        }
                        else
                        {
                            _questItemCollectAmount = 1;
                        }
                    }

                    if (_qItemAmount == QuestItemAmount.Multiple)
                    {
                        _questItemCollectAmount = EditorGUILayout.IntField("How many to collect:", _questItemCollectAmount);

                        for (int i = 0; i < _questItemCollectAmount; i++)
                        {
                            if (GameObject.Find("QuestItem_" + Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex] + "_" + _questTitle + "_" + i + "") == null)
                            {
                                if (!_noObjectsFound)
                                {
                                    _noObjectsFound = true;
                                }

                            }
                            else
                            {

                            }
                        }

                        if (_noObjectsFound)
                        {
                            if (GUILayout.Button("The Quest Items do not exist: Click to Add"))
                            {

                                Quest.QuestSystem.AddQuestItems(Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], _questItemCollectAmount, true, Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex]);

                            }
                        }
                    }
                }


                #endregion

                #region EXPLORE
                if (_questType == QuestType.Explore)
                {
                    _allZones = GameObject.FindGameObjectsWithTag("Zone");
                    _zoneNames = new string[_allZones.Length];
                    for (int i = 0; i < _allZones.Length; i++)
                    {
                        _zoneNames[i] = _allZones[i].GetComponent<Zone>().ReturnName();
                    }
                    GUILayout.Space(20);
                    _zoneSelectedIndex = EditorGUILayout.Popup("Which Zone to explore?: ", _zoneSelectedIndex, _zoneNames);

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(360));
                    GUILayout.Label("Auto Complete Quest?:");
                    _questAutoComplete = EditorGUILayout.Toggle(_questAutoComplete);
                    EditorGUILayout.EndHorizontal();
                }
                #endregion

                #region KILL
                if (_questType == QuestType.Kill)
                {

                    _killSelect = (KillQuestSelection)EditorGUILayout.EnumPopup("Select Enemy From: ", _killSelect);

                    #region INGAME

                    if (_killSelect == KillQuestSelection.InGame)
                    {
                        _killAllEnemies = GameObject.FindGameObjectsWithTag("EnemyMelee");
                        for (int i = 0; i < _killAllEnemies.Length; i++)
                        {
                            if (_killAllEnemies[i].transform.parent.name == "ENEMIES")
                            {

                                string[] _splitArray = _killAllEnemies[i].name.ToString().Split(char.Parse("_"));
                                _killAllEnemiesName.Add(_splitArray[0]);
                                //_killAllEnemiesName.Add(_killAllEnemies[i].name.ToString().Remove(_killAllEnemies[i].name.ToString().Length - 5));
                            }
                        }

                        _killSelectIndex = EditorGUILayout.Popup("Which Enemy?: ", _killSelectIndex, _killAllEnemiesName.ToArray());



                    }

                    #endregion

                    #region DATABASE

                    if (_killSelect == KillQuestSelection.Database)
                    {
                        EnemyCombat.EnemyDatabase.GetAllEnemies();

                        _killSelectIndex = EditorGUILayout.Popup("Which Enemy: ", _killSelectIndex, EnemyCombat.EnemyDatabase.ReturnAllEnemyNames().ToArray());

                    }

                    if (_qItemAmount > 0)
                    {
                        _killAmount = EditorGUILayout.IntField("Amount to Kill: ", _killAmount);
                    }

                    #endregion
                }
                #endregion

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
                        if (_questType == QuestType.Explore)
                        {
                            Quest.QuestDatabaseManager.SaveQuest(Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex], _questTitle, _questText, _questType, "", 0, "", false, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete,  0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        }
                        if (_questType == QuestType.Collect)
                        {
                            Quest.QuestDatabaseManager.SaveQuest(Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex], _questTitle, _questText, _questType, Quest.QuestDatabaseManager.ReturnAllQuestItemPrefab()[_questItemIndex], _questItemCollectAmount, "", true, false, "", false,  0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        }
                        if (_questType == QuestType.Kill)
                        {
                            if (_killSelect == KillQuestSelection.InGame)
                            {
                                Quest.QuestDatabaseManager.SaveQuest(Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex], _questTitle, _questText, _questType, "", _killAmount, _killAllEnemiesName[_killSelectIndex], true, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                            }

                            if (_killSelect == KillQuestSelection.Database)
                            {
                                Quest.QuestDatabaseManager.SaveQuest(Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectedQuestIndex], _questTitle, _questText, _questType, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), true, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                            }
                        }
                        _retrievedAllQuests = false;
                        Quest.QuestSystem.ClearAll();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
