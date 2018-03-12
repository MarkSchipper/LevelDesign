using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using System.Data;
using System;

namespace Dialogue
{

    public class DialogueManager : MonoBehaviour
    {


        private Rect _questWindow;
        private Rect _questTitleRect;
        private Rect _questTextRect;
        private Rect _questAcceptRect;

        private Rect _dialogueBox;
        private Rect _dialogueTextBox;
        private Rect[] _dialogueAnswersBox = new Rect[2];
        private Rect[] _dialogueAnswersRect = new Rect[2];
        private static string[] _dialogueAnswers = new string[2];

        public GameObject _dialogueWheel;
        public Image _dialogueAnswerLeft;
        public Image _dialogueAnswerRight;

        public GUISkin _skin;
        private GUISkin _dialogueSkin;

        private static bool _showQuestWindow = false;
        private static bool _showDialogue = false;
        private static bool _showButton = false;
        private static bool _draggingWindow = false;

        private Vector2 _windowPosition = new Vector2(100, 100);
        private Vector2 _windowSize = new Vector2(400, 400);


        private static string _questTitle;
        private static string _questText;

        private static int _npcID;
        private static int _questID;
        private static bool _questCompleted;

        private GUIStyle _styleLeft;
        private GUIStyle _styleRight;

        private static bool _showZone = false;
        private static string _zoneName;
        private static string _zoneDesc;

        private static bool _showMessage = false;
        private static string _message;

        private static bool _showHint = false;
        private static string _hintMessage;

        private static GameObject _selectedNPC;

        private List<int> _questRewards = new List<int>();
        private List<string> _questAnswers = new List<string>();
        private List<string> _questCompleteAnswers = new List<string>();
        private int _questCounter;

        private static bool _playerFinishedQuest;

        private static int _nodeID = 0;

        public static DialogueManager instance;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
           // DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
           
            _dialogueSkin = Resources.Load("Skins/Dialogue") as GUISkin;
            _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeft");
            _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRight");
        }


        // Use this for initialization
        void Start()
        {

            if (PlayerPrefs.GetFloat("QuestWindowPosX") > 0)
            {
                _windowPosition = new Vector2(PlayerPrefs.GetFloat("QuestWindowPosX"), PlayerPrefs.GetFloat("QuestWindowPosY"));
            }
            ShowDialogueWheel(false);

            _dialogueAnswerLeft.enabled = false;
            _dialogueAnswerRight.enabled = false;

            // Add quest answers
            _questAnswers.Add("Accept");
            _questAnswers.Add("Decline");

            // Add Quest complete answers
            _questCompleteAnswers.Add("Finish");
            _questCompleteAnswers.Add("Quit");
        }

        // Update is called once per frame
        void Update()
        {

        }


        void OnGUI()
        {

            if (_showDialogue)
            {

                ShowDialogue();
            }

            if (_showQuestWindow)
            {
                ShowQuestWindow();
            }

            if (_draggingWindow)
            {
                _windowPosition.x = Event.current.mousePosition.x;
                _windowPosition.y = Event.current.mousePosition.y;

                PlayerPrefs.SetFloat("QuestWindowPosX", _windowPosition.x);
                PlayerPrefs.SetFloat("QuestWindowPosY", _windowPosition.y);
            }
            if (_showZone)
            {
                ShowZone();
            }

            if (_showMessage)
            {
                Rect _zoneRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100);
                GUI.Label(_zoneRect, _message, _skin.GetStyle("Message"));

                InvokeRepeating("CancelShowMessage", 2, 100);
            }
            if (_showHint)
            {
                Rect _hintRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 350, 300, 100);
                GUI.Label(_hintRect, _hintMessage, _skin.GetStyle("Hint"));
                InvokeRepeating("CancelShowHint", 4, 100);
            }
        }

        public void SetDialogue(string _title, string _text, bool _buttonVis, int _nwNpcID, int _nwQuestID, GameObject _npc)
        {

            _questTitle = _title;
            _questText = _text;
            _showButton = _buttonVis;
            _npcID = _nwNpcID;
            _questID = _nwQuestID;
            _showDialogue = true;
            _selectedNPC = _npc;
            _questCompleted = Quest.QuestGameManager.ReturnQuestComplete(_questID);


            CombatSystem.PlayerController.instance.SetInputBlock(true);
        }

        public void SetAnswers(List<string> _answers)
        {
            _dialogueAnswers[0] = _answers[0];
            _dialogueAnswers[1] = _answers[1];
        }

        public void InitiateDialogue(int _id, bool _questGiver, GameObject _npcObject)
        {
            
            if (Dialogue.Game.DialogueGameDatabase.GetInitialQuestionFromNPC(_id) != string.Empty)
            {
                SetDialogue("", _npcObject.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " +  Dialogue.DialogueDatabase.GetInitialQuestionFromNPC(_id), false, _id, 0, _npcObject);

                Quest.QuestGameManager.GetQuestsFromNpc(_npcID);

                // Set NodeID to the next NodeID in line
                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_id, 0);
                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_id, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_id, 0)));

                _selectedNPC.GetComponent<NPC.NpcSystem>().SwitchOnNpcCamera(true);
                CombatSystem.PlayerController.instance.SetInputBlock(true);

                if (CombatSystem.CameraController.ReturnFirstPerson())
                {
                    if (!Cursor.visible)
                    {
                        Cursor.visible = true;
                    }
                }

            }
        }

        void ShowDialogue()
        {
            if (CombatSystem.CameraController.ReturnFirstPerson())
            {
                if (!Cursor.visible)
                {
                    Cursor.visible = true;
                }
            }

            if (!CombatSystem.CameraController.ReturnFirstPerson())
            {
                CombatSystem.InteractionManager.instance.SetNormalCursor();
            }

            ShowDialogueWheel(true);

            //_dialogueBox = new Rect(Screen.width / 2, Screen.height - 400, 500, 100);
            _dialogueTextBox = new Rect(Screen.width / 2 - 225, Screen.height - 450, 450, 100);

            //GUI.Box(_dialogueBox, "");
            GUI.Label(_dialogueTextBox, _questText, _dialogueSkin.GetStyle("DialogueQuestion"));

            _dialogueAnswersBox[0] = new Rect(Screen.width / 2 + 100, Screen.height - 350, 200, 50);
            _dialogueAnswersBox[1] = new Rect(Screen.width / 2 - 300, Screen.height - 350, 200, 50);

            _dialogueAnswersRect[0] = new Rect(Screen.width / 2 + 100, Screen.height - 350, 200, 50);
            _dialogueAnswersRect[1] = new Rect(Screen.width / 2 - 300, Screen.height - 350, 200, 50);

            GUI.Label(_dialogueAnswersRect[0], _dialogueAnswers[0], _styleRight);

            GUI.Label(_dialogueAnswersRect[1], _dialogueAnswers[1], _styleLeft);

            if(_dialogueAnswersBox[0].Contains(Event.current.mousePosition))
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRightHover");
                _dialogueAnswerRight.enabled = true;
                
                if(Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    CreateDialogue(0);
                }
            }

            else if (_dialogueAnswersBox[1].Contains(Event.current.mousePosition))
            {
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeftHover");
                _dialogueAnswerLeft.enabled = true;

                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    CreateDialogue(1);
                }
            }
            else
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRight");
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeft");
                _dialogueAnswerLeft.enabled = false;
                _dialogueAnswerRight.enabled = false;
            }

            #region OLD
            /*
            // If the cursor is hovering over the RIGHT answer
            if (_dialogueAnswersBox[0].Contains(Event.current.mousePosition))
            {
                // we use [0] for the RIGHT answer
                // Change the style to create the hover effect
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeftHover");
                _dialogueAnswerRight.enabled = true;

                // If we have pressed the left mouse button
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    #region OLD
                    // OMBOUWEN NAAR QUEST KRIJGEN VAN QUEST en NPC 
                    #region QuestAnswers 
                    if (_dialogueAnswers[0] == "Accept" || _dialogueAnswers[0] == "Finish")
                    {
                        for (int i = 0; i < Quest.QuestGameManager.ReturnNpcQuestID().Count; i++)
                        {
                            if (Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i] != -1)
                            {
                                if (!Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]))
                                {
                                    Debug.Log(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                    Quest.QuestGameManager.AcceptQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                    CancelDialogue();
                                    Debug.Log("Accepted quest");
                                }
                                else
                                {
                                    Quest.QuestGameManager.FinishQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                    _questCounter++;
                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);

                                    if(Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID) != string.Empty)
                                    {
                                        _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                                        SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                                        _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                                    }
                                    else if(Dialogue.Game.DialogueGameDatabase.GetNextQuestID(_npcID, _nodeID) != -1)
                                    {
                                        Debug.Log("QuestID " + Dialogue.Game.DialogueGameDatabase.GetNextQuestID(_npcID, _nodeID));
                                        _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestTitleByID(Dialogue.Game.DialogueGameDatabase.GetNextQuestID(_npcID, _nodeID));
                                        SetAnswers(_questAnswers);
                                    }
                                    //QuestID never gets refreshed!

                                }
                                //CancelDialogue();
                            }
                        }
                    }
                    else if (_dialogueAnswers[0] == "Decline" || _dialogueAnswers[0] == "Quit")
                    {
                        CancelDialogue();
                    }
                   
                    #endregion

                    else if (Dialogue.Game.DialogueGameDatabase.GetCurrentTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) == "Add Quest" || Dialogue.Game.DialogueGameDatabase.GetCurrentTitle(_npcID, _nodeID) == "Add Quest")
                    {
                        Debug.Log("There is a quest");
                        if (Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[_questCounter]))
                        {
                            _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestCompletedTextByID(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[0]);
                            SetAnswers(_questCompleteAnswers);
                            Debug.Log("Quest is complete");
                        }
                        else
                        {
                            
                            Debug.Log("Return QuestID " + Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[_questCounter]);
                            if (!Quest.QuestGameManager.ReturnQuestActive(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[_questCounter]))
                            {
                                Debug.Log("Set questtext");
                                _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestTitleByID(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[0]);
                                SetAnswers(_questAnswers);
                                Debug.Log(_questText);
                            }
                            else
                            {
                                
                            }
                        }
                    }
                    else
                    {
                        // if question
                        if (Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != string.Empty && Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) == string.Empty)
                        {
                            if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != "End")
                            {
                                if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) == "Add Quest")
                                {
                                    Debug.Log("QUESTSFDF");
                                }
                                else
                                {
                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                                    _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                                }

                            }
                        }
                        // if condition
                        else
                        {
                            if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != "End" && Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != "Add Quest")
                            {
                                // If the title is NOT "End" it is a condition
                                switch (Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]))
                                {
                                    case "If":

                                        switch (Dialogue.Game.DialogueGameDatabase.GetCurrentConditionStatement(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]))
                                        {
                                            case "QuestActive":
                                                Dialogue.DialogueQuestActive.QuestActive(_npcID, _nodeID, _selectedNPC, 0);

                                                break;
                                            case "QuestCompleted":
                                                Dialogue.DialogueQuestComplete.QuestComplete(_npcID, _nodeID, _selectedNPC, 0);

                                                break;
                                            case "PlayerLevel":
                                                Dialogue.DialoguePlayerLevel.PlayerLevel(_npcID, _nodeID, _selectedNPC, 0);

                                                break;
                                            case "ZoneVisited":
                                                Dialogue.DialogueZoneVisited.ZoneVisited(_npcID, _nodeID, _selectedNPC, 0);

                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "IfNot":
                                        break;
                                    default:
                                        break;
                                }
                            }

                            // The title equals "End"
                            else
                            {
                                Debug.Log(Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]));
                        
                                _showDialogue = false;
                                ShowDialogueWheel(false);
                                _selectedNPC.GetComponent<NPC.NpcSystem>().SwitchOnNpcCamera(false);
                                CombatSystem.PlayerController.instance.SetInputBlock(false);

                                if (CombatSystem.CameraController.ReturnFirstPerson())
                                {
                                    if (Cursor.visible)
                                    {
                                        Cursor.visible = false;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            // If we are not hovering over the RIGHT answer
            else
            {
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeft");
                _dialogueAnswerRight.enabled = false;
            }

            if (_dialogueAnswersBox[1].Contains(Event.current.mousePosition))
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRightHover");
                _dialogueAnswerLeft.enabled = true;

                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    #region OLD
                    #region QuestAnswers
                    if (_dialogueAnswers[1] == "Accept" || _dialogueAnswers[1] == "Finish")
                    {
                        for (int i = 0; i < Dialogue.Game.DialogueGameDatabase.ReturnQuestID().Count; i++)
                        {
                            if (Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i] != -1)
                            {
                                if (!Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]))
                                {
                                    Quest.QuestGameManager.AcceptQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                    CancelDialogue();
                                }
                                else
                                {
                                    Quest.QuestGameManager.FinishQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                }
                                CancelDialogue();
                            }
                        }
                    }
                    else if (_dialogueAnswers[1] == "Decline" || _dialogueAnswers[1] == "Quit")
                    {
                        CancelDialogue();
                    }

                    #endregion

                    if (Dialogue.Game.DialogueGameDatabase.GetCurrentTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) == "Add Quest")
                    {
                        if (Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[1]))
                        {
                            Debug.Log("quest complete");
                            _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestCompletedTextByID(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[1]);
                            SetAnswers(_questCompleteAnswers);
                        }
                        else
                        {
                            if (Quest.QuestGameManager.ReturnQuestActive(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[1]))
                            {
                                _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestTitleByID(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[1]);
                                SetAnswers(_questAnswers);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        // if question
                        if (Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != string.Empty && Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) == string.Empty)
                        {
                            if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != "End")
                            {
                                if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) == "Add Quest")
                                {
                                    Debug.Log("QUESTSFDF");
                                }
                                else
                                {
                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1];
                                    _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                                }
                            }
                        }
                        // if condition
                        else
                        {
                            if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != "End")
                            {
                                Debug.Log("sup");
                                // If the title is NOT "End" it is a condition
                                switch (Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]))
                                {
                                    case "If":
                                        switch (Dialogue.Game.DialogueGameDatabase.GetConditionStatement(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]))
                                        {
                                            case "QuestActive":
                                                Dialogue.DialogueQuestActive.QuestActive(_npcID, _nodeID, _selectedNPC, 1);

                                                break;
                                            case "QuestCompleted":
                                                Dialogue.DialogueQuestComplete.QuestComplete(_npcID, _nodeID, _selectedNPC, 1);

                                                break;
                                            case "PlayerLevel":
                                                Dialogue.DialoguePlayerLevel.PlayerLevel(_npcID, _nodeID, _selectedNPC, 1);

                                                break;
                                            case "ZoneVisited":
                                                Dialogue.DialogueZoneVisited.ZoneVisited(_npcID, _nodeID, _selectedNPC, 1);

                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "IfNot":
                                        break;
                                    default:
                                        break;
                                }
                            }

                            // The title equals "End"
                            else
                            {
                                for (int i = 0; i < Dialogue.Game.DialogueGameDatabase.ReturnQuestID().Count; i++)
                                {
                                    if (Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i] != -1)
                                    {
                                        if (Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]))
                                        {
                                            Quest.QuestGameManager.FinishQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                        }
                                        else {
                                            Quest.QuestGameManager.AcceptQuest(Dialogue.Game.DialogueGameDatabase.ReturnQuestID()[i]);
                                            Debug.Log("Quest accepted");
                                        }
                                    }
                                }

                                _showDialogue = false;
                                ShowDialogueWheel(false);
                                _selectedNPC.GetComponent<NPC.NpcSystem>().SwitchOnNpcCamera(false);
                                CombatSystem.PlayerController.instance.SetInputBlock(false);

                                if (CombatSystem.CameraController.ReturnFirstPerson())
                                {
                                    if (Cursor.visible)
                                    {
                                        Cursor.visible = false;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

            }

            else
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRight");
                _dialogueAnswerLeft.enabled = false;
            }
            */
            #endregion
        }
        public void CancelDialogue()
        {
            _showDialogue = false;
            ShowDialogueWheel(false);
            _selectedNPC.GetComponent<NPC.NpcSystem>().SwitchOnNpcCamera(false);
            CombatSystem.PlayerController.instance.SetInputBlock(false);

            if (CombatSystem.CameraController.ReturnFirstPerson())
            {
                if (Cursor.visible)
                {
                    Cursor.visible = false;
                }
            }

        }

        void ShowDialogueWheel(bool _set)
        {
            _dialogueWheel.SetActive(_set);
        }

        void ShowQuestWindow()
        {
            _questWindow = new Rect(_windowPosition.x, _windowPosition.y, _windowSize.x, _windowSize.y);
            _questTitleRect = new Rect(_windowPosition.x + 75, _windowPosition.y + 50, _windowSize.x - 100, _windowSize.y - 300);
            _questTextRect = new Rect(_windowPosition.x + 90, _windowPosition.y + 80, _windowSize.x - 200, _windowSize.y - 50);
            _questAcceptRect = new Rect(_windowPosition.x + 100, _windowPosition.y + _windowSize.y - 50, _windowSize.x - 200, _windowSize.y - 350);

            GUI.Box(_questWindow, "", _skin.GetStyle("QuestWindow"));

            GUI.Label(_questTitleRect, _questTitle, _skin.GetStyle("QuestTitle"));
            GUI.Label(_questTextRect, _questText, _skin.GetStyle("QuestText"));

            if (_showButton)
            {
                if (!_questCompleted)
                {
                    GUI.Box(_questAcceptRect, "", _skin.GetStyle("QuestAcceptButton"));
                }
                else
                {
                    GUI.Box(_questAcceptRect, "", _skin.GetStyle("QuestFinishButton"));
                }
            }
            else
            {

            }

            if (_questWindow.Contains(Event.current.mousePosition))
            {

                CombatSystem.PlayerController.instance.HoverOverUI(true);


                if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && !_draggingWindow)
                {
                    _draggingWindow = true;
                    //  CombatSystem.PlayerMovement.SetDraggingUI(true);
                }

                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && _draggingWindow)
                {
                    _draggingWindow = false;
                    //CombatSystem.PlayerMovement.SetDraggingUI(false);
                }
            }
            else
            {
                CombatSystem.PlayerController.instance.HoverOverUI(false);
            }
            if (_questAcceptRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    if (!_questCompleted)
                    {
                        Quest.QuestGameManager.AcceptQuest(_questID);
                        Quest.QuestLog.UpdateLog();
                        // NPCSystem.NPC.PlayerHasAcceptedQuest();
                        _selectedNPC.GetComponent<NPC.NpcSystem>().CheckForQuest();
                    }
                    if (_questCompleted)
                    {
                        //Quest.QuestDatabase.ReturnQuestRewards(_questID);
                        Quest.QuestGameManager.FinishQuest(_questID);
                        //Quest.QuestLog.ClearAll();
                        //Quest.QuestLog.UpdateLog();
                        _playerFinishedQuest = true;
                        _selectedNPC.GetComponent<NPC.NpcSystem>().CheckForQuest();

                    }
                    _showQuestWindow = false;
                    _selectedNPC.GetComponent<NPC.NpcSystem>().PlayerHasMetNPC();
                    _selectedNPC.GetComponent<NPC.NpcSystem>().SetInteraction(false);
                    CombatSystem.PlayerController.instance.HoverOverUI(false);
                }
            }

        }

        public void ExitDialogue(bool _buttonVis)
        {
            _showQuestWindow = false;
        }

        public void SetEndQuest(int _qID, string _title, string _completeText)
        {
            /*
            _textTitle.gameObject.SetActive(true);
            _textBox.gameObject.SetActive(true);
            _textBG.gameObject.SetActive(true);
            _textTitle.text = _title;
            _textBox.text = _completeText;
            _finishQuest.gameObject.SetActive(true);
            */
        }

        public void SetNodeID(int _id)
        {
            _nodeID = _id;
        }

        public void SetQuestText(string _text)
        {
            _questText = _text;
        }

        void ShowZone()
        {
            Rect _zoneRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100);
            GUI.Label(_zoneRect, _zoneName, _skin.GetStyle("ShowZone"));

            InvokeRepeating("CancelShowZone", 4, 100);
        }

        public void SetShowZone(bool _set, string _name, string _desc)
        {
            _showZone = _set;
            _zoneName = _name;
            _zoneDesc = _desc;
        }

        void CancelShowZone()
        {
            _showZone = false;
            CancelInvoke();
        }

        void CancelShowMessage()
        {
            _showMessage = false;
            _message = "";
            CancelInvoke();
        }

        void CancelShowHint()
        {
            _showHint = false;
            _hintMessage = "";
            CancelInvoke();
        }

        public void ShowMessage(string _msg, bool _set)
        {
            _showMessage = _set;
            _message = _msg;

        }

        public void ShowHint(string _msg, bool _set)
        {
            _showHint = _set;
            _hintMessage = _msg;
        }

        public bool ReturnPlayerFinishedQuest()
        {
            return _playerFinishedQuest;

        }

        public void ResetPlayerFinishedQuest()
        {
            _playerFinishedQuest = false;
        }

        void CreateDialogue(int side)
        {
            if (_dialogueAnswers[side] == "Accept")
            {
                if(!Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.GetQuestID(_npcID, _nodeID)))
                {
                    Quest.QuestGameManager.AcceptQuest(Dialogue.Game.DialogueGameDatabase.GetQuestID(_npcID, _nodeID));
                    CancelDialogue();
                }
                else
                {
                    CancelDialogue();
                }
            }
            else if (_dialogueAnswers[side] == "Finish")
            {
                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                Debug.Log(_nodeID);
            }

            else if (_dialogueAnswers[side] == "Decline" || _dialogueAnswers[side] == "Quit")
            {
                CancelDialogue();
            }


            else if (Dialogue.Game.DialogueGameDatabase.GetCurrentTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) == "Add Quest" || Dialogue.Game.DialogueGameDatabase.GetCurrentTitle(_npcID, _nodeID) == "Add Quest")
            {
                if (!Quest.QuestGameManager.ReturnQuestComplete(Dialogue.Game.DialogueGameDatabase.GetQuestID(_npcID, _nodeID)))
                {
                    _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestTitleByID(Dialogue.Game.DialogueGameDatabase.GetQuestID(_npcID, _nodeID));
                    SetAnswers(_questAnswers);
                }
                else
                {
                    _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Quest.QuestGameManager.ReturnQuestTitleByID(Dialogue.Game.DialogueGameDatabase.GetQuestID(_npcID, _nodeID));
                    SetAnswers(_questCompleteAnswers);
                }
            }
            else
            {
                // if question
                if (Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) != string.Empty && Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) == string.Empty)
                {
                    if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) != "End")
                    {
                        _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                        _questText = _selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                        SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                        _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);

                    }
                }
                // if condition
                else
                {
                    if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) != "End" && Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]) != "Add Quest")
                    {
                        // If the title is NOT "End" it is a condition
                        switch (Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]))
                        {
                            case "If":

                                switch (Dialogue.Game.DialogueGameDatabase.GetCurrentConditionStatement(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]))
                                {
                                    case "QuestActive":
                                        Dialogue.DialogueQuestActive.QuestActive(_npcID, _nodeID, _selectedNPC, side);

                                        break;
                                    case "QuestCompleted":
                                        Dialogue.DialogueQuestComplete.QuestComplete(_npcID, _nodeID, _selectedNPC, side);

                                        break;
                                    case "PlayerLevel":
                                        Dialogue.DialoguePlayerLevel.PlayerLevel(_npcID, _nodeID, _selectedNPC, side);

                                        break;
                                    case "ZoneVisited":
                                        Dialogue.DialogueZoneVisited.ZoneVisited(_npcID, _nodeID, _selectedNPC, side);

                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "IfNot":
                                break;
                            default:
                                break;
                        }
                    }

                    // The title equals "End"
                    else
                    {
                        Debug.Log(Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[side]));

                        _showDialogue = false;
                        ShowDialogueWheel(false);
                        _selectedNPC.GetComponent<NPC.NpcSystem>().SwitchOnNpcCamera(false);
                        CombatSystem.PlayerController.instance.SetInputBlock(false);

                        if (CombatSystem.CameraController.ReturnFirstPerson())
                        {
                            if (Cursor.visible)
                            {
                                Cursor.visible = false;
                            }
                        }
                    }
                }
            }
        }
        
    }
}