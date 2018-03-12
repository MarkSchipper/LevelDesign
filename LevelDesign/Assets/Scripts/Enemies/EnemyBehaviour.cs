using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{
    public class EnemyBehaviour : MonoBehaviour
    {
        // Singleton
        public static EnemyBehaviour instance = null;
  
        private bool STATE_LOOTABLE = false;
        private bool _isSelected    = false;
        private bool _isInRange     = false;
        private bool _isOldPosition = false;
        private bool _leashingBack  = false;
        private bool _spawnOnce     = false;

        [SerializeField] private int _gameID;
        [SerializeField] private int _enemyID;
        [SerializeField] private string _enemyName;
        [SerializeField] private float _enemyHealth;
        [SerializeField] private float _enemyMaxHealth;
        [SerializeField] private int _enemyMana;
        [SerializeField] private string _deathFeedback;
        [SerializeField] private string _hitFeedback;
        [SerializeField] private string _lootTable;
        [SerializeField] private bool _dissolveOnDeath;
        [SerializeField] private float _waitToDissolve;
        [SerializeField] private int _expToGive = 60;

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

        private EnemyMotor enemyMotor;
        private EnemyBattle enemyBattle;
        private LootGenerator _lootGenerator;
        private EnemySoundManager enemySoundManager;

        private float _cooldownTimer;

        // Use this for initialization
        void Start()
        {
            enemySoundManager = GetComponent<EnemySoundManager>();

            enemyMotor = GetComponent<EnemyMotor>();
            enemyBattle = GetComponent<EnemyBattle>();
            
            #region RESOURCE LOADING
            if (_deathFeedback != "")
            {
                _deathParticles = Resources.Load("Characters/Enemies/Feedback/Death/" + _deathFeedback) as GameObject;
            }

            if (_hitFeedback != "")
            {
                _hitParticles = Resources.Load("Characters/Enemies/Feedback/Hit/" + _hitFeedback) as GameObject;
            }

            #endregion

  //          _cooldownTimer = _cooldown;
            
        }

        // Update is called once per frame
        void Update()
        {
            
            if(_enemyHealth < 1)
            {
                enemyMotor.SetEnemyState("STATE_ALIVE", false);
                _isSelected = false;
                EnemyDeath();
            }
        }

        void OnTriggerEnter(Collider coll)
        {
           
            if (coll.tag == "PlayerRangedSpell" && coll.GetComponent<SpellObject>() != null)
            {
                if (enemyMotor.ReturnAliveState())
                {
                    _enemyHealth -= coll.GetComponent<SpellObject>().ReturnDamage();
                    CombatSystem.InteractionManager.instance.SetEnemyHealth(_enemyHealth);
                    CombatSystem.InteractionManager.instance.DisplayDamageDoneToEnemy((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);

                    GameObject _tmp = Instantiate(_hitParticles, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);

                    _targetToAttack = coll.GetComponent<SpellObject>().ReturnSpellCaster();

                    _tmp.GetComponentInChildren<ParticleSystem>().Play();

                    Destroy(_tmp, 1f);
                    Destroy(coll.gameObject);
                    
                    enemyMotor.SetEnemyState("STATE_ATTACK", true);
                    enemyMotor.SetAttack(true, coll.GetComponent<SpellObject>().ReturnSpellCaster());

                    CombatSystem.PlayerController.instance.SetPlayerInCombat(true);
                    if (!CombatSystem.PlayerController.instance.ReturnInCombat())
                    {
                        CombatSystem.PlayerController.instance.AddEnemyList(_gameID, this.GetComponent<EnemyBehaviour>());
                    }
                    //CombatSystem.PlayerController.instance.SetEnemy(this.transform.parent.gameObject);
                    
                }
            }
            else if (coll.tag == "PlayerRangedSpell" && coll.GetComponent<DebuffSpell>() != null)
            {
                if(coll.GetComponent<DebuffSpell>().ReturnDebuff() ==  "Freeze")
                {
                    GetComponentInChildren<EnemyCombat.EnemyTrigger>().SetFrozen(true);
                    enemyMotor.SetEnemyState("STATE_IDLE", true);
                    enemyMotor.SetEnemyState("STATE_PATROL", false);
                    enemyMotor.SetEnemyState("STATE_ATTACK", false);
                    enemyMotor.SetAttack(coll.GetComponent<DebuffSpell>().ReturnSpellCaster());
                    enemyMotor.SetEnemyFrozen();
                    StartCoroutine(UnfreezeEnemy(coll.GetComponent<DebuffSpell>().ReturnDuration()));
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

        public void SetEnemyStats(int _ingameID, int _id, int _health, int _mana, string _death, string _hit, string _table)
        {
            _gameID = _ingameID;
            _enemyID = _id;
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyMaxHealth = _health;
            _deathFeedback = _death;
            _hitFeedback = _hit;
            _lootTable = _table;

        }

        // Overloading the SetEnemyStats with the EnemyName
        public void SetEnemyStats(int _ingameID, int _id, string _name, int _health, int _mana,  string _death, string _hit, string _table)
        {
            _gameID = _ingameID;
            _enemyID = _id;
            _enemyName = _name;
            _enemyHealth = _health;
            _enemyMana = _mana;
            _enemyMaxHealth = _health;
            _deathFeedback = _death;
            _hitFeedback = _hit;
            _lootTable = _table;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          RETURN FUNCTIONS                                            //
        //  Pretty self explanatory                                                                             //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        public string ReturnName()
        {
            return _enemyName;
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
        
        public string ReturnEnemyLootTable()
        {

            return _lootGenerator.ReturnLootTable();
        }

        public int ReturnWayPoints()
        {
            return EnemyDatabase.ReturnWaypoints(_enemyID);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          ResetEnemy()                                                //
        //  Reset the enemy                                                                                     //
        //      Set the health to the max health                                                                //
        //      Reset the movement to the default one and stop all animations                                   //
        //                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ResetEnemy()
        {
            Debug.Log("Reset!");
            _enemyHealth = _enemyMaxHealth;
            enemyMotor.SetEnemyState("STATE_ATTACK", false);
            enemyMotor.ResetEnemy(); 
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
        //                                                                                                      //
        //                                          ***** LOOT *****                                            //
        //                                                                                                      //
        //          Set the state to lootable, for the mouse mouseover ie                                       //
        //          Generate a new LootGenerator                                                                //
        //              Generates the actual loot being displayed to the player                                 //
        //              Delete this enemy from the PlayerController enemy list                                  //                              
        //                  A list is used to make sure the player only goes OOC if all enemies are dead        //
        //                                                                                                      //
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


            enemyMotor.SetEnemyState("STATE_ALIVE", false);

            if (!_spawnOnce)
            {

                CombatSystem.CombatDatabase.UpdatePlayerExp(_expToGive);
                CombatSystem.InteractionManager.instance.DisplayExpGained(_expToGive, _targetToAttack.transform.position);

                GameObject _tmp = Instantiate(_deathParticles, transform.position, Quaternion.identity);
                _tmp.GetComponent<ParticleSystem>().Play();
                Destroy(_tmp, 5f);

                _lootVFX = Instantiate(Resources.Load("VFX/Loot_VFX"), transform.position, Quaternion.identity) as GameObject;
                _lootVFX.transform.Rotate(new Vector3(-90, 0, 0));
                _lootVFX.GetComponent<ParticleSystem>().Play();

                STATE_LOOTABLE = true;

                _lootGenerator = new LootGenerator(_lootTable, _gameID);

                CombatSystem.PlayerController.instance.DeleteEnemyListEntry(_gameID);

                enemySoundManager.PlaySound(EnemySound.DEATH, transform.position);
                
                if(_dissolveOnDeath)
                {
                    StartCoroutine(DissolveEnemy(_waitToDissolve, 1f));
                }

                _spawnOnce = true;
            }

            

            if (CombatSystem.PlayerController.instance.ReturnEnemyList().Count == 0)
            {
                CombatSystem.PlayerController.instance.SetPlayerInCombat(false);
            }

            Quest.QuestGameManager.GetAllQuests();
            string[] _splitArray = this.transform.parent.name.Split(char.Parse("_"));

            if (Quest.QuestGameManager.ReturnEnemyKillQuest(_splitArray[0]))
            {
                Quest.QuestGameManager.UpdateEnemyKillQuest();

                Quest.QuestLog.UpdateLog();
            }
            
        }

        public void DestroyEnemy()
        {
            if (!_dissolveOnDeath)
            {
                StartCoroutine(DissolveEnemy(0f, 1f));
            }
            Destroy(_lootVFX, 1f);
            Destroy(this.gameObject, 1f);
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

        IEnumerator DissolveEnemy(float _wait, float _time)
        {

            yield return new WaitForSeconds(_wait);
            float _timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;
                foreach (Renderer render in GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in render.materials)
                    {
                        mat.SetFloat("_SliceAmount", _timer);
                    }
                }
                //this.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer >= _time)
                {
                    yield break;
                }
            }
        }

        IEnumerator UnfreezeEnemy(float _time)
        {
            yield return new WaitForSeconds(_time);

            enemyMotor.SetEnemyState("STATE_IDLE", false);
            enemyMotor.SetEnemyState("STATE_ATTACK", true);
            GetComponentInChildren<EnemyCombat.EnemyTrigger>().SetFrozen(false);
            enemyMotor.SetEnemyUnFrozen();

            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.INCOMBAT);
            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYHIT, CombatSystem.PlayerController.instance.ReturnPlayerPosition(), true);
            CombatSystem.PlayerController.instance.SetPlayerInCombat(true);
            if (!CombatSystem.PlayerController.instance.ReturnInCombat())
            {
                CombatSystem.PlayerController.instance.AddEnemyList(_gameID, this.GetComponent<EnemyBehaviour>());
            }
            CombatSystem.PlayerController.instance.SetEnemy(this.transform.parent.gameObject);
            Debug.Log("UnfreezeEnemy" + this.transform.parent.gameObject);
        }
    }

}
