using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public class Dungeon : MonoBehaviour
    {
        private static UnityEngine.Object[] _loadDungeonPropsIcons;
        private static UnityEngine.Object[] _loadDungeonBordersIcons;
        private static UnityEngine.Object[] _loadDungeonTilesIcons;
        private static UnityEngine.Object[] _loadDungeonWallsIcons;

        private static List<string> _dungeonPropsIcons = new List<string>();
        private static List<string> _dungeonBordersIcons = new List<string>();
        private static List<string> _dungeonTilesIcons = new List<string>();
        private static List<string> _dungeonWallsIcons = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;

        private static int _selectedIndex;

        private static GameObject _objectToAdd;

        private static bool _hasLoadedObjects;
        private static int _snapAmount;

        public static void ShowAddBuildings(int _numberOfRows)
        {
            GUILayout.Label("This theme does not have any buildings");
        }

        public static void ShowAddTiles(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _dungeonTilesIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewTilesOffset + (_previewWindow * _yPos + 10), _previewWindow, 100);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Dungeon/Tiles/" + _dungeonTilesIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_dungeonTilesIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _selectedIndex = 0;
                        if(_objectToAdd != null)
                        {
                            _objectToAdd = null;
                            _selectedIndex = i;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddProps(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _dungeonPropsIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, 100);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Dungeon/Props/" + _dungeonPropsIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_dungeonPropsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _selectedIndex = 0;
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                            _selectedIndex = i;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddPerimeter(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _dungeonWallsIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, 100);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Dungeon/Walls/" + _dungeonWallsIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_dungeonWallsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _selectedIndex = 0;
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                            _selectedIndex = i;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddBorders(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _dungeonBordersIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, 100);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Dungeon/Borders/" + _dungeonBordersIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_dungeonBordersIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _selectedIndex = 0;
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                            _selectedIndex = i;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void LoadAll()
        {
            _loadDungeonPropsIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Props");
            _loadDungeonBordersIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Borders");
            _loadDungeonTilesIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Tiles");
            _loadDungeonWallsIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Walls");

            for (int i = 0; i < _loadDungeonPropsIcons.Length; i++)
            {
                if (_loadDungeonPropsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _dungeonPropsIcons.Add(_loadDungeonPropsIcons[i].ToString().Remove(_loadDungeonPropsIcons[i].ToString().Length - 24));


                }
            }

            for (int i = 0; i < _loadDungeonBordersIcons.Length; i++)
            {
                if (_loadDungeonBordersIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _dungeonBordersIcons.Add(_loadDungeonBordersIcons[i].ToString().Remove(_loadDungeonBordersIcons[i].ToString().Length - 24));


                }
            }

            for (int i = 0; i < _loadDungeonTilesIcons.Length; i++)
            {
                if (_loadDungeonTilesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _dungeonTilesIcons.Add(_loadDungeonTilesIcons[i].ToString().Remove(_loadDungeonTilesIcons[i].ToString().Length - 24));


                }
            }

            for (int i = 0; i < _loadDungeonWallsIcons.Length; i++)
            {
                if (_loadDungeonWallsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _dungeonWallsIcons.Add(_loadDungeonWallsIcons[i].ToString().Remove(_loadDungeonWallsIcons[i].ToString().Length - 24));


                }
            }
            _hasLoadedObjects = true;
        }

        public static bool ReturnHasLoadedObjects()
        {
            return _hasLoadedObjects;
        }

        public static GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }

        public static int ReturnSnapAmount()
        {
            return _snapAmount;
        }

        public static void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                _objectToAdd = null;
            }
        }

        public static void NextObject(int _tabIndex)
        {
            switch (_tabIndex)
            {
                case 0:
                    // buildings
                   
                    break;
                case 1:
                    if (_selectedIndex + 1 < _dungeonTilesIcons.Count)
                    {
                        _selectedIndex++;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 2:
                    // perimeter
                    if (_selectedIndex + 1 < _dungeonWallsIcons.Count)
                    {
                        _selectedIndex++;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 3:
                    // props
                    if (_selectedIndex + 1 < _dungeonPropsIcons.Count)
                    {
                        _selectedIndex++;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 4:
                    if (_selectedIndex + 1 < _dungeonBordersIcons.Count)
                    {
                        _selectedIndex++;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static void PreviousObject(int _tabIndex)
        {

            switch (_tabIndex)
            {
                case 0:
                    // buildings

                    break;
                case 1:
                    if (_selectedIndex - 1 >= 0)
                    {
                        _selectedIndex--;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 2:
                    // perimeter
                    if (_selectedIndex - 1 >= 0)
                    {
                        _selectedIndex--;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 3:
                    // props
                    if (_selectedIndex - 1 >= 0)
                    {
                        _selectedIndex--;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                case 4:
                    if (_selectedIndex - 1 >= 0)
                    {
                        _selectedIndex--;
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        if (Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[_selectedIndex]) != null)
                        {
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[_selectedIndex])) as GameObject;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}