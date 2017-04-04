using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class ItemDatabase : MonoBehaviour {

    public List<Item> _itemList = new List<Item>();


    private List<int> _itemID = new List<int>();
    private List<string> _itemName = new List<string>();
    private List<string> _itemDesc = new List<string>();
    private List<ItemType> _itemType = new List<ItemType>();
    private List<int> _itemPower = new List<int>();
    private List<int> _itemStats = new List<int>();

    void OnEnable()
    {
        
        GetAllItems();

        for (int i = 0; i < _itemID.Count; i++)
        {
           _itemList.Add(new Item(_itemName[i], _itemID[i], _itemDesc[i],0,_itemStats[i], _itemType[i]));
        }
    }

    void GetAllItems()
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
            
       
            
    


        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}
