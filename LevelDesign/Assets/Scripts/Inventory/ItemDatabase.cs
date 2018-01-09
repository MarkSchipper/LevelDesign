using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class ItemDatabase : MonoBehaviour {

    private static List<Item> _itemList = new List<Item>();
    private static List<int> _itemID = new List<int>();
    private static List<string> _itemName = new List<string>();
    private static List<string> _itemDesc = new List<string>();
    private static List<ItemType> _itemType = new List<ItemType>();
    private static List<int> _itemPower = new List<int>();
    private static List<int> _itemStats = new List<int>();
    private static List<int> _itemObjectID = new List<int>();
    private static List<string> _itemObject = new List<string>();

    void OnEnable()
    {
        
        GetAllItems();

        for (int i = 0; i < _itemID.Count; i++)
        {
           _itemList.Add(new Item(_itemName[i], _itemID[i], _itemDesc[i],0,_itemStats[i], _itemType[i]));
        }
    }

    public static void GetAllItems()
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * " + "FROM Items";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            _itemID.Add(reader.GetInt32(0));
            //_itemName.Add(reader.GetString(1));
            _itemName.Add(reader.GetString(1));
                        
            
            _itemDesc.Add(reader.GetString(2));
            if(reader.GetString(3) == "Weapon")
            {
                _itemType.Add(ItemType.Weapon);
            }
            if (reader.GetString(3) == "Health")
            {
                _itemType.Add(ItemType.Health);
            }
            if (reader.GetString(3) == "Mana")
            {
                _itemType.Add(ItemType.Mana);
            }
            if (reader.GetString(3) == "QuestItem")
            {
                _itemType.Add(ItemType.QuestItem);
            }
            if (reader.GetString(3) == "Armour")
            {
                _itemType.Add(ItemType.Armour);
            }

            _itemStats.Add(reader.GetInt32(4));
            _itemObjectID.Add(reader.GetInt32(5));
            _itemObject.Add(reader.GetString(6));
   
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static void AddItem(string _name, string _desc, ItemType _type, int _stats, int _objectID, string _object)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("INSERT INTO Items (ItemName, ItemDesc, ItemType, ItemStats, ItemObjectID, ItemObject) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\")", _name, _desc, _type.ToString(), _stats, _objectID, _object);
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static void SaveItem(int _id, string _name, string _desc, ItemType _type, int _stats)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("UPDATE Items " + " SET ItemName = " + "'" + _name + "'" + ", ItemDesc = " + "'" + _desc + "'" + ", ItemType = " + "'" + _type.ToString() + "'" + ", ItemStats = " + "'" + _stats + "'" + "WHERE ItemID = " + "'" + _id + "'");

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static void DeleteItem(int _id)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("DELETE FROM Items WHERE ItemID = " + "'" + _id + "'");

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

    }

    public static List<int> ReturnItemIDs()
    {
        return _itemID;
    }

    public static int ReturnItemID(int _id)
    {
        return _itemID[_id];
    }

    public List<Item> ReturnItemList()
    {
        return _itemList;
    }

    public static List<string> ReturnItemNames()
    {
        return _itemName;
    }

    public static string ReturnItemName(int _id)
    {
        return _itemName[_id];
    }

    public static List<string> ReturnItemDescriptions()
    {
        return _itemDesc;
    }

    public static string ReturnItemDescription(int _id)
    {
        return _itemDesc[_id];
    }

    public static List<ItemType> ReturnItemTypes()
    {
        return _itemType;
    }

    public static ItemType ReturnItemType(int _id)
    {
        return _itemType[_id];
    }

    public static List<string> ReturnItemObjects()
    {
        return _itemObject;
    }

    public static string ReturnItemObject(int _id)
    {
        return _itemObject[_id];
    }

    public static List<int> ReturnItemStats()
    {
        return _itemStats;
    }

    public static int ReturnItemStat(int _id)
    {
        return _itemStats[_id];
    }

    public static void ClearLists()
    {
        _itemID.Clear();
        _itemName.Clear();
        _itemDesc.Clear();
        _itemType.Clear();
        _itemStats.Clear();
    }

}
