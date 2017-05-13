using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;
using System.Data;
using System;
using Mono.Data.Sqlite;


namespace CombatSystem
{

    public class PlayerMovement : MonoBehaviour
    {

        private Animator _playerAnimator;

        private Vector3 _targetPosition;
        private GameObject _targetObject;
        private Vector3 _oldPosition;

        private float _playerMoveSpeed;
        private float _distanceTraveled;
        private float _rangedAttackDistance;
        private float _meleeAttackDistance;

        private static bool _isMoving = false;
        private bool _moveToAttack = false;
        private bool _moveToNPC = false;
        private static bool _isKnockedBack = false;

        private GameObject _selectedActor;
        private GameObject _posClicked;
        private GameObject _clickMoveIcon;

        private static bool _hovingOverUI = false;
        private static bool _draggingUI = false;

        private static bool _hoveringOverSpellbar = false;
        private static bool _draggingSpellbar = false;

        private static bool _hoverOverInventory = false;
        private static bool _draggingInventory = false;

        private static bool _hoverOverQuestLog = false;
        private static bool _draggingQuestLog = false;
        

        private static int _playerHealth;
        private static int _playerMana;
        private static int _playerLevel;
        private static int _playerGold;
        private static int _playerExp;

        private static int _expRequired;

        private static int _expMultiplier;
        private float _dmgMultiplier;
        private float _healthMultiplier;
        private float _manaMultiplier;
        private float _healingMultiplier;

        private CharacterController _charController;

        private static bool _castSpell = false;
        private static bool _castInstant = false;
        private static bool _mayCastSpell = false;
        private static bool _castSpellOnce = false;
        private static bool _isInCombat = false;
        private static bool _castHealing = false;

        private static int _healingSpellAmount;

        private static bool _levelUp = false;
        

        private static bool _takenDamage = false;
        private static float _immunity = 2.0f;
        private static float _damageTimer = 0.0f;

        private static bool _mayCastAOE = false;
        private static bool _SetAOE = false;
        private static GameObject _aoeTarget;

        private static bool _isBlink = false;
        private static float _blinkDistance;
        private static GameObject _blinkObject;
        private static bool _playerBlink = false;

        private static bool _playerIsBlinking = false;

        private static float _timer = 0f;
        private static float _spellTimer;

        private static float _disengageDistance;
        private static bool _setDisengageTarget;
        private Vector3 _disengageTarget;

        private static GameObject _playerGameObject;


        void OnEnable()
        {
            _charController = GetComponent<CharacterController>();
            _playerAnimator = GetComponentInChildren<Animator>();

            // Set the AnimatorController is the AnimationSystem
            CombatSystem.AnimationSystem.SetController(_playerAnimator);

            // Set the CastBar GUI to false ( don't show it! )
            

            // Load the move Icon from the folder
            _clickMoveIcon = Resources.Load("Icons/ClickedToMoveTo") as GameObject;


            // Get the Player settings like Health/Mana from the Database
            CombatDatabase.GetPlayerSettings();

            // Set the Player Stats
            _playerHealth = CombatDatabase.ReturnPlayerHealth();
            _playerMana = CombatDatabase.ReturnPlayerMana();
            _playerMoveSpeed = CombatDatabase.ReturnPlayerRunSpeed();
            _rangedAttackDistance = CombatDatabase.ReturnPlayerRangedDistance();
            _meleeAttackDistance = CombatDatabase.ReturnPlayerMeleeRange();

            GameInteraction.SetPlayerMaxHealth(_playerHealth);
            GameInteraction.SetPlayerMaxMana(_playerMana);


        }

        // Use this for initialization
        void Start()
        {
            _playerGameObject = this.gameObject;
            CombatSystem.GameInteraction.DisplayCastBar(false);
            CombatDatabase.GetPlayerStatistics();

            _playerLevel = CombatDatabase.ReturnPlayerLevel();
            _playerExp = CombatDatabase.ReturnPlayerExp();
            _playerGold = CombatDatabase.ReturnPlayerGold();
            _expMultiplier = CombatDatabase.ReturnExpMultiplier();
            _dmgMultiplier = CombatDatabase.ReturnDamageMultiplier();
            _healthMultiplier = CombatDatabase.ReturnHealthMultiplier();
            _manaMultiplier = CombatDatabase.ReturnManaMultiplier();
            _healingMultiplier = CombatDatabase.ReturnHealingMultiplier();

            _expRequired = _playerLevel * _expMultiplier;
            
        }

        void Update()
        {

        }

        // We use FixedUpdate to prevent stuttering in the Player Animation
        void FixedUpdate()
        {


            #region SetLocations

            // If we are not dragging the UI - to prevent setting a movement target while dragging
            if (!_hovingOverUI && !_hoveringOverSpellbar && !_hoverOverInventory)
            {
                
                // Same as DraggingUI - to prevent weird stuff
                if (!_draggingUI)
                {
                    if (!_playerIsBlinking)
                    {
                        if (Input.GetMouseButtonDown(0) && !_hovingOverUI && !_draggingUI)
                        {

                            SetTargetPosition();

                            // If the Player is not in combat we set the SetSelectedUI to nothing 
                            // this removes the Selected HUD
                            if (!CombatSystem.Combat.ReturnInCombat())
                            {
                                CombatSystem.GameInteraction.SetSelectedUI(null);
                            }

                            _moveToAttack = false;
                            if (!_isBlink)
                            {
                                AddLocationMarker();
                            }

                        }
                    }
                    // If we Right Clicked with the mouse
                    if (Input.GetMouseButtonDown(1))
                    {
                        _isMoving = false;

                        _isBlink = false;
                        _playerBlink = false;

                        _mayCastAOE = false;

                        // Cancelling the Blink ability and destroying the _blinkObject
                        if (_blinkObject != null)
                        {
                            Destroy(_blinkObject);
                        }

                        if (_SetAOE)
                        {
                            // Cancelling AOE and destroy the _aoeTarget
                            if (_aoeTarget.gameObject != null)
                            {
                                Destroy(_aoeTarget.gameObject);
                            }
                        }
                        
                        // Check the Actor - Have we right clicked on an NPC or an Enemy
                        CheckActor();
                        CombatSystem.GameInteraction.SetSelectedUI(_selectedActor);
                     
                    }
                }
            }
            #endregion


            #region MovementBooleans

            // If we are Moving ( walking )
            if (_isMoving)
            {
                // Call the MoveToPosition function
                MoveToPosition(_targetPosition);

                // Check if the Player is playing the running animation, if not -  Play the Running Animation
                if (!AnimationSystem.ReturnRunningAnim())
                {    
                    AnimationSystem.SetPlayerRunning();
                }
            }
            
            // Stop the Running Animation if we are not moving and not moving to attack     
            if(!_isMoving && !_moveToAttack)
            {
                AnimationSystem.StopPlayerRunning();
            }
            
            // If we are Moving to Attack an Enemy      
            if (_moveToAttack)
            {
                
                _targetPosition = _targetObject.transform.position;
                
                MoveToEnemy(_targetPosition);
                
                // Stop the combat idle and set the player to running
                if (!AnimationSystem.ReturnRunningAnim())
                {
                    AnimationSystem.StopCombatIdle();

                    AnimationSystem.SetPlayerRunning();
                }
            }
                        
            #endregion



            #region SpellCast
            
            // If we are casting a spell and NOT moving
            if (_castSpell && !_moveToAttack)
            {
                RotatePlayerToEnemy();

                // Display the cast bar
                GameInteraction.DisplayCastBar(true);
                if (!_castSpellOnce)
                {
                    AnimationSystem.StopCombatIdle();
                    AnimationSystem.SetSkipIdle(true);
                 //   AnimationSystem.SetRangedSpell(_spellTimer);

                    _castSpellOnce = true;
                    

                }
               

                // If the player may cast a spell
                if (_mayCastSpell && _castSpell && !_castHealing)
                {

                   if (!_playerBlink)
                   {
                       // Cast the actual Spell
                       Combat.CastSpell(this.transform.position);
                       SoundSystem.PlaySpellCast(this.transform.position);
                   }

                   // Call the GameInteraction Class - SpellHasBeenCast()
                   // This is to trigger the COOlDOWN
                   GameInteraction.SpellHasBeenCast();
                   _mayCastSpell = false;

                   // Set bool _castSpell to false
                   _castSpell = false;

                   // Turn off the DisplayCastBar HUD
                   GameInteraction.DisplayCastBar(false);


                }

                if(_castHealing)
                {
                    if(_mayCastSpell && _castSpell)
                    {
                        Combat.CastHealingSpell(this.transform.position);

                        if (_playerHealth < GameInteraction.ReturnPlayerMaxHealth())
                        {
                            // Actual healing for now
                            _playerHealth += _healingSpellAmount;

                            if(_playerHealth > GameInteraction.ReturnPlayerMaxHealth())
                            {
                                _playerHealth = GameInteraction.ReturnPlayerMaxHealth();
                            }
                            
                            GameInteraction.SetPlayerHealth(_playerHealth);
                            
                        }


                        SoundSystem.Healing(this.transform.position);
                        GameInteraction.SpellHasBeenCast();
                        GameInteraction.DisplayCastBar(false);
                        _mayCastSpell = false;
                        _castSpell = false;
                        _castHealing = false;

                        
                    }
                }
            }

           

            //////////////////////////////////////////////////////////////////////
            //                  CANCEL ONE TIME ANIMATIONS                      //
            //////////////////////////////////////////////////////////////////////

            if (!_castSpell && _isInCombat)
            {
                if (AnimationSystem.RangedSpellFinished())
                {
                    AnimationSystem.StopRangedSpell();
                    AnimationSystem.SetSkipIdle(true);
                    AnimationSystem.SetCombatIdle();
                    Debug.Log("set combat idle");
                    _castSpellOnce = false;
                }
            }
            
            if(!_castSpell && _castSpellOnce)
            {
                AnimationSystem.StopRangedSpell();
                AnimationSystem.SetPlayerIdle();
                _castSpellOnce = false;
            }
            
            // If we are casting a spell and moving display a message
            if (_castSpell && _moveToAttack)
            {
                
                Dialogue.DialogueManager.ShowMessage("You can not cast while moving", true);
            }
            #endregion

            if (_isKnockedBack)
            {
                PlayerThrow();
                
            }

            if(_mayCastAOE)
            {
                SetAoeTarget(transform.position);
                
            }

            if(_isBlink)
            {
                PlayerBlink();
            }

            ImmunityTimer();
            CheckGround(transform.position);

            //  RegenerateHealth();
            //  RegenerateMana();

            CheckMouseOver();

            if(_levelUp)
            {
                GameInteraction.LevelUp(transform.position, this.gameObject);
                SoundSystem.LevelUp(transform.position);
                _levelUp = false;
            }

        }

        
        void SetTargetPosition()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit))
            {
                _targetPosition = _hit.point;
                _isMoving = true;
            }
        }

        void MoveToPosition(Vector3 _targetPos)
        {
            if (_targetPos != null)
            {

                _oldPosition = transform.position;
                Vector3 _dir = _targetPos - transform.position;                                               // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player

                Vector3 _forward = transform.TransformDirection(Vector3.forward);                           // create a forward Vector3

                if (_dir.magnitude > 0.5f)
                {                                                                                            // if the magnitude of the vector is greater than
                    _charController.SimpleMove(_forward * _playerMoveSpeed);                                // move the actual player

                    //////////////////////////////////////////////////////////////////////
                    //                          footstep sounds                         //
                    //////////////////////////////////////////////////////////////////////

                    _distanceTraveled += (transform.position - _oldPosition).magnitude;

                    // Check if the distance traveled is greater than 2
                    // In this case 2f is the distance between each step! ( FOR RUNNING )

                    if (_distanceTraveled >= 2f)
                    {
                        // Call the PlayFootstepSound in the SoundSystem Class
                        SoundSystem.PlayFootSteps(this.transform.position);
                        _distanceTraveled = 0.0f;
                    }
               
                    if(_isInCombat)
                    {
                        CombatSystem.AnimationSystem.StopCombatIdle();
                    }
                    

                }
                else
                {
                    _isMoving = false;
                    if(_isInCombat)
                    {
                        CombatSystem.AnimationSystem.SetCombatIdle();

                    }
                    
                }

                if (transform.position == _targetPos)                                                  // if we have reached our destination
                {
                    _isMoving = false;                                                              // stop moving
                }

            }
        }

        void MoveToEnemy(Vector3 _targetPos)
        {
            if (_targetPos != null)
            {

                _oldPosition = transform.position;
                Vector3 _dir = _targetPos - transform.position;                                               // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player

                Vector3 _forward = transform.TransformDirection(Vector3.forward);                           // create a forward Vector3

                if (Vector3.Distance(transform.position, _targetPos) > _rangedAttackDistance)
                {                                                                                            // if the magnitude of the vector is greater than
                    _charController.SimpleMove(_forward * _playerMoveSpeed);                                // move the actual player

                    //////////////////////////////////////////////////////////////////////
                    //                          footstep sounds                         //
                    //////////////////////////////////////////////////////////////////////

                    _distanceTraveled += (transform.position - _oldPosition).magnitude;

                    // Check if the distance traveled is greater than 2
                    // In this case 2f is the distance between each step! ( FOR RUNNING )

                    if (_distanceTraveled >= 2f)
                    {
                        // Call the PlayFootstepSound in the SoundSystem Class
                        SoundSystem.PlayFootSteps(this.transform.position);
                        _distanceTraveled = 0.0f;
                    }
                }
                else
                {
                    CombatSystem.Combat.InitiateCombat();
                    _moveToAttack = false;
                }

            }
        }

        void CheckActor()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit))
            {

                if (_hit.collider.gameObject.tag == "NPC")
                {
                    _targetPosition = _hit.point;
                    _hit.collider.gameObject.GetComponentInChildren<NPCSystem.NPC>().IsSelected(true);
                    _isMoving = true;
                }
                if (_hit.collider.gameObject.tag == "EnemyRanged" || _hit.collider.gameObject.tag == "EnemyMelee")
                {

                    _targetPosition = _hit.point;
                    _moveToAttack = true;

                    _targetObject = _hit.collider.gameObject;
                    

                }
                _selectedActor = _hit.collider.gameObject;
            }
        }
        
        public static void HoveringOverUI(bool _set)
        {
            _hovingOverUI = _set;

            
        }

        public static void HoveringOverSpellbar(bool _set)
        {
            _hoveringOverSpellbar = _set;

        }

        public static void HoveringOverInventory(bool _set)
        {
            _hoverOverInventory = _set;
        }

        public static void HoveringOverQuestLog(bool _set)
        {
            _hoverOverQuestLog = _set;
        }

        public static void SetDraggingUI(bool _set)
        {
            _draggingUI = _set;
        }

        public static void SetDraggingQuestLog(bool _set)
        {
            _draggingQuestLog = _set;
        }
        
        public static void CompletedQuest()
        {
            if (_playerExp + Quest.QuestDatabase.ReturnCurrentQuestExp() < _expRequired) {
                _playerExp += Quest.QuestDatabase.ReturnCurrentQuestExp();

                
            }
            if(_playerExp + Quest.QuestDatabase.ReturnCurrentQuestExp() >= _expRequired)
            {
                
                _playerExp = ((_expRequired - _playerExp) - Quest.QuestDatabase.ReturnCurrentQuestExp());
                if(_playerExp < 0)
                {
                    _playerExp = _playerExp * -1;
                }
                _playerLevel++;
                CombatDatabase.PlayerLevelUp(_playerLevel, _playerExp);

                _expRequired = _playerLevel * _expMultiplier;

                _levelUp = true;
            }
            GameInteraction.FillExpBar();
        }

        private Vector3 ReturnPlayerPosition()
        {
            return transform.position;
        }

        public void PlayerInCombat(bool _set)
        {
                        

            if (_set)
            {
                CombatSystem.Combat.InitiateCombat();
                CombatSystem.GameInteraction.DisplayPlayerInCombat(true, _playerHealth);
                CombatSystem.AnimationSystem.SetCombatIdle();
                _isInCombat = true;
                
            }

            else
            {
                CombatSystem.GameInteraction.DisplayPlayerInCombat(false, _playerHealth);
                CombatSystem.AnimationSystem.StopCombatIdle();
                _isInCombat = false;

                Debug.Log("Playermovement: PlayerInCombat " + _isInCombat);
            }
        }

        public static int ReturnPlayerHealth()
        {
            return _playerHealth;
        }

        public static int ReturnPlayerMana()
        {
            return _playerMana;
        }

        public static void SetCastSpell(bool _set)
        {
            _castSpell = _set;
        }

        public static void SetMayCastSpell(bool _set)
        {
            _mayCastSpell = _set;
        }

        public static void CastSpell(float _casttime, GameObject _target, float _manaCost)
        {
            if (_target != null)
            {
                if (_target.tag == "EnemyRanged" || _target.tag == "EnemyMelee")
                {
                    if (_playerMana - _manaCost > 0)
                    {
                        _castSpell = true;
                        _playerMana -= (int)_manaCost;
                        GameInteraction.SetPlayerMana(_playerMana);

                    }
                    else
                    {
                        _castSpell = false;
                        Dialogue.DialogueManager.ShowMessage("Not enough mana", true);
                    }
                }
            }
        }

        public static void CastAOE(float _casttime, float _manaCost)
        {
            if (_SetAOE)
            {
                if (_playerMana - _manaCost > 0)
                {
                    _mayCastAOE = true;
                    _playerMana -= (int)_manaCost;
                    GameInteraction.SetPlayerMana(_playerMana);
                    _aoeTarget = Instantiate(Resources.Load("Icons/AOE") as GameObject);
                }
                else
                {
                    _mayCastAOE = false;
                    Dialogue.DialogueManager.ShowMessage("Not enough mana", true);
                }
            }
        }

        public static bool ReturnCastSpell()
        {
            return _castSpell;
        }

        public static bool ReturnCastAOE()
        {
            return _mayCastAOE;
        }


        public static void SetAoeTarget(Vector3 _playerPos)
        {
            if (_SetAOE)
            {
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit))
                {
                    if (Vector3.Distance(_playerPos, _hit.point) < 10f)
                    {
                        _aoeTarget.GetComponent<SpriteRenderer>().color = Color.green;
                        _aoeTarget.transform.position = new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z);

                        if (Input.GetMouseButtonDown(0))
                        {
                            _isMoving = false;
                            Combat.CastAOE(_hit.point);

                            GameInteraction.ResetAoECooldown();

                        }
                    }
                    else
                    {
                        _aoeTarget.transform.position = new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z);
                        _aoeTarget.GetComponent<SpriteRenderer>().color = Color.red;
                        if (Input.GetMouseButtonDown(0))
                        {
                            _isMoving = false;
                            Dialogue.DialogueManager.ShowMessage("Out of range", true);
                        }
                    }
                }
            }
        }

        public static void ToggleAoE()
        {
            _SetAOE = !_SetAOE;

            if (!_SetAOE)
            {
                Destroy(_aoeTarget.gameObject);
            }
        }

        public static void Blink(float _distance)
        {
            _isBlink = !_isBlink;
            _playerBlink = true;
            _blinkDistance = _distance;
            if (_isBlink)
            {
                _blinkObject = Instantiate(Resources.Load("Icons/BlinkPointer")  as GameObject);
                
            }
            else
            {
                if(_blinkObject != null)
                {
                    Destroy(_blinkObject);
                }
            }

            
        }

        void PlayerBlink()
        {
            _blinkObject.transform.position = transform.position;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            ParticleSystem _ps = _blinkObject.GetComponentInChildren<ParticleSystem> ();
            var _pMain = _ps.main;

            if(Physics.Raycast(_ray, out _hit))
            {
                if(Vector3.Distance(transform.position, _hit.point) < _blinkDistance)
                {
                    _blinkObject.transform.position = new Vector3(_hit.point.x, _hit.point.y + 0.2f, _hit.point.z);
                    _blinkObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    _pMain.startColor = new Color(0.43f, 0.57f, 0.99f, 1);

                    if (Input.GetMouseButtonDown(0))
                    {
                        _playerIsBlinking = true;
                        MakePlayerBlink(_hit.point);
                        
                    }

                }
                if(Vector3.Distance(transform.position, _hit.point) > _blinkDistance)
                {
                    _blinkObject.transform.position = new Vector3(_hit.point.x, _hit.point.y + 0.2f, _hit.point.z);
                    _blinkObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
                    _pMain.startColor = new Color(1, 0, 0, 1);
                }
            }
        }

        void MakePlayerBlink(Vector3 _blinkLoc)
        {
            GameObject _blinkedParticles = Instantiate(Resources.Load("PlayerSpells/Ability/PlayerBlinked_Sparkles"), _blinkLoc, Quaternion.identity) as GameObject;
            _blinkedParticles.GetComponent<ParticleSystem>().Play();
            transform.position = new Vector3(_blinkLoc.x, _blinkLoc.y + 1, _blinkLoc.z);
            _isBlink = false;

            SoundSystem.PlayerBlink(this.transform.position);

            GameInteraction.ResetBlinkCooldown();
            CombatSystem.CameraController.CameraShake(2, 0.5f);
            GameInteraction.SpellHasBeenCast();

            Destroy(_blinkObject);
            Destroy(_blinkedParticles, 1.5f);
            _playerIsBlinking = false;
            
        }

        public void SetPlayerIdle()
        {
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isCombatIdle", false);
            _playerAnimator.SetBool("skipIdle", false);
            _isInCombat = false;
            Debug.Log("Player: Player has been set to Idle");

        }

        void OnTriggerEnter(Collider coll)
        {

            if (coll.GetComponent<SpellObject>() != null)
            {
                if (!coll.GetComponent<SpellObject>().ReturnFromPlayer())
                {
                    if (!_takenDamage)
                    {
                        _playerHealth -= (int)coll.GetComponent<SpellObject>().ReturnDamage();
                        GameInteraction.SetPlayerHealth(_playerHealth);
                        GameInteraction.DisplayDamageDoneToPlayer((int)coll.GetComponent<SpellObject>().ReturnDamage(), transform.position);
                        Destroy(coll.gameObject);
                        SoundSystem.PlayerHit(this.transform.position);

                        // Set _takeDamage to true so the player is immune for <seconds>
                        _takenDamage = true;

                    }
                    
                }

            }

            if(coll.tag == "AOE_EarthQuake")
            {
                _isMoving = false;
                _moveToAttack = false;
                _isKnockedBack = true;
                _setDisengageTarget = false;

                Debug.Log("EARTHQUAKE");
                //PlayerThrow();
            }

            if(coll.name == "Enemy_MELEE_TRIGGER")
            {
                if(!_takenDamage)
                {
                    _playerHealth -= (int)coll.GetComponentInParent<EnemyCombat.EnemyCombatSystem>().ReturnEnemyDamage();
                    GameInteraction.SetPlayerHealth(_playerHealth);
                    SoundSystem.PlayerHit(this.transform.position);

                    // Set _takeDamage to true so the player is immune for <seconds>
                    
                    _takenDamage = true;

                    
                }
            }
        }


        void AddLocationMarker()
        {
            if (_posClicked != null)
            {
                Destroy(_posClicked);
                _posClicked = (GameObject)Instantiate(_clickMoveIcon, new Vector3(_targetPosition.x, _targetPosition.y + 0.1f, _targetPosition.z), Quaternion.identity);

            }
            else
            {
                _posClicked = (GameObject)Instantiate(_clickMoveIcon, new Vector3(_targetPosition.x, _targetPosition.y + 0.1f, _targetPosition.z), Quaternion.identity);

            }


        }

        public static void PlayerKnockback(float _distance)
        {
            _disengageDistance = _distance;
            _isKnockedBack = true;
            _setDisengageTarget = false;               
        }

        void PlayerThrow()
        {
            if(!_setDisengageTarget)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if(transform.GetChild(i).name == "DisengageTarget") {
                    _disengageTarget = transform.GetChild(i).gameObject.transform.position;
                    }   
                }
                _setDisengageTarget = true;
            }
            if (_setDisengageTarget)
            {
                _oldPosition = transform.position;
                Vector3 _dir = _disengageTarget - transform.position;                                               // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                
                Vector3 _back = transform.TransformDirection(Vector3.back);                           // create a forward Vector3
                
                if (Vector3.Distance(transform.position, _disengageTarget) > 1f)
                {                                                                                            // if the magnitude of the vector is greater than
                    _charController.SimpleMove(_back * 20);                                // move the actual player
                }

                if (Vector3.Distance(transform.position, _disengageTarget) < 1f)
                {
                    _isKnockedBack = false;
                    //Destroy(_disengageTarget);
                    _setDisengageTarget = false;
                    _isMoving = false;
                    _moveToAttack = false;
                    
                }
            }
            GameInteraction.SpellHasBeenCast();
        }

        static void CheckGround(Vector3 _playerPos)
        {
            RaycastHit hit;
            if (Physics.Raycast(_playerPos, Vector3.down, out hit))
            {
                if (hit.collider.gameObject.tag == "DamageField")
                {
                    if(!_takenDamage)
                    {
                        _playerHealth -= 5;
                        GameInteraction.SetPlayerHealth(_playerHealth);
                        SoundSystem.PlayerHit(_playerPos);
                        _takenDamage = true;
                        Combat.InitiateCombat();
                    }   
                }
                else
                {
                    
                }
            }
        }

        static void ImmunityTimer()
        {
            if (_takenDamage)
            {
                if (_damageTimer < _immunity)
                {
                    _damageTimer += Time.deltaTime;
                    
                }
            }
            if (_damageTimer >= _immunity)
            {

                _takenDamage = false;
                _damageTimer = 0;

            }
        }

        // HEALTH AND MANA CALCULATIONS
 
        void RegenerateHealth()
        {
            if(!Combat.ReturnInCombat())
            {
                InvokeRepeating("RegenerateAddHealth()", 3.0f, 3.0f);
            }
            if(Combat.ReturnInCombat())
            {
                CancelInvoke();
            }
        }

        void RegenerateMana()
        {
            if (!Combat.ReturnInCombat())
            {
                InvokeRepeating("RegenerateAddMana()", 3.0f, 3.0f);
            }
            if(Combat.ReturnInCombat())
            {
                CancelInvoke();
            }
        }

        public static void AddPlayerHealth(int _amount)
        {
            _playerHealth += _amount;
            GameInteraction.SetPlayerHealth(_playerHealth);
        }

        public static void AddPlayerMana(int _amount)
        {
            _playerMana += _amount;
            GameInteraction.SetPlayerMana(_playerMana);
        }

        void RegenerateAddHealth()
        {
            _playerHealth += 5;
        }

        void RegenerateAddMana()
        {
            _playerMana += 5;
        }

        void OnControllerColliderHit(ControllerColliderHit _hit)
        {
            if (_isKnockedBack)
            {
                if(_hit.collider.tag == "Border_Collider")
                {
                    _isKnockedBack = false;
                    Debug.Log("Cancelled disengage / pushback");
                }
            }
        }

        void CheckMouseOver()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            

            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.collider.tag == "NPC")
                {
                    GameInteraction.SetNpcCursor();
                    NPCSystem.NPC.HighlightNPC(true, _hit.collider.gameObject);
      
                }
                if(_hit.collider.tag == "EnemyRanged" || _hit.collider.tag == "EnemyMelee")
                {
                    GameInteraction.SetCombatCursor();
                    
                }
                if(_hit.collider.tag != "NPC" && _hit.collider.tag != "EnemyRanged" && _hit.collider.tag != "EnemyMelee" )
                {
                    GameInteraction.SetNormalCursor();
                    NPCSystem.NPC.HighlightNPC(false, null);
                }
            }
        }

        public void SetEnemy(GameObject _nmy)
        {
            _targetObject = _nmy;

        }

        void RotatePlayerToEnemy()
        {
            if (_targetObject != null)
            {
                Vector3 _dir = _targetObject.transform.position - transform.position;                       // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player


            }
        }

        public static void SetOutOfCombat()
        {
            _isInCombat = false;

        }

        public static GameObject ReturnPlayerGameObject()
        {
            return _playerGameObject;
        }

        public static void CastHealingSpell(float _manaCost, float _value)
        {
            if (_playerMana - _manaCost > 0)
            {
                _healingSpellAmount = (int)_value;
                _castSpell = true;
                _castHealing = true;
                _playerMana -= (int)_manaCost;
                GameInteraction.SetPlayerMana(_playerMana);
            }
            else
            {
                _castHealing = false;
                Dialogue.DialogueManager.ShowMessage("Not enough mana", true);
            }
        }

        public static void StopMoving()
        {
            _isMoving = false;
        }
    }
}
