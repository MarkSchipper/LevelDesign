using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace FeedbackEditor
{

    public class FeedbackDB
    {

        private static int _lastId;

        private static List<int> _feedbackID = new List<int>();
        private static List<string> _feedbackType = new List<string>();
        private static List<string> _feedbackTrigger = new List<string>();
        private static List<float> _feedbackTimer = new List<float>();
        private static List<float> _feedbackIdleTimer = new List<float>();
        private static List<string> _feedbackTriggerShape = new List<string>();
        private static List<string> _feedbackText = new List<string>();
        private static List<string> _feedbackCondition = new List<string>();
        private static List<string> _feedbackAchievement = new List<string>();
        private static List<int> _feedbackTriggerSize = new List<int>();
        private static List<int> _feedbackAchievementAmount = new List<int>();

        public static void SaveFeedback(string _type, string _trigger, float _timer, float _idleTimer, string _shape, string _text, string _condition, string _achievement, int _triggerSize, int _achievementAmount)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/FeedbackDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("INSERT INTO Feedback (FeedbackType, FeedbackTrigger, TriggerShape, HitCondition, AchievementType, Timer, IdleTimer, FeedbackText, TriggerSize, AchievementAmount) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\")", _type, _trigger, _shape, _condition, _achievement,_timer, _idleTimer, _text, _triggerSize, _achievementAmount);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;

            dbcmd = dbconn.CreateCommand();

            string _nwSqlQuery = String.Format("SELECT last_insert_rowid()");
            dbcmd.CommandText = _nwSqlQuery;
            System.Object _temp = dbcmd.ExecuteScalar();
            _lastId = int.Parse(_temp.ToString());
            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        }

        public static void GetAllFeedback()
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/FeedbackDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Feedback";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                _feedbackID.Add(reader.GetInt32(0));
                _feedbackType.Add(reader.GetString(1));
                _feedbackTrigger.Add(reader.GetString(2));
                _feedbackTriggerShape.Add(reader.GetString(3));
                _feedbackCondition.Add(reader.GetString(4));
                _feedbackAchievement.Add(reader.GetString(5));
                _feedbackTimer.Add(reader.GetFloat(6));
                _feedbackIdleTimer.Add(reader.GetFloat(7));
                _feedbackText.Add(reader.GetString(8));
                _feedbackTriggerSize.Add(reader.GetInt32(9));
                _feedbackAchievementAmount.Add(reader.GetInt32(10));

            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static void UpdateFeedback(int _id, string _type, string _trigger, float _timer, float _idleTimer, string _shape, string _text, string _condition, string _achievement, int _triggerSize, int _amount)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/FeedbackDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format("UPDATE Feedback SET FeedbackType = '" + _type + "', FeedbackTrigger = '" + _trigger + "', TriggerShape = '" + _shape + "', HitCondition = '" + _condition + "', AchievementType = '" + _achievement + "', Timer = '" + _timer + "', IdleTimer = '" + _idleTimer + "', FeedbackText = '" + _text + "', TriggerSize = '" + _triggerSize + "', AchievementAmount = '" + _amount + "' WHERE FeedbackID = '" + _id + "'");
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        public static int ReturnLastID()
        {
            return _lastId;
        }

        public static List<string> ReturnAllFeedbackText()
        {
            return _feedbackText;
        }

        public static int ReturnFeedbackID(int _id)
        {
            return _feedbackID[_id];
        }

        public static string ReturnFeedbackText(int _id)
        {
            return _feedbackText[_id];
        }

        public static void DeleteFeedback(int _id)
        {
            string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/FeedbackDB.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "DELETE FROM Feedback WHERE FeedbackID = '" + _id + "'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
         
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            
            
        }

        public static string ReturnFeedbackType(int _id)
        {
            return _feedbackType[_id];
        }

        public static string ReturnFeedbackTrigger(int _id)
        {
            return _feedbackTrigger[_id];
        }

        public static float ReturnFeedbackTimer(int _id)
        {
            return _feedbackTimer[_id];
        }

        public static float ReturnFeedbackIdleTimer(int _id)
        {
            return _feedbackIdleTimer[_id];
        }

        public static string ReturnFeedbackTriggerShape(int _id)
        {
            return _feedbackTriggerShape[_id];
        }

        public static int ReturnFeedbackTriggerSize(int _id)
        {
            return _feedbackTriggerSize[_id];
        }

        public static string ReturnFeedbackCondition(int _id)
        {
            return _feedbackCondition[_id];
        }

        public static string ReturnFeedbackAchievement(int _id)
        {
            return _feedbackAchievement[_id];
        }

        public static int ReturnAchievementAmount(int _id)
        {
            return _feedbackAchievementAmount[_id];
        }

        public static void ClearAll()
        {
            _feedbackID.Clear();
            _feedbackType.Clear();
            _feedbackTrigger.Clear();
            _feedbackTimer.Clear();
            _feedbackIdleTimer.Clear();
            _feedbackTriggerShape.Clear();
            _feedbackText.Clear();
            _feedbackCondition.Clear();
            _feedbackAchievement.Clear();
            _feedbackTriggerSize.Clear();
        }

    }

}
