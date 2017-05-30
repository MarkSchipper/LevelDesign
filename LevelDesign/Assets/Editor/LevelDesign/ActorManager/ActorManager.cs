using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace NPCSystem
{

    public class ActorManager : EditorWindow
    {
        private string _actorName;
        private string _actorProfession = "";
        private bool _actorInteraction = false;
        private string _actorDialog1 = "";
        private string _actorDialog2 = "";
        private bool _actorQuestGiver = false;
        private string _actorQuestDialog1 = "";
        private string _actorQuestDialog2 = "";
        private string _actorQuestComplete = "";

        private bool _addingActor = false;
        private bool _editActor = false;
        private bool _viewActors = false;
        private bool _loadedActors = false;
        private bool _addActorGame = false;
        private bool _editActorGame = false;
        private bool _deleteActor = false;
        private bool _isDeletingActor = false;
        private bool _deleteConfirmation = false;
        private bool _setInteraction = false;
        private bool _isActorInteraction = false;

        private int _deleteActorID;

        private UnityEngine.Object[] _actors;

        private List<int> _allActorID = new List<int>();
        private List<string> _allActorNames = new List<string>();
        private List<string> _allActorPrefabs = new List<string>();
        private List<string> _allActorProfessions = new List<string>();
        private List<bool> _allActorInteractions = new List<bool>();
        private List<string> _allActorDialogue1 = new List<string>();
        private List<string> _allActorDialogue2 = new List<string>();
        private List<string> _allActorQuestGivers = new List<string>();
        private List<int> _allActorQuestGiverID = new List<int>();
        private List<string> _allActorQuestDialogue1 = new List<string>();
        private List<string> _allActorQuestDialogue2 = new List<string>();
        private List<string> _allActorQuestComplete = new List<string>();


        private bool[] _viewActorsFoldout;

        private int _selectedActorIndex;
        private int _selectedPrefabIndex;

        // IN GAME STUFF

        private ActorBehaviour _selectedBehaviour;
        private int _wayPointAmount;
        private int _wayPointIdleTime;
        private float _wayPointSpeed;
        private GameObject[] _allInGameActors;
        private List<GameObject> _allActorWayPoints = new List<GameObject>();
        private List<string> _inGameActorNames = new List<string>();
        private bool _gotInGameActors = false;
        private bool _loadedWaypoints = false;

        private GUISkin _skin;

        private Vector2 scrollPos;

        [MenuItem("Level Design/Managers/NPC Manager")]

        static void ShowEditor()
        {
            ActorManager _actorManager = EditorWindow.GetWindow<ActorManager>();
        }

        void OnEnable()
        {
            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
        }

        void OnGUI()
        {

            GUI.skin = _skin;

            GUILayout.Label("Welcome to the Actor Manager", EditorStyles.boldLabel);


            //////////////////////////////////////////////////////////////////////////////////////////
            //                                  DATABASE OPERATIONS                                 //
            //////////////////////////////////////////////////////////////////////////////////////////

            // If we are on the 'main' screen -> if ALL booleans are false
            if (!_addingActor && !_editActor && !_viewActors && !_addActorGame && !_editActorGame && !_deleteActor)
            {

                //////////////////////////////////////////////////////////////////////////////////////
                //                              BUTTONS ON THE MENU                                 //
                //////////////////////////////////////////////////////////////////////////////////////

                // Load All OBJECTS from the Folder Characters/NPC
                // NOTE: LoadAll loads EVERYTHING from the folder, including textures, materials, animations etc
                // Sort the Array by name

                _actors = Resources.LoadAll("Characters/NPC/");
                _actors.OrderBy(go => go.name).ToArray();

                GUILayout.BeginArea(new Rect(0, 50, 400, 400));
                GUILayout.Label("Database operations:", EditorStyles.boldLabel);
                if (GUILayout.Button("Add an Actor"))
                {
                    _addingActor = true;
                    _allActorPrefabs.Clear();
                }
                if (GUILayout.Button("Edit an Actor"))
                {
                    _editActor = true;
                    if (!_loadedActors)
                    {
                        ClearAll();
                        GetAllActors();
                    }
                }

                if (GUILayout.Button("View all Actors"))
                {
                    _viewActors = true;
                    if (!_loadedActors)
                    {
                        ClearAll();
                        GetAllActors();
                    }
                }

                if (GUILayout.Button("Delete an Actor"))
                {
                    _deleteActor = true;
                    ClearAll();
                    GetAllActors();
                }

                GUILayout.EndArea();



                // IN GAME EDITING
                GUILayout.BeginArea(new Rect(0, 250, 400, 400));
                GUILayout.Label("In Game Operations", EditorStyles.boldLabel);

                if (GUILayout.Button("Add Actor to the Game"))
                {
                    _addActorGame = true;
                    ClearAll();
                    GetAllActors();
                }

                if (GUILayout.Button("Edit an Actor in the Game"))
                {
                    _editActorGame = true;
                }

                GUILayout.EndArea();
            }

            //////////////////////////////////////////////////////////////////////////////////////
            //                                    END BUTTONS                                   //
            //////////////////////////////////////////////////////////////////////////////////////

            #region Add Actor

            // If we pressed on the Add Actor button

            if (_addingActor)
            {

                // For every object in the _actors array
                // If the TYPE of the object is "UnityEngine.GameObject"
                // Strip the last 25 characters -> (UnityEngine.GameObject)
                // So we are left with just the name

                for (int i = 0; i < _actors.Length; i++)
                {
                    if (_actors[i].GetType().ToString() == "UnityEngine.GameObject")
                    {
                        _allActorPrefabs.Add(_actors[i].ToString().Remove(_actors[i].ToString().Length - 25));
                    }
                 
                }

                scrollPos = GUILayout.BeginScrollView(scrollPos);

                GUILayout.Label("Add an Actor");

                _actorName = EditorGUILayout.TextField("Name: ", _actorName);

                GUILayout.Label("Which Model");
                _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorPrefabs.ToArray());

                _actorProfession = EditorGUILayout.TextField("Profession: ", _actorProfession);

                _actorInteraction = EditorGUILayout.Toggle("Interactive?: ", _actorInteraction);
                if (_actorInteraction)
                {

                    GUILayout.Label("Initial Dialogue");
                    _actorDialog1 = EditorGUILayout.TextArea(_actorDialog1, GUILayout.Height(100));

                    GUILayout.Label("Return Dialogue");
                    _actorDialog2 = EditorGUILayout.TextArea(_actorDialog2, GUILayout.Height(100));

                    _actorQuestGiver = EditorGUILayout.Toggle("Quest giver?: ", _actorQuestGiver);

                    if (_actorQuestGiver)
                    {
                        // ADD ALL QUEST IN A POPUP

                        GUILayout.Label("Quest Dialogue");
                        _actorQuestDialog1 = EditorGUILayout.TextArea(_actorQuestDialog1, GUILayout.Height(100));

                        GUILayout.Label("Return Quest Dialogue");
                        _actorQuestDialog2 = EditorGUILayout.TextArea(_actorQuestDialog2, GUILayout.Height(100));

                        GUILayout.Label("Quest Complete Dialogue");
                        _actorQuestComplete = EditorGUILayout.TextArea(_actorQuestComplete, GUILayout.Height(100));

                    }
                }
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("SAVE ACTOR"))
                {
                    AddActor(_actorName, _allActorPrefabs[_selectedActorIndex], _actorProfession, _actorInteraction, _actorDialog1, _actorDialog2, _actorQuestGiver, 0);
                    
                    ClearValues();
                    _addingActor = false;
                }
                if (GUILayout.Button("BACK"))
                {
                    _addingActor = false;
                    ClearValues();
                }
                GUILayout.EndHorizontal();
                // GUILayout.EndArea();
                GUILayout.EndScrollView();
            }
            #endregion
            #region Edit Actor
            if (_editActor)
            {

               

                Debug.Log(_actorInteraction);

                scrollPos = GUILayout.BeginScrollView(scrollPos);

                GUILayout.Label("Edit an Actor");
                GUILayout.Space(10);
                _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorNames.ToArray());

                if (!_setInteraction)
                {

                    _isActorInteraction = _allActorInteractions[_selectedActorIndex];
                    _setInteraction = true;
                }

                if(GUI.changed)
                {
                    _setInteraction = false;
                }

                GUILayout.Space(10);

                if (_selectedActorIndex >= 0)
                {
                    _actorName = EditorGUILayout.TextField("Name: ", _allActorNames[_selectedActorIndex]);

                    GUILayout.Label("Which Model");
                    _selectedPrefabIndex = EditorGUILayout.Popup(_selectedPrefabIndex, _allActorPrefabs.ToArray());

                    _actorProfession = EditorGUILayout.TextField("Profession: ", _allActorProfessions[_selectedActorIndex]);

                    _actorInteraction = EditorGUILayout.Toggle("Interactive?: ", _actorInteraction);

                    if (_actorInteraction)
                    {
                        GUILayout.Label("Initial Dialogue");
                        _actorDialog1 = EditorGUILayout.TextArea(_allActorDialogue1[_selectedActorIndex], GUILayout.Height(100));

                        GUILayout.Label("Return Dialogue");
                        _actorDialog2 = EditorGUILayout.TextArea(_allActorDialogue2[_selectedActorIndex], GUILayout.Height(100));

                        _actorQuestGiver = EditorGUILayout.Toggle("Quest giver?: ", _actorQuestGiver);


                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("SAVE ACTOR"))
                    {
                        SaveActor(_allActorID[_selectedActorIndex], _actorName, _allActorPrefabs[_selectedPrefabIndex], _actorProfession, _actorInteraction, _actorDialog1, _actorDialog2, _actorQuestGiver, 0);
                        ClearValues();
                    }

                    if (GUILayout.Button(" BACK "))
                    {
                        _editActor = false;
                        ClearValues();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            #endregion
            #region View Actors
            if (_viewActors)
            {
                for (int i = 0; i < _allActorID.Count; i++)
                {
                    GUILayout.Label("VIEW ALL ACTORS");
                    GUILayout.Space(20);

                    _viewActorsFoldout[i] = EditorGUILayout.Foldout(_viewActorsFoldout[i], _allActorNames[i]);

                    if (_viewActorsFoldout[i])
                    {

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(40);
                        GUILayout.Label("Profession: " + _allActorProfessions[i]);
                        GUILayout.EndHorizontal();
                        if (GUILayout.Button("EDIT"))
                        {
                            _selectedActorIndex = i;
                            _viewActors = false;
                            _editActor = true;
                        }
                    }

                    if (GUILayout.Button("BACK"))
                    {
                        _viewActors = false;
                    }

                }
            }
            #endregion
            #region Delete Actor
            if (_deleteActor && !_isDeletingActor)
            {
                GUILayout.Label("Delete an Actor");

                GUILayout.Space(30);

                for (int i = 0; i < _allActorID.Count; i++)
                {

                    if (GUILayout.Button("DELETE " + _allActorNames[i] + " From Database"))
                    {
                        _isDeletingActor = true;
                        _deleteActorID = i;
                    }

                    if (GUILayout.Button("BACK"))
                    {
                        _deleteActor = false;
                    }

                }
            }
            #endregion
            #region Actual Deleting of Actor
            if (_isDeletingActor)
            {
                if (!_deleteConfirmation)
                {
                    GUILayout.Label("Are you sure you want to delete " + _allActorNames[_deleteActorID] + "?");
                    if (GUILayout.Button("DELETE"))
                    {
                        DeleteActor(_deleteActorID);
                    }
                    if (GUILayout.Button("BACK"))
                    {
                        _isDeletingActor = false;
                    }
                }
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////////////////
            //                                      GAME OPERATIONS                                 //
            //////////////////////////////////////////////////////////////////////////////////////////
           
            #region Add Actor to the Game
            if (_addActorGame)
            {
                GUILayout.Label("Add an Actor to the Game");
                _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorNames.ToArray());

                GUILayout.Label("Actor Behaviour");
                _selectedBehaviour = (ActorBehaviour)EditorGUILayout.EnumPopup("Behaviour", _selectedBehaviour);

                if (_selectedBehaviour == ActorBehaviour.Patrol)
                {
                    _wayPointAmount = EditorGUILayout.IntField("Amount of waypoints: ", _wayPointAmount);
                    if (_wayPointAmount > 0)
                    {
                        _wayPointSpeed = EditorGUILayout.FloatField("Movement Speed: ", _wayPointSpeed);
                        _wayPointIdleTime = EditorGUILayout.IntField("Wait time when reaching waypoint: ", _wayPointIdleTime);
                    }
                }
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Add '" + _allActorNames[_selectedActorIndex] + "' to the Game"))
                {
                    AddActorToGame(_selectedActorIndex);
                }
                if (GUILayout.Button("BACK"))
                {
                    _addActorGame = false;
                }
                GUILayout.EndHorizontal();
            }
            #endregion
            #region edit actor in game
            if (_editActorGame)
            {
                if (!_gotInGameActors)
                {
                    _allInGameActors = GameObject.FindGameObjectsWithTag("NPC");
                    for (int i = 0; i < _allInGameActors.Length; i++)
                    {
                        _inGameActorNames.Add(_allInGameActors[i].GetComponentInChildren<NPCSystem.NPC>().ReturnNpcName());
                        Debug.Log(_inGameActorNames.Count);
                    }
                    _selectedBehaviour = _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().ReturnBehaviour();
                    _gotInGameActors = true;
                }
                GUILayout.Label("Edit the behaviour of an In Game Actor");
                _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _inGameActorNames.ToArray());

                
                _selectedBehaviour = (ActorBehaviour)EditorGUILayout.EnumPopup("Behaviour", _selectedBehaviour);
                if (_selectedBehaviour == ActorBehaviour.Patrol)
                {
                    Debug.Log("PATROL");
                    if (!_loadedWaypoints)
                    {
                        for (int i = 0; i < _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().ReturnWaypointAmount(); i++)
                        {
                            _allActorWayPoints.Add(GameObject.Find("NPC_" + _inGameActorNames[_selectedActorIndex] + "_Waypoint_" + i + ""));
                            _wayPointAmount = _allActorWayPoints.Count;

                        }
                        _wayPointSpeed = _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().ReturnPatrolSpeed();
                        _loadedWaypoints = true;

                    }

                    _wayPointSpeed = EditorGUILayout.FloatField("Movement Speed: ", _wayPointSpeed);

                    _wayPointAmount = EditorGUILayout.IntField("Amount of waypoints: ", _wayPointAmount);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("UPDATE ACTOR"))
                    {
                        _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().SetNpcBehaviour(_selectedBehaviour);
                        _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().SetPatrolSpeed(_wayPointSpeed);

                        if (_wayPointAmount > _allActorWayPoints.Count)
                        {
                            //_allInGameActors[_selectedActorIndex].GetComponentInChildren<>

                            for (int i = _allActorWayPoints.Count; i < _wayPointAmount; i++)
                            {
                                Debug.Log("i = " + i + " Amount of waypoints = " + _wayPointAmount);
                                GameObject _wayPoint = new GameObject();
                                _wayPoint.name = "NPC_" + _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().ReturnNpcName() + "_WayPoint_" + i + "";
                                _wayPoint.transform.parent = GameObject.Find("NPC_" + _inGameActorNames[_selectedActorIndex] + "").transform;
                                _wayPoint.transform.position = new Vector3(5 * i, 0, 0);
                                _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPCSystem.NPC>().SetWayPoints(_wayPoint.transform);
                            }
                        }
                    }
                    if (GUILayout.Button("BACK"))
                    {
                        _editActorGame = false;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            #endregion
        }

        void GetAllActors()
        {

            _loadedActors = true;

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM Actors";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _allActorID.Add(reader.GetInt32(0));
                _allActorNames.Add(reader.GetString(1));
                _allActorPrefabs.Add(reader.GetString(2));
                _allActorProfessions.Add(reader.GetString(3));
                if (reader.GetString(4) == "True")
                {
                    _allActorInteractions.Add(true);
                }
                else
                {
                    _allActorInteractions.Add(false);
                }
                _allActorDialogue1.Add(reader.GetString(5));
                _allActorDialogue2.Add(reader.GetString(6));
                _allActorQuestGivers.Add(reader.GetString(7));

                _allActorQuestGiverID.Add(reader.GetInt32(8));

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            _viewActorsFoldout = new bool[_allActorProfessions.Count];
        }

        void AddActor(string _name, string _prefab, string _profession, bool _interaction, string _dialogue1, string _dialogue2, bool _questGiver, int _questID)
        {

            //Debug.Log("Query: " + _name + " - " + _prefab + " - " + _profession + " - " + _interaction + " - " + _dialogue1 + " - " + _dialogue2 + " - " + _questGiver + " - " + _questID + " - " + _questDialog1 + " - " + _questDialog2 + " - " + _questComplete);

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Actors (ActorName, ActorPrefab, ActorProfession, ActorInteraction, ActorDialog1, ActorDialog2, ActorQuestgiver, ActorQuestID) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\")", _name, _prefab, _profession, _interaction.ToString(), _dialogue1, _dialogue2, _questGiver.ToString(), _questID);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        void SaveActor(int _id, string _name, string _prefab, string _profession, bool _interaction, string _dialogue1, string _dialogue2, bool _questGiver, int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Actors " + "SET ActorName = " + "'" + _name + "'" + ", ActorPrefab = " + "'" + _prefab + "'" + ", ActorProfession = " + "'" + _profession + "'" + ", ActorInteraction = " + "'" + _interaction.ToString() + "'" + ", ActorDialog1 = " + "'" + _dialogue1 + "'" + ", ActorDialog2 = " + "'" + _actorDialog2 + "'" + ", ActorQuestGiver = " + "'" + _questGiver + "'" + ", ActorQuestID = " + "'" + 0 + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        void ClearValues()
        {
            _actorName = "";
            _actorProfession = "";
            _actorInteraction = false;
            _actorDialog1 = "";
            _actorDialog2 = "";
            _actorQuestGiver = false;
            _actorQuestDialog1 = "";
            _actorQuestDialog2 = "";
            _actorQuestComplete = "";

        }

        void AddActorToGame(int _id)
        {

            // Create a new Parent object for the NPC
            GameObject _npcParent = new GameObject();
            _npcParent.name = "NPC_" + _allActorNames[_id] + "";

            GameObject _NPC = Instantiate(Resources.Load("Characters/NPC/" + _allActorPrefabs[_id], typeof(GameObject))) as GameObject;

            _NPC.transform.parent = _npcParent.transform;
            _NPC.tag = "NPC";
            _NPC.AddComponent<CharacterController>();
            _NPC.GetComponent<CharacterController>().radius = 1.0f;
            _NPC.GetComponent<CharacterController>().height = 4.0f;
            _NPC.GetComponent<CharacterController>().center = new Vector3(0, 2f, 0);

            _NPC.AddComponent<NPCSystem.NPC>();

            _NPC.GetComponent<NPCSystem.NPC>().SetNpcID(_allActorID[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetNPCName(_allActorNames[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetProfession(_allActorProfessions[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetInteraction(_allActorInteractions[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetDialogues(_allActorDialogue1[_id], _allActorDialogue2[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetQuestGiver(_allActorQuestGivers[_id]);
            _NPC.GetComponent<NPCSystem.NPC>().SetNpcBehaviour(_selectedBehaviour);

            if (_selectedBehaviour == ActorBehaviour.Patrol && _wayPointAmount > 0)
            {
                for (int i = 0; i < _wayPointAmount; i++)
                {
                    GameObject _wayPoint = new GameObject();
                    _wayPoint.name = "NPC_" + _allActorNames[_selectedActorIndex] + "_WayPoint_" + i + "";
                    _wayPoint.transform.parent = _npcParent.transform;
                    _wayPoint.transform.position = new Vector3(5 * i, 0, 0);

                    _NPC.GetComponent<NPCSystem.NPC>().SetWayPoints(_wayPoint.transform);

                }
            }

            _NPC.GetComponent<NPCSystem.NPC>().SetPatrolSpeed(_wayPointSpeed);

            GameObject _npcTrigger = new GameObject();
            _npcTrigger.name = "NPC_" + _allActorNames[_selectedActorIndex] + "_TRIGGER";

            _npcTrigger.transform.SetParent(_NPC.transform);

            _npcTrigger.AddComponent<SphereCollider>();
            _npcTrigger.GetComponent<SphereCollider>().isTrigger = true;
            _npcTrigger.GetComponent<SphereCollider>().radius = 3.5f;
            _npcTrigger.layer = 2;
            _npcTrigger.AddComponent<Quest.NPC_Trigger>();

        }


        void DeleteActor(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("DELETE FROM Actors WHERE ActorID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        void ClearAll()
        {
            _allActorID.Clear();
            _allActorNames.Clear();
            _allActorPrefabs.Clear();
            _allActorProfessions.Clear();
            _allActorInteractions.Clear();
            _allActorDialogue1.Clear();
            _allActorDialogue2.Clear();
            _allActorQuestGivers.Clear();
            _allActorQuestGiverID.Clear();
            _allActorQuestDialogue1.Clear();
            _allActorQuestDialogue2.Clear();
            _allActorQuestComplete.Clear();
        }

    }
    
}