using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                    EnemyBehaviour Class                                              //
    //                                                                                                      //
    //  Controls the behaviour for the enemies                                                              //
    //      Idle behaviour                                                                                  //
    //      Patroling behaviour                                                                             //
    //      Attacking behaviour                                                                             //
    //      Leash                                                                                           //
    //  Spawns a prefab to add visual feedback                                                              //
    //                                                                                                      //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class EnemyBehaviour : MonoBehaviour
    {
        // Singleton
        public static EnemyBehaviour instance = null;

        private bool STATE_PATROL   = false;
        private bool STATE_IDLE     = false;
        private bool STATE_ATTACK   = false;
        private bool STATE_ALIVE    = true;
        private bool STATE_LOOTABLE = false;
        private bool _isSelected    = false;
        private bool _isInRange     = false;
        private bool _isChargeSound = false;
        private bool _isOldPosition = false;
        private bool _leashingBack  = false;
        private bool _spawnOnce     = false;
        

        [SerializeField] private int _gameID;
        [SerializeField] private int _enemyID;
        [SerializeField] private string _enemyName;
        [SerializeField] private float _enemyHealth;
        [SerializeField] private float _enemyMaxHealth;
        [SerializeField] private int _enemyMana;
        [SerializeField] private float _enemyDamage;
        [SerializeField] private float _cooldown;
        [SerializeField] private string _deathFeedback;
        [SerializeField] private string _hitFeedback;
        [SerializeField] private string _enemySpecialAttack;
        [SerializeField] private float _attackRange;
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private string _enemyRangedSpell;
        [SerializeField] private EnemyMovement _enemyMovement;
        [SerializeField] private float _maxLeashDistance;
        [SerializeField] private string _lootTable;

        private EnemyAnimationSystem _animationSystem;
        private CharacterController _characterController;

        // MOVEMENT
        private int _currentWayPoint;
        private Vector3 _wayPointTarget;
        private Vector3 _moveDirection;
        private float _enemyMoveSpeed = 1.5f;
        private float _enemyRunSpeed = 4.0f;
        private Vector3 _oldPosition;

        // LEASH
        private Vector3 _leashStart;

        // List for the waypoints
        private List<GameObject> _waypoints = new List<GameObject>();

        private GameObject _targetToAttack;
        private GameObject _rangedSpell;
        private GameObject _deathParticles;
        private GameObject _hitParticles;
        private GameObject _enemySelected;
        private GameObject _lootVFX;

        private LootGenerator _lootGenerator;

        private float _cooldownTimer;

        void OnAwake()
        {
           
        }

        // Use this for initialization
        void Start()
        {
            // Create a new EnemyAnimationSystem() with 'this animator'
            _animationSystem = new EnemyAnimationSystem(GetComponent<Animator>());

            // Get the CharacterController in a Child GameObject
            _characterController = GetComponentInChildren<CharacterController>();

            //////////////////////////////////////////////////////////////////////////////////
            //                                  ENEMY MOVEMENT                              //
            //                                                                              //
            // Check the movement of the enemy:                                             //
            // If the movement is 'Idle' set STATE_IDLE to true which triggers the idle anim   //
            // If the movement is 'Patrol'                                                  //
            //      STATE_PATROL is true                                                       //
            //      Find all the "waypoints" associated with the enemy name                 //
            //      Add it to the _waypoints List<>                                         //
            //                                                                              //
            //////////////////////////////////////////////////////////////////////////////////

            if (_enemyMovement == EnemyMovement.Patrol)
            {
                STATE_PATROL = true;
                for (int i = 0; i < EnemyDatabase.ReturnWaypoints(_enemyID); i++)
                {
                    _waypoints.Add(GameObject.Find(_enemyName + "_" + _gameID + "_WAYPOINT_" + i));

                }
            }
            if (EnemyDatabase.ReturnMovement(_enemyID) == EnemyMovement.Idle)
            {
                STATE_IDLE = true;
            }

            #region RESOURCE LOADING
            if (_enemyRangedSpell != "")
            {
                _rangedSpell = Resources.Load("Characters/Enemies/RangedSpells/" + _enemyRangedSpell) as GameObject;
            }
            if (_deathFeedback != "")
            {
                _deathParticles = Resources.Load("Characters/Enemies/Feedback/Death/" + _deathFeedback) as GameObject;
            }

            if (_hitFeedback != "")
            {
                _hitParticles = Resources.Load("Characters/Enemies/Feedback/Hit/" + _hitFeedback) as GameObject;
            }

            #endregion

            _cooldownTimer = _cooldown;
            
        }

        // Update is called once per frame
        void Update()
        {

            //////////////////////////////////////////////////////////////////////////////////////
            //                                      STATE MACHINE                               //
            //                                                                                  //
            //  First check to see if the enemy is alive ( STATE_ALIVE )                           //
            //  Check to see if the enemy is NOT attacking                                      //
            //  Check the movement and fire the corresponding function                          //
            //                                                                                  //
            //  Check if _isAttacking                                                           //
            //      If the enemy is attacking and we are in range                               //
            //          Attack the Player                                                       //
            //      If the enemy is NOT in range                                                //
            //          Move to the player                                                      //
            //                                                                                  //
            //////////////////////////////////////////////////////////////////////////////////////

            if (STATE_ALIVE)
            {
                if (!STATE_ATTACK)
                {
                    if (STATE_PATROL)
                    {
                        Patrol();
                    }
                    if (STATE_IDLE)
                    {
                        Idle();
                    }
                    if(_leashingBack)
                    {
                        MoveEnemyBack();
                    }
                }
                if (STATE_ATTACK)
                {

                    //EnemyLeash(_targetToAttack);

                    if(_isInRange)
                    {
                        AttackPlayer(_targetToAttack);
                    }
                    if(!_isInRange)
                    {
                        MoveToAttack(_targetToAttack.transform.position);
                        
                    }
                }
            }
            if(_enemyHealth < 1)
            {
                STATE_ALIVE = false;
                STATE_ATTACK = false;
                STATE_PATROL = false;
                STATE_IDLE = false;
                _isSelected = false;
                _animationSystem.SetEnemyDeath();
                EnemyDeath();
            }
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "PlayerRangedSpell")
            {
                if (STATE_ALIVE)
                {
                    _enemyHealth -= coll.GetComponent<SpellObject>().ReturnDamage();
                    CombatSystem.InteractionManager.instance.SetEnemyHealth(_enemyHealth);
                    CombatSystem.InteractionManager.instance.DisplayDamageDoneToEnemy((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);

                    GameObject _tmp = Instantiate(_hitParticles, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);

                    _targetToAttack = coll.GetComponent<SpellObject>().ReturnSpellCaster();

                    _tmp.GetComponentInChildren<ParticleSystem>().Play();

                    Destroy(_tmp, 1f);
                    Destroy(coll.gameObject);
                    
                    STATE_ATTACK = true;

                    CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.INCOMBAT);
                    CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYHIT, CombatSystem.PlayerController.instance.ReturnPlayerPosition(), true);
                    CombatSystem.PlayerController.instance.SetPlayerInCombat(true);
                    CombatSystem.PlayerController.instance.SetEnemy(this.transform.parent.gameObject);
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      SetSelected(bool _set)                                          //
        //                                                                                                      //
        //  When the Player selects an enemy                                                                    //
        //  If _set is true                                                                                     //
        //      If the selected VFX do not exist                                                                //
        //          Instantiate the prefab                                                                      //
        //          The prefab has a script which follows the selected object, parse the selected enemy         //
        //      Else, if the selected VFX prefab does exist                                                     //
        //          Destroy it first                                                                            //
        //          Instantiate it again, same as before                                                        //
        //                                                                                                      //
        //  If _set is False                                                                                    //
        //      If the _selected VFX prefab exists, destroy it                                                  //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetSelected(bool _set)
        {
            if(_set)
            {
                if (_enemySelected == null)
                {
                    _enemySelected = Instantiate(Resources.Load("Characters/Enemies/Feedback/EnemySelected") as GameObject);
                    _enemySelected.GetComponent<FollowObject>().SetObject(this.gameObject);
                }
                else
                {
                    Destroy(_enemySelected);

                    _enemySelected = Instantiate(Resources.Load("Characters/Enemies/Feedback/EnemySelected") as GameObject);
                    _enemySelected.GetComponent<FollowObject>().SetObject(this.gameObject);
                }
            }
            if(!_set)
            {
                if(_enemySelected != null)
                {
                    Destroy(_enemySelected);
                }
                else
                {

                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          SetEnemyStats()                                             //
        //                                                                                                      //
        //  This function is called from the EnemyEditor when adding an enemy to the game                       //
        //  Set all the stats needed                                                                            //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetEnemyStats(int _ingameID, int _id, int _health, int _mana, float _damage, float _range, EnemyType _type, string _death, string _hit, EnemyMovement _movement, string _spell, string _table)
        {
            _gameID = _ingameID;
            _enemyID = _id;
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyDamage = _damage;
            _enemyMaxHealth = _health;
            _attackRange = _range;
            _enemyType = _type;
            _deathFeedback = _death;
            _hitFeedback = _hit;
            _enemyMovement = _movement;
            _enemyRangedSpell = _spell;
            _lootTable = _table;
        }

        // Overloading the SetEnemyStats with the EnemyName
        public void SetEnemyStats(int _ingameID, int _id, string _name, int _health, int _mana, float _damage, float _range, EnemyType _type, string _death, string _hit, EnemyMovement _movement, string _spell, float _cd, string _table)
        {
            _gameID = _ingameID;
            _enemyID = _id;
            _enemyName = _name;
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyDamage = _damage;
            _enemyMaxHealth = _health;
            _attackRange = _range;
            _enemyType = _type;
            _deathFeedback = _death;
            _hitFeedback = _hit;
            _enemyMovement = _movement;
            _enemyRangedSpell = _spell;
            _cooldown = _cd;
            _lootTable = _table;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              Patrol()                                                //
        //                                                                                                      //
        //  If the Current Waypoint the enemy is at is smaller than the amount of waypoints                     //
        //      By default the currrent waypoint is 0                                                           //
        //  Get the new target position from the waypoints list                                                 //
        //  Store the oldPosition from the enemy                                                                //
        //  Create a new Vector3 _dir to store the direction the enemy should move to                           //
        //      Set the y vector of _dir to 0 to prevent rotating up                                            //
        //  If the _dir is not Vector3.zero                                                                     //
        //  Unity uses Quaternions to rotate, convert the Vector3 _dir to Quaternions                           //
        //  Use Spherical Lerp to rotate the enemy                                                              //
        //  Create a new Vector3.Forward and store it in _forward                                               //
        //                                                                                                      //
        //  If the distance between the waypoint and the position of the enemy is less than 0.5f ( magnitude )  //
        //      Switch to a new waypoint ( move to the new waypoint )                                           //
        //  Else                                                                                                //
        //      Move the Enemy using the CharacterController                                                    //
        //      In the AnimationSystem set the SetEnemyWalking to true                                          //
        //          Fire the Walking Animations                                                                 //
        //                                                                                                      //
        //  If we have reached to last waypoint                                                                 //
        //      Set the current waypoint to 0 ( first waypoint )                                                //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void Patrol()
        {
            if (_currentWayPoint < _waypoints.Count)
            {
                _wayPointTarget = _waypoints[_currentWayPoint].transform.position;
                _oldPosition = transform.position;
                Vector3 _dir = _wayPointTarget - transform.position;                                        // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                if (_dir != Vector3.zero)
                {
                    Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player
                }

                Vector3 _forward = transform.TransformDirection(Vector3.forward);                           // create a forward Vector3

                if (_dir.magnitude <= 0.5f)
                {
                    _currentWayPoint++;
                }
                if (_dir.magnitude > 0.5f)
                {
                    _characterController.SimpleMove(_forward * _enemyMoveSpeed);
                    _animationSystem.SetEnemyWalking(true);
                }
            }
            else {
                _currentWayPoint = 0;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              Idle()                                                  //
        //                                                                                                      //
        //  In the AnimationSystem fire the Idle animation                                                      //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void Idle()
        {
            _animationSystem.SetEnemyIdle();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  MoveToAttack(Vector3 _target)                                       //
        //                                                                                                      //
        //  Gets called when the Enemy is in Combat                                                             //
        //      Triggered to the aggro range or the Player fired a spell                                        //
        //                                                                                                      //
        //  If the Enemy is NOT in range                                                                        //
        //      Move the enemy towards the player                                                               //
        //  If the distance is greater than the _attackRange                                                    //
        //      Play the 'EnemyCharge' sound                                                                    //
        //          Auditory feedback                                                                           //
        //  If the enemy is in Range                                                                            //
        //      Fire the Combat Idle animation and stop the enemy running                                       //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void MoveToAttack(Vector3 _target)
        {
            if(!_isInRange)
            {
                Vector3 _oldPosition = transform.position;
                Vector3 _dir = _target - transform.position;
                _dir.y = 0f;                                                                                
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  

                Vector3 _forward = transform.TransformDirection(Vector3.forward);                           

                if (_dir.magnitude > _attackRange)
                {
                    _characterController.SimpleMove(_forward * _enemyRunSpeed);
                    _animationSystem.SetEnemyRunning();
                    if (!_isChargeSound)
                    {
                        CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYCHARGE, transform.position, true);
                        _isChargeSound = true;
                    }

                }
                else
                {
                    _isInRange = true;

                    _animationSystem.StopEnemyRunning();
                    _animationSystem.SetEnemyCombatIdle();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      AttackPlayer(GameObject _target)                                //
        //                                                                                                      //
        //  Set the direction we should attack: Vector3 _dir                                                    //
        //  Check if the Enemy is in range                                                                      //
        //  Again make sure we are not aiming up                                                                //
        //  Convert the direction to Quaternions                                                                //
        //  Use Spherical Lerp to rotate the enemy towards the Player                                           //
        //                                                                                                      //
        //  If the _cooldownTimer is equal or greater than the full cooldown                                    //
        //  Check what kind of Attack we should do                                                              //
        //  If the Enemy is a Melee enemy                                                                       //
        //      SetAttackPlayer - Fire a melee attack                                                           //
        // If the Enemy is a Ranged enemy                                                                       //
        //      Set the RangedAttack animation                                                                  //
        //      Since there is a delay ( spell casting ) set the Combat Idle                                    //
        //      Start a coroutine ( IENumerator ) WaitToFireRangedSpell                                         //
        //          Wait 1.6f ( animation time ) to fire the actual spell                                       //
        //                                                                                                      //
        //  Cancel all the Attack Animations to force the combat idle                                           //
        // Set the _cooldownTimer to 0.0f ( reset it )                                                          //
        //                                                                                                      //
        // If the _cooldownTimer is smaller than the full cooldown                                              //
        //      Add time to the _cooldownTimer                                                                  //
        //      Set the Combat Idle                                                                             //
        //                                                                                                      //
        // If we are not in Range                                                                               //
        //  _isInRange = false - Force the MoveToAttack function                                                //
        //  Stop attacking animations                                                                           //
        //  Stop the Combat Idle animation                                                                      //
        //  Set the enemy Running                                                                               //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void AttackPlayer(GameObject _target)
        {
            Vector3 _dir = _target.transform.position - transform.position;

            if (_isInRange)
            {
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);

                if (_cooldownTimer >= _cooldown)
                {
                    if (_enemyType == EnemyType.Melee)
                    {
                        _animationSystem.SetAttackPlayer();
                    }
                    if (_enemyType == EnemyType.Ranged)
                    {
                        _animationSystem.SetRangedAttackPlayer();

                        StartCoroutine(WaitToFireRangedSpell());

                    }
                    StartCoroutine(CancelAttackAnimations());

                    _cooldownTimer = 0.0f;


                }
                else
                {
                    _cooldownTimer += Time.deltaTime;
                    _animationSystem.SetEnemyCombatIdle();
                }


            }
            if (_dir.magnitude > _attackRange)
            {
                _isInRange = false;
                _animationSystem.SetAttackFalse();
                _animationSystem.StopEnemyCombatIdle();
                _animationSystem.SetEnemyRunning();
                
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                   EnemyCastSpell(GameObject _target)                                 //
        //                                                                                                      //
        //  Create a new Vector3 _aimAt - direction to fire the spell at                                        //
        //  Instantiate the _rangedSpell at the enemy position with a small offset                              //
        //      y + 2 - If 0 it would fire from the ground                                                      //
        //      z + 2 - Makes sure the spell is fired in front of the enemy                                     //
        //  Rotate the spell towards the enemy                                                                  //
        //  Add the SpellObject script which holds who fired it and the damage                                  //
        //      Set the amount of damage                                                                        //
        //  AddForce - make it move forward                                                                     //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void EnemyCastSpell(GameObject _target)
        {
            Vector3 _aimAt = _target.transform.position - transform.position;

            GameObject _projectile = Instantiate(_rangedSpell, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity) as GameObject;
            _projectile.transform.rotation = Quaternion.LookRotation(_aimAt);
            _projectile.AddComponent<SpellObject>();
            _projectile.GetComponent<SpellObject>().SetDamage(_enemyDamage);
            _projectile.GetComponent<Rigidbody>().AddForce(_aimAt * 0.7f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                               SetAttack(bool _set, GameObject _target)                               //
        //                                                                                                      //
        //  Sets the enemy in or out of combat                                                                  //
        //  If we are in combat it gets the GameObject which set it in Combat ( usually the player )            //
        //                                                                                                      //
        //  If the players aggro's the enemy:                                                                   //
        //          Example: SetAttack(true, PlayerGameObject)                                                  //
        //          We want the Patrol to stop and the enemy move towards the player                            //
        //              So we invert the _set boolean                                                           //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetAttack(bool _set, GameObject _target)
        {
            STATE_ATTACK = _set;
            _targetToAttack = _target;
            STATE_PATROL = !_set;

        }

        #region LEASH

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      EnemyLeash(GameObject _target)                                  //
        //                                                                                                      //
        //  If the Distance between the starting position ( _leashStart ) and its current position is greater   //
        //  _leashStart is defined by the EnemyTrigger script, when the player enters the aggro range           //
        //      than the maximum distance allowed( _maxLeashDistance )                                          //
        //  Set the enemy out of combat STATE_ATTACK = false                                                    //
        //  Set _leashingBack to true, to prevent retriggering the enemy                                        //
        //  Set the _currentWaypoint to 0, the first waypoint in the list                                       //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void EnemyLeash(GameObject _target)
        {
            if(Vector3.Distance(transform.position, _leashStart) > _maxLeashDistance)
            {
                STATE_ATTACK = false;
                _leashingBack = true;
                _currentWayPoint = 0;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                           MoveEnemyBack()                                            //
        //                                                                                                      //
        // While _leashingBack is true                                                                          //
        //      Move the enemy back to the first waypoint in the list                                           //
        //  If the distance between the enemy and the waypoint is smaller than 0.5f                             //
        //      Set _leashingBack to false to stop the enemy from moving back                                   //
        //      Set STATE_PATROL to true to force the patrol behaviour                                             //
        //      Stop the Enemy Running animation                                                                //
        //      Play the Walking animation                                                                      //
        //      Update the _currentWaypoint                                                                     //
        //  Else                                                                                                //
        //      Move the enemy towards the waypoint                                                             //
        //      Play the running animation                                                                      //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void MoveEnemyBack()
        {
           _wayPointTarget = _waypoints[0].transform.position;
            _oldPosition = transform.position;
            Vector3 _dir = _wayPointTarget - transform.position;                                        // get the Vector we are going to move to
            _dir.y = 0f;                                                                                // we dont want to move up
            if (_dir != Vector3.zero)
            {
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player
            }

            Vector3 _forward = transform.TransformDirection(Vector3.forward);                           // create a forward Vector3

            if (_dir.magnitude <= 0.5f)
            {
                _leashingBack = false;
                STATE_PATROL = true;
                _animationSystem.StopEnemyRunning();
                _animationSystem.SetEnemyWalking(true);
                _currentWayPoint++;
            }
            if (_dir.magnitude > 0.5f)
            {
                _characterController.SimpleMove(_forward * _enemyRunSpeed);
                _animationSystem.SetEnemyRunning();
            }
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          MaxLeashDistanceMet()                                       //
        //                                                                                                      //
        //  Function called by the EnemyTrigger class                                                           //
        //  If the Player is in the aggro ( OnTriggerStay )                                                     //
        //  EnemyTrigger will call this function  to make sure to perform it only once                          //
        //      If returned False set the Enemy in combat and parse the Player Object                           //
        //      If true the EnemyTrigger will do nothing                                                        //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool MaxLeashDistanceMet()
        {
            if (Vector3.Distance(transform.position, _leashStart) > _maxLeashDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //      Returns _leashingBack... duh                                                                    //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ReturnLeashingBack()
        {
            return _leashingBack;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  SetLeashStart(Vector3 _t)                                           //
        //                                                                                                      //
        //  _leastStart is set by the EnemyTrigger                                                              //
        //  When the Player enters the aggro radius we want to know where the enemy was at that time            //
        //  We need it to calculate the distance                                                                //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetLeashStart(Vector3 _t)
        {
            _leashStart = _t;
        }

        #endregion

        public bool ReturnIsAlive()
        {
            return STATE_ALIVE;
        }

        public float ReturnHealth()
        {
            return _enemyHealth;
        }

        public float ReturnMaxHealth()
        {
            return _enemyMaxHealth;
        }

        public float ReturnMana()
        {
            return _enemyMana;
        }

        public float ReturnDamage()
        {
            return _enemyDamage;
        }

        public string ReturnName()
        {
            return _enemyName;
        }

        public float ReturnCooldown()
        {
            return _cooldown;
        }

        public float ReturnAttackRange()
        {
            return _attackRange;
        }

        public EnemyType ReturnType()
        {
            return _enemyType;
        }

        public EnemyMovement ReturnMovement()
        {
            return _enemyMovement;
        }

        public int ReturnWayPointAmount()
        {
            return _waypoints.Count;
        }

        public string ReturnDeathFeedback()
        {
            return _deathFeedback;
        }

        public string ReturnHitFeedback()
        {
            return _hitFeedback;
        }

        public int ReturnGameID()
        {
            return _gameID;
        }

        public int ReturnEnemyID()
        {
            return _enemyID;
        }

        public string ReturnEnemySpell()
        {
            return _enemyRangedSpell;
        }

        public string ReturnEnemyLootTable()
        {

            return _lootTable;
            
        }

        public int ReturnWayPoints()
        {
            return EnemyDatabase.ReturnWaypoints(_enemyID);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                           EnemyDeath()                                               //
        //                                                                                                      //
        //  Destroy the enemy GameObject after 5 seconds                                                        //
        //  Destroy the feedback that the enemy is selected after 5 seconds                                     //
        //      If _spawnOnce is False ( used for user feedback )                                               //
        //          Instantiate the 'death particles' for user feedback                                         //
        //          Play the 'death particles'                                                                  //
        //          Destroy the 'death particles' after 5 seconds                                               //
        //          Set _spawnOnce to true                                                                      //
        //                                                                                                      //
        //  Set the Player out of combat                                                                        //
        //                                                                                                      //
        //      Quest Checking                                                                                  //
        //          Get all the Quests                                                                          //
        //              Split the enemy parent name after the _                                                 //
        //              Check the database if the enemy name corresponds with something in the DB               //
        //                  Update the Quest database                                                           //
        //                  Update the Quest log                                                                //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void EnemyDeath()
        {
            Destroy(this.gameObject, 120f);
            Destroy(_enemySelected, 5f);

            if (!_spawnOnce)
            {
                GameObject _tmp = Instantiate(_deathParticles, transform.position, Quaternion.identity);
                _tmp.GetComponent<ParticleSystem>().Play();
                Destroy(_tmp, 5f);

                _lootVFX = Instantiate(Resources.Load("VFX/Loot_VFX"), transform.position, Quaternion.identity) as GameObject;
                _lootVFX.transform.Rotate(new Vector3(-90, 0, 0));
                _lootVFX.GetComponent<ParticleSystem>().Play();

                STATE_LOOTABLE = true;

                _lootGenerator = new LootGenerator(_lootTable);

                CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYDEATH, _targetToAttack.transform.position, true);
                _spawnOnce = true;
            }

            
            CombatSystem.PlayerController.instance.SetPlayerInCombat(false);

            Quest.QuestDatabase.GetAllQuests();
            string[] _splitArray = this.transform.parent.name.Split(char.Parse("_"));

            if (Quest.QuestDatabase.ReturnEnemyKillQuest(_splitArray[0]))
            {
                Quest.QuestDatabase.UpdateEnemyKillQuest();

                Quest.QuestLog.UpdateLog();
            }
            
        }

        public void DestroyEnemy()
        {
            Destroy(_lootVFX);
            Destroy(this.gameObject);
        }

        IEnumerator WaitToFireRangedSpell()
        {
            yield return new WaitForSeconds(1.6f);
            EnemyCastSpell(_targetToAttack);

        }

        IEnumerator CancelAttackAnimations()
        {

            yield return new WaitForSeconds(0.5f);
            _animationSystem.CancelAttackBool();

        }

        public bool ReturnLootable()
        {
            return STATE_LOOTABLE;
        }
      
        public List<LootTypes> ReturnLootTypes()
        {
            return _lootGenerator.ReturnLootType();
        }

        public List<int> ReturnLootValues()
        {
            return _lootGenerator.ReturnValueList();
        }

        public List<int> ReturnLootItemID()
        {
            return _lootGenerator.ReturnItemIDList();
        }

        public void RemoveFromLoot(LootTypes _gold)
        {
            _lootGenerator.DeleteEntry(LootTypes.Gold, _lootTable);
        }

        public void RemoveFromLoot(LootTypes _item, int _id)
        {
            _lootGenerator.DeleteEntry(LootTypes.Items, _id, _lootTable);
        }

    }

}
