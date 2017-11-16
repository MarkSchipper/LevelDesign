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
        private bool _addTrees = false;
        private bool _addDecals = false;
        
        private bool _addNamesOnce = false;

        private UnityEngine.Object[] _grassObjects;
        private UnityEngine.Object[] _treeObjects;
        private UnityEngine.Object[] _decalObjects;

        private List<string> _foliageNames = new List<string>();
        private List<string> _treeNames = new List<string>();
        private List<string> _decalNames = new List<string>();

        private List<string> _objectsToPaint = new List<string>();
        private List<int> _amountToPaint = new List<int>();
        private float _paintDensity = 0.5f;
        private float _brushSize = 1.0f;

        private bool _randomScale = false;
        private bool _randomRotate = false;

        private bool _checkOnce = false;

        private float _randomScaleMin = 0.0f;
        private float _randomScaleMax = 0.0f;

        private float _randomRotateValueMin = 0.0f;
        private float _randomRotateValueMax = 0.0f;

        private int _previewWindow = 128;
        private Editor _gameObjectEditor;

        private GUISkin _skin;

        private int _selectIndex = 0;
        private List<int> _selectListIndex = new List<int>();

        private GameObject _parent;
        private GameObject _objectToAdd;
        private bool _isAddingToScene;

        private int _tabIndex = -1;
        private int _foliageTabIndex = -1;

        private Vector2 _scrollPos;

        static void ShowWindow()
        {
            FoliagePainter _fp = EditorWindow.GetWindow<FoliagePainter>();
        }

        void OnEnable()
        {
            _foliageNames.Clear();
            _treeNames.Clear();

            // Set the OnSceneGUI delegate to this OnSceneGUI
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            // Load ALL Objects from the Folder
            _grassObjects = Resources.LoadAll("World_Building/Foliage/");
            _treeObjects = Resources.LoadAll("World_Building/Trees");
            _decalObjects = Resources.LoadAll("Decals");


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

            for (int i = 0; i < _treeObjects.Length; i++)
            {
                // if the type of the object in the folders matches "UnityEngine.GameObject"
                if (_treeObjects[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _treeNames.Add(_treeObjects[i].ToString().Remove(_treeObjects[i].ToString().Length - 25));
                }
            }

            for (int i = 0; i < _decalObjects.Length; i++)
            {
                // if the type of the object in the folders matches "UnityEngine.GameObject"
                if (_decalObjects[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _decalNames.Add(_decalObjects[i].ToString().Remove(_decalObjects[i].ToString().Length - 25));

                }
            }
            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
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
            _tabIndex = GUILayout.Toolbar(_tabIndex, new string[] { "Add Foliage", "Add Decals" });
            switch (_tabIndex)
            {
                case 0:
                    ShowAddFoliage();
                    break;
                case 1:
                    AddDecals();
                    break;
                default:
                    break;
            }
           
        }

        void OnSceneGUI(SceneView _sceneView)
        {
            // We need to start with Handles.BeginGUI() 
            // Otherwise we can not use the OnSceneGUI

            Handles.BeginGUI();

            // For the scroll window
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            #region GRASS
            if (_addGrass)
            {
                // GUIPointToWorldRay is used since we are not using a Camera 
                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;
           
             
                // the actual stuff

                if (Physics.Raycast(_ray, out _hit))
                {
                    Vector3 pos = _hit.point;
                    _myPointer.transform.position = pos;



                    // If we have pressed the left mouse button and the mouse button is DOWN
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                    {

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

            #endregion

            #region TREES
            if(_addTrees)
            {
                if (_isAddingToScene)
                {
                    // GUIPointToWorldRay is used since we are not using a Camera 
                    Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit _hit;


                    // the actual stuff

                    if (Physics.Raycast(_ray, out _hit))
                    {
                        Vector3 pos = _hit.point;
                        _myPointer.transform.position = pos;



                        // If we have pressed the left mouse button and the mouse button is DOWN
                        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                        {

                            // If we cant find the GameObject "FOLIAGE" we add it to the game
                            if (GameObject.Find("TREES") == null)
                            {
                                GameObject _foliageParent = new GameObject();
                                _foliageParent.name = "TREES";
                            }
                            else
                            {
                                _parent = GameObject.Find("TREES");
                            }

                            // The following
                            // Mathf.Pow((float)3, _brushSize) * _paintDensity
                            // We calculate 3 ( predefined number ) to the power of the size of the brush
                            // We multiply that number by the paintDensity, since that is always a number <= 1 we limit the amount of objects painted

                            for (int i = 0; i < (Mathf.Pow((float)3, _brushSize) * (_paintDensity / 75 * _paintDensity / 75)); i++)
                            {
                                float _scaleRandom = Random.Range(_randomScaleMin, _randomScaleMax);
                                float _rotateRandom = Random.Range(_randomRotateValueMin, _randomRotateValueMax);

                                // Offset the position of the objects randomly based on half the size of the brushsize
                                GameObject _foliage = Instantiate(_objectToAdd, new Vector3(pos.x + (Random.Range(_brushSize / 2 * -1, _brushSize / 2)), pos.y, pos.z + (Random.Range(_brushSize / 2 * -1, _brushSize / 2))), Quaternion.identity) as GameObject;
                                if (_randomScale)
                                {
                                    _foliage.transform.localScale = new Vector3(_scaleRandom, _scaleRandom, _scaleRandom);
                                }
                                if (_randomRotate)
                                {
                                    _foliage.transform.eulerAngles = new Vector3(0, _rotateRandom, 0);
                                }

                                // for organising purposes we set every new GameObject as a child of the "FOLIAGE" GameObject
                                _foliage.transform.parent = _parent.transform;
                                
                            }
                        }
                    }
                }
            }
            #endregion

            #region DECALS
            if (_addDecals)
            {
                if (_isAddingToScene)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    Vector3 _newPos;
                    // GUIPointToWorldRay is used since we are not using a Camera 
                    Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit _hit;


                    // the actual stuff

                    if (Physics.Raycast(_ray, out _hit))
                    {
                        // Set the object layer to 2 ( IGNORE RAYCAST ) so we can raycast on the new object
                        _objectToAdd.layer = 2;

                        // Snapping
                        _newPos = new Vector3(_hit.point.x, _hit.point.y + 0.1f, _hit.point.z);
                        _objectToAdd.transform.position = _newPos;
                        _objectToAdd.transform.localScale = new Vector3(_brushSize, 1f, _brushSize);

                        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                        {
                            #region SAFEGUARDS
                            //////////////////////////////////////////////////////////////////////
                            //                              Safeguards                          //
                            //////////////////////////////////////////////////////////////////////

                            if (GameObject.Find("WORLD") == null)
                            {
                                GameObject _world = new GameObject();
                                _world.name = "WORLD";
                            }

                            if (GameObject.Find("PROPS") == null)
                            {
                                GameObject _props = new GameObject();
                                _props.name = "PROPS";
                                _props.transform.SetParent(GameObject.Find("WORLD").transform);
                            }

                            if (GameObject.Find("POTIONS") == null)
                            {
                                GameObject _potions = new GameObject();
                                _potions.name = "POTIONS";
                                _potions.transform.SetParent(GameObject.Find("WORLD").transform);
                            }
                            if (GameObject.Find("STATICPROPS") == null)
                            {
                                GameObject _staticProps = new GameObject();
                                _staticProps.name = "STATICPROPS";
                                _staticProps.transform.SetParent(GameObject.Find("WORLD").transform);
                            }
                            if(GameObject.Find("DECALS") == null)
                            {
                                GameObject _decals = new GameObject();
                                _decals.name = "DECALS";
                                _decals.transform.SetParent(GameObject.Find("WORLD").transform);
                            }
                            #endregion

                            _objectToAdd.transform.SetParent(GameObject.Find("DECALS").transform);

                            //Selection.
                            _isAddingToScene = false;

                            
                            //HandleUtility.PickGameObject(Event.current.mousePosition, true);

                                                   }
                        if (Event.current.button == 1 && Event.current.type == EventType.mouseDown)
                        {
                            DestroyImmediate(_objectToAdd);
                            _isAddingToScene = false;
                        }
                    }
                    if (Event.current.keyCode == (KeyCode.A) && Event.current.type == EventType.keyDown)
                    {
                        _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y + 22.5f, _objectToAdd.transform.eulerAngles.z);
                    }
                    if (Event.current.keyCode == (KeyCode.D) && Event.current.type == EventType.keyDown)
                    {
                        _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y - 22.5f, _objectToAdd.transform.eulerAngles.z);
                    }
                }
            }
            #endregion
            EditorGUILayout.EndScrollView();
            Handles.EndGUI();
        }

        void ShowAddFoliage()
        {
            _foliageTabIndex = GUILayout.Toolbar(_foliageTabIndex, new string[] { "Small Foliage", "Trees" });
            switch (_foliageTabIndex)
            {
                case 0:
                    AddGrass();
                    break;
                case 1:
                    AddTrees();
                    break;
                default:
                    break;
            }
        }

        void AddGrass()
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

            if (!_checkOnce)
            {
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
                }
                _checkOnce = true;
            }
            _myPointer.transform.localScale = new Vector3(_brushSize, 0.1f, _brushSize);
            _myPointer.GetComponent<Renderer>().material = Resources.Load("World_Building/PainterMaterial") as Material;
        }

        void AddTrees()
        {
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


            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _treeNames.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 250 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % 3 == 0)
                    {
                        _yPos++;
                        _xPos = 0;
                    }
                }

                _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Trees/" + _treeNames[i]));
                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_decalNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        _isAddingToScene = true;
                        _objectToAdd = Resources.Load("World_Building/Trees/" + _treeNames[i]) as GameObject;

                        Event.current.Use();
                    }
                }
            }

            if (!_checkOnce)
            {
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
                }
                _checkOnce = true;
            }
            _myPointer.transform.localScale = new Vector3(_brushSize, 0.1f, _brushSize);
            _myPointer.GetComponent<Renderer>().material = Resources.Load("World_Building/PainterMaterial") as Material;
        }

        void AddDecals()
        {
            _brushSize = EditorGUILayout.Slider("Brush Size: ", _brushSize, 0, 10);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            for (int i = 0; i < _decalNames.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 150 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                _xPos++;

                if (i > 0)
                {
                    if ((i + 1) % 3 == 0)
                    {
                        _yPos++;
                        _xPos = 0;
                    }
                }

                _gameObjectEditor = Editor.CreateEditor(Resources.Load("Decals/" + _decalNames[i]));
                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    EditorGUILayout.HelpBox(_decalNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        _isAddingToScene = true;
                        _objectToAdd = Instantiate(Resources.Load("Decals/" + _decalNames[i])) as GameObject;
                                                
                        Event.current.Use();
                    }
                }
            }
        }
    }
}
#endif