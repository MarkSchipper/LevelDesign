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

    public enum QuestType
    {
        None,
        Collect,
        Kill,
        Explore,
    }

    public enum QuestItemAmount
    {
        None,
        Single,
        Multiple,
    }

    public enum QuestReward
    {
        None,
        Gold,
        Experience,
        Both,
        
    }
#if UNITY_EDITOR
    public class QuestSystem : EditorWindow
    {
        // Menu switching
        private bool _addingQuest = false;
        private bool _editQuest = false;
        private bool _activeQuests = false;
        private bool _deleteQuests = false;

        // quest type select index
        
        private QuestType _questType;
        private int _questItemIndex;

        private QuestItemAmount _qItemAmount;
        private int _questItemCollectAmount;

        private int _oldNpcID;

        // Quest rewards
        private QuestReward _questReward;
        private int _goldAmount;
        private int _expAmount;
        

        private int _actorSelectionIndex;
        private int _selectedActiveQuestIndex;
        private bool[] _activeQuestActive;

        private string _questTitle;
        private string _questText;
        private string _questComplete;

        private bool _questEnabled = false;

        private Vector2 _scrollPos;

        // fetch all quests
        private int _selectedQuestIndex;
        private bool _retrievedAllQuests = false;
        private string[] _allItemNames;
        private string[] _allNpcNames;

        private List<int> _activeID = new List<int>();

        private bool _noObjectsFound = false;

        private List<GameObject> _createdQuestItems = new List<GameObject>();

        // Explore Quests

        private GameObject[] _allZones;
        private string[] _zoneNames;
        private int _zoneSelectedIndex;
        private bool _questAutoComplete;

        [MenuItem("Level Design/Quest System/Quest Manager")]
        static void ShowEditor()
        {
            QuestSystem _qSystem = EditorWindow.GetWindow<QuestSystem>();
        }

        void OnEnable()
        {
            Quest.QuestDatabase.ClearAll();
        }

        void OnGUI()
        {


            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            GUILayout.Label("Welcome to the Quest System", EditorStyles.boldLabel);

            if (!_addingQuest && !_editQuest && !_activeQuests && !_deleteQuests)
            {

                if (GUILayout.Button("Add Quest"))
                {
                    _addingQuest = true;
                }
                if(GUILayout.Button("Edit Quest"))
                {
                    _editQuest = true;
                }

                if(GUILayout.Button("View Active Quests"))
                {
                    _activeQuests = true;
                }

                if(GUILayout.Button(" Delete Quests"))
                {
                    _deleteQuests = true;
                }
            }
            

            // Add quest
            if(_addingQuest)
            {

                AddQuest();
            }

            // EDIT QUESTS

            if(_editQuest)
            {
                EditQuest();
            }

            if(_activeQuests)
            {
                ViewActiveQuests();
            }

            if(_deleteQuests)
            {
                DeleteQuests();
            }

            EditorGUILayout.EndScrollView();
        }

        void AddQuestItems(string _obj, int _amount)
        {
            for (int i = 0; i < _amount; i++)
            {

                if (GameObject.Find("QuestItem_" + _obj + "_" + i + "") == null)
                {
                    GameObject QuestObject = Instantiate(Resources.Load("Collectables/QuestItems/" + _obj, typeof(GameObject))) as GameObject;
                    QuestObject.name = "QuestItem_" + _obj + "_" + i + "";
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
                    QuestObject.AddComponent<Quest.QuestItem>();
                    
                    _createdQuestItems.Add(QuestObject);
                }
            }
        }

        void AddQuest()
        {
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
                    CollectQuest();
                }


                //////////////////////////////////////////////////////////
                //                                                      //
                //                      EXPLORE QUEST                   //
                //                                                      //
                //////////////////////////////////////////////////////////

                if (_questType == QuestType.Explore)
                {
                    ExploreQuest();
                }


            }
            if (GUILayout.Button("BACK"))
            {
                _addingQuest = false;
            }
        }

        void CollectQuest()
        {
            // Query the Database to get all QuestItems
            Quest.QuestDatabase.GetQuestItems();

            _questItemIndex = EditorGUILayout.Popup("Which Item:", _questItemIndex, Quest.QuestDatabase.ReturnQuestItemNames().ToArray());

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
                        if (GameObject.Find("QuestItem_" + Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex) + "_" + i + "") == null)
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
                            AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount);
                        }
                    }
                }
            }

            // Else if the player only has to collect one

            if (_qItemAmount == QuestItemAmount.Single)
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Add Item to the Game"))
                {
                    AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), 1);
                }
            }

            // NPC selection
            if (_qItemAmount != QuestItemAmount.None)
            {

                Quest.QuestDatabase.GetAllNpcs();
                GUILayout.Space(20);
                EditorGUILayout.Separator();
                _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, Quest.QuestDatabase.ReturnActorNames().ToArray());

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

                GUILayout.Label("Enable this quest?");
                _questEnabled = EditorGUILayout.Toggle(_questEnabled);

                if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                {
                    if (GUILayout.Button("SAVE QUEST"))
                    {
                        Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled);
                        Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));

                        if(_createdQuestItems.Count > 0)
                        {
                            for (int i = 0; i < _createdQuestItems.Count; i++)
                            {
                                _createdQuestItems[i].GetComponent<Quest.QuestItem>().SetQuestID(Quest.QuestDatabase.ReturnLastQuestID());
                            }
                        }
                    }
                }
            }
        }

        void EditQuest()
        {
            
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabase.ClearAll();
                Quest.QuestDatabase.GetAllQuests();
                Quest.QuestDatabase.GetQuestItems();
                Quest.QuestDatabase.GetAllNpcs();

                if (Quest.QuestDatabase.ReturnAllQuestsCount() > 0)
                {

                    // SET ALL VALUES ONCE SO WE CAN EDIT THEM
                    _questTitle = Quest.QuestDatabase.GetQuestTitle(_selectedQuestIndex);
                    _questText = Quest.QuestDatabase.GetQuestText(_selectedQuestIndex);
                    _questType = Quest.QuestDatabase.GetQuestType(_selectedQuestIndex);

                    _allItemNames = Quest.QuestDatabase.ReturnQuestItemNames().ToArray();
                    _allNpcNames = Quest.QuestDatabase.ReturnActorNames().ToArray();

                    _qItemAmount = Quest.QuestDatabase.GetAllQuestAmountTypes(_selectedQuestIndex);
                    _questItemCollectAmount = Quest.QuestDatabase.GetAllQuestAmounts(_selectedQuestIndex);
                    _questReward = Quest.QuestDatabase.GetQuestReward(_selectedQuestIndex);
                    _goldAmount = Quest.QuestDatabase.GetQuestGold(_selectedQuestIndex);
                    _expAmount = Quest.QuestDatabase.GetQuestExp(_selectedQuestIndex);
                    _questEnabled = Quest.QuestDatabase.GetQuestEnabled(_selectedQuestIndex);
                    _oldNpcID = Quest.QuestDatabase.GetNpcIdFromQuest(_selectedQuestIndex);
                }
                _retrievedAllQuests = true;
            }

            if (Quest.QuestDatabase.ReturnAllQuestsCount() > 0)
            {

                _selectedQuestIndex = EditorGUILayout.Popup("Edit Quest: ", _selectedQuestIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                if (GUI.changed)
                {
                    _retrievedAllQuests = false;
                }
                GUILayout.Space(20);


                EditorGUILayout.Separator();
                _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, _allNpcNames);

                GUILayout.Space(20);

                GUILayout.Label("Quest Title");
                _questTitle = EditorGUILayout.TextField(_questTitle);

                GUILayout.Label("Quest Dialogue");
                _questText = EditorGUILayout.TextArea(_questText, GUILayout.Height(100));

                _questType = (QuestType)EditorGUILayout.EnumPopup("Type of Quest: ", _questType);

                if (_questType == QuestType.Collect)
                {
                    _questItemIndex = EditorGUILayout.Popup("Which Item: ", _questItemIndex, _allItemNames);
                }

                _qItemAmount = (QuestItemAmount)EditorGUILayout.EnumPopup("Amount to Collect: ", _qItemAmount);

                if (_qItemAmount == QuestItemAmount.Multiple)
                {
                    _questItemCollectAmount = EditorGUILayout.IntField("How many to collect:", _questItemCollectAmount);

                    for (int i = 0; i < _questItemCollectAmount; i++)
                    {
                        if (GameObject.Find("QuestItem_" + Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex) + "_" + i + "") == null)
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

                            AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount);

                        }
                    }
                }

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

                GUILayout.Label("Quest Enabled?");
                _questEnabled = EditorGUILayout.Toggle(_questEnabled);

                if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                {
                    if (GUILayout.Button("SAVE QUEST"))
                    {
                        Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled);
                        if (_oldNpcID != Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex))
                        {
                            Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                        }
                        else
                        {
                            Quest.QuestDatabase.UpdateNPC(_oldNpcID);
                        }
                    }
                }
            }
                if (GUILayout.Button("BACK"))
                {
                    _editQuest = false;
                    _retrievedAllQuests = false;
                }
            
        }

        void ViewActiveQuests()
        {
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabase.ClearAll();
                Quest.QuestDatabase.GetAllActiveQuests();
                _activeQuestActive = new bool[Quest.QuestDatabase.ReturnActiveQuestIDS().Count];

                for (int i = 0; i < _activeQuestActive.Length; i++)
                {
                    _activeQuestActive[i] = true;
                    _activeID.Add(Quest.QuestDatabase.ReturnActiveQuestID(i));
                    
                }

                _retrievedAllQuests = true;
            }
            
            for (int i = 0; i < Quest.QuestDatabase.ReturnActiveQuestCount(); i++)
            {
                
                
                
                GUILayout.BeginHorizontal(GUILayout.Width(250));
                GUILayout.Label("Quest ID: " + Quest.QuestDatabase.ReturnActiveQuestID(i));
                GUILayout.Label("Quest Title: " + Quest.QuestDatabase.ReturnActiveQuestTitle(i));
                GUILayout.Space(50);
                GUILayout.Label("Active: ");
                _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                GUILayout.EndHorizontal();
                

            }
            
            if(GUILayout.Button("SAVE CHANGES"))
            {
                for (int i = 0; i < Quest.QuestDatabase.ReturnActiveQuestCount(); i++)
                {
                    Debug.Log(_activeID[i]);
                    Quest.QuestDatabase.UpdateActiveQuests(_activeID[i], _activeQuestActive[i]);
                }
            }

            if(GUILayout.Button("BACK"))
            {
                _activeQuests = false;
                _retrievedAllQuests = false;
            }
        }

        void DeleteQuests()
        {
            
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabase.ClearAll();
                Quest.QuestDatabase.GetAllQuests();
                _activeQuestActive = new bool[Quest.QuestDatabase.ReturnQuestTitles().Count];
                for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
                {
                    _activeID.Add(Quest.QuestDatabase.GetQuestID(i));

                }   
                _retrievedAllQuests = true;
            }

            for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(250));
                    GUILayout.Label("Quest ID: " + Quest.QuestDatabase.GetQuestID(i));
                    GUILayout.Label("Quest Title: " + Quest.QuestDatabase.GetQuestTitle(i));
                    GUILayout.Space(50);
                    GUILayout.Label("Select: ");
                    _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                GUILayout.EndHorizontal();
             
            }
            
            if (GUILayout.Button("DELETE SELECTED"))
            {
                for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
                {
                    Quest.QuestDatabase.DeleteQuests(_activeID[i], _activeQuestActive[i]);
                    _deleteQuests = false;
                    _retrievedAllQuests = false;
                }
            }

            if (GUILayout.Button("BACK"))
            {
                _retrievedAllQuests = false;
                _deleteQuests = false;
            }
        }

        void ExploreQuest()
        {
            _allZones = GameObject.FindGameObjectsWithTag("Zone");
            _zoneNames = new string[_allZones.Length];
            for (int i = 0; i < _allZones.Length; i++)
            {
                _zoneNames[i] = _allZones[i].GetComponent<Zone>().ReturnName();
            }
            GUILayout.Space(20);
            _zoneSelectedIndex = EditorGUILayout.Popup("Which Zone to explore?: ", _zoneSelectedIndex, _zoneNames);

            Quest.QuestDatabase.GetAllNpcs();
            GUILayout.Space(20);
            EditorGUILayout.Separator();
            _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, Quest.QuestDatabase.ReturnActorNames().ToArray());

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
            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("Enable this quest?");
            _questEnabled = EditorGUILayout.Toggle(_questEnabled);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("Auto Complete Quest?:");
            _questAutoComplete = EditorGUILayout.Toggle(_questAutoComplete);
            EditorGUILayout.EndHorizontal();

            if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
            {
                if (GUILayout.Button("SAVE QUEST"))
                {

                    Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", 0, "", false, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled);
                    Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                    Quest.QuestDatabase.UpdateQuestZone(_allZones[_zoneSelectedIndex], Quest.QuestDatabase.ReturnLastQuestID());
                    _addingQuest = false;
                }
            }

        }
    }
#endif
}
