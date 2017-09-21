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
 
        public GUISkin _skin;

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

        // Use this for initialization
        void Start()
        {

            if (PlayerPrefs.GetFloat("QuestWindowPosX") > 0)
            {
                _windowPosition = new Vector2(PlayerPrefs.GetFloat("QuestWindowPosX"), PlayerPrefs.GetFloat("QuestWindowPosY"));
            }



        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {

            //_player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        }

        void OnGUI()
        {

            if(_showDialogue)
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
            if(_showHint)
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
            //_showQuestWindow = true;
            _selectedNPC = _npc;

            _questCompleted = Quest.QuestDatabase.CheckQuestCompleteByID(_questID);
        }

        public static void SetAnswers(string _one, string _two)
        {
            _dialogueAnswers[0] = _one;
            _dialogueAnswers[1] = _two;


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
            _dialogueBox = new Rect(Screen.width / 2 - 250, Screen.height - 400, 500, 100);
            _dialogueTextBox = new Rect(Screen.width / 2 - 250 + 25, Screen.height - 360, 450, 100);

            GUI.Box(_dialogueBox, "");
            GUI.Label(_dialogueTextBox, _questText, _skin.GetStyle("QuestText"));

            _dialogueAnswersBox[0] = new Rect(Screen.width / 2 - 250, Screen.height - 300, 200, 50);
            _dialogueAnswersBox[1] = new Rect(Screen.width / 2, Screen.height - 300, 200, 50);

            _dialogueAnswersRect[0] = new Rect(Screen.width / 2 - 250, Screen.height - 300, 200, 50);
            _dialogueAnswersRect[1] = new Rect(Screen.width / 2, Screen.height - 300, 200, 50);

            GUI.Box(_dialogueAnswersBox[0], "");
            GUI.Label(_dialogueAnswersRect[0], _dialogueAnswers[0]);

            GUI.Box(_dialogueAnswersBox[1], "");
            GUI.Label(_dialogueAnswersRect[1], _dialogueAnswers[1]);

            if (_dialogueAnswersBox[0].Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {

                    Debug.Log(DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]));
                    if (DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]) != -1)
                    {
                        Debug.Log("ShowDialogue - node id " + DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]));
                        DialogueSystem.DialogueDatabase.GetAnswersByNode(DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]));
                        _questText = DialogueSystem.DialogueDatabase.GetFollowupQuestion(_npcID, DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]));

                        if (_questText != String.Empty)
                        {
                            SetAnswers(DialogueSystem.DialogueDatabase.ReturnAnswersByNode()[0], DialogueSystem.DialogueDatabase.ReturnAnswersByNode()[1]);
                        }
                        else
                        {
                            _showDialogue = false;
                            if (CombatSystem.CameraController.ReturnFirstPerson())
                            {
                                if (Cursor.visible)
                                {
                                    Cursor.visible = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        _showDialogue = false;
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

            if (_dialogueAnswersBox[1].Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    Debug.Log("ShowDialogue - node id " + DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[0]));
                    if (DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[1]) != -1)
                    {
                        
                        DialogueSystem.DialogueDatabase.GetAnswersByNode(DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[1]));
                        _questText = DialogueSystem.DialogueDatabase.GetFollowupQuestion(_npcID, DialogueSystem.DialogueDatabase.GetNodeID(_npcID, DialogueSystem.DialogueDatabase.ReturnNodeIdByNode()[1]));
                        if (_questText != String.Empty)
                        {
                            SetAnswers(DialogueSystem.DialogueDatabase.ReturnAnswersByNode()[0], DialogueSystem.DialogueDatabase.ReturnAnswersByNode()[1]);
                        }
                        else
                        {
                            _showDialogue = false;
                        }
                    }
                    else
                    {
                        _showDialogue = false;
                    }
                }

            }


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