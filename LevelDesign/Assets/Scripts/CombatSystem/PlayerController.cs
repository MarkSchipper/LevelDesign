using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace CombatSystem
{
    
    public class PlayerController : MonoBehaviour
    {
        // Booleans
        private bool STATE_WALKING      = false;
        private bool STATE_RUNNING      = false;
        private bool STATE_BACKWARDS    = false;
        private bool STATE_IDLE         = false;
        private bool STATE_INCOMBAT     = false;
        private bool STATE_JUMPING      = false;
        private bool STATE_DEAD         = false;
        private bool STATE_REGENERATE   = false;

        private bool _jumpOnce          = false;
        private bool _hasTakenDamage    = false;
        private bool _isCastingSpell    = false;
        private bool _spellcastComplete = false;
        private bool _isCameraDragging  = false;
        private bool _isLevelUp         = false;
        private bool _isBlink           = false;
        private bool _isOverUI          = false;
        private bool _isFirstPerson     = false;
        private bool INPUT_BLOCK        = false;
        private bool _hasBarrier        = false;

        private bool _setOnce           = false;
        private bool _regen             = false;

        // Movement Floats
        private float _runSpeed = 0.3f;
        private float _walkSpeed = 0.15f;
        private float _rotateAngle = 0f;
        private float _rotatedAngle = 0f;
        private float _playerDistanceTraveled = 0.0f;
        private float _playerFallingDistance = 0.0f;
        private float _blinkMaxDistance;

        // out of combat timer
        private float _oocTimer;

        // Player stats
        private float _playerHealth;
        private float _playerMana;
        private float _playerRunSpeed;
        private float _playerWalkSpeed;
        private float _playerAttackRange;
        private int _playerLevel;
        private float _playerExperience;
        private float _playerGold;
        private float _expMultiplier;
        private float _damageMultiplier;
        private float _healthMultiplier;
        private float _manaMultiplier;
        private float _healingMultiplier;
        private float _experienceRequired;

        private Vector3 _moveDirection = new Vector3(0, 0, 0);
        private Vector3 _blinkTargetPosition;

        private CharacterController _charController;
        private CameraController _camController;

        private GameObject _selectedNPC;
        private GameObject _previousNPC;
        private GameObject _selectedActor;
        private GameObject _blinkMarker;
        private GameObject _barrierGameObject;
        private GameObject _debuffGameObject;

        private GameObject _playerMesh;

        private List<int> _enemyList = new List<int>();
        private List<EnemyCombat.EnemyBehaviour> _enemyGameObjectList = new List<EnemyCombat.EnemyBehaviour>();

        private Ray _ray;

        private List<Animator> _animators = new List<Animator>();

        private CombatSystem.PlayerMovement PLAYER_MOVEMENT;

        // Singleton
        public static PlayerController instance = null;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                               Awake()                                                    //
        //                                                                                                          //
        //  Create the singleton                                                                                    //
        //      Comparable to 'static'                                                                              //
        //          Downside of static is that you cant use, for example, transform.position                        //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

         

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            
        }

        void OnEnable()
        {
            LoadPlayerData();

            foreach (Animator anim in GetComponentsInChildren<Animator>()) 
            {

                if(anim.runtimeAnimatorController.ToString() == "PlayerController (UnityEngine.AnimatorController)")
                {
                    CombatSystem.AnimationSystem.SetController(anim);
                }
                if(anim.runtimeAnimatorController.ToString() == "StaffController (UnityEngine.AnimatorController)")
                {
                    CombatSystem.AnimationSystem.SetStaffController(anim);

                }

                if(anim.name == "PlayerMesh")
                {
                    _playerMesh = anim.gameObject;
                }
            }

            if (GetComponent<CombatSystem.PlayerMovement>() == null)
            {
                this.gameObject.AddComponent<CombatSystem.PlayerMovement>();  
            }
            else
            {
                PLAYER_MOVEMENT = GetComponent<CombatSystem.PlayerMovement>();
                PLAYER_MOVEMENT.SetValues(_playerRunSpeed, _playerWalkSpeed);
            }
           
        }  

        // Use this for initialization
        void Start()
        {
            _charController = this.GetComponent<CharacterController>();

            InteractionManager.instance.SetPlayerMaxHealth(_playerHealth);
            InteractionManager.instance.SetPlayerMaxMana(_playerMana);

            if (CombatSystem.CameraController.ReturnFirstPerson())
            {
                foreach (SkinnedMeshRenderer skin in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    skin.enabled = false;
                }
                
            }

            
        }

        // Update is called once per frame
        void Update()
        {
            if (!STATE_DEAD)
            {
                if (!INPUT_BLOCK)
                {
                    PLAYER_MOVEMENT.CheckInputs();
                    CheckMouseOver();
                }

                if (INPUT_BLOCK)
                {

                    STATE_IDLE = true;
                    STATE_RUNNING = false;

                    STATE_INCOMBAT = false;
                    _moveDirection = new Vector3(0, 0, 0);
                    _selectedNPC = null;
                }

                //////////////////////////////////////////////////////////////////////////
                //                                                                      //
                //                          STATE MACHINE                               //
                //                                                                      //
                //////////////////////////////////////////////////////////////////////////

                if (STATE_IDLE && !STATE_INCOMBAT)
                {
                    PlayerIdle();
                }
                if (STATE_WALKING)
                {
                    PLAYER_MOVEMENT.PlayerWalking();
                }
                if (STATE_RUNNING)
                {
                    PLAYER_MOVEMENT.PlayerRunning();
                }
                if (STATE_JUMPING)
                {
                    if (!_jumpOnce)
                    {
                        PLAYER_MOVEMENT.PlayerJump();
                        _jumpOnce = true;
                    }
                    if (_jumpOnce)
                    {
                        if (CombatSystem.AnimationSystem.ReturnJumpingFinished())
                        {
                            CombatSystem.AnimationSystem.StopPlayerJumping();
                            _jumpOnce = false;
                        }
                    }
                }

                if (STATE_INCOMBAT)
                {
                    PlayerCombatIdle();
                }

                if (!STATE_RUNNING || !STATE_WALKING || !STATE_INCOMBAT || STATE_JUMPING)
                {
                    STATE_IDLE = true;
                }

                Immunity();

                if (_isBlink)
                {
                    SetBlinkTarget();
                }

                if (_hasBarrier)
                {
                    if (_barrierGameObject != null)
                    {
                        _barrierGameObject.transform.position = transform.position;
                    }
                }

                if (!STATE_INCOMBAT)
                {
                    _oocTimer += Time.deltaTime;
                    if (_oocTimer > 5)
                    {
                        PlayerRegenerate();
                    }
                }
            }
        }
       
        public void SetPlayerState(string _state, bool _set)
        {
            switch (_state)
            {
                case "STATE_WALKING":
                    STATE_WALKING = _set;
                    break;
                case "STATE_RUNNING":
                    STATE_RUNNING = _set;
                    break;
                case "STATE_BACKWARDS":
                    STATE_BACKWARDS = _set;
                    break;
                case "STATE_IDLE":
                    STATE_IDLE = _set;
                    break;
                case "STATE_INCOMBAT":
                    STATE_INCOMBAT = _set;
                    break;
                case "STATE_JUMPING":
                    STATE_JUMPING = _set;
                    break;
                case "STATE_DEAD":
                    STATE_DEAD = _set;
                    break;
                case "STATE_REGENERATE":
                    STATE_REGENERATE = _set;
                    break;
                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                          //
        //                                                  CHECK INPUTS                                            //
        //                                                                                                          //
        //  If the player is grounded                                                                               //
        //  Check if the player pressed WASD, if so change the _moveDirection                                       //
        //                                                                                                          //
        //  If the player is not grounded                                                                           //
        //      Apply gravity                                                                                       //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // If the Player is idle, stop playing the running and/or walking animations
        void PlayerIdle()
        {

            CombatSystem.AnimationSystem.StopPlayerRunning();
            CombatSystem.AnimationSystem.StopPlayerWalking();

            // Start the Idle animation
            CombatSystem.AnimationSystem.SetPlayerIdle();
        } 

        void PlayerCombatIdle()
        {
            CombatSystem.AnimationSystem.SetCombatIdle();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                  RotatePlayer()                                          //
        //                                                                                                          //
        //  Rotate the player based on the amount parsed to the function                                            //
        //  _rotateAngel + the amount                                                                               //
        //  Convert a new Vector3 to Quaternion, transform.rotation uses Quaternions                                //
        //  Rotate the player character                                                                             //  
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RotatePlayer(float _amount)
        {
              
            _rotateAngle += _amount;
            // We use Euler in this case, dirty but works
            transform.rotation = Quaternion.Euler(new Vector3(0, _rotateAngle, 0));
            //transform.rotation = Quaternion.Slerp(transform.rotation, turnAngle, Time.deltaTime * 100f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          CheckMouseOver()                                                //
        //                                                                                                          //
        //  If the player is NOT hovering over the UI                                                               //
        //      If there is a Raycast hit                                                                           //
        //          If the object hit is an NPC                                                                     //
        //              Highlight the NPC the mouse is hovering over                                                //
        //          If the oject is an Enemy                                                                        //
        //              Set the Combat Cursor                                                                       //
        //          If it is neither then convert it to the regular cursor                                          //
        //                                                                                                          //
        //          If we have Right Clicked                                                                        //
        //              Again the if it is an NPC                                                                   //
        //                  Set the NPC to the 'Selected' state                                                     //
        //                  Update the InteractionManager                                                           //
        //              If it is an Enemy                                                                           //
        //                  Set the Enemy to Selected                                                               //
        //                  Update the InteractionManager                                                           //
        //          If we have Left Clicked                                                                         //
        //              Deselect everything                                                                         //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void CheckMouseOver()
        {
        
            if (!InteractionManager.instance.ReturnHoveringOverUI()) {

                if (!CombatSystem.CameraController.ReturnFirstPerson())
                {
                    _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                }
                if(CombatSystem.CameraController.ReturnFirstPerson())
                {
                   _ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                }
                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit))
                {
                    // If the mouse is over an NPC
                    if (_hit.collider.tag == "NPC")
                    {
                        if (_hit.collider.gameObject.GetComponent<NPC.NpcSystem>().ReturnHasConversation())
                        {
                            // Set the NPC cursor
                            InteractionManager.instance.SetNpcCursor();

                            // Highlight the specific NPC ( _hit.collider.gameObject )
                            _hit.collider.gameObject.GetComponent<NPC.NpcSystem>().HighlightNPC(true, _hit.collider.gameObject);
                            _selectedNPC = _hit.collider.gameObject;
                        }
                    }

                    // If the mouse is over an Enemy
                    if (_hit.collider.tag == "EnemyRanged" || _hit.collider.tag == "EnemyMelee")
                    {
                        if (!_hit.collider.gameObject.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnLootable())
                        {
                            // Set the Combat cursor
                            InteractionManager.instance.SetCombatCursor();
                        }
                        else
                        {
                            InteractionManager.instance.SetNpcCursor();
                        }
                    }

                    // If the mouse is NOT over an NPC or enemy
                    if (_hit.collider.tag != "NPC" && _hit.collider.tag != "EnemyRanged" && _hit.collider.tag != "EnemyMelee")
                    {
                        // Set the normal cursor
                        InteractionManager.instance.SetNormalCursor();

                        // If there is NO NPC selected, set the highlight to false
                        if (_selectedNPC != null)
                        {
                            if (!_selectedNPC.GetComponent<NPC.NpcSystem>().ReturnIsSelected())
                            {
                                _selectedNPC.GetComponent<NPC.NpcSystem>().HighlightNPC(false, null);
                            }

                        }
                    }

                    // If we pressed the Left Mouse Button
                    if (Input.GetMouseButtonDown(0))
                    {

                        if (!_isCameraDragging)
                        {
                            if (!InteractionManager.instance.ReturnHoveringOverUI())
                            {
                                if (_hit.collider.tag == "NPC")
                                {
                                    if (_previousNPC != null)
                                    {
                                        _previousNPC.GetComponent<NPC.NpcSystem>().IsSelected(false);
                                        _previousNPC = null;
                                    }
                                    _selectedNPC = _hit.collider.gameObject;

                                    // Set the NPC selected to "Selected" in its NPC Class
                                    if (_hit.collider.gameObject.GetComponent<NPC.NpcSystem>().ReturnHasConversation())
                                    {
                                        _selectedNPC.GetComponentInChildren<NPC.NpcSystem>().IsSelected(true);
                                        _selectedNPC.gameObject.GetComponentInChildren<NPC.NpcSystem>().SetInteraction(true);

                                        InteractionManager.instance.SetSelected(_selectedNPC);
                                    }
                                    _previousNPC = _selectedNPC;

                                }
                                else if (_hit.collider.tag == "EnemyMelee")
                                {
                                    
                                    if(_selectedActor != _hit.collider.gameObject && _selectedActor != null)
                                    {
                                        _selectedActor.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetSelected(false);
                                    }

                                    if (!_hit.collider.gameObject.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnLootable())
                                    {
                                        _selectedActor = _hit.collider.gameObject;

                                        InteractionManager.instance.SetSelected(_selectedActor);
                                        _selectedActor.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetSelected(true);
                                    }
                                    else
                                    {
                                        _selectedActor = _hit.collider.transform.parent.gameObject;
                                        
                                        if (_selectedActor.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnEnemyLootTable() != string.Empty)
                                        {
                                            Inventory.instance.ShowLootWindow(_selectedActor);
                                        }
                                    }
                                }
                                else
                                {
                                    
                                    _selectedActor = null;
                                    InteractionManager.instance.SetSelected(null);
                                }

                                if (!Inventory.instance.ReturnShowLootWindow())
                                {
                                    if (_hit.collider.tag != "NPC" && _hit.collider.tag != "EnemyMelee")
                                    {
                                        InteractionManager.instance.SetSelected(null);
                                        if (_selectedActor != null)
                                        {

                                            if (_selectedActor.tag == "EnemyMelee")
                                            {
                                                _selectedActor.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetSelected(false);
                                            }
                                        }

                                        if (_selectedNPC != null)
                                        {
                                            _selectedNPC.GetComponent<NPC.NpcSystem>().IsSelected(false);
                                        }

                                        _selectedActor = null;
                                        _selectedNPC = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Camera dragging");
                        }
                    }

                    if(Input.GetMouseButtonDown(1))
                    {
                        if (_isBlink)
                        {
                            _isBlink = !_isBlink;
                            if (_blinkMarker != null)
                            {
                                DestroyImmediate(_blinkMarker);
                            }
                        }
                    }
                }
            }
            if(_isOverUI)
            {
                InteractionManager.instance.SetNormalCursor();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      OnTriggerEnter(Collider coll)                                       //
        //                                                                                                          //
        //  If a GameObject hit us                                                                                  //
        //      Check to see if the GameObject has the Component 'SpellObject'                                      //
        //          SpellObject - Class that all projectiles get                                                    //
        //          If the GameObject has not been cast by the Player                                               //
        //              Meaning it is coming from an enemy                                                          //
        //              If the Player has not taken any damage in the last few seconds                              //
        //                  Check if the Players HP - the damage done by the spell is greater than 0                //
        //                      The Player can take damage without dieing                                           //                                
        //                      Reduce the the players HP by the amount given by the SpellObect class               //
        //                          Update the UI                                                                   //
        //                              Display Feedback                                                            //
        //                                  Destroy the GameObject                                                  //
        //                                  Play the audio feedback                                                 //
        //                                  Set _hasTakenDamage to true                                             //
        //                                      If the player is not yet in Combat set the player in Combat         //
        //                  If the Players HP - the damage done by the spell is less than 0                         //
        //                      The Player dies                                                                     //
        //      If not                                                                                              //
        //          Check if the GameObject that triggered the trigger is ENEMY_MELEE_TRIGGER                       //
        //              Again check if the player has taken damage recently                                         //
        //                  Reduce the Players HP                                                                   //
        //                  Update the UI                                                                           //
        //                  Play the feedback sound                                                                 //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void OnTriggerEnter(Collider coll)
        {
            if (!STATE_DEAD)
            {
                // If it is spell cast from something
                if (coll.GetComponent<SpellObject>() != null)
                {
                    // If the spell is not from the Player
                    if (!coll.GetComponent<SpellObject>().ReturnFromPlayer())
                    {
                        // If the player has not taken damage
                        if (!_hasTakenDamage)
                        {
                            // If the players health minus the damage from the spell is greater than 0
                            if (_playerHealth - coll.GetComponent<SpellObject>().ReturnDamage() > 0)
                            {
                                if (_hasBarrier)
                                {
                                    _playerHealth -= coll.GetComponent<SpellObject>().ReturnDamage() / 4;
                                }
                                if (!_hasBarrier)
                                {
                                    // Reduce the players health with the damage from the spell
                                    _playerHealth -= coll.GetComponent<SpellObject>().ReturnDamage();
                                }

                                // Update the UI
                                InteractionManager.instance.SetPlayerHealth(_playerHealth);

                                // Display the damage done in the game ( damage number flying up )
                                InteractionManager.instance.DisplayDamageDoneToPlayer((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);

                                // Destroy the spell gameobject
                                Destroy(coll.gameObject);

                                // Play a sound
                                SoundManager.instance.PlaySound(SOUNDS.PLAYERHIT, transform.position, true);

                                // Set _hasTakeDamage = true 
                                _hasTakenDamage = true;

                                // If the player is not in Combat, set the player in combat
                                if (!STATE_INCOMBAT)
                                {
                                    STATE_INCOMBAT = true;
                                    Combat.InitiateCombat();
                                }

                            }

                            // If the players health minus the damage from the spell is less than 0
                            else
                            {
                                PlayerDeath(false);
                                STATE_DEAD = true;

                                for (int i = 0; i < _enemyList.Count; i++)
                                {
                                    _enemyGameObjectList[i].ResetEnemy();
                                }

                            }
                        }
                    }
                }
                if (coll.name == "Enemy_MELEE_TRIGGER" && coll.GetComponentInParent<EnemyCombat.EnemyBehaviour>().ReturnIsAlive())
                {
                    if (!_hasTakenDamage)
                    {
                        if (_playerHealth - (int)coll.GetComponentInParent<EnemyCombat.EnemyBehaviour>().ReturnDamage() > 0)
                        {
                            if (_hasBarrier)
                            {
                                _playerHealth -= (int)coll.GetComponentInParent<EnemyCombat.EnemyBehaviour>().ReturnDamage();
                            }
                            if (!_hasBarrier)
                            {
                                // Reduce the players health with the damage from the spell
                                _playerHealth -= (int)coll.GetComponentInParent<EnemyCombat.EnemyBehaviour>().ReturnDamage();
                            }


                            InteractionManager.instance.SetPlayerHealth(_playerHealth);
                            SoundManager.instance.PlaySound(SOUNDS.PLAYERHIT, transform.position, true);
                            SetPlayerInCombat(true);
                            // Set _takeDamage to true so the player is immune for <seconds>

                            _hasTakenDamage = true;
                        }
                        else
                        {
                            PlayerDeath(false);
                            STATE_DEAD = true;

                            for (int i = 0; i < _enemyList.Count; i++)
                            {
                                _enemyGameObjectList[i].ResetEnemy();
                            }
                        }


                    }
                }
                else if (coll.tag == "InstantDeath")
                {
                    STATE_DEAD = true;
                    PlayerDeath(true);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          LoadPlayerData()                                                //
        //                                                                                                          //
        //  Loads everything from the Database and store it in the local variables                                  //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadPlayerData()
        { 
            CombatDatabase.GetPlayerSettings();

            _playerHealth =         CombatDatabase.ReturnPlayerHealth();
            _playerMana =           CombatDatabase.ReturnPlayerMana();
            _playerRunSpeed =       CombatDatabase.ReturnPlayerRunSpeed();
            _playerWalkSpeed =      CombatDatabase.ReturnPlayerWalkSpeed();
            _playerAttackRange =    CombatDatabase.ReturnPlayerRangedDistance();

            CombatDatabase.GetPlayerStatistics();

            _playerLevel =          CombatDatabase.ReturnPlayerLevel();
            _playerExperience =     CombatDatabase.ReturnPlayerExp();
            _playerGold =           CombatDatabase.ReturnPlayerGold();
            _expMultiplier =        CombatDatabase.ReturnExpMultiplier();
            _healthMultiplier =     CombatDatabase.ReturnHealthMultiplier();
            _manaMultiplier =       CombatDatabase.ReturnManaMultiplier();
            _healingMultiplier =    CombatDatabase.ReturnHealingMultiplier();
            _damageMultiplier =     CombatDatabase.ReturnDamageMultiplier();

            // Set the experience required to level up
            _experienceRequired = _playerLevel * _expMultiplier;

            //InteractionManager.instance.SetPlayerMaxHealth(_playerHealth);
            //InteractionManager.instance.SetPlayerMaxMana(_playerMana);

        }

        public void RespawnPlayer(Vector3 _spawnPos)
        {
            transform.position = _spawnPos;
            STATE_DEAD = false;
            AnimationSystem.PlayerAlive();
            InteractionManager.instance.ShowUI(true);
            _playerHealth = InteractionManager.instance.ReturnPlayerMaxHealth();
            _playerMana = InteractionManager.instance.ReturnPlayerMaxMana();
            _setOnce = false;
            StartCoroutine(CharacterSpawnVFX(false));
        }

        public void PlayerDeath(bool _set)
        {
            INPUT_BLOCK = true;
            InteractionManager.instance.ShowUI(false);
            if(_set)
            {
               AnimationSystem.PlayerInstantDeath();
            }
            else
            {
                AnimationSystem.StopPlayerIdle();
                AnimationSystem.PlayerDeath();
                StartCoroutine(AnimationSystem.StopPlayerDeath());
            }
            SoundManager.instance.PlaySound(SOUNDS.PLAYERDEATH, transform.position, false);
            StartCoroutine(CharacterSpawnVFX(true));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              Immunity()                                                  //
        //                                                                                                          //
        //  If the Player has taken damage                                                                          //
        //      Start the Coroutine ImmunityTimer()                                                                 //
        //          If that has finished                                                                            //
        //              Set _hasTakenDamage to False                                                                //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void Immunity()
        {
            if(_hasTakenDamage)
            {
                StartCoroutine(ImmunityTimer());
                _hasTakenDamage = false;
            }
        }

        void PlayerRegenerate()
        {
            if (!_regen)
            {
                StartCoroutine(Regenerate());
                _regen = true;
            }
        }

        // Return _isCastingSpell
        public bool ReturnIsCastingSpell()
        {
            return _isCastingSpell;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                      CanPlayerCastSpell                                  //
        //                                                                                                          //
        // Can the player Cast a Spell                                                                              //
        // Check if the Players current Mana - the mana cost of a spell is greater than 0                           //
        // If greater than 0 return true                                                                            //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanPlayerCastSpell(float _manaCost)
        {

            if(_playerMana - _manaCost > 0)
            {
                _playerMana -= _manaCost;
                return true;
            }
            else if(_playerMana - _manaCost <= 0)
            {
                Dialogue.DialogueManager.ShowMessage("NOT ENOUGH MANA", true); 
                return false;
            }
            else
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                              SetCasttimeComplete(bool _set)                                              //
        //                                                                                                          //
        //      Set _spellcastComplete to the parsed value _set                                                     //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetCasttimeComplete(bool _set)
        {
            _spellcastComplete = _set;
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          ReturnPlayerHasCastSpell()                                      //
        //                                                                                                          //
        //    Returns _spellcastComplete                                                                            //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ReturnPlayerHasCastSpell()
        {
            return _spellcastComplete;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          PlayerSpellCast()                                               //
        //                                                                                                          //
        //  The Player casting a spell                                                                              //
        //      Call the Combat.CastSpell function - parse the Players current position                             //
        //          Once it is cast, set _isCastingSpell to false                                                   //
        //          Set _spellcastComplete to false                                                                 //
        //      Update the InteractionManager and update the Mana                                                   //
        //      Play the Spellcast sound                                                                            //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PlayerSpellCast()
        {
            StartCoroutine(WaitForSpellAnimation(0.7f));
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                 PlayerHealingSpellCast(float _amount                                     //
        //                                                                                                          //
        //  Call the Combat.CastHealingSpell function and parse the Players current position                        //
        //      Set _isCastingSpell to false                                                                        //
        //      Set _spellcastComplete to false                                                                     //
        //                                                                                                          //
        //  If the current HP plus the amount is greater than the players maximum health                            //
        //      Call the InteractionManager.ReturnPlayerMaxHealth and set the players HP to that value              //
        //                                                                                                          //
        //  Else add the amount to the current HP                                                                   //
        //                                                                                                          //
        //  Call the InteractionManager and update the Player Mana                                                  //
        //      Play the Healing sound                                                                              //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PlayerHealingSpellCast(float _amount)
        {
            Combat.CastHealingSpell(this.transform.position);

            _isCastingSpell = false;
            _spellcastComplete = false;

            if(_playerHealth + _amount > InteractionManager.instance.ReturnPlayerMaxHealth())
            {
                _playerHealth = InteractionManager.instance.ReturnPlayerMaxHealth();
            }
            else
            {
                _playerHealth += _amount;
            }

            

            InteractionManager.instance.SetPlayerMana(_playerMana);
            SoundManager.instance.PlaySound(SOUNDS.HEALING, transform.position, true);
        }
        
        // Return the players GameObject
        public GameObject ReturnPlayerGameObject()
        {
            return this.gameObject;
        }

        public int ReturnPlayerLevel()
        {
            return _playerLevel;
        }

        public bool ReturnPlayerDead()
        {
            return STATE_DEAD;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              IsPlayerFacingEnemy                                         //
        //                                                                                                          //
        // Is the Player facing the selected enemy                                                                  //
        // Perform the DOT on the players current Forward vector                                                    //
        // DOT(Player Forward Vector, Position of the selected enemy - the players position) and normalize it       //
        // If the DOT Product is greater than 0.5f then we are 'facing' the enemy                                   //
        //  Return true                                                                                             //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsPlayerFacingEnemy()
        {
            float dot;
            if(_selectedActor != null)
            {
                dot = Vector3.Dot(transform.forward, (_selectedActor.transform.position - transform.position).normalized);
                
                if(dot > 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  SetPlayerInCombat(bool _set)                                            //
        //                                                                                                          //
        //  Set STATE_INCOMBAT to _set                                                                                 //
        //      Call the InteractionManager and display the Player in Combat                                        //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetPlayerInCombat(bool _set)
        {
            STATE_INCOMBAT = _set;
            
            if(_set)
            {
                STATE_IDLE = false;
                CombatSystem.AnimationSystem.StopPlayerIdle();
                CombatSystem.AnimationSystem.SetCombatIdle();
                InteractionManager.instance.DisplayPlayerInCombat(_set, _playerHealth);
                _setOnce = false;
            }
            if(!_set)
            {
                InteractionManager.instance.DisplayPlayerInCombat(_set, _playerHealth);
                if (!_setOnce)
                {
                    _oocTimer = 0;
                    _setOnce = true;
                }
            }
        }

        public bool ReturnInCombat()
        {
            return STATE_INCOMBAT;
        }

        public void AddEnemyList(int _id, EnemyCombat.EnemyBehaviour _enemy)
        {
            if (!_enemyList.Contains(_id)) { 
                _enemyList.Add(_id);
                _enemyGameObjectList.Add(_enemy);
            }
        }

        public void DeleteEnemyListEntry(int _id)
        {
            _enemyList.Remove(_id);
        }

        public List<int> ReturnEnemyList()
        {
            return _enemyList;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  SetEnemy(GameObject _target)                                            //
        //                                                                                                          //
        //   Set _selectedActor to _target                                                                          //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetEnemy(GameObject _target)
        {
            _selectedActor = _target;
        }

        public void CameraDragging(bool _set)
        {
            _isCameraDragging = _set;
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      AddPlayerHealth(float _amount)                                      //
        //                                                                                                          //
        //  If the current players health + amount is smaller than the maximum health                               //
        //      Add the _amount to the player health                                                                //
        //  else set the player health to the maximum health                                                        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPlayerHealth(float _amount)
        {
            if(_playerHealth + _amount < InteractionManager.instance.ReturnPlayerMaxHealth())
            {
                _playerHealth += _amount;
            }
            else
            {
                _playerHealth = InteractionManager.instance.ReturnPlayerMaxHealth();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      AddPlayerMana(float _amount)                                        //
        //                                                                                                          //
        //  Similar to AddPlayerHealth()                                                                            //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPlayerMana(float _amount)
        {
            if(_playerMana + _amount < InteractionManager.instance.ReturnPlayerMaxMana())
            {
                _playerMana += _amount;
            }
            else
            {
                _playerMana = InteractionManager.instance.ReturnPlayerMaxMana();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          Blink(float _distance)                                          //
        //  If _isBlink is False                                                                                    //
        //      Set _blinkMaxDistance to _distance                                                                  //
        //      Instantiate the BlinkPointer GameObject                                                             //
        //      Invert _isBlink                                                                                     //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Blink(float _distance)
        {
            if(!_isBlink)
            {
                _blinkMaxDistance = _distance;
                _blinkMarker = Instantiate(Resources.Load("Icons/BlinkPointer") as GameObject);
                _isBlink = !_isBlink;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          SetBlinkTarget()                                                //
        //                                                                                                          //
        //  Update the position of the _blinkMarker GameObject to the players position                              //
        //      Create a new Ray and RaycastHit                                                                     //
        //      Get the ParticleSystem of the _blinkMarker GameObject                                               //
        //                                                                                                          //
        //  If the Raycast hit something                                                                            //
        //      If the Distance between the player and the position of the object we hit is less than the max dist  //
        //          Update the blinkMarker position to the new posotion                                             //
        //          Change the Color of the blinkMarker to white ( original colour )                                //
        //          Change the colours of the ParticleSystem                                                        //
        //                                                                                                          //
        //          If the user has left clicked                                                                    //
        //              Instantiate the blink user feedback                                                         //
        //              Play the Particles                                                                          //
        //              Transform the player to the new location ( plus 1 offset on the y axis )                    //
        //              Set _isBlink to false                                                                       //
        //              Play the blink sound                                                                        //
        //              Activate the camera shake                                                                   //
        //              Destroy the blink marker and the particles                                                  //
        //                                                                                                          //
        //      If the Distance between the player and the object position is greater than the max dist             //
        //          Update the position of the marker                                                               //
        //          Change the colour to RED                                                                        //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetBlinkTarget()
        {
            _blinkMarker.transform.position = transform.position;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            ParticleSystem _ps = _blinkMarker.GetComponentInChildren<ParticleSystem>();
            var _pMain = _ps.main;

            if (Physics.Raycast(_ray, out _hit))
            {
                if (Vector3.Distance(transform.position, _hit.point) < _blinkMaxDistance)
                {
                    _blinkMarker.transform.position = new Vector3(_hit.point.x, _hit.point.y + 0.2f, _hit.point.z);
                    _blinkMarker.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    _pMain.startColor = new Color(0.43f, 0.57f, 0.99f, 1);

                    if (Input.GetMouseButtonDown(0))
                    {

                        GameObject _blinkedParticles = Instantiate(Resources.Load("PlayerSpells/Ability/PlayerBlinked_Sparkles"), _hit.point, Quaternion.identity) as GameObject;
                        _blinkedParticles.GetComponent<ParticleSystem>().Play();
                        transform.position = new Vector3(_hit.point.x, _hit.point.y, _hit.point.z);
                        _isBlink = false;

                        SoundManager.instance.PlaySound(SOUNDS.PLAYERBLINK, transform.position, true);
                        
                        CombatSystem.CameraController.CameraShake(2, 0.5f);

                        Destroy(_blinkMarker);
                        Destroy(_blinkedParticles, 1.5f);
                        
                    }

                }
                if (Vector3.Distance(transform.position, _hit.point) > _blinkMaxDistance)
                {
                    _blinkMarker.transform.position = new Vector3(_hit.point.x, _hit.point.y + 0.2f, _hit.point.z);
                    _blinkMarker.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
                    _pMain.startColor = new Color(1, 0, 0, 1);
                }
            }
        }

        public void CreateBarrier(float _time)
        {
            _barrierGameObject = Instantiate(Resources.Load("PlayerSpells/Ability/Barrier_Spell"), transform.position, Quaternion.identity) as GameObject;
            _hasBarrier = true;
            StartCoroutine(BarrierRise(_time));

            SoundManager.instance.PlaySound(SOUNDS.PLAYERBARRIER, transform.position, true);

        }

        public void CastDebuff(float _time, string _prefab, DebuffAbility _debuff)
        {
            if(_selectedActor != null)
            {
                _debuffGameObject = Instantiate(Resources.Load("PlayerSpells/Debuff/" + _prefab), _selectedActor.transform.position, Quaternion.identity) as GameObject;
                _debuffGameObject.AddComponent<DebuffSpell>();
                _debuffGameObject.GetComponent<DebuffSpell>().SetDebuff(_debuff.ToString(), _time, this.gameObject);
                StartCoroutine(DebuffRise(_time));

                SoundManager.instance.PlaySound(SOUNDS.PLAYERICETHRONE, transform.position, true);

            }
            else
            {
                Dialogue.DialogueManager.ShowMessage("No Enemy Selected", true);
            }
        }

        public GameObject ReturnSelectedActor()
        {
            return _selectedActor;
        }

        public bool ReturnPlayerIsInRange()
        {
            if (Vector3.Distance(transform.position, _selectedActor.transform.position) <= _playerAttackRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              CompletedQuest                                              //
        //                                                                                                          //
        //  If the player has finished a quest                                                                      //
        //  If the current exp + the exp reward is less than the exp required                                       //
        //          Simply add the exp reward to the current player exp                                             //
        //                                                                                                          //
        //  if the current exp + the exp reward is greater or equal than the exp required                           //
        //          The new exp is the exp required - current exp - quest exp reward                                //
        //          If the new exp is less than 0 simply invert it                                                  //
        //          Add 1 to the _playerLevel ( current Level of the player )                                       //
        //          Level up to the database to save the data                                                       //
        //          Tell the InteractionManager that there is a level up                                            //
        //                  Visual Feedback ( particles )                                                           //
        //          Tell the SoundSystem that there is a level up                                                   //
        //                  Auditory Feedback                                                                       //
        //                                                                                                          //
        //  Regardless of Level up or not, fill the exp bar                                                         //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CompletedQuest()
        {
            if(_playerExperience + Quest.QuestDatabase.ReturnCurrentQuestExp() < _experienceRequired)
            {
                _playerExperience += Quest.QuestDatabase.ReturnCurrentQuestExp();
            }

            else if (_playerExperience + Quest.QuestDatabase.ReturnCurrentQuestExp() >= _experienceRequired)
            {
                _playerExperience = (_experienceRequired - _playerExperience) - Quest.QuestDatabase.ReturnCurrentQuestExp();

                if(_playerExperience < 0)
                {
                    _playerExperience *= -1;
                    
                }
                _playerLevel++;
                CombatSystem.CombatDatabase.PlayerLevelUp(_playerLevel, (int) _playerExperience);

                _experienceRequired *= _expMultiplier;

                InteractionManager.instance.LevelUp(transform.position, this.gameObject);
                SoundManager.instance.PlaySound(SOUNDS.LEVELUP, transform.position, true);

            }

            InteractionManager.instance.FillExperienceBar();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                       ReturnPlayerPosition()                                             //
        //                                                                                                          //
        //  Return the player position ( Vector 3 )                                                                 //
        //      For example for the VFX of spells                                                                   //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 ReturnPlayerPosition()
        {
            return transform.position;
        }

        public void HoverOverUI(bool _set)
        {
            _isOverUI = _set;
        }

        IEnumerator WaitForSpellAnimation(float _time)
        {
            yield return new WaitForSeconds(_time);
            Combat.CastSpell(this.transform.position);
            _isCastingSpell = false;
            _spellcastComplete = false;

            InteractionManager.instance.SetPlayerMana(_playerMana);
            SoundManager.instance.PlaySound(SOUNDS.PLAYERSPELLCAST, transform.position, true);
            
        }

        IEnumerator ImmunityTimer()
        {
            yield return new WaitForSeconds(2);

        }

        IEnumerator Regenerate()
        {
            yield return new WaitForSeconds(2);
            if (_playerHealth < CombatSystem.InteractionManager.instance.ReturnPlayerMaxHealth())
            {
                _playerHealth += CombatSystem.InteractionManager.instance.ReturnPlayerMaxHealth() / 100;
                CombatSystem.InteractionManager.instance.SetPlayerHealth(_playerHealth);
            }
            if (_playerMana < CombatSystem.InteractionManager.instance.ReturnPlayerMaxMana())
            {
                _playerMana += CombatSystem.InteractionManager.instance.ReturnPlayerMaxMana() / 100;
                CombatSystem.InteractionManager.instance.SetPlayerMana(_playerMana);
            }
            _regen = false;
        }

        IEnumerator CharacterSpawnVFX(bool _despawn)
        {
            
            Renderer[] _allRenders = this.GetComponentsInChildren<Renderer>();
            if (_despawn)
            {
                float _timer = 1f;
                while (true)
                {
                    yield return new WaitForEndOfFrame();
                    _timer -= Time.deltaTime;

                    for (int i = 0; i < _allRenders.Length; i++)
                    {
                        _allRenders[i].material.color = new Color(1, 1, 1, _timer);
                    }

                    if (_timer <= 0)
                    {

                        yield break;
                    }
                }
            }
            else
            {
                float _timer = 0f;
                yield return new WaitForSeconds(1);
                while (true)
                {
                    
                    yield return new WaitForEndOfFrame();
                    _timer += Time.deltaTime / 2;

                    for (int i = 0; i < _allRenders.Length; i++)
                    {
                        _allRenders[i].material.color = new Color(1, 1, 1, _timer);
                    }

                    if (_timer >= 1)
                    {

                        yield break;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          Barrier IENumerators                                            //
        //                                                                                                          //
        //  When the barrier spell is cast it starts the coroutine BarrierRise with the duration time               //
        //                                                                                                          //
        //  In 1 second the barrier dissolve goes from 1 ( fully dissolved ) to 0 to create a growing effect        //
        //      We set the float _SliceAmount defined in the shader using the _timer ( 1 - Time.deltaTime )         //
        //          If the timer is ~0                                                                              //
        //              We wait for the duration time                                                               //
        //              Play the Barrier dissolve sound                                                             //
        //                  Start the coroutine BarrierDecay which is the same but inverted                         //
        //                      Break the coroutine ( stop it )                                                     //
        //                  The 'inverted coroutine' does the same but when the _timer > 1 we destroy the object    //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IEnumerator BarrierRise(float _waitTime)
        {
            float _timer = 1f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer -= Time.deltaTime;
                _barrierGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer <= 0)
                {
                    yield return new WaitForSeconds(_waitTime);
                    SoundManager.instance.PlaySound(SOUNDS.PLAYERBARRIERDECAY, transform.position, true);
                    StartCoroutine(BarrierDecay());
                    yield break;
                }
            }
        }

        IEnumerator BarrierDecay()
        {
            float _timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;
                if (_barrierGameObject != null)
                {
                    _barrierGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);
                }
                if (_timer >= 1)
                {
                    Destroy(_barrierGameObject);
                    _hasBarrier = false;
                    yield break;
                }
            }
        }

        IEnumerator DebuffRise(float _waitTime)
        {
            float _timer = 1f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer -= Time.deltaTime;
                _debuffGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer <= 0)
                {
                    yield return new WaitForSeconds(_waitTime);
                    SoundManager.instance.PlaySound(SOUNDS.PLAYERBARRIERDECAY, transform.position, true);
                    StartCoroutine(DebuffDecay());
                    yield break;
                }
            }
        }

        IEnumerator DebuffDecay()
        {
            float _timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;
                _debuffGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer >= 1)
                {
                    Destroy(_debuffGameObject);
                    yield break;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                              Conversation Mode                                           //
        //                                                                                                          //
        //  When talkin to an NPC we go in to Conversation Mode                                                     //
        //  Every NPC has his own camera which will be turned on to create a more storytelling POV                  //
        //                                                                                                          //
        //      SwitchOnPlayerCamera(bool _set)                                                                     //
        //          Turn off the Player Camera                                                                      //
        //                                                                                                          //
        //      SetInputBlock(bool _set)                                                                            //
        //          Turn off the Player Input so the player cant accidentally move the player character             //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SwitchOnPlayerCamera(bool _set)
        {
            GetComponentInChildren<Camera>().enabled = _set;

        }
        
        public void SetInputBlock(bool set)
        {
            INPUT_BLOCK = set;
        }

        public void SetHandSpellCastinVFX(bool _set)
        {
            if(_set)
            {
                GetComponentInChildren<ParticleSystem>().Play();
            }
            if(!_set)
            {
                if(GetComponentInChildren<ParticleSystem>().isPlaying)
                {
                    GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
        }



    }
}
