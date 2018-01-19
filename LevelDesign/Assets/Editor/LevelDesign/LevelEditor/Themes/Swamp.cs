using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public class Swamp : MonoBehaviour
    {
        private static bool _hasLoadedObjects;

        private static int _snapAmount;

        private static UnityEngine.Object[] _loadSwampPropsIcons;
        private static UnityEngine.Object[] _loadSwampBuildingsIcons;
        private static UnityEngine.Object[] _loadSwampTilesIcons;
        private static UnityEngine.Object[] _loadSwampTreesIcons;

        private static List<string> _swampPropsIcons = new List<string>();
        private static List<string> _swampBuildingsIcons = new List<string>();
        private static List<string> _swampTilesIcons = new List<string>();
        private static List<string> _swampTreesIcons = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;

        private static GameObject _objectToAdd;

        public static void ShowAddBuildings(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _swampBuildingsIcons.Count; i++)
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
                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Swamp/Buildings/" + _swampBuildingsIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_swampBuildingsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Swamp/Buildings/" + _swampBuildingsIcons[i])) as GameObject;
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

            for (int i = 0; i < _swampTilesIcons.Count; i++)
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


                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Swamp/Tiles/" + _swampTilesIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Swamp/Tiles/" + _swampTilesIcons[i])) as GameObject;
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

            for (int i = 0; i < _swampPropsIcons.Count; i++)
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

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Swamp/Props/" + _swampPropsIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_swampPropsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Swamp/Props/" + _swampPropsIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddTrees(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _swampTreesIcons.Count; i++)
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

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Swamp/Trees/" + _swampTreesIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_swampTreesIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Swamp/Trees/" + _swampTreesIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static bool ReturnHasLoadedObjects()
        {
            return _hasLoadedObjects;
        }

        public static GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }
        public static void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }


        public static void LoadAll()
        {
            _loadSwampBuildingsIcons = Resources.LoadAll("World_Building/ICONS/Swamp/Buildings");
            _loadSwampTilesIcons = Resources.LoadAll("World_Building/ICONS/Swamp/Tiles");
            _loadSwampPropsIcons = Resources.LoadAll("World_Building/ICONS/Swamp/Props");
            _loadSwampTreesIcons = Resources.LoadAll("World_Building/ICONS/Swamp/Trees");

            #region SWAMP ICONS
            for (int i = 0; i < _loadSwampBuildingsIcons.Length; i++)
            {
                if (_loadSwampBuildingsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _swampBuildingsIcons.Add(_loadSwampBuildingsIcons[i].ToString().Remove(_loadSwampBuildingsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSwampPropsIcons.Length; i++)
            {
                if (_loadSwampPropsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _swampPropsIcons.Add(_loadSwampPropsIcons[i].ToString().Remove(_loadSwampPropsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSwampTilesIcons.Length; i++)
            {
                if (_loadSwampTilesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _swampTilesIcons.Add(_loadSwampTilesIcons[i].ToString().Remove(_loadSwampTilesIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadSwampTreesIcons.Length; i++)
            {
                if (_loadSwampTreesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _swampTreesIcons.Add(_loadSwampTreesIcons[i].ToString().Remove(_loadSwampTreesIcons[i].ToString().Length - 24));

                }
            }
            #endregion

            _hasLoadedObjects = true;
        }

    }
}
