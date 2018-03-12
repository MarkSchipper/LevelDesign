using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace Quest
{

    public enum KillQuestSelection
    {
        None,
        InGame,
        Database,
    }

    public class QuestSystem : EditorWindow
    {
        // Menu switching
        private bool _addingQuest = false;
        private bool _editQuest = false;
        private bool _activeQuests = false;
        private bool _deleteQuests = false;
        private bool _viewQuestChains = false;
        private bool _resetQuest = false;

        // quest type select index
        private static QuestType _questType;
        private static int _questItemIndex;

        private static QuestItemAmount _qItemAmount;
        private static int _questItemCollectAmount;

        private static int _oldNpcID;

        // Quest rewards
        private static QuestReward _questReward;
        private static int _goldAmount;
        private static int _expAmount;

        private static QuestChain _chain;
        private static QuestChainType _chainType;
        private static int _questChainSelectIndex;

        private static int _actorSelectionIndex;
        private static int _selectedActiveQuestIndex;
        private static bool[] _activeQuestActive;

        private static bool[] _questChainFoldout;

        private static string _questTitle;
        private static string _questText;
        private static string _questComplete;
        private static string _questZone;

        private static bool _questEnabled = false;
        private static bool _questGameStart = false;

        private static Vector2 _scrollPos;

        // fetch all quests
        private static int _selectedQuestIndex;
        private static bool _retrievedAllQuests = false;
        private static string[] _allItemNames;
        private static string[] _allNpcNames;

        private static List<int> _activeID = new List<int>();

        private static bool _noObjectsFound = false;

        private static List<GameObject> _createdQuestItems = new List<GameObject>();

        // Explore Quests

        private static GameObject[] _allZones;
        private static string[] _zoneNames;
        private static int _zoneSelectedIndex;
        private static bool _questAutoComplete;

        private static GameObject QuestObject;

        // kill quests

        private static KillQuestSelection _killSelect;
        private static int _killSelectIndex;
        private static GameObject[] _killAllEnemies;
        private static List<string> _killAllEnemiesName = new List<string>();
        private static int _killAmount;

        private GUISkin _skin;

        
        static void ShowEditor()
        {
            QuestSystem _qSystem = EditorWindow.GetWindow<QuestSystem>();
        }

        void OnEnable()
        {

            // Clear the current Lists in the Database
            Quest.QuestDatabaseManager.ClearAll();

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
        }
     
        //////////////////////////////////////////////////////////////////////////////////////
        //                                                                                  //
        // Add Quest Item void                                                              //
        // _obj = the object from the dropdown menu ( Quest Item object )                   //
        // _amount = how many to create                                                     //
        // _edit = are we editing a quest or creating a new quest                           //
        // _questID = if we are editing, we need the quest  ID                              //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        public static void AddQuestItems(string _obj, int _amount, bool _edit, int _questID)
        {
            for (int i = 0; i < _amount; i++)
            {
                // if there are no items

                if (GameObject.Find("QuestItem_" + _obj + "_" + i + "") == null)
                {
                    // instantiate it
                    QuestObject = Instantiate(Resources.Load("Collectables/QuestItems/" + _obj, typeof(GameObject))) as GameObject;

                    // We give the obj a temp name since we want to have it a unique name ( quest title )
                    QuestObject.name = "tmpQuestItem" + _obj + "_" + i + "";

                    // Parenting
                    if (GameObject.Find("QuestItems") == null)
                    {
                        GameObject _questItemParent = new GameObject();
                        _questItemParent.name = "QuestItems";
                        QuestObject.transform.parent = GameObject.Find("QuestItems").transform;

                    }
                    else
                    {
                        QuestObject.transform.parent = GameObject.Find("QuestItems").transform;
                    }

                    // Add the AddComponent QuestItem 
                    QuestObject.AddComponent<Quest.QuestItem>();
                    
                    _createdQuestItems.Add(QuestObject);
                    
                    // If we are editing
                    if(_edit)
                    {
                        QuestObject.GetComponent<Quest.QuestItem>().SetQuestID(_questID);
                    }
                }
            }
        }

        public static void ShowAddQuest()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _questType = (QuestType)EditorGUILayout.EnumPopup("Type of quest:", _questType);

            if (_questType != QuestType.None)
            {

                //////////////////////////////////////////////////////////
                //                                                      //
                //                      COLLECTING QUEST                //
                //                                                      //
                //////////////////////////////////////////////////////////

                if (_questType == QuestType.Collect)
                {
                    Quest.CollectQuest.Collect();
                }

                //////////////////////////////////////////////////////////
                //                                                      //
                //                      EXPLORE QUEST                   //
                //                                                      //
                //////////////////////////////////////////////////////////

                if (_questType == QuestType.Explore)
                {
                    Quest.ExploreQuest.Explore();
                }

                if(_questType == QuestType.Kill)
                {
                    Quest.KillQuest.Kill();
                }

            }
            EditorGUILayout.EndScrollView();
            
        }
        
        public static void ShowEditQuest()
        {
            Quest.QuestEdit.EditQuest();
        }

        public static void ShowActiveQuests()
        {
            Quest.QuestActives.ActiveQuests();
        }

        public static void ShowDeleteQuests()
        {
            Quest.QuestDelete.Delete();
        }

        public static void ShowQuestChains()
        {
            Quest.QuestChains.Chains();
        }

        public static void ShowResetQuest()
        {
            Quest.QuestReset.Reset();
        }

        public static void ClearAll()
        {
            _questTitle = "";
            _questText = "";
            _questComplete = "";
            _questType = QuestType.None;
            _actorSelectionIndex = 0;
            _zoneSelectedIndex = 0;
            _qItemAmount = 0;
            _questItemCollectAmount = 0;
            _questReward = QuestReward.None;
            _goldAmount = 0;
            _expAmount = 0;
            _questEnabled = false;
            _oldNpcID = 0;
            _chain = QuestChain.Single;
            _chainType = QuestChainType.Start;
            _questChainSelectIndex = 0;
            _questGameStart = false;
        }

        public static void SetRetrievedQuests(bool _set)
        {
            _retrievedAllQuests = _set;
        }

        
    }

}
