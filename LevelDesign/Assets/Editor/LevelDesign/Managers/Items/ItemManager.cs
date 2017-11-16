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
    private static UnityEngine.Object[] _loadPotions;
    private static List<string> _loadPotionsName = new List<string>();

    private static UnityEngine.Object[] _loadQuestItems;
    private static List<string> _loadQuestItemsName = new List<string>();

    // Create Lists in which we store all values from the Database
    private static List<int> _itemID = new List<int>();
    private static List<string> _itemNameList = new List<string>();
    private static List<string> _itemDescList = new List<string>();
    private static List<ItemType> _itemTypeList = new List<ItemType>();
    private static List<int> _itemStatsList = new List<int>();
    private static List<int> _itemObjectIDList = new List<int>();
    private static List<string> _itemObjectList = new List<string>();

    private static string _itemObject;
    private static int _itemObjectID;

    private static string _itemName = "";
    private static string _itemDesc = "";
    private static int _itemPower;
    private static int _itemStats;
    private static ItemType _itemType;

    private bool _addItem = false;
    private bool _editItem = false;
    private bool _deleteItem = false;
    private bool _itemsLoaded = false;
    private static bool _isDeletingItem = false;
    private static bool _deleteConfirmation = false;
    private static bool _isLoadedFromDatabase = false;
    private static bool _savedItem;

    private static int _editSelectIndex;
    private static int _itemToDelete;

    private static int _selected;
 
    /*
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
                    ItemDatabase.AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadPotionsName[_selected]);
                }

                if(_itemType == ItemType.QuestItem)
                {
                    ItemDatabase.AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadQuestItemsName[_selected]);
                }
                // Clear ALL lists so we can repopulate them 
                ClearLists();
                ItemDatabase.GetAllItems();
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
                        ItemDatabase.SaveItem(_itemID[i], _itemNameList[i], _itemDescList[i], _itemTypeList[i], _itemStatsList[i]);
                        ItemDatabase.ClearLists();
                        ItemDatabase.GetAllItems();
                    }

                    /*
                    if(GUILayout.Button("Delete Item"))
                    {
                        DeleteItem(_itemID[i]);
                        ClearLists();
                        GetAllItems();
                    }
                 

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
                ItemDatabase.DeleteItem(_itemToDelete);
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
    */
    public static void ShowAddItem()
    {
        _isLoadedFromDatabase = false;
        _savedItem = false;

        // Basic UI layout 
        _itemName = EditorGUILayout.TextField("Item Name", _itemName);
        GUILayout.Label("Item Description");
        _itemDesc = EditorGUILayout.TextArea(_itemDesc, GUILayout.Height(100));
        _itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", _itemType);
        if (_itemType == ItemType.Health || _itemType == ItemType.Mana)
        {
            _selected = EditorGUILayout.Popup("Which Prefab: ", _selected, _loadPotionsName.ToArray());

        }

        if (_itemType == ItemType.QuestItem)
        {
            _selected = EditorGUILayout.Popup("Which Prefab: ", _selected, _loadQuestItemsName.ToArray());
        }

        _itemStats = EditorGUILayout.IntField("Item Value: ", _itemStats);

        if (GUILayout.Button("Save Item"))
        {
            if (_itemType == ItemType.Health || _itemType == ItemType.Mana)
            {
                ItemDatabase.AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadPotionsName[_selected]);
            }

            if (_itemType == ItemType.QuestItem)
            {
                ItemDatabase.AddItem(_itemName, _itemDesc, _itemType, _itemStats, _selected, _loadQuestItemsName[_selected]);
            }
            // Clear ALL lists so we can repopulate them 
            ClearLists();
            ItemDatabase.GetAllItems();
        }
    }
    
    public static void ShowEditItem()
    {
        if (!_savedItem)
        {
            if (!_isLoadedFromDatabase)
            {
                ClearLists();
                LoadFromDatabase();
                Debug.Log("loaded");
                _isLoadedFromDatabase = true;
            }
            
            GUILayout.Space(20);
            _editSelectIndex = EditorGUILayout.Popup(_editSelectIndex, ItemDatabase.ReturnItemNames().ToArray());

            _itemNameList[_editSelectIndex] = EditorGUILayout.TextField("Item Name: ", _itemNameList[_editSelectIndex]);
            _itemDescList[_editSelectIndex] = EditorGUILayout.TextArea(_itemDescList[_editSelectIndex], GUILayout.Height(100));
            _itemTypeList[_editSelectIndex] = (ItemType)EditorGUILayout.EnumPopup("Item Type: ", _itemTypeList[_editSelectIndex]);
            _itemStatsList[_editSelectIndex] = EditorGUILayout.IntField("Item Stats: ", _itemStatsList[_editSelectIndex]);

            if (_itemTypeList[_editSelectIndex] == ItemType.Health || _itemTypeList[_editSelectIndex] == ItemType.Mana)
            {
                _itemObjectIDList[_editSelectIndex] = EditorGUILayout.Popup("Which Prefab: ", _itemObjectIDList[_editSelectIndex], _loadPotionsName.ToArray());
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Save Changes"))
            {
                ItemDatabase.SaveItem(ItemDatabase.ReturnItemID(_editSelectIndex), _itemNameList[_editSelectIndex], _itemDescList[_editSelectIndex], _itemTypeList[_editSelectIndex], _itemStatsList[_editSelectIndex]);
                ItemDatabase.ClearLists();
                ItemDatabase.GetAllItems();
                ClearValues();
                _savedItem = true;
            }
        }
        else
        {
            GUILayout.Label("The item has been saved");
        }
    }

    public static void ShowDeleteItem()
    {
        if (!_isLoadedFromDatabase)
        {
            ClearLists();
            LoadFromDatabase();
            _isLoadedFromDatabase = true;
        }
        GUILayout.Space(20);
        if (!_isDeletingItem)
        {
            for (int i = 0; i < _itemID.Count; i++)
            {
                if (GUILayout.Button("Delete " + _itemNameList[i]))
                {
                    _isDeletingItem = true;
                    _itemToDelete = i;
                }
            }
        }
        if(_isDeletingItem && !_deleteConfirmation)
        {
            if (GUILayout.Button("Are you sure you want to delete " + _itemNameList[_itemToDelete] + " ?"))
            {
                _deleteConfirmation = true;
            }
        }
        if(_deleteConfirmation)
        {
            ItemDatabase.DeleteItem(_itemToDelete);
            ClearLists();
        }
    }

    static void ClearLists()
    {
        _itemID.Clear();
        _itemNameList.Clear();
        _itemDescList.Clear();
        _itemTypeList.Clear();
        _itemStatsList.Clear();
    }

    public static void ClearValues()
    {
        _itemName = "";
        _itemDesc = "";
        _itemType = ItemType.Armour;
        _itemStats = 0;
    }

    public static void LoadResources()
    {
        // Load all objects from the Potions folder - Note: ALL objects so also the textures and meshes
        _loadPotions = Resources.LoadAll("Collectables/Potions/");

        for (int i = 0; i < _loadPotions.Length; i++)
        {
            // Create a filter so we only add the GameObjects to the loadPotionsName List
            if (_loadPotions[i].GetType().ToString() == "UnityEngine.GameObject")
            {

                _loadPotionsName.Add(_loadPotions[i].ToString().Remove(_loadPotions[i].ToString().Length - 25));

            }
        }

        _loadQuestItems = Resources.LoadAll("Collectables/QuestItems");

        for (int i = 0; i < _loadQuestItems.Length; i++)
        {
            if (_loadQuestItems[i].GetType().ToString() == "UnityEngine.GameObject")
            {
                _loadQuestItemsName.Add(_loadQuestItems[i].ToString().Remove(_loadQuestItems[i].ToString().Length - 25));
            }
        }
    }

    static void LoadFromDatabase()
    {
        ItemDatabase.GetAllItems();
        _itemID = ItemDatabase.ReturnItemIDs();
        _itemNameList = ItemDatabase.ReturnItemNames();
        _itemDescList = ItemDatabase.ReturnItemDescriptions();
        _itemTypeList = ItemDatabase.ReturnItemTypes();
        _itemStatsList = ItemDatabase.ReturnItemStats();
        _itemObjectIDList = ItemDatabase.ReturnItemIDs();
        _itemObjectList = ItemDatabase.ReturnItemObjects();

        
    }

    
}
#endif
      