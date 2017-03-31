using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;

#if UNITY_EDITOR

public class PlayerManager : EditorWindow {

    private int _playerHealth;
    private int _playerMana;
    private float _runSpeed;
    private float _walkSpeed;
    private float _rangedDistance;
    private float _meleeRange;

    private static GUISkin _skin;

    private bool _loadSettings = false;

    [MenuItem("Level Design/Player/Player Settings")]

    static void ShowEditor()
    {

        PlayerManager _PM = EditorWindow.GetWindow<PlayerManager>();
    }

    void OnEnable()
    {
        _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
    }

    void OnGUI()
    {
        GUI.skin = _skin;
        if (!_loadSettings)
        {
            GetPlayerSettings();
            _loadSettings = true;
        }

        GUILayout.Label("Welcome to the Player Settings", EditorStyles.boldLabel);

        GUILayout.Space(10);

        GUILayout.Label("Health and Mana", EditorStyles.boldLabel);

        _playerHealth = EditorGUILayout.IntField("Player Health: ", _playerHealth);
        _playerMana = EditorGUILayout.IntField("Player Mana: ", _playerMana);

        GUILayout.Space(10);

        GUILayout.Label("Movement", EditorStyles.boldLabel);

        _runSpeed = EditorGUILayout.FloatField("Run Speed: ", _runSpeed);
        _walkSpeed = EditorGUILayout.FloatField("Walk Speed: ", _walkSpeed);

        GUILayout.Space(10);

        GUILayout.Label("Combat", EditorStyles.boldLabel);

        _rangedDistance = EditorGUILayout.FloatField("Min. distance: Ranged Attack: ", _rangedDistance);
        _meleeRange = EditorGUILayout.FloatField("Min. distance: Melee Attack: ", _meleeRange);

        if (GUILayout.Button("SAVE SETTINGS"))
        {
            SavePlayerSettings(_playerHealth, _playerMana, _runSpeed, _walkSpeed, _rangedDistance, _meleeRange);
        }
    }

    void GetPlayerSettings()
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM PlayerSettings";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            _playerHealth = reader.GetInt32(1);
            _playerMana = reader.GetInt32(2);
            _runSpeed = reader.GetFloat(3);
            _walkSpeed = reader.GetFloat(4);
            _rangedDistance = reader.GetFloat(5);
            _meleeRange = reader.GetFloat(6);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void SavePlayerSettings(int _health, int _mana, float _run, float _walk, float _ranged, float _melee)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "UPDATE PlayerSettings SET PlayerHealth = '" + _health + "', PlayerMana = '" + _mana + "', RunSpeed = '" + _run + "', WalkSpeed = '" + _walk + "', RangedDistance = '" + _ranged + "', MeleeRange = '" + _melee + "' WHERE PlayerID = '1'";
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

}
#endif