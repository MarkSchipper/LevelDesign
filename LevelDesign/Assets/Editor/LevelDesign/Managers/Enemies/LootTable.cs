using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LootTable : MonoBehaviour
{

    private static string _lootTableName;
    private static List<LootTypes> _lootType = new List<LootTypes>();
    private static List<int> _lootTypeIndex = new List<int>();
    private static List<int> _lootValue = new List<int>();
    private static List<int> _lootWeight = new List<int>();
    private static List<int> _itemID = new List<int>();

    private static int _lootID;
    private static int _lootSelectIndex;

    private static bool _isCleared;
    private static bool _loadedDatabase;

    public static void ShowAddLootTable()
    {
        if (_lootTypeIndex.Count == 0)
        {
            _lootTypeIndex.Add(0);
            _lootType.Add(LootTypes.Gold);
            _lootValue.Add(0);
            _lootWeight.Add(0);
            _itemID.Add(0);
        }
        GUILayout.Space(20);
        _lootTableName = EditorGUILayout.TextField("Table name: ", _lootTableName);
        GUILayout.Space(10);
        for (int i = 0; i < _lootTypeIndex.Count; i++)
        {
            GUILayout.BeginHorizontal();
            _lootTypeIndex[i] = i;
            GUILayout.Label("Item: ");
            _lootType[i] = (LootTypes)EditorGUILayout.EnumPopup(_lootType[i]);
            if (_lootType[i] == LootTypes.Items)
            {
                if (ItemDatabase.ReturnItemNames().Count == 0)
                {
                    ItemDatabase.GetAllItems();
                }
                _itemID[i] = EditorGUILayout.Popup(_itemID[i], ItemDatabase.ReturnItemNames().ToArray());
            }
            if (_lootType[i] == LootTypes.Gold)
            {
                _lootValue[i] = EditorGUILayout.IntField("Value: ", _lootValue[i]);
            }
            _lootWeight[i] = EditorGUILayout.IntField("Weight: ( in % )", _lootWeight[i]);

            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+"))
        {
            _lootTypeIndex.Add(0);
            _lootType.Add(LootTypes.Gold);
            _lootValue.Add(0);
            _lootWeight.Add(0);
            _itemID.Add(0);
        }

        if (GUILayout.Button("Save Loot Table"))
        {
            for (int i = 0; i < _lootTypeIndex.Count; i++)
            {
                LootDatabase.AddLootTable(_lootTableName, _lootType[i].ToString(), _lootValue[i], ItemDatabase.ReturnItemID(_itemID[i]), _lootWeight[i]);
            }
            _isCleared = false;
            ClearAll();
        }

    }

    public static void ShowEditLootTable()
    {

        GUILayout.Space(20);
        _isCleared = false;
        if (!_loadedDatabase)
        {
            LootDatabase.GetAllLootTables();
            LootDatabase.GetLootTable(LootDatabase.ReturnLootTableNames()[_lootSelectIndex]);
            _lootTableName = EditorGUILayout.TextField(LootDatabase.ReturnLootTableNames()[_lootSelectIndex]);
            _lootType = LootDatabase.ReturnLootTypeByTable();
            _itemID = LootDatabase.ReturnItemIDByTable();
            _lootValue = LootDatabase.ReturnLootValueByTable();
            _lootWeight = LootDatabase.ReturnLootWeightByTable();
            _loadedDatabase = true;
        }

        EditorGUI.BeginChangeCheck();
        _lootSelectIndex = EditorGUILayout.Popup(_lootSelectIndex, LootDatabase.ReturnLootTableNames().ToArray());

        if(EditorGUI.EndChangeCheck())
        {
            _loadedDatabase = false;
        }

        _lootTableName = EditorGUILayout.TextField(_lootTableName);

        for (int i = 0; i < LootDatabase.ReturnLootIdByTable().Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + LootDatabase.ReturnLootIdByTable()[i]);
            GUILayout.Label("Item: ");
            _lootType[i] = (LootTypes)EditorGUILayout.EnumPopup(_lootType[i]);
            if (_lootType[i] == LootTypes.Items)
            {
                if (ItemDatabase.ReturnItemNames().Count == 0)
                {
                    ItemDatabase.GetAllItems();
                }
                _itemID[i] = EditorGUILayout.Popup(_itemID[i], ItemDatabase.ReturnItemNames().ToArray());
            }
            if (_lootType[i] == LootTypes.Gold)
            {
                _lootValue[i] = EditorGUILayout.IntField("Value: ", _lootValue[i]);
            }
            _lootWeight[i] = EditorGUILayout.IntField("Weight: ( in % )", _lootWeight[i]);

            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Save to Database"))
        {
            for (int i = 0; i < LootDatabase.ReturnLootIdByTable().Count; i++)
            {
                LootDatabase.UpdateLootTable(LootDatabase.ReturnLootIdByTable()[i], _lootTableName, _lootType[i].ToString(), _lootValue[i], _itemID[i], _lootWeight[i]);
            }
        }
    }


    public static void ShowDeleteLootTable()
    {
        _isCleared = false;
        GUILayout.Space(20);
        if (!_loadedDatabase)
        {
            LootDatabase.GetAllLootTables();
            _loadedDatabase = true;
        }
        if (LootDatabase.ReturnLootID().Count > 0)
        {
            for (int i = 0; i < LootDatabase.ReturnLootID().Count; i++)
            {
                if (GUILayout.Button("Delete " + LootDatabase.ReturnLootTableNames()[i]))
                {
                    LootDatabase.DeleteLootTable(LootDatabase.ReturnLootTableNames()[i]);
                    _loadedDatabase = false;
                }
            }
        }
        else
        {
            GUILayout.Label("Nothing to delete");
        }
    }

    public static void ClearAll()
    {
        if (!_isCleared)
        {
            _lootTableName = "";
            _lootType.Clear();
            _lootTypeIndex.Clear();
            _lootValue.Clear();
            _lootWeight.Clear();
            _itemID.Clear();

            _isCleared = true;
        }
    }

    public static void ReloadDatabase()
    {
        _loadedDatabase = false;
    }
}
