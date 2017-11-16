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

        private static Image _dialogueWheel;
        private Image _dialogueAnswerLeft;
        private Image _dialogueAnswerRight;

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

        private static bool _playerFinishedQuest;

        private static int _nodeID;

        void OnEnable()
        {
            _dialogueWheel = GameObject.Find("DialogueWheel").GetComponent<Image>();
            _dialogueAnswerLeft = GameObject.Find("DialogueAnswerLeft").GetComponent<Image>();
            _dialogueAnswerRight = GameObject.Find("DialogueAnswerRight").GetComponent<Image>();

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

        public static void SetDialogue(string _title, string _text, bool _buttonVis, int _nwNpcID, int _nwQuestID, GameObject _npc)
        {

            _questTitle = _title;
            _questText = _text;
            _showButton = _buttonVis;
            _npcID = _nwNpcID;
            _questID = _nwQuestID;
            _showDialogue = true;
            _selectedNPC = _npc;
            _questCompleted = Quest.QuestDatabase.CheckQuestCompleteByID(_questID);


            CombatSystem.PlayerController.instance.SetInputBlock(true);
        }

        public static void SetAnswers(List<string> _answers)
        {
            _dialogueAnswers[0] = _answers[0];
            _dialogueAnswers[1] = _answers[1];
            Debug.Log(_dialogueAnswers[0]);
        }

        static void ShowDialogueWheel(bool _set)
        {
            _dialogueWheel.transform.gameObject.SetActive(_set);
        }

        public static void InitiateDialogue(int _id, bool _questGiver, GameObject _npcObject)
        {
            if (Dialogue.DialogueDatabase.GetInitialQuestionFromNPC(_id) != string.Empty)
            {
                SetDialogue("", Dialogue.DialogueDatabase.GetInitialQuestionFromNPC(_id), false, _id, 0, _npcObject);

                // Get the followup NodeID
                _nodeID = Dialogue.DialogueDatabase.GetNextNodeID(_id, 0);
                SetAnswers(Dialogue.DialogueDatabase.GetAnswersForQuestion(_npcID, _nodeID));

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

            _dialogueAnswersBox[0] = new Rect(Screen.width / 2 - 300, Screen.height - 350, 200, 50);
            _dialogueAnswersBox[1] = new Rect(Screen.width / 2 + 100, Screen.height - 350, 200, 50);

            _dialogueAnswersRect[0] = new Rect(Screen.width / 2 - 300, Screen.height - 350, 200, 50);
            _dialogueAnswersRect[1] = new Rect(Screen.width / 2 + 100, Screen.height - 350, 200, 50);

            GUI.Label(_dialogueAnswersRect[0], _dialogueAnswers[0], _styleLeft);

            GUI.Label(_dialogueAnswersRect[1], _dialogueAnswers[1], _styleRight);

            // If the cursor is hovering over the left answer
            if (_dialogueAnswersBox[0].Contains(Event.current.mousePosition))
            {
                // Change the style to create the hover effect
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeftHover");
                _dialogueAnswerLeft.enabled = true;

                // If we have pressed the left mouse button
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    // Check if there are 2 answers - to catch the argument exception
                    if (Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID).Count > 1)
                    {
                        // If the question is not empty
                        if (Dialogue.DialogueDatabase.GetFollowupQuestion(_npcID, Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[0]) != string.Empty)
                        {
                            // Set the new question from the NPC
                            _questText = Dialogue.DialogueDatabase.GetFollowupQuestion(_npcID, Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[0]);

                            // Set the NodeID to the NodeID of the question
                            // This is done because of the following:
                            // Every question has a NodeID
                            //      Every answer has a 'previousNode' to refers to the question
                            // If we dont update the NodeID to the current question it will bug out
                            _nodeID = Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[0];

                            // If the title of the nodes is not "End" - meaning the end of the conversation
                            if (Dialogue.DialogueDatabase.GetTitleByNode(_npcID, Dialogue.DialogueDatabase.GetNextNodeID(_npcID, _nodeID))[0] != "End")
                            {
                                // Set the Answers with the new replies
                                SetAnswers(Dialogue.DialogueDatabase.GetAnswersForQuestion(_npcID, Dialogue.DialogueDatabase.GetNextNodeID(_npcID, _nodeID)));

                            }

                            // If the title is "End" - cancel the dialogue window
                            else
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
                        }
                    }
                    // If there are less than 2 answers - cancel everything
                    else
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

                }
            }

            // If we are not hovering over the left answer
            else
            {
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeft");
                _dialogueAnswerLeft.enabled = false;
            }

            if (_dialogueAnswersBox[1].Contains(Event.current.mousePosition))
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRightHover");
                _dialogueAnswerRight.enabled = true;

                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    if (Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID).Count > 1)
                    {
                        if (Dialogue.DialogueDatabase.GetFollowupQuestion(_npcID, Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[1]) != string.Empty)
                        {
                            _questText = Dialogue.DialogueDatabase.GetFollowupQuestion(_npcID, Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[1]);
                            _nodeID = Dialogue.DialogueDatabase.GetNodeIDFromAnswer(_npcID, _nodeID)[1];

                            if (Dialogue.DialogueDatabase.GetTitleByNode(_npcID, Dialogue.DialogueDatabase.GetNextNodeID(_npcID, _nodeID))[1] != "End")
                            {
                                SetAnswers(Dialogue.DialogueDatabase.GetAnswersForQuestion(_npcID, Dialogue.DialogueDatabase.GetNextNodeID(_npcID, _nodeID)));
                            }
                            else
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
                        }
                    }
                    else
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
                }

            }

            else
            {
                _styleRight = _dialogueSkin.GetStyle("DialogueAnswerRight");
                _dialogueAnswerRight.enabled = false;
            }

        }
        public static void CancelDialogue()
        {
            _showDialogue = false;
            ShowDialogueWheel(false);

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


                if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag && !_draggingWindow)
                {
                    _draggingWindow = true;
                    //  CombatSystem.PlayerMovement.SetDraggingUI(true);
                }

                if (Event.current.button == 0 && Event.current.type == EventType.mouseUp && _draggingWindow)
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
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    if (!_questCompleted)
                    {
                        Quest.QuestDatabase.AcceptQuest(_questID);
                        Quest.QuestLog.UpdateLog();
                        // NPCSystem.NPC.PlayerHasAcceptedQuest();
                        _selectedNPC.GetComponent<NPC.NpcSystem>().CheckForQuest();
                    }
                    if (_questCompleted)
                    {
                        //Quest.QuestDatabase.ReturnQuestRewards(_questID);
                        Quest.QuestDatabase.FinishQuest(_questID);
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

        public static void ExitDialogue(bool _buttonVis)
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

        void ShowZone()
        {
            Rect _zoneRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100);
            GUI.Label(_zoneRect, _zoneName, _skin.GetStyle("ShowZone"));

            InvokeRepeating("CancelShowZone", 4, 100);
        }

        public static void SetShowZone(bool _set, string _name, string _desc)
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

        public static void ShowMessage(string _msg, bool _set)
        {
            _showMessage = _set;
            _message = _msg;

        }

        public static void ShowHint(string _msg, bool _set)
        {
            _showHint = _set;
            _hintMessage = _msg;
        }

        public static bool ReturnPlayerFinishedQuest()
        {
            return _playerFinishedQuest;

        }

        public static void ResetPlayerFinishedQuest()
        {
            _playerFinishedQuest = false;
        }

    }
}