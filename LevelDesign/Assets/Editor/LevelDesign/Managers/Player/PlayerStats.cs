using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

    public class PlayerStats : EditorWindow
    {
        private static bool _loadedData;
        private static int _playerLevel;
        private static int _playerExp;
        private static int _playerGold;
        private static float _expMultiplier;
        private static float _dmgMultiplier;
        private static float _healthMultiplier;
        private static float _manaMultiplier;
        private static float _healingMultiplier;

        private static Vector2 _scrollPos;

        public static void GetPlayerData()
        {
            CombatSystem.CombatDatabase.GetPlayerData();
            _playerLevel = CombatSystem.CombatDatabase.ReturnPlayerLevel();
            _playerExp = CombatSystem.CombatDatabase.ReturnPlayerExp();
            _playerGold = CombatSystem.CombatDatabase.ReturnPlayerGold();
            _expMultiplier = CombatSystem.CombatDatabase.ReturnExpMultiplier();
            _dmgMultiplier = CombatSystem.CombatDatabase.ReturnDamageMultiplier();
            _healthMultiplier = CombatSystem.CombatDatabase.ReturnHealthMultiplier();
            _manaMultiplier = CombatSystem.CombatDatabase.ReturnManaMultiplier();
            _healingMultiplier = CombatSystem.CombatDatabase.ReturnHealingMultiplier();
        }

        public static void ShowPlayerStatistics()
        {
            if(!_loadedData)
            {
                GetPlayerData();
                _loadedData = true;
            }
            GUILayout.Space(20);
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

            GUILayout.Space(20);

            if (GUILayout.Button("Save Changes"))
            {
                CombatSystem.CombatDatabase.UpdatePlayerData(_playerLevel, _playerExp, _playerGold, _expMultiplier, _dmgMultiplier, _healthMultiplier, _manaMultiplier, _healingMultiplier);
                _loadedData = false;
            }
            EditorGUILayout.EndScrollView();


    }

        
    }
