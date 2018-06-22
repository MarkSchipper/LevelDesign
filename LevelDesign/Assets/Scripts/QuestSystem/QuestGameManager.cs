using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace Quest
{

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                  QuestGameManager                                            //
    //                                                                                                              //
    //  QuestGameManager handles are databases queries related to quest in game                                     //
    //                                                                                                              //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class QuestGameManager : ScriptableObject
    {
        private static int _questID;
        private static string _questText;
        private static string _questTitle;
        private static string _questCompleteText;
        private static int _currentQuestExp;
        private static int _tmpID;
        private static bool _hasFollowUpQuest;

        // Lists for editing the quests
        private static List<int> _allQuestID = new List<int>();
        private static List<string> _allQuestTitles = new List<string>();
        private static List<string> _allQuestTexts = new List<string>();
        private static List<QuestType> _allQuestsType = new List<QuestType>();
        private static List<string> _allQuestItems = new List<string>();
        private static List<QuestItemAmount> _allQuestAmountType = new List<QuestItemAmount>();
        private static List<int> _allQuestAmount = new List<int>();
        private static List<string> _allQuestMobs = new List<string>();
        private static List<bool> _allQuestActive = new List<bool>();
        private static List<bool> _allQuestCompleted = new List<bool>();
        private static List<string> _allQuestZones = new List<string>();
        private static List<bool> _allQuestZoneAutoComplete = new List<bool>();
        private static List<int> _allQuestNpcID = new List<int>();
        private static List<int> _allQuestCollected = new List<int>();
        private static List<string> _allQuestCompleteText = new List<string>();
        private static List<QuestReward> _allQuestRewards = new List<QuestReward>();
        private static List<int> _allQuestGold = new List<int>();
        private static List<int> _allQuestExp = new List<int>();
        private static List<string> _allQuestItemReward = new List<string>();
        private static List<int> _allQuestFollowupID = new List<int>();
        private static List<bool> _allQuestEnabled = new List<bool>();
        private static List<QuestChain> _allQuestChain = new List<QuestChain>();
        private static List<QuestChainType> _allQuestChainType = new List<QuestChainType>();
        private static List<int> _allQuestChainFollowupID = new List<int>();

        private static List<int> _questItemID = new List<int>();
        private static List<string> _questItemNames = new List<string>();
        private static List<string> _questItemPrefabs = new List<string>();
        private static List<int> _activeQuestID = new List<int>();
        private static List<string> _activeQuestTitle = new List<string>();
        private static List<QuestType> _activeQuestTypes = new List<QuestType>();

        private static List<int> _npcQuestID = new List<int>();

        public static void GetAllQuests()
        {
           
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Quests";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _allQuestID.Add(reader.GetInt32(0));
                _allQuestTitles.Add(reader.GetString(1));
                _allQuestTexts.Add(reader.GetString(2));
                if (reader.GetString(3) == "Collect")
                {
                    _allQuestsType.Add(QuestType.Collect);
                }
                if (reader.GetString(3) == "Explore")
                {
                    _allQuestsType.Add(QuestType.Explore);
                }

                if (reader.GetString(3) == "Kill")
                {
                    _allQuestsType.Add(QuestType.Kill);
                }
                _allQuestItems.Add(reader.GetString(4));
                if (reader.GetInt32(5) > 1)
                {
                    _allQuestAmountType.Add(QuestItemAmount.Multiple);

                    _allQuestAmount.Add(reader.GetInt32(5));
                }
                else
                {
                    _allQuestAmountType.Add(QuestItemAmount.Single);
                    _allQuestAmount.Add(reader.GetInt32(5));
                }
                _allQuestMobs.Add(reader.GetString(6));
                Debug.Log(reader.GetString(6));
                if (reader.GetString(7) == "True")
                {
                    _allQuestActive.Add(true);
                }
                else
                {
                    _allQuestActive.Add(false);
                }
                if (reader.GetString(8) == "True")
                {
                    _allQuestCompleted.Add(true);
                }
                else
                {
                    _allQuestCompleted.Add(false);
                }
                _allQuestZones.Add(reader.GetString(9));
                if (reader.GetString(10) == "True")
                {
                    _allQuestZoneAutoComplete.Add(true);
                }
                else
                {
                    _allQuestZoneAutoComplete.Add(false);
                }
                _allQuestNpcID.Add(reader.GetInt32(11));
                _allQuestCollected.Add(reader.GetInt32(12));
                _allQuestCompleteText.Add(reader.GetString(13));

                if (reader.GetInt32(14) > 0 && reader.GetInt32(15) == 0)
                {
                    _allQuestRewards.Add(QuestReward.Gold);
                }

                if (reader.GetInt32(14) == 0 && reader.GetInt32(15) > 0)
                {
                    _allQuestRewards.Add(QuestReward.Experience);
                }
                if (reader.GetInt32(14) > 0 && reader.GetInt32(15) > 0)
                {
                    _allQuestRewards.Add(QuestReward.Both);
                }


                _allQuestGold.Add(reader.GetInt32(14));
                _allQuestExp.Add(reader.GetInt32(15));
                _allQuestItemReward.Add(reader.GetString(16));
                _allQuestFollowupID.Add(reader.GetInt32(17));
                if (reader.GetString(18) == "True")
                {
                    _allQuestEnabled.Add(true);
                }
                else
                {
                    _allQuestEnabled.Add(false);
                }

                if (reader.GetString(19) == "Single")
                {

                    _allQuestChain.Add(QuestChain.Single);
                }
                if (reader.GetString(19) == "Chain")
                {
                    _allQuestChain.Add(QuestChain.Chain);
                }
                if (reader.GetString(20) == "Start")
                {
                    _allQuestChainType.Add(QuestChainType.Start);
                }
                if (reader.GetString(20) == "Followup")
                {
                    _allQuestChainType.Add(QuestChainType.Followup);
                }
                if (reader.GetString(20) == "End")
                {
                    _allQuestChainType.Add(QuestChainType.End);
                }
                _allQuestChainFollowupID.Add(reader.GetInt32(21));

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void GetAllActiveQuests()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Quests WHERE QuestActive = 'True' AND QuestEnabled = 'True'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _activeQuestID.Add(reader.GetInt32(0));
                _activeQuestTitle.Add(reader.GetString(1));

                if (reader.GetString(3) == "Collect")
                {
                    _activeQuestTypes.Add(QuestType.Collect);

                }
                if (reader.GetString(3) == "Explore")
                {
                    _activeQuestTypes.Add(QuestType.Explore);
                }
                if (reader.GetString(3) == "Kill")
                {
                    _activeQuestTypes.Add(QuestType.Kill);
                }
                if (reader.GetString(3) == "None")
                {
                    _activeQuestTypes.Add(QuestType.None);
                }
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void AcceptQuest(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestActive = 'True' WHERE QuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            Quest.QuestLog.UpdateLog();
        }
       
        public static float ReturnCurrentQuestExp()
        {
            return _currentQuestExp;
        }

        public static void GetQuestsFromNpc(int _npcID)
        {
            ClearAll();

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestID FROM Quests WHERE NPC_ID = '" + _npcID + "' AND QuestEnabled = 'True'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _npcQuestID.Add(reader.GetInt32(0));
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void FinishQuest(int _questID)
        {

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("Select QuestExp FROM Quests WHERE QuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();
            CheckFollowupQuest(_questID);
            _currentQuestExp = int.Parse(_tmp.ToString());

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            //
            string connSec = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconnSec;
            dbconnSec = (IDbConnection)new SqliteConnection(connSec);
            dbconnSec.Open(); //Open connection to the database.

            IDbCommand dbcmdSec = dbconnSec.CreateCommand();

            string sqlQuerySec = String.Format("UPDATE Quests SET QuestEnabled = 'False', QuestActive = 'False' WHERE QuestID = '" + _questID + "'");
            dbcmdSec.CommandText = sqlQuerySec;
            dbcmdSec.ExecuteScalar();
            dbcmdSec.Dispose();
            dbcmdSec = null;
            dbconnSec.Close();
            dbconnSec = null;

            //Quest.QuestLog.ClearAll();

            CombatSystem.PlayerController.instance.CompletedQuest();
            //NPCSystem.NPC.PlayerHasFinishedQuest();
            Quest.QuestLog.UpdateLog();
        }

        static void CheckFollowupQuest(int _id)
        {

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT QuestID FROM Quests WHERE QuestChainFollowupID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            if (_tmp != null)
            {
                if (int.Parse(_tmp.ToString()) != _id)
                {
                    if (int.Parse(_tmp.ToString()) > 0)
                    {
                        SetFollowupQuestActive(int.Parse(_tmp.ToString()));
                        _hasFollowUpQuest = true;
                    }
                }
            }
            else
            {
                _hasFollowUpQuest = false;
                FinishedQuestUpdateNPC(_id);
            }

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static bool ReturnHasFollowUpQuest()
        {
            return _hasFollowUpQuest;
        }

        static void SetFollowupQuestActive(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestEnabled = 'True' WHERE QuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;

            NPC.NpcSystem[] _allNPC = GameObject.FindObjectsOfType<NPC.NpcSystem>();

            for (int i = 0; i < _allNPC.Length; i++)
            {
                _allNPC[i].CheckForQuest();
            }

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        static void FinishedQuestUpdateNPC(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Actors SET ActorQuestgiver = 'False ' WHERE ActorQuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;


            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static bool ReturnEnemyKillQuest(string _name)
        {
            bool test = false;
            Debug.Log(_allQuestMobs.Count);
            for (int j = 0; j < _allQuestMobs.Count; j++)
            {
                Debug.Log(_allQuestMobs[j]);
                if (_allQuestMobs[j] == _name)
                {
                    _questID = _allQuestID[j];
                    CheckKillQuestComplete(j);
                    _tmpID = j;
                    test = true;
                    break;
                }
                else
                {
                    test = false;
                }
               
            }
            Debug.Log(test);
            return test;
        }

        public static void CheckKillQuestComplete(int _id)
        {
            Debug.Log(_allQuestAmount[_id] + " - " + _allQuestCollected[_id]);
            if (_allQuestAmount[_id] == ReturnQuestItemsCollected(_questID))
            {
                SetQuestComplete(_questID);
                Quest.QuestLog.UpdateLog();
            }
        }

        public static void UpdateEnemyKillQuest()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestCollected = QuestCollected + 1 WHERE QuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            CheckKillQuestComplete(_tmpID);
        }

        public static void SetQuestComplete(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestComplete = 'True' WHERE QuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            //   NPCSystem.NPC.PlayerHasFinishedQuest();
        }

        public static bool NPCHasNewQuest(int _npcID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT QuestActive FROM Quests WHERE NPC_ID = '" + _npcID + "' AND QuestEnabled = 'True' AND QuestComplete = 'False'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                if (_tmp.ToString() == "False")
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

        public static bool ReturnQuestActive(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT QuestActive FROM Quests WHERE QuestID = '" + _questID + "' AND QuestEnabled = 'True'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            if (_tmp != null)
            {
                if (_tmp.ToString() == "True")
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

        public static int ReturnQuestItemsCollected(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestCollected FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return int.Parse(_tmp.ToString());
        }

        public static int ReturnQuestAmount(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestAmount FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return int.Parse(_tmp.ToString());
        }

        public static string ReturnQuestTitleByID(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestTitle FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return _tmp.ToString();
        }

        public static string ReturnQuestTextByID(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestText FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return _tmp.ToString();
        }

        public static string ReturnQuestCompletedTextByID(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestCompletedText FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return _tmp.ToString();
        }

        public static void AddQuestItemCollected(int _id, int _amount)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestCollected = '" + _amount + "' WHERE QuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static List<int> ReturnNpcQuestID()
        {
            return _npcQuestID;
        }

        public static List<int> ReturnActiveQuestID()
        {
            return _activeQuestID;
        }

        public static List<string> ReturnActiveQuestTitle()
        {
            return _activeQuestTitle;
        }

        public static List<QuestType> ReturnActiveQuestType()
        {
            return _activeQuestTypes;
        }

        public static bool ReturnQuestComplete(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestComplete FROM Quests WHERE QuestID = '" + _id + "' AND QuestActive = 'True' AND QuestEnabled = 'True'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                return bool.Parse(_tmp.ToString());
            }
            else {
                return false;
            }
        }

        public static bool ReturnQuestCompleted(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestComplete FROM Quests WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                return bool.Parse(_tmp.ToString());
            }
            else {
                return false;
            }
        }

        public static List<int> ReturnQuestID()
        {
            return _allQuestID;
        }

        public static bool ReturnZoneAutoComplete(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestZoneAutoComplete FROM Quests WHERE QuestID = '" + _id + "' AND QuestActive = 'True'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                return bool.Parse(_tmp.ToString());
            }
            else {
                return false;
            }
        }

        public static void ClearAll()
        {
            _questItemID.Clear();
            _questItemNames.Clear();
            _questItemPrefabs.Clear();
            _allQuestID.Clear();
            _allQuestTitles.Clear();
            _allQuestTexts.Clear();
            _allQuestsType.Clear();
            _allQuestItems.Clear();
            _allQuestAmountType.Clear();
            _allQuestAmount.Clear();
            _allQuestMobs.Clear();
            _allQuestActive.Clear();
            _allQuestCompleted.Clear();
            _allQuestZones.Clear();
            _allQuestZoneAutoComplete.Clear();
            _allQuestNpcID.Clear();
            _allQuestCollected.Clear();
            _allQuestCompleteText.Clear();
            _allQuestRewards.Clear();
            _allQuestGold.Clear();
            _allQuestExp.Clear();
            _allQuestItemReward.Clear();
            _allQuestFollowupID.Clear();
            _allQuestEnabled.Clear();
            _activeQuestID.Clear();
            _activeQuestTitle.Clear();
            _activeQuestTypes.Clear();
            _allQuestChainFollowupID.Clear();
            _allQuestChain.Clear();
            _npcQuestID.Clear();
        }

    }
}
