using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatSystem
{

    public class PlayerSpells
    {
        private static bool _savedSpell;
        private static bool _deleteConfirmation = false;

        private static int _deleteSpellID;

        private static Vector2 _scrollPos;

        private static string _spellName;
        private static string _spellDesc;
        private static SpellTypes _spellType;
        private static float _spellValue;
        private static float _spellManaCost;
        private static float _spellCasttime;
        private static GameObject _spellPrefab;
        private static string _spellIcon;

        private static List<string> _damageSpellNames = new List<string>();
        private static List<string> _healingSpellNames = new List<string>();
        private static List<string> _buffSpellNames = new List<string>();
        private static List<string> _debuffSpellNames = new List<string>();
        private static List<string> _abilitySpellNames = new List<string>();
        private static List<string> _allSpellIconNames = new List<string>();
        private static string _spellPrefabName;
        private static float _spellCooldown;

        private static UnityEngine.Object[] _damageSpellPrefabs;
        private static UnityEngine.Object[] _healingSpellPrefabs;
        private static UnityEngine.Object[] _buffSpellPrefabs;
        private static UnityEngine.Object[] _debuffSpellPrefabs;
        private static UnityEngine.Object[] _abilitySpellPrefabs;
        private static UnityEngine.Object[] _allSpellIcons;

        private static Abilities _abilities;
        private static DebuffAbility _debuffAbilities;

        private static int _spellIndex;
        private static int _spellIconIndex;
        private static int _abilityIndex;

        private static float _chargeRange;
        private static float _disengageDistance;
        private static float _blinkRange;
        private static float _barrierSize;

        private static bool _loadedSpells = false;

        private static int _editSpellIndex;

        public static void GetResources()
        {
            _damageSpellNames.Clear();
            _allSpellIconNames.Clear();
            CombatSystem.CombatDatabase.ClearAll();

            _damageSpellPrefabs = Resources.LoadAll("PlayerSpells/Damage");
            _healingSpellPrefabs = Resources.LoadAll("PlayerSpells/Healing");
            _buffSpellPrefabs = Resources.LoadAll("PlayerSpells/Buff");
            _debuffSpellPrefabs = Resources.LoadAll("PlayerSpells/Debuff");
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

            for (int i = 0; i < _debuffSpellPrefabs.Length; i++)
            {
                // Create a filter so we only add the GameObjects to the loadPotionsName List
                if (_debuffSpellPrefabs[i].GetType().ToString() == "UnityEngine.GameObject")
                {

                    _debuffSpellNames.Add(_debuffSpellPrefabs[i].ToString().Remove(_debuffSpellPrefabs[i].ToString().Length - 25));

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

        public static void ShowAddPlayerSpells()
        {
           // ClearAll();
            if(_savedSpell)
            {
                ClearAll();
                _savedSpell = false;
            }
            if (!_savedSpell)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                GUILayout.Label("Add a Spell", EditorStyles.boldLabel);

                _spellName = EditorGUILayout.TextField("Spell Name: ", _spellName);
                GUILayout.Label("Spell Description:");
                _spellDesc = EditorGUILayout.TextArea(_spellDesc, GUILayout.Height(50));

                EditorGUIUtility.labelWidth = 200;

                _spellType = (SpellTypes)EditorGUILayout.EnumPopup("Type of Spell:", _spellType);

                if (_spellType != SpellTypes.None)
                {

                    if (_spellType != SpellTypes.Ability)
                    {
                        _spellValue = EditorGUILayout.FloatField("Spell Value: (dmg / duration) ", _spellValue);
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
                        if(_spellType == SpellTypes.Debuff)
                        {
                            GUILayout.Space(10);
                            _debuffAbilities = (DebuffAbility)EditorGUILayout.EnumPopup("Typ of Debuff: ", _debuffAbilities);
                            GUILayout.Space(10);
                            _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _debuffSpellNames.ToArray());
                        }

                    }

                    if (_spellType == SpellTypes.Ability)
                    {
                        _abilities = (Abilities)EditorGUILayout.EnumPopup("Type of Ability: ", _abilities);

                        if (_abilities == Abilities.Charge)
                        {
                            _chargeRange = EditorGUILayout.FloatField("Charge Range: ", _chargeRange);
                            _spellValue = _chargeRange;
                        }
                        if (_abilities == Abilities.Disengage)
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
                        if (_abilities == Abilities.Blink)
                        {
                            _blinkRange = EditorGUILayout.FloatField("Blink Range: ", _blinkRange);
                            _spellValue = _blinkRange;
                        }

                        if(_abilities == Abilities.Barrier)
                        {
                            _barrierSize = EditorGUILayout.FloatField("Barrier Duration: ", _barrierSize);
                            _spellValue = _barrierSize;

                            _spellManaCost = EditorGUILayout.FloatField("Mana Cost: ", _spellManaCost);
                        }

                    }


                    GUILayout.Space(20);

                    _spellIconIndex = EditorGUILayout.Popup("Which Icon:", _spellIconIndex, _allSpellIconNames.ToArray());
                    GUILayout.Space(20);
                    if (GUILayout.Button("Save Spell"))
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
                        if(_spellType == SpellTypes.Debuff)
                        {
                            CombatSystem.CombatDatabase.AddSpell(_spellName, _spellDesc, _spellType, _spellValue, _spellManaCost, _spellCasttime, _debuffSpellNames[_spellIndex], _allSpellIconNames[_spellIconIndex], _chargeRange, _disengageDistance, _blinkRange, _debuffAbilities, _spellCooldown);
                        }

                        _spellName = "";
                        _spellDesc = "";
                        _spellValue = 0f;
                        _spellCasttime = 0f;
                        _spellManaCost = 0f;
                        _spellCooldown = 0f;

                        if (_abilities == Abilities.Disengage)
                        {
                            if (GameObject.Find("DisengageTarget") != null)
                            {
                                GameObject.Find("DisengageTarget").transform.position = new Vector3(0, 1, _disengageDistance);
                            }
                        }
                        _savedSpell = true;
                    }

                }


                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Spell has been saved");
            }
        }

        public static void ShowEditPlayerSpells()
        {
            if(_savedSpell)
            {
                _savedSpell = false;
            }
            if (!_savedSpell)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                GUILayout.Label("Edit a Spell", EditorStyles.boldLabel);

                _editSpellIndex = EditorGUILayout.Popup("Which Spell: ", _editSpellIndex, CombatDatabase.ReturnAllSpellNames().ToArray());
                if (!_loadedSpells)
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
                    _debuffAbilities = CombatDatabase.ReturnDebuffAbility(_editSpellIndex);
                    _spellCooldown = CombatDatabase.ReturnSpellCooldown(_editSpellIndex);
                    _barrierSize = CombatDatabase.ReturnSpellValue(_editSpellIndex);

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

                if (GUI.changed)
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
                        if(_spellType == SpellTypes.Debuff)
                        {
                            GUILayout.Space(10);
                            _debuffAbilities = (DebuffAbility)EditorGUILayout.EnumPopup("Typ of Debuff: ", _debuffAbilities);
                            GUILayout.Space(10);
                            _spellIndex = EditorGUILayout.Popup("Which Spell Prefab: ", _spellIndex, _debuffSpellNames.ToArray());
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
                        if(_abilities == Abilities.Barrier)
                        {
                            _barrierSize = EditorGUILayout.FloatField("Barrier Duration: ", _barrierSize);
                        }

                        _spellManaCost = EditorGUILayout.FloatField("Mana Cost: ", _spellManaCost);
                        _spellCooldown = EditorGUILayout.FloatField("Cooldown: ", _spellCooldown);

                    }
                    GUILayout.Space(20);

                    _spellIconIndex = EditorGUILayout.Popup("Which Icon:", _spellIconIndex, _allSpellIconNames.ToArray());
                    GUILayout.Space(20);
                    if (GUILayout.Button("Save Spell"))
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

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Spell has been saved");
            }
        }

        public static void ShowDeletePlayerSpells()
        {

            GUILayout.Space(50);

            if(!_loadedSpells)
            {
                CombatSystem.CombatDatabase.GetAllSpells();
                _loadedSpells = true;
            }

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
            if (_deleteConfirmation)
            {
                GUILayout.Label("Are you sure you want to delete " + CombatDatabase.ReturnSpellName(_deleteSpellID) + "?");
                if (GUILayout.Button("YES"))
                {
                    CombatDatabase.DeleteSpell(_deleteSpellID);
                }

            }
        }

        static void ClearAll()
        {
            _spellName = "";
            _spellDesc = "";
            _spellType = SpellTypes.None;
            _spellValue = 0;
            _spellManaCost = 0;
            _spellCasttime = 0;
            _spellPrefab = null;
            _spellIcon = "";
    }
    }
}