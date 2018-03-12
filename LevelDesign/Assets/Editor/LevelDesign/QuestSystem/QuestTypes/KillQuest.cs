using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quest
{
    public class KillQuest : BaseQuest
    {
        public static void Kill()
        {

            // _tmpID = temp int to store the QuestID
            int _tmpID = 0;

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

            #endregion

            if (_killSelect != KillQuestSelection.None)
            {
                _killAmount = EditorGUILayout.IntField("Amount to Kill: ", _killAmount);

                if (_killAmount > 0)
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

                                if (_killSelect == KillQuestSelection.InGame)
                                {
                                    Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Kill, "", _killAmount, _killAllEnemiesName[_killSelectIndex], false, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                                }

                                if (_killSelect == KillQuestSelection.Database)
                                {
                                    Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Kill, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), false, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                                }
                            }

                            if (_questGameStart)
                            {
                                if (_killSelect == KillQuestSelection.InGame)
                                {
                                    Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Kill, "", _killAmount, _killAllEnemiesName[_killSelectIndex], true, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                                }

                                if (_killSelect == KillQuestSelection.Database)
                                {
                                    Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Kill, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), true, false, "", false, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                                }

                            }

                            Quest.QuestSystem.ClearAll();

                        }
                    }
                }
            }
        }

    }
}
