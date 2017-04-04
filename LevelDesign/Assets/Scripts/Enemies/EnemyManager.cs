using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


namespace EnemyCombat
{
    
    public class EnemyManager : EditorWindow
    {

        private bool _addingEnemy = false;
        private bool _editingEnemy = false;
        private bool _viewingEnemies = false;
        private bool _addingEnemyToGame = false;
        private bool _deleteEnemy = false;

        private string _enemyName;
        private EnemyType _enemyType;
        private float _enemyDamage;
        private float _enemyCooldown;
        private int _enemyHealth;
        private int _enemyMana;
        private EnemyMeleeSpecial _meleeSpecial;
        private EnemyRangedSpecial _rangedSpecial;
        private EnemyMovement _enemyMovement;
        private EnemySpawn _enemySpawn;
        private int _enemyWaypointAmount;
        private int _enemyAggroRange;
        private float _enemyAttackRange;
        

        private UnityEngine.Object[] _meleeEnemies;
        private UnityEngine.Object[] _rangedEnemies;
        private UnityEngine.Object[] _deathFeedback;
        private UnityEngine.Object[] _hitFeedback;

        private UnityEngine.Object[] _enemyRangedSpells;

        private int _meleeIndex;
        private int _rangedIndex;
        private int _deathIndex;
        private int _hitIndex;
        private int _spellIndex;

        private string _special;

        private List<string> _meleeEnemyNames = new List<string>();
        private List<string> _rangedEnemyNames = new List<string>();
        private List<string> _deathFeedbackList = new List<string>();
        private List<string> _hitFeedbackList = new List<string>();
        private List<string> _enemyRangedSpellsList = new List<string>();

        private int _editSelectIndex;
        private bool _hasLoadedEnemy = false;

        private Editor _gameObjectEditor;
        private GameObject _selectedObject;

        private bool[] _deleteEnemyBool;

        private Vector2 _scrollPos;

        [MenuItem("Level Design/Enemies/Enemy Manager")]

        static void ShowEditor()
        {
            EnemyManager _EM = EditorWindow.GetWindow<EnemyManager>();
        }

        // Use this for initialization
        void Start()
        {

        }
        
        void OnEnable()
        {
            EnemyDatabase.GetAllEnemies();

            _meleeEnemies = Resources.LoadAll("Characters/Enemies/Melee");
            _rangedEnemies = Resources.LoadAll("Characters/Enemies/Ranged");
            _deathFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Death/");
            _hitFeedback = Resources.LoadAll("Characters/Enemies/Feedback/Hit/");
            _enemyRangedSpells = Resources.LoadAll("Characters/Enemies/RangedSpells");

            _deleteEnemyBool = new bool[EnemyDatabase.ReturnAllEnemyID().Count];
            

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

            if(_rangedEnemies.Length > 0)
            {
                for (int i = 0; i < _meleeEnemies.Length; i++)
                {
                    // Create a filter so we only add the GameObjects to the loadPotionsName List
                    if (_rangedEnemies[i].GetType().ToString() == "UnityEngine.GameObject")
                    {

                        _rangedEnemyNames.Add(_rangedEnemies[i].ToString().Remove(_rangedEnemies[i].ToString().Length - 25));

                    }
                }
            }

            if(_deathFeedback.Length > 0)
            {
                for (int i = 0; i < _deathFeedback.Length; i++)
                {
                    if(_deathFeedback[i].GetType().ToString() == "UnityEngine.GameObject")
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

            if(_enemyRangedSpells.Length > 0)
            {
                for (int i = 0; i < _enemyRangedSpells.Length; i++)
                {
                    _enemyRangedSpellsList.Add(_enemyRangedSpells[i].ToString().Remove(_enemyRangedSpells[i].ToString().Length - 25));
                }
            }

            EnemyDatabase.GetAllEnemies();

        }

        void OnGUI()
        {
            

            if(!_addingEnemy && !_viewingEnemies && !_editingEnemy && !_addingEnemyToGame && !_deleteEnemy)
            {

                GUILayout.Label("Welcome to the Enemy Manager", EditorStyles.boldLabel);

                if (GUILayout.Button("Add Enemy"))
                {
                    ClearAll();
                    _addingEnemy = true;
                }

                if(GUILayout.Button("Edit an Enemy"))
                {
                    _editingEnemy = true;
                }

                if (GUILayout.Button("Delete an Enemy"))
                {
                    _deleteEnemy = true;
                }



                GUILayout.Space(50);

                if(GUILayout.Button("Add Enemy to Game"))
                {
                    _addingEnemyToGame = true;
                }
            }


            if(_addingEnemy)
            {
                AddEnemy();
            }

            if(_editingEnemy)
            {
                EditEnemies();
            }

            if(_addingEnemyToGame)
            {
                AddEnemyToGame();
            }

            if(_deleteEnemy)
            {
                DeleteEnemy();
            }

        }


        void AddEnemy()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Adding an Enemy", EditorStyles.boldLabel);

            _enemyName = EditorGUILayout.TextField("Enemy Name: ", _enemyName);
            _enemyHealth = EditorGUILayout.IntField("Enemy Health: ", _enemyHealth);
            _enemyMana = EditorGUILayout.IntField("Enemy Mana: ", _enemyMana);

            _enemyType = (EnemyType)EditorGUILayout.EnumPopup("Type: ", _enemyType);
            if(_enemyType != EnemyType.None)
            {
                _enemyDamage = EditorGUILayout.FloatField("Damage: ", _enemyDamage);
                _enemyCooldown = EditorGUILayout.FloatField("Cooldown: ", _enemyCooldown);

                if(_enemyDamage > 0)
                {
                    if(_enemyType == EnemyType.Melee)
                    {
                        _meleeSpecial = (EnemyMeleeSpecial)EditorGUILayout.EnumPopup("Special Ability: ", _meleeSpecial);
                        _special = _meleeSpecial.ToString();
                        

                    }
                    if(_enemyType == EnemyType.Ranged)
                    {
                        _rangedSpecial = (EnemyRangedSpecial)EditorGUILayout.EnumPopup("Special Ability: ", _rangedSpecial);
                        _special = _rangedSpecial.ToString();
                    }
                }

                GUILayout.Space(20);
                GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
                if (_enemyType == EnemyType.Melee)
                {
                    _meleeIndex = EditorGUILayout.Popup("Which Prefab: ", _meleeIndex, _meleeEnemyNames.ToArray());
                }

                if (_enemyType == EnemyType.Ranged)
                {
                    _rangedIndex = EditorGUILayout.Popup("Which Prefab: ", _rangedIndex, _rangedEnemyNames.ToArray());
                }

                GUILayout.Space(20);
                _enemySpawn = (EnemySpawn)EditorGUILayout.EnumPopup("In Game by: ", _enemySpawn);

                GUILayout.Space(20);
                _enemyMovement = (EnemyMovement)EditorGUILayout.EnumPopup("Behaviour: ", _enemyMovement);

                if(_enemyMovement == EnemyMovement.Patrol)
                {
                    _enemyWaypointAmount = EditorGUILayout.IntField("How many waypoints: ", _enemyWaypointAmount);
                }

                GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);
                _enemyAggroRange = EditorGUILayout.IntField("Aggro Range: ", _enemyAggroRange);
                _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);
                if (_enemyType == EnemyType.Ranged)
                {
                    _spellIndex = EditorGUILayout.Popup("Which Spell: ", _spellIndex, _enemyRangedSpellsList.ToArray());
                }

                GUILayout.Label("[ FEEDBACK ]", EditorStyles.boldLabel);
                _deathIndex = EditorGUILayout.Popup("Death Feedback: ", _deathIndex, _deathFeedbackList.ToArray());
                _hitIndex = EditorGUILayout.Popup("Hit Feedback: ", _hitIndex, _hitFeedbackList.ToArray());

            }

            EditorGUILayout.BeginHorizontal();

            if (_enemyType == EnemyType.Melee)
            {
                if (GUILayout.Button("ADD ENEMY"))
                {
                    EnemyDatabase.AddEnemy(_enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _meleeEnemyNames[_meleeIndex], _enemyMovement, _enemyWaypointAmount, _enemyAggroRange, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, "", _enemySpawn);
                    _addingEnemy = false;
                }
            }

            if (_enemyType == EnemyType.Ranged)
            {
                if (GUILayout.Button("ADD ENEMY"))
                {
                    EnemyDatabase.AddEnemy(_enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _rangedEnemyNames[_rangedIndex], _enemyMovement, _enemyWaypointAmount, _enemyAggroRange, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, _enemyRangedSpellsList[_spellIndex], _enemySpawn);
                    _addingEnemy = false;
                }
            }

            if (GUILayout.Button("BACK"))
            {
                _addingEnemy = false;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

        }

        void EditEnemies()
        {

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Welcome to the Edit Enemies Window", EditorStyles.boldLabel);
            GUILayout.Space(20);

            _editSelectIndex = EditorGUILayout.Popup("Select Enemy", _editSelectIndex, EnemyDatabase.ReturnAllEnemyNames().ToArray());

            
             
            if (!_hasLoadedEnemy)
            {
                _enemyName = EnemyDatabase.ReturnEnemyName(_editSelectIndex);
                _enemyHealth = EnemyDatabase.ReturnEnemyHealth(_editSelectIndex);
                _enemyMana = EnemyDatabase.ReturnEnemyMana(_editSelectIndex);
                _enemyType = EnemyDatabase.ReturnEnemyType(_editSelectIndex);
                _enemyDamage = EnemyDatabase.ReturnEnemyDamage(_editSelectIndex);
                _enemyCooldown = EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex);
                _meleeSpecial = EnemyDatabase.ReturnMeleeSpecial(_editSelectIndex);
                _rangedSpecial = EnemyDatabase.ReturnRangedSpecial(_editSelectIndex);
                _enemyMovement = EnemyDatabase.ReturnEnemyMovement(_editSelectIndex);
                _enemyWaypointAmount = EnemyDatabase.ReturnEnemyWaypoint(_editSelectIndex);
                _enemyAggroRange = EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex);
                _enemyAttackRange = EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex);
                _enemySpawn = EnemyDatabase.ReturnEnemySpawn(_editSelectIndex);

                _hasLoadedEnemy = true;
            }

            if(GUI.changed)
            {
                _hasLoadedEnemy = false;
            }

            // edit

            _enemyName = EditorGUILayout.TextField("Enemy Name: ", _enemyName);
            _enemyHealth = EditorGUILayout.IntField("Enemy Health: ", _enemyHealth);
            _enemyMana = EditorGUILayout.IntField("Enemy Mana: ", _enemyMana);

            _enemyType = (EnemyType)EditorGUILayout.EnumPopup("Type: ", _enemyType);
            if (_enemyType != EnemyType.None)
            {
                _enemyDamage = EditorGUILayout.FloatField("Damage: ", _enemyDamage);
                _enemyCooldown = EditorGUILayout.FloatField("Cooldown: ", _enemyCooldown);

                if (_enemyDamage > 0)
                {
                    if (_enemyType == EnemyType.Melee)
                    {
                        _meleeSpecial = (EnemyMeleeSpecial)EditorGUILayout.EnumPopup("Special Ability: ", _meleeSpecial);
                        _special = _meleeSpecial.ToString();


                    }
                    if (_enemyType == EnemyType.Ranged)
                    {
                        _rangedSpecial = (EnemyRangedSpecial)EditorGUILayout.EnumPopup("Special Ability: ", _rangedSpecial);
                        _special = _rangedSpecial.ToString();
                    }
                }

                GUILayout.Space(20);
                GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
                if (_enemyType == EnemyType.Melee)
                {
                    _meleeIndex = EditorGUILayout.Popup("Which Prefab: ", _meleeIndex, _meleeEnemyNames.ToArray());
                }

                if (_enemyType == EnemyType.Ranged)
                {
                    _rangedIndex = EditorGUILayout.Popup("Which Prefab: ", _rangedIndex, _rangedEnemyNames.ToArray());
                }

                GUILayout.Space(20);
                _enemySpawn = (EnemySpawn)EditorGUILayout.EnumPopup("In Game by: ", _enemySpawn);

                GUILayout.Space(20);
                _enemyMovement = (EnemyMovement)EditorGUILayout.EnumPopup("Behaviour: ", _enemyMovement);

                if (_enemyMovement == EnemyMovement.Patrol)
                {
                    _enemyWaypointAmount = EditorGUILayout.IntField("How many waypoints: ", _enemyWaypointAmount);
                }

                GUILayout.Label("[ COMBAT ]", EditorStyles.boldLabel);
                _enemyAggroRange = EditorGUILayout.IntField("Aggro Range: ", _enemyAggroRange);
                _enemyAttackRange = EditorGUILayout.FloatField("Attack Range: ", _enemyAttackRange);
                if (_enemyType == EnemyType.Ranged)
                {
                    _spellIndex = EditorGUILayout.Popup("Which Spell: ", _spellIndex, _enemyRangedSpellsList.ToArray());
                }

                GUILayout.Label("[ FEEDBACK ]", EditorStyles.boldLabel);
                _deathIndex = EditorGUILayout.Popup("Death Feedback: ", _deathIndex, _deathFeedbackList.ToArray());
                _hitIndex = EditorGUILayout.Popup("Hit Feedback: ", _hitIndex, _hitFeedbackList.ToArray());
            }

            EditorGUILayout.BeginHorizontal();
            if (_enemyType == EnemyType.Melee)
            {
                if (GUILayout.Button("SAVE ENEMY"))
                {
                    EnemyDatabase.UpdateEnemy(EnemyDatabase.ReturnEnemyID(_editSelectIndex), _enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _meleeEnemyNames[_meleeIndex], _enemyMovement, _enemyWaypointAmount, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, "", _enemySpawn);
                    _editingEnemy = false;
                }
            }

            if (_enemyType == EnemyType.Ranged)
            {
                if (GUILayout.Button("SAVE ENEMY"))
                {
                    EnemyDatabase.UpdateEnemy(EnemyDatabase.ReturnEnemyID(_editSelectIndex), _enemyName, _enemyHealth, _enemyMana, _enemyType, _enemyDamage, _enemyCooldown, _special, _rangedEnemyNames[_rangedIndex], _enemyMovement, _enemyWaypointAmount, _deathFeedbackList[_deathIndex], _hitFeedbackList[_hitIndex], _enemyAttackRange, _enemyRangedSpellsList[_spellIndex], _enemySpawn);
                    _editingEnemy = false;
                }
            }

            if (GUILayout.Button("BACK"))
            {
                _editingEnemy = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        void AddEnemyToGame()
        {

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            _editSelectIndex = EditorGUILayout.Popup("Select Enemy", _editSelectIndex, EnemyDatabase.ReturnAllEnemyNames().ToArray());

            if (EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyType.Melee)
            { 
                _selectedObject = Resources.Load("Characters/Enemies/Melee/" + EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex)) as GameObject;
                Debug.Log(_selectedObject);
            }

            if (EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyType.Ranged)
            {
                _selectedObject = Resources.Load("Characters/Enemies/Ranged/" + EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex)) as GameObject;
                Debug.Log(_selectedObject);
            }

            if (_selectedObject != null)
            {
                _gameObjectEditor = Editor.CreateEditor(_selectedObject);
                _gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(250, 250), EditorStyles.whiteLabel);
            }

            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("ADD ENEMY TO GAME"))
            {
                // Generate a random to use as an unique ID
                int _random = Random.Range(0, 10000);

                // Create a GLOBAL ENEMIES empty object to organize the scene
                GameObject _parent;

                if(GameObject.Find("ENEMIES") == null)
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
                _enemyPrefab.name = EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random;
                _enemyPrefab.transform.parent = _parent.transform;
                _enemyPrefab.tag = "EnemyMelee";

                #region ENEMY PLACEMENT
                if (EnemyDatabase.ReturnEnemySpawn(_editSelectIndex) == EnemySpawn.Placement)
                {


                    // the actual prefab
                    GameObject _enemy = Instantiate(_selectedObject, new Vector3(0, 0, 0), Quaternion.identity);
                    _enemy.name = EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex);
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
                    _enemy.AddComponent<EnemyCombat.EnemyCombatSystem>();

                    // Set the GameID 
                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetGameID(_random);

                    // Set the ID from the Database so we can fetch all the data
                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetEnemyID(EnemyDatabase.ReturnEnemyID(_editSelectIndex));

                    // Set the Name
                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetEnemyName(EnemyDatabase.ReturnEnemyName(_editSelectIndex));

                    // Set the Health and mana
                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetEnemyStats(EnemyDatabase.ReturnEnemyHealth(_editSelectIndex), EnemyDatabase.ReturnEnemyMana(_editSelectIndex), EnemyDatabase.ReturnEnemyDamage(_editSelectIndex), EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex), EnemyDatabase.ReturnEnemyType(_editSelectIndex));

                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetCooldown(EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex));

                    _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetFeedback(EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex), EnemyDatabase.ReturnEnemyHitFeedback(_editSelectIndex));

                    if (EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyType.Ranged)
                    {
                        _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetRangedSpell(EnemyDatabase.ReturnEnemyRangedSpell(_editSelectIndex));
                        _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetSpecialAttack(EnemyDatabase.ReturnRangedSpecial(_editSelectIndex).ToString());
                    }

                    if (EnemyDatabase.ReturnEnemyType(_editSelectIndex) == EnemyType.Melee)
                    {
                        _enemy.GetComponent<EnemyCombat.EnemyCombatSystem>().SetSpecialAttack(EnemyDatabase.ReturnMeleeSpecial(_editSelectIndex).ToString());
                    }


                    // Create a seperate GameObject for the AggroRange
                    GameObject _enemyAggro = new GameObject();
                    _enemyAggro.name = EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_AGGRO";

                    _enemyAggro.AddComponent<SphereCollider>();
                    _enemyAggro.GetComponent<SphereCollider>().isTrigger = true;
                    _enemyAggro.GetComponent<SphereCollider>().radius = EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex);
                    _enemyAggro.AddComponent<CombatSystem.EnemyTrigger>();

                    //GameObject _death = Instantiate(Resources.Load("Characters/Enemies/Feedback/Death/" + EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex)) as GameObject );

                    // Set the layer to 2 ( IGNORE RAYCAST )
                    _enemyAggro.layer = 2;

                    _enemyAggro.transform.parent = _enemy.transform;

                    // Waypoints

                    if (EnemyDatabase.ReturnEnemyMovement(_editSelectIndex) == EnemyMovement.Patrol)
                    {
                        for (int i = 0; i < EnemyDatabase.ReturnEnemyWaypoint(_editSelectIndex); i++)
                        {
                            GameObject _wayPoint = new GameObject();

                            _wayPoint.name = EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random + "_WAYPOINT_" + i + "";
                            _wayPoint.transform.parent = _enemyPrefab.transform;

                            
                        }
                    }



                    _addingEnemyToGame = false;
                }
                #endregion

                #region ENEMY SPAWN

                if(EnemyDatabase.ReturnEnemySpawn(_editSelectIndex) == EnemySpawn.Spawn)
                {
                    GameObject _spawnContainer = new GameObject();
                    _spawnContainer.name = EnemyDatabase.ReturnEnemyName(_editSelectIndex) + "_" + _random + " - SpawnLocation";
                    _spawnContainer.transform.SetParent(_enemyPrefab.transform);

                    _spawnContainer.layer = 2;

                    _spawnContainer.AddComponent<SphereCollider>();
                    _spawnContainer.GetComponent<SphereCollider>().isTrigger = true;
                    _spawnContainer.GetComponent<SphereCollider>().radius = EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex) * 1.5f;

                    _spawnContainer.AddComponent<EnemySpawner>();
                    _spawnContainer.GetComponent<EnemyCombat.EnemySpawner>().SetData(_random, EnemyDatabase.ReturnEnemyID(_editSelectIndex), EnemyDatabase.ReturnEnemyName(_editSelectIndex), EnemyDatabase.ReturnEnemyHealth(_editSelectIndex), EnemyDatabase.ReturnEnemyMana(_editSelectIndex), EnemyDatabase.ReturnEnemyDamage(_editSelectIndex), EnemyDatabase.ReturnEnemyAttackRange(_editSelectIndex), EnemyDatabase.ReturnEnemyType(_editSelectIndex), EnemyDatabase.ReturnEnemyCooldown(_editSelectIndex), EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex), EnemyDatabase.ReturnEnemyHitFeedback(_editSelectIndex), EnemyDatabase.ReturnEnemyRangedSpell(_editSelectIndex), EnemyDatabase.ReturnEnemyPrefab(_editSelectIndex), EnemyDatabase.ReturnEnemyAggroRange(_editSelectIndex), EnemyDatabase.ReturnEnemyMovement(_editSelectIndex));

                    _addingEnemyToGame = false;
                }

                #endregion
            }

            if (GUILayout.Button("BACK"))
            {
                _addingEnemyToGame = false;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

        }

        void DeleteEnemy()
        {
            GUILayout.Label("Delete an Enemy", EditorStyles.boldLabel);
            for (int i = 0; i < _deleteEnemyBool.Length; i++)
            {
             
                if(GUILayout.Button("Delete Enemy -" + EnemyDatabase.ReturnEnemyName(i)))
                {
                    DeleteFromDatabase(i);
                }
            }
        }

        void DeleteFromDatabase(int _id)
        {
            EnemyDatabase.DeleteEnemy(_id);
            _deleteEnemy = false;
        }

        void ClearAll()
        {
            _enemyName = "";
            _enemyType = EnemyType.None;
            _enemyDamage = 0;
            _enemyCooldown = 0.0f;
            _enemyHealth = 0;
            _enemyMana = 0;
            _meleeSpecial = EnemyMeleeSpecial.None;
            _rangedSpecial = EnemyRangedSpecial.None;
            _enemyMovement = EnemyMovement.None;
            _enemyWaypointAmount = 0;
            _enemyAggroRange = 0;
            _enemyAttackRange = 0f;
            _enemySpawn = EnemySpawn.None;
        }


    }
}
#endif