using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Theme
{
    public class Settlement : MonoBehaviour
    {
        private static int _snapAmount;

        private static UnityEngine.Object[] _loadSettlementPropsIcons;
        private static UnityEngine.Object[] _loadSettlementBuildingsIcons;
        private static UnityEngine.Object[] _loadSettlementPerimeterIcons;
        private static UnityEngine.Object[] _loadSettlementTilesIcons;

        private static List<string> _settlementPropsIcons = new List<string>();
        private static List<string> _settlementBuildingsIcons = new List<string>();
        private static List<string> _settlementPerimeterIcons = new List<string>();
        private static List<string> _settlementTilesIcons = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;

        private static GameObject _objectToAdd;

        private static bool _hasLoadedObjects;

        public static void ShowAddBuildings(int _numberOfRows)
        {
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            for (int i = 0; i < _settlementBuildingsIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Settlement/Buildings/" + _settlementBuildingsIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_settlementBuildingsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Buildings/" + _settlementBuildingsIcons[i])) as GameObject;
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

            for (int i = 0; i < _settlementPerimeterIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Settlement/Perimeter/" + _settlementPerimeterIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_settlementPerimeterIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Perimeter/" + _settlementPerimeterIcons[i])) as GameObject;
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

            for (int i = 0; i < _settlementPropsIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Settlement/Props/" + _settlementPropsIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_settlementPropsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Props/" + _settlementPropsIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddTiles(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _settlementTilesIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewTilesOffset + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                _xPos++;

                if (i > 0)
                {
                    if (i % _numberOfRows == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }


                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Settlement/Tiles/" + _settlementTilesIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Tiles/" + _settlementTilesIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void LoadAll()
        {
            _settlementBuildingsIcons.Clear();
            _settlementPerimeterIcons.Clear();
            _settlementPropsIcons.Clear();
            _settlementTilesIcons.Clear();

            _loadSettlementBuildingsIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Buildings");
            _loadSettlementPerimeterIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Perimeter");
            _loadSettlementPropsIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Props");
            _loadSettlementTilesIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Tiles");

            for (int i = 0; i < _loadSettlementBuildingsIcons.Length; i++)
            {
                if (_loadSettlementBuildingsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementBuildingsIcons.Add(_loadSettlementBuildingsIcons[i].ToString().Remove(_loadSettlementBuildingsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSettlementPerimeterIcons.Length; i++)
            {
                if (_loadSettlementPerimeterIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementPerimeterIcons.Add(_loadSettlementPerimeterIcons[i].ToString().Remove(_loadSettlementPerimeterIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSettlementPropsIcons.Length; i++)
            {
                if (_loadSettlementPropsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementPropsIcons.Add(_loadSettlementPropsIcons[i].ToString().Remove(_loadSettlementPropsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSettlementTilesIcons.Length; i++)
            {
                if (_loadSettlementTilesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementTilesIcons.Add(_loadSettlementTilesIcons[i].ToString().Remove(_loadSettlementTilesIcons[i].ToString().Length - 24));

                }
            }

            _hasLoadedObjects = true;
        }

        public static GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }

        public static bool ReturnHasLoadedObjects()
        {
            return _hasLoadedObjects;
        }

        public static void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }

    }
}