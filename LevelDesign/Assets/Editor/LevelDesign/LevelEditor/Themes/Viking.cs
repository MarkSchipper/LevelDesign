using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public class Viking : MonoBehaviour
    {

        private static int _snapAmount;

        private static UnityEngine.Object[] _loadVikingPropsIcons;
        private static UnityEngine.Object[] _loadVikingBuildingsIcons;
        private static UnityEngine.Object[] _loadVikingPerimeterIcons;
        private static UnityEngine.Object[] _loadVikingEdgesIcons;
        private static UnityEngine.Object[] _loadVikingSurfacesIcons;

        private static List<string> _vikingPropsIcons = new List<string>();
        private static List<string> _vikingBuildingsIcons = new List<string>();
        private static List<string> _vikingPerimeterIcons = new List<string>();
        private static List<string> _vikingEdgesIcons = new List<string>();
        private static List<string> _vikingSurfacesIcons = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;

        private static GameObject _objectToAdd;

        private static bool _hasLoadedObjects;

        public static void ShowAddBuildings(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _vikingBuildingsIcons.Count; i++)
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
                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Viking/Buildings/" + _vikingBuildingsIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_vikingBuildingsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Buildings/" + _vikingBuildingsIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddTiles(int _numberOfRows)
        {

        }

        public static void ShowAddSurfaces(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _vikingSurfacesIcons.Count; i++)
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

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Viking/Tiles/Surfaces/" + _vikingSurfacesIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {

                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Surfaces/" + _vikingSurfacesIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void ShowAddEdges(int _numberOfRows)
        {

            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _vikingEdgesIcons.Count; i++)
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

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Viking/Tiles/Edges/" + _vikingEdgesIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {

                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if (_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Edges/" + _vikingEdgesIcons[i])) as GameObject;
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

            for (int i = 0; i < _vikingPropsIcons.Count; i++)
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
                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Viking/Props/" + _vikingPropsIcons[i]) as Texture2D);
                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_vikingPropsIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if(_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Props/" + _vikingPropsIcons[i])) as GameObject;
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

            for (int i = 0; i < _vikingPerimeterIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), _previewOffset + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);


                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % 3 == 0)
                    {

                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Viking/Perimeter/" + _vikingPerimeterIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_vikingPerimeterIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if(_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Perimeter/" + _vikingPerimeterIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void LoadAll()
        {
            _loadVikingBuildingsIcons = Resources.LoadAll("World_Building/ICONS/Viking/Buildings");
            _loadVikingEdgesIcons = Resources.LoadAll("World_Building/ICONS/Viking/Tiles/Edges");
            _loadVikingPerimeterIcons = Resources.LoadAll("World_Building/ICONS/Viking/Perimeter");
            _loadVikingPropsIcons = Resources.LoadAll("World_Building/ICONS/Viking/Props");
            _loadVikingSurfacesIcons = Resources.LoadAll("World_Building/ICONS/Viking/Tiles/Surfaces");

            #region VIKING ICONS
            for (int i = 0; i < _loadVikingBuildingsIcons.Length; i++)
            {
                if (_loadVikingBuildingsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingBuildingsIcons.Add(_loadVikingBuildingsIcons[i].ToString().Remove(_loadVikingBuildingsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadVikingEdgesIcons.Length; i++)
            {
                if (_loadVikingEdgesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingEdgesIcons.Add(_loadVikingEdgesIcons[i].ToString().Remove(_loadVikingEdgesIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadVikingPerimeterIcons.Length; i++)
            {
                if (_loadVikingPerimeterIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingPerimeterIcons.Add(_loadVikingPerimeterIcons[i].ToString().Remove(_loadVikingPerimeterIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadVikingPropsIcons.Length; i++)
            {
                if (_loadVikingPropsIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingPropsIcons.Add(_loadVikingPropsIcons[i].ToString().Remove(_loadVikingPropsIcons[i].ToString().Length - 24));

                }
            }

            for (int i = 0; i < _loadVikingSurfacesIcons.Length; i++)
            {
                if (_loadVikingSurfacesIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingSurfacesIcons.Add(_loadVikingSurfacesIcons[i].ToString().Remove(_loadVikingSurfacesIcons[i].ToString().Length - 24));

                }
            }
            #endregion

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
        public static void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }

    }
}