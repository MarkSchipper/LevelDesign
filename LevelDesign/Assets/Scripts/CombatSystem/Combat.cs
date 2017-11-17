using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CombatSystem
{

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

    public class Combat : MonoBehaviour
    {

        private static bool _inCombat = false;

        private static bool _startTimer;

        private static float _timer;
        private static float _spellCastTimer;
        private static bool _mayCastSpell = false;

        private static int _spellID;
        private static SpellTypes _spellType;
        private static Abilities _ability;
        private static float _spellValue;
        private static float _spellMana;
        private static float _spellCastTime;
        private static GameObject _spellPrefab;
        private static GameObject _selectedTarget;
        private static GameObject _spellCaster;
        private static float _blinkRange;
        private static float _chargeRange;
        private static float _disengageDistance;

        private static GameObject _projectile;

        public static void InitiateCombat()
        {
            _inCombat = true;
            
        }

        public static bool ReturnInCombat ()
        {
            return _inCombat;
            
        }

        public static void OutofCombat()
        {
            _inCombat = false;
            
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      SetSpell(int _id, ... )                                             //
        //                                                                                                          //
        //  Initialising the Spell, setting all the values                                                          //
        //                                                                                                          //
        //  If the given _target is not null                                                                        //
        //      If the tag of the target is EnemyRanged or EnemyMelee                                               //
        //          Set the values                                                                                  //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SetSpell(int _id, SpellTypes _type, float _value, float _manaCost, float _castTime, string _prefab, GameObject _target, GameObject _caster)
        {
            if (_target != null)
            {
                if (_target.tag == "EnemyRanged" || _target.tag == "EnemyMelee")
                {
                    _spellID = _id;
                    _spellType = _type;
                    
                    _spellValue = _value;
                    _spellMana = _manaCost;
                    _spellCastTime = _castTime;
                    
                    _spellPrefab = Resources.Load("PlayerSpells/" + _type.ToString() + "/" + _prefab + "") as GameObject;
                    _selectedTarget = _target;
                    _spellCaster = _caster;

                    
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                      SetHealingSpell(float _value, ...)                                  //
        //                                                                                                          //
        //   Set the _spellPrefab                                                                                   //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SetHealingSpell(float _value, float _manaCost, float _castTime, string _prefab)
        {
            _spellPrefab = Resources.Load("PlayerSpells/Healing/" + _prefab) as GameObject;
            _spellValue = _value;
            _spellMana = _manaCost;
            _spellCastTime = _castTime;
        }

        public static void SetAOE(float _value, string _prefab)
        {
            _spellPrefab = Resources.Load("PlayerSpells/" + _prefab) as GameObject;
            _spellValue = _value;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  CastSpell(Vector3 _playerPos)                                           //
        //                                                                                                          //
        //  The actual Spell Casting                                                                                //
        //      Again check if the _selectedTarget is not null                                                      //
        //          Set the _playerAimVector                                                                        //
        //          Instantiate the prefab                                                                          //
        //          Make sure the projectile is facing the target                                                   //
        //          AddForce to the projectile                                                                      //
        //          Add the SpellObject Component                                                                   //
        //          If the spell is from the Player then set SetFromPlayer to true                                  //
        //          Set the damage in the SpellObject                                                               //
        //          Set the SetSpellCaster to _spellCaster                                                          //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CastSpell(Vector3 _playerPos)
        {
            if (_selectedTarget != null)
            {
                    Vector3 _playerAimVector = new Vector3(_selectedTarget.transform.position.x, _selectedTarget.transform.position.y+ 1f, _selectedTarget.transform.position.z) - new Vector3(_playerPos.x, _playerPos.y + 1f, _playerPos.z);
                    _projectile = Instantiate(_spellPrefab, new Vector3(_playerPos.x, _playerPos.y + 2f, _playerPos.z), Quaternion.identity) as GameObject;

                    _projectile.transform.LookAt(_selectedTarget.transform);
                    _projectile.GetComponent<Rigidbody>().AddForce(new Vector3(_playerAimVector.x, _playerAimVector.y, _playerAimVector.z) * 1f);
                    _projectile.AddComponent<SpellObject>();
                    _projectile.GetComponent<SpellObject>().SetFromPlayer(true);
                    _projectile.GetComponent<SpellObject>().SetDamage(_spellValue);
                    _projectile.GetComponent<SpellObject>().SetSpellCaster(_spellCaster);
                
            }

        }

        public static void CastHealingSpell(Vector3 _playerPos)
        {
            _projectile = Instantiate(_spellPrefab, _playerPos, Quaternion.Euler(new Vector3(-90,0,0))) as GameObject;
            Destroy(_projectile, 2.0f); 

        }

        public static void CastAOE(Vector3 _location)
        {
            _projectile = Instantiate(_spellPrefab, _location, Quaternion.identity) as GameObject;
            _projectile.GetComponentInChildren<SpellObject>().SetDamage(_spellValue);
        }

        
    }
}