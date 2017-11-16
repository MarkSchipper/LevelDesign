using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public enum LootTypes
{
    None,
    Gold,
    Items,
}


public class LootDatabase : MonoBehaviour {

    private static List<int> _lootID = new List<int>();
    private static List<string> _lootTableName = new List<string>();
    private static List<LootTypes> _lootTypeByTable = new List<LootTypes>();
    private static List<int> _lootValueByTable = new List<int>();
    private static List<int> _lootWeightByTable = new List<int>();
    private static List<int> _itemIDByTable = new List<int>();
    private static List<int> _lootIDByTable = new List<int>();
    private static List<int> _tableID = new List<int>();

    public static void AddLootTable(string _name, string _type, int _value, int _go, int _weight)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/LootDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("INSERT INTO LootTable (LootName, LootType, LootValue, LootItemID, LootWeight) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\")", _name, _type, _value, _go, _weight);
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static void UpdateLootTable(int _id, string _name, string _type, int _value, int _go, int _weight)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/LootDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("UPDATE LootTable SET LootName = '" + _name + "', LootType = '" + _type + "', LootValue = '" + _value + "', LootItemID = '" + _go + "', LootWeight = '" + _weight + "' WHERE LootID = '" + _id + "'");
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;
    }

    public static void DeleteLootTable(string _name)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/LootDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("DELETE FROM LootTable WHERE LootName = '" + _name + "'");
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static void GetAllLootTables()
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/LootDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("SELECT * FROM LootTable GROUP BY LootName");
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while(reader.Read())
        {
            _lootID.Add(reader.GetInt32(0));
            _lootTableName.Add(reader.GetString(1));
        }
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static List<int> ReturnLootID()
    {
        return _lootID;
    }

    public static List<string> ReturnLootTableNames()
    {
        return _lootTableName;
    }

    public static void GetLootTable(string _name)
    {
        ClearByTable();
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/LootDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("SELECT * FROM LootTable WHERE LootName = '" + _name + "'");
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
             _lootIDByTable.Add(reader.GetInt32(0));
            if(reader.GetString(2) == "Gold")
            {
                _lootTypeByTable.Add(LootTypes.Gold);
            }
            if(reader.GetString(2) == "Items")
            {
                _lootTypeByTable.Add(LootTypes.Items);
            }
            _lootValueByTable.Add(reader.GetInt32(3));
            _itemIDByTable.Add(reader.GetInt32(4));
            _lootWeightByTable.Add(reader.GetInt32(5));
        }
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static List<int> ReturnLootIdByTable()
    {
        return _lootIDByTable;
    }

    public static List<LootTypes> ReturnLootTypeByTable()
    {
        return _lootTypeByTable;
    }

    public static List<int> ReturnLootValueByTable()
    {
        return _lootValueByTable;
    }

    public static List<int> ReturnItemIDByTable()
    {
        return _itemIDByTable;
    }

    public static List<int> ReturnLootWeightByTable()
    {
        return _lootWeightByTable;
    }

    public static void ClearAll()
    {
        _lootID.Clear();
        _lootTableName.Clear();
        _lootTypeByTable.Clear();

        _lootValueByTable.Clear();
        _lootWeightByTable.Clear();
        _itemIDByTable.Clear();
        _lootIDByTable.Clear();
    }

    static void ClearByTable()
    {
        _lootTypeByTable.Clear();

        _lootValueByTable.Clear();
        _lootWeightByTable.Clear();
        _itemIDByTable.Clear();
        _lootIDByTable.Clear();
    }
}
