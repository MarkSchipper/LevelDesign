using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Mono.Data.Sqlite;
using System.Data;

namespace LevelEditor
{

    public class ObjectPainter : EditorWindow
    {

        private bool _SHOWICONS = true;
        private UnityEngine.Object[] _loadAllLevels;
        private UnityEngine.Object[] _loadAllLoadingScreens;

        private List<string> _loadLevelNames = new List<string>();
        private List<string> _loadingScreenNames = new List<string>();

        // Environment ICONS

        private List<string> _AllPotionNames = new List<string>();

        private string[] _themeSelection;
        private string[] _themeSelectType;
        private string[] _gameplaySelectType;
        private string[] _triggerSelectType;
        private string[] _vikingTileSelectType;

        private int _loadLevelIndex;
        private int _loadScreenIndex;
        private int _triggerSelectIndex;
        private int _soundSelectIndex;

        private int _vikingTileSelectIndex;
        
        private bool _addLoadLevels;
        private bool _addTriggers;
        private bool _addItems;
        private bool _addPotions;
        private static bool _isAddingTriggers = false;

        private bool _isGroundLevel;

        private bool _isLayerSet = false;
        private int _oldLayer = 0;
        private int[] _childLayers;

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
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;

            _loadAllLevels = Resources.LoadAll("Scenes/");
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

            _themeSelection = new string[] { "Settlement", "Viking", "Graveyard", "Dungeon" };
            _themeSelectType = new string[] { "Buildings", "Tiles", "Perimeter", "Props", "Borders" };
            _gameplaySelectType = new string[] { "Switch Scene Trigger", "Triggers", "Events" };
            _triggerSelectType = new string[] { "Audio Trigger", "Animation Trigger", "GamePlay" };
            _vikingTileSelectType = new string[] {"Surfaces", "Edges" };

            LevelEditor.Utils.FloorCheck.GetAllFloors();
            Theme.LevelItems.LoadAll();
            titleContent = new GUIContent("Level Editor");
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

        //////////////////////////////////////////////////////////////////////////////////////////
        //                                      OnGUI actions                                   //
        //                                                                                      //
        //  Create the buttons in the main window                                               //
        //      Switch based on the input and call the corresponding functions                  //
        //                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////

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
            Rect view = GUILayoutUtility.GetRect(750, 1500);
            EditorGUILayout.EndScrollView();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //                                       OnSceneGUI                                     //
        //                                                                                      //                            
        //  Delegate of the Editor                                                              //
        //      Overrides the default Unity Editor                                              //
        //  Call the Handles.BeginGUI to set the override                                       //
        //                                                                                      //                            
        //////////////////////////////////////////////////////////////////////////////////////////

        void OnSceneGUI(SceneView _sceneView)
        {
            Handles.BeginGUI();

            Vector3 _newPos;
            int counter = 0;

            // Check which theme has the _objectToAdd and get the snap amount

            if(Theme.Settlement.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Settlement.ReturnObjectToAdd();
                _snapAmount = Theme.Settlement.ReturnSnapAmount();
            }

            if (Theme.Viking.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Viking.ReturnObjectToAdd();
                _snapAmount = Theme.Viking.ReturnSnapAmount();
            }
            if (Theme.Graveyard.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Graveyard.ReturnObjectToAdd();
                _snapAmount = Theme.Graveyard.ReturnSnapAmount();
            }
            if (Theme.Dungeon.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Dungeon.ReturnObjectToAdd();
                _snapAmount = Theme.Dungeon.ReturnSnapAmount();
            }

            if (Theme.Swamp.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Swamp.ReturnObjectToAdd();
                _snapAmount = Theme.Swamp.ReturnSnapAmount();
            }
            if(Theme.Environment.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Environment.ReturnObjectToAdd();
                _snapAmount = Theme.Environment.ReturnSnapAmount();
            }
            if(Theme.Triggers.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.Triggers.ReturnObjectToAdd();
            }
            if(Theme.LevelItems.ReturnObjectToAdd() != null)
            {
                _objectToAdd = Theme.LevelItems.ReturnObjectToAdd();
                _snapAmount = Theme.LevelItems.ReturnSnapAmount();
            }

            //////////////////////////////////////////////////////////////////////////////////////////
            //                                      adding to the scene                             //
            //                                                                                      //
            //  Call HandleUtility.AddDefaultControl                                                //
            //      Helper functions for the Scene View                                             //
            //  SceneViews[0] - get the default scene view from Unity                               //
            //      Set the Focus on this view since the focus was on the EditorWindow              //
            //                                                                                      //
            //////////////////////////////////////////////////////////////////////////////////////////

            #region ADDING TO SCENE
            if (_isAddingToScene && _objectToAdd != null)
            {
                
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                Ray _ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _hit;

                // Set the focus on the editor
                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                sceneView.Focus();

                //////////////////////////////////////////////////////////////////////////////////////////////
                // This is to get all the layers from the selected object                                   //
                //  This is done since we need to override all the layers and set it to Ignore Raycast      //
                //  Afterwards we need to set the layers back to the original                               //
                //////////////////////////////////////////////////////////////////////////////////////////////

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

                    // Snapping
                    _newPos = new Vector3((int)Mathf.Round(_hit.point.x / _snapAmount) * _snapAmount, (int)Mathf.Round(_hit.point.y / 1) * 1, (int)Mathf.Round(_hit.point.z / _snapAmount) * _snapAmount);
                    _objectToAdd.transform.position = _newPos;

                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                    {
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

                            //////////////////////////////////////////////////////////////////////////////////////////////////////
                            //                                                                                                  //
                            //                                          Switcharoo                                              //
                            //  We want to keep on painting so on mouse button down we create a new instance of the             //
                            //  currently selected object                                                                       //
                            //      Call the ParentCheck class to place it under the correct and corresponding GameObject       //
                            //  Set the position of the clone of the object to the snapped position                             //
                            //                                                                                                  //
                            //////////////////////////////////////////////////////////////////////////////////////////////////////
                            GameObject _oldObj = Instantiate(_objectToAdd);
                            LevelEditor.Utils.ParentCheck.CheckParentObjects(_worldTabIndex, _oldObj, _worldTypeTabIndex);
                            _oldObj.transform.position = _newPos;
                        }

                        // If we are adding a Trigger we set the layer to 2 ( Ignore Raycast )
                        // Also cancel the _isAddingToScene so we can only add 1 object

                        if (_isAddingTriggers)
                        {
                            _objectToAdd.layer = 2;
                            _objectToAdd.transform.position = _newPos;
                            _isAddingToScene = false;
                        }
                        // If it is a Procedural Building, call the CreateBuilding function

                        if (_objectToAdd.GetComponent<ProceduralBuilding>() != null)
                        {
                            _objectToAdd.GetComponent<ProceduralBuilding>().CreateBuilding();
                        }
                        counter = 0;
                    }

                    // If right click, cancel the adding to scene and destroy the object
                    if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
                    {
                        if (_objectToAdd != null)
                        {
                            DestroyImmediate(_objectToAdd);
                        }
                        _isAddingToScene = false;
                    }
                }

                // Rotation functions

                if (Event.current.keyCode == (KeyCode.A) && Event.current.type == EventType.KeyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y + 45, _objectToAdd.transform.eulerAngles.z);
                }
                if (Event.current.keyCode == (KeyCode.D) && Event.current.type == EventType.KeyDown)
                {
                    _objectToAdd.transform.localEulerAngles = new Vector3(_objectToAdd.transform.eulerAngles.x, _objectToAdd.transform.eulerAngles.y - 45, _objectToAdd.transform.eulerAngles.z);
                }

                // Cancel the adding the scene 
                if(Event.current.keyCode == KeyCode.Escape)
                {
                    _isAddingToScene = false;
                }

                // If W is pressed, cycle up through the objects from the current theme 
                if(Event.current.keyCode == KeyCode.W && Event.current.type == EventType.KeyUp)
                {
                    if (Theme.Settlement.ReturnObjectToAdd() != null)
                    {
                        Theme.Settlement.NextObject(_worldTypeTabIndex);
                    }
                    if (Theme.Viking.ReturnObjectToAdd() != null)
                    {
                        Theme.Viking.NextObject(_worldTypeTabIndex, _vikingTileSelectIndex);
                    }
                    if (Theme.Graveyard.ReturnObjectToAdd() != null)
                    {
                        Theme.Graveyard.NextObject();
                    }
                    if (Theme.Dungeon.ReturnObjectToAdd() != null)
                    {
                        Theme.Dungeon.NextObject(_worldTypeTabIndex);
                    }
                    if (Theme.Swamp.ReturnObjectToAdd() != null)
                    {
                        Theme.Swamp.NextObject(_worldTypeTabIndex);
                    }
                    if (Theme.Environment.ReturnObjectToAdd() != null)
                    {
                        Theme.Environment.NextObject();
                    }
                }
                if(Event.current.keyCode == KeyCode.S && Event.current.type == EventType.KeyUp)
                {
                    if (Theme.Settlement.ReturnObjectToAdd() != null)
                    {
                        Theme.Settlement.PreviousObject(_worldTypeTabIndex);
                    }
                    if (Theme.Viking.ReturnObjectToAdd() != null)
                    {
                        Theme.Viking.PreviousObject(_worldTypeTabIndex, _vikingTileSelectIndex);
                    }
                    if (Theme.Graveyard.ReturnObjectToAdd() != null)
                    {
                        Theme.Graveyard.PreviousObject();
                    }
                    if (Theme.Dungeon.ReturnObjectToAdd() != null)
                    {
                        Theme.Dungeon.PreviousObject(_worldTypeTabIndex);
                    }
                    if (Theme.Swamp.ReturnObjectToAdd() != null)
                    {
                        Theme.Swamp.PreviousObject(_worldTypeTabIndex);
                    }
                    if (Theme.Environment.ReturnObjectToAdd() != null)
                    {
                        Theme.Environment.PreviousObject();
                    }
                }

                
            }
            #endregion

            if(!_isAddingToScene)
            {
                Theme.Settlement.DeleteLoadedObject();
                Theme.Dungeon.DeleteLoadedObject();
                Theme.Environment.DeleteLoadedObject();
                Theme.Graveyard.DeleteLoadedObject();
                Theme.Swamp.DeleteLoadedObject();
                Theme.Viking.DeleteLoadedObject();
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
                        LevelEditor.Utils.FloorCheck.CheckFloors();
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
            Debug.Log(_worldTypeTabIndex);
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
                        LevelEditor.Utils.FloorCheck.CheckFloors();
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
                    LevelEditor.Utils.FloorCheck.CheckFloors();
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
                    LevelEditor.Utils.FloorCheck.CheckFloors();
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
                    Theme.Triggers.AudioTrigger();
                    break;
                case 1:
                    break;
                case 2:
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                    Theme.Triggers.GameplayTrigger();
                    break;
                default:
                    break;
            }
        }

        void AddStaticProps()
        {
            if(!Theme.Environment.ReturnHasLoadedObjects())
            {
                Theme.Environment.LoadAll();
            } 
            Theme.Environment.AddStaticProps(5);
        }

        void AddItems()
        {
            _itemTabIndex = GUILayout.Toolbar(_itemTabIndex, new string[] { "Add Potions" });
            switch (_itemTabIndex)
            {
                case 0:
                    Theme.LevelItems.AddPotions(5);
                    break;
                default:
                    break;
            }
        }
       
        public static void SetAddingToScene()
        {
            _isAddingToScene = true;
        }
        
        public static void SetAddingTriggersToScene(bool _set)
        {
            _isAddingTriggers = _set;
        }

    }
}
