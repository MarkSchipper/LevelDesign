using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EnemyCombat
{
    public class EnemyCombatSystem : MonoBehaviour
    {
        [SerializeField]
        private  int _gameID;

        [SerializeField]
        private int _enemyID;

        [SerializeField]
        private string _enemyName;

        private bool _isPatrol = false;
        private bool _isIdle = false;
        private bool _isAttacking = false;
        private static bool _isAlive = true;

        private bool _chargeSound = false;

        private bool _fireRanged = false;

        [SerializeField]
        private float _enemyHealth;

        [SerializeField]
        private float _enemyMaxHealth;

        [SerializeField]
        private int _enemyMana;

        [SerializeField]
        private float _enemyDamage;

        private List<GameObject> _waypoints = new List<GameObject>();

        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private string _deathFeedback;

        [SerializeField]
        private string _hitFeedback;

        [SerializeField]
        private string _enemySpecialAttack;

        [SerializeField]
        private float _attackRange;

        [SerializeField]
        private EnemyType _enemyType;

        [SerializeField]
        private string _enemyRangedSpell;

        private int _currentWayPoint;
        private Vector3 _wayPointTarget;
        private Vector3 _moveDirection;
        private float _enemyMoveSpeed = 1.5f;
        private float _enemyRunSpeed = 4.0f;
        private Vector3 _oldPosition;

        private GameObject _deathParticles;
        private GameObject _hitParticles;

        private GameObject _rangedSpell;
        private GameObject _specialAttack;

        private CharacterController _characterController;
        private EnemyAnimationSystem EnemyAnim;

        private GameObject _targetToAttack;
        private bool _isInRange = false;

        private float _cooldownTimer = 0;
        private float _aoeCooldownTimer;

        void OnEnable()
        {
            
        }

        // Use this for initialization
        void Start()
        {

            _cooldownTimer = _cooldown;

            _aoeCooldownTimer = _cooldown;

            _characterController = GetComponentInChildren<CharacterController>();
            EnemyAnim = new EnemyAnimationSystem(GetComponent<Animator>());

            if (_deathFeedback != "")
            {
                _deathParticles = Resources.Load("Characters/Enemies/Feedback/Death/" + _deathFeedback) as GameObject;

            }

            if (_hitFeedback != "")
            {
                _hitParticles = Resources.Load("Characters/Enemies/Feedback/Hit/" + _hitFeedback) as GameObject;

            }

            if(_enemyRangedSpell != "")
            {
                _rangedSpell = Resources.Load("Characters/Enemies/RangedSpells/" + _enemyRangedSpell) as GameObject;
            }

            if(_enemySpecialAttack != "")
            {
                _specialAttack = Resources.Load("Characters/Enemies/SpecialAttacks/" + _enemySpecialAttack) as GameObject;
            }

            if (EnemyDatabase.ReturnMovement(_enemyID) == EnemyMovement.Patrol)
            {
                _isPatrol = true;
                for (int i = 0; i < EnemyDatabase.ReturnWaypoints(_enemyID); i++)
                {
                    _waypoints.Add(GameObject.Find(_enemyName + "_" + _gameID + "_WAYPOINT_" + i));

                }
            }

            if(EnemyDatabase.ReturnMovement(_enemyID) == EnemyMovement.Idle)
            {
                _isIdle = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(_isPatrol)
            {
                Patrol();
            }

            if(_isAttacking)
            {
                if(_isInRange)
                {
                    AttackPlayer();
                }
                if(!_isInRange)
                {
                    MoveToPlayer();
                }
                /*
                if (_enemyType == EnemyType.Ranged)
                {
                    if (Vector3.Distance(transform.position, _targetToAttack.transform.position) < _attackRange)

                    {
                        StartCoroutine(WaitToFireAoE());
                    }

                }
                */
            }
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "PlayerRangedSpell")
            {
                _enemyHealth -= coll.GetComponent<SpellObject>().ReturnDamage();
                CombatSystem.GameInteraction.SetEnemyHealth(_enemyHealth);
                CombatSystem.GameInteraction.DisplayDamageDoneToEnemy((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);

                GameObject _tmp = Instantiate(_hitParticles, transform.position, Quaternion.identity);

                _targetToAttack = coll.GetComponent<SpellObject>().ReturnSpellCaster();

                _tmp.GetComponent<ParticleSystem>().Play();

                StartCoroutine(KillParticleSystem(_tmp, 1));

                Destroy(coll.gameObject);

                SetTarget(_targetToAttack);
                SetAttack(true);

                CombatSystem.SoundSystem.InCombat();
                _targetToAttack.GetComponent<CombatSystem.PlayerMovement>().PlayerInCombat(true);
                _targetToAttack.GetComponent<CombatSystem.PlayerMovement>().SetEnemy(this.transform.parent.gameObject);
            }

            if(coll.tag == "PlayerAOE_DMG")
            {
                _enemyHealth -= coll.GetComponent<SpellObject>().ReturnDamage();
                CombatSystem.GameInteraction.SetEnemyHealth(_enemyHealth);

                GameObject _tmp = Instantiate(_hitParticles, transform.position, Quaternion.identity);
                _tmp.GetComponent<ParticleSystem>().Play();

                StartCoroutine(KillParticleSystem(_tmp, 1));

            }

            if (_enemyHealth < 1)
            {
                if (_isAlive)
                {
                    _isAttacking = false;
                    _isPatrol = false;
                    EnemyAnim.SetEnemyDeath();
                    StartCoroutine(WaitForDeath());
                    _isAlive = false;


                    CombatSystem.AnimationSystem.SetPlayerIdle();
                    CombatSystem.PlayerMovement.SetOutOfCombat();
                    CombatSystem.Combat.OutofCombat();

                }
            }
        }

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
                if(_dir.magnitude > 0.5f)
                {
                    _characterController.SimpleMove(_forward * _enemyMoveSpeed);
                    EnemyAnim.SetEnemyWalking(true);
                }

            }
            else {

                _currentWayPoint = 0;


            }

        }

        public void SetGameID(int _id)
        {
            _gameID = _id;
        }

        public void SetEnemyID(int _id)
        {
            _enemyID = _id;
        }

        public void SetEnemyName(string _name)
        {
            _enemyName = _name;
        }

        public void SetEnemyStats(int _health, int _mana, float _damage, float _range, EnemyType _type)
        {
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyDamage = _damage;
            _enemyMaxHealth = _health;
            _attackRange = _range;
            _enemyType = _type;
            
        }

        public void SetFeedback(string _death, string _hit)
        {
            _deathFeedback = _death;
            _hitFeedback = _hit;
        }

        public float ReturnHealth()
        {
            
            return _enemyHealth;
        }

        public float ReturnMaxHealth()
        {
            return _enemyMaxHealth;
        }

        public int ReturnMana()
        {
            return _enemyMana;
        }

        public string ReturnName()
        {
            return _enemyName;
        }

        public float ReturnEnemyDamage()
        {
            return _enemyDamage;
        }

        public void SetTarget(GameObject _target)
        {
            if(_target != null)
            {
                _targetToAttack = _target;
                _oldPosition = transform.position;
                Vector3 _dir = _target.transform.position - transform.position;                                        // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                if (_dir != Vector3.zero)
                {
                    Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player
                }
            }
        }

        public void SetAttack(bool _set)
        {
            if (_set)
            {
                _isPatrol = false;
                EnemyAnim.SetEnemyWalking(false);
            }
            if(!_set)
            {
                _isPatrol = true;
                EnemyAnim.StopEnemyCombatIdle();
                EnemyAnim.StopEnemyRunning();
            }
            _isAttacking = _set;


        }
        
        public  void KillEnemy()
        {

            _isAttacking = false;
            _isPatrol = false;
            EnemyAnim.SetEnemyDeath();
            _targetToAttack = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(WaitForDeath());
            _isAlive = false;
            Debug.Log(this.GetComponentInChildren<Renderer>().material.GetFloat("_Distance"));

        }

        public void SetCooldown(float _cd)
        {
            _cooldown = _cd;
            
        }

        public void SetRangedSpell(string _spell)
        {
            _enemyRangedSpell = _spell;
        }

        public void SetSpecialAttack(string _special)
        {
            _enemySpecialAttack = _special;
        }

        void AttackPlayer()
        {
            Vector3 _dir = _targetToAttack.transform.position - transform.position;

            if (_isInRange)
            {
               
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);

                if (_cooldownTimer >= _cooldown)
                {
                    if (_enemyType == EnemyType.Melee)
                    {
                        EnemyAnim.SetAttackPlayer();
                    }
                    if(_enemyType == EnemyType.Ranged)
                    {
                        EnemyAnim.SetRangedAttackPlayer();
                        EnemyAnim.SetEnemyCombatIdle();
                        
                        StartCoroutine(WaitToFireRangedSpell());
                        
                    }
                    StartCoroutine(CancelAttackAnimations());
                    
                    _cooldownTimer = 0.0f;
                    
                    
                }
                else
                {
                    _cooldownTimer += Time.deltaTime;
                    EnemyAnim.SetEnemyCombatIdle();
                }


            }
            if(_dir.magnitude > _attackRange) 
            {
                _isInRange = false;
                EnemyAnim.SetAttackFalse();
                EnemyAnim.StopEnemyCombatIdle();
                EnemyAnim.SetEnemyRunning();
                MoveToPlayer();
            }
        }

        void CastAOE()
        {
            if (_aoeCooldownTimer >= _cooldown)
            {
                GameObject _aoe = Instantiate(_specialAttack, transform.position, Quaternion.identity) as GameObject;
                _aoe.GetComponentInChildren<ParticleSystem>().Play();

                _aoeCooldownTimer = 0f;
                _cooldownTimer = 0f;

                StartCoroutine(KillAoE(_aoe));

            }
            else
            {
                _aoeCooldownTimer += Time.deltaTime;
            }
        }

        void MoveToPlayer()
        {
            if (!_isInRange)
            {
                _oldPosition = transform.position;
                Vector3 _dir = _targetToAttack.transform.position - transform.position;                                        // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player

                Vector3 _forward = transform.TransformDirection(Vector3.forward);                           // create a forward Vector3

                if (_dir.magnitude > _attackRange)
                {
                    _characterController.SimpleMove(_forward * _enemyRunSpeed);
                    EnemyAnim.SetEnemyRunning();
                    if (!_chargeSound)
                    {
                        CombatSystem.SoundSystem.EnemyCharge(_targetToAttack.transform.position);
                        _chargeSound = true;
                    }

                }
                else
                {
                    _isInRange = true;
                    Debug.Log("we are in range");
                    EnemyAnim.StopEnemyRunning();
                    EnemyAnim.SetEnemyCombatIdle();
                }
            }
        }

        void EnemyDeath()
        {
            
            
            Destroy(this.gameObject);

            CombatSystem.PlayerMovement.SetOutOfCombat();

        }

        void EnemyCastSpell()
        {
            Vector3 _aimAt = _targetToAttack.transform.position - transform.position;

            GameObject _projectile = Instantiate(_rangedSpell, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z ), Quaternion.identity) as GameObject;
            _projectile.transform.rotation = Quaternion.LookRotation(_aimAt);
            _projectile.AddComponent<SpellObject>();
            _projectile.GetComponent<SpellObject>().SetDamage(_enemyDamage);

          
                
                _projectile.GetComponent<Rigidbody>().AddForce(_aimAt * 0.7f);
                _fireRanged = false;
          

        }

        public static bool ReturnIsAlive()
        {
            return _isAlive;
        }

        void DissolveEnemy()
        {
            //Debug.Log(this.GetComponentInChildren<Renderer>().material.GetFloat("_Distance"));

            InvokeRepeating("DissolveOverTime", 0.1f, 0.1f);
           
        }

        void DissolveOverTime()
        {
          
            this.GetComponentInChildren<Renderer>().material.SetFloat(Shader.PropertyToID("_Distance"), this.GetComponentInChildren<Renderer>().material.GetFloat("_Distance") - 0.1f);

            if (GetComponentInChildren<Renderer>().material.GetFloat("_Distance") < 0)
            {
                CancelInvoke();
                EnemyDeath();
            }
        }

        IEnumerator WaitForDeath()
        {
            CombatSystem.SoundSystem.EnemyDeath(_targetToAttack.transform.position);

            GameObject _tmp = Instantiate(_deathParticles, transform.position, Quaternion.identity);
            _tmp.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(5);
            EnemyDeath();
        }
               
        IEnumerator KillParticleSystem(GameObject _obj, int _seconds)
        {
            yield return new WaitForSeconds(_seconds);
            Destroy(_obj);
        }

        IEnumerator CancelAttackAnimations()
        {
            
            yield return new WaitForSeconds(0.5f);
            EnemyAnim.CancelAttackBool();
            
        }

        IEnumerator WaitToFireRangedSpell()
        {
            yield return new WaitForSeconds(1.6f);
            EnemyCastSpell();
            
        }

        IEnumerator WaitToFireAoE()
        {
            EnemyAnim.SetSpecialAttack();
            yield return new WaitForSeconds(1);
            CastAOE();
            EnemyAnim.StopSpecialAttack();
        }

        IEnumerator KillAoE(GameObject _aoe)
        {
            yield return new WaitForSeconds(1);
            Destroy(_aoe);
        }

    }
}