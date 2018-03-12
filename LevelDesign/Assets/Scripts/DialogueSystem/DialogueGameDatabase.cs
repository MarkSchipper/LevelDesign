using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace Dialogue
{
    namespace Game
    {
        class DialogueGameDatabase : MonoBehaviour
        {
            //vars
            private static List<int> _nodeIDList = new List<int>();

            private static List<int> _questID = new List<int>();

            // functions
            private static List<string> _answers = new List<string>();

            public static string GetInitialQuestionFromNPC(int _id)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Question FROM Dialogue WHERE NpcID = '" + _id + "' AND Question != '' AND PreviousNode = '0'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();               

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                if(_tmp.ToString() != string.Empty)
                {
                    return _tmp.ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

            public static List<string> GetAnswersByQuestion(int _npc, int _node)
            {
                _answers.Clear();
                _questID.Clear();

                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Answer, Title, QuestID FROM Dialogue WHERE PreviousNode = '" + _node + "' AND NpcID = '" + _npc + "'";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(1) == string.Empty)
                        {
                            _answers.Add(reader.GetString(0));
                            _questID.Add(-1);

                        }
                        else if (reader.GetString(1) == "Add Quest")
                        {
                            if (!Quest.QuestGameManager.ReturnQuestActive(reader.GetInt32(2)))
                            {
                                if (Quest.QuestGameManager.ReturnQuestComplete(reader.GetInt32(2)))
                                {
                                    _answers.Add(Quest.QuestGameManager.ReturnQuestCompletedTextByID(reader.GetInt32(2)));
                                    _questID.Add(reader.GetInt32(2));
                                    
                                }
                                else
                                {
                                    _answers.Add(Quest.QuestGameManager.ReturnQuestTitleByID(reader.GetInt32(2)));
                                    _questID.Add(reader.GetInt32(2));
                                }
                            }
                            else if (Quest.QuestGameManager.ReturnQuestActive(reader.GetInt32(2)))
                            {
                                _answers.Add(Quest.QuestGameManager.ReturnQuestTitleByID(reader.GetInt32(2)));
                                _questID.Add(reader.GetInt32(2));
                            }
                            else
                            {
                                _answers.Add("Quit");
                                _questID.Add(-1);
                            }
                        }
                        else if (reader.GetString(1) == "ConditionNode")
                        {

                            if (GetConditionStatement(_npc, _node) == "ZoneVisited")
                            {
                                _answers.Add("Zone Visit");
                            }
                            else if(GetConditionStatement(_npc, _node) == "QuestCompleted")
                            {
                                _answers.Add("Did you complete");
                            }
                            else if(GetConditionStatement(_npc, _node) == "PlayerLevel")
                            {
                                _answers.Add("Are yoy strong enough");
                            }
                            else if (GetConditionStatement(_npc, _node) == "QuestActive")
                            {
                                _answers.Add("Are you working on");
                            }
                            _questID.Add(-1);
                        }
                    }

                }

                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _answers;
            }

            public static int GetNextNodeID(int _id, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT NodeID FROM Dialogue WHERE NpcID = '" + _id + "' AND PreviousNode = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                if (_tmp != null)
                {
                    return int.Parse(_tmp.ToString());
                }
                else
                {
                    return -1;
                }
            }

            public static int GetNextNodeID(int _id, int _node, string _bool)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT NodeID FROM Dialogue WHERE NpcID = '" + _id + "' AND PreviousNode = '" + _node + "' AND CorrectAnswer = '" + _bool + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                if (_tmp != null)
                {
                    return int.Parse(_tmp.ToString());
                }
                else
                {
                    return -1;
                }
            }


            public static List<int> GetNodeIDsByAnswer(int _npc, int _node)
            {
                _nodeIDList.Clear();
            
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT NodeID FROM Dialogue WHERE PreviousNode = '" + _node + "' AND NpcID = '" + _npc + "'";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        _nodeIDList.Add(reader.GetInt32(0));

                    }
                }

                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _nodeIDList;
            }

            public static string GetNextQuestion(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Question FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetNextQuestion(int _npc, int _node, string _bool)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Question FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "' AND CorrectAnswer = '" + _bool + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetCurrentQuestion(int _npc, int _node, string _bool)
            {

                Debug.Log(_node + " - " + _bool);
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Question FROM Dialogue WHERE NpcID = '" + _npc + "'  AND NodeID = '" + _node + "' AND CorrectAnswer = '" + _bool + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetTitle(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Title FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetCurrentTitle(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Title FROM Dialogue WHERE NpcID = '" + _npc + "'  AND NodeID = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetCondition(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Condition FROM Dialogue WHERE NpcID = '" + _npc + "'  AND NodeID = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetConditionStatement(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT ConditionStatement FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetCurrentConditionStatement(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT ConditionStatement FROM Dialogue WHERE NpcID = '" + _npc + "'  AND NodeID = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();


                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static string GetConditionValue(int _npc, int _node)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT ConditionValue FROM Dialogue WHERE NpcID = '" + _npc + "'  AND NodeID = '" + _node + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _tmp.ToString();
            }

            public static int GetQuestID(int _npc, int _previousNode)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT QuestID FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _previousNode + "'";
                dbcmd.CommandText = sqlQuery;
                System.Object _tmp = dbcmd.ExecuteScalar();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return int.Parse(_tmp.ToString());
            }

            public static List<int> GetQuestsFromNPC(int _npc)
            {
                List<int> _allID = new List<int>();
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT QuestID FROM Dialogue WHERE NpcID = '" + _npc + "' AND QuestID != '-1'";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        _allID.Add(reader.GetInt32(0));
                    }
                }

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;

                return _allID;
            }

            public static List<int> ReturnQuestID()
            {
                return _questID;
            }

        }
    }
}
