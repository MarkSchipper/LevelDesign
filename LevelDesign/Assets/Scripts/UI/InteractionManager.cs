using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;

namespace CombatSystem
{
    public class InteractionManager : MonoBehaviour
    {

        // Singleton
        public static InteractionManager instance = null;

        private Vector2 _devScreenSize = new Vector2(1754, 987);

        private bool _isLoadingLevel = false;
        private bool _showTooltip = false;
        private bool _isSpellCasting = false;
        private bool _showActorHUD = false;
        private bool _hoveringOverUI = false;
        private bool _showUI = true;
        private bool _gamePaused = false;

        private float _spellCastTimer;
        private int _spellID;

        private GameObject _selectedActor;

        private string _toolTip;
                
        private List<Texture2D> _guiIcons = new List<Texture2D>();

        private Text _expText;

        private Image _expFill;
        private Image _castBar;

        private Image _deathScreen_bg;
        private Image _deatScreen_hover;

        // Enemy
        private Image[] _enemyBars;
        private Image _enemyHP;
        private Image _enemyHUD;
        private Text _enemyText;

        // Player
        private Image _playerHP;
        private Image _playerMana;
        private Image _playerInCombat;

        private float _playerMaxHealth;
        private float _playerMaxMana;

        private Texture2D _cursorNormal;
        private Texture2D _cursorNPC;
        private Texture2D _cursorCombat;
        private static Texture2D[] _icons;

        

        private List<float> _allCooldowns;
        private float[] _spellTimer;
        private bool[] _cooldownComplete;

        private bool _playSoundOnce = false;

        private GameObject _spawnVFX;
        private GameObject _spellCastVFX;
        private GameObject _canvas;
        private GameObject _pauseScreen;

        // loading screen
        public Sprite _loadingScreenImage;
        private GameObject _canvasLoadingScreenImage;
        private Texture2D _loadBar;
        private float _loadingProgress;

        private GUISkin _skin;

        void Awake()
        {
            if (instance == null)
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

            // Loading from the Resources Folder
            _skin = Resources.Load("Skins/Combat_HUD") as GUISkin;
            _cursorNormal = Resources.Load("Icons/Cursor/Cursor_Normal") as Texture2D;
            _cursorCombat = Resources.Load("Icons/Cursor/Cursor_Combat") as Texture2D;
            _cursorNPC = Resources.Load("Icons/Cursor/Cursor_NPC") as Texture2D;
            _icons = Resources.LoadAll<Texture2D>("PlayerSpells/SpellIcons");
            _spawnVFX = Resources.Load("VFX/SpellCasting_VFX") as GameObject;
            _pauseScreen = GameObject.Find("PauseMenu");

            _pauseScreen.SetActive(false);

            // Call the Database to get all the spells and store them in their respective variables in the CombatDatabase Class for future use
            CombatSystem.CombatDatabase.GetAllSpells();

            CreateLoadingScreen();

            // Store all the cooldowns from ALL spells in _allCooldowns
            _allCooldowns = CombatDatabase.ReturnAllSpellCooldowns();

            // Create a new float Array with the Count of all the cooldowns
            _spellTimer = new float[_allCooldowns.Count];
            
            
            _cooldownComplete = new bool[_allCooldowns.Count];

            _expFill = GameObject.FindGameObjectWithTag("ExpBar_FILL").GetComponent<Image>();
            _expText = GameObject.FindGameObjectWithTag("ExpBar_Text").GetComponent<Text>();
            _castBar = GameObject.FindGameObjectWithTag("CastBar").GetComponent<Image>();

            _playerHP = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Image>();
            _playerMana = GameObject.FindGameObjectWithTag("PlayerMana").GetComponent<Image>();
            _playerInCombat = GameObject.FindGameObjectWithTag("PlayerInCombat").GetComponent<Image>();

            _deathScreen_bg = GameObject.Find("DeathScreen_BG").GetComponent<Image>();
            _deatScreen_hover = GameObject.Find("DeathScreen_Hover").GetComponent<Image>();

        }

        // Use this for initialization
        void Start()
        {
            
            // Get all the Spells in the database
            for (int i = 0; i < CombatSystem.CombatDatabase.ReturnSpellCount(); i++)
            {
                for (int j = 0; j < _icons.Length; j++)
                {
                    // If the Spell Icon in the database matches the Icons loaded from the resources folder ( stripped 23 characters )
                    if (CombatSystem.CombatDatabase.ReturnSpellIcon(i).ToString() == _icons[j].ToString().Remove(_icons[j].ToString().Length - 23))
                    {
                        // Add the Icon to be displayed in the _guiIcons List<>
                        _guiIcons.Add(_icons[j]);
                    }
                }
            }

            // Get the amount of cooldowns
            for (int i = 0; i < _allCooldowns.Count; i++)
            {
                
                _spellTimer[i] = _allCooldowns[i];
                _cooldownComplete[i] = true;
            }

            FillExperienceBar();


            // On start of game turn the Castbar off
            DisplayCastBar(false);

            SetNormalCursor();

            DisplayPlayerInCombat(false, _playerMaxHealth);

            _canvas = GameObject.Find("player_HUD");

            _deathScreen_bg.enabled = false;
            _deatScreen_hover.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //                                          SPELL CASTING                                           //
            //                                                                                                  //
            // if _isSpellCasting is true continue                                                              //
            //      Play the casting spell sound only once                                                      //
            //  if the spellCastTimer ( set when casting a spell ) is smaller than the casttime from the DB     //
            //      Display the CastBar                                                                         //
            //      Fill the CastBar                                                                            //
            //      Add Time.deltaTime to the _spellCastTimer                                                   //
            //                                                                                                  //
            // If the spellCastTimer is equal or greater than the casttime from the DB                          //
            //  Check if the spell that is being casted is a damage spell or a healing spell                    //
            //      Cast the corresponding spell in the PlayerController class                                  //
            //      Play the sound                                                                              //
            // Turn the Cast Bar off                                                                            //
            // Set _iSpellCasting to false ( break the loop )                                                   //
            // Reset the _playSound once                                                                        //
            //                                                                                                  //
            //////////////////////////////////////////////////////////////////////////////////////////////////////


            #region SPELL CASTING
            // Fill the cast bar while we are casting
            if (_isSpellCasting)
            {
                if(!_playSoundOnce)
                {
                    
                    SoundManager.instance.PlaySound(SOUNDS.PLAYERSPELLWARMUP, PlayerController.instance.ReturnPlayerPosition(), true);
                    CombatSystem.AnimationSystem.SetRangedSpell();
                    _playSoundOnce = true;
                }
                if (_spellCastTimer < CombatDatabase.ReturnCastTime(_spellID))
                {
                    DisplayCastBar(true);
                    FillCastBar(_spellCastTimer / CombatDatabase.ReturnCastTime(_spellID));
                    _spellCastTimer += Time.deltaTime;

                    

                }
                if (_spellCastTimer >= CombatDatabase.ReturnCastTime(_spellID))
                {
                    if (CombatDatabase.ReturnSpellType(_spellID) == SpellTypes.Damage)
                    {
                        PlayerController.instance.PlayerSpellCast();
                        CombatSystem.AnimationSystem.CastRangedSpell();
                        CombatSystem.AnimationSystem.SetCombatIdle();
                        //CombatSystem.AnimationSystem.StopRangedSpell();
                        if (_spellCastVFX != null)
                        {
                            Destroy(_spellCastVFX, 0.7f);
                        }
                        CombatSystem.PlayerController.instance.SetHandSpellCastinVFX(false);
                        
                    }
                    else if (CombatDatabase.ReturnSpellType(_spellID) == SpellTypes.Healing)
                    {
                        PlayerController.instance.PlayerHealingSpellCast(CombatDatabase.ReturnSpellValue(_spellID));
                        CombatSystem.AnimationSystem.CastHealingSpell();
                        CombatSystem.AnimationSystem.SetCombatIdle();
                    }
                    else if (CombatDatabase.ReturnSpellType(_spellID) == SpellTypes.Debuff)
                    {
                        PlayerController.instance.CastDebuff(CombatDatabase.ReturnSpellValue(_spellID), CombatDatabase.ReturnSpellPrefab(_spellID), CombatDatabase.ReturnDebuffAbility(_spellID));
                        CombatSystem.AnimationSystem.CastRangedSpell();
                        CombatSystem.AnimationSystem.SetCombatIdle();

                    }
                    SoundManager.instance.PlaySound(SOUNDS.PLAYERSPELLWARMUP, PlayerController.instance.ReturnPlayerPosition(), false);

                    DisplayCastBar(false);
                    _isSpellCasting = false;
                    _playSoundOnce = false;

                }

            }
            if (CombatSystem.AnimationSystem.RangedSpellFinished())
            {
                CombatSystem.AnimationSystem.StopRangedSpell();
            }

            if(CombatSystem.AnimationSystem.HealingSpellFinished())
            {
                CombatSystem.AnimationSystem.StopHealingSpell();
            }

            #endregion


            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //                              check if the cooldown is done                                       //
            //                                                                                                  //
            //  Check the _spellTimer ( set to zero when spell has been cast ) if its smaller than the full CD  //
            // If true, add Time.deltaTime                                                                      //
            // If greater than or equal to the full CD, set the cooldownComplete to true                        //
            //                                                                                                  //
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            #region COOLDOWN CHECK
            for (int i = 0; i < _allCooldowns.Count; i++)
            {
                if (_spellTimer[i] < _allCooldowns[i])
                {
                    _spellTimer[i] += Time.deltaTime;

                    if (_spellTimer[i] >= _allCooldowns[i])
                    {
                        _cooldownComplete[i] = true;

                    }

                }
            }
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //                                CASTING SPELLS WITH KEYBOARD INPUT                                //
            //                                                                                                  //
            // If a key has been pressed ( 1 - 9 ) and the cooldown is complete                                 //
            //  Check if it is a Ability or not                                                                 //
            //      Check what kind of Spell it is, Damage, AOE, Healing or Buff                                // 
            //          Check if the Player has enough mana to actually cast the spell                          //
            //              Is the  Player facing the enemy?                                                    //
            //                  Set the spell in the Combat class                                               //
            //                  Set the spellCastTimer ( to fill the Cat Bar ) to 0.0f                          //
            //                  Set spellTimer for the CD to 0.0f                                               //
            //                  Set isSpellCasting to true                                                      //
            //                  Set the SpellID                                                                 //
            //              If not display message                                                              //
            //                                                                                                  //
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _gamePaused = !_gamePaused;
                if (_gamePaused)
                {
                    _pauseScreen.SetActive(true);
                    Time.timeScale = 0;
                }
                else
                {
                    _pauseScreen.SetActive(false);
                    Time.timeScale = 1;
                }
            }

            if (_showUI)
            {
                #region SPELL CASTING INPUT CHECK

                for (int i = 0; i < _allCooldowns.Count; i++)
                {
                    // if the player inputs Key i + 1 ( since i starts at 0 ) AND the cooldown is complete
                    if (Input.GetKeyDown((i + 1).ToString()) && _cooldownComplete[i])
                    {
                        // if it is not an ability -> it is a spell
                        if (CombatDatabase.ReturnAbility(i) == Abilities.None)
                        {
                            // if its a damage spell
                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Damage)
                            {
                                if (PlayerController.instance.ReturnSelectedActor() != null)
                                {
                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        if (PlayerController.instance.ReturnPlayerIsInRange())
                                        {
                                            if (PlayerBattle.instance.IsPlayerFacingEnemy())
                                            {
                                                _spellCastVFX = Instantiate(_spawnVFX, CombatSystem.PlayerController.instance.ReturnPlayerPosition(), Quaternion.identity) as GameObject;
                                                _spellCastVFX.transform.Rotate(new Vector3(-90, 0, 0));

                                                CombatSystem.PlayerController.instance.SetHandSpellCastinVFX(true);
                                                Combat.SetSpell(CombatDatabase.ReturnSpellID(i), CombatDatabase.ReturnSpellType(i), CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i), _selectedActor, PlayerController.instance.ReturnPlayerGameObject());

                                                _spellCastTimer = 0.0f;
                                                _spellTimer[i] = 0.0f;

                                                _isSpellCasting = true;
                                                _spellID = i;
                                            }
                                            else
                                            {
                                                Dialogue.DialogueManager.instance.ShowMessage("You are not facing your target", true);
                                            }
                                        }
                                        else
                                        {
                                            Dialogue.DialogueManager.instance.ShowMessage("You are to far away", true);
                                        }
                                    }
                                    else
                                    {
                                        Dialogue.DialogueManager.instance.ShowMessage("Not enough mana", true);
                                    }
                                }
                                else
                                {
                                    Dialogue.DialogueManager.instance.ShowMessage("Nothing selected", true);
                                }
                            }
                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.AOE)
                            {
                                //PlayerMovement.ToggleAoE();
                                //Combat.SetAOE(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellPrefab(i));
                                //PlayerMovement.CastAOE(CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellManaCost(i));
                                break;
                            }

                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Healing)
                            {

                                if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                {
                                    Combat.SetHealingSpell(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i));
                                    _spellCastTimer = 0.0f;
                                    _spellTimer[i] = 0.0f;
                                    _isSpellCasting = true;
                                    _spellID = i;
                                    _cooldownComplete[i] = false;
                                }

                            }
                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Buff)
                            {
                                // buff
                            }
                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Debuff)
                            {
                                if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                {
                                    if (PlayerController.instance.ReturnSelectedActor() != null)
                                    {
                                        if (PlayerController.instance.ReturnPlayerIsInRange())
                                        {

                                            _spellCastTimer = 0.0f;
                                            _spellTimer[i] = 0.0f;
                                            _isSpellCasting = true;
                                            _spellID = i;
                                            _cooldownComplete[i] = false;
                                        }
                                        else
                                        {
                                            Dialogue.DialogueManager.instance.ShowMessage("You are to far away", true);
                                        }
                                    }
                                    else
                                    {
                                        Dialogue.DialogueManager.instance.ShowMessage("Nothing selected", true);
                                    }
                                }
                            }

                        }
                        if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Ability)
                        {
                            if (CombatDatabase.ReturnAbility(i) == Abilities.Blink)
                            {
                                if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                {
                                    PlayerController.instance.Blink(CombatDatabase.ReturnBlinkRange(i));
                                    _spellCastTimer = 0.0f;
                                    _spellTimer[i] = 0.0f;
                                    _spellID = i;
                                    _cooldownComplete[i] = false;
                                }
                            }

                            if (CombatDatabase.ReturnAbility(i) == Abilities.Disengage)
                            {
                                // disengage
                            }

                            if (CombatDatabase.ReturnAbility(i) == Abilities.Charge)
                            {
                                // charge
                            }

                            if (CombatDatabase.ReturnAbility(i) == Abilities.Barrier)
                            {

                                if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                {
                                    PlayerController.instance.CreateBarrier(CombatDatabase.ReturnSpellValue(i));
                                    _spellCastTimer = 0.0f;
                                    _spellTimer[i] = 0.0f;
                                    _spellID = i;
                                    _cooldownComplete[i] = false;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }


        void OnGUI()
        {
            #region DEATH SCREEN
            if(PlayerController.instance.ReturnPlayerDead())
            {

                _deathScreen_bg.enabled = true;
                Rect _deadButton = new Rect(Screen.width / 2 - (50 * (Screen.width / _devScreenSize.x)), Screen.height / 2 - (35 * (Screen.height / _devScreenSize.y)), 120 * (Screen.width / _devScreenSize.x), 120 * (Screen.height / _devScreenSize.y));
                if(_deadButton.Contains(Event.current.mousePosition))
                {
                    _deatScreen_hover.enabled = true;
                    if(Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                    {
                        PlayerSpawner.instance.PlayerRespawn(CombatSystem.PlayerController.instance.ReturnPlayerPosition());
                        CombatSystem.SoundManager.instance.PlaySound(SOUNDS.UICLICK);
                    }
                }
                else
                {
                    _deatScreen_hover.enabled = false;
                    
                }
                
            }
            if(!PlayerController.instance.ReturnPlayerDead())
            {
                if(_deathScreen_bg.enabled && _deathScreen_bg != null)
                {
                    _deathScreen_bg.enabled = false;
                    _deatScreen_hover.enabled = false;
                }
            }
            #endregion

            #region LEVELISACTIVE
            // If we are not loading a Level, perform the OnGUI
            if (!_isLoadingLevel)
            {

                if (_showUI)
                {
                    if (_showTooltip)
                    {
                        GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 200, 250, 200), _toolTip, _skin.GetStyle("Tooltip"));
                    }
                    DisplaySpellIcons();

                    

                    // If the player has selected an Actor ( NPC or Enemy )

                    if (_showActorHUD)
                    {
                        if (_selectedActor != null)
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            //                                                              Enemy Selected                                                          //
                            //                                                                                                                                      //
                            // Since we use Bars over the Enemy we need to go through all the components to find the GameObject with the tag "EnemyHP"              //
                            // Store that GO in _enemyHP, enable it and fill it with the Enemy Health                                                               //
                            //                                                                                                                                      //
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            #region ENEMY SELECTED
                            if (_selectedActor.tag == "EnemyRanged" || _selectedActor.tag == "EnemyMelee")
                            {
                                _enemyBars = _selectedActor.GetComponentsInChildren<Image>();
                                for (int i = 0; i < _enemyBars.Length; i++)
                                {

                                    if (_enemyBars[i].tag == "EnemyHP")
                                    {
                                        _enemyHP = _enemyBars[i];
                                    }

                                    _enemyBars[i].enabled = true;

                                }

                                _enemyHP.fillAmount = _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnHealth() / _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnMaxHealth();

                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (_enemyBars != null)
                        {
                            if (_enemyBars.Length > 0)
                            {
                                for (int i = 0; i < _enemyBars.Length; i++)
                                {
                                    if (_enemyBars[i] != null)
                                    {
                                        if (_enemyBars[i].enabled)
                                        {
                                            _enemyBars[i].enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(_isLoadingLevel)
            { 
                if (_canvasLoadingScreenImage != null)
                {
                    
                    GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, _loadingProgress, 50), _loadBar);
                }
            }
            #endregion

            #region PAUSE SCREEN

            if(_gamePaused)
            {

            }

            #endregion
        }

        void CreateLoadingScreen()
        {
            

            _canvasLoadingScreenImage = new GameObject();
            _canvasLoadingScreenImage.name = "LoadImage";
            _canvasLoadingScreenImage.transform.SetParent(GameObject.Find("Canvas").transform);

            _canvasLoadingScreenImage.AddComponent<Image>();
            _canvasLoadingScreenImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            _canvasLoadingScreenImage.GetComponent<Image>().sprite = _loadingScreenImage;
            _canvasLoadingScreenImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _canvasLoadingScreenImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            _canvasLoadingScreenImage.SetActive(false);

            _loadBar = Resources.Load("Scenes/LoadingScreens/LoadingBar") as Texture2D;

        }

        public void SetLoadingProgress(float progress)
        {
            _loadingProgress = progress;
        }

        void DisplaySpellIcons()
        {
            // The rectangles for the actuall spells
            Rect[] _spellRect = new Rect[_guiIcons.Count];
            Rect[] _cooldownRect = new Rect[_guiIcons.Count];
            int[] _uiCooldown = new int[_guiIcons.Count];

            // We create a MainRect to 'catch' the mouse and prevent moving the character when pressing a spell
            Rect _mainRect = new Rect(Screen.width / 2 - (282 * (Screen.width / _devScreenSize.x)), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 500, 70);
            GUI.Box(_mainRect, "", _skin.GetStyle("SpellBox"));

            if(_mainRect.Contains(Event.current.mousePosition))
            {
                _hoveringOverUI = true;
            }
            else
            {
                _hoveringOverUI = false;
                _showTooltip = false;
            }

            for (int i = 0; i < _guiIcons.Count; i++)
            {
                _uiCooldown[i] = (int)_allCooldowns[i] - (int)_spellTimer[i];

                if (_uiCooldown[i] <= 0)
                {
                    _cooldownComplete[i] = true;

                }

                // Create the spell buttons    

                // The scaling of the button is done by:
                // Screen.width / 2 so it is in the center of the screen
                // Screen.width / 2 - 218 so the buttons fit in the squares of the UI ( manually placed )
                // We multiply that 218 by ( Screen.width divided by the _devScreenSize.x ) [ _devScreenSize ] is the size of the GameView of the developer
                // So for example:
                // 218 * 0.95 
                //
                // We then take that number and add 77 multiplied by again the Screen.width divided by the horizontal size of the gamedeveloper gameview multiplied by i
                // So for example:
                // 218 * 0.95 + 77 * 0.95 * 1
                // The 77 is an Offset between each button over horizontal space

                _spellRect[i] = new Rect(Screen.width / 2 - (282 * (Screen.width / _devScreenSize.x)) + ((74.5f * (Screen.width / _devScreenSize.x)) * i), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 58 * (Screen.width / _devScreenSize.x), 58 * (Screen.height / _devScreenSize.y));
                GUI.Box(_spellRect[i], _guiIcons[i], _skin.GetStyle("SpellBox"));

                _cooldownRect[i] = new Rect(Screen.width / 2 - (282 * (Screen.width / _devScreenSize.x)) + ((74.5f * (Screen.width / _devScreenSize.x)) * i), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 58 * (Screen.width / _devScreenSize.x), 58 * (Screen.height / _devScreenSize.y));

                if (_uiCooldown[i] > 0)
                {
                    GUI.Box(_cooldownRect[i], _uiCooldown[i].ToString(), _skin.GetStyle("Cooldown"));
                    _cooldownComplete[i] = false;
                }
                
                //GUI.Box(_spellRect[i],)
                // If the mouse is in one of the rectangles
                if (_spellRect[i].Contains(Event.current.mousePosition))
                {
                    
                    _toolTip = CreateToolTip(CombatDatabase.ReturnSpellDesc(i), CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellType(i));
                    _showTooltip = true;


                    // If we are NOT spell casting - we can cast a new spell
                    if (!_isSpellCasting)
                    {
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !PlayerBattle.instance.ReturnIsCastingSpell())
                        {
                            // if it is not an ability -> it is a spell
                            if (CombatDatabase.ReturnAbility(i) == Abilities.None)
                            {
                                // if its a damage spell
                                if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Damage)
                                {
                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        if (PlayerBattle.instance.IsPlayerFacingEnemy())
                                        {
                                            _spellCastVFX = Instantiate(_spawnVFX, CombatSystem.PlayerController.instance.ReturnPlayerPosition(), Quaternion.identity) as GameObject;
                                            _spellCastVFX.transform.Rotate(new Vector3(-90, 0, 0));

                                            CombatSystem.PlayerController.instance.SetHandSpellCastinVFX(true);
                                            Combat.SetSpell(CombatDatabase.ReturnSpellID(i), CombatDatabase.ReturnSpellType(i), CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i), _selectedActor, PlayerController.instance.ReturnPlayerGameObject());

                                            _spellCastTimer = 0.0f;
                                            _spellTimer[i] = 0.0f;

                                            _isSpellCasting = true;
                                            _spellID = i;
                                        }
                                        else
                                        {
                                            Dialogue.DialogueManager.instance.ShowMessage("YOU ARE NOT FACING YOUR TARGET", true);
                                        }
                                    }
                                }
                                if (CombatDatabase.ReturnSpellType(i) == SpellTypes.AOE)
                                {
                                    //PlayerMovement.ToggleAoE();
                                    //Combat.SetAOE(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellPrefab(i));
                                    //PlayerMovement.CastAOE(CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellManaCost(i));
                                    break;
                                }

                                if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Healing)
                                {

                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        Combat.SetHealingSpell(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i));
                                        _spellCastTimer = 0.0f;
                                        _spellTimer[i] = 0.0f;
                                        _isSpellCasting = true;
                                        _spellID = i;
                                        _cooldownComplete[i] = false;
                                    }

                                }
                                if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Buff) 
                                {
                                    // buff
                                }
                                if(CombatDatabase.ReturnSpellType(i) == SpellTypes.Debuff)
                                {
                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        if (PlayerController.instance.ReturnSelectedActor() != null)
                                        {
                                            if (PlayerController.instance.ReturnPlayerIsInRange())
                                            {
                                                
                                                _spellCastTimer = 0.0f;
                                                _spellTimer[i] = 0.0f;
                                                _isSpellCasting = true;
                                                _spellID = i;
                                                _cooldownComplete[i] = false;
                                            }
                                            else
                                            {
                                                Dialogue.DialogueManager.instance.ShowMessage("You are to far away", true);
                                            }
                                        }
                                        else
                                        {
                                            Dialogue.DialogueManager.instance.ShowMessage("Nothing selected", true);
                                        }
                                    }
                                }
                            }
                            
                            if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Ability)
                            {

                                if (CombatDatabase.ReturnAbility(i) == Abilities.Blink)
                                {
                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        PlayerController.instance.Blink(CombatDatabase.ReturnBlinkRange(i));
                                        _spellCastTimer = 0.0f;
                                        _spellTimer[i] = 0.0f;
                                        _spellID = i;
                                        _cooldownComplete[i] = false;
                                    }
                                }

                                if (CombatDatabase.ReturnAbility(i) == Abilities.Disengage)
                                {
                                    // disengage
                                }

                                if (CombatDatabase.ReturnAbility(i) == Abilities.Charge)
                                {
                                    // charge
                                }

                                if (CombatDatabase.ReturnAbility(i) == Abilities.Barrier)
                                {
                                    
                                    if (PlayerController.instance.CanPlayerCastSpell(CombatDatabase.ReturnSpellManaCost(i)))
                                    {
                                        PlayerController.instance.CreateBarrier(CombatDatabase.ReturnSpellValue(i));
                                        _spellCastTimer = 0.0f;
                                        _spellTimer[i] = 0.0f;
                                        _spellID = i;
                                        _cooldownComplete[i] = false;
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    //_showTooltip = false;
                }
            }

            // If the mouse is over the MainRect 
            if (_mainRect.Contains(Event.current.mousePosition))
            {

                //CombatSystem.PlayerMovement.HoveringOverSpellbar(true);


            }
            else
            {
                //CombatSystem.PlayerMovement.HoveringOverSpellbar(false);

            }
        }

        string CreateToolTip(string _text, float _value, float _manacost, float _casttime, SpellTypes _type)
        {
            if (_type == SpellTypes.Damage || _type == SpellTypes.AOE)
            {
                _toolTip = _text + "\n" + "Damage: " + _value + "\n" + "Mana: " + _manacost + "\n" + "Cast Time: " + _casttime;
            }
            if(_type == SpellTypes.Ability)
            {
                _toolTip = _text + "\n" + "Mana: " + _manacost + "\n" + "Cast Time: " + _casttime;
            }
            if(_type == SpellTypes.Debuff)
            {
                _toolTip = _text + "\n" + "Duration: " + _value + "\n" + "Mana: " + _manacost + "\n" + "Cast Time: " + _casttime;
            }
            if(_type == SpellTypes.Healing)
            {
                _toolTip = _text + "\n" + "Healing: " + _value + "\n" + "Mana: " + _manacost + "\n" + "Cast Time: " + _casttime;
            }

            return _toolTip;
        }

        void DisplayCastBar(bool _set)
        {
            _castBar.transform.parent.gameObject.SetActive(_set);
        }

        void FillCastBar(float _value)
        {
            _castBar.fillAmount = _value;
        }

        //////////////////////////////////////////////////////////////////////////////
        //                              Cursor swapping                             //
        //                                                                          //
        // If the mouse is over an NPC -> NPC cursor                                //
        // If the mouse is over an Enemy -> Combat Cursor                           //
        // Else -> Normal Cursor                                                    //
        //                                                                          //
        //////////////////////////////////////////////////////////////////////////////

        #region CURSOR SWAPPING
        public void SetNpcCursor()
        {
            Cursor.SetCursor(_cursorNPC, Vector2.zero, CursorMode.Auto);

        }
        public void SetCombatCursor()
        {
            Cursor.SetCursor(_cursorCombat, Vector2.zero, CursorMode.Auto);
        }
        public void SetNormalCursor()
        {
            Cursor.SetCursor(_cursorNormal, Vector2.zero, CursorMode.Auto);
        }
        #endregion

        public void SetSelected(GameObject _selected)
        {
            
            if (_selected != null)
            {
                _selectedActor = _selected;
                _showActorHUD = true;
            }
            else
            {
                _showActorHUD = false;

            }
        }

        public void SetEnemyHealth(float _health)
        {
            if (_enemyHP.fillAmount > 0)
            {
                _enemyHP.fillAmount = _health / _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnMaxHealth();
            }
        }

        public void DisplayDamageDoneToEnemy(int _dmg, Vector3 _enemyPos)
        {
            Vector2 _screenPos = Camera.main.WorldToScreenPoint(_enemyPos);

            GameObject _floatingText = Instantiate(Resources.Load("UI/EnemyDamagePopups"), _enemyPos, Quaternion.identity) as GameObject;
            _floatingText.GetComponentInChildren<Text>().text = _dmg.ToString();

            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 200);

            Destroy(_floatingText, 2f);
        }

        public void DisplayDamageDoneToPlayer(int _dmg, Vector3 _playerPos)
        {
            Vector2 _screenPos = Camera.main.WorldToScreenPoint(_playerPos);

            GameObject _floatingText = Instantiate(Resources.Load("UI/PlayerDamagePopups"), _playerPos, Quaternion.identity) as GameObject;
            _floatingText.GetComponentInChildren<Text>().text = _dmg.ToString();


            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 200);

            Destroy(_floatingText, 2f);
        }

        public void DisplayExpGained(int _exp, Vector3 _playerPos)
        {
            Vector2 _screenPos = Camera.main.WorldToScreenPoint(_playerPos);

            GameObject _floatingText = Instantiate(Resources.Load("UI/PlayerExpPopups"), _playerPos, Quaternion.identity) as GameObject;
            _floatingText.GetComponentInChildren<Text>().text = "Exp: " + _exp.ToString(); 
            

            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 200);

            Destroy(_floatingText, 2f);

            FillExperienceBar();
        }

        public void SetPlayerMaxHealth(float _health)
        {
            _playerMaxHealth = _health;
        }

        public void SetPlayerMaxMana(float _mana)
        {
            _playerMaxMana = _mana;
        }

        public void SetPlayerMana(float _amount)
        {
            _playerMana.fillAmount = _amount / _playerMaxMana;
        }

        public void SetPlayerHealth(float _amount)
        {
            _playerHP.fillAmount = _amount / _playerMaxHealth;
            if(_playerInCombat.enabled)
            {
                _playerInCombat.fillAmount = _amount / _playerMaxHealth;
            }
        }

        public float ReturnPlayerMaxHealth()
        {
            return _playerMaxHealth;
        }

        public float ReturnPlayerMaxMana()
        {
            return _playerMaxMana;
        }

        public void DisplayPlayerInCombat(bool _set, float _health)
        {
            _playerInCombat.enabled = _set;
            _playerInCombat.fillAmount = _health / _playerMaxHealth;
        }

        public bool ReturnLoadingLevel()
        {
            return _isLoadingLevel;
        }

        public void FillExperienceBar()
        {
            // Set the Experience Bar and fill it with the players current progress
            _expFill.fillAmount = (float)CombatDatabase.ReturnPlayerExp() / (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());
            _expText.text = CombatDatabase.ReturnPlayerExp().ToString() + "/" + (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());
        }

        public void LoadLevel(bool _set)
        {
            _isLoadingLevel = _set;
            _canvasLoadingScreenImage.SetActive(_set);
        }

        public void LevelUp()
        {
            GameObject _tmp = Instantiate(Resources.Load("VFX/LevelUp"), CombatSystem.PlayerController.instance.ReturnPlayerPosition(), Quaternion.identity) as GameObject;
            SoundManager.instance.PlaySound(SOUNDS.LEVELUP, CombatSystem.PlayerController.instance.ReturnPlayerPosition(),true);
            Destroy(_tmp, 3f);

            Vector2 _screenPos = Camera.main.WorldToScreenPoint(CombatSystem.PlayerController.instance.ReturnPlayerPosition());

            GameObject _floatingText = Instantiate(Resources.Load("UI/PlayerLevelUp"), CombatSystem.PlayerController.instance.ReturnPlayerPosition(), Quaternion.identity) as GameObject;
            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 400);

            Destroy(_floatingText, 2f);

        }

        public bool ReturnHoveringOverUI()
        {
            return _hoveringOverUI;
        }

        public bool ReturnIsSpellCasting()
        {
            return _isSpellCasting;
        }
        
        public void CancelSpellCasting()
        {
            _isSpellCasting = false;
            CombatSystem.AnimationSystem.StopHealingSpell();
            CombatSystem.AnimationSystem.StopRangedSpell();
            DisplayCastBar(false);
            CombatSystem.SoundManager.instance.PlaySound(SOUNDS.PLAYERSPELLWARMUP, Vector3.zero, false);
            CombatSystem.AnimationSystem.StopRangedSpell();
            CombatSystem.AnimationSystem.StopCombatIdle();

            if (_spellCastVFX != null)
            {
                Destroy(_spellCastVFX, 0.7f);
            }
            //CombatSystem.PlayerController.instance.SetHandSpellCastinVFX(false);

        }

        public void ShowUI(bool _set)
        {
            _showUI = _set;
            _canvas.SetActive(_set);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _gamePaused = false;
            _pauseScreen.SetActive(false);
        }
    }

}