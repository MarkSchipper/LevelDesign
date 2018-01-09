using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{
    public class Graveyard : MonoBehaviour
    {

        private static UnityEngine.Object[] _loadGraveyardIcons;

        private static List<string> _graveyardIcons = new List<string>();

        private static bool _hasLoadedObjects;
        private static GameObject _objectToAdd;
        private static int _snapAmount;
        private static int _previewWindow = 128;
        private static int _previewOffset = 175;
        private static int _previewTilesOffset = 200;


        public static void ShowGraveyard(int _numberOfRows)
        {
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

            for (int i = 0; i < _graveyardIcons.Count; i++)
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

                EditorGUI.DrawPreviewTexture(_previewRect[i], Resources.Load("World_Building/ICONS/Graveyard/" + _graveyardIcons[i]) as Texture2D);

                //EditorGUILayout.LabelField("test", _nameRect[i]);

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_graveyardIcons[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        if(_objectToAdd != null)
                        {
                            _objectToAdd = null;
                        }
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Graveyard/" + _graveyardIcons[i])) as GameObject;
                        LevelEditor.ObjectPainter.SetAddingToScene();
                        Event.current.Use();
                    }
                }
            }
        }

        public static void LoadAll()
        {
            _loadGraveyardIcons = Resources.LoadAll("World_Building/ICONS/Graveyard");

            for (int i = 0; i < _loadGraveyardIcons.Length; i++)
            {
                if (_loadGraveyardIcons[i].GetType().ToString() == "UnityEngine.Texture2D")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _graveyardIcons.Add(_loadGraveyardIcons[i].ToString().Remove(_loadGraveyardIcons[i].ToString().Length - 24));

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

        public static void DeleteLoadedObject()
        {
            if (_objectToAdd != null)
            {
                DestroyImmediate(_objectToAdd);
            }
        }
    }
}