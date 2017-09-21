using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;


namespace Quest
{
    public enum QuestType
    {
        None,
        Collect,
        Kill,
        Explore,
    }

    public enum QuestItemAmount
    {
        None,
        Single,
        Multiple,
    }

    public enum QuestReward
    {
        None,
        Gold,
        Experience,
        Both,

    }

    public enum QuestChain
    {
        Single,
        Chain,
    }

    public enum QuestChainType
    {
        Start,
        Followup,
        End,
    }

    public class QuestDatabase
    {

        private static List<int> _questItemID = new List<int>();
        private static List<string> _questItemNames = new List<string>();
        private static List<string> _questItemPrefabs = new List<string>();

        private static List<int> _actorID = new List<int>();
        private static List<string> _actorNames = new List<string>();

        private static int _questID;
        private static string _questText;
        private static string _questTitle;
        private static string _questCompleteText;
        private static int _currentQuestExp;

        private static int _lastQuestID;

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

        // Lists for all the ACTIVE quests
        private static List<int> _activeQuestID = new List<int>();
        private static List<string> _activeQuestTitle = new List<string>();
        private static List<QuestType> _activeQuestTypes = new List<QuestType>();

        private static List<int> _questReward = new List<int>();

        private static int _tmpID;

        private static bool _hasFollowUpQuest;

        // Fetch ALL quests regardless of Active or Enabled
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

                if (reader.GetString(19) == "Single") {

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

        public static void GetAllQuestsNyNPC(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Quests WHERE NPC_ID = '" + _id + "'";
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

        public static bool ReturnNpcHasQuests(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestID FROM Quests WHERE NPC_ID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            if(_tmp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int ReturnAllQuestsCount()
        {
            return _allQuestID.Count;
        }

        // Called to get the QuestID when Editing a Quest
        public static int GetQuestID(int _id)
        {
            return _allQuestID[_id];
        }

        // Called to get ALL Quest Titles when Editing a Quest
        public static List<string> ReturnQuestTitles()
        {
            return _allQuestTitles;
        }

        // Called to get a specific Title based on the ID given from the QuestSystem
        public static string GetQuestTitle(int _id)
        {
            return _allQuestTitles[_id];
        }

        // Called to get a specific Text based on the ID given from the QuestSystem
        public static string GetQuestText(int _id)
        {
            if (_allQuestTexts[_id] != null)
            {
                return _allQuestTexts[_id];
            }
            else
            {
                return String.Empty;
            }
        }

        public static string GetQuestCompletedText(int _id)
        {
            return _allQuestCompleteText[_id];
        }

        // Called to get a specific QuestType based on the ID given from the QuestSystem
        public static QuestType GetQuestType(int _id)
        {
            return _allQuestsType[_id];
        }

        // Called to get a specific QuestItem based on the ID given from the QuestSystem
        public static string GetQuestItem(int _id)
        {
            return _allQuestItems[_id];
        }

        public static QuestItemAmount GetAllQuestAmountTypes(int _id)
        {
            return _allQuestAmountType[_id];
        }

        public static int GetAllQuestAmounts(int _id)
        {
            return _allQuestAmount[_id];
        }

        public static string GetAllQuestZones(int _id)
        {
            return _allQuestZones[_id];
        }

        public static bool GetAllQuestZonesAutoComplete(int _id)
        {
            return _allQuestZoneAutoComplete[_id];
        }

        public static QuestReward GetQuestReward(int _id)
        {
            return _allQuestRewards[_id];
        }

        public static int GetQuestGold(int _id)
        {
            return _allQuestGold[_id];
        }

        public static int GetQuestExp(int _id)
        {
            return _allQuestExp[_id];
        }

        public static bool GetQuestEnabled(int _id)
        {
            return _allQuestEnabled[_id];
        }

        public static int GetNpcIdFromQuest(int _id)
        {
            return _allQuestNpcID[_id];
        }

        public static QuestChain GetQuestChain(int _id)
        {
            return _allQuestChain[_id];
        }
        public static QuestChainType GetQuestChainType(int _id)
        {
            return _allQuestChainType[_id];
        }

        public static int GetQuestChainFollowupID(int _id)
        {
            int _returnValue = 0;
            for (int i = 0; i < _allQuestChainFollowupID.Count; i++)
            {
                if (_allQuestChainFollowupID[i] == _id)
                {
                    _returnValue = i;
                }

            }
            return _returnValue;

        }

        // Get all QuestItems from the quest database
        public static void GetQuestItems()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM Items WHERE ItemType = 'QuestItem'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _questItemID.Add(reader.GetInt32(0));
                _questItemNames.Add(reader.GetString(1));
                _questItemPrefabs.Add(reader.GetString(6));
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        // Get all the Quest Item Names
        public static List<string> ReturnQuestItemNames()
        {
            return _questItemNames;
        }

        public static string ReturnQuestItemName(int _id)
        {
            return _questItemNames[_id];
        }

        public static string ReturnQuestItemPrefab(int _id)
        {
            return _questItemPrefabs[_id];
        }

        public static void GetAllNpcs()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT ActorID, ActorName " + "FROM Actors";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _actorID.Add(reader.GetInt32(0));
                _actorNames.Add(reader.GetString(1));

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static List<int> ReturnActorID()
        {
            return _actorID;
        }

        public static List<string> ReturnActorNames()
        {
            return _actorNames;
        }

        public static string ReturnActorName(int _id)
        {
            return _actorNames[_id];
        }

        public static int ReturnActorID(int _id)
        {
            return _actorID[_id];
        }

        //////////////////////////////////////////////////////////
        //                                                      //
        //               SAVING TO THE DATABASE                 //
        //                                                      //
        //////////////////////////////////////////////////////////

        public static void AddQuest(string _title, string _text, QuestType _type, string _item, int _amount, string _mob, bool _active, bool _complete, string _zone, bool _zoneAutoComplete, int _npcID, int _collected, string _completeText, int _gold, int _exp, string _itemReward, int _followup, bool _enabled, string _chain, string _chainType, int _chainFollowupID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Quests (QuestTitle, QuestText, QuestType, QuestItem, QuestAmount, QuestMob, QuestActive, QuestComplete, QuestZone, QuestZoneAutoComplete, NPC_ID, QuestCollected, QuestCompletedText, QuestGold, QuestExp, QuestRewardItem, FollowupID, QuestEnabled, QuestChain, QuestChainType, QuestChainFollowupID) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\", \"{14}\", \"{15}\", \"{16}\", \"{17}\", \"{18}\", \"{19}\", \"{20}\")", _title, _text, _type.ToString(), _item, _amount, _mob.ToString(), _active.ToString(), _complete.ToString(), _zone, _zoneAutoComplete.ToString(), _npcID, _collected, _completeText, _gold, _exp, _itemReward, _followup, _enabled.ToString(), _chain, _chainType, _chainFollowupID);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbcmd = dbconn.CreateCommand();

            string _nwSqlQuery = String.Format("SELECT last_insert_rowid()");
            dbcmd.CommandText = _nwSqlQuery;
            System.Object _temp = dbcmd.ExecuteScalar();
            _lastQuestID = int.Parse(_temp.ToString());
            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        }

        public static void SaveQuest(int _id, string _title, string _text, QuestType _type, string _item, int _amount, string _mob, bool _active, bool _complete, string _zone, bool _zoneAutoComplete, int _npcID, int _collected, string _completeText, int _gold, int _exp, string _itemReward, int _followup, bool _enabled, string _chain, string _chainType, int _chainFollowupID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestTitle = '" + _title + "', QuestText = '" + _text + "', QuestType = '" + _type.ToString() + "', QuestItem = '" + _item + "', QuestAmount = '" + _amount + "', QuestMob = '" + _mob + "', QuestActive = '" + _active.ToString() + "', QuestComplete = '" + _complete + "', QuestZone = '" + _zone + "', QuestZoneAutoComplete = '" + _zoneAutoComplete.ToString() + "', NPC_ID = '" + _npcID + "', QuestCollected = '" + _collected + "', QuestCompletedText = '" + _completeText + "', QuestGold = '" + _gold + "', QuestExp = '" + _exp + "', QuestRewardItem = '" + _itemReward + "', FollowupID = '" + _followup + "', QuestEnabled = '" + _enabled + "', QuestChain = '" + _chain + "', QuestChainType = '" + _chainType + "', QuestChainFollowupID = '" + _chainFollowupID + "' WHERE QuestID = " + _id);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

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

        static void FinishedQuestUpdateNPC(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Actors SET ActorQuestgiver = 'False ' WHERE ActorQuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();
            
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
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
            System.Object _tmp = dbcmd.ExecuteScalar();

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

        public static int ReturnCurrentQuestExp()
        {
            return _currentQuestExp;
        }

        public static void UpdatePlayerStats(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/PlayerStatsDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE PlayerStats SET QuestEnabled = 'False', QuestActive = 'False' WHERE QuestID = '" + _questID + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;


        }

        public static void UpdateNPC(int _npcID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Actors SET ActorQuestGiver = 'True', ActorQuestID = '" + _lastQuestID + "' WHERE ActorID = '" + _npcID + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void GetQuestFromNpc(int _npcID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestID, QuestTitle, QuestText, QuestCompletedText " + "FROM Quests WHERE NPC_ID = '" + _npcID + "' AND QuestEnabled = 'True'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _questID = reader.GetInt32(0);
                _questTitle = reader.GetString(1);
                _questText = reader.GetString(2);
                _questCompleteText = reader.GetString(3);

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static bool GetActiveFromNPC(int _npID)
        {

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT QuestActive FROM Quests WHERE NPC_ID = '" + _npID + "' AND QuestEnabled = 'True'");
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

        public static bool GetNPCQuestGiver(int _npcID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("SELECT ActorQuestgiver FROM Actors WHERE ActorID = '" + _npcID + "'");
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return bool.Parse(_tmp.ToString());
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

        // Return the LAST inserted QuestID
        public static int ReturnLastQuestID()
        {
            return _lastQuestID;
        }

        public static int ReturnQuestID()
        {
            return _questID;
        }

        public static string ReturnQuestTitle()
        {
            return _questTitle;
        }

        public static string ReturnQuestText()
        {
            return _questText;
        }

        public static string ReturnQuestCompleteText()
        {
            return _questCompleteText;
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

        public static List<int> ReturnActiveQuestIDS()
        {
            return _activeQuestID;
        }

        public static List<string> ReturnActiveQuestTitles()
        {
            return _activeQuestTitle;
        }

        public static int ReturnActiveQuestID(int _id)
        {
            return _activeQuestID[_id];
        }

        public static string ReturnActiveQuestTitle(int _id)
        {
            return _activeQuestTitle[_id];
        }

        public static int ReturnActiveQuestCount()
        {
            return _activeQuestID.Count;

        }

        public static QuestType ReturnActiveQuestType(int _id)
        {

            return _activeQuestTypes[_id];


        }

        public static void ClearAll()
        {
            _questItemID.Clear();
            _questItemNames.Clear();
            _questItemPrefabs.Clear();
            _actorID.Clear();
            _actorNames.Clear();
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
        }

        public static void UpdateActiveQuests(int _id, bool _state)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestActive = '" + _state + "' WHERE QuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void DeleteQuests(int _id, bool _state)
        {
            if (_state)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.

                IDbCommand dbcmd = dbconn.CreateCommand();

                string sqlQuery = String.Format("DELETE FROM Quests WHERE QuestID = '" + _id + "'");
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                DeleteQuestUpdateNPC(_id);
            }
        }

        static void DeleteQuestUpdateNPC(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Actors SET ActorQuestgiver = 'False', ActorQuestID = '0' WHERE ActorQuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static bool CheckQuestCompleteNpc(int _npcID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestComplete FROM Quests WHERE NPC_ID = '" + _npcID + "' AND QuestActive = 'True' AND QuestEnabled = 'True'";
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
            else
            {
                return false;
            }
        }

        public static int GetQuestItemsCollected(int _id)
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

        public static int GetQuestAmount(int _id)
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

        public static void UpdateQuestZone(GameObject _zone, int _id)
        {
            GameObject.Find(_zone.name).GetComponent<Zone>().SetQuestID(_id);
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

        public static bool CheckQuestCompleteByID(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestComplete FROM Quests WHERE QuestID = '" + _id + "' AND QuestActive = 'True'";
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

        public static List<int> ReturnQuestRewards(int _questID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestGold, QuestExp FROM Quests WHERE QuestID = '" + _questID + "' AND QuestActive = 'True'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                _questReward.Add(reader.GetInt32(0));
                _questReward.Add(reader.GetInt32(1));
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;



            return _questReward;
        }

        public static void ResetQuestChain(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "UPDATE Quests SET QuestActive = 'False', QuestComplete = 'False', QuestCollected = '0', QuestEnabled = 'True' WHERE QuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            ResetFollowupQuest(_id);
        }
        private static void ResetFollowupQuest(int _id)
        {
            Debug.Log(_id);
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database. 
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "UPDATE Quests SET QuestActive = 'False', QuestComplete = 'False', QuestCollected = '0', QuestEnabled = 'False' WHERE QuestChainFollowupID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            //dbcmd.Dispose();

            string sqlQueryTwo = "SELECT QuestID FROM Quests WHERE QuestChainFollowupID = '" + _id + "'";
            dbcmd.CommandText = sqlQueryTwo;
            System.Object _tmp = dbcmd.ExecuteScalar();

            if (_tmp != null)
            {
                ResetFollowupQuest(int.Parse(_tmp.ToString()));
            }

            dbcmd.Dispose();

            dbcmd = null;
            dbconn.Close();
            dbconn = null;

        }

        public static void ResetQuest(int _id, bool _state, int npcID)
        {
            if (_state)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery;
                if (npcID != -1)
                {
                    sqlQuery = "UPDATE Quests SET QuestActive = 'False', QuestComplete = 'False',  QuestCollected = '0', QuestEnabled = 'True' WHERE QuestID = '" + _id + "'";
                }
                else
                {
                    sqlQuery = "UPDATE Quests SET QuestActive = 'True', QuestComplete = 'False',  QuestCollected = '0', QuestEnabled = 'True' WHERE QuestID = '" + _id + "'";
                }
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                ResetQuestUpdateNPC(_id);
            }
        }

        static void ResetQuestUpdateNPC(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ActorDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "UPDATE Actors SET ActorQuestgiver = 'True' WHERE ActorQuestID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static bool ReturnEnemyKillQuest(string _name)
        {

            for (int i = 0; i < _allQuestMobs.Count; i++)
            {

                if (_allQuestMobs[i] == _name)
                {
                    _questID = _allQuestID[i];
                    //CheckKillQuestComplete(i);
                    _tmpID = i;
                    return true;
                    
                }
                else
                {
                    return false;
                }
            }
            return false;
            
        }

        public static void CheckKillQuestComplete(int _id)
        {
            ClearAll();
            GetAllQuests();

            Debug.Log(_allQuestAmount[_id] + " - " + _allQuestCollected[_id]);

            if(_allQuestAmount[_id] == _allQuestCollected[_id])
            {
                
                SetQuestComplete(_questID);
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

        public static void SaveQuestFromNode(int _id, string _title, string _text, string _complete)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestTitle = '" + _title + "', QuestText = '" + _text + "', QuestCompletedText = '" + _complete + "'  WHERE QuestID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }
}
