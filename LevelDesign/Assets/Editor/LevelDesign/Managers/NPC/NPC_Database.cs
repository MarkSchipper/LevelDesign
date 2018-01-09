using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;
using System;

public class NPC_Database  {


    private static string _actorName;
    private static string _actorProfession = "";

    private static int _selectedActorIndex;
    private static int _selectedPrefabIndex;
    private static NPC.ActorBehaviour _selectedBehaviour;


    private static UnityEngine.Object[] _actors;

    private static List<int> _allActorID = new List<int>();
    private static List<string> _allActorNames = new List<string>();
    private static List<string> _allActorPrefabs = new List<string>();
    private static List<string> _allActorProfessions = new List<string>();
    

    private static Vector2 _scrollPos;
    private static bool _loadedNPC;
    private static bool _confirmDeletetion;
    private static bool _npcDeleted;
    private static bool _npcSaved;
    private static int _deleteActorID;



    public static void LoadActors()
    {
        _actors = Resources.LoadAll("Characters/NPC/");
        _actors.OrderBy(go => go.name).ToArray();
    }

    public static void AddNPC()
    {
        if (!_npcSaved)
        {
            for (int i = 0; i < _actors.Length; i++)
            {
                if (_actors[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    _allActorPrefabs.Add(_actors[i].ToString().Remove(_actors[i].ToString().Length - 25));
                }

            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Add an Actor", EditorStyles.boldLabel);

            _actorName = EditorGUILayout.TextField("Name: ", _actorName);

            GUILayout.Label("Which Model", EditorStyles.boldLabel);
            _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorPrefabs.ToArray());

            _actorProfession = EditorGUILayout.TextField("Profession: ", _actorProfession);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save NPC"))
            {
                AddActor(_actorName, _allActorPrefabs[_selectedActorIndex], _actorProfession);
                ClearValues();
                _npcSaved = true;
                _loadedNPC = false;
            }

            GUILayout.EndHorizontal();
            // GUILayout.EndArea();
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("NPC has been saved to the database");
        }
        _loadedNPC = false;
        _confirmDeletetion = false;
    }

    public static void EditNPC()
    {
        if(!_loadedNPC)
        {
            ClearLists();
            GetAllActors();
            ClearValues();
            _loadedNPC = true;
        }
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("Edit an Actor");
        GUILayout.Space(10);
        _selectedActorIndex = EditorGUILayout.Popup(_selectedActorIndex, _allActorNames.ToArray());

        GUILayout.Space(10);

        if (_selectedActorIndex >= 0)
        {
            _actorName = EditorGUILayout.TextField("Name: ", _allActorNames[_selectedActorIndex]);

            GUILayout.Label("Which Model");
            _selectedPrefabIndex = EditorGUILayout.Popup(_selectedPrefabIndex, _allActorPrefabs.ToArray());

            _actorProfession = EditorGUILayout.TextField("Profession: ", _allActorProfessions[_selectedActorIndex]);
      
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SAVE ACTOR"))
            {
                SaveActor(_allActorID[_selectedActorIndex], _actorName, _allActorPrefabs[_selectedPrefabIndex], _actorProfession);
                ClearValues();
                _loadedNPC = false;
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        _confirmDeletetion = false;
        _npcSaved = false;
    }

    public static void DeleteNPC()
    {
        if (!_loadedNPC)
        {
            ClearLists();
            GetAllActors();
            ClearValues();
            _loadedNPC = true;
        }
        GUILayout.Space(30);

        if (!_confirmDeletetion)
        {
            for (int i = 0; i < _allActorID.Count; i++)
            {
                if (GUILayout.Button("DELETE " + _allActorNames[i] + " From Database"))
                {
                    _confirmDeletetion = true;
                    _deleteActorID = i;
                }

            }
        }
        if(_confirmDeletetion && !_npcDeleted)
        {
            if(GUILayout.Button("Are you sure you want to delete " + _allActorNames[_deleteActorID]))
            {
                DeleteActor(_deleteActorID);
                _npcDeleted = true;
                ClearValues();
                _confirmDeletetion = false;
                _loadedNPC = false;
            }
        }
        if(_npcDeleted)
        {
            GUILayout.Label("NPC has been deleted");
            _loadedNPC = false;
        }
        _npcSaved = false;
    }

    static void GetAllActors()
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

    static void AddActor(string _name, string _prefab, string _profession)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("INSERT INTO Actors (ActorName, ActorPrefab, ActorProfession) VALUES (\"{0}\", \"{1}\", \"{2}\")", _name, _prefab, _profession);
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    static void SaveActor(int _id, string _name, string _prefab, string _profession)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("UPDATE Actors " + "SET ActorName = " + "'" + _name + "'" + ", ActorPrefab = " + "'" + _prefab + "'" + ", ActorProfession = " + "'" + _profession + "' WHERE ActorID = '" + _id + "'");
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    static void DeleteActor(int _id)
    {

        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("DELETE FROM Actors WHERE ActorID = '" + _allActorID[_id] + "'");
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
       
        ClearValues();
        ClearLists();
    }

    public static void ClearValues()
    {
        
        _actorName = "";
        _actorProfession = "";
    }

    static void ClearLists()
    {
        _allActorID.Clear();
        _allActorNames.Clear();
        _allActorPrefabs.Clear();
        _allActorProfessions.Clear();
    }
}
