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

        // Dungeon ICONS

        private UnityEngine.Object[] _loadDungeonPropsIcons;
        private UnityEngine.Object[] _loadDungeonBordersIcons;
        private UnityEngine.Object[] _loadDungeonTilesIcons;
        private UnityEngine.Object[] _loadDungeonWallsIcons;

        private List<string> _dungeonPropsIcons = new List<string>();
        private List<string> _dungeonBordersIcons = new List<string>();
        private List<string> _dungeonTilesIcons = new List<string>();
        private List<string> _dungeonWallsIcons = new List<string>();

        // Settlement ICONS

        private UnityEngine.Object[] _loadSettlementPropsIcons;
        private UnityEngine.Object[] _loadSettlementBuildingsIcons;
        private UnityEngine.Object[] _loadSettlementPerimeterIcons;
        private UnityEngine.Object[] _loadSettlementTilesIcons;

        private List<string> _settlementPropsIcons = new List<string>();
        private List<string> _settlementBuildingsIcons = new List<string>();
        private List<string> _settlementPerimeterIcons = new List<string>();
        private List<string> _settlementTilesIcons = new List<string>();

        // Viking ICONS

        private UnityEngine.Object[] _loadVikingPropsIcons;
        private UnityEngine.Object[] _loadVikingBuildingsIcons;
        private UnityEngine.Object[] _loadVikingPerimeterIcons;
        private UnityEngine.Object[] _loadVikingEdgesIcons;
        private UnityEngine.Object[] _loadVikingSurfacesIcons;

        private List<string> _vikingPropsIcons = new List<string>();
        private List<string> _vikingBuildingsIcons = new List<string>();
        private List<string> _vikingPerimeterIcons = new List<string>();
        private List<string> _vikingEdgesIcons = new List<string>();
        private List<string> _vikingSurfacesIcons = new List<string>();

        // Graveyard ICONS

        private UnityEngine.Object[] _loadGraveyardIcons;

        private List<string> _graveyardIcons = new List<string>();

        

        // dungeon

        
        // Other
        

        

        private List<string> _AllPotionNames = new List<string>();

        private string[] _themeSelection;
        private string[] _themeSelectType;
        private string[] _gameplaySelectType;
        private string[] _triggerSelectType;
        private string[] _vikingTileSelectType;
        private string[] _gameplayTriggerSelectType;

        private int _themeSelectionIndex;
        private int _themeSelectTypeIndex;
        private int _gameplaySelectIndex;
        private int _loadLevelIndex;
        private int _loadScreenIndex;
        private int _triggerSelectIndex;
        private int _soundSelectIndex;
        private int _gameplayTriggerIndex;

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

        private bool _isLayerSet = false;
        private int _oldLayer = 0;
    

        private Editor _gameObjectEditor;

        private Vector2 _scrollPos = Vector2.zero;

        private bool _isAddingToScene = false;
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

        private int _numberOfRows = 5;

        [MenuItem("Level Design/World Builder/Level Editor")]
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

            _loadDungeonPropsIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Props");
            _loadDungeonBordersIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Borders");
            _loadDungeonTilesIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Tiles");
            _loadDungeonWallsIcons = Resources.LoadAll("World_Building/ICONS/Dungeon/Walls");

            _loadSettlementBuildingsIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Buildings");
            _loadSettlementPerimeterIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Perimeter");
            _loadSettlementPropsIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Props");
            _loadSettlementTilesIcons = Resources.LoadAll("World_Building/ICONS/Settlement/Tiles");

            _loadVikingBuildingsIcons = Resources.LoadAll("World_Building/ICONS/Viking/Buildings");
            _loadVikingEdgesIcons = Resources.LoadAll("World_Building/ICONS/Viking/Tiles/Edges");
            _loadVikingPerimeterIcons = Resources.LoadAll("World_Building/ICONS/Viking/Perimeter");
            _loadVikingPropsIcons = Resources.LoadAll("World_Building/ICONS/Viking/Props");
            _loadVikingSurfacesIcons = Resources.LoadAll("World_Building/ICONS/Viking/Tiles/Surfaces");

            _loadGraveyardIcons = Resources.LoadAll("World_Building/ICONS/Graveyard");

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;


            #region DUNGEON ICONS
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
            #endregion
            #region SETTLEMENT ICONS
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
            #endregion
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
            #region GRAVEYARD ICONS
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
            #endregion
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

            _themeSelection = new string[] { "Select a Theme", "Settlement", "Viking", "Graveyard", "Dungeon" };
            _themeSelectType = new string[] { "Select the type of Object", "Buildings", "Tiles", "Perimeter", "Props", "Borders" };
            _gameplaySelectType = new string[] { "Select the type of Gameplay Trigger", "Switch Scene Trigger", "Triggers", "Events" };
            _triggerSelectType = new string[] { "Select the type of Trigger", "Audio Trigger", "Animation Trigger", "GamePlay" };
            _vikingTileSelectType = new string[] { "", "Surfaces", "Edges" };
            _gameplayTriggerSelectType = new string[] { "", "Instant Death", "Level Up" };


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


            //_scrollPos = GUILayout.BeginScrollView(_scrollPos, true, true, GUILayout.Width(400), GUILayout.Height(400));
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            GUI.skin = _skin;

            //GUILayout.BeginArea(new Rect(0, 0, 1000, 1000));

            if (!_addBuildings && !_addGameplay && !_addLoadLevels && !_addTriggers && !_addItems && !_addPotions && !_addStaticProps)
            {
                if (GUILayout.Button("Add World Objects", GUILayout.Width(750)))
                {
                    _addBuildings = true;
                }

                if (GUILayout.Button("Add Gameplay Triggers", GUILayout.Width(750)))
                {
                    _addGameplay = true;
                }
                if (GUILayout.Button("Add Items", GUILayout.Width(750)))
                {
                    _addItems = true;
                }
                if (GUILayout.Button("Add Environmental Props", GUILayout.Width(750)))
                {
                    _addStaticProps = true;
                }
            }
            
            if (_addBuildings)
            {                
                AddBuildings();
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
            if (_addPotions)
            {
                AddPotions();
            }

            if (_addStaticProps)
            {
                AddStaticProps();
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

            if (_isAddingToScene)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;

                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                sceneView.Focus();


                if (!_isLayerSet)
                {
                    _oldLayer = _objectToAdd.layer;
                    _isLayerSet = true;
                    
                }
                
                if (Physics.Raycast(_ray, out _hit))
                {
                    
                    
                    // Set the object layer to 2 ( IGNORE RAYCAST ) so we can raycast on the new object
                    foreach (Transform child in _objectToAdd.transform)
                    {
                        child.gameObject.layer = 2;
                    }

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

            GUILayout.Label("Add World Objects", EditorStyles.boldLabel);

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

                        

                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Settlement/Buildings/" + _settlementBuildingsIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_settlementBuildingsIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Buildings/" + _settlementBuildingsIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                        
                    }
                    
                }
                #endregion

                #region SETTLEMENT TILES
                if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                {

                    CheckFloors();

                        _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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

                            
                            _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Settlement/Tiles/" + _settlementTilesIcons[i]));
                            _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                            if (_previewRect[i].Contains(Event.current.mousePosition))
                            {

                                if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                {
                                    _isAddingToScene = true;
                                    _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Tiles/" + _settlementTilesIcons[i])) as GameObject;

                                    Event.current.Use();
                                }
                            }
                        }
                    
                }
                #endregion

                #region SETTLEMENT PERIMETER
                if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Settlement/Perimeter/" + _settlementPerimeterIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_settlementPerimeterIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Perimeter/" + _settlementPerimeterIcons[i])) as GameObject;

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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Settlement/Props/" + _settlementPropsIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_settlementPropsIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Settlement/Props/" + _settlementPropsIcons[i])) as GameObject;

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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Viking/Buildings/" + _vikingBuildingsIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingBuildingsIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Buildings/" + _vikingBuildingsIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region VIKING TILES
                if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                {
                    _vikingTileSelectIndex = EditorGUILayout.Popup(_vikingTileSelectIndex, _vikingTileSelectType);


                    CheckFloors();

                    if (_vikingTileSelectType[_vikingTileSelectIndex] == "Surfaces")
                    {


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

                            // GUILayout.BeginHorizontal();
                            _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Viking/Tiles/Surfaces/" + _vikingSurfacesIcons[i]));
                            _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                            if (_previewRect[i].Contains(Event.current.mousePosition))
                            {

                                if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                {
                                    _isAddingToScene = true;
                                    _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Surfaces/" + _vikingSurfacesIcons[i])) as GameObject;

                                    Event.current.Use();
                                }
                            }
                        }
                    }

                    if (_vikingTileSelectType[_vikingTileSelectIndex] == "Edges")
                    {

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

                            //                                GUILayout.BeginHorizontal();
                            _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Viking/Tiles/Edges/" + _vikingEdgesIcons[i]));
                            _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                            if (_previewRect[i].Contains(Event.current.mousePosition))
                            {

                                if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                                {
                                    _isAddingToScene = true;
                                    _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Tiles/Edges/" + _vikingEdgesIcons[i])) as GameObject;

                                    Event.current.Use();
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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Viking/Perimeter/" + _vikingPerimeterIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingPerimeterIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Perimeter/" + _vikingPerimeterIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region VIKING PROPS
                if (_themeSelectType[_themeSelectTypeIndex] == "Props" && _themeSelection[_themeSelectionIndex] == "Viking")
                {
                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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


                        _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Viking/Props/" + _vikingPropsIcons[i]));

                        _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                        //EditorGUILayout.LabelField("test", _nameRect[i]);

                        if (_previewRect[i].Contains(Event.current.mousePosition))
                        {
                            EditorGUILayout.HelpBox(_vikingPropsIcons[i].ToString(), MessageType.Info);
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Viking/Props/" + _vikingPropsIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                
            }
            #endregion
            #region DUNGEON
            if (_themeSelection[_themeSelectionIndex] == "Dungeon")
            {

                _themeSelectTypeIndex = EditorGUILayout.Popup(_themeSelectTypeIndex, _themeSelectType);


                #region TILES
                if (_themeSelectType[_themeSelectTypeIndex] == "Tiles")
                {

                    CheckFloors();

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Tiles/" + _dungeonTilesIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region PROPS
                if (_themeSelectType[_themeSelectTypeIndex] == "Props")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Props/" + _dungeonPropsIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region WALLS
                if (_themeSelectType[_themeSelectTypeIndex] == "Perimeter")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Walls/" + _dungeonWallsIcons[i])) as GameObject;

                                Event.current.Use();
                            }
                        }
                    }
                }
                #endregion
                #region BORDERS
                if (_themeSelectType[_themeSelectTypeIndex] == "Borders")
                {

                    _snapAmount = EditorGUILayout.IntSlider("Snap: ", _snapAmount, 1, 10);

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
                            if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                            {
                                _isAddingToScene = true;
                                _objectToAdd = Instantiate(Resources.Load("World_Building/Dungeon/Borders/" + _dungeonBordersIcons[i])) as GameObject;

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


                    _gameObjectEditor = Editor.CreateEditor(Resources.Load("World_Building/ICONS/Graveyard/" + _graveyardIcons[i]));

                    _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));
                    //EditorGUILayout.LabelField("test", _nameRect[i]);

                    if (_previewRect[i].Contains(Event.current.mousePosition))
                    {
                        EditorGUILayout.HelpBox(_graveyardIcons[i].ToString(), MessageType.Info);
                        if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                        {
                            _isAddingToScene = true;
                            _objectToAdd = Instantiate(Resources.Load("World_Building/Graveyard/" + _graveyardIcons[i])) as GameObject;

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
                if (_gameplaySelectType[_gameplaySelectIndex] == "Switch Scene Trigger")
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

            if (_triggerSelectType[_triggerSelectIndex] == "GamePlay")
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

            if (GUILayout.Button("BACK"))
            {                
                _addTriggers = false;
                _gameplaySelectIndex = 0;
            }

        }

        void AddStaticProps()
        {
            Rect[] _previewRect = new Rect[50];

            int _yPos = 0;
            int _xPos = 0;

            GUILayout.Button("Add Environmental Props", EditorStyles.boldLabel);
            
            for (int i = 0; i < _environmentIcons.Count; i++)
            {
                _previewRect[i] = new Rect(20 + (_previewWindow * _xPos), 150 + (_previewWindow * _yPos + 10), _previewWindow, _previewWindow);
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
                    if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
                    {
                        _isAddingToScene = true;
                        _objectToAdd = Instantiate(Resources.Load("World_Building/Rocks/" + _environmentIcons[i])) as GameObject;
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
            if (GUILayout.Button("Add Potions"))
            {
                _addPotions = true;
            }
            if (GUILayout.Button("BACK"))
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


                _gameObjectEditor.OnPreviewGUI(_previewRect[i], _skin.GetStyle("PreviewWindow"));

                if (_previewRect[i].Contains(Event.current.mousePosition))
                {
                    _snapAmount = 1;

                    EditorGUILayout.HelpBox(_AllPotionNames[i].ToString(), MessageType.Info);
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

            if (GUILayout.Button("BACK"))
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
                    _plane.GetComponent<MeshRenderer>().enabled = false;
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
                    _plane.GetComponent<MeshRenderer>().enabled = false;
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
                    _plane.GetComponent<MeshRenderer>().enabled = false;
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
                    Debug.Log("OLD FLOOR " + _current.name);
                    _current.GetComponent<FloorObject>().SetObjectActive(false);
                    _current = GameObject.Find(_allFloors[_floorObjectIndex]);
                    Debug.Log("NEW FLOOR " + _current.name);
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
    }
}
#endif