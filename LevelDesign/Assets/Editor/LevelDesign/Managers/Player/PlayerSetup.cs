using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class PlayerSetup 
{

    private static int _playerHealth;
    private static int _playerMana;
    private static float _runSpeed;
    private static float _walkSpeed;
    private static float _rangedDistance;
    private static float _meleeRange;


    private static bool _loadSettings = false;



    public static void ShowPlayerSettings()
    {
        if (!_loadSettings)
        {
            CombatSystem.CombatDatabase.GetPlayerSettings();
            
            _playerHealth = CombatSystem.CombatDatabase.ReturnPlayerHealth();
            _playerMana = CombatSystem.CombatDatabase.ReturnPlayerMana();
            _runSpeed = CombatSystem.CombatDatabase.ReturnPlayerRunSpeed();
            _walkSpeed = CombatSystem.CombatDatabase.ReturnPlayerWalkSpeed();
            _rangedDistance = CombatSystem.CombatDatabase.ReturnPlayerRangedDistance();
            _meleeRange = CombatSystem.CombatDatabase.ReturnPlayerMeleeRange();
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

        GUILayout.Space(20);
        if (GUILayout.Button("SAVE SETTINGS"))
        {
            CombatSystem.CombatDatabase.SavePlayerSettings(_playerHealth, _playerMana, _runSpeed, _walkSpeed, _rangedDistance, _meleeRange);
            _loadSettings = false;
        }
    }

}
