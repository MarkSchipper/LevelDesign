using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum SpellTypes
{
    None,
    Buff,
    Damage,
    AOE,
    Healing,
    Ability,
    Melee,
}

public enum Abilities
{
    None,
    Blink,
    Charge,
    Disengage,
}

namespace CombatSystem
{

#if UNITY_EDITOR
    public class SpellManager : EditorWindow
    {


        private bool _addSpell = false;
        private bool _editSpell = false;
        private bool _deleteSpell = false;
        private bool _deleteConfirmation = false;

        private int _deleteSpellID;

        private Vector2 _scrollPos;

        private string _spellName;
        private string _spellDesc;
        private SpellTypes _spellType;
        private float _spellValue;
        private float _spellManaCost;
        private float _spellCasttime;
        private GameObject _spellPrefab;
        private string _spellIcon;

        private List<string> _damageSpellNames = new List<string>();
        private List<string> _healingSpellNames = new List<string>();
        private List<string> _buffSpellNames = new List<string>();
        private List<string> _abilitySpellNames = new List<string>();
        private List<string> _allSpellIconNames = new List<string>();
        private string _spellPrefabName;
        private float _spellCooldown;

        private UnityEngine.Object[] _damageSpellPrefabs;
        private UnityEngine.Object[] _healingSpellPrefabs;
        private UnityEngine.Object[] _buffSpellPrefabs;
        private UnityEngine.Object[] _abilitySpellPrefabs;
        private UnityEngine.Object[] _allSpellIcons;

        private Abilities _abilities;
        

        private int _spellIndex;
        private int _spellIconIndex;
        private int _abilityIndex;

        private float _chargeRange;
        private float _disengageDistance;
        private float _blinkRange;

        private bool _loadedSpells = false;

        private int _editSpellIndex;

        private GUISkin _skin;

        [MenuItem("Level Design/Player/Spell Manager")]
        // Use this for initialization



        static void ShowEditor()
        {
            SpellManager _spellManager = EditorWindow.GetWindow<SpellManager>();
        }

        void OnEnable()
        {

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;

            _damageSpellNames.Clear();
            _allSpellIconNames.Clear();
            CombatDatabase.ClearAll();

            _damageSpellPrefabs = Resources.LoadAll("PlayerSpells/Damage");
            _healingSpellPrefabs = Resources.LoadAll("PlayerSpells/Healing");
            _buffSpellPrefabs = Resources.LoadAll("PlayerSpells/Buff");
            _abilitySpellPrefabs = Resources.LoadAll("PlayerSpells/Ability");

            _allSpellIcons = Resources.LoadAll("PlayerSpells/SpellIcons");
            
            for (int i = 0; i < _damageSpellPrefabs.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_damageSpellPrefabs[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _damageSpellNames.Add(_damageSpellPrefabs[i].ToString().Remove(_damageSpellPrefabs[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _healingSpellPrefabs.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_healingSpellPrefabs[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _healingSpellNames.Add(_healingSpellPrefabs[i].ToString().Remove(_healingSpellPrefabs[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _buffSpellPrefabs.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_buffSpellPrefabs[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _buffSpellNames.Add(_buffSpellPrefabs[i].ToString().Remove(_buffSpellPrefabs[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _abilitySpellPrefabs.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_abilitySpellPrefabs[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _abilitySpellNames.Add(_abilitySpellPrefabs[i].ToString().Remove(_abilitySpellPrefabs[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _allSpellIcons.Length; i++)
            {
                
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_allSpellIcons[i].GetType().ToString() == "UnityEngine.Sprite")
                {

                    _allSpellIconNames.Add(_allSpellIcons[i].ToString().Remove(_allSpellIcons[i].ToString().Length - 20));

                }
            }
        }

        void OnGUI()
        {

            GUI.skin = _skin;

            GUILayout.Label("Welcome to the Spell Manager", EditorStyles.boldLabel);

            if (!_addSpell && !_editSpell && !_deleteSpell)
            {

                if (GUILayout.Button("Add Spell"))
                {
                    _addSpell = true;
                }

                if(GUILayout.Button("Edit Spell"))
                {
                    _editSpell = true;
                    if (!_loadedSpells)
                    {
                        CombatDatabase.GetAllSpells();
                        _loadedSpells = true;
                    }
                }

                if(GUILayout.Button("Delete Spell"))
                {
                    if (!_loadedSpells)
                    {
                        CombatDatabase.GetAllSpells();
                        _loadedSpells = true;
                    }
                    _deleteSpell = true;
                    
                }
            }
            if (_addSpell)
            {
                AddSpell();
            }

            if(_editSpell)
            {
                EditSpell();
            }
            if(_deleteSpell)
            {
                DeleteSpell();   
            }

        }

        void AddSpell()
        {

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Add a Spell", EditorStyles.boldLabel);

            _spellName = EditorGUILayout.TextField("Spell Name: ", _spellName);
            GUILayout.Label("Spell Description:");
            _spellDesc = EditorGUILayout.TextArea(_spellDesc, GUILayout.Height(50));

            _spellType = (SpellTypes)EditorGUILayout.EnumPopup("Type of Spell:", _spellType);

            if (_spellType != SpellTypes.None)
            {
                
                if (_spellType != SpellTypes.Ability)
                {
                    _spellValue = EditorGUILayout.FloatField("Spell Value: ", _spellValue);
                    _spellManaCost = EditorGUILayout.FloatField("Mana Cost: ", _spellManaCost);
                    _spellCooldown = EditorGUILayout.FloatField("Cooldown: ", _spellCooldown);
                    _spellCasttime = EditorGUILayout.FloatField("Cast Time: ", _spellCasttime);
                    if (_spellType == SpellTypes.Damage)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _damageSpellNames.ToArray());
                    }
                    if (_spellType == SpellTypes.Healing)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _healingSpellNames.ToArray());
                    }
                    if (_spellType == SpellTypes.Buff)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _buffSpellNames.ToArray());
                    }
                    
                }

                if(_spellType == SpellTypes.Ability)
                {
                    _abilities = (Abilities)EditorGUILayout.EnumPopup("Type of Ability: ", _abilities);

                    if(_abilities == Abilities.Charge)
                    {
                        _chargeRange = EditorGUILayout.FloatField("Charge Range: ", _chargeRange);
                        _spellValue = _chargeRange;
                    }
                    if(_abilities == Abilities.Disengage)
                    {
                        _disengageDistance = EditorGUILayout.FloatField("Disengage Distance: ", _disengageDistance);
                        _spellValue = _disengageDistance;

                        if (_disengageDistance > 0)
                        {
                            if (GameObject.Find("DisengageTarget") == null)
                            {
                                if (GUILayout.Button("Add Disengage Target"))
                                {
                                    GameObject _disTarget = new GameObject();
                                    _disTarget.name = "DisengageTarget";
                                    _disTarget.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
                                    _disTarget.transform.position = new Vector3(0, 1, _disengageDistance);
                                }
                            }
                        }

                    }
                    if(_abilities == Abilities.Blink)
                    {
                        _blinkRange = EditorGUILayout.FloatField("Blink Range: ", _blinkRange);
                        _spellValue = _blinkRange;
                    }

                }
                
                GUILayout.Space(20);

                _spellIconIndex = EditorGUILayout.Popup("Which Icon:", _spellIconIndex, _allSpellIconNames.ToArray());

                if (GUILayout.Button("ADD SPELL"))
                {
                    if (_spellType == SpellTypes.Damage)
                    {
                        CombatSystem.CombatDatabase.AddSpell(_spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _damageSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    if (_spellType == SpellTypes.Healing)
                    {
                        CombatSystem.CombatDatabase.AddSpell(_spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _healingSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    if (_spellType == SpellTypes.Buff)
                    {
                        CombatSystem.CombatDatabase.AddSpell(_spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _buffSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    if (_spellType == SpellTypes.Ability)
                    {
                        CombatSystem.CombatDatabase.AddSpell(_spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _abilitySpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    _addSpell = false;

                    _spellName = "";
                    _spellDesc = "";
                    _spellValue = 0f;
                    _spellCasttime = 0f;

                   if(_abilities == Abilities.Disengage)
                    {
                        if(GameObject.Find("DisengageTarget") != null)
                        {
                            GameObject.Find("DisengageTarget").transform.position = new Vector3(0, 1, _disengageDistance);
                        }
                    }
                }

            }

            if (GUILayout.Button("BACK"))
            {
                _addSpell = false;
            }

            EditorGUILayout.EndScrollView();

        }

        void EditSpell()
        {
    
     
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
 
            GUILayout.Label("Edit a Spell", EditorStyles.boldLabel);

            _editSpellIndex = EditorGUILayout.Popup("Which Spell: ", _editSpellIndex, CombatDatabase.ReturnAllSpellNames().ToArray());
            if(!_loadedSpells)
            {
                _spellName = CombatDatabase.ReturnSpellName(_editSpellIndex);
                _spellDesc = CombatDatabase.ReturnSpellDesc(_editSpellIndex);
                _spellType = CombatDatabase.ReturnSpellType(_editSpellIndex);
                _spellValue = CombatDatabase.ReturnSpellValue(_editSpellIndex);
                _spellCasttime = CombatDatabase.ReturnCastTime(_editSpellIndex);
                _spellPrefabName = CombatDatabase.ReturnSpellPrefab(_editSpellIndex);
                _spellIcon = CombatDatabase.ReturnSpellIcon(_editSpellIndex);
                _chargeRange = CombatDatabase.ReturnChargeRange(_editSpellIndex);
                _blinkRange = CombatDatabase.ReturnBlinkRange(_editSpellIndex);
                _disengageDistance = CombatDatabase.ReturnDisengageDistance(_editSpellIndex);
                _spellManaCost = CombatDatabase.ReturnSpellManaCost(_editSpellIndex);
                _abilities = CombatDatabase.ReturnAbility(_editSpellIndex);
                _spellCooldown = CombatDatabase.ReturnSpellCooldown(_editSpellIndex);

                for (int i = 0; i < _damageSpellNames.Count; i++)
                {
                    if (_spellPrefabName == _damageSpellNames[i])
                    {
                        _spellIndex = i;
                    }
                }

                for (int i = 0; i < _allSpellIconNames.Count; i++)
                {
                    if (_spellIcon == _allSpellIconNames[i])
                    {
                        _spellIconIndex = i;

                    }
                }


                _loadedSpells = true;
            }

            if(GUI.changed)
            {
                _loadedSpells = false;
            }

            _spellName = EditorGUILayout.TextField("Spell name: ", _spellName);

            _spellDesc = EditorGUILayout.TextArea(_spellDesc, GUILayout.Height(50));

            _spellType = (SpellTypes)EditorGUILayout.EnumPopup("Type of Spell:", _spellType);

            if (_spellType != SpellTypes.None)
            {

                if (_spellType != SpellTypes.Ability)
                {
                    _spellValue = EditorGUILayout.FloatField("Spell Value: ", _spellValue);
                    _spellManaCost = EditorGUILayout.FloatField("Mana Cost: ", _spellManaCost);
                    _spellCooldown = EditorGUILayout.FloatField("Cooldown: ", _spellCooldown);
                    _spellCasttime = EditorGUILayout.FloatField("Cast Time: ", _spellCasttime);


                    if (_spellType == SpellTypes.Damage)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _damageSpellNames.ToArray());
                    }
                    if (_spellType == SpellTypes.Healing)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _healingSpellNames.ToArray());
                    }
                    if (_spellType == SpellTypes.Buff)
                    {
                        _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _buffSpellNames.ToArray());
                    }
                }

                if (_spellType == SpellTypes.Ability)
                {
                    

                    _abilities = (Abilities)EditorGUILayout.EnumPopup("Type of Ability: ", _abilities);

                    if (_abilities == Abilities.Charge)
                    {
                        _chargeRange = EditorGUILayout.FloatField("Charge Range: ", _chargeRange);
                     
                    }
                    if (_abilities == Abilities.Disengage)
                    {
                        _disengageDistance = EditorGUILayout.FloatField("Disengage Distance: ", _disengageDistance);
                        if (_disengageDistance > 0)
                        {
                            if (GameObject.Find("DisengageTarget") == null)
                            {
                                if (GUILayout.Button("Add Disengage Target"))
                                {
                                    GameObject _disTarget = new GameObject();
                                    _disTarget.name = "DisengageTarget";
                                    _disTarget.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
                                    _disTarget.transform.position = new Vector3(0, 1, _disengageDistance);
                                }
                            }
                        }
                    }
                    if (_abilities == Abilities.Blink)
                    {
                        _blinkRange = EditorGUILayout.FloatField("Blink Range: ", _blinkRange);
                     
                    }
                    _spellManaCost = EditorGUILayout.FloatField("Mana Cost: ", _spellManaCost);
                    _spellCooldown = EditorGUILayout.FloatField("Cooldown: ", _spellCooldown);

                }
                GUILayout.Space(20);
               
                _spellIconIndex = EditorGUILayout.Popup("Which Icon:", _spellIconIndex, _allSpellIconNames.ToArray());

                if (GUILayout.Button("SAVE SPELL"))
                {
                    if (_spellType == SpellTypes.Damage)
                    {
                        CombatSystem.CombatDatabase.SaveSpell(CombatDatabase.ReturnSpellID(_editSpellIndex), _spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _damageSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    if (_spellType == SpellTypes.Healing)
                    {
                        CombatSystem.CombatDatabase.SaveSpell(CombatDatabase.ReturnSpellID(_editSpellIndex), _spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _healingSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    if (_spellType == SpellTypes.Buff)
                    {
                        CombatSystem.CombatDatabase.SaveSpell(CombatDatabase.ReturnSpellID(_editSpellIndex), _spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _buffSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _abilities, _spellCooldown);
                    }

                    _editSpell = false;

                    _spellName = "";
                    _spellDesc = "";
                    _spellValue = 0f;
                    _spellCasttime = 0f;


                    if (_abilities == Abilities.Disengage)
                    {

                        GameObject _tmpPlayer = GameObject.FindGameObjectWithTag("Player");

                        for (int i = 0; i < _tmpPlayer.transform.childCount; i++)
                        {
                            Debug.Log(_tmpPlayer.transform.childCount);
                            if (_tmpPlayer.transform.GetChild(i).name == "DisengageTarget")
                            {
                                _tmpPlayer.transform.GetChild(i).transform.position = new Vector3(0, 1, _disengageDistance * -1);
                                
                            }
                        }
                    }

                }

            }

            if (GUILayout.Button("BACK"))
            {
                
                _editSpell = false;
            }
            EditorGUILayout.EndScrollView();
          
        }

        void DeleteSpell()
        {
            
            if (!_deleteConfirmation)
            {
                for (int i = 0; i < CombatDatabase.ReturnSpellCount(); i++)
                {
                    if (GUILayout.Button("Delete " + CombatDatabase.ReturnSpellName(i)))
                    {
                        _deleteConfirmation = true;
                        _deleteSpellID = i;
                    }
                }
            }
            if(_deleteConfirmation)
            {
                GUILayout.Label("Are you sure you want to delete " + CombatDatabase.ReturnSpellName(_deleteSpellID) + "?");
                if(GUILayout.Button("YES"))
                {
                    CombatDatabase.DeleteSpell(_deleteSpellID);
                }
                if(GUILayout.Button("BACK"))
                {
                    _deleteSpell = false;
                    _deleteConfirmation = false;
                }

            }
        }
    }
#endif
}
