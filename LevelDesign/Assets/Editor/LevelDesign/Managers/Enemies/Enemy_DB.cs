using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy_DB : MonoBehaviour {

    private static Vector2 _scrollPos;
    private static string _enemyName;
    private static EnemyCombat.EnemyType _enemyType;
    private static float _enemyDamage;
    private static float _enemyCooldown;
    private static int _enemyHealth;
    private static int _enemyMana;
    private static EnemyCombat.EnemySpecial _specialAttack;
    private static EnemyCombat.EnemyMovement _enemyMovement;
    private static EnemyCombat.EnemySpawn _enemySpawn;
    private static int _enemyWaypointAmount;
    private static int _enemyAggroRange;
    private static float _enemyAttackRange;


    private static int _meleeIndex;
    private static int _rangedIndex;
    private static int _deathIndex;
    private static int _hitIndex;
    private static int _spellIndex;
    private static int _lootIndex;

    private static string _special;

    private static List<string> _meleeEnemyNames = new List<string>();
    private static List<string> _rangedEnemyNames = new List<string>();
    private static List<string> _deathFeedbackList = new List<string>();
    private static List<string> _hitFeedbackList = new List<string>();
    private static List<string> _enemyRangedSpellsList = new List<string>();

    private static UnityEngine.Object[] _meleeEnemies;
    private static UnityEngine.Object[] _rangedEnemies;
    private static UnityEngine.Object[] _deathFeedback;
    private static UnityEngine.Object[] _hitFeedback;
    private static UnityEngine.Object[] _enemyRangedSpells;

    private static bool _addedEnemy;
    private static bool _hasLoadedEnemy;
    private static bool _deleteConfirmation;
    private static int _editSelectIndex;
    private static int _deleteEnemyID;
    private static bool _deletedEnemy;

    public static void AddEnemy()
    {
        if (!_addedEnemy)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            LootDatabase.GetAllLootTables();

            GUILayout.Label("Adding an Enemy", EditorStyles.boldLabel);

            _enemyName = EditorGUILayout.TextField("Enemy Name: ", _enemyName);
            _enemyHealth = EditorGUILayout.IntField("Enemy Health: ", _enemyHealth);
            _enemyMana = EditorGUILayout.IntField("Enemy Mana: ", _enemyMana);

            _enemyType = (EnemyCombat.EnemyType)EditorGUILayout.EnumPopup("Type: ", _enemyType);
            if (_enemyType != EnemyCombat.EnemyType.None)
            {
                _enemyDamage = EditorGUILayout.FloatField("Damage: ", _enemyDamage);
                _enemyCooldown = EditorGUILayout.FloatField("Cooldown: ", _enemyCooldown);

                if (_enemyDamage > 0)
                {
                    _specialAttack = (EnemyCombat.EnemySpecial)EditorGUILayout.EnumPopup("Special Ability: ", _specialAttack);
                    _special = _specialAttack.ToString();
                }

                GUILayout.Space(20);
                GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
                if (_enemyType == EnemyCombat.EnemyType.Melee)
                {
                    _meleeIndex = EditorGUILayout.Popup("Which Prefab: ", _meleeIndex, _meleeEnemyNames.ToArray());
                }

                if (_enemyType == EnemyCombat.EnemyType.Ranged)
                {
                    _rangedIndex = EditorGUILayout.Popup("Which Prefab: ", _rangedIndex, _rangedEnemyNames.ToArray());
                }

                GUILayout.Space(20);
                _enemySpawn = (EnemyCombat.EnemySpawn)EditorGUILayout.EnumPopup("In Game by: ", _enemySpawn);

                GUILayout.Space(20);
                _enemyMovement = (EnemyCombat.EnemyMovement)EditorGUILayout.EnumPopup("Behaviour: ", _enemyMovement);

                if (_enemyMovement == EnemyCombat.EnemyMovement.Patrol)
                {
                    _enemyWaypointAmount = EditorGUILayout.IntField("How many waypoints: ", _enemyWaypointAmount);
                }

                GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);
                _enemyAggroRange = EditorGUILayout.IntField("Aggro Range: ", _enemyAggroRange);
                _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);
                if (_enemyType == EnemyCombat.EnemyType.Ranged)
                {
                    _spellIndex = EditorGUILayout.Popup("Which Spell: ", _spellIndex, _enemyRangedSpellsList.ToArray());
                }

                GUILayout.Label("[ FEEDBACK ]", EditorStyles.boldLabel);
                _deathIndex = EditorGUILayout.Popup("Death Feedback: ", _deathIndex, _deathFeedbackList.ToArray());
                _hitIndex = EditorGUILayout.Popup("Hit Feedback: ", _hitIndex, _hitFeedbackList.ToArray());

                GUILayout.Label("[ LOOT TABLE ]", EditorStyles.boldLabel);
                _lootIndex = EditorGUILayout.Popup("Loot Table: ", _lootIndex ,LootDatabase.ReturnLootTableNames().ToArray());
            }

            EditorGUILayout.BeginHorizontal();

            if (_enemyType == EnemyCombat.EnemyType.Melee)
            {
                if (GUILayout.Button("ADD ENEMY"))
                {
                    EnemyCombat.EnemyDatabase.AddEnemy(_enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _meleeEnemyNames[_meleeIndex], _enemyMovement, _enemyWaypointAmount, _enemyAggroRange, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, "", _enemySpawn, LootDatabase.ReturnLootTableNames()[_lootIndex]);
                    _addedEnemy = true;
                }
            }

            if (_enemyType == EnemyCombat.EnemyType.Ranged)
            {
                if (GUILayout.Button("ADD ENEMY"))
                {
                    EnemyCombat.EnemyDatabase.AddEnemy(_enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _rangedEnemyNames[_rangedIndex], _enemyMovement, _enemyWaypointAmount, _enemyAggroRange, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, _enemyRangedSpellsList[_spellIndex], _enemySpawn, LootDatabase.ReturnLootTableNames()[_lootIndex]);
                    _addedEnemy = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Enemy has been saved");
        }
    }

    public static void EditEnemy()
    {
        _addedEnemy = false;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("Welcome to the Edit Enemies Window", EditorStyles.boldLabel);
        GUILayout.Space(20);

        _editSelectIndex = EditorGUILayout.Popup("Select Enemy", _editSelectIndex, EnemyCombat.EnemyDatabase.ReturnAllEnemyNames().ToArray());

        if (!_hasLoadedEnemy)
        {
            _enemyName = EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex);
            _enemyHealth = EnemyCombat.EnemyDatabase.ReturnEnemyHealth(_editSelectIndex);
            _enemyMana = EnemyCombat.EnemyDatabase.ReturnEnemyMana(_editSelectIndex);
            _enemyType = EnemyCombat.EnemyDatabase.ReturnEnemyType(_editSelectIndex);
            _enemyDamage = EnemyCombat.EnemyDatabase.ReturnEnemyDamage(_editSelectIndex);
            _enemyCooldown = EnemyCombat.EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex);
            _specialAttack = EnemyCombat.EnemyDatabase.ReturnSpecial(_editSelectIndex);
            _enemyMovement = EnemyCombat.EnemyDatabase.ReturnEnemyMovement(_editSelectIndex);
            _enemyWaypointAmount = EnemyCombat.EnemyDatabase.ReturnEnemyWaypoint(_editSelectIndex);
            _enemyAggroRange = EnemyCombat.EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex);
            _enemyAttackRange = EnemyCombat.EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex);
            _enemySpawn = EnemyCombat.EnemyDatabase.ReturnEnemySpawn(_editSelectIndex);

            _hasLoadedEnemy = true;
        }

        if (GUI.changed)
        {
            _hasLoadedEnemy = false;
        }

        // edit

        _enemyName = EditorGUILayout.TextField("Enemy Name: ", _enemyName);
        _enemyHealth = EditorGUILayout.IntField("Enemy Health: ", _enemyHealth);
        _enemyMana = EditorGUILayout.IntField("Enemy Mana: ", _enemyMana);

        _enemyType = (EnemyCombat.EnemyType)EditorGUILayout.EnumPopup("Type: ", _enemyType);
        if (_enemyType != EnemyCombat.EnemyType.None)
        {
            _enemyDamage = EditorGUILayout.FloatField("Damage: ", _enemyDamage);
            _enemyCooldown = EditorGUILayout.FloatField("Cooldown: ", _enemyCooldown);

            if (_enemyDamage > 0)
            {
                _specialAttack = (EnemyCombat.EnemySpecial)EditorGUILayout.EnumPopup("Special Ability: ", _specialAttack);
                _special = _specialAttack.ToString();
            }

            GUILayout.Space(20);
            GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
            if (_enemyType == EnemyCombat.EnemyType.Melee)
            {
                _meleeIndex = EditorGUILayout.Popup("Which Prefab: ", _meleeIndex, _meleeEnemyNames.ToArray());
            }

            if (_enemyType == EnemyCombat.EnemyType.Ranged)
            {
                _rangedIndex = EditorGUILayout.Popup("Which Prefab: ", _rangedIndex, _rangedEnemyNames.ToArray());
            }

            GUILayout.Space(20);
            _enemySpawn = (EnemyCombat.EnemySpawn)EditorGUILayout.EnumPopup("In Game by: ", _enemySpawn);

            GUILayout.Space(20);
            _enemyMovement = (EnemyCombat.EnemyMovement)EditorGUILayout.EnumPopup("Behaviour: ", _enemyMovement);

            if (_enemyMovement == EnemyCombat.EnemyMovement.Patrol)
            {
                _enemyWaypointAmount = EditorGUILayout.IntField("How many waypoints: ", _enemyWaypointAmount);
            }

            GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);
            _enemyAggroRange = EditorGUILayout.IntField("Aggro Range: ", _enemyAggroRange);
            _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);
            if (_enemyType == EnemyCombat.EnemyType.Ranged)
            {
                _spellIndex = EditorGUILayout.Popup("Which Spell: ", _spellIndex, _enemyRangedSpellsList.ToArray());
            }

            GUILayout.Label("[ FEEDBACK ]", EditorStyles.boldLabel);
            _deathIndex = EditorGUILayout.Popup("Death Feedback: ", _deathIndex, _deathFeedbackList.ToArray());
            _hitIndex = EditorGUILayout.Popup("Hit Feedback: ", _hitIndex, _hitFeedbackList.ToArray());

            GUILayout.Label("[ LOOT TABLE ]", EditorStyles.boldLabel);
            _lootIndex = EditorGUILayout.Popup("Loot Table: ", _lootIndex, LootDatabase.ReturnLootTableNames().ToArray());
        }

        if (_enemyType == EnemyCombat.EnemyType.Melee)
        {
            if (GUILayout.Button("SAVE ENEMY"))
            {
                EnemyCombat.EnemyDatabase.UpdateEnemy(EnemyCombat.EnemyDatabase.ReturnEnemyID(_editSelectIndex), _enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _meleeEnemyNames[_meleeIndex], _enemyMovement, _enemyWaypointAmount, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, "", _enemySpawn, LootDatabase.ReturnLootTableNames()[_lootIndex]);
            }
        }

        if (_enemyType == EnemyCombat.EnemyType.Ranged)
        {
            if (GUILayout.Button("SAVE ENEMY"))
            {
                EnemyCombat.EnemyDatabase.UpdateEnemy(EnemyCombat.EnemyDatabase.ReturnEnemyID(_editSelectIndex), _enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _rangedEnemyNames[_rangedIndex], _enemyMovement, _enemyWaypointAmount, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, _enemyRangedSpellsList[_spellIndex], _enemySpawn, LootDatabase.ReturnLootTableNames()[_lootIndex]);
            }
        }

      
        EditorGUILayout.EndScrollView();
    }

    public static void DeleteEnemy()
    {
        _addedEnemy = false;
        GUILayout.Label("Delete an Enemy", EditorStyles.boldLabel);
        if (!_deleteConfirmation)
        {
            for (int i = 0; i < EnemyCombat.EnemyDatabase.ReturnAllEnemyID().Count; i++)
            {

                if (GUILayout.Button("Delete " + EnemyCombat.EnemyDatabase.ReturnEnemyName(i) + " from database"))
                {
                    _deleteConfirmation = true;
                    _deleteEnemyID = i;
                }

            }
        }

        if (_deleteConfirmation && !_deletedEnemy)
        {
            if (GUILayout.Button("Are you sure you want to delete " + EnemyCombat.EnemyDatabase.ReturnEnemyName(_deleteEnemyID)))
            {
                _deleteConfirmation = false;
                _deletedEnemy = true;
            }
        }

        if (_deletedEnemy)
        {
            EnemyCombat.EnemyDatabase.DeleteEnemy(EnemyCombat.EnemyDatabase.ReturnEnemyID(_deleteEnemyID));
            _deleteConfirmation = false;
            _deletedEnemy = false;
        }
    }

    public static void LoadEnemies()
    {
        _meleeEnemies = Resources.LoadAll("Characters/Enemies/Melee");
        _rangedEnemies = Resources.LoadAll("Characters/Enemies/Ranged");
        _deathFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Death/");
        _hitFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Hit/");
        _enemyRangedSpells = Resources.LoadAll("Characters/Enemies/RangedSpells");

        if (_meleeEnemies.Length > 0)
        {
            for (int i = 0; i < _meleeEnemies.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_meleeEnemies[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _meleeEnemyNames.Add(_meleeEnemies[i].ToString().Remove(_meleeEnemies[i].ToString().Length - 25));

                }
            }
        }

        if (_rangedEnemies.Length > 0)
        {
            for (int i = 0; i < _rangedEnemies.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_rangedEnemies[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _rangedEnemyNames.Add(_rangedEnemies[i].ToString().Remove(_rangedEnemies[i].ToString().Length - 25));

                }
            }
        }

        if (_deathFeedback.Length > 0)
        {
            for (int i = 0; i < _deathFeedback.Length; i++)
            {
                if (_deathFeedback[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    _deathFeedbackList.Add(_deathFeedback[i].ToString().Remove(_deathFeedback[i].ToString().Length - 25));
                }
            }
        }

        if (_hitFeedback.Length > 0)
        {
            for (int i = 0; i < _hitFeedback.Length; i++)
            {
                if (_hitFeedback[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    _hitFeedbackList.Add(_hitFeedback[i].ToString().Remove(_hitFeedback[i].ToString().Length - 25));
                }
            }
        }

        if (_enemyRangedSpells.Length > 0)
        {
            for (int i = 0; i < _enemyRangedSpells.Length; i++)
            {
                _enemyRangedSpellsList.Add(_enemyRangedSpells[i].ToString().Remove(_enemyRangedSpells[i].ToString().Length - 25));
            }
        }
    }

    public static void ClearLists()
    {
        _meleeEnemyNames.Clear();
        _rangedEnemyNames.Clear();
        _deathFeedbackList.Clear();
        _hitFeedbackList.Clear();
        _enemyRangedSpellsList.Clear();
    }

    public static void ClearValues()
    {
        _enemyName = "";
        _enemyType = EnemyCombat.EnemyType.None;
        _enemyDamage = 0;
        _enemyCooldown = 0.0f;
        _enemyHealth = 0;
        _enemyMana = 0;
        _specialAttack = EnemyCombat.EnemySpecial.None;
        _enemyMovement = EnemyCombat.EnemyMovement.None;
        _enemyWaypointAmount = 0;
        _enemyAggroRange = 0;
        _enemyAttackRange = 0f;
        _enemySpawn = EnemyCombat.EnemySpawn.None;
    }

}
