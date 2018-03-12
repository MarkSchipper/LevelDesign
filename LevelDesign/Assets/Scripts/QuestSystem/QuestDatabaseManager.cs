using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace Quest {

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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                              QuestDatabaseManager                                            //
    //                                                                                                              //
    //  Editor class                                                                                                //
    //      Used to create, edit and delete quests in the window editor                                             //
    //                                                                                                              //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class QuestDatabaseManager : ScriptableObject {

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

        private static List<int> _questItemID = new List<int>();
        private static List<string> _questItemNames = new List<string>();
        private static List<string> _questItemPrefabs = new List<string>();

        private static List<int> _actorID = new List<int>();
        private static List<string> _actorNames = new List<string>();
        // Lists for all the ACTIVE quests
        private static List<int> _activeQuestID = new List<int>();
        private static List<string> _activeQuestTitle = new List<string>();
        private static List<QuestType> _activeQuestTypes = new List<QuestType>();

        //////////////////////////////////////////////////////////////////////////////////////
        //                              Add Quest to the Database                           //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        public static void AddQuest(string _title, string _text, QuestType _type, string _item, int _amount, string _mob, bool _active, bool _complete, string _zone, bool _zoneAutoComplete, int _collected, string _completeText, int _gold, int _exp, string _itemReward, int _followup, bool _enabled, string _chain, string _chainType, int _chainFollowupID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Quests (QuestTitle, QuestText, QuestType, QuestItem, QuestAmount, QuestMob, QuestActive, QuestComplete, QuestZone, QuestZoneAutoComplete, NPC_ID, QuestCollected, QuestCompletedText, QuestGold, QuestExp, QuestRewardItem, FollowupID, QuestEnabled, QuestChain, QuestChainType, QuestChainFollowupID) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\", \"{14}\", \"{15}\", \"{16}\", \"{17}\", \"{18}\", \"{19}\", \"{20}\")", _title, _text, _type.ToString(), _item, _amount, _mob.ToString(), _active.ToString(), _complete.ToString(), _zone, _zoneAutoComplete.ToString(), -1, _collected, _completeText, _gold, _exp, _itemReward, _followup, _enabled.ToString(), _chain, _chainType, _chainFollowupID);
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

        //////////////////////////////////////////////////////////////////////////////////////
        //                              Update Quest in the Database                        //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        public static void SaveQuest(int _id, string _title, string _text, QuestType _type, string _item, int _amount, string _mob, bool _active, bool _complete, string _zone, bool _zoneAutoComplete, int _collected, string _completeText, int _gold, int _exp, string _itemReward, int _followup, bool _enabled, string _chain, string _chainType, int _chainFollowupID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET QuestTitle = '" + _title + "', QuestText = '" + _text + "', QuestType = '" + _type.ToString() + "', QuestItem = '" + _item + "', QuestAmount = '" + _amount + "', QuestMob = '" + _mob + "', QuestActive = '" + _active.ToString() + "', QuestComplete = '" + _complete + "', QuestZone = '" + _zone + "', QuestZoneAutoComplete = '" + _zoneAutoComplete.ToString() + "', NPC_ID = '-1', QuestCollected = '" + _collected + "', QuestCompletedText = '" + _completeText + "', QuestGold = '" + _gold + "', QuestExp = '" + _exp + "', QuestRewardItem = '" + _itemReward + "', FollowupID = '" + _followup + "', QuestEnabled = '" + _enabled + "', QuestChain = '" + _chain + "', QuestChainType = '" + _chainType + "', QuestChainFollowupID = '" + _chainFollowupID + "' WHERE QuestID = " + _id);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //                       Update Active Quest in the Database                        //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

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

        public static void UpdateQuestZone(GameObject _zone, int _id)
        {
            GameObject.Find(_zone.name).GetComponent<Zone>().SetQuestID(_id);
        }

        public static void UpdateQuestNPC(int _quest, int _npc)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Quests SET NPC_ID = '" + _npc + "' WHERE QuestID = '" + _quest + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
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

        public static void GetActiveQuests()
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

        public static void GetAllQuestsByNPC(int _id)
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

        //////////////////////////////////////////////////////////////////////////////////////
        //                              Return Functions                                    //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        public static List<int> ReturnAllQuestID()
        {
            return _allQuestID;
        }

        public static int ReturnLastQuestID()
        {
            return _lastQuestID;
        }

        public static List<string> ReturnAllQuestTitles()
        {
            return _allQuestTitles;
        }

        public static List<string> ReturnAllQuestTexts()
        {
            return _allQuestTexts;
        }

        public static List<string> ReturnAllQuestCompleteTexts()
        {
            return _allQuestCompleteText;
        }

        public static List<QuestType> ReturnAllQuestTypes()
        {
            return _allQuestsType;
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //                              Return Quest Items                                  //
        //                                                                                  //
        //////////////////////////////////////////////////////////////////////////////////////

        public static List<string> ReturnAllQuestItemNames()
        {
            return _questItemNames;
        }

        public static List<string> ReturnAllQuestItemPrefab()
        {
            return _questItemPrefabs;
        }

        public static List<QuestItemAmount> ReturnAllQuestItemAmount()
        {
            return _allQuestAmountType;
        }

        public static List<int> ReturnAllQuestAmount()
        {
            return _allQuestAmount;
        }

        public static List<string> ReturnAllQuestMobs()
        {
            return _allQuestMobs;
        }

        public static List<bool> ReturnAllQuestsActive()
        {
            return _allQuestActive;
        }

        public static List<bool> ReturnAllQuestComplete()
        {
            return _allQuestCompleted;
        }

        public static List<string> ReturnAllQuestZones()
        {
            return _allQuestZones;
        }

        public static List<bool> ReturnAllQuestZoneAutoComplete()
        {
            return _allQuestZoneAutoComplete;
        }

        public static List<int> ReturnAllQuestNpcID()
        {
            return _allQuestNpcID;
        }

        public static List<int> ReturnAllQuestCollected()
        {
            return _allQuestCollected;
        }

        public static List<QuestReward> ReturnAllQuestRewards()
        {
            return _allQuestRewards;
        }

        public static List<int> ReturnAllQuestGold()
        {
            return _allQuestGold;
        }

        public static List<int> ReturnAllQuestExperience()
        {
            return _allQuestExp;
        }

        public static List<string> ReturnAllQuestItemRewards()
        {
            return _allQuestItemReward;
        }

        public static List<int> ReturnAllQuestFollowupID()
        {
            return _allQuestFollowupID;
        }

        public static List<bool> ReturnAllQuestEnabled()
        {
            return _allQuestEnabled;
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

            if (_tmp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<QuestChain> ReturnAllQuestChain()
        {
            return _allQuestChain;
        }

        public static List<QuestChainType> ReturnAllQuestChainTypes()
        {
            return _allQuestChainType;
        }

        public static List<int> ReturnAllQuestChainFollowupID()
        {
            return _allQuestChainFollowupID;
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

        public static void ResetQuest(int _id, bool _state)
        {
            if (_state)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/QuestDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery;
                
                    sqlQuery = "UPDATE Quests SET QuestActive = 'False', QuestComplete = 'False',  QuestCollected = '0', QuestEnabled = 'True' WHERE QuestID = '" + _id + "'";
                
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

            }
        }

        public static List<int> ReturnAllActiveQuestID()
        {
            return _activeQuestID;
        }

        public static List<string> ReturnAllActiveQuestTitles()
        {
            return _activeQuestTitle;
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
            }
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
    }
}
