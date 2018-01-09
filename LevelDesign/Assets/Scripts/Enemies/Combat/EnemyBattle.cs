using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{

    public class EnemyBattle : MonoBehaviour
    {

        private float _cooldownTimer;
        [SerializeField]
        private float _cooldown;
        private EnemyAnimationSystem _animationSystem;

        private EnemyBehaviour enemyBehaviour;
        private EnemyMotor enemyMotor;
        private EnemySoundManager enemySoundManager;

        [SerializeField]
        private string _enemySpecialAttack;
        [SerializeField]
        private float _enemyDamage;
        [SerializeField]
        private EnemyType _enemyType;
        private string _enemyRangedSpell;
        private GameObject _rangedSpell;

        private Vector3 _targetVector;

        private int _closeCounter;
        private int _specialAttackCounter;

        // Use this for initialization
        void Start()
        {
            enemyBehaviour = GetComponent<EnemyBehaviour>();
            enemyMotor = GetComponent<EnemyMotor>();
            enemySoundManager = GetComponent<EnemySoundManager>();

            _specialAttackCounter = Random.Range(3, 7);

            if (_enemyRangedSpell != "")
            {
                _rangedSpell = Resources.Load("Characters/Enemies/RangedSpells/" + _enemyRangedSpell) as GameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AttackPlayer(Vector3 enemyPos, Vector3 targetPos, float attackRange)
        {
            _targetVector = targetPos;
            Vector3 _dir = targetPos - enemyPos;

            if (_closeCounter != _specialAttackCounter)
            {
                if (_dir.magnitude < attackRange)
                {
                    _dir.y = 0f;                                                                                // we dont want to move up
                    Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);
                    if (_cooldownTimer >= _cooldown)
                    {
                        if (_enemyType == EnemyType.Melee)
                        {
                            _animationSystem.SetAttackPlayer();
                            Debug.Log(_animationSystem);
                            if (this.gameObject.name == "MushroomSoldier")
                            {
                                enemySoundManager.PlaySound(EnemySound.ATTACK, transform.position);
                            }
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
            }
            else
            {
                Debug.Log("SPECIAL ATTACK");
                SpecialAttackPlayer();
                _closeCounter = 0;
            }
            if (_dir.magnitude > attackRange)
            {
                enemyMotor.SetEnemyState("STATE_INRANGE", false);
                _animationSystem.SetAttackFalse();
                _animationSystem.StopEnemyCombatIdle();
                _animationSystem.SetEnemyRunning(1);

            }
            if (_dir.magnitude < attackRange - 2)
            {
                enemyMotor.SetEnemyState("STATE_TOCLOSE", true);
                Debug.Log("to close");
            }
        }

        void SpecialAttackPlayer()
        {
            if (_enemySpecialAttack != "")
            {
                GameObject _special = Instantiate(Resources.Load("Characters/Enemies/SpecialAttacks/" + _enemySpecialAttack), _targetVector, Quaternion.identity) as GameObject;
                if (_special.GetComponent<EnemySpecialAttack>().ReturnAttackType() == SpecialAttackType.KNOCKBACK)
                {
                    CombatSystem.PlayerController.instance.AddKnockback(Vector3.back, 125f);
                }
                if (_special.GetComponent<EnemySpecialAttack>().ReturnAttackType() == SpecialAttackType.INCAPACITATE)
                {
                    CombatSystem.PlayerController.instance.PlayerWebbed(2f);
                }
                if (_special.GetComponent<EnemySpecialAttack>().ReturnAttackType() == SpecialAttackType.STUN)
                {
                    CombatSystem.PlayerController.instance.PlayerStunned(2f);
                }
            }
        }

        public void SetAnimator(EnemyAnimationSystem anim)
        {
            _animationSystem = anim;
        }

        public void SetCombatStats(float _cd, float _dmg, string _spell, EnemyType _type)
        {
            _cooldown = _cd;
            _enemyDamage = _dmg;
            _enemyRangedSpell = _spell;
            _enemyType = _type;
        }

        public void SetCloseCounter(int i)
        {
            _closeCounter += i;
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

        public float ReturnDamage()
        {
            return _enemyDamage;
        }

        public float ReturnCooldown()
        {
            return _cooldown;
        }

        public EnemyType ReturnType()
        {
            return _enemyType;
        }

        void EnemyCastSpell(Vector3 _target)
        {
            Vector3 _aimAt = _target - transform.position;

            GameObject _projectile = Instantiate(_rangedSpell, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity) as GameObject;
            _projectile.transform.rotation = Quaternion.LookRotation(_aimAt);
            _projectile.AddComponent<SpellObject>();
            _projectile.GetComponent<SpellObject>().SetDamage(_enemyDamage);
            _projectile.GetComponent<Rigidbody>().AddForce(_aimAt * 0.7f);
        }

        IEnumerator WaitToFireRangedSpell()
        {
            yield return new WaitForSeconds(1.6f);
            EnemyCastSpell(_targetVector);

        }

        IEnumerator CancelAttackAnimations()
        {

            yield return new WaitForSeconds(0.5f);
            _animationSystem.CancelAttackBool();

        }
    }
}