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

        private bool _SHOWICONS = true;

        // ENEMIES NAKIJKEN

        // Settlement
        
        private UnityEngine.Object[] _loadAllLevels;
        private UnityEngine.Object[] _loadAllLoadingScreens;
        private UnityEngine.Object[] _loadAllPotions;


        private List<string> _loadLevelNames = new List<string>();
        private List<string> _loadingScreenNames = new List<string>();

        // Environment ICONS
        
        
        private UnityEngine.Object[] _loadEnvIcons;

        private List<string> _environmentIcons = new List<string>();

        private List<string> _AllPotionNames = new List<string>();

        private string[] _themeSelection;
        private string[] _themeSelectType;
        private string[] _gameplaySelectType;
        private string[] _triggerSelectType;
        private string[] _vikingTileSelectType;
        private string[] _gameplayTriggerSelectType;

        private int _loadLevelIndex;
        private int _loadScreenIndex;
        private int _triggerSelectIndex;
        private int _soundSelectIndex;
        private int _gameplayTriggerIndex;

        private int _vikingTileSelectIndex;

        private int _soundTriggerSize = 5;
        private float _soundVolume = 0.5f;
        
        private bool _addLoadLevels;
        private bool _addTriggers;
        private bool _playSoundOnce;
        private bool _addItems;
        private bool _addPotions;
        private bool _isAddingTriggers = false;


        private bool _isGroundLevel;

        private bool _isLayerSet = false;
        private int _oldLayer = 0;
        private int[] _childLayers;
    

        private Editor _gameObjectEditor;

        private Vector2 _scrollPos = Vector2.zero;

        private static bool _isAddingToScene = false;
        private GameObject _objectToAdd;

        private int _snapAmount = 5;

        private GameObject _groundFloor;
        private List<GameObject> _lowerLevels = new List<GameObject>();

        private List<GameObject> _upperLevels = new List<GameObject>();

        private bool _groundIsActive = true;
        private List<bool> _lowerLevelsIsActive = new List<bool>();
        private List<bool> _upperLevelsIsActive = new List<bool>();

        private FloorObject[] _getFloors;
        private List<string> _allFloors = new List<string>();
        private int _floorObjectIndex = 0;
        private List<GameObject> _activeFloors = new List<GameObject>();

        private int _previewWindow = 128;
        private int _previewOffset = 175;
        private int _previewTilesOffset = 200;

        private List<int> _itemID = new List<int>();
        private List<string> _itemName = new List<string>();
        private List<string> _itemDesc = new List<string>();
        private List<ItemType> _itemType = new List<ItemType>();
        private List<int> _itemStats = new List<int>();
        private List<string> _itemObject = new List<string>();

        private GUISkin _skin;

        private int _tabIndex = -1;
        private int _worldTabIndex = -1;
        private int _worldTypeTabIndex = -1;
        private int _gameplayTabIndex = -1;
        private int _vikingTileTabIndex = -1;
        private int _triggerTabIndex = -1;
        private int _itemTabIndex = -1;

        private int _numberOfRows = 5;

        static void ShowWindow()
        {
            ObjectPainter _objectPainter = EditorWindow.GetWindow<ObjectPainter>(true, "Level Editor");
        }

        void OnEnable()
        {

            ClearItems();
            _getFloors = GameObject.FindObjectsOfType<FloorObject>();

            for (int i = 0; i < _getFloors.Length; i++)
            {

                _allFloors.Add(_getFloors[i].name);

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


            _loadAllLevels = Resources.LoadAll("Scenes/");
            _loadAllPotions = Resources.LoadAll("Items/Potions/");
            _loadEnvIcons = Resources.LoadAll("World_Building/ICONS/Rocks");
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
            #region STATIC PROPS

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

            _themeSelection = new string[] { "Settlement", "Viking", "Graveyard", "Dungeon" };
            _themeSelectType = new string[] { "Buildings", "Tiles", "Perimeter", "Props", "Borders" };
            _gameplaySelectType = new string[] { "Switch Scene Trigger", "Triggers", "Events" };
            _triggerSelectType = new string[] { "Audio Trigger", "Animation Trigger", "GamePlay" };
            _vikingTileSelectType = new string[] {"Surfaces", "Edges" };
            _gameplayTriggerSelectType = new string[] { "Instant Death", "Level Up" };


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
            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            _tabIndex = GUILayout.Toolbar(_tabIndex, new string[] { "Add World Objects", "Add Gameplay Trigger", "Add Items", "Add Environmental Props" });

            switch (_tabIndex)
            {
                case 0:
                    GUI.backgroundColor = new Color(0.5f, 0.7f, 0.2f, 1f);
                    AddBuildings();
                    break;
                case 1:
                    GUI.backgroundColor = new Color(0.2f, 0.7f, 0.5f, 1);
                    AddGameplay();
                    break;
                case 2:
                    GUI.backgroundColor = new Color(0.7f, 0.5f, 0.4f, 1);
                    AddItems();
                    break;
                case 3:
                    GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                    AddStaticProps();
                    break;
                default:
                    break;
            }
           
            //GUILayout.EndArea();
            Rect view = GUILayoutUtility.GetRect(750, 1500);
            //GUILayout.EndScrollView();
            EditorGUILayout.EndScrollView();
            
            
        }

        // OnSceneGUI gets activated in the editor
        void OnSceneGUI(SceneView _sceneView)
        {
            Handles.BeginGUI();

            Vector3 _newPos;
            int counter = 0;
            

            if(Theme.Settlement.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Settlement.ReturnObjectToAdd();
            }

            if (Theme.Viking.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Viking.ReturnObjectToAdd();
            }
            if (Theme.Graveyard.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Graveyard.ReturnObjectToAdd();
            }
            if (Theme.Dungeon.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Dungeon.ReturnObjectToAdd();
            }

            if (Theme.Swamp.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Swamp.ReturnObjectToAdd();
            }

            #region ADDING TO SCENE
            if (_isAddingToScene && _objectToAdd != null)
            {
                
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;


                // Set the focus on the editor
                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                sceneView.Focus();


                if (!_isLayerSet)
                {
                    // Create an int array to store all the old layers with the length of the amount of children in an object
                    _childLayers = new int[_objectToAdd.transform.childCount];

                    // Get the original layer index and store it in the _childLayers array

                    // make sure there are children
                    if (_childLayers.Length > 0)
                    {
                        foreach (Transform child in _objectToAdd.transform)
                        {
                            _childLayers[counter] = child.gameObject.layer;
                            counter++;
                        }
                    }

                    _oldLayer = _objectToAdd.layer;

                    // reset the counter
                    counter = 0;
                    _isLayerSet = true;

                }
                
                if (Physics.Raycast(_ray, out _hit))
                {

                    // Set the object layer to 2 ( IGNORE RAYCAST ) so we can raycast on the new object
                    foreach (Transform child in _objectToAdd.transform)
                    {
                        child.gameObject.layer = 2;
                    }
                    
                    //_objectToAdd.layer = 2;


                    // Snapping
                    _newPos = new Vector3((int)Mathf.Round(_hit.point.x / _snapAmount) * _snapAmount, (int)Mathf.Round(_hit.point.y / 1) * 1, (int)Mathf.Round(_hit.point.z / _snapAmount) * _snapAmount);
                    _objectToAdd.transform.position = _newPos;
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
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

                        switch (_worldTabIndex)
                        {
                            case 0:
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

                                switch (_worldTypeTabIndex)
                                {
                                    case 0:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Buildings").transform);
                                        break;
                                    case 1:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Tiles").transform);
                                        break;
                                    case 2:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Perimeter").transform);
                                        break;
                                    case 3:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Props").transform);
                                        break;
                                    default:
                                        break;
                                }
                               // Theme.Settlement.DeleteLoadedObject();

                                break;
                            case 1:

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

                                switch (_worldTypeTabIndex)
                                {
                                    case 0:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Viking_Buildings").transform);
                                        break;
                                    case 1:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Viking_Tiles").transform);
                                        break;
                                    case 2:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Viking_Perimeter").transform);
                                        break;
                                    case 3:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Viking_Props").transform);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2:
                                if (GameObject.Find("Graveyard") == null)
                                {
                                    GameObject _graveyard = new GameObject();
                                    _graveyard.name = "Graveyard";
                                    _graveyard.transform.SetParent(GameObject.Find("WORLD").transform);
                                    _objectToAdd.transform.SetParent(GameObject.Find("Graveyard").transform);
                                }
                                break;
                            case 3:

                                if (GameObject.Find("Dungeon") == null)
                                {
                                    GameObject _dungeonParent = new GameObject();
                                    _dungeonParent.name = "Dungeon";
                                    _dungeonParent.transform.SetParent(GameObject.Find("WORLD").transform);
                                }

                                if (GameObject.Find("Dungeon_Buildings") == null)
                                {
                                    GameObject _buildingParent = new GameObject();
                                    _buildingParent.name = "Dungeon_Buildings";
                                    _buildingParent.transform.SetParent(GameObject.Find("Dungeon").transform);
                                }

                                if (GameObject.Find("Dungeon_Perimeter") == null)
                                {
                                    GameObject _perimeterParent = new GameObject();
                                    _perimeterParent.name = "Dungeon_Perimeter";
                                    _perimeterParent.transform.SetParent(GameObject.Find("Dungeon").transform);
                                }

                                if (GameObject.Find("Dungeon_Props") == null)
                                {
                                    GameObject _props = new GameObject();
                                    _props.name = "Dungeon_Props";
                                    _props.transform.SetParent(GameObject.Find("Dungeon").transform);
                                }

                                if (GameObject.Find("Dungeon_Tiles") == null)
                                {
                                    GameObject _tiles = new GameObject();
                                    _tiles.name = "Dungeon_Tiles";
                                    _tiles.transform.SetParent(GameObject.Find("Dungeon").transform);
                                }

                                switch (_worldTypeTabIndex)
                                {
                                    case 0:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Buildings").transform);
                                        break;
                                    case 1:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Tiles").transform);
                                        break;
                                    case 2:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Perimeter").transform);
                                        break;
                                    case 3:
                                        _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Props").transform);
                                        break;
                                    default:
                                        break;
                                }

                                break;

                            default:
                                break;
                        }
                        #endregion
                        
                            // Set the Layer to 0 ( standard ) if it is not a gameplay trigger
                            if (!_isAddingTriggers)
                            {
                                if (_childLayers.Length > 0)
                                {
                                    foreach (Transform child in _objectToAdd.transform)
                                    {
                                        child.gameObject.layer = _childLayers[counter];
                                        counter++;
                                    }
                                }

                                _objectToAdd.layer = _oldLayer;

                            }
                            if (_isAddingTriggers)
                            {
                                _objectToAdd.layer = 2;
                            }

                            if (_objectToAdd.GetComponent<ProceduralBuilding>() != null)
                            {
                                _objectToAdd.GetComponent<ProceduralBuilding>().CreateBuilding();
                            }
                            counter = 0;
                        
                        _isAddingToScene = false;
                        
                    }
                    if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
                    {
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        _isAddingToScene = false;
                    }
                }


                if (Event.current.keyCode == (KeyCode.A) && Event.current.type == EventType.KeyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y + 45, _objectToAdd.transform.eulerAngles.z);
                }
                if (Event.current.keyCode == (KeyCode.D) && Event.current.type == EventType.KeyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y - 45, _objectToAdd.transform.eulerAngles.z);
                }
            }
            #endregion

            if(!_isAddingToScene)
            {
               // Theme.Settlement.DeleteLoadedObject();
            }

            Handles.EndGUI();
            
        }

        void AddBuildings()
        {
            
            _worldTabIndex = GUILayout.Toolbar(_worldTabIndex, new string[] { "Settlement", "Viking", "Graveyard", "Dungeon", "Swamp" });
            if (_worldTabIndex != -1)
            {
                switch (_worldTabIndex)
                {
                    case 0:
                        GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                        ShowSettlementTheme();
                        break;
                    case 1:
                        GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                        ShowVikingTheme();
                        break;
                    case 2:
                        GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                        ShowGraveyardTheme();
                        break;
                    case 3:
                        GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                        ShowDungeonTheme();
                        break;
                    case 4:
                        GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                        ShowSwampTheme();
                        break;
                    default:
                        break;
                }
            }
        }

        void ShowSettlementTheme()
        {
            _worldTypeTabIndex = GUILayout.Toolbar(_worldTypeTabIndex, new string[] { "Buildings", "Tiles", "Perimeter", "Props" });

            if (_worldTypeTabIndex != -1)
            {

                switch (_worldTypeTabIndex)
                {
                    case 0:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Settlement.ReturnHasLoadedObjects())
                        {
                            Theme.Settlement.LoadAll();
                        }
                        Theme.Settlement.ShowAddBuildings(5);
                        break;
                    case 1:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Settlement.ReturnHasLoadedObjects())
                        {
                            Theme.Settlement.LoadAll();
                        }
                        CheckFloors();
                        Theme.Settlement.ShowAddTiles(5);
                        break;
                    case 2:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Settlement.ReturnHasLoadedObjects())
                        {
                            Theme.Settlement.LoadAll();
                        }
                        Theme.Settlement.ShowAddPerimeter(5);
                        break;
                    case 3:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Settlement.ReturnHasLoadedObjects())
                        {
                            Theme.Settlement.LoadAll();
                        }
                        Theme.Settlement.ShowAddProps(5);
                        break;
                    default:
                        break;
                }
            }
        }

        void ShowVikingTheme()
        {
            _worldTypeTabIndex = GUILayout.Toolbar(_worldTypeTabIndex, new string[] { "Buildings", "Tiles", "Perimeter", "Props" });

            if (_worldTypeTabIndex != -1)
            {
                switch (_worldTypeTabIndex)
                {
                    case 0:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Viking.ReturnHasLoadedObjects())
                        {
                            Theme.Viking.LoadAll();
                        }
                        Theme.Viking.ShowAddBuildings(5);
                        break;
                    case 1:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Viking.ReturnHasLoadedObjects())
                        {
                            Theme.Viking.LoadAll();
                        }
                        CheckFloors();
                        //Viking.ShowAddTiles(5);
                        _vikingTileTabIndex = GUILayout.Toolbar(_vikingTileTabIndex, new string[] { "Surfaces", "Edges" });
                        switch (_vikingTileTabIndex)
                        {
                            case 0:
                                Theme.Viking.ShowAddSurfaces(5);
                                break;
                            case 1:
                                Theme.Viking.ShowAddEdges(5);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Viking.ReturnHasLoadedObjects())
                        {
                            Theme.Viking.LoadAll();
                        }
                        Theme.Viking.ShowAddPerimeter(5);
                        break;
                    case 3:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        if (!Theme.Viking.ReturnHasLoadedObjects())
                        {
                            Theme.Viking.LoadAll();
                        }
                        Theme.Viking.ShowAddProps(5);
                        break;
                    default:
                        break;
                }
            }
        }

        void ShowGraveyardTheme()
        {
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
            if (!Theme.Graveyard.ReturnHasLoadedObjects())
            {
                Theme.Graveyard.LoadAll();
            }
            Theme.Graveyard.ShowGraveyard(5);
        }

        void ShowDungeonTheme()
        {
            _worldTypeTabIndex = GUILayout.Toolbar(_worldTypeTabIndex, new string[] { "Buildings", "Tiles", "Perimeter", "Props", "Borders" });
            switch (_worldTypeTabIndex)
            {
                case 0:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Dungeon.ReturnHasLoadedObjects())
                    {
                        Theme.Dungeon.LoadAll();
                    }
                    Theme.Dungeon.ShowAddBuildings(5);
                    break;
                case 1:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    CheckFloors();
                    if (!Theme.Dungeon.ReturnHasLoadedObjects())
                    {
                        Theme.Dungeon.LoadAll();
                    }
                    Theme.Dungeon.ShowAddTiles(5);
                    break;
                case 2:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Dungeon.ReturnHasLoadedObjects())
                    {
                        Theme.Dungeon.LoadAll();
                    }
                    Theme.Dungeon.ShowAddPerimeter(5);
                    break;
                case 3:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Dungeon.ReturnHasLoadedObjects())
                    {
                        Theme.Dungeon.LoadAll();
                    }
                    Theme.Dungeon.ShowAddProps(5);
                    break;
                case 4:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Dungeon.ReturnHasLoadedObjects())
                    {
                        Theme.Dungeon.LoadAll();
                    }
                    Theme.Dungeon.ShowAddBorders(5);
                    break;
                default:
                    break;
            }
        }

        void ShowSwampTheme()
        {
            _worldTypeTabIndex = GUILayout.Toolbar(_worldTypeTabIndex, new string[] { "Buildings", "Tiles", "Props", "Trees" });
            switch (_worldTypeTabIndex)
            {
                case 0:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Swamp.ReturnHasLoadedObjects())
                    {
                        Theme.Swamp.LoadAll();
                    }
                    Theme.Swamp.ShowAddBuildings(5);
                    break;
                case 1:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    CheckFloors();
                    if (!Theme.Swamp.ReturnHasLoadedObjects())
                    {
                        Theme.Swamp.LoadAll();
                    }
                    Theme.Swamp.ShowAddTiles(5);
                    break;
                case 2:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Swamp.ReturnHasLoadedObjects())
                    {
                        Theme.Swamp.LoadAll();
                    }
                    Theme.Swamp.ShowAddProps(5);
                    break;
                case 3:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    if (!Theme.Swamp.ReturnHasLoadedObjects())
                    {
                        Theme.Swamp.LoadAll();
                    }
                    Theme.Swamp.ShowAddTrees(5);
                    break;
                default:
                    break;
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
        }

        void AddGameplay()
        {
  
            if (!_addLoadLevels && !_addTriggers)
            {
                _gameplayTabIndex = GUILayout.Toolbar(_gameplayTabIndex, new string[] { "Switch Scene Trigger", "Triggers", "Events" });
                switch (_gameplayTabIndex)
                {
                    case 0:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        AddLoadLevel();
                        break;
                    case 1:
                        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                        AddTriggers();
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }

            }
        }

        void AddTriggers()
        {
            _triggerTabIndex = GUILayout.Toolbar(_triggerTabIndex, new string[] { "Audio Trigger", "Animation Trigger", "Gameplay" });
            switch (_triggerTabIndex)
            {
                case 0:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    AudioTrigger();
                    break;
                case 1:
                    break;
                case 2:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    GameplayTrigger();
                    break;
                default:
                    break;
            }
        }

        void AddStaticProps()
        {
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
                        _isAddingToScene = true;
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _environmentIcons[i])) as GameObject;
                        _objectToAdd.transform.SetParent(GameObject.Find("STATICPROPS").transform);

                        Event.current.Use();
                    }
                }
            }
        }

        void AddItems()
        {
            _itemTabIndex = GUILayout.Toolbar(_itemTabIndex, new string[] { "Add Potions" });
            switch (_itemTabIndex)
            {
                case 0:
                    AddPotions();
                    break;
                default:
                    break;
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


                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    _snapAmount = 1;

                    EditorGUILayout.HelpBox(_AllPotionNames[i].ToString(), MessageType.Info);
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _isAddingToScene = true;
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

                        Event.current.Use();
                    }
                }
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

        void CheckFloors()
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
                    _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                    _plane.GetComponent<MeshRenderer>().enabled = true;
                    _plane.AddComponent<BoxCollider>();

                    _plane.transform.localScale = new Vector3(1000, 0, 1000);
                    _plane.name = "GroundLevel";
                    _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                    _plane.AddComponent<FloorObject>();
                    _plane.GetComponent<FloorObject>().SetLocation(-1);
                    _plane.GetComponent<FloorObject>().SetObjectActive(true);

                    _groundFloor = _plane;
                    _activeFloors.Add(_groundFloor);
                    _allFloors.Add(_groundFloor.name);

                }
            }
            else {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Level Beneath"))
                {
                    GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                    _plane.GetComponent<MeshRenderer>().enabled = true;
                    _plane.AddComponent<BoxCollider>();

                    _plane.transform.localScale = new Vector3(1000, 0, 1000);

                    _plane.transform.position = new Vector3(0, ((_lowerLevels.Count * 5) + 5) * -1, 0);

                    _plane.name = "LowerLevel_" + _lowerLevels.Count;
                    _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                    _plane.AddComponent<FloorObject>();
                    _plane.GetComponent<FloorObject>().SetObjectActive(true);
                    _plane.GetComponent<FloorObject>().SetLocation(0);

                    _lowerLevels.Add(_plane);
                    _lowerLevelsIsActive.Add(true);
                    _activeFloors.Add(_plane);
                    _allFloors.Add(_plane.name);



                }

                if (GUILayout.Button("Add Level Above"))
                {
                    GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                    _plane.GetComponent<MeshRenderer>().enabled = true;
                    _plane.AddComponent<BoxCollider>();

                    _plane.transform.localScale = new Vector3(1000, 0, 1000);

                    _plane.transform.position = new Vector3(0, ((_upperLevels.Count * 5) + 5), 0);

                    _plane.name = "UpperLevels_" + _upperLevels.Count;
                    _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                    _plane.AddComponent<FloorObject>();
                    _plane.GetComponent<FloorObject>().SetObjectActive(true);
                    _plane.GetComponent<FloorObject>().SetLocation(1);

                    _upperLevels.Add(_plane);
                    _upperLevelsIsActive.Add(true);
                    _activeFloors.Add(_plane);
                    _allFloors.Add(_plane.name);

                }
                EditorGUILayout.EndHorizontal();

                GameObject _current = GameObject.Find(_allFloors[_floorObjectIndex]);

                GUILayout.Label("Which Floor is Active");
                _floorObjectIndex = EditorGUILayout.Popup(_floorObjectIndex, _allFloors.ToArray());

                if(GameObject.Find(_allFloors[_floorObjectIndex]) != _current)
                {
                    _current.GetComponent<FloorObject>().SetObjectActive(false);
                    _current = GameObject.Find(_allFloors[_floorObjectIndex]);
                    _current.GetComponent<FloorObject>().SetObjectActive(true);

                } 

                /*
                _groundIsActive = EditorGUILayout.Toggle(_groundFloor.name + " - Active: ", _groundIsActive);
                //_groundIsActive = true;

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
                */

            }
        }

        #region TRIGGERS

        void AudioTrigger()
        {
            
            CombatSystem.SoundManager.GetAllFoliage();

            List<string> _sounds = CombatSystem.SoundManager.ReturnAllFoliage();

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

        void GameplayTrigger()
        {
            _gameplayTriggerIndex = EditorGUILayout.Popup(_gameplayTriggerIndex, _gameplayTriggerSelectType);

            if (_gameplayTriggerSelectType[_gameplayTriggerIndex] == "Instant Death")
            {
                // add cube that kills the player
            }
            if (_gameplayTriggerSelectType[_gameplayTriggerIndex] == "Level Up")
            {

            }
        }

        #endregion

        public static void SetAddingToScene()
        {
            _isAddingToScene = true;
        }


    }

}
#endif