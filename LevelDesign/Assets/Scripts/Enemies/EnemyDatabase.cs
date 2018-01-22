using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

namespace EnemyCombat
{

    public enum EnemyType
    {
        None,
        Melee,
        Ranged,
    }

    public enum EnemySpecial
    {
        None,
        Charge,
        Hook,
        Teleport,
        EarthQuake,
        Webbing,
    }

    public enum EnemyMovement
    {
        None,
        Idle,
        Patrol,
    }

    public enum EnemySpawn
    {
        None,
        Spawn,
        Placement,
    }


    public static  class EnemyDatabase
    {

        private static List<int> _enemyID = new List<int>();
        private static List<string> _enemyName = new List<string>();
        private static List<int> _enemyHealth = new List<int>();
        private static List<int> _enemyMana = new List<int>();
        private static List<EnemyType> _enemyType = new List<EnemyType>();
        private static List<float> _enemyDamage = new List<float>();
        private static List<EnemySpecial> _specialAttack = new List<EnemySpecial>();
        private static List<string> _enemyPrefab = new List<string>();
        private static List<EnemyMovement> _enemyMovement = new List<EnemyMovement>();
        private static List<int> _enemyWaypoints = new List<int>();
        private static List<float> _enemyCooldown = new List<float>();
        private static List<int> _enemyAggroRange = new List<int>();
        private static List<string> _enemyDeathFeedback = new List<string>();
        private static List<string> _enemyHitFeedback = new List<string>();
        private static List<float> _enemyAttackRange = new List<float>();
        private static List<string> _enemyRangedSpell = new List<string>();
        private static List<EnemySpawn> _enemySpawn = new List<EnemySpawn>();
        private static List<string> _enemyLootTable = new List<string>();

        public static void GetAllEnemies()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Enemies";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _enemyID.Add(reader.GetInt32(0));
                _enemyName.Add(reader.GetString(1));
                if(reader.GetString(2) == "Melee")
                {
                    _enemyType.Add(EnemyType.Melee);
                }
                if(reader.GetString(2) == "Ranged")
                {
                    _enemyType.Add(EnemyType.Ranged);
                }
                _enemyDamage.Add(reader.GetFloat(3));
                if (reader.GetString(4) == "Hook")
                {
                    _specialAttack.Add(EnemySpecial.Hook);
                }
                if (reader.GetString(4) == "Charge")
                {
                    _specialAttack.Add(EnemySpecial.Charge);
                }
                if (reader.GetString(4) == "Teleport")
                {
                    _specialAttack.Add(EnemySpecial.Teleport);
                }
                if(reader.GetString(4) == "EarthQuake")
                {
                    _specialAttack.Add(EnemySpecial.EarthQuake);
                }

                if(reader.GetString(4) == "Webbing")
                {
                    _specialAttack.Add(EnemySpecial.Webbing);
                }

                if(reader.GetString(4) == "None")
                {
                    _specialAttack.Add(EnemySpecial.None);
                }
                _enemyHealth.Add(reader.GetInt32(5));
                _enemyMana.Add(reader.GetInt32(6));
                _enemyPrefab.Add(reader.GetString(7));

                if(reader.GetString(8) == "Patrol")
                {
                    _enemyMovement.Add(EnemyMovement.Patrol);
                }
                if(reader.GetString(8) == "Idle")
                {
                    _enemyMovement.Add(EnemyMovement.Idle);
                }
                if(reader.GetString(8) == "None")
                {
                    _enemyMovement.Add(EnemyMovement.None);
                }
                _enemyWaypoints.Add(reader.GetInt32(9));
                _enemyCooldown.Add(reader.GetFloat(10));
                _enemyAggroRange.Add(reader.GetInt32(11));
                _enemyDeathFeedback.Add(reader.GetString(12));
                _enemyHitFeedback.Add(reader.GetString(13));
                _enemyAttackRange.Add(reader.GetFloat(14));

                _enemyRangedSpell.Add(reader.GetString(15));

                if(reader.GetString(16) == "None")
                {
                    _enemySpawn.Add(EnemySpawn.None);
                }
                if (reader.GetString(16) == "Spawn")
                {
                    _enemySpawn.Add(EnemySpawn.Spawn);
                }
                if (reader.GetString(16) == "Placement")
                {
                    _enemySpawn.Add(EnemySpawn.Placement);
                }
                _enemyLootTable.Add(reader.GetString(17));

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        } 

        public static void AddEnemy(string _name, int _health, int _mana, EnemyType _type, float _enemyDamage, float _cooldown ,string _special, string _prefab, EnemyMovement _enemyMovement, int _waypoints, int _aggroRange, string _death, string _hit, float _range, string _spell, EnemySpawn _spawn, string _lootTable)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Enemies (EnemyName, EnemyType, EnemyDamage, EnemySpecial, EnemyHealth, EnemyMana, EnemyPrefab, EnemyBehaviour, EnemyWaypointsAmount, EnemyCooldown, EnemyAggroRange, EnemyDeathFeedback, EnemyHitFeedback, EnemyAttackRange, EnemyRangedSpell, EnemySpawn, LootTable) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\", \"{14}\", \"{15}\", \"{16}\")", _name, _type.ToString(), _enemyDamage, _special, _health, _mana, _prefab, _enemyMovement.ToString(), _waypoints, _cooldown, _aggroRange,_death, _hit, _range, _spell, _spawn.ToString(), _lootTable);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        }

        public static List<int> ReturnAllEnemyID()
        {
            return _enemyID;
        }

        public static List<string> ReturnAllEnemyNames()
        {
            return _enemyName;
        }

        public static int ReturnEnemyID(int _id)
        {
            return _enemyID[_id];
        }

        public static string ReturnEnemyName(int _id)
        {
            return _enemyName[_id];
        }

        public static int ReturnEnemyHealth(int _id)
        {
            return _enemyHealth[_id];
        }

        public static int ReturnEnemyMana(int _id)
        {
            return _enemyMana[_id];
        }

        public static EnemyType ReturnEnemyType(int _id)
        {
            return _enemyType[_id];
        }

        public static float ReturnEnemyDamage(int _id)
        {
            return _enemyDamage[_id];
        }

        public static string ReturnEnemyRangedSpell(int _id)
        {
            return _enemyRangedSpell[_id];
        }

        public static EnemySpecial ReturnSpecial(int _id)
        {
            return _specialAttack[_id];
        }

        public static string ReturnEnemyPrefab(int _id)
        {
            return _enemyPrefab[_id];
        }

        public static EnemyMovement ReturnEnemyMovement(int _id)
        {
            return _enemyMovement[_id];
        }

        public static int ReturnEnemyWaypoint(int _id)
        {
            return _enemyWaypoints[_id];
        }

        public static float ReturnEnemyCooldown(int _id)
        {
            return _enemyCooldown[_id];
        }

        public static int ReturnEnemyAggroRange(int _id)
        {
            return _enemyAggroRange[_id];
        }

        public static string ReturnEnemyDeathFeedback(int _id)
        {
            return _enemyDeathFeedback[_id];
        }

        public static string ReturnEnemyHitFeedback(int _id)
        {
            return _enemyHitFeedback[_id];
        }

        public static float ReturnEnemyAttackRange(int _id)
        {
            return _enemyAttackRange[_id];
        }

        public static EnemySpawn ReturnEnemySpawn(int _id)
        {
            return _enemySpawn[_id];
        }

        public static string ReturnEnemyLootTable(int _id)
        {
            return _enemyLootTable[_id];
        }

        // update

        public static void UpdateEnemy(int _id, string _name, int _health, int _mana, EnemyType _type, float _enemyDamage, float _cooldown, string _special, string _prefab, EnemyMovement _enemyMovement, int _waypoints, string _death, string _hit, float _range, string _spell, EnemySpawn _spawn, string _lootTable)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Enemies SET EnemyName = '" + _name + "', EnemyType = '" + _type.ToString() + "', EnemyDamage = '" + _enemyDamage + "', EnemySpecial = '" + _special + "', EnemyHealth = '" + _health + "', EnemyMana = '" + _mana + "', EnemyPrefab = '" + _prefab + "', EnemyBehaviour = '" + _enemyMovement + "', EnemyWaypointsAmount = '" + _waypoints + "', EnemyCooldown = '" + _cooldown + "', EnemyDeathFeedback = '" + _death + "', EnemyHitFeedback = '" + _hit + "', EnemyAttackRange = '" + _range + "', EnemyRangedSpell = '" + _spell + "', EnemySpawn = '" + _spawn + "', LootTable = '" + _lootTable + "' WHERE EnemyID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        }


        // specific enemy

        public static EnemyMovement ReturnMovement(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT EnemyBehaviour FROM Enemies WHERE EnemyID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;

            if(_tmp.ToString() == "Patrol")
            {
                return EnemyMovement.Patrol;
            }
            if(_tmp.ToString() == "Idle")
            {
                return EnemyMovement.Idle;
            }

            else
            {
                return EnemyMovement.None;
            }

        }

        public static int ReturnWaypoints(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT EnemyWaypointsAmount FROM Enemies WHERE EnemyID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;

            return int.Parse(_tmp.ToString());

        }

        public static void DeleteEnemy(int _id)
        {
            Debug.Log("deleting " + _id);
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/EnemyDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("DELETE FROM Enemies WHERE EnemyID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        // Clear all lists

        public static void ClearLists()
        {
            _enemyID.Clear();
            _enemyName.Clear();
            _enemyHealth.Clear();
            _enemyMana.Clear();
            _enemyType.Clear();
            _enemyDamage.Clear();
            _specialAttack.Clear();
            _enemyPrefab.Clear();
            _enemyMovement.Clear();
            _enemyWaypoints.Clear();
            _enemyCooldown.Clear();
            _enemyAggroRange.Clear();
            _enemyDeathFeedback.Clear();
            _enemyHitFeedback.Clear();
            _enemyAttackRange.Clear();
            _enemyRangedSpell.Clear();
            _enemySpawn.Clear();
        }

    }
}