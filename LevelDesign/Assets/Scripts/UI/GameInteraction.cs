using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class mySorter : IComparer
{

    int IComparer.Compare(System.Object x, System.Object y)
    {
        return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
    }


}
namespace CombatSystem
{

    public class GameInteraction : MonoBehaviour
    {

        private static Image _enemyHP;
        private Image _enemyHUD;
        private Text _enemyText;

        private bool _hasFilledEnemyHP = false;

        private static Image _castBar;
        private static Image _playerHP;
        private static Image _playerMana;
        private static Image _playerInCombat;

        private static Image _expFill;
        private static Text _expText;

        private bool _inCombat = false;
        private static bool _spellHasBeenCast = false;

        public float _globalCooldown;
        private float _globalTimer;

        private bool _globalTrigger;

        private int _spellIndex;
        private float[] _spellCD;
        private static float[] _spellTimer;
        private static float _spellCastTimer;
        private bool[] _spellTrigger;
        private static bool[] _cooldownComplete;
        private static bool _isSpellCasting = false;
        private static int _spellID;

        private GameObject[] _playerSpells;
        private static List<float> _allCooldowns;
        
        private static Texture2D[] _icons;
        private static List<Texture2D> _guiIcons = new List<Texture2D>();

        private static GUISkin _skin;
        private static GameObject _selectedActor;
        private static bool _showBars = false;

        private static bool _showIcons = false;

        private static Texture2D _cursorNormal;
        private static Texture2D _cursorNPC;
        private static Texture2D _cursorCombat;

        private static int _playerMaxHealth;
        private static int _playerMaxMana;

        private static int _blinkSpellID;

        private static bool _loadingLevel = false;

        private static Vector2 _devScreenSize = new Vector2(1754, 987);

        void OnEnable()
        {

            _enemyHP = GameObject.FindGameObjectWithTag("EnemyHP").GetComponent<Image>();
            _enemyHP.transform.parent.gameObject.SetActive(false);

            _playerHP = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Image>();
            _playerMana = GameObject.FindGameObjectWithTag("PlayerMana").GetComponent<Image>();
            _playerInCombat = GameObject.FindGameObjectWithTag("PlayerInCombat").GetComponent<Image>();
            
            _castBar = GameObject.FindGameObjectWithTag("CastBar").GetComponent<Image>();

            _expFill = GameObject.FindGameObjectWithTag("ExpBar_FILL").GetComponent<Image>();
            _expText = GameObject.FindGameObjectWithTag("ExpBar_Text").GetComponent<Text>();

            _skin = Resources.Load("Skins/Combat_HUD") as GUISkin;
            CombatSystem.CombatDatabase.GetAllSpells();
            _icons = Resources.LoadAll<Texture2D>("PlayerSpells/SpellIcons");

            _allCooldowns = CombatDatabase.ReturnAllSpellCooldowns();
            _spellTimer = new float[_allCooldowns.Count];
            _cooldownComplete = new bool[_allCooldowns.Count];

            _cursorNormal = Resources.Load("Icons/Cursor/Cursor_Normal") as Texture2D;
            _cursorCombat = Resources.Load("Icons/Cursor/Cursor_Combat") as Texture2D;
            _cursorNPC = Resources.Load("Icons/Cursor/Cursor_NPC") as Texture2D;

            

        }

        // Use this for initialization
        void Start()
        {

            SetNormalCursor();

             _globalTimer = _globalCooldown;
            _globalTrigger = false;

            //_spellCD = new float[_playerSpells.Length];
            //_spellTimer = new float[_playerSpells.Length];
            //_spellTrigger = new bool[_playerSpells.Length];
            _playerInCombat.enabled = false;

            for (int i = 0; i < CombatSystem.CombatDatabase.ReturnSpellCount(); i++)
            {

                for (int j = 0; j < _icons.Length; j++)
                {
                    if (CombatSystem.CombatDatabase.ReturnSpellIcon(i).ToString() == _icons[j].ToString().Remove(_icons[j].ToString().Length - 23))
                    {
                        _guiIcons.Add(_icons[j]);
                    }
                }
            }


            for (int i = 0; i < _allCooldowns.Count; i++)
            {
                _spellTimer[i] = _allCooldowns[i];
                _cooldownComplete[i] = true;
            }

            CombatDatabase.GetPlayerStatistics();

            _expFill.fillAmount = (float)CombatDatabase.ReturnPlayerExp() / (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());
            _expText.text = CombatDatabase.ReturnPlayerExp().ToString() + "/" + (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());

        }
        // Update is called once per frame
        void Update()
        {

            if(Input.GetKeyDown("r"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }

            for (int i = 0; i < _allCooldowns.Count; i++)
            {
                if(_spellTimer[i] < _allCooldowns[i] && _spellHasBeenCast)
                {
                    _spellTimer[i] += Time.deltaTime;

                    if (_spellTimer[i] >= _allCooldowns[i])
                    {
                        _cooldownComplete[i] = true;
                        _spellHasBeenCast = false;
                        
                    }
                    
                }
             
            }
            
            // Fill the cast bar while we are casting
            if(_isSpellCasting)
            {
                if (_spellCastTimer < CombatDatabase.ReturnCastTime(_spellID)) {
                    FillCastBar(_spellCastTimer / CombatDatabase.ReturnCastTime(_spellID));
                    _spellCastTimer += Time.deltaTime;
                }
                if(_spellCastTimer >= CombatDatabase.ReturnCastTime(_spellID))
                {
                    _isSpellCasting = false;
                    PlayerMovement.SetMayCastSpell(true);
                    DisplayCastBar(false);
                }
             
            }
            
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
                            PlayerMovement.CastSpell(CombatDatabase.ReturnCastTime(i), _selectedActor, CombatDatabase.ReturnSpellManaCost(i));
                            _spellCastTimer = 0.0f;
                            _isSpellCasting = true;
                            _spellID = i;
                            if (PlayerMovement.ReturnCastSpell())
                            {
                                Combat.SetSpell(CombatDatabase.ReturnSpellID(i), CombatDatabase.ReturnSpellType(i), CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i), _selectedActor, PlayerMovement.ReturnPlayerGameObject());
                                _spellTimer[i] = 0.0f;
                                _cooldownComplete[i] = false;
                                break;
                            }
                        }
                        if (CombatDatabase.ReturnSpellType(i) == SpellTypes.AOE)
                        {
                            PlayerMovement.ToggleAoE();
                            Combat.SetAOE(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellPrefab(i));
                            PlayerMovement.CastAOE(CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellManaCost(i));
                            break;
                        }



                        if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Healing)
                        {
                            PlayerMovement.CastHealingSpell(CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnSpellValue(i));
                            _spellCastTimer = 0.0f;
                            _isSpellCasting = true;
                            _spellID = i;
                            if(PlayerMovement.ReturnCastSpell())
                            {
                                Combat.SetHealingSpell(CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i));
                                _spellTimer[i] = 0.0f;
                                _cooldownComplete[i] = false;
                                break;
                            }
                        }
                        if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Buff)
                        {
                            // buff
                        }
                    } 
                    if (CombatDatabase.ReturnSpellType(i) == SpellTypes.Ability)
                    {
                        if (CombatDatabase.ReturnAbility(i) == Abilities.Blink)
                        {
                            // blink
                            PlayerMovement.Blink(CombatDatabase.ReturnBlinkRange(i));
                            _blinkSpellID = i;
                            
                            break;
                        }

                        if (CombatDatabase.ReturnAbility(i) == Abilities.Disengage)
                        {
                            // disengage
                        }

                        if (CombatDatabase.ReturnAbility(i) == Abilities.Charge)
                        {
                            // charge
                        }
                    }
                }
            }
        
        }

        void OnGUI()
        {
            if (!_loadingLevel)
            {
                DisplaySpellIcons();

                if (_selectedActor != null)
                {
                    if (_showBars)
                    {
                        Rect _rect = new Rect(Screen.width - Screen.width + 60, Screen.height - Screen.height + 50, 310, 128);
                        Rect _npcName = new Rect(_rect.x + 150, _rect.y + 30, 100, 20);
                        Rect _npcProfession = new Rect(_rect.x + 150, _rect.y + 80, 100, 20);
                        //GUI.Box(_rect, "");
                        if (_selectedActor.tag == "NPC")
                        {
                            GUI.Box(_rect, "", _skin.GetStyle("NPC_HUD"));
                            GUI.Label(_npcName, _selectedActor.GetComponentInChildren<NPCSystem.NPC>().ReturnNpcName(), _skin.GetStyle("NPC_Name"));
                            GUI.Label(_npcProfession, _selectedActor.GetComponentInChildren<NPCSystem.NPC>().ReturnProfession(), _skin.GetStyle("NPC_Name"));
                        }
                        if (_selectedActor.tag == "EnemyRanged" || _selectedActor.tag == "EnemyMelee")
                        {
                            _enemyHP.transform.parent.gameObject.SetActive(true);
                            _enemyHP.fillAmount = _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().ReturnHealth() / _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().ReturnMaxHealth();

                            _enemyHP.transform.parent.GetComponentInChildren<Text>().text = _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().ReturnName();

                        }
                    }
                    if (!_showBars)
                    {
                        _enemyHP.transform.parent.gameObject.SetActive(false);
                    }
                }
                else
                {
                    _enemyHP.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public void SetGlobalCooldown()
        {
            _globalTrigger = true;

            _globalTimer = 0;
        }

        void GlobalCooldown()
        {

            GameObject[] _globalCD = GameObject.FindGameObjectsWithTag("GlobalCooldown");

            for (int i = 0; i < _globalCD.Length; i++)
            {

                if (_globalTimer < _globalCooldown)
                {

                    _globalCD[i].GetComponent<Image>().fillAmount = _globalTimer / _globalCooldown;

                }
                if (Math.Round(_globalTimer, 1) == _globalCooldown)
                {

                    _globalCD[i].GetComponent<Image>().fillAmount = 0;
                    _globalTrigger = false;

                }
            }


        }

        public void SetSelected(GameObject _selected)
        {

            if (_selected != null)
            {

                _enemyHUD = GameObject.FindGameObjectWithTag("EnemyHUD").GetComponent<Image>();
                _enemyHUD.enabled = true;
                _enemyHP = GameObject.FindGameObjectWithTag("EnemyHP").GetComponent<Image>();
                _enemyText = GameObject.FindGameObjectWithTag("EnemyName").GetComponent<Text>();

                if (_selected.tag == "EnemyRanged")
                {

                    _enemyHP.fillAmount = _selected.GetComponent<EnemyCombat.EnemyCombatSystem>().ReturnHealth() / _selected.GetComponent<EnemyCombat.EnemyCombatSystem>().ReturnMaxHealth();
                    _enemyText.text = _selected.GetComponent<EnemyCombat.EnemyCombatSystem>().ReturnName();
                    _enemyText.fontSize = _enemyText.fontSize * (int)(Screen.width / _devScreenSize.x);
                }

                if (_selected.tag == "EnemyMelee")
                {

                    _enemyHP.fillAmount = _selected.GetComponent<EnemyCombat.EnemyCombatSystem>().ReturnHealth() / 100;
                    _enemyText.text = _selected.GetComponent<EnemyCombat.EnemyCombatSystem>().ReturnName();
                    _enemyText.fontSize = _enemyText.fontSize * (int)(Screen.width / _devScreenSize.x);
                }

                if (_selected.tag == "NPC")
                {

                    _enemyText.text = _selected.GetComponentInChildren<NPCSystem.NPC>().ReturnNpcName();
                    _enemyHP.fillAmount = _selected.GetComponentInChildren<NPCSystem.NPC>().ReturnNpcHealth();
                    _enemyText.fontSize = _enemyText.fontSize * (int)(Screen.width / _devScreenSize.x);

                }
            }

            if (_selected = null)
            {
                _enemyHUD = GameObject.FindGameObjectWithTag("EnemyHUD").GetComponent<Image>();
                _enemyHUD.enabled = false;
            }

        }
       
        public static void SetEnemyHealth(float _health)
        {
            //Debug.Log("Health: " + _health + " - Max Health " + _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().ReturnMaxHealth());
            if (_enemyHP.fillAmount > 0)
            {
                _enemyHP.fillAmount = _health / _selectedActor.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().ReturnMaxHealth();
            }

        }

        public static void SetPlayerHealth(float _health)
        {

            _playerHP.fillAmount = _health / _playerMaxHealth;
            _playerInCombat.fillAmount = _health / _playerMaxHealth;

        }

        public static void SetPlayerMana(float _mana)
        {

            _playerMana.fillAmount = _mana / _playerMaxMana;


        }

        public static void SetPlayerMaxHealth(int _health)
        {
            _playerMaxHealth = _health;
        }

        public static void SetPlayerMaxMana(int _mana)
        {
            _playerMaxMana = _mana;
        }

        public static int ReturnPlayerMaxHealth()
        {
            return _playerMaxHealth;
        }

        public void EnemyDeath()
        {

            SetSelectedUI(null);

        }

        public static void DisplayPlayerInCombat(bool _combat, float _health)
        {
            _playerInCombat.enabled = _combat;
            _playerInCombat.fillAmount = _health / _playerMaxHealth;
            
        } 

        public static void SetSelectedUI(GameObject _selected)
        {
            _selectedActor = _selected;

            if (_selected != null)
            {
                _showBars = true;
            }
            if(_selected == null)
            {
                _showBars = false;
            }

        }

        static void DisplaySpellIcons()
        {
            // The rectangles for the actuall spells
            Rect[] _spellRect = new Rect[_guiIcons.Count];
            Rect[] _cooldownRect = new Rect[_guiIcons.Count];
            int[] _uiCooldown = new int[_guiIcons.Count];

            // We create a MainRect to 'catch' the mouse and prevent moving the character when pressing a spell
            Rect _mainRect = new Rect(Screen.width / 2 - (218 * (Screen.width / _devScreenSize.x)), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 500, 70);
            GUI.Box(_mainRect, "", _skin.GetStyle("SpellBox"));

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

                _spellRect[i] = new Rect(Screen.width / 2 - (218 * (Screen.width / _devScreenSize.x))  + ((77 * (Screen.width / _devScreenSize.x)) * i), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 58 * (Screen.width / _devScreenSize.x), 58 * (Screen.height / _devScreenSize.y));
                GUI.Box(_spellRect[i], _guiIcons[i], _skin.GetStyle("SpellBox"));

                _cooldownRect[i] = new Rect(Screen.width / 2 - (218 * (Screen.width / _devScreenSize.x)) + ((77 * (Screen.width / _devScreenSize.x)) * i), Screen.height - (154 * (Screen.height / _devScreenSize.y)), 58 * (Screen.width / _devScreenSize.x), 58 * (Screen.height / _devScreenSize.y));

                if (_uiCooldown[i] > 0)
                {
                    GUI.Box(_cooldownRect[i], _uiCooldown[i].ToString(), _skin.GetStyle("Cooldown"));
                    _cooldownComplete[i] = false;
                }
                //GUI.Box(_spellRect[i],)
                // If the mouse is in one of the rectangles
                if (_spellRect[i].Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseDown && !PlayerMovement.ReturnCastSpell())
                    {
                        if (CombatDatabase.ReturnAbility(i) != Abilities.None)
                        {
                            if (CombatDatabase.ReturnAbility(i) == Abilities.Disengage)
                            {
                                PlayerMovement.PlayerKnockback(CombatDatabase.ReturnDisengageDistance(i));
                                _spellTimer[i] = 0.0f;
                                _cooldownComplete[i] = false;
                            }
                            if(CombatDatabase.ReturnAbility(i) == Abilities.Blink)
                            {
                                PlayerMovement.Blink(CombatDatabase.ReturnBlinkRange(i));
                                _cooldownComplete[i] = false;
                            }
                        }
                        else
                        {
                            PlayerMovement.CastSpell(CombatDatabase.ReturnCastTime(i), _selectedActor, CombatDatabase.ReturnSpellManaCost(i));
                            if (PlayerMovement.ReturnCastSpell())
                            {
                               // Combat.SetSpell(CombatDatabase.ReturnSpellID(i), CombatDatabase.ReturnSpellType(i), CombatDatabase.ReturnSpellValue(i), CombatDatabase.ReturnSpellManaCost(i), CombatDatabase.ReturnCastTime(i), CombatDatabase.ReturnSpellPrefab(i), _selectedActor);
                                _spellTimer[i] = 0.0f;
                                _cooldownComplete[i] = false;
                            }
                        }
                    }
                }   
            }

            // If the mouse is over the MainRect 
            if (_mainRect.Contains(Event.current.mousePosition))
            {

                CombatSystem.PlayerMovement.HoveringOverUI(true);
                CombatSystem.PlayerMovement.SetDraggingUI(true);
                
            }
            else
            {
                CombatSystem.PlayerMovement.HoveringOverUI(false);
                CombatSystem.PlayerMovement.SetDraggingUI(false);
            }

        }

        public static void DisplayCastBar(bool _set)
        {
            _castBar.transform.parent.gameObject.SetActive(_set);
        }

        public static void FillCastBar(float _value)
        {
            _castBar.fillAmount = _value;
        }

        public static void FillExpBar()
        {
            CombatDatabase.GetPlayerStatistics();
            _expFill.fillAmount = (float)CombatDatabase.ReturnPlayerExp() / (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());
            _expText.text = CombatDatabase.ReturnPlayerExp().ToString() + "/" + (float)(CombatDatabase.ReturnPlayerLevel() * (float)CombatDatabase.ReturnExpMultiplier());
        }

        public static void SpellHasBeenCast()
        {
            _spellHasBeenCast = true;
        }

        public static void SetNpcCursor()
        {
            Cursor.SetCursor(_cursorNPC, Vector2.zero, CursorMode.Auto);
            
        }

        public static void SetCombatCursor()
        {
            Cursor.SetCursor(_cursorCombat, Vector2.zero, CursorMode.Auto);
        }
        public static void SetNormalCursor()
        {
            Cursor.SetCursor(_cursorNormal, Vector2.zero, CursorMode.Auto);
         
        }

        public static void DisplayDamageDoneToEnemy(int _dmg, Vector3 _enemyPos)
        {
            Vector2 _screenPos = Camera.main.WorldToScreenPoint(_enemyPos);

            GameObject _floatingText = Instantiate(Resources.Load("UI/EnemyDamagePopups"), _enemyPos, Quaternion.identity) as GameObject;
            _floatingText.GetComponentInChildren<Text>().text = _dmg.ToString();

            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 200);

            Destroy(_floatingText, 2f);

        }

        public static void DisplayDamageDoneToPlayer(int _dmg, Vector3 _playerPos)
        {
            Vector2 _screenPos = Camera.main.WorldToScreenPoint(_playerPos);

            GameObject _floatingText = Instantiate(Resources.Load("UI/PlayerDamagePopups"), _playerPos, Quaternion.identity) as GameObject;
            _floatingText.GetComponentInChildren<Text>().text = _dmg.ToString();

            Debug.Log(_dmg);
            _floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);
            _floatingText.transform.position = new Vector2(_screenPos.x, _screenPos.y + 200);

            Destroy(_floatingText, 2f);
        }

        public static void ResetBlinkCooldown()
        {

            _spellTimer[_blinkSpellID] = 0.0f;
            _cooldownComplete[_blinkSpellID] = false;

            
        }

        public static void ResetAoECooldown()
        {
            _spellTimer[4] = 0.0f;
            _cooldownComplete[3] = false;
        }

        public static void LoadingLevel()
        {
            _loadingLevel = true;
        }

        public static bool ReturnLoadingLevel()
        {
            return _loadingLevel;
        }

        public static void LevelUp(Vector3 _pos, GameObject _player)
        {
            GameObject _tmp = Instantiate(Resources.Load("VFX/LevelUp"), _pos, Quaternion.identity) as GameObject;
            _tmp.transform.SetParent(_player.transform);
            
            Destroy(_tmp, 3f);
        }

    }
}