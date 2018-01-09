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
            Quest.QuestDatabase.ClearAll();

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
        }
        /*
        void OnGUI()
        {
            GUI.skin = _skin;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            GUILayout.Label("Welcome to the Quest System", EditorStyles.boldLabel);

            if (!_addingQuest && !_editQuest && !_activeQuests && !_deleteQuests && !_viewQuestChains && !_resetQuest)
            {

                if (GUILayout.Button("Add Quest"))
                {
                    _addingQuest = true;
                }
                if(GUILayout.Button("Edit Quest"))
                {
                    ClearAll();
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

                GUILayout.Space(20);

                if(GUILayout.Button("View Quest Chains"))
                {
                    _viewQuestChains = true;
                }

                if(GUILayout.Button("Reset a Quest"))
                {
                    _resetQuest = true;
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

            if(_viewQuestChains)
            {
                ViewQuestChains();
            }

            if(_resetQuest)
            {
                ResetQuest();
            }

            EditorGUILayout.EndScrollView();
        }
        */
        //////////////////////////////////////////////////////////////////////////////////////
        //                                                                                  //
        // Add Quest Item void                                                              //
        // _obj = the object from the dropdown menu ( Quest Item object )                   //
        // _amount = how many to create                                                     //
        // _edit = are we editing a quest or creating a new quest                           //
        // _questID = if we are editing, we need the quest  ID                              //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        static void AddQuestItems(string _obj, int _amount, bool _edit, int _questID)
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

                if(_questType == QuestType.Kill)
                {
                    KillQuest();
                }

            }
            EditorGUILayout.EndScrollView();
            
        }

        static void CollectQuest()
        {
            // Query the Database to get all QuestItems
            Quest.QuestDatabase.GetQuestItems();
            Quest.QuestDatabase.GetAllQuests();

            // _tmpID = temp int to store the QuestID
            int _tmpID = 0;

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
                            AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, false, 0);
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
                        AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), 1, false, 0);
                    }
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
                

                _chain = (QuestChain)EditorGUILayout.EnumPopup("Single or Chain?: ",_chain);
                if(_chain == QuestChain.Chain)
                {
                    _chainType = (QuestChainType)EditorGUILayout.EnumPopup("Type of Quest in Chain: ", _chainType);
                    if(_chainType == QuestChainType.Followup)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("What is the previous quest?");
                            _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                        EditorGUILayout.EndHorizontal();

                        if (_questChainSelectIndex >= 0)
                        {
                            _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                        }

                    }
                    if(_chainType == QuestChainType.End)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("What is the previous quest?");
                            _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                        EditorGUILayout.EndHorizontal();

                        if (_questChainSelectIndex >= 0)
                        {
                            _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                        }
                    }
                }
                
                if(_chain == QuestChain.Single)
                {
                    _chainType = QuestChainType.Start;
                    _questChainSelectIndex = -1;

                    GUILayout.BeginHorizontal();
                    _questEnabled = true;
                    GUILayout.EndHorizontal();

                }
                if (_chain == QuestChain.Chain && _chainType == QuestChainType.Start)
                {
                    GUILayout.BeginHorizontal();
                    _questEnabled = true;
                    GUILayout.EndHorizontal();
                    _tmpID = 0;
                    
                }
                if(_chain == QuestChain.Chain)
                {
                    if(_chainType == QuestChainType.Followup || _chainType == QuestChainType.End)
                    {
                        _questEnabled = false;
                    }
                }


                if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                {
                    if (GUILayout.Button("SAVE QUEST"))
                    {
                        if (!_questGameStart)
                        {
                            if (GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)) != null)
                            {
                            
                            GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)).GetComponentInChildren<NPC.NpcSystem>().UpdateQuestNPC(true);
                            
                            }
                                                    
                            Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                            Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                        }
                        if(_questGameStart)
                        {
                            Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                        }
                        if(_createdQuestItems.Count > 0)
                        {
                            for (int i = 0; i < _createdQuestItems.Count; i++)
                            {
                                _createdQuestItems[i].GetComponent<Quest.QuestItem>().SetQuestID(Quest.QuestDatabase.ReturnLastQuestID());
                            }
                        }

                        for (int i = 0; i < _questItemCollectAmount; i++)
                        {
                            QuestObject.name = "QuestItem_" + Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex) + "_" + _questTitle + "_" + i;
                        }


                        ClearAll();
                    }
                }
            }
        }

        static void KillQuest()
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

                _killSelectIndex = EditorGUILayout.Popup("Which Enemy: ",_killSelectIndex, EnemyCombat.EnemyDatabase.ReturnAllEnemyNames().ToArray());

            }


            #endregion

            if (_killSelect != KillQuestSelection.None)
            {

                _killAmount = EditorGUILayout.IntField("Amount to Kill: ", _killAmount);

                if (_killAmount > 0)
                {
                    Quest.QuestDatabase.GetAllNpcs();
                    GUILayout.Space(20);
                    EditorGUILayout.Separator();
                    
                    _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, Quest.QuestDatabase.ReturnActorNames().ToArray());
                    
                    //_actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, Quest.QuestDatabase.ReturnActorNames().ToArray());

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


                    _chain = (QuestChain)EditorGUILayout.EnumPopup("Single or Chain?: ", _chain);
                    if (_chain == QuestChain.Chain)
                    {
                        _chainType = (QuestChainType)EditorGUILayout.EnumPopup("Type of Quest in Chain: ", _chainType);
                        if (_chainType == QuestChainType.Followup)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("What is the previous quest?");
                            _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                            EditorGUILayout.EndHorizontal();

                            if (_questChainSelectIndex >= 0)
                            {
                                _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                            }

                        }
                        if (_chainType == QuestChainType.End)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("What is the previous quest?");
                            _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                            EditorGUILayout.EndHorizontal();

                            if (_questChainSelectIndex >= 0)
                            {
                                _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                            }
                        }
                    }

                    if (_chain == QuestChain.Single)
                    {
                        _chainType = QuestChainType.Start;
                        _questChainSelectIndex = -1;

                        GUILayout.BeginHorizontal();
                        _questEnabled = true;
                        GUILayout.EndHorizontal();

                    }
                    if (_chain == QuestChain.Chain && _chainType == QuestChainType.Start)
                    {
                        GUILayout.BeginHorizontal();
                        _questEnabled = true;
                        GUILayout.EndHorizontal();
                        _tmpID = 0;

                    }
                    if (_chain == QuestChain.Chain)
                    {
                        if (_chainType == QuestChainType.Followup || _chainType == QuestChainType.End)
                        {
                            _questEnabled = false;
                        }
                    }


                    if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                    {
                        if (GUILayout.Button("SAVE QUEST"))
                        {
                            if (!_questGameStart)
                            {
                                if (GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)) != null)
                                {

                                    GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)).GetComponentInChildren<NPC.NpcSystem>().UpdateQuestNPC(true);

                                }

                                if (_killSelect == KillQuestSelection.InGame)
                                {
                                    Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", _killAmount, _killAllEnemiesName[_killSelectIndex], false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }

                                if (_killSelect == KillQuestSelection.Database)
                                {
                                    Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }

                                Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                            }

                            if(_questGameStart)
                            {
                                if (_killSelect == KillQuestSelection.InGame)
                                {
                                    Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", _killAmount, _killAllEnemiesName[_killSelectIndex], true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }

                                if (_killSelect == KillQuestSelection.Database)
                                {
                                    Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }

                            }

                            ClearAll();
 
                        }
                    }
                }
            }
        }

        public static void ShowEditQuest()
        {
            int _tmpID = 0;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);



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
                    _questComplete = Quest.QuestDatabase.GetQuestCompletedText(_selectedQuestIndex);
                    _allItemNames = Quest.QuestDatabase.ReturnQuestItemNames().ToArray();
                    _allNpcNames = Quest.QuestDatabase.ReturnActorNames().ToArray();
                    _questZone = Quest.QuestDatabase.GetAllQuestZones(_selectedQuestIndex);
                    _questAutoComplete = Quest.QuestDatabase.GetAllQuestZonesAutoComplete(_selectedQuestIndex);
                    _qItemAmount = Quest.QuestDatabase.GetAllQuestAmountTypes(_selectedQuestIndex);
                    _questItemCollectAmount = Quest.QuestDatabase.GetAllQuestAmounts(_selectedQuestIndex);
                    _questReward = Quest.QuestDatabase.GetQuestReward(_selectedQuestIndex);
                    _goldAmount = Quest.QuestDatabase.GetQuestGold(_selectedQuestIndex);
                    _expAmount = Quest.QuestDatabase.GetQuestExp(_selectedQuestIndex);
                    _questEnabled = Quest.QuestDatabase.GetQuestEnabled(_selectedQuestIndex);
                    _oldNpcID = Quest.QuestDatabase.GetNpcIdFromQuest(_selectedQuestIndex);
                    _chain = Quest.QuestDatabase.GetQuestChain(_selectedQuestIndex);
                    _chainType = Quest.QuestDatabase.GetQuestChainType(_selectedQuestIndex);
                    _questChainSelectIndex = Quest.QuestDatabase.GetQuestChainFollowupID(_selectedQuestIndex);
                    _killAmount = _questItemCollectAmount;
                    _killSelect = KillQuestSelection.InGame;
                    if(_oldNpcID == -1)
                    {
                        _questGameStart = true;
                    }

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
                

                _questGameStart = EditorGUILayout.Toggle("Quest Game Start? ", _questGameStart);
                if (!_questGameStart)
                {
                    _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, _allNpcNames);
                }

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
                        if (GameObject.Find("QuestItem_" + Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex) + "_" + _questTitle + "_" + 0) == null)
                        {
                            GUILayout.Space(20);
                            if (GUILayout.Button("Add Item to the Game"))
                            {
                                _questItemCollectAmount = 1;
                                AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), 1, true, Quest.QuestDatabase.GetQuestID(_selectedQuestIndex));
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
                            if (GameObject.Find("QuestItem_" + Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex) + "_" + _questTitle + "_" + i + "") == null)
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

                                AddQuestItems(Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, true, Quest.QuestDatabase.GetQuestID(_selectedQuestIndex));

                            }
                        }
                    }
                }


                #endregion

                #region EXPLORE
                if(_questType == QuestType.Explore)
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
                if(_questType == QuestType.Kill)
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

                    if(_qItemAmount > 0)
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

                _chain = (QuestChain)EditorGUILayout.EnumPopup("Single or Chain?: ", _chain);
                if(_chain == QuestChain.Single)
                {
                    _chainType = QuestChainType.Start;
                    _questChainSelectIndex = -1;
                    _tmpID = -1;
                }

                if(_chain == QuestChain.Chain)
                {
                    _chainType = (QuestChainType)EditorGUILayout.EnumPopup("Type of Quest in Chain: ", _chainType);
                    if (_chainType == QuestChainType.Followup)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("What is the previous quest?");
                        _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                        EditorGUILayout.EndHorizontal();

                        if (_questChainSelectIndex >= 0)
                        {
                            _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);

                        }

                    }
                    if (_chainType == QuestChainType.End)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("What is the previous quest?");
                        _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                        EditorGUILayout.EndHorizontal();
                        _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                    }
                }

                if (_chain == QuestChain.Single || _chainType == QuestChainType.Start)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(360));
                    GUILayout.Label("Quest Enabled?");
                    _questEnabled = EditorGUILayout.Toggle(_questEnabled);
                    GUILayout.EndHorizontal();
                }
                if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
                {
                    if (GUILayout.Button("SAVE QUEST"))
                    {
                                              

                        if (_questType == QuestType.Explore)
                        {
                            if (!_questGameStart)
                            {
                                Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", 0, "", false, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                            }
                            if(_questGameStart)
                            {
                                Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", 0, "", true, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                            }
                        }
                        if (_questType == QuestType.Collect)
                        {
                            if (!_questGameStart)
                            {
                                Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                            }
                            if(_questGameStart)
                            {
                                Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, Quest.QuestDatabase.ReturnQuestItemPrefab(_questItemIndex), _questItemCollectAmount, "", true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                            }
                        }
                        if (_questType == QuestType.Kill)
                        {
                            if (_killSelect == KillQuestSelection.InGame)
                            {
                                if (!_questGameStart)
                                {
                                    Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", _killAmount, _killAllEnemiesName[_killSelectIndex], false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }
                                if(_questGameStart)
                                {
                                    Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", _killAmount, _killAllEnemiesName[_killSelectIndex], true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }
                            }

                            if (_killSelect == KillQuestSelection.Database)
                            {
                                if (!_questGameStart)
                                {
                                    Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), false, false, "", false, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }
                                if(_questGameStart)
                                {
                                    Quest.QuestDatabase.SaveQuest(Quest.QuestDatabase.GetQuestID(_selectedQuestIndex), _questTitle, _questText, _questType, "", _killAmount, EnemyCombat.EnemyDatabase.ReturnEnemyName(_killSelectIndex), true, false, "", false, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                                }
                            }
                        }

                        if (!_questGameStart)
                        {
                            if (_oldNpcID != Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex))
                            {
                                Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                            }
                            else
                            {
                                Quest.QuestDatabase.UpdateNPC(_oldNpcID);
                            }
                        }

         
                        _retrievedAllQuests = false;
                        ClearAll();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            
        }

        public static void ShowActiveQuests()
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
                    
                    Quest.QuestDatabase.UpdateActiveQuests(_activeID[i], _activeQuestActive[i]);
                }
            }

            if(GUILayout.Button("BACK"))
            {
                _retrievedAllQuests = false;
            }
        }

        public static void ShowDeleteQuests()
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
            if (Quest.QuestDatabase.ReturnQuestTitles().Count > 0)
            {
                for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(550));
                    GUILayout.Label("Quest ID: " + Quest.QuestDatabase.GetQuestID(i), GUILayout.Width(150));
                    GUILayout.Label("Quest Title: " + Quest.QuestDatabase.GetQuestTitle(i), GUILayout.Width(200));
                    GUILayout.Label("Select: ");
                    _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                    GUILayout.EndHorizontal();

                }

                if (GUILayout.Button("DELETE SELECTED"))
                {
                    for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
                    {
                        Quest.QuestDatabase.DeleteQuests(_activeID[i], _activeQuestActive[i]);
                        _retrievedAllQuests = false;
                    }
                }
            }
        }

        static void ExploreQuest()
        {
            int _tmpID = 0;

            Quest.QuestDatabase.GetAllQuests();

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

            _questGameStart = EditorGUILayout.Toggle("Quest Game Start? ", _questGameStart);
            if (!_questGameStart)
            {
                _actorSelectionIndex = EditorGUILayout.Popup("NPC QuestGiver: ", _actorSelectionIndex, Quest.QuestDatabase.ReturnActorNames().ToArray());
            }

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

            _chain = (QuestChain)EditorGUILayout.EnumPopup("Single or Chain?: ", _chain);
            if (_chain == QuestChain.Chain)
            {
                _chainType = (QuestChainType)EditorGUILayout.EnumPopup("Type of Quest in Chain: ", _chainType);
                if (_chainType == QuestChainType.Followup)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("What is the previous quest?");
                    _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                    EditorGUILayout.EndHorizontal();

                    if (_questChainSelectIndex >= 0)
                    {
                        _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                    }
                }
                if (_chainType == QuestChainType.End)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("What is the previous quest?");
                    _questChainSelectIndex = EditorGUILayout.Popup(_questChainSelectIndex, Quest.QuestDatabase.ReturnQuestTitles().ToArray());
                    Debug.Log(_questChainSelectIndex);
                    EditorGUILayout.EndHorizontal();

                    if (_questChainSelectIndex >= 0)
                    {
                        _tmpID = Quest.QuestDatabase.GetQuestID(_questChainSelectIndex);
                    }
                }
            }

            if (_chain == QuestChain.Single)
            {
                _chainType = QuestChainType.Start;
                _questChainSelectIndex = -1;

                GUILayout.BeginHorizontal();
                _questEnabled = true;
                GUILayout.EndHorizontal();
                _tmpID = -1;

            }
            if (_chain != QuestChain.Single)
            {
                _questEnabled = false;
            }
            if (_chain == QuestChain.Chain && _chainType == QuestChainType.Start)
            {

                GUILayout.BeginHorizontal();
                _questEnabled = true;
                GUILayout.EndHorizontal();
            }

            if (_questReward != QuestReward.None && _goldAmount > 0 || _expAmount > 0)
            {
                if (GUILayout.Button("SAVE QUEST"))
                {
                    if (!_questGameStart)
                    {
                        if (GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)) != null)
                        {
                            GameObject.Find("NPC_" + Quest.QuestDatabase.ReturnActorName(_actorSelectionIndex)).GetComponentInChildren<NPC.NpcSystem>().UpdateQuestNPC(true);

                        }

                        Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", 0, "", false, false, _zoneNames[_zoneSelectedIndex], _questAutoComplete, Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex), 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                        Quest.QuestDatabase.UpdateNPC(Quest.QuestDatabase.ReturnActorID(_actorSelectionIndex));
                        Quest.QuestDatabase.UpdateQuestZone(_allZones[_zoneSelectedIndex], Quest.QuestDatabase.ReturnLastQuestID());
                    }
                    if(_questGameStart)
                    {
                        Quest.QuestDatabase.AddQuest(_questTitle, _questText, _questType, "", 0, "", true, false, _zoneNames[_zoneSelectedIndex], true, -1, 0, _questComplete, _goldAmount, _expAmount, "", 0, _questEnabled, _chain.ToString(), _chainType.ToString(), _tmpID);
                        Quest.QuestDatabase.UpdateQuestZone(_allZones[_zoneSelectedIndex], Quest.QuestDatabase.ReturnLastQuestID());
                    }
                    ClearAll();
                   
                    
                }
            }

        }

        public static void ShowQuestChains()
        {
            _questChainFoldout = new bool[100];
            if (!_retrievedAllQuests)
            {
                Quest.QuestDatabase.ClearAll();
                QuestDatabase.GetAllQuests();
               _retrievedAllQuests = true;
            }
            for (int i = 0; i < QuestDatabase.ReturnAllQuestsCount(); i++)
            {
                if(QuestDatabase.GetQuestChain(i) == QuestChain.Chain)
                {
                    if(QuestDatabase.GetQuestChainType(i) == QuestChainType.Start)
                    {
                        if(GUILayout.Button("RESET [ " + QuestDatabase.GetQuestTitle(i) + " ]"))
                        {
                                QuestDatabase.ResetQuestChain(Quest.QuestDatabase.GetQuestID(i));
                        }
                    }
                }
                else
                {
                    GUILayout.Label("None of the Quests are part of a chain");
                }
            }

            if (QuestDatabase.ReturnAllQuestsCount() < 1)
            {
                GUILayout.Label("No quest chains found");
            }
        }

        public static void ShowResetQuest()
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
                GUILayout.BeginHorizontal(GUILayout.Width(550));
                GUILayout.Label("Quest ID: " + Quest.QuestDatabase.GetQuestID(i), GUILayout.Width(150));
                GUILayout.Label("Quest Title: " + Quest.QuestDatabase.GetQuestTitle(i), GUILayout.Width(200));
                GUILayout.Label("Select: ");
                _activeQuestActive[i] = EditorGUILayout.Toggle(_activeQuestActive[i]);
                GUILayout.EndHorizontal();

            }

            if (GUILayout.Button("RESET SELECTED"))
            {
                for (int i = 0; i < Quest.QuestDatabase.ReturnQuestTitles().Count; i++)
                {
                    Quest.QuestDatabase.ResetQuest(_activeID[i], _activeQuestActive[i], Quest.QuestDatabase.GetNpcIdFromQuest(i));
                
                    _retrievedAllQuests = false;
                }
            }

          
        }

        static void ClearAll()
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
