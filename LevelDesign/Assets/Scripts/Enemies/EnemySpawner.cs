using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{

    public class EnemySpawner : MonoBehaviour
    {

        [SerializeField]
        private int _enemyGameID;
        [SerializeField]
        private int _enemyID;
        [SerializeField]
        private string _enemyName;
        [SerializeField]
        private int _enemyHealth;
        [SerializeField]
        private int _enemyMana;
        [SerializeField]
        private float _enemyDamage;
        [SerializeField]
        private float _enemyAttackRange;
        [SerializeField]
        private EnemyType _enemyType;
        [SerializeField]
        private float _enemyCooldown;
        [SerializeField]
        private string _enemyDeathFeedback;
        [SerializeField]
        private string _enemyHitFeedback;
        [SerializeField]
        private EnemyMovement _enemyMovement;

        [SerializeField]
        private string _enemyRangedSpell;
        [SerializeField]
        private string _enemySpecialAttack;

        [SerializeField]
        private float _enemyAggroRange;

        [SerializeField]
        private string _enemyPrefab;

        private GameObject _rangedSpell;

        private bool _hasSpawned = false;

        private GameObject _deathParticles;
        private GameObject _hitParticles;
        private GameObject _specialAttack;

        private string _lootTable;


        void OnEnable()
        {

        }

        // Use this for initialization
        void Start()
        {
          

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetData(int _gameID, int _id, string _name, int _health, int _mana, float _dmg, float _attackRange, EnemyType _type, float _cooldown, string _death, string _hit, string _rangedSpell, string _prefab, float _range, EnemyMovement _movement, string _table)
        {
            _enemyGameID = _gameID;
            _enemyID = _id;
            _enemyName = _name;
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyDamage = _dmg;
            _enemyAttackRange = _attackRange;
            _enemyType = _type;
            _enemyCooldown = _cooldown;
            _enemyDeathFeedback = _death;
            _enemyHitFeedback = _hit;
            _enemyRangedSpell = _rangedSpell;
            _enemyPrefab = _prefab;
            _enemyAggroRange = _range;
            _enemyMovement = _movement;
            _lootTable = _table;
        }

        void OnTriggerEnter(Collider coll)
        {
            if(coll.tag == "Player")
            {
                if(!_hasSpawned)
                {
                    EnemySpawnFeedback();
                    
                    StartCoroutine(EnemySpawnDelay());
                    _hasSpawned = true;
                }
            }
        }

        void SpawnEnemy()
        {

            

            GameObject _enemyParent = new GameObject();
            _enemyParent.name = _enemyName + "_" + _enemyGameID;
            _enemyParent.tag = "EnemyMelee";
            _enemyParent.transform.SetParent(GameObject.Find("ENEMIES").transform);

            GameObject _enemy;

            if (_enemyType == EnemyType.Melee)
            {

                 _enemy = Instantiate(Resources.Load("Characters/Enemies/Melee/" + _enemyPrefab)) as GameObject;
            }
            else
            {
                 _enemy = Instantiate(Resources.Load("Characters/Enemies/Ranged/" + _enemyPrefab)) as GameObject;
            }
            
                _enemy.tag = "EnemyMelee";
                _enemy.name = _enemyName;
                _enemy.transform.SetParent(_enemyParent.transform);
                _enemy.transform.position = this.transform.position;

                _enemy.AddComponent<CapsuleCollider>();
                _enemy.GetComponent<CapsuleCollider>().isTrigger = true;
                _enemy.GetComponent<CapsuleCollider>().radius = 1.5f;
                _enemy.GetComponent<CapsuleCollider>().height = 4.0f;
                _enemy.GetComponent<CapsuleCollider>().center = new Vector3(0, 2, 0);

                _enemy.AddComponent<CharacterController>();

                // Set the radius to be 1.5f so its a bit bigger than the character
                _enemy.GetComponent<CharacterController>().radius = 1.5f;
                // Set the Height of the capsule collider so its roughly human height
                _enemy.GetComponent<CharacterController>().height = 4.0f;
                // Move the center of the capsule collider to 2 ( half of 4.0f) so the collider is resting on the ground
                _enemy.GetComponent<CharacterController>().center = new Vector3(0, 2, 0);

                // Add the EnemyCombatSystem class to the enemy
                _enemy.AddComponent<EnemyCombat.EnemyBehaviour>();
                EnemyCombat.EnemyBehaviour _ecs = _enemy.GetComponent<EnemyCombat.EnemyBehaviour>();

                //_ecs.SetEnemyStats(_enemyGameID, _enemyID, _enemyName, _enemyHealth, _enemyMana, _enemyDamage, _enemyAttackRange, _enemyType, _enemyDeathFeedback, _enemyHitFeedback, _enemyMovement, _enemyRangedSpell, _enemyCooldown, _lootTable);

                // Create a seperate GameObject for the AggroRange
                GameObject _enemyAggro = new GameObject();
                _enemyAggro.transform.position = this.transform.position;
                _enemyAggro.name = _enemyName + "_AGGRO";

                _enemyAggro.AddComponent<SphereCollider>();
                _enemyAggro.GetComponent<SphereCollider>().isTrigger = true;
                _enemyAggro.GetComponent<SphereCollider>().radius = _enemyAggroRange;
                _enemyAggro.AddComponent<EnemyCombat.EnemyTrigger>();

                //GameObject _death = Instantiate(Resources.Load("Characters/Enemies/Feedback/Death/" + EnemyDatabase.ReturnEnemyDeathFeedback(_editSelectIndex)) as GameObject );

                // Set the layer to 2 ( IGNORE RAYCAST )
                _enemyAggro.layer = 2;

                _enemyAggro.transform.parent = _enemy.transform;

                if (_enemyMovement == EnemyMovement.Patrol)
                {
                    for (int i = 0; i < EnemyDatabase.ReturnEnemyWaypoint(_enemyID); i++)
                    {
                        GameObject _wayPoint = new GameObject();

                        _wayPoint.name = _enemyName + "_" + _enemyGameID + "_WAYPOINT_" + i + "";
                        _wayPoint.transform.parent = _enemyParent.transform;


                    }
                }
          
          
        }

        void EnemySpawnFeedback()
        {
            GameObject _spawnFeedbackPosition = Instantiate(Resources.Load("Characters/Enemies/Feedback/Spawn/EnemySpawn")) as GameObject;
            _spawnFeedbackPosition.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.2f, this.transform.position.z);
           

            GameObject _spawnImpactParticles = Instantiate(Resources.Load("Characters/Enemies/Feedback/Spawn/EnemySpawn_ImpactParticles")) as GameObject;
            _spawnImpactParticles.transform.position = this.transform.position;
            _spawnImpactParticles.GetComponentInChildren<Animator>().SetBool("spawn", true);
            _spawnImpactParticles.GetComponentInChildren<ParticleSystem>().Play();
            StartCoroutine(DestroySpawnPosition(_spawnFeedbackPosition, _spawnImpactParticles));

        }
        IEnumerator EnemySpawnDelay()
        {
            yield return new WaitForSeconds(0.66f);
            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYSPAWN, this.transform.position, true);
            CombatSystem.CameraController.CameraShake(8, 1.5f);
            SpawnEnemy();
        }

        IEnumerator DestroySpawnPosition(GameObject _obj, GameObject _particles)
        {
            yield return new WaitForSeconds(2);
            Destroy(_obj);
            Destroy(_particles);
        }
    }
}