using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public class Environment : MonoBehaviour
    {
        private static UnityEngine.Object[] _loadEnvIcons;

        private static List<string> _environmentIcons = new List<string>();

        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;
        private static int _snapAmount;

        private static int _selectedIndex;
        private static bool _hasLoadedObjects;

        private static GameObject _objectToAdd;

        public static void AddStaticProps(int _numberOfRows)
        {
            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _environmentIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 50 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % _numberOfRows == 0)
                    {
                        _yPos++;
                        _xPos = 0;
                    }
                }

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Rocks/" + _environmentIcons[i]) as Texture2D);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    _snapAmount = 1;

                    EditorGUILayout.HelpBox(_environmentIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {

                        _selectedIndex = 0;
                        if(_objectToAdd != null)
                        {
                            _objectToAdd = null;
                            _selectedIndex = i;
                        }

                        _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _environmentIcons[i])) as GameObject;
                        _objectToAdd.transform.SetParent(GameObject.Find("STATICPROPS").transform);
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void LoadAll()
        {
            _environmentIcons.Clear();

            _loadEnvIcons = Resources.LoadAll("World_Building/ICONS/Rocks");

            for (int i = 0; i < _loadEnvIcons.Length; i++)
            {
                if (_loadEnvIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _environmentIcons.Add(_loadEnvIcons[i].ToString().Remove(_loadEnvIcons[i].ToString().Length - 24));
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

        public static void NextObject()
        {
            if (_selectedIndex + 1 < _environmentIcons.Count)
            {
                _selectedIndex++;
                if (_objectToAdd != null)
                {
                    DestroyImmediate(_objectToAdd);
                }
                if (Resources.Load("World_Building/Rocks/" + _environmentIcons[_selectedIndex]) != null)
                {
                    _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _environmentIcons[_selectedIndex])) as GameObject;
                }
            }
        }

        public static void PreviousObject()
        {
            if (_selectedIndex - 1 >= 0)
            {
                _selectedIndex--;
                if (_objectToAdd != null)
                {
                    DestroyImmediate(_objectToAdd);
                }
                if (Resources.Load("World_Building/Rocks/" + _environmentIcons[_selectedIndex]) != null)
                {
                    _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _environmentIcons[_selectedIndex])) as GameObject;
                }
            }
        }

        public static void DeleteLoadedObject()
        {
            if(_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }

    }
}
