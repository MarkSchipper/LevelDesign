using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;

#if UNITY_EDITOR

public class ItemManager : EditorWindow {


    // Since we are loading Objects from the Resources folder we need to use UnityEngine.Object
    private UnityEngine.Object[] _loadPotions;
    private List<string> _loadPotionsName = new List<string>();

    private UnityEngine.Object[] _loadQuestItems;
    private List<string> _loadQuestItemsName = new List<string>();

    // Create Lists in which we store all values from the Database
    private List<int> _itemID = new List<int>();
    private List<string> _itemNameList = new List<string>();
    private List<string> _itemDescList = new List<string>();
    private List<ItemType> _itemTypeList = new List<ItemType>();
    private List<int> _itemStatsList = new List<int>();
    private List<int> _itemObjectIDList = new List<int>();
    private List<string> _itemObjectList = new List<string>();

    // Unfold boolean for the edit menu
    private bool[] _unfold;

    private string _itemName = "";
    private string _itemDesc = "";
    private int _itemPower;
    private int _itemStats;
    private ItemType _itemType;

    private bool _addItem = false;
    private bool _editItem = false;
    private bool _deleteItem = false;
    private bool _itemsLoaded = false;
    private bool _isDeletingItem = false;
    private bool _deleteConfirmation = false;

    private int _itemToDelete;


    private int _selected;

    [MenuItem("Level Design/Managers/Item Manager")]

    static void ShowEditor()
    {
        ItemManager _itemManager = EditorWindow.GetWindow<ItemManager>();

    }

    void OnEnable()
    {
        ClearLists();
        // OnEnable we get all items from the Items database
        GetAllItems();

        // Load all objects from the Potions folder - Note: ALL objects so also the textures and meshes
        _loadPotions = Resources.LoadAll("Collectables/Potions/");
        
        for (int i = 0; i < _loadPotions.Length; i++)
        {
            // Create a filter so we only add the GameObjects to the loadPotionsName List
            if(_loadPotions[i].GetType().ToString() == "UnityEngine.GameObject") {

                _loadPotionsName.Add(_loadPotions[i].ToString().Remove(_loadPotions[i].ToString().Length - 25));
                          
            }
        }

        _loadQuestItems = Resources.LoadAll("Collectables/QuestItems");

        for (int i = 0; i < _loadQuestItems.Length; i++)
        {
            if(_loadQuestItems[i].GetType().ToString() == "UnityEngine.GameObject")
            {
                _loadQuestItemsName.Add(_loadQuestItems[i].ToString().Remove(_loadQuestItems[i].ToString().Length - 25));
            }
        }

    }

    void OnGUI()
    {
        if (!_addItem && !_editItem && !_deleteItem && !_isDeletingItem) {
            if (GUILayout.Button("Add Item"))
            {
                _addItem = !_addItem;
            }

            if (GUILayout.Button("Edit Items"))
            {
                _editItem = !_editItem;


                _unfold = new bool[_itemID.Count];
            }

            if (GUILayout.Button("Delete an Item"))
            {
                _deleteItem = true;
            }
        }
        #region ADD ITEM
        if (_addItem)
        {

            // Basic UI layout 
            _itemName = EditorGUILayout.TextField("Item Name", _itemName);
            GUILayout.Label("Item Description");
            _itemDesc = EditorGUILayout.TextArea(_itemDesc, GUILayout.Height(100));
            _itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", _itemType);
            if(_itemType == ItemType.Health || _itemType == ItemType.Mana)
            {
                _selected = EditorGUILayout.Popup("Which Prefab: ", _selected, _loadPotionsName.ToArray());
                
            }

            if(_itemType == ItemType.QuestItem)
            {
                _selected = EditorGUILayout.Popup("Which Prefab: ", _selected, _loadQuestItemsName.ToArray());
            }

            _itemStats = EditorGUILayout.IntField("Item Value: ", _itemStats);

            if (GUILayout.Button("Save Item"))
            {
                if (_itemType == ItemType.Health || _itemType == ItemType.Mana)
                {
                    AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadPotionsName[_selected]);
                }

                if(_itemType == ItemType.QuestItem)
                {
                    AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadQuestItemsName[_selected]);
                }
                // Clear ALL lists so we can repopulate them 
                ClearLists();
                GetAllItems();
            }
            if(GUILayout.Button("BACK"))
            {
                _addItem = false;
            }
        }
        #endregion
        #region EDIT AN ITEM
        if (_editItem && !_addItem)
        {

            for (int i = 0; i < _itemID.Count; i++)
            {

                _unfold[i] = EditorGUILayout.Foldout(_unfold[i], _itemNameList[i]);

                if (_unfold[i])
                {
                    _itemNameList[i] = EditorGUILayout.TextField("Item Name: ", _itemNameList[i]);
                    _itemDescList[i] = EditorGUILayout.TextArea(_itemDescList[i], GUILayout.Height(100));
                    _itemTypeList[i] = (ItemType)EditorGUILayout.EnumPopup("Item Type: ", _itemTypeList[i]);

                    if(_itemTypeList[i] == ItemType.Health || _itemTypeList[i] == ItemType.Mana)
                    {
                        _itemObjectIDList[i] = EditorGUILayout.Popup("Which Prefab: ", _itemObjectIDList[i], _loadPotionsName.ToArray());
                    }
                    if(GUILayout.Button("Save Changes"))
                    {
                        SaveItem(_itemID[i], _itemNameList[i], _itemDescList[i], _itemTypeList[i], _itemStatsList[i]);
                        ClearLists();
                        GetAllItems();
                    }

                    /*
                    if(GUILayout.Button("Delete Item"))
                    {
                        DeleteItem(_itemID[i]);
                        ClearLists();
                        GetAllItems();
                    }
                    */

                }
            }

            if (GUILayout.Button("BACK"))
            {
                _editItem = false;
            }

        }
        #endregion
        #region DELETE ITEM
        if(_deleteItem)
        {
            for (int i = 0; i < _itemID.Count; i++)
            {
                if(GUILayout.Button("Delete " + _itemNameList[i] + " from the database"))
                {
                    _isDeletingItem = true;
                    _itemToDelete = i;
                }
            }
            if(GUILayout.Button("BACK"))
            {
                _deleteItem = false;
            }
        }

        if(_isDeletingItem)
        {
            GUILayout.Label("Are you sure you want to delete " + _itemNameList[_itemToDelete] + "?");
            if(GUILayout.Button("YES"))
            {
                DeleteItem(_itemToDelete);
                _isDeletingItem = false;
                
            }
            if(GUILayout.Button("BACK"))
            {
                _isDeletingItem = false;
                _deleteItem = false;
            }
        }
        #endregion
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
            _itemNameList.Add(reader.GetString(1));
            _itemDescList.Add(reader.GetString(2));
           
            if(reader.GetString(3) == "Weapon")
            {
                _itemTypeList.Add(ItemType.Weapon);
            }

            if (reader.GetString(3) == "Health")
            {
                _itemTypeList.Add(ItemType.Health);
            }

            if (reader.GetString(3) == "Mana")
            {
                _itemTypeList.Add(ItemType.Mana);
            }

            if (reader.GetString(3) == "QuestItem")
            {
                _itemTypeList.Add(ItemType.QuestItem);
            }
            if (reader.GetString(3) == "Armour")
            {
                _itemTypeList.Add(ItemType.Armour);
            }
            
            _itemStatsList.Add(reader.GetInt32(4));
            _itemObjectIDList.Add(reader.GetInt32(5));
            _itemObjectList.Add(reader.GetString(6));
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void AddItem(string _name, string _desc, ItemType _type, int _stats, int _objectID, string _object)
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

    void SaveItem(int _id, string _name, string _desc, ItemType _type, int _stats)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/Databases/ItemDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format("UPDATE Items " + " SET ItemName = " + "'" + _name + "'" + ", ItemDesc = " + "'" + _desc + "'" + ", ItemType = " + "'" + _type.ToString() + "'" + ", ItemStats = " + "'" + _stats + "'" + "WHERE ItemID = " + "'" + _id + "'" );
        
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void DeleteItem(int _id)
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

        Repaint();
    }

    void ClearLists()
    {
        _itemID.Clear();
        _itemNameList.Clear();
        _itemDescList.Clear();
        _itemTypeList.Clear();
        _itemStatsList.Clear();
    }
}
#endif