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

    public class PlayerStatsManager : EditorWindow
    {

        private int _playerLevel;
        private int _playerExp;
        private int _playerGold;
        private float _expMultiplier;
        private float _dmgMultiplier;
        private float _healthMultiplier;
        private float _manaMultiplier;
        private float _healingMultiplier;

        private Vector2 _scrollPos;
        private GUISkin _skin;

        [MenuItem("Level Design/Player/Player Statistics")]

        static void ShowEditor()
        {
            PlayerStatsManager _pStats = EditorWindow.GetWindow<PlayerStatsManager>();

        }

        void OnEnable()
        {
            GetPlayerData();
            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            GUI.skin = _skin;

            GUILayout.Label("Welcome to the Player Statistics Window!", EditorStyles.boldLabel);
            GUILayout.Space(50);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            _playerLevel = EditorGUILayout.IntField("Current Player Level: ", _playerLevel);
            _playerExp = EditorGUILayout.IntField("Current Player Exp: ", _playerExp);
            _playerGold = EditorGUILayout.IntField("Current Amount of Gold: ", _playerGold);
            GUILayout.Label("How much Exp is added per level in %");
            _expMultiplier = EditorGUILayout.FloatField("Exp multiplier: ", _expMultiplier);
            GUILayout.Label("Damage increase per level in %");
            _dmgMultiplier = EditorGUILayout.FloatField("Damage multiplier: ", _dmgMultiplier);
            GUILayout.Label("Health increase per level in %");
            _healthMultiplier = EditorGUILayout.FloatField("Health multiplier: ", _healthMultiplier);
            GUILayout.Label("Mana increase per level in %");
            _manaMultiplier = EditorGUILayout.FloatField("Mana multiplier: ", _manaMultiplier);
            GUILayout.Label("Healing Power increase per level in %");
            _healingMultiplier = EditorGUILayout.FloatField("Healing Power multiplier: ", _healingMultiplier);

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Save Changes"))
            {
                UpdatePlayerData(_playerLevel, _playerExp, _playerGold, _expMultiplier, _dmgMultiplier, _healthMultiplier, _manaMultiplier, _healingMultiplier);
            }


        }

        void GetPlayerData()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM PlayerStats WHERE PlayerID = '1'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                _playerLevel = reader.GetInt32(1);
                _playerExp = reader.GetInt32(2);
                _playerGold = reader.GetInt32(3);
                _expMultiplier = reader.GetFloat(4);
                _dmgMultiplier = reader.GetFloat(5);
                _healthMultiplier = reader.GetFloat(6);
                _manaMultiplier = reader.GetFloat(7);
                _healingMultiplier = reader.GetFloat(8);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        void UpdatePlayerData(int _level, int _exp, int _gold, float _expM, float _dmgM, float _healthM, float _manaM, float _healingM)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE PlayerStats SET PlayerLevel = '" + _level + "', PlayerExp = '" + _exp + "', PlayerGold = '" + _gold + "', ExpMultiplier = '" + _expM + "', DamageMultiplier = '" + _dmgM + "', HealthMultiplier = '" + _healthM + "', ManaMultiplier = '" + _manaM + "', HealingMultiplier = '" + _healingM + "' WHERE PlayerID = '1'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }
}