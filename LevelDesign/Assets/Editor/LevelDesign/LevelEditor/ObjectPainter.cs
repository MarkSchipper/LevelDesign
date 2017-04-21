using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Mono.Data.Sqlite;
using System.Data;


#if UNITY_EDITOR

namespace LevelEditor
{

    public class ObjectPainter : EditorWindow
    {

        // Settlement
        private UnityEngine.Object[] _loadSettlementBuildings;
        private UnityEngine.Object[] _loadSettlementTiles;
        private UnityEngine.Object[] _loadSettlementPerimeter;
        private UnityEngine.Object[] _loadSettlementProps;
        private UnityEngine.Object[] _loadAllLevels;
        private UnityEngine.Object[] _loadAllLoadingScreens;
        

        private UnityEngine.Object[] _loadAllPotions;

        private List<string> _buildingNames = new List<string>();
        private List<string> _settlementTileNames = new List<string>();
        private List<string> _settlementPerimeterNames = new List<string>();
        private List<string> _settlementPropsNames = new List<string>();

        private List<string> _loadLevelNames = new List<string>();
        private List<string> _loadingScreenNames = new List<string>();

        // Viking

        private UnityEngine.Object[] _loadVikingBuildings;
        private UnityEngine.Object[] _loadVikingTiles;
        private UnityEngine.Object[] _loadVikingPerimeter;
        private UnityEngine.Object[] _loadVikingProps;
        private UnityEngine.Object[] _loadVikingSurfaces;
        private UnityEngine.Object[] _loadVikingEdges;
        private UnityEngine.Object[] _loadStaticProps;

        private List<string> _vikingBuildingNames = new List<string>();
        private List<string> _vikingTilesNames = new List<string>();
        private List<string> _vikingSurfaceNames = new List<string>();
        private List<string> _vikingEdgeNames = new List<string>();
        private List<string> _vikingPerimeterNames = new List<string>();
        private List<string> _vikingPropsNames = new List<string>();
        private List<string> _staticPropNames = new List<string>();

        // Other
        private UnityEngine.Object[] _loadGraveyard;

        private List<string> _graveyardNames = new List<string>();

        private List<string> _AllPotionNames = new List<string>();

        private string[] _themeSelection;
        private string[] _themeSelectType;
        private string[] _gameplaySelectType;
        private string[] _triggerSelectType;
        private string[] _vikingTileSelectType;

        private int _themeSelectionIndex;
        private int _themeSelectTypeIndex;
        private int _gameplaySelectIndex;
        private int _loadLevelIndex;
        private int _loadScreenIndex;
        private int _triggerSelectIndex;
        private int _soundSelectIndex;

        private int _vikingTileSelectIndex;

        private int _soundTriggerSize = 5;
        private float _soundVolume = 0.5f;

        private bool _addBuildings;
        private bool _addGameplay;
        private bool _addLoadLevels;
        private bool _addTriggers;
        private bool _playSoundOnce;
        private bool _addItems;
        private bool _addPotions;
        private bool _isAddingTriggers = false;
        private bool _addStaticProps;
        
        private bool _isGroundLevel;

        private Editor _gameObjectEditor;

        private Vector2 _scrollPos;

        private bool _isAddingToScene = false;
        private GameObject _objectToAdd;

        private int _snapAmount = 5;

        private GameObject _groundFloor;
        private List<GameObject> _lowerLevels = new List<GameObject>();

        private List<GameObject> _upperLevels = new List<GameObject>();

        private bool _groundIsActive;
        private List<bool> _lowerLevelsIsActive = new List<bool>();
        private List<bool> _upperLevelsIsActive = new List<bool>();

        private FloorObject[] _getFloors;

        private int _previewWindow = 128;

        private List<int> _itemID = new List<int>();
        private List<string> _itemName = new List<string>();
        private List<string> _itemDesc = new List<string>();
        private List<ItemType> _itemType = new List<ItemType>();
        private List<int> _itemStats = new List<int>();
        private List<string> _itemObject = new List<string>();

        private GUISkin _skin;

        [MenuItem("Level Design/World Builder/Level Editor")]
        static void ShowWindow()
        {
            ObjectPainter _objectPainter = EditorWindow.GetWindow<ObjectPainter>();
        }

        void OnEnable()
        {

            ClearItems();
            _getFloors = GameObject.FindObjectsOfType<FloorObject>();

            for (int i = 0; i < _getFloors.Length; i++)
            {
                if (_getFloors[i].ReturnLocation() == 0)
                {
                    _lowerLevels.Add(_getFloors[i].gameObject);
                    _lowerLevelsIsActive.Add(_getFloors[i].ReturnObjectActive());

                }
                if (_getFloors[i].ReturnLocation() == 1)
                {
                    _upperLevels.Add(_getFloors[i].gameObject);
                    _upperLevelsIsActive.Add(_getFloors[i].ReturnObjectActive());
                }
                if (_getFloors[i].ReturnLocation() == -1)
                {
                    _groundFloor = _getFloors[i].gameObject;
                    _groundIsActive = _getFloors[i].ReturnObjectActive();
                }


            }


            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            _loadSettlementBuildings = Resources.LoadAll("World_Building/Settlement/Buildings/");
            _loadSettlementTiles = Resources.LoadAll("World_Building/Settlement/Tiles/");
            _loadSettlementPerimeter = Resources.LoadAll("World_Building/Settlement/Perimeter");
            _loadSettlementProps = Resources.LoadAll("World_Building/Settlement/Props");
            _loadAllLevels = Resources.LoadAll("Scenes/");
            _loadAllPotions = Resources.LoadAll("Items/Potions/");

            _loadVikingBuildings = Resources.LoadAll("World_Building/Viking/Buildings");
            _loadVikingSurfaces = Resources.LoadAll("World_Building/Viking/Tiles/Surfaces");
            _loadVikingEdges = Resources.LoadAll("World_Building/Viking/Tiles/Edges");
            _loadVikingPerimeter = Resources.LoadAll("World_Building/Viking/Perimeter");
            _loadVikingProps = Resources.LoadAll("World_Building/Viking/Props");
            _loadStaticProps = Resources.LoadAll("World_Building/Rocks");

            _loadGraveyard = Resources.LoadAll("World_Building/Graveyard");
            _loadAllLoadingScreens = Resources.LoadAll("Scenes/LoadingScreens");

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;


            #region LEVEL SWITCHING
            for (int i = 0; i < _loadAllLoadingScreens.Length; i++)
            {
                if (_loadAllLoadingScreens[i].GetType().ToString() == "UnityEngine.Sprite")
                {

                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.SceneAsset )
                    // Add it to a list
                    _loadingScreenNames.Add(_loadAllLoadingScreens[i].ToString().Remove(_loadAllLoadingScreens[i].ToString().Length - 20));
                }
            }

            for (int i = 0; i < _loadAllLevels.Length; i++)
            {

                if (_loadAllLevels[i].GetType().ToString() == "UnityEditor.SceneAsset")
                {

                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.SceneAsset )
                    // Add it to a list
                    _loadLevelNames.Add(_loadAllLevels[i].ToString().Remove(_loadAllLevels[i].ToString().Length - 25));


                }
            }
            #endregion
            #region SETTLEMENT

            for (int i = 0; i < _loadSettlementBuildings.Length; i++)
            {
                if (_loadSettlementBuildings[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _buildingNames.Add(_loadSettlementBuildings[i].ToString().Remove(_loadSettlementBuildings[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadSettlementTiles.Length; i++)
            {
                if (_loadSettlementTiles[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementTileNames.Add(_loadSettlementTiles[i].ToString().Remove(_loadSettlementTiles[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadSettlementPerimeter.Length; i++)
            {
                if (_loadSettlementPerimeter[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementPerimeterNames.Add(_loadSettlementPerimeter[i].ToString().Remove(_loadSettlementPerimeter[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadSettlementProps.Length; i++)
            {
                if (_loadSettlementProps[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _settlementPropsNames.Add(_loadSettlementProps[i].ToString().Remove(_loadSettlementProps[i].ToString().Length - 25));

                }
            }
            #endregion

            #region VIKING

            for (int i = 0; i < _loadVikingBuildings.Length; i++)
            {
                if (_loadVikingBuildings[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingBuildingNames.Add(_loadVikingBuildings[i].ToString().Remove(_loadVikingBuildings[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadVikingSurfaces.Length; i++)
            {
                if (_loadVikingSurfaces[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingSurfaceNames.Add(_loadVikingSurfaces[i].ToString().Remove(_loadVikingSurfaces[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadVikingEdges.Length; i++)
            {
                if (_loadVikingEdges[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingEdgeNames.Add(_loadVikingEdges[i].ToString().Remove(_loadVikingEdges[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadVikingPerimeter.Length; i++)
            {
                if (_loadVikingPerimeter[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingPerimeterNames.Add(_loadVikingPerimeter[i].ToString().Remove(_loadVikingPerimeter[i].ToString().Length - 25));

                }
            }

            for (int i = 0; i < _loadVikingProps.Length; i++)
            {
                if (_loadVikingProps[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _vikingPropsNames.Add(_loadVikingProps[i].ToString().Remove(_loadVikingProps[i].ToString().Length - 25));

                }
            }

            #endregion

            #region STATIC PROPS

            for (int i = 0; i < _loadGraveyard.Length; i++)
            {
                if (_loadGraveyard[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _graveyardNames.Add(_loadGraveyard[i].ToString().Remove(_loadGraveyard[i].ToString().Length - 25));

                }
            }


            for (int i = 0; i < _loadStaticProps.Length; i++)
            {
                if (_loadStaticProps[i].GetType().ToString() == "UnityEngine.GameObject")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _staticPropNames.Add(_loadStaticProps[i].ToString().Remove(_loadStaticProps[i].ToString().Length - 25));
                }
            }
            #endregion

            #region POTIONS
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
            #endregion

            _themeSelection = new string[] { "", "Settlement", "Viking", "Graveyard" };
            _themeSelectType = new string[] { "", "Buildings", "Tiles", "Perimeter", "Props" };
            _gameplaySelectType = new string[] { "", "Scene Management", "Triggers", "Events" };
            _triggerSelectType = new string[] { "", "Audio Trigger", "Animation Trigger" };
            _vikingTileSelectType = new string[] { "", "Surfaces", "Edges" };
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            _upperLevels.Clear();
            _upperLevelsIsActive.Clear();
            _lowerLevels.Clear();
            _lowerLevelsIsActive.Clear();
            _loadLevelNames.Clear();
            _loadingScreenNames.Clear();

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {

            GUI.skin = _skin;

            if (!_addBuildings && !_addGameplay && !_addLoadLevels && !_addTriggers && !_addItems && !_addPotions && !_addStaticProps)
            {
                if (GUILayout.Button("Add Static Objects"))
                {
                    _addBuildings = true;
                }

                if (GUILayout.Button("Add GamePlay Objects"))
                {
                    _addGameplay = true;
                }
                if (GUILayout.Button("Add Items"))
                {
                    _addItems = true;
                }
                if(GUILayout.Button("Add Static Props"))
                {
                    _addStaticProps = true;
                }
            }

            if (_addBuildings)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                AddBuildings();
                EditorGUILayout.EndScrollView();
            }
            if (_addGameplay)
            {
                AddGameplay();
            }
            if (_addLoadLevels)
            {
                AddLoadLevel();
            }
            if (_addTriggers)
            {
                AddTriggers();
            }
            if (_addItems)
            {
                if (!_addPotions)
                {
                    AddItems();
                    GetAllItems();
                }
            }
            if(_addPotions)
            {
                AddPotions();
            }

            if(_addStaticProps)
            {
                AddStaticProps();
            }
        }

        // OnSceneGUI gets activated in the editor
        void OnSceneGUI(SceneView _sceneView)
        {
            Handles.BeginGUI();

            Vector3 _newPos;

            if (_isAddingToScene)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit))
                {
                    // Set the object layer to 2 ( IGNORE RAYCAST ) so we can raycast on the new object
                    _objectToAdd.layer = 2;

                    // Snapping
                    _newPos = new Vector3((int)Mathf.Round(_hit.point.x / _snapAmount) * _snapAmount, (int)Mathf.Round(_hit.point.y / 1) * 1, (int)Mathf.Round(_hit.point.z / _snapAmount) * _snapAmount);
                    _objectToAdd.transform.position = _newPos;

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

                        if(GameObject.Find("PROPS") == null)
                        {
                            GameObject _props = new GameObject();
                            _props.name = "PROPS";
                            _props.transform.SetParent(GameObject.Find("WORLD").transform);
                        }

                        if(GameObject.Find("POTIONS") == null)
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


                        if (_themeSelection[_themeSelectionIndex] == "Settlement")
                        {

                            if (GameObject.Find("Settlement") == null)
                            {
                                GameObject _settlementParent = new GameObject();
                                _settlementParent.name = "Settlement";
                                _settlementParent.transform.SetParent(GameObject.Find("WORLD").transform);
                            }

                            if (GameObject.Find("Settlement_Buildings") == null)
                            {
                                GameObject _buildingParent = new GameObject();
                                _buildingParent.name = "Settlement_Buildings";
                                _buildingParent.transform.SetParent(GameObject.Find("Settlement").transform);
                            }

                            if (GameObject.Find("Settlement_Perimeter") == null)
                            {
                                GameObject _perimeterParent = new GameObject();
                                _perimeterParent.name = "Settlement_Perimeter";
                                _perimeterParent.transform.SetParent(GameObject.Find("Settlement").transform);
                            }

                            if (GameObject.Find("Settlement_Props") == null)
                            {
                                GameObject _props = new GameObject();
                                _props.name = "Settlement_Props";
                                _props.transform.SetParent(GameObject.Find("Settlement").transform);
                            }

                            if (GameObject.Find("Settlement_Tiles") == null)
                            {
                                GameObject _settlementTiles = new GameObject();
                                _settlementTiles.name = "Settlement_Tiles";
                                _settlementTiles.transform.SetParent(GameObject.Find("Settlement").transform);
                            }

                        }

                        if (_themeSelection[_themeSelectionIndex] == "Viking")
                        {

                            if (GameObject.Find("Viking") == null)
                            {
                                GameObject _settlementParent = new GameObject();
                                _settlementParent.name = "Viking";
                                _settlementParent.transform.SetParent(GameObject.Find("WORLD").transform);
                            }

                            if (GameObject.Find("Viking_Buildings") == null)
                            {
                                GameObject _buildingParent = new GameObject();
                                _buildingParent.name = "Viking_Buildings";
                                _buildingParent.transform.SetParent(GameObject.Find("Viking").transform);
                            }

                            if (GameObject.Find("Viking_Perimeter") == null)
                            {
                                GameObject _perimeterParent = new GameObject();
                                _perimeterParent.name = "Viking_Perimeter";
                                _perimeterParent.transform.SetParent(GameObject.Find("Viking").transform);
                            }

                            if (GameObject.Find("Viking_Props") == null)
                            {
                                GameObject _props = new GameObject();
                                _props.name = "Viking_Props";
                                _props.transform.SetParent(GameObject.Find("Viking").transform);
                            }

                            if (GameObject.Find("Viking_Tiles") == null)
                            {
                                GameObject _settlementTiles = new GameObject();
                                _settlementTiles.name = "Viking_Tiles";
                                _settlementTiles.transform.SetParent(GameObject.Find("Viking").transform);
                            }

                        }

                        #endregion
                        // For scene organising we parent it to the corresponding GameObject
                        #region PARENTING
                        if (_themeSelection[_themeSelectionIndex] == "Settlement")
                        {
                            if (_themeSelectType[_themeSelectTypeIndex] == "Buildings")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Buildings").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Tiles").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Perimeter").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Props")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Props").transform);
                            }
                        }

                        if (_themeSelection[_themeSelectionIndex] == "Viking")
                        {
                            if (_themeSelectType[_themeSelectTypeIndex] == "Buildings")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Buildings").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Tiles").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Perimeter").transform);
                            }
                            if (_themeSelectType[_themeSelectTypeIndex] == "Props")
                            {
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Props").transform);
                            }
                        }
                        #endregion
                        //Selection.
                        _isAddingToScene = false;
                        

                        // Set the Layer to 0 ( standard ) if it is not a gameplay trigger
                        if (!_isAddingTriggers)
                        {
                            _objectToAdd.layer = 0;
                        }
                        if (_isAddingTriggers)
                        {
                            _objectToAdd.layer = 2;
                        }

                        if (_objectToAdd.GetComponent<ProceduralBuilding>() != null)
                        {
                            _objectToAdd.GetComponent<ProceduralBuilding>().CreateBuilding();
                        }
                    }
                    if (Event.current.button == 1 && Event.current.type == EventType.mouseDown)
                    {
                        DestroyImmediate(_objectToAdd);
                        _isAddingToScene = false;
                    }
                }


                if (Event.current.keyCode == (KeyCode.A) && Event.current.type == EventType.keyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y + 45, _objectToAdd.transform.eulerAngles.z);
                }
                if (Event.current.keyCode == (KeyCode.D) && Event.current.type == EventType.keyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y - 45, _objectToAdd.transform.eulerAngles.z);
                }
            }


            Handles.EndGUI();
        }

        void AddBuildings()
        {

            GUILayout.Label("Add a building", EditorStyles.boldLabel);
            _themeSelectionIndex = EditorGUILayout.Popup(_themeSelectionIndex, _themeSelection);

            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;


            #region SETTLEMENT

            if (_themeSelection[_themeSelectionIndex] == "Settlement")
            {

                _themeSelectTypeIndex = EditorGUILayout.Popup(_themeSelectTypeIndex, _themeSelectType);

                #region SETTLEMENT BUILDINGS

                if (_themeSelectType[_themeSelectTypeIndex] == "Buildings")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _buildingNames.Count; i++)
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Settlement/Buildings/" + _buildingNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_buildingNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Buildings/" + _buildingNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion

                #region SETTLEMENT TILES
                if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                {
                    if (GameObject.Find("GroundLevel") == null)
                    {
                        if (GUILayout.Button("Add Ground Level"))
                        {

                            if (GameObject.Find("WORLD") == null)
                            {
                                GameObject _worldParent = new GameObject();
                                _worldParent.name = "WORLD";

                            }

                            if (GameObject.Find("WorldLevels") == null)
                            {
                                GameObject _levelsParent = new GameObject();
                                _levelsParent.name = "WorldLevels";
                                _levelsParent.transform.SetParent(GameObject.Find("WORLD").transform);
                            }

                            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            _plane.GetComponent<MeshRenderer>().enabled = false;
                            _plane.AddComponent<BoxCollider>();

                            _plane.transform.localScale = new Vector3(1000, 0, 1000);
                            _plane.name = "GroundLevel";
                            _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                            _plane.AddComponent<FloorObject>();
                            _plane.GetComponent<FloorObject>().SetLocation(-1);
                            _plane.GetComponent<FloorObject>().SetObjectActive(true);

                            _groundFloor = _plane;


                        }
                    }
                    else {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Level Beneath"))
                        {
                            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            _plane.GetComponent<MeshRenderer>().enabled = false;
                            _plane.AddComponent<BoxCollider>();

                            _plane.transform.localScale = new Vector3(1000, 0, 1000);

                            _plane.transform.position = new Vector3(0, ((_lowerLevels.Count * 5) + 5) * -1, 0);

                            _plane.name = "LowerLevel" + _lowerLevels.Count;
                            _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                            _plane.AddComponent<FloorObject>();
                            _plane.GetComponent<FloorObject>().SetObjectActive(true);
                            _plane.GetComponent<FloorObject>().SetLocation(0);

                            _lowerLevels.Add(_plane);
                            _lowerLevelsIsActive.Add(true);



                        }

                        if (GUILayout.Button("Add Level Above"))
                        {
                            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            _plane.GetComponent<MeshRenderer>().enabled = false;
                            _plane.AddComponent<BoxCollider>();

                            _plane.transform.localScale = new Vector3(1000, 0, 1000);

                            _plane.transform.position = new Vector3(0, ((_upperLevels.Count * 5) + 5), 0);

                            _plane.name = "UpperLevels" + _upperLevels.Count;
                            _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                            _plane.AddComponent<FloorObject>();
                            _plane.GetComponent<FloorObject>().SetObjectActive(true);
                            _plane.GetComponent<FloorObject>().SetLocation(1);

                            _upperLevels.Add(_plane);
                            _upperLevelsIsActive.Add(true);

                        }
                        EditorGUILayout.EndHorizontal();

                        _groundIsActive = EditorGUILayout.Toggle(_groundFloor.name + " - Active: ", _groundIsActive);
                        _groundIsActive = true;

                        if (!_groundIsActive)
                        {
                            _groundFloor.GetComponent<FloorObject>().SetObjectActive(false);
                        }
                        if (_groundIsActive)
                        {
                            _groundFloor.GetComponent<FloorObject>().SetObjectActive(true);
                        }

                        for (int i = 0; i < _lowerLevels.Count; i++)
                        {
                            _lowerLevelsIsActive[i] = EditorGUILayout.Toggle(_lowerLevels[i].name + " - Active: ", _lowerLevelsIsActive[i]);

                            if (!_lowerLevelsIsActive[i])
                            {
                                _lowerLevels[i].GetComponent<FloorObject>().SetObjectActive(false);
                            }
                            else
                            {
                                _lowerLevels[i].GetComponent<FloorObject>().SetObjectActive(true);
                            }
                        }

                        for (int i = 0; i < _upperLevels.Count; i++)
                        {
                            _upperLevelsIsActive[i] = EditorGUILayout.Toggle(_upperLevels[i].name + " - Active: ", _upperLevelsIsActive[i]);
                            if (!_upperLevelsIsActive[i])
                            {
                                _upperLevels[i].GetComponent<FloorObject>().SetObjectActive(false);
                            }
                            else
                            {
                                _upperLevels[i].GetComponent<FloorObject>().SetObjectActive(true);
                            }
                        }

                        _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                        for (int i = 0; i < _settlementTileNames.Count; i++)
                        {
                            _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 300 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                            _xPos++;

                            if (i > 0)
                            {
                                if (i % 3 == 0)
                                {

                                    _yPos++;
                                    _xPos = 0;
                                }
                            }

                            GUILayout.BeginHorizontal();
                            _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Settlement/Tiles/" + _settlementTileNames[i]));
                            _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                            if (_previewRect[i].Contains(Event.current.mousePosition))
                            {

                                if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                {
                                    _isAddingToScene = true;
                                    _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Tiles/" + _settlementTileNames[i])) as GameObject;

                                    Event.current.Use();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region SETTLEMENT PERIMETER
                if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _settlementPerimeterNames.Count; i++)
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Settlement/Perimeter/" + _settlementPerimeterNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_settlementPerimeterNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Perimeter/" + _settlementPerimeterNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion

                #region SETTLEMENT PROPS
                if (_themeSelectType[_themeSelectTypeIndex] == "Props")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _settlementPropsNames.Count; i++)
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Settlement/Props/" + _settlementPropsNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_settlementPropsNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Perimeter/" + _settlementPropsNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region VIKING
            if (_themeSelection[_themeSelectionIndex] == "Viking")
            {

                _themeSelectTypeIndex = EditorGUILayout.Popup(_themeSelectTypeIndex, _themeSelectType);

                #region VIKING BUILDINGS

                if (_themeSelectType[_themeSelectTypeIndex] == "Buildings")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _vikingBuildingNames.Count; i++)
                    {
                        _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 150 + (_previewWindow * _yPos + 10), _previewWindow, 100);


                        _xPos++;

                        if (i > 0)
                        {
                            if ((i + 1) % 3 == 0)
                            {

                                _yPos++;
                                _xPos = 0;
                            }
                        }


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Viking/Buildings/" + _vikingBuildingNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingBuildingNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Buildings/" + _vikingBuildingNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion

                #region VIKING TILES
                if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                {
                    if (!_isGroundLevel)
                    {
                        #region LEVELS
                        if (GameObject.Find("GroundLevel") == null)
                        {
                            if (GUILayout.Button("Add Ground Level"))
                            {

                                if (GameObject.Find("WORLD") == null)
                                {
                                    GameObject _worldParent = new GameObject();
                                    _worldParent.name = "WORLD";

                                }

                                if (GameObject.Find("WorldLevels") == null)
                                {
                                    GameObject _levelsParent = new GameObject();
                                    _levelsParent.name = "WorldLevels";
                                    _levelsParent.transform.SetParent(GameObject.Find("WORLD").transform);
                                }

                                GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                                _plane.GetComponent<MeshRenderer>().enabled = false;
                                _plane.AddComponent<BoxCollider>();

                                _plane.transform.localScale = new Vector3(1000, 0, 1000);
                                _plane.name = "GroundLevel";
                                _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                                _plane.AddComponent<FloorObject>();
                                _plane.GetComponent<FloorObject>().SetLocation(-1);
                                _plane.GetComponent<FloorObject>().SetObjectActive(true);

                                _groundFloor = _plane;

                                _isGroundLevel = true;
                            }
                        }
                        else
                        {
                            _isGroundLevel = true;
                            _groundIsActive = true;
                        }
                    }
                    if(_isGroundLevel) {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Level Beneath"))
                        {
                            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            _plane.GetComponent<MeshRenderer>().enabled = false;
                            _plane.AddComponent<BoxCollider>();

                            _plane.transform.localScale = new Vector3(1000, 0, 1000);

                            _plane.transform.position = new Vector3(0, ((_lowerLevels.Count * 5) + 5) * -1, 0);

                            _plane.name = "LowerLevel" + _lowerLevels.Count;
                            _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                            _plane.AddComponent<FloorObject>();
                            _plane.GetComponent<FloorObject>().SetObjectActive(true);
                            _plane.GetComponent<FloorObject>().SetLocation(0);

                            _lowerLevels.Add(_plane);
                            _lowerLevelsIsActive.Add(true);



                        }

                        if (GUILayout.Button("Add Level Above"))
                        {
                            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            _plane.GetComponent<MeshRenderer>().enabled = false;
                            _plane.AddComponent<BoxCollider>();

                            _plane.transform.localScale = new Vector3(1000, 0, 1000);

                            _plane.transform.position = new Vector3(0, ((_upperLevels.Count * 5) + 5), 0);

                            _plane.name = "UpperLevels" + _upperLevels.Count;
                            _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                            _plane.AddComponent<FloorObject>();
                            _plane.GetComponent<FloorObject>().SetObjectActive(true);
                            _plane.GetComponent<FloorObject>().SetLocation(1);

                            _upperLevels.Add(_plane);
                            _upperLevelsIsActive.Add(true);

                        }
                        #endregion



                        EditorGUILayout.EndHorizontal();



                        _groundIsActive = EditorGUILayout.Toggle(_groundFloor.name + " - Active: ", _groundIsActive);


                        if (!_groundIsActive)
                        {
                            _groundFloor.GetComponent<FloorObject>().SetObjectActive(false);
                        }
                        if (_groundIsActive)
                        {
                            _groundFloor.GetComponent<FloorObject>().SetObjectActive(true);
                        }

                        for (int i = 0; i < _lowerLevels.Count; i++)
                        {
                            _lowerLevelsIsActive[i] = EditorGUILayout.Toggle(_lowerLevels[i].name + " - Active: ", _lowerLevelsIsActive[i]);

                            if (!_lowerLevelsIsActive[i])
                            {
                                _lowerLevels[i].GetComponent<FloorObject>().SetObjectActive(false);
                            }
                            else
                            {
                                _lowerLevels[i].GetComponent<FloorObject>().SetObjectActive(true);
                            }
                        }

                        for (int i = 0; i < _upperLevels.Count; i++)
                        {
                            _upperLevelsIsActive[i] = EditorGUILayout.Toggle(_upperLevels[i].name + " - Active: ", _upperLevelsIsActive[i]);
                            if (!_upperLevelsIsActive[i])
                            {
                                _upperLevels[i].GetComponent<FloorObject>().SetObjectActive(false);
                            }
                            else
                            {
                                _upperLevels[i].GetComponent<FloorObject>().SetObjectActive(true);
                            }
                        }

                        _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);
                        GUILayout.Space(20);
                        _vikingTileSelectIndex = EditorGUILayout.Popup(_vikingTileSelectIndex, _vikingTileSelectType);

                        if (_vikingTileSelectType[_vikingTileSelectIndex] == "Surfaces")
                        {

                            for (int i = 0; i < _vikingSurfaceNames.Count; i++)
                            {


                                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 320 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                                _xPos++;

                                if (i > 0)
                                {
                                    if (i % 5 == 0)
                                    {

                                        _yPos++;
                                        _xPos = 0;
                                    }
                                }

                                GUILayout.BeginHorizontal();
                                _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Viking/Tiles/Surfaces/" + _vikingSurfaceNames[i]));
                                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                                if (_previewRect[i].Contains(Event.current.mousePosition))
                                {

                                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                    {
                                        _isAddingToScene = true;
                                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Surfaces/" + _vikingSurfaceNames[i])) as GameObject;

                                        Event.current.Use();
                                    }
                                }
                            }
                        }

                        if (_vikingTileSelectType[_vikingTileSelectIndex] == "Edges")
                        {

                            for (int i = 0; i < _vikingEdgeNames.Count; i++)
                            {


                                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 320 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
                                _xPos++;

                                if (i > 0)
                                {
                                    if (i % 5 == 0)
                                    {

                                        _yPos++;
                                        _xPos = 0;
                                    }
                                }

                                GUILayout.BeginHorizontal();
                                _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Viking/Tiles/Edges/" + _vikingEdgeNames[i]));
                                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                                if (_previewRect[i].Contains(Event.current.mousePosition))
                                {

                                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                    {
                                        _isAddingToScene = true;
                                        _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Edges/" + _vikingEdgeNames[i])) as GameObject;

                                        Event.current.Use();
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region VIKING PERIMETER
                if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                {


                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _vikingPerimeterNames.Count; i++)
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Viking/Perimeter/" + _vikingPerimeterNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingPerimeterNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Perimeter/" + _vikingPerimeterNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region VIKING PROPS
                if (_themeSelectType[_themeSelectTypeIndex] == "Props")
                {
                    Debug.Log(_vikingPropsNames.Count);

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

                    for (int i = 0; i < _vikingPropsNames.Count; i++)
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Viking/Props/" + _vikingPropsNames[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingPropsNames[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Props/" + _vikingPropsNames[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region GRAVEYARD
            if (_themeSelection[_themeSelectionIndex] == "Graveyard")
            {
                for (int i = 0; i < _graveyardNames.Count; i++)
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


                    _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Graveyard/" + _graveyardNames[i]));

                    _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                    //EditorGUILayout.LabelField("test", _nameRect[i]);

                    if (_previewRect[i].Contains(Event.current.mousePosition))
                    {
                        EditorGUILayout.HelpBox(_graveyardNames[i].ToString(), MessageType.Info);
                        if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                        {
                            _isAddingToScene = true;
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Graveyard/" + _graveyardNames[i])) as GameObject;

                            Event.current.Use();
                        }
                    }
                }
            }
            #endregion

            if (GUILayout.Button("BACK"))
            {
                _addBuildings = false;
            }


        }

        void AddLoadLevel()
        {
            GUILayout.Label("Add a Load Level Trigger", EditorStyles.boldLabel);
            _loadLevelIndex = EditorGUILayout.Popup("Which Level: ", _loadLevelIndex, _loadLevelNames.ToArray());
            _loadScreenIndex = EditorGUILayout.Popup("Which Loading Screen: ", _loadScreenIndex, _loadingScreenNames.ToArray());


            if (GUILayout.Button("Add Object"))
            {
                _isAddingToScene = true;
                _objectToAdd = Instantiate(Resources.Load("World_Building/GamePlay/LevelLoadTrigger")) as GameObject;
                _objectToAdd.GetComponentInChildren<LevelTrigger>().SetLevel(_loadLevelNames[_loadLevelIndex], _loadingScreenNames[_loadScreenIndex]);

            }

            if (GUILayout.Button("BACK"))
            {
                _addLoadLevels = false;
            }

        }

        void AddGameplay()
        {
            if (!_addLoadLevels && !_addTriggers)
            {
                _gameplaySelectIndex = EditorGUILayout.Popup(_gameplaySelectIndex, _gameplaySelectType);

                if (_addGameplay)
                {

                    if (GUILayout.Button("BACK"))
                    {
                        _addGameplay = false;
                    }
                }
                if (_gameplaySelectType[_gameplaySelectIndex] == "Scene Management")
                {
                    if (GUILayout.Button("Add Trigger to load scene"))
                    {
                        _addLoadLevels = true;
                    }
                }
                if (_gameplaySelectType[_gameplaySelectIndex] == "Triggers")
                {
                    _addTriggers = true;

                }
                if (_gameplaySelectType[_gameplaySelectIndex] == "Events")
                {

                }
            }
        }

        void AddTriggers()
        {
            GUILayout.Label("Add a Trigger Event", EditorStyles.boldLabel);

            _triggerSelectIndex = EditorGUILayout.Popup(_triggerSelectIndex, _triggerSelectType);

            if (_triggerSelectType[_triggerSelectIndex] == "Audio Trigger")
            {
                CombatSystem.SoundSystem.GetAllFoliage();
                List<string> _sounds = CombatSystem.SoundSystem.ReturnAllFoliage();

                GUILayout.Label("Which Sound");
                _soundSelectIndex = EditorGUILayout.Popup(_soundSelectIndex, _sounds.ToArray());

                _playSoundOnce = EditorGUILayout.Toggle("Play Once?: ", _playSoundOnce);
                _soundVolume = EditorGUILayout.FloatField("Volume: ", _soundVolume);

                _soundTriggerSize = EditorGUILayout.IntField("Size of Trigger: ", _soundTriggerSize);

                if (GUILayout.Button("Add Sound Trigger"))
                {
                    _isAddingToScene = true;
                    _objectToAdd = Instantiate(Resources.Load("World_Building/GamePlay/SoundTrigger")) as GameObject;
                    _objectToAdd.GetComponent<Transform>().localScale = new Vector3(_soundTriggerSize, _soundTriggerSize, _soundTriggerSize);
                    _objectToAdd.GetComponentInChildren<SoundTrigger>().SetData(_sounds[_soundSelectIndex], _playSoundOnce, _soundVolume);
                    _objectToAdd.name = "SoundTrigger-" + _sounds[_soundSelectIndex];

                    _isAddingTriggers = true;

                    if (GameObject.Find("AUDIO") != null)
                    {
                        _objectToAdd.transform.SetParent(GameObject.FindGameObjectWithTag("Audio").transform);
                    }
                    else
                    {
                        GameObject _obj = new GameObject();
                        _obj.name = "AUDIO";
                        _obj.tag = "Audio";

                        _objectToAdd.transform.SetParent(_obj.transform);
                    }
                }

            }

            if (_triggerSelectType[_triggerSelectIndex] == "Animation Trigger")
            {

            }

            if (GUILayout.Button("BACK"))
            {
                _addGameplay = false;
                _addTriggers = false;
            }

        }

        void AddStaticProps()
        {
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            GUILayout.Button("Add Static Props", EditorStyles.boldLabel);

            for (int i = 0; i < _staticPropNames.Count; i++)
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

                _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/Rocks/" + _staticPropNames[i]));
                Debug.Log(_gameObjectEditor);

                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    _snapAmount = 1;

                    EditorGUILayout.HelpBox(_buildingNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        _isAddingToScene = true;
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _staticPropNames[i])) as GameObject;
                        _objectToAdd.transform.SetParent(GameObject.Find("STATICPROPS").transform);
                        
                        Event.current.Use();
                    }
                }
            }

            if (GUILayout.Button("BACK"))
            {
                _addStaticProps = false;
            }
        }

        void AddItems()
        {
            if(GUILayout.Button("Add Potions"))
            {
                _addPotions = true;
            }   
            if(GUILayout.Button("BACK"))
            {
                _addItems = false;
            }        
        }

        void AddPotions()
        {
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
                    if ((i + 1) % 3 == 0)
                    {
                        _yPos++;
                        _xPos = 0;
                    }
                }
                
                _gameObjectEditor = Editor.CreateEditor(Resources.Load("Items/Potions/" + _AllPotionNames[i]));
                Debug.Log(_gameObjectEditor);
                 
                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    _snapAmount = 1;

                    EditorGUILayout.HelpBox(_buildingNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        _isAddingToScene = true;
                        _objectToAdd = Instantiate(Resources.Load("Items/Potions/" + _AllPotionNames[i])) as GameObject;
                        _objectToAdd.transform.SetParent(GameObject.Find("POTIONS").transform);
                        _objectToAdd.AddComponent<ItemCollectable>();
                        _objectToAdd.GetComponent<ItemCollectable>().SetValues(_itemID[i], _itemName[i], _itemType[i].ToString(), _itemStats[i]);

                        Event.current.Use();
                    }
                }
            }

            if(GUILayout.Button("BACK"))
            {
                _addPotions = false;
            }
        }

        void GetAllItems()
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

        void ClearItems()
        {
            _itemID.Clear();
            _itemName.Clear();
            _itemDesc.Clear();
            _itemType.Clear();
            _itemStats.Clear();
            _itemObject.Clear();
            _AllPotionNames.Clear();
    }
    }
}
#endif