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

        private static int _nodeID = 0;

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
        }

        static void ShowDialogueWheel(bool _set)
        {
            _dialogueWheel.transform.gameObject.SetActive(_set);
        }

        public static void InitiateDialogue(int _id, bool _questGiver, GameObject _npcObject)
        {
            
            if(Dialogue.Game.DialogueGameDatabase.GetInitialQuestionFromNPC(_id) != string.Empty)
            {
                SetDialogue("", Dialogue.DialogueDatabase.GetInitialQuestionFromNPC(_id), false, _id, 0, _npcObject);

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

            GUI.Label(_dialogueAnswersRect[0], _dialogueAnswers[0], _styleLeft);

            GUI.Label(_dialogueAnswersRect[1], _dialogueAnswers[1], _styleRight);

            // If the cursor is hovering over the left answer
            if (_dialogueAnswersBox[0].Contains(Event.current.mousePosition))
            {
                // we use [0] for the LEFT answer


                // Change the style to create the hover effect
                _styleLeft = _dialogueSkin.GetStyle("DialogueAnswerLeftHover");
                _dialogueAnswerLeft.enabled = true;

                // If we have pressed the left mouse button
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {

                    // if question
                    if(Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != string.Empty)
                    {
                        if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != "End")
                        {
                            _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                            _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                            SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                            _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                        }
                    }
                    // if condition
                    else
                    {
                        if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]) != "End")
                        {
                            // If the title is NOT "End" it is a condition
                            switch (Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]))
                            {
                                case "If":
                                    switch (Dialogue.Game.DialogueGameDatabase.GetConditionStatement(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]))
                                    {
                                        case "QuestActive":

                                            Quest.QuestDatabase.GetAllQuests();

                                            // check if quest is active
                                            if (Quest.QuestDatabase.ReturnQuestActive(Quest.QuestDatabase.GetQuestID(int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])))))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }

                                            break;
                                        case "QuestCompleted":


                                            Quest.QuestDatabase.GetAllQuests();
                                            // check if quest is active
                                            if (Quest.QuestDatabase.CheckQuestComplete(Quest.QuestDatabase.GetQuestID(int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])))))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }


                                            break;
                                        case "PlayerLevel":

                                            if(CombatSystem.PlayerController.instance.ReturnPlayerLevel() == int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }

                                            break;
                                        case "ZoneVisited":

                                            if (GameObject.Find("Zone_" + Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])) != null)
                                            {
                                                if (GameObject.Find("Zone_" + Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])).GetComponent<Quest.Zone>().ReturnPlayerVisited())
                                                {
                                                    // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                    // Set the Question
                                                    _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                    // Fetch the corresponding answers
                                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                                }
                                                else
                                                {
                                                    // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                    // Set the Question
                                                    _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                    // Fetch the corresponding answers
                                                    //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Zone does not exist");
                                            }


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

                    // if question
                    if (Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != string.Empty)
                    {
                        if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != "End")
                        {
                            _nodeID = Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1];
                            _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID);
                            SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID)));
                            _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID);
                        }
                    }
                    // if condition
                    else
                    {
                        if (Dialogue.Game.DialogueGameDatabase.GetTitle(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]) != "End")
                        {
                            // If the title is NOT "End" it is a condition
                            switch (Dialogue.Game.DialogueGameDatabase.GetCondition(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]))
                            {
                                case "If":
                                    switch (Dialogue.Game.DialogueGameDatabase.GetConditionStatement(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]))
                                    {
                                        case "QuestActive":

                                            // check if quest is active
                                            if (Quest.QuestDatabase.ReturnQuestActive(Quest.QuestDatabase.GetQuestID(int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0])))))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);
                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }

                                            break;
                                        case "QuestCompleted":

                                            Quest.QuestDatabase.GetAllQuests();
                                            // check if quest is active
                                            if (Quest.QuestDatabase.CheckQuestComplete(Quest.QuestDatabase.GetQuestID(int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1])))))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);
                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }

                                            break;
                                        case "PlayerLevel":


                                            if (CombatSystem.PlayerController.instance.ReturnPlayerLevel() == int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1])))
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);

                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                // Fetch the corresponding answers
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                            }
                                            else
                                            {
                                                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);
                                                // Set the Question
                                                _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                // Fetch the corresponding answers
                                                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                            }

                                            break;
                                        case "ZoneVisited":
                                            if(GameObject.Find("Zone_" + Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1])) != null)
                                            {
                                                if(GameObject.Find("Zone_" + Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1])).GetComponent<Quest.Zone>().ReturnPlayerVisited())
                                                {
                                                    // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);

                                                    // Set the Question
                                                    _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "True");

                                                    // Fetch the corresponding answers
                                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True")));
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "True");
                                                }
                                                else
                                                {
                                                    // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[1]);

                                                    // Set the Question
                                                    _questText = Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID, "False");

                                                    // Fetch the corresponding answers
                                                    //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                                                    SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False")));
                                                    _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID, "False");
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Zone does not exist");
                                            }

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