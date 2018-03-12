using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;

namespace Theme
{
    public class LevelItems : MonoBehaviour
    {
        private static List<string> _AllPotionNames = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;

        private static int _snapAmount;

        private static GameObject _objectToAdd;

        private static List<int> _itemID = new List<int>();
        private static List<string> _itemName = new List<string>();
        private static List<string> _itemDesc = new List<string>();
        private static List<ItemType> _itemType = new List<ItemType>();
        private static List<int> _itemStats = new List<int>();
        private static List<string> _itemObject = new List<string>();

        private static Editor _gameObjectEditor;
        private static UnityEngine.Object[] _loadAllPotions;

        private static GUISkin _skin;

        public static void LoadAll()
        {
            ClearItems();

            _loadAllPotions = Resources.LoadAll("Items/Potions/");
            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;

            for (int i = 0; i < _loadAllPotions.Length; i++)
            {
                if (_loadAllPotions[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _AllPotionNames.Add(_loadAllPotions[i].ToString().Remove(_loadAllPotions[i].ToString().Length - 25));
                }
            }

            GetAllItems();
        }

        public static void AddPotions(int _numberOfRows)
        {

            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            GUILayout.Button("Add an Potion", EditorStyles.boldLabel);

            for (int i = 0; i < _AllPotionNames.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 150 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {
                        _yPos++;
                        _xPos = 0;
                    }
                }

                _gameObjectEditor = Editor.CreateEditor(Resources.Load("Items/Potions/" + _AllPotionNames[i]));
                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {

                    EditorGUILayout.HelpBox(_AllPotionNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _objectToAdd = Instantiate(Resources.Load("Items/Potions/" + _AllPotionNames[i])) as GameObject;
                        if (GameObject.Find("POTIONS") != null)
                        {
                            _objectToAdd.transform.SetParent(GameObject.Find("POTIONS").transform);
                        }
                        else
                        {
                            GameObject _potionParent = new GameObject();
                            _potionParent.name = "POTIONS";

                            _objectToAdd.transform.SetParent(_potionParent.transform);

                        }

                        _objectToAdd.AddComponent<SphereCollider>();
                        _objectToAdd.GetComponent<SphereCollider>().isTrigger = true;
                        _objectToAdd.GetComponent<SphereCollider>().radius = 1.5f;

                        _objectToAdd.AddComponent<ItemCollectable>();
                        _objectToAdd.GetComponent<ItemCollectable>().SetValues(_itemID[i], _itemName[i], _itemType[i].ToString(), _itemStats[i]);

                        LevelEditor.ObjectPainter.SetAddingToScene();

                        Event.current.Use();
                    }
                }
            }
        }

        private static void GetAllItems()
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
                _itemName.Add(reader.GetString(1));
                _itemDesc.Add(reader.GetString(2));
                if (reader.GetString(3) == "Weapon")
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
                _itemObject.Add(reader.GetString(6));

            }

        }

        private static void ClearItems()
        {
            _itemID.Clear();
            _itemName.Clear();
            _itemDesc.Clear();
            _itemType.Clear();
            _itemStats.Clear();
            _itemObject.Clear();
            _AllPotionNames.Clear();
        }

        public static GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }

        public static int ReturnSnapAmount()
        {
            return _snapAmount;
        }

    }
}
