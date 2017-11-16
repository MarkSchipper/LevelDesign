using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy_Game : MonoBehaviour {

    private static Vector2 _scrollPos;
    private static int _editSelectIndex;

    private static Editor _gameObjectEditor;
    private static GameObject _selectedObject;

    private static int _editGameSelectIndex;
    private static EnemyCombat.EnemyBehaviour _enemyData;
    private static string _enemyName;
    private static float _enemyHealth;
    private static float _enemyMana;
    private static float _enemyDamage;
    private static float _enemyCooldown;
    private static float _enemyAggroRange;
    private static float _enemyAttackRange;
    private static EnemyCombat.EnemyType _enemyType;
    private static EnemyCombat.EnemyMovement _enemyMovement;
    private static int _wayPointAmount;

    private static int _deathIndex;
    private static int _hitIndex;
    private static string _enemyLootTable;
    private static int _lootIndex;

    private static UnityEngine.Object[] _deathFeedback;
    private static UnityEngine.Object[] _hitFeedback;

    private static List<string> _deathFeedbackList = new List<string>();
    private static List<string> _hitFeedbackList = new List<string>();

    private static bool _enemySaved;
    private static bool _loadedDatabase;

    public static void ShowAddEnemyToGame()
    {
        _enemySaved = false;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        _editSelectIndex = EditorGUILayout.Popup("Select Enemy", _editSelectIndex, EnemyCombat.EnemyDatabase.ReturnAllEnemyNames().ToArray());

        if (EnemyCombat.EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyCombat.EnemyType.Melee)
        {
            _selectedObject = Resources.Load("Characters/Enemies/Melee/" + EnemyCombat.EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex)) as GameObject;

        }

        if (EnemyCombat.EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyCombat.EnemyType.Ranged)
        {
            _selectedObject = Resources.Load("Characters/Enemies/Ranged/" + EnemyCombat.EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex)) as GameObject;

        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Enemy to Game"))
        {
            // Generate a random to use as an unique ID
            int _random = Random.Range(0, 10000);

            // Create a GLOBAL ENEMIES empty object to organize the scene
            GameObject _parent;

            if (GameObject.Find("ENEMIES") == null)
            {
                _parent = new GameObject();
                _parent.name = "ENEMIES";
            }
            else
            {
                _parent = GameObject.Find("ENEMIES");
            }


            // Create the Parent GameObject for the Enemy 
            // We do this so it is easier to add childs like the trigger for the aggro range

            GameObject _enemyPrefab = new GameObject();
            _enemyPrefab.name = EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random;
            _enemyPrefab.transform.parent = _parent.transform;
            _enemyPrefab.tag = "EnemyMelee";

            #region ENEMY PLACEMENT
            if (EnemyCombat.EnemyDatabase.ReturnEnemySpawn(_editSelectIndex) == EnemyCombat.EnemySpawn.Placement)
            {
                // the actual prefab
                GameObject _enemy = Instantiate(_selectedObject, new Vector3(0, 0, 0), Quaternion.identity);
                _enemy.name = EnemyCombat.EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex);
                _enemy.tag = "EnemyMelee";
                _enemy.transform.parent = _enemyPrefab.transform;

                // Add the hitbox

                _enemy.AddComponent<CapsuleCollider>();
                _enemy.GetComponent<CapsuleCollider>().isTrigger = true;
                _enemy.GetComponent<CapsuleCollider>().radius = 1.5f;
                _enemy.GetComponent<CapsuleCollider>().height = 4.0f;
                _enemy.GetComponent<CapsuleCollider>().center = new Vector3(0, 2, 0);


                // Add the CharacterController to be able to MOVE the character and to add a Collision for combat
                _enemy.AddComponent<CharacterController>();

                // Set the radius to be 1.5f so its a bit bigger than the character
                _enemy.GetComponent<CharacterController>().radius = 1.5f;
                // Set the Height of the capsule collider so its roughly human height
                _enemy.GetComponent<CharacterController>().height = 4.0f;
                // Move the center of the capsule collider to 2 ( half of 4.0f) so the collider is resting on the ground
                _enemy.GetComponent<CharacterController>().center = new Vector3(0, 2, 0);


                // Add the EnemyCombatSystem class to the enemy
                _enemy.AddComponent<EnemyCombat.EnemyBehaviour>();
               
                _enemy.GetComponent<EnemyCombat.EnemyBehaviour>().SetEnemyStats(_random, EnemyCombat.EnemyDatabase.ReturnEnemyID(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyHealth(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyMana(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyDamage(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyType(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyHitFeedback(_editSelectIndex),EnemyCombat.EnemyDatabase.ReturnEnemyMovement(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyRangedSpell(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyLootTable(_editSelectIndex));

                // Create a seperate GameObject for the AggroRange
                GameObject _enemyAggro = new GameObject();
                _enemyAggro.name = EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_AGGRO";

                _enemyAggro.AddComponent<SphereCollider>();
                _enemyAggro.GetComponent<SphereCollider>().isTrigger = true;
                _enemyAggro.GetComponent<SphereCollider>().radius = EnemyCombat.EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex);
                _enemyAggro.AddComponent<CombatSystem.EnemyTrigger>();

                //GameObject _death = Instantiate(Resources.Load("Characters/Enemies/Feedback/Death/" + EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex)) as GameObject );

                // Set the layer to 2 ( IGNORE RAYCAST )
                _enemyAggro.layer = 2;

                _enemyAggro.transform.parent = _enemy.transform;

                // Waypoints

                if (EnemyCombat.EnemyDatabase.ReturnEnemyMovement(_editSelectIndex) == EnemyCombat.EnemyMovement.Patrol)
                {
                    for (int i = 0; i < EnemyCombat.EnemyDatabase.ReturnEnemyWaypoint(_editSelectIndex); i++)
                    {
                        GameObject _wayPoint = new GameObject();

                        _wayPoint.name = EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random + "_WAYPOINT_" + i + "";
                        _wayPoint.transform.parent = _enemyPrefab.transform;


                    }
                }
                
            }
            #endregion

            #region ENEMY SPAWN

            if (EnemyCombat.EnemyDatabase.ReturnEnemySpawn(_editSelectIndex) == EnemyCombat.EnemySpawn.Spawn)
            {
                GameObject _spawnContainer = new GameObject();
                _spawnContainer.name = EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random + " - SpawnLocation";
                _spawnContainer.transform.SetParent(_enemyPrefab.transform);

                _spawnContainer.layer = 2;

                _spawnContainer.AddComponent<SphereCollider>();
                _spawnContainer.GetComponent<SphereCollider>().isTrigger = true;
                _spawnContainer.GetComponent<SphereCollider>().radius = EnemyCombat.EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex) * 1.5f;

                _spawnContainer.AddComponent<EnemyCombat.EnemySpawner>();
                _spawnContainer.GetComponent<EnemyCombat.EnemySpawner>().SetData(_random, EnemyCombat.EnemyDatabase.ReturnEnemyID(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyName(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyHealth(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyMana(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyDamage(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyType(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyHitFeedback(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyRangedSpell(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyMovement(_editSelectIndex), EnemyCombat.EnemyDatabase.ReturnEnemyLootTable(_editSelectIndex));

            }

            #endregion
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    public static void ShowEditEnemy()
    {
        if (!_enemySaved)
        {
            if(!_loadedDatabase)
            {
                LootDatabase.GetAllLootTables();
                _loadedDatabase = true;
            }
            GUILayout.Label("Edit an Enemy in Game", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GameObject[] _allEnemies = GameObject.FindGameObjectsWithTag("EnemyMelee");
            List<string> _enemiesSorted = new List<string>();
            List<GameObject> _wayPoints = new List<GameObject>();

            for (int i = 0; i < _allEnemies.Length; i++)
            {
                if (_allEnemies[i].transform.parent.name == "ENEMIES")
                {
                    _enemiesSorted.Add(_allEnemies[i].name);

                }
            }

             _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            _editGameSelectIndex = EditorGUILayout.Popup("Which Enemy: ", _editGameSelectIndex, _enemiesSorted.ToArray());
            if (GUI.changed)
            {
                _enemyData = GameObject.Find(_enemiesSorted[_editGameSelectIndex]).GetComponentInChildren<EnemyCombat.EnemyBehaviour>();
                _enemyName = _enemyData.ReturnName();
                _enemyHealth = _enemyData.ReturnHealth();
                _enemyMana = _enemyData.ReturnMana();
                _enemyDamage = _enemyData.ReturnDamage();
                _enemyCooldown = _enemyData.ReturnCooldown();
                //Debug.Log(_enemyData.transform.parent.transform.Find("AGGRO").name);
                foreach (Transform item in _enemyData.transform)
                {

                    if (item.ToString().Contains("AGGRO"))
                    {
                        _enemyAggroRange = (int)item.GetComponent<SphereCollider>().radius;
                    }
                }
                //_enemyAggroRange = (int)_enemyData.transform.parent.GetComponentInChildren<SphereCollider>().radius;
                _enemyAttackRange = _enemyData.ReturnAttackRange();
                _enemyLootTable = _enemyData.ReturnEnemyLootTable();
                _wayPointAmount = _enemyData.ReturnWayPoints();

            }

            _enemyName = EditorGUILayout.TextField("Enemy Name: ", _enemyName);
            _enemyHealth = EditorGUILayout.FloatField("Enemy Health: ", _enemyHealth);
            _enemyMana = EditorGUILayout.FloatField("Enemy Mana: ", _enemyMana);

            GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);

            _enemyAggroRange = EditorGUILayout.FloatField("Aggro Range: ", _enemyAggroRange);
            _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);


            _enemyType = _enemyData.ReturnType();

            if (_enemyType != EnemyCombat.EnemyType.None)
            {
                _enemyDamage = EditorGUILayout.FloatField("Damage: ", _enemyDamage);
                _enemyCooldown = EditorGUILayout.FloatField("Cooldown: ", _enemyCooldown);

                GUILayout.Space(20);
                GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
            
                GUILayout.Space(20);
            
                _enemyMovement = (EnemyCombat.EnemyMovement)EditorGUILayout.EnumPopup("Behaviour: ", _enemyData.ReturnMovement());

                if (_enemyMovement == EnemyCombat.EnemyMovement.Patrol)
                {
                    GUILayout.BeginHorizontal();
                    _wayPointAmount = EditorGUILayout.IntField("How many waypoints: ", _wayPointAmount);

                    for (int i = 0; i < _wayPointAmount; i++)
                    {
                        _wayPoints.Add(GameObject.Find("" + _enemyData.ReturnName() + "_" + _enemyData.ReturnGameID() + "_WAYPOINT_" + i));
                    }
                    if (_wayPointAmount > 0)
                    {
                        if (GUILayout.Button("-", GUILayout.Width(50)))
                        {
                            RemoveWayPoint();
                            _wayPointAmount--;
                            _wayPoints.Clear();
                        }
                    }
                    if (GUILayout.Button("+", GUILayout.Width(50)))
                    {
                        AddWayPoint();
                        _wayPointAmount++;
                        _wayPoints.Clear();

                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);
                _enemyAggroRange = EditorGUILayout.FloatField("Aggro Range: ", _enemyAggroRange);
                _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);
          
                GUILayout.Label("[ FEEDBACK ]", EditorStyles.boldLabel);
                _deathIndex = EditorGUILayout.Popup("Death Feedback: ", _deathIndex, _deathFeedbackList.ToArray());
                _hitIndex = EditorGUILayout.Popup("Hit Feedback: ", _hitIndex, _hitFeedbackList.ToArray());

                GUILayout.Label("[ LOOT TABLE ]", EditorStyles.boldLabel);
                _lootIndex = EditorGUILayout.Popup("Loot Table: ", _lootIndex, LootDatabase.ReturnLootTableNames().ToArray());
            }
        
            if (GUILayout.Button("Update Enemy"))
            {
                _enemyData.SetEnemyStats(_enemyData.ReturnGameID(), _enemyData.ReturnEnemyID(), _enemyName, (int)_enemyHealth, (int)_enemyMana, _enemyDamage, _enemyAttackRange, _enemyType, _deathFeedbackList[_deathIndex],_hitFeedbackList[_hitIndex], _enemyMovement, "", _enemyCooldown, LootDatabase.ReturnLootTableNames()[_lootIndex]);
                _enemySaved = true;
                _loadedDatabase = false;
            }
            EditorGUILayout.EndScrollView();
        }
        
        if (_enemySaved)
        {
            GUILayout.Label("Enemy has been updated");
        }
        
        
    }

    static void RemoveWayPoint()
    {
        int _minus = _wayPointAmount - 1;
        DestroyImmediate(GameObject.Find(_enemyData.ReturnName() + "_" + _enemyData.ReturnGameID() + "_WAYPOINT_" + _minus));
    }

    static void AddWayPoint()
    {
        GameObject _new = new GameObject();

        _new.name = _enemyData.ReturnName() + "_" + _enemyData.ReturnGameID() + "_WAYPOINT_" + _wayPointAmount;
        _new.transform.SetParent(GameObject.Find("" + _enemyData.ReturnName() + "_" + _enemyData.ReturnGameID()).transform);

        
    }

    public static void GetAllFeedback()
    {
        _deathFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Death/");
        _hitFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Hit/");

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
    }

    public static void ClearLists()
    {
        _deathFeedbackList.Clear();
        _hitFeedbackList.Clear();
    }
}
