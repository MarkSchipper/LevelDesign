using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quest
{
    public class ExploreQuest : BaseQuest
    {
        

        public static void Explore()
        {
            int _tmpID = 0;

            Quest.QuestDatabaseManager.GetAllQuests();

            _allZones = GameObject.FindGameObjectsWithTag("Zone");
            _zoneNames = new string[_allZones.Length];
            for (int i = 0; i < _allZones.Length; i++)
            {
                _zoneNames[i] = _allZones[i].GetComponent<Zone>().ReturnName();
            }
            GUILayout.Space(20);
            _zoneSelectedIndex = EditorGUILayout.Popup("Which Zone to explore?: ", _zoneSelectedIndex, _zoneNames);

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


            EditorGUILayout.BeginHorizontal(GUILayout.Width(350));
            GUILayout.Label("Auto Complete Quest?:");
            if (!_questGameStart)
            {
                _questAutoComplete = EditorGUILayout.Toggle(_questAutoComplete);
            }
            else
            {
                GUILayout.Label("Auto Complete is On by default");
            }
            EditorGUILayout.EndHorizontal();
            
            if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
            {
                if (GUILayout.Button("SAVE QUEST"))
                {
                    if (!_questGameStart)
                    {

                        Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Explore, "", 0, "", false, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete,  0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        Quest.QuestDatabaseManager.UpdateQuestZone(_allZones[_zoneSelectedIndex], Quest.QuestDatabaseManager.ReturnLastQuestID());
                    }
                    if (_questGameStart)
                    {
                        Quest.QuestDatabaseManager.AddQuest(_questTitle, _questText, QuestType.Explore, "", 0, "", true, false, _zoneNames[_zoneSelectedIndex], true, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, "Single", "Start", _tmpID);
                        Quest.QuestDatabaseManager.UpdateQuestZone(_allZones[_zoneSelectedIndex], Quest.QuestDatabaseManager.ReturnLastQuestID());
                    }
                    Quest.QuestSystem.ClearAll();
                }
            }

        }
    }
}
