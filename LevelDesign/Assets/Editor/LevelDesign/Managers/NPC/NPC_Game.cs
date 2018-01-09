using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;
using System;

public class NPC_Game : MonoBehaviour {

    private static int _selectedActorIndex;

    private static List<int> _allActorID = new List<int>();
    private static List<string> _allActorNames = new List<string>();
    private static List<string> _allActorPrefabs = new List<string>();
    private static List<string> _allActorProfessions = new List<string>();

    private static NPC.ActorBehaviour _selectedBehaviour;
    private static int _wayPointAmount;
    private static float _wayPointSpeed;
    private static int _wayPointIdleTime;

    private static bool _gotInGameActors;
    private static bool _loadedWaypoints;
    private static GameObject[] _allInGameActors;
    private static List<GameObject> _allActorWayPoints = new List<GameObject>();
    private static List<string> _inGameActorNames = new List<string>();

    public static void ShowAddGame()
    {
        GUILayout.Label("Add an Actor to the Game");
        _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorNames.ToArray());

        GUILayout.Label("Actor Behaviour");
        _selectedBehaviour = (NPC.ActorBehaviour)EditorGUILayout.EnumPopup("Behaviour", _selectedBehaviour);

        if (_selectedBehaviour == NPC.ActorBehaviour.Patrol)
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
      
        GUILayout.EndHorizontal();
    }

    public static void ShowEditGame()
    {
        if (!_gotInGameActors)
        {
            _allInGameActors = GameObject.FindGameObjectsWithTag("NPC");
            for (int i = 0; i < _allInGameActors.Length; i++)
            {
                _inGameActorNames.Add(_allInGameActors[i].GetComponentInChildren<NPC.NpcSystem>().ReturnNpcName());
            }
            _selectedBehaviour = _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnNpcBehaviour();
            _gotInGameActors = true;
        }
        GUILayout.Label("Edit the behaviour of an In Game Actor");
        _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _inGameActorNames.ToArray());

        _selectedBehaviour = (NPC.ActorBehaviour)EditorGUILayout.EnumPopup("Behaviour", _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnNpcBehaviour());
        if (_allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnNpcBehaviour() == NPC.ActorBehaviour.Patrol)
        {
            if (!_loadedWaypoints)
            {
                for (int i = 0; i < _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnWaypointAmount(); i++)
                {
                    _allActorWayPoints.Add(GameObject.Find("NPC_" + _inGameActorNames[_selectedActorIndex] + "_Waypoint_" + i + ""));
                    _wayPointAmount = _allActorWayPoints.Count;

                }
                _wayPointSpeed = _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnPatrolSpeed();
                _loadedWaypoints = true;

            }

            _wayPointSpeed = EditorGUILayout.FloatField("Movement Speed: ", _wayPointSpeed);

            _wayPointAmount = EditorGUILayout.IntField("Amount of waypoints: ", _wayPointAmount);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("UPDATE ACTOR"))
            {
                _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().SetNpcBehaviour(_selectedBehaviour);
                _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().SetPatrolSpeed(_wayPointSpeed);

                if (_wayPointAmount > _allActorWayPoints.Count)
                {
                    for (int i = _allActorWayPoints.Count; i < _wayPointAmount; i++)
                    {
                        Debug.Log("i = " + i + " Amount of waypoints = " + _wayPointAmount);
                        GameObject _wayPoint = new GameObject();
                        _wayPoint.name = "NPC_" + _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().ReturnNpcName() + "_WayPoint_" + i + "";
                        _wayPoint.transform.parent = GameObject.Find("NPC_" + _inGameActorNames[_selectedActorIndex] + "").transform;
                        _wayPoint.transform.position = new Vector3(5 * i, 0, 0);
                        _allInGameActors[_selectedActorIndex].GetComponentInChildren<NPC.NpcSystem>().SetWaypoints(_wayPoint.transform);
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }

    public static void GetAllActors()
    {

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
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        // _viewActorsFoldout = new bool[_allActorProfessions.Count];
    }

    static void AddActorToGame(int _id)
    {

        // Create a new Parent object for the NPC
        GameObject _npcParent = new GameObject();
        _npcParent.name = "NPC_" + _allActorNames[_id] + "";
        Debug.Log(_allActorPrefabs[_id]);
        GameObject _NPC = Instantiate(Resources.Load("Characters/NPC/" + _allActorPrefabs[_id], typeof(GameObject))) as GameObject;

        _NPC.transform.parent = _npcParent.transform;
        _NPC.tag = "NPC";
        _NPC.AddComponent<CharacterController>();
        _NPC.GetComponent<CharacterController>().radius = 1.0f;
        _NPC.GetComponent<CharacterController>().height = 4.0f;
        _NPC.GetComponent<CharacterController>().center = new Vector3(0, 2f, 0);

        _NPC.AddComponent<NPC.NpcSystem>();
        _NPC.GetComponent<NPC.NpcSystem>().SetData(_allActorID[_id], _allActorNames[_id], _selectedBehaviour, _wayPointSpeed, _allActorPrefabs[_id], _wayPointIdleTime);

        if (_selectedBehaviour == NPC.ActorBehaviour.Patrol && _wayPointAmount > 0)
        {
            for (int i = 0; i < _wayPointAmount; i++)
            {
                GameObject _wayPoint = new GameObject();
                _wayPoint.name = "NPC_" + _allActorNames[_selectedActorIndex] + "_WayPoint_" + i + "";
                _wayPoint.transform.parent = _npcParent.transform;
                _wayPoint.transform.position = new Vector3(5 * i, 0, 0);

                _NPC.GetComponent<NPC.NpcSystem>().SetWaypoints(_wayPoint.transform);

            }
        }

        GameObject _npcTrigger = new GameObject();
        _npcTrigger.name = "NPC_" + _allActorNames[_selectedActorIndex] + "_TRIGGER";

        _npcTrigger.transform.SetParent(_NPC.transform);

        _npcTrigger.AddComponent<SphereCollider>();
        _npcTrigger.GetComponent<SphereCollider>().isTrigger = true;
        _npcTrigger.GetComponent<SphereCollider>().radius = 3.5f;
        _npcTrigger.layer = 2;

    }

}
