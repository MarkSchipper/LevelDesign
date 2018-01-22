using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{
    public class EnemyMotor : MonoBehaviour
    {

        private bool STATE_PATROL = false;
        private bool STATE_IDLE = false;
        private bool STATE_ATTACK = false;
        private bool STATE_ALIVE = true;
        private bool STATE_INRANGE = false;
        private bool STATE_TOCLOSE = false;
        private bool STATE_LEASHED = false;
        [SerializeField]
        private bool _climbOutOfGround;
        private bool _climbFinished;
        private bool _isCloseCounterSet = false;
        private bool _isLeashSet = false;

        // MOVEMENT
        private int _currentWayPoint;
        private Vector3 _wayPointTarget;
        private Vector3 _moveDirection;
        private float _enemyMoveSpeed = 1.5f;
        private float _enemyRunSpeed = 4.0f;
        [SerializeField]
        private float _attackRange;
        private float _cooldownTimer;
        private float _distanceTraveled = 0.0f;
        [SerializeField]
        private EnemyMovement _movement;

        private Vector3 _oldPosition;
        private List<GameObject> _waypoints = new List<GameObject>();

        private float _maxLeashDistance = 25f;
        private Vector3 _leashStart;
        private bool _leashingBack = false;

        private EnemyAnimationSystem _animationSystem;
        private EnemyBehaviour enemyBehaviour;
        private EnemyBattle enemyBattle;
        private EnemySoundManager enemySoundManager;

        private CharacterController _characterController;
        private GameObject _targetToAttack;

        private bool _isChargeSound = false;

        // Use this for initialization
        void Start()
        {
            _animationSystem = new EnemyAnimationSystem(GetComponent<Animator>());
            _characterController = GetComponent<CharacterController>();
            enemyBattle = GetComponent<EnemyBattle>();
            enemySoundManager = GetComponent<EnemySoundManager>();
            enemyBehaviour = GetComponent<EnemyBehaviour>();

            enemyBattle.SetAnimator(_animationSystem);

            if (!_climbOutOfGround)
            {
                _animationSystem.SetEnemyClimb(false);
                _climbFinished = true;

                if (EnemyDatabase.ReturnMovement(enemyBehaviour.ReturnEnemyID()) == EnemyMovement.Idle)
                {
                    STATE_IDLE = true;
                }
                if (EnemyDatabase.ReturnMovement(enemyBehaviour.ReturnEnemyID()) == EnemyMovement.Patrol)
                {
                    STATE_PATROL = true;
                }
            }

            if (_climbOutOfGround)
            {
                _animationSystem.StopEnemyWalking();
            }

            if (EnemyDatabase.ReturnMovement(enemyBehaviour.ReturnEnemyID()) == EnemyMovement.Patrol)
            {
                for (int i = 0; i < EnemyDatabase.ReturnWaypoints(enemyBehaviour.ReturnEnemyID()); i++)
                {
                    _waypoints.Add(GameObject.Find(enemyBehaviour.ReturnName() + "_" + enemyBehaviour.ReturnGameID() + "_WAYPOINT_" + i));
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (_climbOutOfGround)
            {
                if (_animationSystem.ClimbFinished())
                {
                    _climbFinished = true;
                    _climbOutOfGround = false;

                    if (_movement == EnemyMovement.Patrol)
                    {
                        _animationSystem.SetEnemyWalking(true);
                        STATE_PATROL = true;
                    }
                    if (_movement == EnemyMovement.Idle)
                    {
                        STATE_IDLE = true;
                    }
                }
            }
            if (!_climbOutOfGround)
            {
                if (STATE_ALIVE)
                {
                    if (!STATE_ATTACK)
                    {
                        if (STATE_IDLE)
                        {
                            Idle();
                        }
                        if (STATE_PATROL)
                        {
                            Patrol();
                        }

                        if (!STATE_LEASHED && !STATE_PATROL && !STATE_IDLE)
                        {
                            if (_leashingBack)
                            {
                                STATE_ATTACK = false;
                                if (_movement == EnemyMovement.Patrol)
                                {
                                    if (Vector3.Distance(transform.position, _waypoints[0].transform.position) > 1f)
                                    {
                                        MoveToLeashStart(_waypoints[0].transform.position);
                                    }
                                }
                                else {
                                    if (Vector3.Distance(transform.position, _leashStart) > 1f)
                                    {
                                        MoveToLeashStart(_leashStart);
                                    }
                                }
                            }
                            else
                            {
                                if (_movement == EnemyMovement.Patrol)
                                {
                                    _animationSystem.StopEnemyRunning();
                                    _animationSystem.SetEnemyWalking(true);
                                    STATE_PATROL = true;
                                    _currentWayPoint++;
                                }
                                if (_movement == EnemyMovement.Idle)
                                {
                                    STATE_IDLE = true;
                                }
                            }
                        }
                    }
                    if (STATE_ATTACK)
                    {
                        if (!_isLeashSet)
                        {
                            _leashStart = transform.position;
                            _isLeashSet = true;
                            STATE_LEASHED = true;
                            STATE_IDLE = false;
                            STATE_PATROL = false;
                        }

                        EnemyLeash(_targetToAttack);

                        if (STATE_INRANGE)
                        {
                            _animationSystem.SetDirectionFloat(1);
                            enemyBattle.AttackPlayer(this.transform.position, _targetToAttack.transform.position, _attackRange);
                            CheckDistance(_targetToAttack.transform.position);
                        }
                        if (!STATE_INRANGE)
                        {
                            _animationSystem.SetDirectionFloat(1);
                            if (_targetToAttack != null)
                            {
                                MoveToAttack(_targetToAttack.transform.position);

                            }
                        }
                        if (STATE_TOCLOSE)
                        {
                            MoveBack(_targetToAttack.transform.position);
                        }
                    }
                }
                if (!STATE_ALIVE)
                {

                    STATE_ATTACK = false;
                    STATE_IDLE = false;
                    STATE_PATROL = false;
                    _animationSystem.SetEnemyDeath();
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
                if (_dir.magnitude > 0.5f)
                {
                    _characterController.SimpleMove(_forward * _enemyMoveSpeed);
                    _animationSystem.SetEnemyWalking(true);
                    _distanceTraveled += (transform.position - _oldPosition).magnitude;

                    if (_distanceTraveled > 1.18f)
                    {
                        enemySoundManager.PlaySound(EnemySound.FOOTSTEPS, transform.position);
                        _distanceTraveled = 0.0f;
                    }

                }
            }
            else {
                _currentWayPoint = 0;
            }
        }

        void Idle()
        {
            _animationSystem.SetEnemyIdle();
        }

        void MoveToAttack(Vector3 _target)
        {
            if (!STATE_INRANGE)
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
                    _animationSystem.SetEnemyRunning(1);
                    if (!_isChargeSound)
                    {
                        CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYCHARGE, transform.position, true);
                        _isChargeSound = true;
                    }
                    _distanceTraveled += (transform.position - _oldPosition).magnitude;

                    if (_distanceTraveled > 2.2f)
                    {
                        enemySoundManager.PlaySound(EnemySound.FOOTSTEPS, transform.position);
                        _distanceTraveled = 0.0f;
                    }

                }
                else
                {
                    _animationSystem.StopEnemyRunning();
                    _animationSystem.StopEnemyWalking();
                    //_animationSystem.SetEnemyCombatIdle();
                    STATE_INRANGE = true;

                    _cooldownTimer = 4.5f;
                }
            }
        }

        void MoveBack(Vector3 _target)
        {
            if (STATE_TOCLOSE)
            {
                Vector3 _oldPosition = transform.position;
                Vector3 _dir = _target - transform.position;
                _dir.y = 0f;
                Quaternion _targetRot = Quaternion.LookRotation(_dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);

                Vector3 _back = transform.TransformDirection(Vector3.back);

                if (_dir.magnitude < _attackRange - 1)
                {

                    _characterController.SimpleMove(_back * _enemyRunSpeed);
                    _animationSystem.StopEnemyCombatIdle();
                    _animationSystem.SetEnemyRunning(-1);

                    if (!_isCloseCounterSet)
                    {
                        enemyBattle.SetCloseCounter(1);
                        _isCloseCounterSet = true;
                        StartCoroutine(CloseEncounterTimer());
                    }

                    if (!_isChargeSound)
                    {
                        CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYCHARGE, transform.position, true);
                        _isChargeSound = true;
                    }
                }
                else
                {
                    STATE_INRANGE = true;
                    STATE_TOCLOSE = false;
                    _animationSystem.SetDirectionFloat(1);
                    _animationSystem.SetEnemyCombatIdle();
                    _animationSystem.StopEnemyRunning();
                }
            }
        }

        void MoveToLeashStart(Vector3 _pos)
        {
            Debug.Log("Move to leash start " + _pos);
            Vector3 _oldPosition = transform.position;
            Vector3 _dir = _pos - transform.position;
            _dir.y = 0f;

            Quaternion _targetRotation = Quaternion.LookRotation(_dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 4);

            Vector3 _forward = transform.TransformDirection(Vector3.forward);

            if (_dir.magnitude >= 1.1f)
            {
                _characterController.SimpleMove(_forward * _enemyRunSpeed);
                _animationSystem.StopEnemyCombatIdle();
                _animationSystem.SetEnemyRunning(1);

                _distanceTraveled += (transform.position - _oldPosition).magnitude;

                if (_distanceTraveled > 1.2f)
                {
                    enemySoundManager.PlaySound(EnemySound.FOOTSTEPS, transform.position);
                    _distanceTraveled = 0.0f;
                }
            }
            else
            {
                Debug.Log("AT LEASH START");
                _leashingBack = false;
            }
        }

        void CheckDistance(Vector3 _target)
        {
            Vector3 _dir = _target - transform.position;
            _dir.y = 0f;


            if (_dir.magnitude < _attackRange - 1)
            {
                STATE_INRANGE = false;
                STATE_TOCLOSE = true;
            }

        }

        public void SetAttackRange(float _range)
        {
            _attackRange = _range;
        }

        public void SetAttack(GameObject _target)
        {
            _targetToAttack = _target;
            enemySoundManager.PlaySound(EnemySound.CHARGE, transform.position);
        }

        public void SetAttack(bool _set, GameObject _target)
        {
            STATE_ATTACK = _set;
            _targetToAttack = _target;
            STATE_PATROL = !_set;
            enemySoundManager.PlaySound(EnemySound.CHARGE, transform.position);
        }

        public void SetEnemyState(string state, bool set)
        {
            switch (state)
            {
                case "STATE_PATROL":
                    STATE_PATROL = set;
                    break;
                case "STATE_IDLE":
                    STATE_IDLE = set;
                    break;
                case "STATE_ATTACK":
                    STATE_ATTACK = set;
                    break;
                case "STATE_ALIVE":
                    STATE_ALIVE = set;
                    break;
                case "STATE_INRANGE":
                    STATE_INRANGE = set;
                    break;
                default:
                    break;
            }
        }

        public void SetEnemyFrozen()
        {
            _animationSystem.SetEnemyFrozen();
        }

        public void SetEnemyUnFrozen()
        {
            _animationSystem.SetEnemyUnFrozen();
        }

        public void ResetEnemy()
        {
            _animationSystem.StopEnemyCombatIdle();
            _animationSystem.StopEnemyRunning();

            if (_movement == EnemyMovement.Patrol)
            {
                STATE_PATROL = true;
            }
            else
            {
                STATE_IDLE = true;
            }
        }

        public bool ReturnAliveState()
        {
            return STATE_ALIVE;
        }

        public bool ReturnClimbGround()
        {
            return _climbOutOfGround;
        }

        public void EnemyStartClimb()
        {
            _animationSystem.StartEnemyClimb();
        }

        public bool ReturnClimbFinished()
        {
            return _climbFinished;
        }

        void EnemyLeash(GameObject _target)
        {
            if (Vector3.Distance(transform.position, _leashStart) > _maxLeashDistance)
            {
                STATE_ATTACK = false;
                STATE_LEASHED = false;
                _isLeashSet = false;
                _leashingBack = true;
                CombatSystem.PlayerController.instance.SetPlayerInCombat(false);
            }
        }

        public bool ReturnLeashingBack()
        {
            return STATE_LEASHED;
        }

        public float ReturnAttackRange()
        {
            return _attackRange;
        }

        public EnemyMovement ReturnMovement()
        {
            return _movement;
        }

        IEnumerator CloseEncounterTimer()
        {
            yield return new WaitForSeconds(1f);
            _isCloseCounterSet = false;
        }

      
    }
}
