using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

#if UNITY_EDITOR

//  Create a enum
// We say " Zero = 0"  because we want to use an integer for the character ID

public enum CharacterList
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Camera = 4,
}

public class NodeEditor : EditorWindow {

    [SerializeField]
    private List<BaseNode> windows = new List<BaseNode>();

    [SerializeField]
    private List<BaseNode> cameraWindows = new List<BaseNode>();

    [SerializeField]
    private List<BaseKnob> windowKnob = new List<BaseKnob>();

    [SerializeField]
    private List<int> nodeID = new List<int>();

    [SerializeField]
    private List<int> cameraNodeID = new List<int>();

    [SerializeField]
    private List<GameObject> allGO = new List<GameObject>();

    [SerializeField]
    private List<GameObject> cameraGO = new List<GameObject>();

    // the nodeCounter to make sure every node has a unique ID
    private int nodeCounter;

    // the Camera node Counter to make sure every node has an unique ID
    private int cameraCounter;

    // Store the mousePosition ( current event e.mousePosition
    private Vector2 mousePos;

    // The selected node
    private BaseNode selectedNode;

    // Are we in transitionMode
    private bool makeTransitionMode = false;

    

    public GUISkin _skin;

    // Array to store all the CameraNode GameObjects in the scene
    private GameObject[] _cameraGO;

    // Array to store all the GameObjects in the scene
    private GameObject[] _sceneGO;

    // Are we loading the canvas
    private bool _isLoading = false;

    
    // Floats to change the size of our node windows
    private float _windowWidth = 250f;
    private float _windowHeight = 120f;

    // Vars to create the dirty canvas moving
    private Vector2 _initialPos;
    private bool _setPosition;

    private CharacterList _charSelect;



    // Booleans for loading the canvas
    private bool _charOneNodesLoaded = false;
    private bool _charTwoNodesLoaded = false;
    private bool _charThreeNodesLoaded = false;
    private bool _charFourNodesLoaded = false;
    private bool _cameraNodesLoaded = false;

    [MenuItem("Cinematics Manager/Node Editor")]

    static void ShowEditor()
    {

        // Creating a new window
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
                          

    }

    void Update()
    {
       
        Repaint();

    
    }

    void OnGUI()
    {
  

        GUI.skin = _skin;
        //Handles.BeginGUI();

        GameObject[] _tmpGO = GameObject.FindGameObjectsWithTag("Node").OrderBy(go => go.name).ToArray();

        // Different sorting method
        // OrderBy works fine but when you have more than 9 items it sorts it wrong
        // 1,10,2,3,4,5,6,7,8,9
        //
        // By using the int parser and substring we can sort it out properly
        _sceneGO = GameObject.FindGameObjectsWithTag("Node").OrderBy(go => int.Parse(go.name.Substring(4))).ToArray();

        _cameraGO = GameObject.FindGameObjectsWithTag("CameraNode").OrderBy(go => int.Parse(go.name.Substring(10))).ToArray();

        _charSelect = (CharacterList)EditorGUILayout.EnumPopup("CharacterID", _charSelect);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                  //
        //                                  LOADING FROM THE SCENE                                                          //
        //                                                                                                                  //
        // We check to see if there are any gameobjects in the scene with the tag 'Node'                                    //
        // If there are we:                                                                                                 //
        //              Create a new Node based on the animation type ( ReturnAnim() )                                      //
        //              Draw a new Rect and place it at the positions we get from the NodeObject component                  //
        //              Set the WayPoint ( GameObject )                                                                     //
        //              We add the node to the windows List                                                                 //
        //              We add the current GameObject from the scene in our allGO List                                      //
        //              We set the nodeID which we get from nodeCounter var                                                 //
        //              We add the nodeID ( nodeCounter ) to the nodeID list                                                //
        //              We add 1 to the nodeCounter                                                                         //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (!_charOneNodesLoaded || !_charTwoNodesLoaded || !_charThreeNodesLoaded || !_charFourNodesLoaded)
        {
            if ((int)_charSelect < 4)
            {
                if (GUILayout.Button("Load Character " + (int)_charSelect + " nodes from scene"))
                {

                    if ((int)_charSelect == 0)
                    {
                        _charOneNodesLoaded = true;
                    }
                    if ((int)_charSelect == 1)
                    {
                        _charTwoNodesLoaded = true;
                    }

                    if ((int)_charSelect == 2)
                    {
                        _charThreeNodesLoaded = true;
                    }

                    if ((int)_charSelect == 3)
                    {
                        _charFourNodesLoaded = true;
                    }

                    // LOAD ALL THE ANIMATION NODES 
                    for (int i = 0; i < _sceneGO.Length; i++)
                    {
                        if (_sceneGO[i].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                        {

                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Start")
                            {
                                AnimationStartNode animStartNode = new AnimationStartNode();
                                animStartNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);


                                animStartNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());

                                windows.Add(animStartNode);


                                allGO.Add(_sceneGO[i]);

                                animStartNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);



                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Idle")
                            {
                                IdleNode idleNode = new IdleNode();
                                idleNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);



                                windows.Add(idleNode);

                                allGO.Add(_sceneGO[i]);

                                idleNode.SetIdle(_sceneGO[i].GetComponent<NodeObject>().ReturnIdleWait());
                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    idleNode.SetSound("Yes");
                                    idleNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                idleNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Walk")
                            {
                                WalkNode walkNode = new WalkNode();
                                walkNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);



                                walkNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());

                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    walkNode.SetSound("Yes");
                                    walkNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                windows.Add(walkNode);

                                allGO.Add(_sceneGO[i]);

                                walkNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Run")
                            {
                                RunNode runNode = new RunNode();
                                runNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);


                                runNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());


                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    runNode.SetSound("Yes");
                                    runNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                windows.Add(runNode);

                                allGO.Add(_sceneGO[i]);

                                runNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }

                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "CombatIdle")
                            {
                                CombatIdleNode combatIdleNode = new CombatIdleNode();
                                combatIdleNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);

                                combatIdleNode.SetIdle(_sceneGO[i].GetComponent<NodeObject>().ReturnIdleWait());


                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    combatIdleNode.SetSound("Yes");
                                    combatIdleNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                windows.Add(combatIdleNode);

                                allGO.Add(_sceneGO[i]);

                                combatIdleNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }

                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "JumpTo")
                            {
                                JumpToNode jumpToNode = new JumpToNode();
                                jumpToNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);



                                jumpToNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());


                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    jumpToNode.SetSound("Yes");
                                    jumpToNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                windows.Add(jumpToNode);

                                allGO.Add(_sceneGO[i]);

                                jumpToNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }


                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "CustomNode")
                            {

                                CustomNode customNode = new CustomNode();
                                customNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 100);



                                customNode.SetCustomAnimation(_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnim());

                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Idle")
                                {
                                    customNode.SetIdle(_sceneGO[i].GetComponent<NodeObject>().ReturnIdleWait());
                                    customNode.SetAnimType("Idle");
                                }
                                else if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Walk" || _sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Run" || _sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Ranged")
                                {

                                    customNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());

                                    if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Walk")
                                    {
                                        customNode.SetAnimType("Walk");
                                    }
                                    if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Run")
                                    {
                                        customNode.SetAnimType("Run");
                                    }

                                    if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Ranged")
                                    {
                                        customNode.SetAnimType("Ranged");
                                    }

                                    if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                    {

                                        customNode.SetSound("Yes");
                                        customNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                    }

                                }
                                else if (_sceneGO[i].GetComponent<NodeObject>().ReturnCustomAnimType() == "Gesture")
                                {
                                    customNode.SetAnimType("Gesture");
                                }

                                windows.Add(customNode);

                                allGO.Add(_sceneGO[i]);

                                customNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Ranged")
                            {
                                RangedNode rangedNode = new RangedNode();
                                rangedNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);


                                rangedNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());
                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {

                                    rangedNode.SetSound("Yes");
                                    rangedNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }


                                windows.Add(rangedNode);

                                allGO.Add(_sceneGO[i]);


                                rangedNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "SoundTrack")
                            {
                                SoundTrackNode soundtrackNode = new SoundTrackNode();
                                soundtrackNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight);


                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {


                                    soundtrackNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }


                                windows.Add(soundtrackNode);

                                allGO.Add(_sceneGO[i]);


                                soundtrackNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }

                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Fade")
                            {
                                FadeNode fadeNode = new FadeNode();
                                fadeNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 160);

                                fadeNode.SetFade(_sceneGO[i].GetComponent<NodeObject>().ReturnFadeTime());
                                fadeNode.SetSolid(_sceneGO[i].GetComponent<NodeObject>().ReturnSolidTime());
                                fadeNode.SetFadeAnimStart(_sceneGO[i].GetComponent<NodeObject>().ReturnFadeAnimStart());

                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnFadeAction() != "Nothing")
                                {
                                    fadeNode.SetAction("Teleport");
                                }

                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint() != null)
                                {
                                    fadeNode.SetWayPoint(_sceneGO[i].GetComponent<NodeObject>().ReturnWayPoint());
                                }

                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {


                                    fadeNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }



                                windows.Add(fadeNode);

                                allGO.Add(_sceneGO[i]);


                                fadeNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }
                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "ParticleSystem")
                            {
                                ParticleNode particleNode = new ParticleNode();
                                particleNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 50);

                                particleNode.SetParticleSystem(_sceneGO[i].GetComponent<NodeObject>().ReturnParticleSystem());
                                particleNode.SetParticleAction(_sceneGO[i].GetComponent<NodeObject>().ReturnParticleAction());
                                particleNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                windows.Add(particleNode);
                                allGO.Add(_sceneGO[i]);
                                particleNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }

                            if (_sceneGO[i].GetComponent<NodeObject>().ReturnAnim() == "Image")
                            {
                                ImageNode imageNode = new ImageNode();
                                imageNode.windowRect = new Rect(_sceneGO[i].GetComponent<NodeObject>().ReturnPosX(), _sceneGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 100);

                                imageNode.SetImage(_sceneGO[i].GetComponent<NodeObject>().ReturnUserImage());
                                imageNode.SetImageMode(_sceneGO[i].GetComponent<NodeObject>().ReturnImageMode());
                                imageNode.SetScreenTime(_sceneGO[i].GetComponent<NodeObject>().ReturnImageTime());


                                if (_sceneGO[i].GetComponent<NodeObject>().ReturnAudio() != null)
                                {


                                    imageNode.SetAudio(_sceneGO[i].GetComponent<NodeObject>().ReturnAudio());

                                }

                                windows.Add(imageNode);
                                allGO.Add(_sceneGO[i]);
                                imageNode.SetID(nodeCounter);
                                nodeID.Add(nodeCounter);
                                nodeCounter++;
                            }


                           



                        }
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                                          //
        //                                                                                                                                          //
        //                                                              CAMERA NODES                                                                //
        //                                                                                                                                          //
        //                                                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        if (!_cameraNodesLoaded)
        {
            if ((int)_charSelect == 4)
            {
                if (GUILayout.Button("Load Camera Nodes from scene"))
                {

                    _cameraNodesLoaded = true;

                    // LOAD ALL THE CAMERA NODES
                    for (int i = 0; i < _cameraGO.Length; i++)
                    {

                        if (_cameraGO[i].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                        {
                            _cameraGO[i].GetComponent<NodeObject>().ReturnCamera().SetActive(true);
                            if (_cameraGO[i].GetComponent<NodeObject>().ReturnAnim() == "CameraSetup")
                            {
                                CameraSetupNode cameraSetupNode = new CameraSetupNode();
                                cameraSetupNode.windowRect = new Rect(_cameraGO[i].GetComponent<NodeObject>().ReturnPosX(), _cameraGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 150);


                                cameraSetupNode.SetCameraSettings(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnCameraSettings());
                                cameraSetupNode.SetCameraName(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnCameraName());
                                cameraSetupNode.SetAutoCreateCamera(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnAutoCreateCamera());
                                cameraSetupNode.SetInitialCamera(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnInitialCamera());
                                cameraSetupNode.SetHasAnimation(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnInitialCameraAnimation());
                                cameraSetupNode.SetCameraMode(_cameraGO[i].GetComponent<NodeObject>().ReturnCameraEnd());
                                cameraSetupNode.SetCameraEndTime(_cameraGO[i].GetComponent<NodeObject>().ReturnCameraEndTime());


                                cameraWindows.Add(cameraSetupNode);
                                cameraGO.Add(_cameraGO[i]);
                                cameraSetupNode.SetID(cameraCounter);
                                cameraNodeID.Add(cameraCounter);
                                cameraCounter++;
                            }

                            if (_cameraGO[i].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                            {
                                CameraNode cameraNode = new CameraNode();
                                cameraNode.windowRect = new Rect(_cameraGO[i].GetComponent<NodeObject>().ReturnPosX(), _cameraGO[i].GetComponent<NodeObject>().ReturnPosY(), _windowWidth, _windowHeight + 100);

                                cameraNode.SetCamera(_cameraGO[i].GetComponent<NodeObject>().ReturnCamera());
                                cameraNode.SetCameraMode(_cameraGO[i].GetComponent<NodeObject>().ReturnCameraEnd());
                                cameraNode.SetCameraEndTime(_cameraGO[i].GetComponent<NodeObject>().ReturnCameraEndTime());

                                cameraWindows.Add(cameraNode);
                                cameraGO.Add(_cameraGO[i]);
                                cameraNode.SetID(cameraCounter);
                                cameraNodeID.Add(cameraCounter);
                                cameraCounter++;
                            }

                        }

                    }
                }
            }

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                                          //
        // We try to redraw the existing connections between nodes                                                                                  //
        // if the _sceneGO is not empty AND there is at least 1 node in the windows list                                                            //
        //      Go through all the objects                                                                                                          //
        //      Safety check to see if the specific _sceneGO object is not null                                                                     //
        //      If the outputID is greater than 0 ( else we get a circle since the curve will be drawn back to its origin                           //
        //      Draw the curve from the current window to the window that is stated in the outputID from the NodeObject attached to itself          //
        //                                                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if ((int)_charSelect < 4)
        {
            if (_sceneGO != null && windows.Count > 0)
            {
                for (int j = 0; j < _sceneGO.Length; j++)
                {
                    if (_sceneGO[j] != null)
                    {
                        if (_sceneGO[j].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                        {

                            if (_sceneGO[j].GetComponent<NodeObject>().ReturnOutputID() > 0)
                            {

                                if (windows[_sceneGO[j].GetComponent<NodeObject>().ReturnOutputID()] != null && _sceneGO[j].GetComponent<NodeObject>().ReturnOutputID() > 0)
                                {
                                    DrawNodeCurve(windows[j].windowRect, windows[_sceneGO[j].GetComponent<NodeObject>().ReturnOutputID()].windowRect);
                                    Repaint();

                                }
                                else
                                {

                                }

                            }
                            else
                            {
                                // Debug.Log("There is a node that no longer exists so we shall skip it");
                            }
                        }
                    }
                    else
                    {

                        // Error catching
                        Debug.Log("the array _sceneGO is empty");

                    }
                }
            }
        }

        if ((int)_charSelect == 4)
        { 
            if (_cameraGO != null && cameraWindows.Count > 0)
            {
                for (int j = 0; j < _cameraGO.Length; j++)
                {
                    if (_cameraGO[j] != null)
                    {
                        if (_cameraGO[j].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                        {

                            if (_cameraGO[j].GetComponent<NodeObject>().ReturnOutputID() > 0)
                            {

                                if (cameraWindows[_cameraGO[j].GetComponent<NodeObject>().ReturnOutputID()] != null && _cameraGO[j].GetComponent<NodeObject>().ReturnOutputID() > 0)
                                {
                                    DrawNodeCurve(cameraWindows[j].windowRect, cameraWindows[_cameraGO[j].GetComponent<NodeObject>().ReturnOutputID()].windowRect);
                                    Repaint();

                                }
                                else
                                {

                                }

                            }
                            else
                            {
                                // Debug.Log("There is a node that no longer exists so we shall skip it");
                            }
                        }
                    }
                    else
                    {

                        // Error catching
                        Debug.Log("the array _cameraGO is empty");

                    }
                }
            }
        }
    
        else
        {
            
          //  Debug.Log("Window counting error");
        }

        /////////////////////////////////////////////////////////////////////////
        //                          CLEARING THE CANVAS
        //
        // Clear Canvas
        // Get all the GameObjects that are asociated with a node
        // DestroyImmediate -> Editor function -> Destroy does not work in the editor
        //
        // Clear all the lists
        /////////////////////////////////////////////////////////////////////////

        if (GUILayout.Button("Clear Canvas"))
        {


            for (int i = 0; i < _tmpGO.Length; i++)
            {
                if (_tmpGO[i].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                {
                    DestroyImmediate(_tmpGO[i]);
                }
            }

            windows.Clear();
            allGO.Clear();
            nodeCounter = 0;


        }


        Event e = Event.current;
        mousePos = e.mousePosition;

        // If Right button and we are not in transitionMode ( drawing a curve )

        if (e.button == 1 && !makeTransitionMode)
        {
            _isLoading = false;
            // Have we clicked
            if (e.type == EventType.mouseDown)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                if ((int)_charSelect < 4)
                {

                    for (int i = 0; i < windows.Count; i++)
                    {
                        // Check to see if we clicked on our selected node (itself)
                        if (windows[i].windowRect.Contains(mousePos))
                        {

                            selectIndex = i;
                            clickedOnWindow = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cameraWindows.Count; i++)
                    {
                        // Check to see if we clicked on our selected node (itself)
                        if (cameraWindows[i].windowRect.Contains(mousePos))
                        {

                            selectIndex = i;
                            clickedOnWindow = true;
                            break;
                        }
                    }
                }

                // If we have not clicked on a window but still right clicked we want to Menu to appear to add new nodes

                if (!clickedOnWindow)
                {
                    GenericMenu menu = new GenericMenu();


                    // If we are NOT in the Camera Node Editor
                    if (_charSelect != CharacterList.Camera)
                    {
                        // ANIMATION MENUS
                        menu.AddDisabledItem(new GUIContent("[ Cinematics ]"));
                        menu.AddItem(new GUIContent("Animations/Add Animation Start Node"), false, ContextCallback, "animStartNode");
                        menu.AddItem(new GUIContent("Animations/Add Idle Node"), false, ContextCallback, "idleNode");
                        menu.AddItem(new GUIContent("Animations/Add Walk To Node"), false, ContextCallback, "walkNode");
                        menu.AddItem(new GUIContent("Animations/Add Run To Node"), false, ContextCallback, "runNode");
                        menu.AddItem(new GUIContent("Animations/Add Combat Idle Node"), false, ContextCallback, "combatIdleNode");
                        menu.AddItem(new GUIContent("Animations/Add Teleport To Node"), false, ContextCallback, "jumpToNode");
                        menu.AddItem(new GUIContent("Animations/Add Custom Animation Node"), false, ContextCallback, "customNode");

                        //ATTACK ANIMATIONS
                        menu.AddItem(new GUIContent("Attack Animations/Add Ranged Attack Node"), false, ContextCallback, "rangedNode");


                        // SOUND
                        menu.AddItem(new GUIContent("Audio/Add Sound Track"), false, ContextCallback, "soundtrackNode");

                        // EDITING
                        menu.AddItem(new GUIContent("Editing/Add Fade to colour"), false, ContextCallback, "fadeNode");
                        menu.AddItem(new GUIContent("Editing/Add Image Node"), false, ContextCallback, "imageNode");

                        // SFX
                        menu.AddItem(new GUIContent("SFX/Particle System Node "), false, ContextCallback, "particleNode");
                    }

                    // IF we are in the Camera Node Editor only show the Camera Nodes
                    if (_charSelect == CharacterList.Camera)
                    {
                        menu.AddItem(new GUIContent("Camera/Setup"), false, ContextCallback, "cameraSetupNode");
                        menu.AddItem(new GUIContent("Camera/Camera Animation"), false, ContextCallback, "cameraNode");
                    }

                    menu.ShowAsContext();
                    e.Use();

                }

                else
                {
                    // If we have clicked on a itself show the different menu
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
                    menu.AddSeparator(" ");
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                    menu.ShowAsContext();
                    e.Use();

                }

            }
        }

        // If we have clicked on makeTransition 

        else if (e.button == 0 && e.type == EventType.mouseDown && makeTransitionMode)
        {

            bool clickedOnWindow = false;
            int selectIndex = -1;

            if ((int)_charSelect < 4)
            {

                for (int i = 0; i < windows.Count; i++)
                {
                    // Get the current window we have selected ( FROM: Node )
                    if (windows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                // If we have clicked on a window that is not the window we have selected ( TO: node )
                if (clickedOnWindow && !windows[selectIndex].Equals(selectedNode))
                {

                    // Set the input in the TO node
                    windows[selectIndex].SetInput((BaseInputNode)selectedNode, mousePos);
                    Debug.Log("From node: " + selectedNode + " to: " + windows[selectIndex]);
                    //Debug.Log(selectedNode.ReturnID());
                    //Debug.Log(windows[selectIndex].ReturnID());

                    // In our FROM node we set the outputID from the TO node

                    if (allGO[selectedNode.ReturnID()] != null)
                    {
                        allGO[selectedNode.ReturnID()].GetComponent<NodeObject>().setOutputID(windows[selectIndex].ReturnID());
                    }

                    makeTransitionMode = false;
                    selectedNode = null;

                }

                if (!clickedOnWindow)
                {
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                e.Use();
            }
            else
            {

                for (int i = 0; i < cameraWindows.Count; i++)
                {
                    if(cameraWindows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }

                }

                if(clickedOnWindow && !cameraWindows[selectIndex].Equals(selectedNode))
                {
                    cameraWindows[selectIndex].SetInput((BaseInputNode)selectedNode, mousePos);

                    if(cameraGO[selectedNode.ReturnID()] != null)
                    {
                        cameraGO[selectedNode.ReturnID()].GetComponent<NodeObject>().setOutputID(cameraWindows[selectIndex].ReturnID());
                    }
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                if(!clickedOnWindow)
                {
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                e.Use();

            }
        }

        // If we have clicked on a window but we are NOT in transitionMode
        // We set the node we have clicked to nodeToChange
        else if (e.button == 0 && e.type == EventType.mouseDown && !makeTransitionMode)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            if ((int)_charSelect < 4)
            {

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    BaseInputNode nodeToChange = windows[selectIndex].ClickedOnInput(mousePos);
                    if (nodeToChange != null)
                    {
                        selectedNode = nodeToChange;
                        makeTransitionMode = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cameraWindows.Count; i++)
                {
                    if (cameraWindows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    BaseInputNode nodeToChange = cameraWindows[selectIndex].ClickedOnInput(mousePos);
                    if (nodeToChange != null)
                    {
                        selectedNode = nodeToChange;
                        makeTransitionMode = true;
                    }
                }
            }
        }

        // If we are in transitionMode and clicked on a node 

        if (makeTransitionMode && selectedNode != null)
        {
            // Draw the Bezier Curve
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            DrawNodeCurve(selectedNode.windowRect, mouseRect);

            Repaint();
        }

        foreach (BaseNode n in windows)
        {
            n.DrawCurves();
        }

        BeginWindows();
        // GUILayout.BeginScrollView();
        if ((int)_charSelect < 4)
        {
            for (int i = 0; i < windows.Count; i++)
            {

                if (allGO[i] != null)
                {
                    if (allGO[i].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                    {
                        windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);

                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cameraWindows.Count; i++)
            {

                if (cameraGO[i] != null)
                {
                    if (cameraGO[i].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
                    {
                        cameraWindows[i].windowRect = GUI.Window(i, cameraWindows[i].windowRect, DrawNodeWindow, cameraWindows[i].windowTitle);

                    }
                }
            }
        }

        

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                    DIRTY CANVAS MOVING               
        // 
        // Scaling with the GUI.matrix does not really work and is pretty nasty we create a workaround
        //
        //          If we are dragging with the scrollWheel
        //          If the initial position is not equal to the current mousePosition
        //          If the bool _setPosition is false
        //          Set the _initialPos to our current mousePosition
        //          Set the bool _setPosition to true ( so we only set it ONCE per drag )
        //          Calculate the difference between the initial position and the current mousePosition
        //                          The difference is used to offset the nodes
        //          Get ALL the windows in our current windows List
        //          Move them on x and y
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        if (e.button == 2 && e.type ==  EventType.MouseDrag)
        {
            if (_initialPos != e.mousePosition)
            {
               
                if(!_setPosition)
                {
                    _initialPos = e.mousePosition;
                    _setPosition = true;
                }

                Vector2 _offSet = _initialPos - e.mousePosition;

                if ((int)_charSelect < 4)
                {

                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (_initialPos.x < e.mousePosition.x)
                        {
                            windows[i].windowRect.x -= _offSet.x;
                            _setPosition = false;

                        }
                        if (_initialPos.x > e.mousePosition.x)
                        {
                            windows[i].windowRect.x -= _offSet.x;
                            _setPosition = false;

                        }
                        if (_initialPos.y < e.mousePosition.y)
                        {
                            windows[i].windowRect.y -= _offSet.y;
                            _setPosition = false;
                        }
                        if (_initialPos.y > e.mousePosition.y)
                        {
                            windows[i].windowRect.y -= _offSet.y;
                            _setPosition = false;
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < cameraWindows.Count; i++)
                    {
                        if (_initialPos.x < e.mousePosition.x)
                        {
                            cameraWindows[i].windowRect.x -= _offSet.x;
                            _setPosition = false;

                        }
                        if (_initialPos.x > e.mousePosition.x)
                        {
                            cameraWindows[i].windowRect.x -= _offSet.x;
                            _setPosition = false;

                        }
                        if (_initialPos.y < e.mousePosition.y)
                        {
                            cameraWindows[i].windowRect.y -= _offSet.y;
                            _setPosition = false;
                        }
                        if (_initialPos.y > e.mousePosition.y)
                        {
                            cameraWindows[i].windowRect.y -= _offSet.y;
                            _setPosition = false;
                        }

                    }
                }
            }
        }

        
        EndWindows();
           
        
    }


    // Draw the node as a window

    void DrawNodeWindow(int id)
    {
        if ((int)_charSelect < 4)
        {
            if (allGO[id].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
            {
                windows[id].DrawWindow();
                // set it dragable
                GUI.DragWindow();

                if (id != null)
                {

                    allGO[id].GetComponent<NodeObject>().setPosition(windows[id].windowRect.x, windows[id].windowRect.y);
                }

            }
            else
            {
                Repaint();
            }
        }
        else
        {
            if (cameraGO[id].GetComponent<NodeObject>().ReturnCharID() == (int)_charSelect)
            {
                cameraWindows[id].DrawWindow();
                GUI.DragWindow();

                if (id != null)
                {
                    cameraGO[id].GetComponent<NodeObject>().setPosition(cameraWindows[id].windowRect.x, cameraWindows[id].windowRect.y);
                }
            }
        }
    }

    void DrawInputWindow(int id)
    {
        windowKnob[id].DrawWindow();
        GUI.DragWindow();

        Repaint();
    }

    // ContextCallBack()
    // Called by the OnGUI() to create a new Menu item
    // clb = Callback


    void ContextCallback(object obj)
    {
        string clb = obj.ToString();
        GameObject _parent = GameObject.FindGameObjectWithTag("NodeParent");
        
        // If the callback is....        
        if(clb.Equals("animStartNode"))
        {

            // Create a new instance of a class and create a new rect
            AnimationStartNode animStartNode = new AnimationStartNode();
            animStartNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            // Add it to the windows list
            windows.Add(animStartNode);

            // Create a new empty gameobject so we can store it and set the nodeID, title, animation and character id
            GameObject newNode = new GameObject();

                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(animStartNode.windowTitle);
                newNode.GetComponent<NodeObject>().setAnimation("Start");
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);
            
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);

            animStartNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

            
        }

        else if (clb.Equals("animNode"))
        {
            AnimationNode animNode = new AnimationNode();
            animNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(animNode);

            GameObject newNode = new GameObject();

                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(animNode.windowTitle);
                newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            animNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

          
        }

        else if(clb.Equals("idleNode"))
        {
            IdleNode idleNode = new IdleNode();
            idleNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            idleNode.SetID(nodeCounter);
            windows.Add(idleNode);
        

            GameObject newNode = new GameObject();
            

                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(idleNode.windowTitle);
                newNode.GetComponent<NodeObject>().setAnimation("Idle");
                newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect); 

            idleNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

           
        }

        else if(clb.Equals("walkNode"))
        {
            WalkNode walkNode = new WalkNode();
            walkNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(walkNode);

            GameObject newNode = new GameObject();

                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(walkNode.windowTitle);
                newNode.GetComponent<NodeObject>().setAnimation("Walk");
                newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            walkNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

        }

        else if(clb.Equals("runNode"))
        {
            RunNode runNode = new RunNode();
            runNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(runNode);

            GameObject newNode = new GameObject();

                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(runNode.windowTitle);
                newNode.GetComponent<NodeObject>().setAnimation("Run");
                newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            runNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

         
        }

        else if(clb.Equals("combatIdleNode"))   
        {
            CombatIdleNode combatIdleNode = new CombatIdleNode();
            combatIdleNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(combatIdleNode);

            GameObject newNode = new GameObject();


                newNode.AddComponent<NodeObject>();
                newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
                newNode.GetComponent<NodeObject>().setTitle(combatIdleNode.windowTitle);
                newNode.GetComponent<NodeObject>().setAnimation("CombatIdle");
                newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
                newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            combatIdleNode.SetID(nodeCounter);
                

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

           
        }

        else if(clb.Equals("jumpToNode"))
        {
            JumpToNode jumpNode = new JumpToNode();
            jumpNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);
            windows.Add(jumpNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(jumpNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("JumpTo");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);


            jumpNode.SetID(nodeCounter);


            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

        }

        else if(clb.Equals("customNode"))
        {
            CustomNode customNode = new CustomNode();
            customNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 100);

            windows.Add(customNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(customNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("CustomNode");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);
            

            customNode.SetID(nodeCounter);


            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;
        }

        else if (clb.Equals("rangedNode"))
        {
            RangedNode rangedNode = new RangedNode();
            rangedNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(rangedNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(rangedNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("Ranged");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            rangedNode.SetID(nodeCounter);
            

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;

        }

        else if (clb.Equals("soundtrackNode"))
        {
            SoundTrackNode soundNode = new SoundTrackNode();
            soundNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);

            windows.Add(soundNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(soundNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("SoundTrack");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            soundNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;
        }

        else if (clb.Equals("fadeNode"))
        {
            FadeNode fadeNode = new FadeNode();
            fadeNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 160);
                       
            windows.Add(fadeNode);
            
            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(fadeNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("Fade");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            fadeNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;
        }

        else if (clb.Equals("particleNode"))
        {
            ParticleNode particleNode = new ParticleNode();
            particleNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 50);

            windows.Add(particleNode);


            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(particleNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("ParticleSystem");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            particleNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;
        }
        else if (clb.Equals("imageNode"))
        {
            ImageNode imageNode = new ImageNode();
            imageNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 100);

            windows.Add(imageNode);


            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(nodeCounter);
            newNode.GetComponent<NodeObject>().setTitle(imageNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("Image");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            imageNode.SetID(nodeCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "Node" + nodeCounter;
            newNode.tag = "Node";
            nodeID.Add(nodeCounter);
            allGO.Add(newNode);
            nodeCounter++;
        }

        else if (clb.Equals("cameraSetupNode"))
        {
            CameraSetupNode cameraSetupNode = new CameraSetupNode();
            cameraSetupNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 150);

            cameraWindows.Add(cameraSetupNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(cameraCounter);
            newNode.GetComponent<NodeObject>().setTitle(cameraSetupNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("CameraSetup");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);

            cameraSetupNode.SetID(cameraCounter);

            newNode.transform.parent = _parent.transform;
            newNode.name = "CameraNode" + cameraCounter;
            newNode.tag = "CameraNode";
            cameraNodeID.Add(cameraCounter);
            cameraGO.Add(newNode);
            cameraCounter++;

            if(GameObject.Find("CameraManager") == null)
            {
                GameObject cameraManager = new GameObject();
                cameraManager.name = "CameraManager";
                cameraManager.AddComponent<CameraManager>();
            }


        }

        else if (clb.Equals("cameraNode"))
        {
            CameraNode cameraNode = new CameraNode();
            cameraNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight + 100);

            cameraWindows.Add(cameraNode);

            GameObject newNode = new GameObject();

            newNode.AddComponent<NodeObject>();
            newNode.GetComponent<NodeObject>().setNodeID(cameraCounter);
            newNode.GetComponent<NodeObject>().setTitle(cameraNode.windowTitle);
            newNode.GetComponent<NodeObject>().setAnimation("CameraAnimation");
            newNode.GetComponent<NodeObject>().setPosition(mousePos.x, mousePos.y);
            newNode.GetComponent<NodeObject>().SetCharID((int)_charSelect);
            

            cameraNode.SetID(cameraCounter);
            //cameraNode.SetAutoCreate(newNode.GetComponent<NodeObject>().ReturnCameraSetup().GetComponent<NodeObject>().ReturnAutoCam());

            newNode.transform.parent = _parent.transform;
            newNode.name = "CameraNode" + cameraCounter;
            newNode.tag = "CameraNode";
            cameraNodeID.Add(cameraCounter);
            cameraGO.Add(newNode);
            cameraCounter++;

        }


        else if(clb.Equals("makeTransition"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            if ((int)_charSelect < 4)
            {

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }
                if (clickedOnWindow)
                {
                    selectedNode = windows[selectIndex];
                    makeTransitionMode = true;
                }
            }
            else
            {
                for (int i = 0; i < cameraWindows.Count; i++)
                {
                    if (cameraWindows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }
                if (clickedOnWindow)
                {
                    selectedNode = cameraWindows[selectIndex];
                    makeTransitionMode = true;
                }
            }
        }

        else if(clb.Equals("deleteNode"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            if ((int)_charSelect < 4)
            {

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;

                        Debug.Log(windows[i]);

                        break;
                    }
                }

                if (clickedOnWindow)
                {


                    BaseNode selNode = windows[selectIndex];

                    foreach (BaseNode n in windows)
                    {
                        n.NodeDeleted(selNode);
                    }


                    for (int i = 0; i < allGO.Count; i++)
                    {

                        if (allGO[i] != null && allGO[i].GetComponent<NodeObject>().ReturnOutputID() == selectIndex)
                        {
                            allGO[i].GetComponent<NodeObject>().setOutputID(0);
                        }

                        if (allGO[i] != null && allGO[i].GetComponent<NodeObject>().ReturnNodeID() > selectIndex)
                        {
                            if (allGO[i].GetComponent<NodeObject>().ReturnOutputID() != 0)
                            {
                                allGO[i].GetComponent<NodeObject>().setOutputID(-1);
                            }
                            allGO[i].GetComponent<NodeObject>().SetName(-1);
                            allGO[i].GetComponent<NodeObject>().setNodeID(-1);

                            if (allGO[selectIndex].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                            {
                                allGO[i].GetComponent<NodeObject>().SetCameraName(allGO[i].GetComponent<NodeObject>().ReturnCamera());
                            }

                            windows[selectIndex].SetID(-1);
                        }

                    }



                    if (allGO[selectIndex].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                    {
                        DestroyImmediate(allGO[selectIndex].GetComponent<NodeObject>().ReturnCamera().gameObject);
                    }
                    DestroyImmediate(allGO[selectIndex].gameObject);
                    windows.RemoveAt(selectIndex);


                    Debug.Log("Removed windows " + selectIndex);

                    nodeID.RemoveAt(selectIndex);

                    allGO.RemoveAt(selectIndex);

                    nodeCounter--;

                }
            }
            else
            {

                for (int i = 0; i < cameraWindows.Count; i++)
                {
                    if (cameraWindows[i].windowRect.Contains(mousePos))
                    {

                        selectIndex = i;
                        clickedOnWindow = true;

                        Debug.Log(cameraWindows[i]);

                        break;
                    }
                }

                if (clickedOnWindow)
                {


                    BaseNode selNode = cameraWindows[selectIndex];

                    foreach (BaseNode n in cameraWindows)
                    {
                        n.NodeDeleted(selNode);
                    }


                    for (int i = 0; i < cameraGO.Count; i++)
                    {

                        if (cameraGO[i] != null && cameraGO[i].GetComponent<NodeObject>().ReturnOutputID() == selectIndex)
                        {
                            cameraGO[i].GetComponent<NodeObject>().setOutputID(0);
                        }

                        if (cameraGO[i] != null && cameraGO[i].GetComponent<NodeObject>().ReturnNodeID() > selectIndex)
                        {
                            if (cameraGO[i].GetComponent<NodeObject>().ReturnOutputID() != 0)
                            {
                                cameraGO[i].GetComponent<NodeObject>().setOutputID(-1);
                            }
                            cameraGO[i].GetComponent<NodeObject>().SetName(-1);
                            cameraGO[i].GetComponent<NodeObject>().setNodeID(-1);

                            if (cameraGO[selectIndex].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                            {
                                cameraGO[i].GetComponent<NodeObject>().SetCameraName(cameraGO[i].GetComponent<NodeObject>().ReturnCamera());
                            }

                            cameraWindows[selectIndex].SetID(-1);
                        }

                    }



                    if (cameraGO[selectIndex].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                    {
                        DestroyImmediate(cameraGO[selectIndex].GetComponent<NodeObject>().ReturnCamera().gameObject);
                    }
                    DestroyImmediate(cameraGO[selectIndex].gameObject);
                    cameraWindows.RemoveAt(selectIndex);


                    Debug.Log("Removed windows " + selectIndex);

                    cameraNodeID.RemoveAt(selectIndex);

                    cameraGO.RemoveAt(selectIndex);

                    cameraCounter--;


                }
            }
        }
    }
 
    public static void DrawNodeCurve(Rect start, Rect end)
    {

        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);

        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, .06f);


        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        
        

    }

   
}
#endif