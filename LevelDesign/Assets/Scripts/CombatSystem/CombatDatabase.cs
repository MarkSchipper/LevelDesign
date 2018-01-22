using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;


namespace CombatSystem
{

    public static class CombatDatabase
    {

        // player settings
        private static int _playerHealth;
        private static int _playerMana;
        private static float _playerRunSpeed;
        private static float _playerWalkSpeed;
        private static float _playerRangedDistance;
        private static float _playerMeleeRange;

        private static List<int> _spellID = new List<int>();
        private static List<string> _spellNames = new List<string>();
        private static List<string> _spellDescriptions = new List<string>();
        private static List<SpellTypes> _spellTypes = new List<SpellTypes>();
        private static List<float> _spellValues = new List<float>();
        private static List<float> _spellCastTimes = new List<float>();
        private static List<string> _spellPrefabs = new List<string>();
        private static List<string> _spellIcons = new List<string>();
        private static List<float> _chargeRange = new List<float>();
        private static List<float> _disDistance = new List<float>();
        private static List<float> _blinkRange = new List<float>();
        private static List<float> _spellMana = new List<float>();
        private static List<Abilities> _ability = new List<Abilities>();
        private static List<DebuffAbility> _debuffAbility = new List<DebuffAbility>();
        private static List<float> _spellCooldown = new List<float>();

        private static int _playerLevel;
        private static int _playerExp;
        private static int _playerGold;
        private static int _expMultiplier;
        private static int _dmgMultiplier;
        private static int _healthMultiplier;
        private static int _manaMultiplier;
        private static int _healingMultiplier;


        public static void AddSpell(string _name, string _desc, SpellTypes _type, float _value, float _manaCost, float _casttime, string _prefab, string _icon, float _chargeRange, float _disDistance, float _blinkRange, Abilities _ability, float _cooldown)
        {

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerSpellsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO PlayerSpells (SpellName, SpellDesc, SpellType, SpellValue, SpellCasttime, SpellPrefab, SpellIcon, ChargeRange, DisEngageDistance, BlinkRange, SpellManaCost, Ability, SpellCooldown) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\")", _name, _desc, _type.ToString(), _value, _casttime, _prefab, _icon, _chargeRange, _disDistance, _blinkRange, _manaCost, _ability.ToString(), _cooldown);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;

        }

        public static void AddSpell(string _name, string _desc, SpellTypes _type, float _value, float _manaCost, float _casttime, string _prefab, string _icon, float _chargeRange, float _disDistance, float _blinkRange, DebuffAbility _ability, float _cooldown)
        {

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerSpellsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO PlayerSpells (SpellName, SpellDesc, SpellType, SpellValue, SpellCasttime, SpellPrefab, SpellIcon, ChargeRange, DisEngageDistance, BlinkRange, SpellManaCost, Ability, SpellCooldown) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\")", _name, _desc, _type.ToString(), _value, _casttime, _prefab, _icon, _chargeRange, _disDistance, _blinkRange, _manaCost, _ability.ToString(), _cooldown);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;

        }

        public static void SaveSpell(int _id, string _name, string _desc, SpellTypes _type, float _value, float _manaCost, float _casttime, string _prefab, string _icon, float _chargeRange, float _disDistance, float _blinkRange, Abilities _ability, float _cooldown)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerSpellsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE PlayerSpells SET SpellName = '" + _name + "', SpellDesc = '" + _desc + "', SpellType = '" + _type.ToString() + "', SpellValue = '" + _value + "', SpellCasttime = '" + _casttime + "', SpellPrefab = '" + _prefab + "', SpellIcon = '" + _icon + "', ChargeRange = '" + _chargeRange + "', DisEngageDistance = '" + _disDistance + "', BlinkRange = '" + _blinkRange + "', SpellManaCost = '" + _manaCost + "', Ability = '" + _ability.ToString() + "', SpellCooldown = '" + _cooldown + "' WHERE SpellID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;

            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void DeleteSpell(int _id)
        {

        }

        public static void GetAllSpells()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerSpellsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM PlayerSpells";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _spellID.Add(reader.GetInt32(0));
                _spellNames.Add(reader.GetString(1));
                _spellDescriptions.Add(reader.GetString(2));
                if (reader.GetString(3) == "Buff")
                {
                    _spellTypes.Add(SpellTypes.Buff);
                }
                if (reader.GetString(3) == "Damage")
                {
                    _spellTypes.Add(SpellTypes.Damage);
                }
                if (reader.GetString(3) == "AOE")
                {
                    _spellTypes.Add(SpellTypes.AOE);
                }
                if (reader.GetString(3) == "Healing")
                {
                    _spellTypes.Add(SpellTypes.Healing);
                }
                if (reader.GetString(3) == "Ability")
                {
                    _spellTypes.Add(SpellTypes.Ability);
                }
                if(reader.GetString(3) == "Debuff")
                {
                    _spellTypes.Add(SpellTypes.Debuff);
                }

                _spellValues.Add(reader.GetFloat(4));
                _spellCastTimes.Add(reader.GetFloat(5));
                _spellPrefabs.Add(reader.GetString(6));
                _spellIcons.Add(reader.GetString(7));
                _chargeRange.Add(reader.GetFloat(8));
                _disDistance.Add(reader.GetFloat(9));
                _blinkRange.Add(reader.GetFloat(10));
                _spellMana.Add(reader.GetInt32(11));

                if (reader.GetString(12) == "Disengage")
                {
                    _ability.Add(Abilities.Disengage);
                    _debuffAbility.Add(DebuffAbility.None);
                }
                else if (reader.GetString(12) == "Charge")
                {
                    _ability.Add(Abilities.Charge);
                    _debuffAbility.Add(DebuffAbility.None);
                }
                else if (reader.GetString(12) == "Blink")
                {
                    _ability.Add(Abilities.Blink);
                    _debuffAbility.Add(DebuffAbility.None);
                }
                else if (reader.GetString(12) == "Barrier")
                {
                    _ability.Add(Abilities.Barrier);
                    _debuffAbility.Add(DebuffAbility.None);
                }

                else if (reader.GetString(12) == "Freeze")
                {
                    _debuffAbility.Add(DebuffAbility.Freeze);
                    _ability.Add(Abilities.None);
                }
                else if (reader.GetString(12) == "Slow")
                {
                    _debuffAbility.Add(DebuffAbility.Slow);
                    _ability.Add(Abilities.None);
                }
                else
                {
                    _ability.Add(Abilities.None);
                    _debuffAbility.Add(DebuffAbility.None);
                }
                _spellCooldown.Add(reader.GetFloat(13));

                

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            
        }

        public static List<int> ReturnAllSpellID()
        {
            return _spellID;
        }

        public static List<string> ReturnAllSpellNames()
        {
            return _spellNames;
        }

        public static int ReturnSpellCount()
        {
            return _spellID.Count;
        }

        public static string ReturnSpellIcon(int _id)
        {
            return _spellIcons[_id];
        }

        public static float ReturnSpellValue(int _id)
        {
            return _spellValues[_id];
        }

        public static int ReturnSpellID(int _id)
        {
            return _spellID[_id];
        }

        public static float ReturnCastTime(int _id)
        {
            return _spellCastTimes[_id];
        }

        public static SpellTypes ReturnSpellType(int _id)
        {
            return _spellTypes[_id];
        }

        public static string ReturnSpellPrefab(int _id)
        {
            return _spellPrefabs[_id];
        }

        public static float ReturnSpellManaCost(int _id)
        {
            return _spellMana[_id];
        }

        public static string ReturnSpellName(int _id)
        {
            return _spellNames[_id];
        }

        public static string ReturnSpellDesc(int _id)
        {
            return _spellDescriptions[_id];
        }

        public static float ReturnChargeRange(int _id)
        {
            return _chargeRange[_id];
        }

        public static float ReturnDisengageDistance(int _id)
        {
            return _disDistance[_id];
        }

        public static float ReturnBlinkRange(int _id)
        {
            return _blinkRange[_id];
        }

        public static Abilities ReturnAbility(int _id)
        {
            return _ability[_id];
        }

        public static DebuffAbility ReturnDebuffAbility(int _id)
        {
            return _debuffAbility[_id];
        }

        public static float ReturnSpellCooldown(int _id)
        {
            return _spellCooldown[_id];
        }

        public static List<float> ReturnAllSpellCooldowns()
        {
            return _spellCooldown;
        }

        public static void GetPlayerSettings()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM PlayerSettings";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _playerHealth = reader.GetInt32(1);
                _playerMana = reader.GetInt32(2);
                _playerRunSpeed = reader.GetFloat(3);
                _playerWalkSpeed = reader.GetFloat(4);
                _playerRangedDistance = reader.GetFloat(5);
                _playerMeleeRange = reader.GetFloat(6);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void SavePlayerSettings(int _health, int _mana, float _run, float _walk, float _ranged, float _melee)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "UPDATE PlayerSettings SET PlayerHealth = '" + _health + "', PlayerMana = '" + _mana + "', RunSpeed = '" + _run + "', WalkSpeed = '" + _walk + "', RangedDistance = '" + _ranged + "', MeleeRange = '" + _melee + "' WHERE PlayerID = '1'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();


            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void GetPlayerData()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM PlayerStats WHERE PlayerID = '1'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                _playerLevel = reader.GetInt32(1);
                _playerExp = reader.GetInt32(2);
                _playerGold = reader.GetInt32(3);
                _expMultiplier = reader.GetInt32(4);
                _dmgMultiplier = reader.GetInt32(5);
                _healthMultiplier = reader.GetInt32(6);
                _manaMultiplier = reader.GetInt32(7);
                _healingMultiplier = reader.GetInt32(8);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void UpdatePlayerData(int _level, int _exp, int _gold, float _expM, float _dmgM, float _healthM, float _manaM, float _healingM)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE PlayerStats SET PlayerLevel = '" + _level + "', PlayerExp = '" + _exp + "', PlayerGold = '" + _gold + "', ExpMultiplier = '" + _expM + "', DamageMultiplier = '" + _dmgM + "', HealthMultiplier = '" + _healthM + "', ManaMultiplier = '" + _manaM + "', HealingMultiplier = '" + _healingM + "' WHERE PlayerID = '1'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static int ReturnPlayerHealth()
        {
            return _playerHealth;
        }

        public static int ReturnPlayerMana()
        {
            return _playerMana;
        }

        public static float ReturnPlayerRunSpeed()
        {
            return _playerRunSpeed;
        }

        public static float ReturnPlayerWalkSpeed()
        {
            return _playerWalkSpeed;
        }

        public static float ReturnPlayerRangedDistance()
        {
            return _playerRangedDistance;
        }

        public static float ReturnPlayerMeleeRange()
        {
            return _playerMeleeRange;
        }

        public static void GetPlayerStatistics()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM PlayerStats WHERE PlayerID = '1'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                _playerLevel = reader.GetInt32(1);
                _playerExp = reader.GetInt32(2);
                _playerGold = reader.GetInt32(3);
                _expMultiplier = reader.GetInt32(4);
                _dmgMultiplier = reader.GetInt32(5);
                _healthMultiplier = reader.GetInt32(6);
                _manaMultiplier = reader.GetInt32(7);
                _healingMultiplier = reader.GetInt32(8);


            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static int ReturnPlayerLevel()
        {
            return _playerLevel;
        }

        public static int ReturnPlayerExp()
        {
            return _playerExp;
        }

        public static void UpdatePlayerExp(int _exp)
        {

            _playerExp += _exp;
            if (_playerExp < _playerLevel * _expMultiplier)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "UPDATE PlayerStats SET PlayerExp = '" + _playerExp + "' WHERE PlayerID = '1'";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            else
            {
                _playerExp -= _playerLevel * _expMultiplier;
                _playerLevel += 1;
                
                PlayerLevelUp(_playerLevel, _playerExp);
                InteractionManager.instance.LevelUp();
            }

            
        }

        public static int ReturnPlayerGold()
        {
            return _playerGold;
        }

        public static int ReturnExpMultiplier()
        {
            return _expMultiplier;
        }

        public static int ReturnDamageMultiplier()
        {
            return _dmgMultiplier;
        }

        public static int ReturnHealthMultiplier()
        {
            return _healthMultiplier;
        }

        public static int ReturnManaMultiplier()
        {
            return _manaMultiplier;
        }

        public static int ReturnHealingMultiplier()
        {
            return _healingMultiplier;
        }

        public static void PlayerLevelUp(int _level, int _exp)
        {
            string connSec = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconnSec;
            dbconnSec = (IDbConnection)new SqliteConnection(connSec);
            dbconnSec.Open(); //Open connection to the database.

            IDbCommand dbcmdSec = dbconnSec.CreateCommand();

            string sqlQuerySec = String.Format("UPDATE PlayerStats SET PlayerLevel = '" + _level + "', PlayerExp = '" + _exp + "' WHERE PlayerID = '1'");
            dbcmdSec.CommandText = sqlQuerySec;
            dbcmdSec.ExecuteScalar();
            dbcmdSec.Dispose();
            dbcmdSec = null;
            dbconnSec.Close();
            dbconnSec = null;
        }

        public static void ClearAll()
        {
             _spellID.Clear();
            _spellNames.Clear();
            _spellDescriptions.Clear();
            _spellTypes.Clear();
            _spellValues.Clear();
            _spellCastTimes.Clear();
            _spellPrefabs.Clear();
            _spellIcons.Clear();
            _chargeRange.Clear();
            _disDistance.Clear();
            _blinkRange.Clear();
            _spellMana.Clear();
            _ability.Clear();
            _spellCooldown.Clear();
    }

        public static void AddGold(int _gold)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "UPDATE PlayerStats SET PlayerGold = '" + _gold + "' WHERE PlayerID = '1'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }
}
