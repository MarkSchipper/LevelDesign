using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace DialogueSystem
{

    public class NodeEditor : EditorWindow
    {

        [SerializeField]
        private List<BaseNode> windows = new List<BaseNode>();

        [SerializeField]
        private List<BaseKnob> windowInputKnob = new List<BaseKnob>();

        [SerializeField]
        private List<BaseKnob> windowOutputKnob = new List<BaseKnob>();

        [SerializeField]
        private List<BaseKnob> windowYesKnob = new List<BaseKnob>();

        [SerializeField]
        private List<BaseKnob> windowNoKnob = new List<BaseKnob>();

        [SerializeField]
        private List<int> nodeID = new List<int>();

        // the nodeCounter to make sure every node has a unique ID
        private int nodeCounter;

        // Store the mousePosition ( current event e.mousePosition
        private Vector2 mousePos;

        // The selected node
        private BaseKnob selectedNode;
        private BaseKnob selectedOutputNode;
        private BaseKnob selectedInputNode;

        // Are we in transitionMode
        private bool makeTransitionMode = false;

        private int _previousNode;

        public GUISkin _skin;


        // Are we loading the canvas
        private bool _isLoading = false;


        // Floats to change the size of our node windows
        private float _windowWidth = 250f;
        private float _windowHeight = 120f;

        // Vars to create the dirty canvas moving
        private Vector2 _initialPos;
        private bool _setPosition;

        private GameObject[] _allNPCs;
        private List<string> _allNames = new List<string>();
        private int _selectIndex = 0;
        private int _npcID;
        private int _oldNpcID;

        private Texture2D _knob;
        private Texture2D _yesKnob;
        private Texture2D _noKnob;

        private Texture2D _inputKnob;
        private Texture2D _outputKnob;

        private bool _loadedNodes;
        private bool _addedData;
        private int _loadCounter = 0;
        private bool _redrawCurves;
        private bool _canvasChange = false;

        private static NodeEditor editor;

        [MenuItem("Level Design/Dialogue Tree Editor")]

        static void ShowEditor()
        {

            // Creating a new window
            editor = EditorWindow.GetWindow<NodeEditor>();
            editor.Init();

        }

        void Init()
        {
            _knob = Resources.Load("DialogueTree/Knob") as Texture2D;
            _yesKnob = Resources.Load("DialogueTree/YesKnob") as Texture2D;
            _noKnob = Resources.Load("DialogueTree/NoKnob") as Texture2D;
            _inputKnob = Resources.Load("DialogueTree/InputKnob") as Texture2D;
            _outputKnob = Resources.Load("DialogueTree/OutputKnob") as Texture2D;

            GetAllNPCs();
            
        }

        void Update()
        {

            Repaint();


        }

        void OnGUI()
        {

            GUI.skin = _skin;
            //_skin.GetStyle("background");

            Event e = Event.current;
            mousePos = e.mousePosition;

            EditorGUILayout.BeginHorizontal();

            _selectIndex = EditorGUILayout.Popup(_selectIndex, _allNames.ToArray(), GUILayout.Width(150));
            
            if(GUI.changed)
            {
                ClearWindows();
            }

            if (_allNPCs.Count() > 0)
            {
                _npcID = _allNPCs[_selectIndex].GetComponent<NPC.NpcSystem>().ReturnID();
            }

            
            if(GUILayout.Button("LOAD"))
            {
                DialogueDatabase.ClearAll();
                DialogueDatabase.GetDialogueByNPC(_npcID);
                ClearWindows();
                if (DialogueDatabase.ReturnCount() > 0)
                {
                    LoadNodes();
                    _loadedNodes = true;
                }
            }

            if (GUILayout.Button("CLEAR"))
            {
                ClearWindows();
            }

            if (GUILayout.Button("SAVE"))
            {
                if (!_loadedNodes)
                {
                    Debug.Log(windows.Count);
                    for (int i = 0; i < windows.Count; i++)
                    {
                       DialogueSystem.DialogueDatabase.AddDialogue(windows[i].ReturnConversationID(), i, _npcID, windows[i].ReturnPreviousNode(), windows[i].ReturnQuestion(), windows[i].ReturnResponse(), windows[i].ReturnCorrectAnswer(), windows[i].ReturnTitle(), windows[i].windowRect.position, windows[i].ReturnQuestID());
                    }
                    _loadedNodes = true;
                }
                if(_loadedNodes)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        DialogueSystem.DialogueDatabase.UpdateDialogue(windows[i].ReturnConversationID(), i, _npcID, windows[i].ReturnPreviousNode(), windows[i].ReturnQuestion(), windows[i].ReturnResponse(), windows[i].ReturnCorrectAnswer(), windows[i].ReturnTitle(), windows[i].windowRect.position, windows[i].ReturnQuestID());
                    }
                    DialogueDatabase.ResetDeletedDialogue();
                }
                
            }

            EditorGUILayout.EndHorizontal();

            #region mouse

            // If Right button and we are not in transitionMode ( drawing a curve )
            #region Menu
            if (e.button == 1 && !makeTransitionMode)
            {
                // Have we clicked
                if (e.type == EventType.mouseDown)
                {
                    bool clickedOnWindow = false;
                    int selectIndex = -1;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i].windowRect.Contains(mousePos))
                        {
                            selectIndex = i;
                            clickedOnWindow = true;
                            break;
                        }

                    }

                    // If we have not clicked on a window but still right clicked we want to Menu to appear to add new nodes

                    if (!clickedOnWindow)
                    {
                        GenericMenu menu = new GenericMenu();

                        // ANIMATION MENUS
                        menu.AddDisabledItem(new GUIContent("[ Dialogue ]"));
                        menu.AddItem(new GUIContent("Dialogue/Add Dialogue Start Node"), false, ContextCallback, "dialogueStartNode");
                        menu.AddItem(new GUIContent("Dialogue/Add Conversation Node"), false, ContextCallback, "conversationNode");
                        menu.AddItem(new GUIContent("Dialogue/Response Node"), false, ContextCallback, "responseNode");
                        menu.AddItem(new GUIContent("Dialogue/Quest Node"), false, ContextCallback, "questNode");
                        menu.AddItem(new GUIContent("Dialogue/End Node"), false, ContextCallback, "endNode");
                        _addedData = false;

                        menu.ShowAsContext();
                        e.Use();

                    }

                    else
                    {
                        // If we have clicked on a itself show the different menu
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                        menu.ShowAsContext();
                        e.Use();

                    }
                }
            }
            #endregion
            // If we have clicked on makeTransition 

            else if (e.button == 0 && e.type == EventType.mouseDown && makeTransitionMode)
            {

                bool clickedOnWindow = false;
                int selectIndex = -1;

                if (!clickedOnWindow)
                {
                    for (int i = 0; i < windowInputKnob.Count; i++)
                    {
                        // Get the current window we have selected ( FROM: Node )
                        if (windowInputKnob[i].windowRect.Contains(mousePos))
                        {

                            selectIndex = i;
                            clickedOnWindow = true;
                            selectedInputNode = windowInputKnob[i];
                            break;
                        }
                    }
                }


                if (clickedOnWindow)
                {
                    if (selectedOutputNode.GetType().ToString() == "OutputKnob")
                    {
                        windowInputKnob[selectIndex].SetInput((InputKnob)selectedInputNode, (OutputKnob)selectedOutputNode, mousePos);
                        windows[selectIndex].SetConversationID(windows[_previousNode].ReturnConversationID());
                        windows[selectIndex].SetPreviousNode(_previousNode);
                        windows[selectIndex].SetNpcID(windows[_previousNode].ReturnNpcID());

                    }

                    if (selectedOutputNode.GetType().ToString() == "NoOutputKnob")
                    {
                        windowInputKnob[selectIndex].SetNoInput((InputKnob)selectedInputNode, (NoOutputKnob)selectedOutputNode, mousePos);
                        windows[selectIndex].SetConversationID(windows[_previousNode].ReturnConversationID());
                        windows[selectIndex].SetAnswer(false);
                        windows[selectIndex].SetPreviousNode(_previousNode);
                        windows[selectIndex].SetNpcID(windows[_previousNode].ReturnNpcID());
                    }

                    if (selectedOutputNode.GetType().ToString() == "YesOutputKnob")
                    {
                        windowInputKnob[selectIndex].SetYesInput((InputKnob)selectedInputNode, (YesOutputKnob)selectedOutputNode, mousePos);
                        windows[selectIndex].SetConversationID(windows[_previousNode].ReturnConversationID());
                        windows[selectIndex].SetAnswer(true);
                        windows[selectIndex].SetPreviousNode(_previousNode);
                        windows[selectIndex].SetNpcID(windows[_previousNode].ReturnNpcID());
                    }

                    makeTransitionMode = false;
                    selectedInputNode = null;
                }

                if (!clickedOnWindow)
                {
                    makeTransitionMode = false;
                    selectedInputNode = null;
                }

                e.Use();


            }

            // If we have clicked on a window but we are NOT in transitionMode
            // We set the node we have clicked to nodeToChange
            else if (e.button == 0 && e.type == EventType.mouseDown && !makeTransitionMode)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;



                for (int i = 0; i < windowOutputKnob.Count; i++)
                {
                    if (windowOutputKnob[i].windowRect.Contains(mousePos))
                    {
                        selectedOutputNode = windowOutputKnob[i];
                        windowOutputKnob[i].SetOutput((OutputKnob)selectedOutputNode, mousePos);
                        selectIndex = i;
                        clickedOnWindow = true;
                        _previousNode = selectIndex;
                        break;
                    }
                }

                for (int i = 0; i < windowNoKnob.Count; i++)
                {
                    if (windowNoKnob[i].windowRect.Contains(mousePos))
                    {
                        selectedOutputNode = windowNoKnob[i];
                        windowNoKnob[i].SetNoOutput((NoOutputKnob)selectedOutputNode, mousePos);
                        selectIndex = i;
                        clickedOnWindow = true;
                        _previousNode = selectIndex;
                    }

                }

                for (int i = 0; i < windowYesKnob.Count; i++)
                {
                    if (windowYesKnob[i].windowRect.Contains(mousePos))
                    {
                        selectedOutputNode = windowYesKnob[i];
                        windowYesKnob[i].SetYesOutput((YesOutputKnob)selectedOutputNode, mousePos);
                        selectIndex = i;
                        clickedOnWindow = true;
                        _previousNode = selectIndex;

                    }
                }

                if (clickedOnWindow)
                {
                    OutputKnob nodeToChange = windowOutputKnob[selectIndex].ClickedOnOutput(mousePos);
                    NoOutputKnob noToChange = windowNoKnob[selectIndex].ClickedOnNoOutput(mousePos);
                    YesOutputKnob yesToChange = windowYesKnob[selectIndex].ClickedOnYesOutput(mousePos);

                    if (nodeToChange != null)
                    {
                        selectedOutputNode = nodeToChange;
                        makeTransitionMode = true;

                    }
                    if (noToChange != null)
                    {
                        selectedOutputNode = noToChange;
                        makeTransitionMode = true;
                    }

                    if (yesToChange != null)
                    {
                        selectedOutputNode = yesToChange;
                        makeTransitionMode = true;
                    }
                }

            }
            #endregion
            
         
            
            // If we are in transitionMode and clicked on a node 
            if (makeTransitionMode && selectedOutputNode != null)
            {

                // Draw the Bezier Curve
                Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
                DrawNodeCurve(selectedOutputNode.windowRect, mouseRect);

                Repaint();
            }
            
            foreach (BaseNode n in windows)
            {
                n.DrawCurves();
            }

            foreach (BaseKnob n in windowInputKnob)
            {

                n.DrawCurves();
            }
            

            BeginWindows();

            if (windows.Count > 0)
            {
                for (int i = 0; i < windows.Count; i++)
                {
                    windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);

                    if (windows[i].ReturnHasInputs())
                    {
                        windowInputKnob[i].DrawWindow(windows[i].windowRect, _inputKnob);
                    }
                    if (windows[i].ReturnHasOutputs())
                    {
                        windowOutputKnob[i].DrawWindow(windows[i].windowRect, _outputKnob);
                    }

                    windowYesKnob[i].DrawWindow(windows[i].windowRect, _yesKnob);
                    windowNoKnob[i].DrawWindow(windows[i].windowRect, _noKnob);

                }
            }
            #region mousedrag
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


            if (e.button == 2 && e.type == EventType.MouseDrag)
            {
                if (_initialPos != e.mousePosition)
                {

                    if (!_setPosition)
                    {
                        _initialPos = e.mousePosition;
                        _setPosition = true;
                    }

                    Vector2 _offSet = _initialPos - e.mousePosition;



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
            }
            #endregion


            EndWindows();



        }


        //////////////////////////////////////////////////////////////////////////////
        //                          DrawNodeWindows(int id)                         //
        //                                                                          //
        //  Draws the the nodes as windows                                          //
        //      If the window.count is higher than 0 ( are there windows to draw )  //
        //          If the windows is not null                                      //
        //              If the window with the given id exists                      //
        //                  Call the DrawWindow()                                   //
        //                  Set it draggable                                        //
        //                                                                          //                     
        //////////////////////////////////////////////////////////////////////////////

        void DrawNodeWindow(int id)
        {
            if (windows.Count > 0)
            {
                if (windows != null)
                {
                    if (windows[id] != null)
                    {
                        windows[id].DrawWindow();

                        // set it dragable
                        GUI.DragWindow();
                    }
                }
            }
        }

        void DrawInputWindow(int id)
        {
            windowInputKnob[id].DrawWindow(windows[id].windowRect, _knob);
            GUI.DragWindow();
        }

        //////////////////////////////////////////////////////////////////////////////
        //                                  ContextCallBack()                       //
        //                                                                          //
        // Called by the OnGUI() to create a new Menu item                          //
        //  If clb ( CallBack ) equals string                                       //
        //      Create a new Node and there for a new instance                      //
        //      Add an Input knob and Output knob                                   //
        //          Empty Constructor means no input/output                         //
        //      Create a new Rect for the window and input/output                   //
        //                                                                          //
        //      Add everything to its corresponding List                            //
        //                                                                          //
        //////////////////////////////////////////////////////////////////////////////


        void ContextCallback(object obj)
        {
            string clb = obj.ToString();

            // If the callback is....        
            if (clb.Equals("dialogueStartNode"))
            {

                // Create a new instance of a class and create a new rect
                DialogueStartNode dialogueStart = new DialogueStartNode();
                InputKnob input = new InputKnob();
                OutputKnob output;

                if (_addedData)
                {
                    dialogueStart.windowRect = new Rect(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y, _windowWidth, _windowHeight);
                    output = new OutputKnob(nodeCounter, DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y + _windowHeight, 20, 20);
                }
                else
                {
                    dialogueStart.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);
                    output = new OutputKnob(nodeCounter, mousePos.x + (_windowWidth / 2), mousePos.y + _windowHeight, 20, 20);
                }
                YesOutputKnob yesOutput = new YesOutputKnob();
                NoOutputKnob noOutput = new NoOutputKnob();

                // Add it to the windows list
                windows.Add(dialogueStart);
                windowOutputKnob.Add(output);
                windowInputKnob.Add(input);
                windowYesKnob.Add(yesOutput);
                windowNoKnob.Add(noOutput);

                dialogueStart.SetID(nodeCounter);

                nodeID.Add(nodeCounter);
                nodeCounter++;

                if(_addedData)
                {
                    dialogueStart.SetConversationID(DialogueDatabase.ReturnConversationID(_loadCounter));
                    dialogueStart.SetNpcID(_npcID);
                    dialogueStart.SetPreviousNode(0);
                    dialogueStart.SetTitle(DialogueDatabase.ReturnTitle(_loadCounter));
                    _addedData = false;
                }

            }

            else if (clb.Equals("conversationNode"))
            {
                ConversationNode convers = new ConversationNode();
                InputKnob input;
                OutputKnob output = new OutputKnob();
                YesOutputKnob yesOutput;
                NoOutputKnob noOutput;

                if (_addedData)
                {
                    convers.windowRect = new Rect(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y, _windowWidth, _windowHeight);
                    input = new InputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y - 20, 20, 20);
                    yesOutput = new YesOutputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y + (_windowHeight / 2), 20, 20);
                    noOutput = new NoOutputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y + (_windowHeight / 2), 20, 20);
                }
                else
                {
                    convers.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);
                    input = new InputKnob(mousePos.x + (_windowWidth / 2), mousePos.y - 20, 20, 20);
                    yesOutput = new YesOutputKnob(mousePos.x, mousePos.y + (_windowHeight / 2), 20, 20);
                    noOutput = new NoOutputKnob(mousePos.x, mousePos.y + (_windowHeight / 2), 20, 20);
                }

                

                windows.Add(convers);
                windowInputKnob.Add(input);
                windowOutputKnob.Add(output);
                windowYesKnob.Add(yesOutput);
                windowNoKnob.Add(noOutput);

                convers.SetID(nodeCounter);

                nodeID.Add(nodeCounter);
                nodeCounter++;

                if(_addedData)
                {
                    convers.SetConversationID(DialogueDatabase.ReturnConversationID(_loadCounter));
                    convers.SetNpcID(_npcID);
                    convers.SetPreviousNode(DialogueDatabase.ReturnPreviousNode(_loadCounter));
                    convers.SetQuestion(DialogueDatabase.ReturnQuestion(_loadCounter));
                    _addedData = false;
                }


            }
            else if (clb.Equals("responseNode"))
            {
                ResponseNode response = new ResponseNode();

                InputKnob input;
                OutputKnob output;
                
                if(_addedData)
                {
                    response.windowRect = new Rect(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y, _windowWidth, _windowHeight);
                    input = new InputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y - 20, 20, 20);
                    output = new OutputKnob(nodeCounter, DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y + _windowHeight, 20, 20);
                }
                else
                {
                    response.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight);
                    input = new InputKnob(mousePos.x + (_windowWidth / 2), mousePos.y - 20, 20, 20);
                    output = new OutputKnob(nodeCounter, mousePos.x + (_windowWidth / 2), mousePos.y + _windowHeight, 20, 20);
                }

                YesOutputKnob yesOutput = new YesOutputKnob();
                NoOutputKnob noOutput = new NoOutputKnob();

                windows.Add(response);
                windowInputKnob.Add(input);
                windowOutputKnob.Add(output);
                windowYesKnob.Add(yesOutput);
                windowNoKnob.Add(noOutput);

                response.SetID(nodeCounter);
                nodeID.Add(nodeCounter);
                nodeCounter++;

                if(_addedData)
                {
                    response.SetConversationID(DialogueDatabase.ReturnConversationID(_loadCounter));
                    response.SetNpcID(_npcID);
                    response.SetPreviousNode(DialogueDatabase.ReturnPreviousNode(_loadCounter));
                    response.SetResponse(DialogueDatabase.ReturnResponse(_loadCounter));
                    response.SetAnswer(DialogueDatabase.ReturnType(_loadCounter));
                    _addedData = false;
                }
            }

            else if(clb.Equals("questNode"))
            {
                QuestNode questNode = new QuestNode(_npcID);

                InputKnob input;
                OutputKnob output;

                YesOutputKnob yesOutput = new YesOutputKnob();
                NoOutputKnob noOutput = new NoOutputKnob();

                if(_addedData)
                {
                    questNode.windowRect = new Rect(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y, _windowWidth, _windowHeight * 3);
                    input = new InputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y - 20, 20, 20);
                    output = new OutputKnob(nodeCounter, DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y + _windowHeight, 20, 20);
                }
                else
                {
                    questNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, _windowHeight * 3);
                    input = new InputKnob(mousePos.x + (_windowWidth / 2), mousePos.y - 20, 20, 20);
                    output = new OutputKnob(nodeCounter, mousePos.x + (_windowWidth / 2), mousePos.y + _windowHeight, 20, 20);
                }

                windows.Add(questNode);
                windowInputKnob.Add(input);
                windowOutputKnob.Add(output);
                windowYesKnob.Add(yesOutput);
                windowNoKnob.Add(noOutput);
                questNode.SetID(nodeCounter);
                nodeID.Add(nodeCounter);
                nodeCounter++;
            }

            else if(clb.Equals("endNode"))
            {
                EndNode endNode = new EndNode();

                InputKnob input;
                OutputKnob output;

                YesOutputKnob yesOutput = new YesOutputKnob();
                NoOutputKnob noOutput = new NoOutputKnob();

                if (_addedData)
                {
                    endNode.windowRect = new Rect(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x, DialogueDatabase.ReturnCanvasPosition(_loadCounter).y, _windowWidth, 50);
                    input = new InputKnob(DialogueDatabase.ReturnCanvasPosition(_loadCounter).x + (_windowWidth / 2), DialogueDatabase.ReturnCanvasPosition(_loadCounter).y - 20, 20, 20);
                    output = new OutputKnob();
                }
                else
                {
                    endNode.windowRect = new Rect(mousePos.x, mousePos.y, _windowWidth, 50);
                    input = new InputKnob(mousePos.x + (_windowWidth / 2), mousePos.y - 20, 20, 20);
                    output = new OutputKnob();
                }

                windows.Add(endNode);
                windowInputKnob.Add(input);
                windowOutputKnob.Add(output);
                windowYesKnob.Add(yesOutput);
                windowNoKnob.Add(noOutput);
                endNode.SetID(nodeCounter);
                
                nodeID.Add(nodeCounter);
                nodeCounter++;
            }

            else if (clb.Equals("deleteNode"))
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

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

                    BaseNode selNode = windows[selectIndex];

                    foreach (BaseNode n in windows)
                    {
                        n.NodeDeleted(selNode);
                    }

                    windows.RemoveAt(selectIndex);
                    windowInputKnob.RemoveAt(selectIndex);
                    windowOutputKnob.RemoveAt(selectIndex);
                    windowYesKnob.RemoveAt(selectIndex);
                    windowNoKnob.RemoveAt(selectIndex);
                    nodeID.RemoveAt(selectIndex);
                    nodeCounter--;
                    Repaint();
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

        void GetAllNPCs()
        {
            _allNPCs = GameObject.FindGameObjectsWithTag("NPC");

            for (int i = 0; i < _allNPCs.Length; i++)
            {
                _allNames.Add(_allNPCs[i].transform.parent.name);
            }
        }

        void LoadNodes()
        {
            Debug.Log(DialogueDatabase.ReturnCount());
            for (int i = 0; i < DialogueDatabase.ReturnCount(); i++)
            {
                Debug.Log(i);
                if (DialogueSystem.DialogueDatabase.ReturnTitle(i) != "" && DialogueSystem.DialogueDatabase.ReturnTitle(i) != "End")
                {
                    _addedData = true;
                    _loadCounter = i;
                    ContextCallback("dialogueStartNode");
                }
                if(DialogueDatabase.ReturnTitle(i) == "End")
                {
                    _addedData = true;
                    _loadCounter = i;
                    ContextCallback("endNode");
                }
                if(DialogueSystem.DialogueDatabase.ReturnQuestion(i) != "")
                {
                    _addedData = true;
                    _loadCounter = i;
                    ContextCallback("conversationNode");
                }
                
                if(DialogueSystem.DialogueDatabase.ReturnResponse(i) != "")
                {
                    _addedData = true;
                    _loadCounter = i;
                    ContextCallback("responseNode");

                }

                
            }
            RedrawInputs();
        }

        void RedrawInputs()
        {

            for (int i = 0; i < DialogueSystem.DialogueDatabase.ReturnCount(); i++)
            {
                if(DialogueDatabase.ReturnTitle(i) != "" && DialogueDatabase.ReturnTitle(i) != "End")
                {

                }

                if(DialogueDatabase.ReturnTitle(i) == "End")
                {
                    selectedInputNode = windowInputKnob[i];
                    selectedOutputNode = windowOutputKnob[DialogueDatabase.ReturnPreviousNode(i)];

                    windowInputKnob[i].SetInput((InputKnob)selectedInputNode, (OutputKnob)selectedOutputNode, mousePos);

                    selectedInputNode = null;
                    selectedOutputNode = null;
                }

                if (DialogueDatabase.ReturnQuestion(i) != "")
                {
                    selectedInputNode = windowInputKnob[i];
                    
                    selectedOutputNode = windowOutputKnob[DialogueDatabase.ReturnPreviousNode(i)];
                    windowInputKnob[i].SetInput((InputKnob)selectedInputNode, (OutputKnob)selectedOutputNode, mousePos);

                    selectedInputNode = null;
                    selectedOutputNode = null;

                    
                }

                if(DialogueDatabase.ReturnResponse(i) != "")
                {
                    if(DialogueDatabase.ReturnType(i))
                    {
                        selectedInputNode = windowInputKnob[i];
                        selectedOutputNode = windowYesKnob[DialogueDatabase.ReturnPreviousNode(i)];
                        windowInputKnob[i].SetYesInput((InputKnob)selectedInputNode, (YesOutputKnob)selectedOutputNode, mousePos);
                    }
                    if(!DialogueDatabase.ReturnType(i))
                    {
                        selectedInputNode = windowInputKnob[i];
                        selectedOutputNode = windowNoKnob[DialogueDatabase.ReturnPreviousNode(i)];
                        windowInputKnob[i].SetNoInput((InputKnob)selectedInputNode, (NoOutputKnob)selectedOutputNode, mousePos);
                    }
                }
                /*
                    windowInputKnob[i].SetInput((InputKnob)selectedInputNode, (OutputKnob)selectedOutputNode, mousePos);
                    windows[i].SetConversationID(windows[_previousNode].ReturnConversationID());
                    windows[i].SetPreviousNode(_previousNode);
                    windows[i].SetNpcID(windows[_previousNode].ReturnNpcID());
                */
            }
            

        }

        void ClearWindows()
        {
            if (windows.Count > 0)
            {
                windows.Clear();
                windowInputKnob.Clear();
                windowOutputKnob.Clear();
                windowYesKnob.Clear();
                windowNoKnob.Clear();
                nodeID.Clear();
                nodeCounter = 0;
            }
        }

    }
}
