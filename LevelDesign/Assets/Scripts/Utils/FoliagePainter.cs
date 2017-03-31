using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace LevelEditor
{

    public class FoliagePainter : EditorWindow
    {


        private GameObject _myPointer;
        private bool _addFoliage = false;
        private bool _addGrass = false;

        private bool _addNamesOnce = false;

        private UnityEngine.Object[] _grassObjects;
        private List<string> _foliageNames = new List<string>();
        private List<string> _objectsToPaint = new List<string>();
        private List<int> _amountToPaint = new List<int>();
        private float _paintDensity = 0.5f;
        private float _brushSize = 1.0f;

        private bool _randomScale = false;
        private bool _randomRotate = false;

        private float _randomScaleMin = 0.0f;
        private float _randomScaleMax = 0.0f;

        private float _randomRotateValueMin = 0.0f;
        private float _randomRotateValueMax = 0.0f;

        private int _selectIndex = 0;
        private List<int> _selectListIndex = new List<int>();

        private GameObject _parent;

        private Vector2 _scrollPos;

        [MenuItem("Level Design/World Builder/Foliage Painter")]
        static void ShowWindow()
        {
            FoliagePainter _fp = EditorWindow.GetWindow<FoliagePainter>();
        }

        void OnEnable()
        {
            _foliageNames.Clear();

            // Set the OnSceneGUI delegate to this OnSceneGUI
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            // Load ALL Objects from the Folder
            _grassObjects = Resources.LoadAll("World_Building/Foliage/");


            for (int i = 0; i < _grassObjects.Length; i++)
            {
                // if the type of the object in the folders matches "UnityEngine.GameObject"
                if (_grassObjects[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _foliageNames.Add(_grassObjects[i].ToString().Remove(_grassObjects[i].ToString().Length - 25));

                }
            }

            _selectListIndex.Clear();

        }

        // If we close the window
        void OnDisable()
        {
            // Remove the delegate
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

            // Remove the GameObject called "PAINT" ( the brush )
            DestroyImmediate(GameObject.Find("PAINT"));
        }


        void OnGUI()
        {
            if (!_addFoliage && !_addGrass)
            {
                if (GUILayout.Button("Add Foliage"))
                {
                    _addFoliage = true;
                }
            }


            if (_addFoliage)
            {
                if (GUILayout.Button("Paint Small Foliage"))
                {
                    _addGrass = true;
                }
            }

            if (_addGrass)
            {
                GUILayout.Label("Paint Small Foliage", EditorStyles.boldLabel);

                GUILayout.Label("[ BRUSH SETTINGS ]");

                _paintDensity = EditorGUILayout.Slider("Density: ", _paintDensity, 0, 1);
                _brushSize = EditorGUILayout.Slider("Brush Size: ", _brushSize, 0, 10);

                _randomScale = EditorGUILayout.Toggle("Random Scale: ", _randomScale);

                if (_randomScale)
                {
                    _randomScaleMin = EditorGUILayout.Slider("Min Scale: ", _randomScaleMin, 0.1f, 1f);
                    _randomScaleMax = EditorGUILayout.Slider("Max Scale: ", _randomScaleMax, 0f, 1.5f);
                }

                _randomRotate = EditorGUILayout.Toggle("Random Rotation: ", _randomRotate);

                if (_randomRotate)
                {
                    _randomRotateValueMin = EditorGUILayout.Slider("Min Rotation: ", _randomRotateValueMin, 0f, 360);
                    _randomRotateValueMax = EditorGUILayout.Slider("Max Rotation: ", _randomRotateValueMax, 0f, 360);
                }

                GUILayout.Label("[ GAMEOBJECTS SETTINGS ]");

                if (GUILayout.Button("Add GameObject"))
                {
                    _amountToPaint.Add(1);
                    _selectListIndex.Add(1);
                }

                if (GUILayout.Button("Remove GameObject"))
                {
                    _amountToPaint.RemoveAt(_amountToPaint.Count - 1);
                    _selectListIndex.RemoveAt(_selectListIndex.Count - 1);
                }

                if (_amountToPaint.Count > 0)
                {
                    for (int i = 0; i < _selectListIndex.Count; i++)
                    {
                        _selectListIndex[i] = EditorGUILayout.Popup(_selectListIndex[i], _foliageNames.ToArray());

                    }
                }

                //_amountToPaint = EditorGUILayout.IntField("Amount of GameObjects: ", _amountToPaint;)
                /*
                if (_amountToPaint <= _foliageNames.Count)
                {
                    if (!_addNamesOnce)
                    {
                        for (int i = 0; i < _amountToPaint; i++)
                        {
                            _selectListIndex.Add(i);
                            _addNamesOnce = true;
                        }

                    }


                    for (int i = 0; i < _selectListIndex.Count; i++)
                    {
                        _selectListIndex[i] = EditorGUILayout.Popup(_selectListIndex[i], _foliageNames.ToArray());
                        Debug.Log(_selectListIndex[i]);
                    }

                    Debug.Log(_selectListIndex.Count);

                }

                if(GUI.changed)
                {
                    _selectListIndex.Clear();
                    _addNamesOnce = false;
                }
                */

                if (GameObject.Find("PAINT") == null)
                {
                    _myPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    _myPointer.transform.localScale = new Vector3(_brushSize, 0.1f, _brushSize);
                    _myPointer.name = "PAINT";
                    _myPointer.layer = 2;
                }
                else
                {
                    _myPointer = GameObject.Find("PAINT");
                    _myPointer.transform.localScale = new Vector3(_brushSize, 0.1f, _brushSize);
                }
            }
        }

        void OnSceneGUI(SceneView _sceneView)
        {
            // We need to start with Handles.BeginGUI() 
            // Otherwise we can not use the OnSceneGUI

            Handles.BeginGUI();

            // For the scroll window
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_addGrass)
            {
                // GUIPointToWorldRay is used since we are not using a Camera 
                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;

                // If we cant find the GameObject "FOLIAGE" we add it to the game
                if (GameObject.Find("FOLIAGE") == null)
                {
                    GameObject _foliageParent = new GameObject();
                    _foliageParent.name = "FOLIAGE";
                }
                else
                {
                    _parent = GameObject.Find("FOLIAGE");
                }

                // the actual stuff

                if (Physics.Raycast(_ray, out _hit))
                {
                    Vector3 pos = _hit.point;
                    _myPointer.transform.position = pos;



                    // If we have pressed the left mouse button and the mouse button is DOWN
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                    {



                        // The following
                        // Mathf.Pow((float)3, _brushSize) * _paintDensity
                        // We calculate 3 ( predefined number ) to the power of the size of the brush
                        // We multiply that number by the paintDensity, since that is always a number <= 1 we limit the amount of objects painted

                        GameObject _childParent = new GameObject();
                        _childParent.name = "Foliage_Child";
                        _childParent.transform.parent = _parent.transform;

                        for (int i = 0; i < (Mathf.Pow((float)3, _brushSize) * (_paintDensity * _paintDensity)); i++)
                        {

                            float _scaleRandom = Random.Range(_randomScaleMin, _randomScaleMax);
                            float _rotateRandom = Random.Range(_randomRotateValueMin, _randomRotateValueMax);

                            for (int j = 0; j < _selectListIndex.Count; j++)
                            {
                                // Offset the position of the objects randomly based on half the size of the brushsize
                                GameObject _foliage = Instantiate(Resources.Load("World_Building/Foliage/" + _foliageNames[_selectListIndex[j]]), new Vector3(pos.x + (Random.Range(_brushSize / 2 * -1, _brushSize / 2)), pos.y, pos.z + (Random.Range(_brushSize / 2 * -1, _brushSize / 2))), Quaternion.identity) as GameObject;
                                if (_randomScale)
                                {
                                    _foliage.transform.localScale = new Vector3(_scaleRandom, _scaleRandom, _scaleRandom);
                                }
                                if (_randomRotate)
                                {
                                    _foliage.transform.eulerAngles = new Vector3(0, _rotateRandom, 0);
                                }

                                // for organising purposes we set every new GameObject as a child of the "FOLIAGE" GameObject
                                _foliage.transform.parent = _childParent.transform;

                            }
                        }

                    }
                }
            }
            EditorGUILayout.EndScrollView();
            Handles.EndGUI();
        }
    }
}
#endif