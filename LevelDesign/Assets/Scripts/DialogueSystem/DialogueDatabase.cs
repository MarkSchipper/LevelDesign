using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;


namespace Dialogue
{

    public class DialogueDatabase : MonoBehaviour
    {

        private static List<int> _dialogueIDList = new List<int>();
        private static List<int> _conversationIDList = new List<int>();
        private static List<int> _npcIDList = new List<int>();
        private static List<int> _previousNodeList = new List<int>();
        private static List<string> _questionList = new List<string>();
        private static List<string> _answerList = new List<string>();
        private static List<bool> _typeList = new List<bool>();
        private static List<string> _titleList = new List<string>();
        private static List<Vector2> _canvasPos = new List<Vector2>();
        private static List<int> _nodeIDList = new List<int>();
        private static List<int> _questIDList = new List<int>();
        private static List<string> _conditionList = new List<string>();
        private static List<string> _conditionStatementList = new List<string>();
        private static List<string> _conditionTermList = new List<string>();
        private static List<string> _conditionValueList = new List<string>();

        private static List<string> _multipleQuestions = new List<string>();

        private static bool _deletedDialogue;

        private static string _question;
        private static List<string> _answers = new List<string>();
        private static List<string> _titles = new List<string>();
        private static List<int> _previousNode = new List<int>();
        private static List<int> _nodeID = new List<int>(); 

        public static void AddDialogue(int _conversID, int _nodeID, int _npc, int _node, string _question, string _answer, bool _type, string _title, Vector2 pos, int _qID, string _condition, string _conditionStatement, string _conditionTerm, string _conditionValue)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Dialogue (ConversationID, NodeID, NpcID, PreviousNode, Question, Answer, CorrectAnswer, Title, CanvasPosition, QuestID, Condition, ConditionStatement, ConditionTerm, ConditionValue) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\")", _conversID, _nodeID, _npc, _node, _question, _answer, _type.ToString(), _title, pos.ToString(), _qID, _condition, _conditionStatement, _conditionTerm, _conditionValue);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void UpdateDialogue(int _conversID, int _nodeID, int _npc, int _node, string _question, string _answer, bool _type, string _title, Vector2 pos, int _qID, string _condition, string _conditionStatement, string _conditionTerm, string _conditionValue)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Dialogue (ConversationID, NodeID, NpcID, PreviousNode, Question, Answer, CorrectAnswer, Title, CanvasPosition, QuestID, Condition, ConditionStatement, ConditionTerm, ConditionValue) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\", \"{10}\", \"{11}\", \"{12}\", \"{13}\")", _conversID, _nodeID, _npc, _node, _question, _answer, _type.ToString(), _title, pos.ToString(), _qID, _condition, _conditionStatement, _conditionTerm, _conditionValue);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;



        }

        public static void DeleteByNPC(int _npc)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string deleteQuery = String.Format("DELETE  FROM Dialogue WHERE NpcID = '" + _npc + "'");
            dbcmd.CommandText = deleteQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void DeletePreviousDialogue(int _conversID)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string deleteQuery = String.Format("DELETE  FROM Dialogue WHERE ConversationID = '" + _conversID + "'");
            dbcmd.CommandText = deleteQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void GetDialogueByNPC(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Dialogue WHERE NpcID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    _dialogueIDList.Add(reader.GetInt32(0));
                    _conversationIDList.Add(reader.GetInt32(1));
                    _nodeIDList.Add(reader.GetInt32(2));
                    _npcIDList.Add(reader.GetInt32(3));
                    _previousNodeList.Add(reader.GetInt32(4));
                    _questionList.Add(reader.GetString(5));
                    _answerList.Add(reader.GetString(6));
                    _typeList.Add(bool.Parse(reader.GetString(7)));
                    _titleList.Add(reader.GetString(8));
                    string[] _splitArray = reader.GetString(9).Substring(1, reader.GetString(9).Length - 2).Split(char.Parse(","));
                    _canvasPos.Add(new Vector2(float.Parse(_splitArray[0]), float.Parse(_splitArray[1])));
                    _questIDList.Add(reader.GetInt32(10));
                    _conditionList.Add(reader.GetString(11));
                    _conditionStatementList.Add(reader.GetString(12));
                    _conditionTermList.Add(reader.GetString(13));
                    _conditionValueList.Add(reader.GetString(14));

                }
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

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

            if (_tmp != null)
            {
                return _tmp.ToString();
            }
            else
            {
                return string.Empty;
            }

        }

        public static int GetInitialQuestIDFromNPC(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT QuestID FROM Dialogue WHERE NpcID = '" + _id + "' AND QuestID != '-1' AND PreviousNode = '0'";
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

        public static List<int> ReturnNodeIDList()
        {
            return _nodeID;
        }

        public static List<string> GetAnswersForQuestion(int _npcID, int _nodeID)
        {
            _answers.Clear();


            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT Answer FROM Dialogue WHERE PreviousNode = '" + _nodeID + "' AND NpcID = '" + _npcID + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    _answers.Add(reader.GetString(0));
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

        public static List<int> GetNodeIDFromAnswer(int _npcID, int _node)
        {
            if (_nodeID.Count > 0)
            {
                _nodeID.Clear();
            }
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT NodeID FROM Dialogue WHERE PreviousNode = '" + _node + "' AND NpcID = '" + _npcID + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    _nodeID.Add(reader.GetInt32(0));
                    
                }
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return _nodeID;

        }

        public static List<string> GetTitleByNode(int _npcID, int _node)
        {
            if (_titles.Count > 0)
            {
                _titles.Clear();
            }

            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT Title FROM Dialogue WHERE PreviousNode = '" + _node + "' AND NpcID = '" + _npcID + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            if (reader != null)
            {
                while (reader.Read())
                {
                    _titles.Add(reader.GetString(0));
                }
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return _titles;
        }

        public static string GetFollowupQuestion(int _npc, int _node)
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

        public static string GetFollowupQuestion(int _npc, int _node, string _bool)
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

        public static string GetFollowupCondition(int _npc, int _node)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT Condition FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                return _tmp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetFollowupConditionStatement(int _npc, int _node)
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

            if (_tmp != null)
            {
                return _tmp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetFollowupConditionTerm(int _npc, int _node)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT ConditionTerm FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
            dbcmd.CommandText = sqlQuery;
            System.Object _tmp = dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (_tmp != null)
            {
                return _tmp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static int GetFollowupConditionValue(int _npc, int _node)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/DialogueDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT ConditionValue FROM Dialogue WHERE NpcID = '" + _npc + "'  AND PreviousNode = '" + _node + "'";
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

        public static void ResetDeletedDialogue()
        {
            _deletedDialogue = false;
        }

        public static int ReturnCount()
        {
            return _conversationIDList.Count;
        }

        public static int ReturnDialogueID(int _id)
        {
            return _dialogueIDList[_id];
        }

        public static int ReturnConversationID(int _id)
        {
            return _conversationIDList[_id];
        }

        public static int ReturnPreviousNode(int _id)
        {
            return _previousNodeList[_id];
        }

        public static string ReturnQuestion(int _id)
        {
            return _questionList[_id];
        }

        public static string ReturnSingleQuestion()
        {

            return _question;
        }

        public static string ReturnResponse(int _id)
        {
            return _answerList[_id];
        }

        public static bool ReturnType(int _id)
        {
            return _typeList[_id];
        }

        public static string ReturnTitle(int _id)
        {
            return _titleList[_id];
        }

        public static string ReturnAnswer(int _id)
        {
            return _answerList[_id];
        }

        public static Vector2 ReturnCanvasPosition(int _id)
        {
            return _canvasPos[_id];
        }

        public static int ReturnQuestID(int _id)
        {
            return _questIDList[_id];
        }

        public static string ReturnCondition(int _id)
        {
            return _conditionList[_id];
        }

        public static string ReturnConditionStatement(int _id)
        {
            return _conditionStatementList[_id];
        }

        public static string ReturnConditionTerms(int _id)
        {
            return _conditionTermList[_id];
        }

        public static string ReturnConditionValue(int _id)
        {
            return _conditionValueList[_id];
        }

        public static void ClearAll()
        {
            _dialogueIDList.Clear();
            _conversationIDList.Clear();
            _npcIDList.Clear();
            _previousNodeList.Clear();
            _questionList.Clear();
            _answerList.Clear();
            _typeList.Clear();
            _titleList.Clear();
            _nodeID.Clear();
        }
    }
}