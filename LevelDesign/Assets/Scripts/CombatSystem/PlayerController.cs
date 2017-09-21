using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem
{
    public class PlayerController : MonoBehaviour
    {
        // Booleans
        private bool _isWalking         = false;
        private bool _isRunning         = false;
        private bool _isIdle            = false;
        private bool _isInCombat        = false;
        private bool _isJumping         = false;
        private bool _hasTakenDamage    = false;
        private bool _isCastingSpell    = false;
        private bool _spellcastComplete = false;
        private bool _isCameraDragging  = false;
        private bool _isLevelUp         = false;
        private bool _isBlink           = false;
        private bool _isOverUI          = false;
        private bool _isFirstPerson     = false;

        // Movement Floats
        private float _runSpeed = 0.3f;
        private float _walkSpeed = 0.15f;
        private float _rotateAngle = 0f;
        private float _rotatedAngle = 0f;
        private float _playerDistanceTraveled = 0.0f;
        private float _blinkMaxDistance;

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

        private Ray _ray;

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
        }

        // Use this for initialization
        void Start()
        {
            _charController = this.GetComponent<CharacterController>();
            CombatSystem.AnimationSystem.SetController(this.GetComponentInChildren<Animator>());

            InteractionManager.instance.SetPlayerMaxHealth(_playerHealth);
            InteractionManager.instance.SetPlayerMaxMana(_playerMana);

            if(CombatSystem.CameraController.ReturnFirstPerson())
            {
                foreach (SkinnedMeshRenderer skin in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    skin.enabled = false;
                }
                
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            CheckInputs();
            CheckMouseOver();
           
            //////////////////////////////////////////////////////////////////////////
            //                                                                      //
            //                          STATE MACHINE                               //
            //                                                                      //
            //////////////////////////////////////////////////////////////////////////

            if (_isIdle)
            {
                PlayerIdle();
            }
            if (_isWalking)
            {
                PlayerWalking();
            }
            if (_isRunning)
            {
                PlayerRunning();
            }
            if (!_isRunning || !_isWalking || _isInCombat)
            {
                _isIdle = true;
            }

            Immunity();

            if(_isBlink)
            {
                SetBlinkTarget();
            }
        }
        #region MOVEMENT

        //////////////////////////////////////////////////////////////////////////
        //                                                                      //
        //                              CHECK INPUTS                            //
        //                                                                      //
        //  If the player is grounded                                           //
        //  Check if the player pressed WASD, if so change the _moveDirection   //
        //                                                                      //
        //  If the player is not grounded                                       //
        //      Apply gravity                                                   //
        //                                                                      //
        //////////////////////////////////////////////////////////////////////////

        void CheckInputs()
        {            
            if (_charController.isGrounded)
            {
                _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _moveDirection *= _walkSpeed;
                }
                else
                {
                    _moveDirection *= _runSpeed;
                }
                if (_moveDirection.z != 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _isWalking = true;
                        _isRunning = false;
                    }
                    else
                    {
                        _isRunning = true;
                        _isWalking = false;
                    }
                }
                else
                {
                    _isRunning = false;
                    _isWalking = false;
                }

                if(Input.GetKey(KeyCode.Space))
                {
                    _moveDirection.y = 0.6f;
                }
                _moveDirection = transform.TransformDirection(_moveDirection);
            }
            else
            {
                _moveDirection.y -= 1 * Time.deltaTime;
                _charController.Move(_moveDirection * Time.deltaTime);
    
            }
        }

        // If the Player is idle, stop playing the running and/or walking animations
        void PlayerIdle()
        {
            CombatSystem.AnimationSystem.StopPlayerRunning();
            CombatSystem.AnimationSystem.StopPlayerWalking();

            // Start the Idle animation
            CombatSystem.AnimationSystem.SetPlayerIdle();
        } 

        // If the player is walking, move the player and play the walking animation
        void PlayerWalking()
        {
            Vector3 _oldPosition = this.transform.position;
            _charController.Move(_moveDirection * _walkSpeed);
            CombatSystem.AnimationSystem.SetPlayerWalking();

            _playerDistanceTraveled += (transform.position - _oldPosition).magnitude;

            if(_playerDistanceTraveled > 0.5f)
            {
                if (_charController.isGrounded)
                {
                    _playerDistanceTraveled = 0.0f;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //                                  PlayerRunning()                                         //
        // If the player is running, move the player and play the running animation                 //
        //  Check the distance the player has traveled, if it is greater than 1.75f                 //
        //      1.75f is an estimation of the distance between footsteps                            //
        //  Call the SoundSystem and play the Footstep sounds                                       //
        //                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////

        void PlayerRunning()
        {
            Vector3 _oldPosition = this.transform.position;
            _charController.Move(_moveDirection * _runSpeed);
            CombatSystem.AnimationSystem.SetPlayerRunning();

            _playerDistanceTraveled += (transform.position - _oldPosition).magnitude;

            if(_playerDistanceTraveled > 1.75f)
            {
                if (_charController.isGrounded)
                {
                    _playerDistanceTraveled = 0.0f;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //                                       RotatePlayer()                                     //
        //                                                                                          //
        //  Rotate the player based on the amount parsed to the function                            //
        //  _rotateAngel + the amount                                                               //
        //  Convert a new Vector3 to Quaternion, transform.rotation uses Quaternions                //
        //  Rotate the player character                                                             //  
        //                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////

        public void RotatePlayer(float _amount)
        {
            
            _rotateAngle += _amount;
            // We use Euler in this case, dirty but works
            transform.rotation = Quaternion.Euler(new Vector3(0, _rotateAngle, 0));
            //transform.rotation = Quaternion.Slerp(transform.rotation, turnAngle, Time.deltaTime * 100f);
        }

        #endregion
               

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
        
            if (!_isOverUI) {

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
                        // Set the NPC cursor
                        InteractionManager.instance.SetNpcCursor();

                        // Highlight the specific NPC ( _hit.collider.gameObject )
                        _hit.collider.gameObject.GetComponent<NPC.NpcSystem>().HighlightNPC(true, _hit.collider.gameObject);
                        _selectedNPC = _hit.collider.gameObject;

                    }

                    // If the mouse is over an Enemy
                    if (_hit.collider.tag == "EnemyRanged" || _hit.collider.tag == "EnemyMelee")
                    {
                        // Set the Combat cursor
                        InteractionManager.instance.SetCombatCursor();
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

                    // If we pressed the Right Mouse Button
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (!_isCameraDragging)
                        {
                            if (_hit.collider.tag == "NPC")
                            {
                                if(_previousNPC != null)
                                {
                                    _previousNPC.GetComponent<NPC.NpcSystem>().IsSelected(false);
                                    _previousNPC = null;
                                }
                                _selectedNPC = _hit.collider.gameObject;

                                // Set the NPC selected to "Selected" in its NPC Class
                                _selectedNPC.GetComponentInChildren<NPC.NpcSystem>().IsSelected(true);
                                _selectedNPC.gameObject.GetComponentInChildren<NPC.NpcSystem>().SetInteraction(true);

                                InteractionManager.instance.SetSelected(_selectedNPC);

                                _previousNPC = _selectedNPC;

                            }
                            if (_hit.collider.tag == "EnemyMelee")
                            {
                                _selectedActor = _hit.collider.gameObject;
                                InteractionManager.instance.SetSelected(_selectedActor);
                                _selectedActor.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetSelected(true);
                            }
                        }
                    }

                    if (Input.GetMouseButtonDown(0))
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
            // If it is spell cast from something
            if(coll.GetComponent<SpellObject>() != null)
            {
                // If the spell is not from the Player
                if(!coll.GetComponent<SpellObject>().ReturnFromPlayer())
                {
                    // If the player has not taken damage
                    if(!_hasTakenDamage)
                    {
                        // If the players health minus the damage from the spell is greater than 0
                        if(_playerHealth - coll.GetComponent<SpellObject>().ReturnDamage() > 0)
                        {
                            // Reduce the players health with the damage from the spell
                            _playerHealth -= coll.GetComponent<SpellObject>().ReturnDamage();

                            // Update the UI
                            InteractionManager.instance.SetPlayerHealth(_playerHealth);

                            // Display the damage done in the game ( damage number flying up )
                            InteractionManager.instance.DisplayDamageDoneToPlayer((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);

                            // Destroy the spell gameobject
                            Destroy(coll.gameObject);

  

                            // Set _hasTakeDamage = true 
                            _hasTakenDamage = true;

                            // If the player is not in Combat, set the player in combat
                            if(!_isInCombat)
                            {
                                _isInCombat = true;
                                Combat.InitiateCombat();
                            }

                        }

                        // If the players health minus the damage from the spell is less than 0
                        else
                        {
                            PlayerDeath();
                        }
                    }
                }
            }
            if (coll.name == "Enemy_MELEE_TRIGGER")
            {
                if (!_hasTakenDamage)
                {
                    _playerHealth -= (int)coll.GetComponentInParent<EnemyCombat.EnemyCombatSystem>().ReturnEnemyDamage();
                    InteractionManager.instance.SetPlayerHealth(_playerHealth);

                    // Set _takeDamage to true so the player is immune for <seconds>

                    _hasTakenDamage = true;


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

        void PlayerDeath()
        {

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
            Combat.CastSpell(this.transform.position);
            _isCastingSpell = false;
            _spellcastComplete = false;

            InteractionManager.instance.SetPlayerMana(_playerMana);

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
        }
        
        // Return the players GameObject
        public GameObject ReturnPlayerGameObject()
        {
            return this.gameObject;
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
        //  Set _isInCombat to _set                                                                                 //
        //      Call the InteractionManager and display the Player in Combat                                        //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetPlayerInCombat(bool _set)
        {
            _isInCombat = _set;
            InteractionManager.instance.DisplayPlayerInCombat(_set, _playerHealth);
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

        IEnumerator ImmunityTimer()
        {
            yield return new WaitForSeconds(2);
            
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
                        transform.position = new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z);
                        _isBlink = false;
                        
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

        //////////////////////////////////////////////////////////////////////////////////////
        //                                      CompletedQuest                              //
        //                                                                                  //
        //  If the player has finished a quest                                              //
        //  If the current exp + the exp reward is less than the exp required               //
        //          Simply add the exp reward to the current player exp                     //
        //                                                                                  //
        //  if the current exp + the exp reward is greater or equal than the exp required   //
        //          The new exp is the exp required - current exp - quest exp reward        //
        //          If the new exp is less than 0 simply invert it                          //
        //          Add 1 to the _playerLevel ( current Level of the player )               //
        //          Level up to the database to save the data                               //
        //          Tell the InteractionManager that there is a level up                    //
        //                  Visual Feedback ( particles )                                   //
        //          Tell the SoundSystem that there is a level up                           //
        //                  Auditory Feedback                                               //
        //                                                                                  //
        //  Regardless of Level up or not, fill the exp bar                                 //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

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

            }

            InteractionManager.instance.FillExperienceBar();
        }

        public Vector3 ReturnPlayerPosition()
        {
            return transform.position;
        }
        
        public Quaternion ReturnRotation()
        {
            return transform.localRotation;
        }

        public void HoverOverUI(bool _set)
        {
            _isOverUI = _set;
        }
        
    }
}
