using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;


namespace Quest
{
    public class QuestLog : MonoBehaviour
    {

        public GUISkin _skin;

        private Vector2 _inGameLogPos = new Vector2(100, 100);

        private Vector2 _questWindowPos = new Vector2(100, 100);
        private Vector2 _questWindowSize = new Vector2(700, 500);

        private static bool _queryDatabase = false;
        private static bool _clearLists = false;

        private List<int> _questLogQuestID = new List<int>();

        private static bool _questQuestCollection = false;

        private List<int> _currentCollected = new List<int>();
        private List<int> _totalToCollect = new List<int>();
        private List<string> _questTitles = new List<string>();
        private List<QuestType> _questTypes = new List<QuestType>();
        

        private List<Rect> _displayTitles = new List<Rect>();

        private bool _showQuestLogWindow = false;
        private bool _draggingGameLog = false;



        // Use this for initialization
        void Start()
        {
            _inGameLogPos = new Vector2(PlayerPrefs.GetFloat("InGameLogPosX"), PlayerPrefs.GetFloat("InGameLogPosY"));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("l"))
            {
                _showQuestLogWindow = !_showQuestLogWindow;
            }

        }

        void OnGUI()
        {
            if (!CombatSystem.InteractionManager.instance.ReturnLoadingLevel())
            {
                InGameQuestLog();

                // QUESTLOG WINDOW
                if (_showQuestLogWindow)
                {
                    QuestLogWindow();

                }


                if (_draggingGameLog)
                {
                    _inGameLogPos.x = Event.current.mousePosition.x;
                    _inGameLogPos.y = Event.current.mousePosition.y;

                    PlayerPrefs.SetFloat("InGameLogPosX", _inGameLogPos.x);
                    PlayerPrefs.SetFloat("InGameLogPosY", _inGameLogPos.y);
                }
            }

            if(_clearLists)
            {
                Clear();
                _clearLists = false;
            }
        }

        void QuestLogWindow()
        {
            Rect _questLogWindow = new Rect(_questWindowPos.x, _questWindowPos.y, _questWindowSize.x,_questWindowSize.y);
            Rect _questLogWindowTitle = new Rect(_questWindowPos.x + 125, _questWindowPos.y + 40, 200, 25);

            GUI.Box(_questLogWindow, "", _skin.GetStyle("MainQuestWindow"));
            GUI.Label(_questLogWindowTitle, "Quest Log Window");

            for (int i = 0; i < _questLogQuestID.Count; i++)
            {
                _displayTitles.Add(new Rect(_questWindowPos.x + 50, _questWindowPos.y + 80 + (20 * i), 275, 30));
                GUI.Box(_displayTitles[i], _questTitles[i]);
            }


        }

        void InGameQuestLog()
        {
            if (!_queryDatabase)
            {
                Clear();
                Quest.QuestDatabase.ClearAll();
                Quest.QuestDatabase.GetAllActiveQuests();

                for (int i = 0; i < Quest.QuestDatabase.ReturnActiveQuestCount(); i++)
                {
                    _questLogQuestID.Add(Quest.QuestDatabase.ReturnActiveQuestID(i));
                    _currentCollected.Add(Quest.QuestDatabase.GetQuestItemsCollected(_questLogQuestID[i]));
                    _totalToCollect.Add(Quest.QuestDatabase.GetQuestAmount(_questLogQuestID[i]));
                    _questTitles.Add(Quest.QuestDatabase.ReturnActiveQuestTitle(i));
                    _questTypes.Add(Quest.QuestDatabase.ReturnActiveQuestType(i));
                }

                _queryDatabase = true;
            }

            if (!_questQuestCollection)
            {

                for (int i = 0; i < Quest.QuestDatabase.ReturnActiveQuestCount(); i++)
                {

                    _currentCollected[i] = Quest.QuestDatabase.GetQuestItemsCollected(_questLogQuestID[i]);
                    _totalToCollect[i] = Quest.QuestDatabase.GetQuestAmount(_questLogQuestID[i]);
                }

                _questQuestCollection = true;
            }

            // IN GAME QUESTLOG
            Rect _gameQuestLog = new Rect(_inGameLogPos.x, _inGameLogPos.y, 250, 200);
            Rect _gameQuestLogTitle = new Rect(_inGameLogPos.x + 10, _inGameLogPos.y + 10, 80, 20);
            GUI.Box(_gameQuestLog, "");
            GUI.Label(_gameQuestLogTitle, "Quest Log", _skin.GetStyle("QuestLogTitle"));


            if (_queryDatabase)
            {
                if (_questLogQuestID.Count() > 0)
                {
                    for (int i = 0; i < Quest.QuestDatabase.ReturnActiveQuestCount(); i++)
                    {
                        if (_questTypes[i] != QuestType.None)
                        {
                            if (_questTypes[i] == QuestType.Collect || _questTypes[i] == QuestType.Kill)
                            {
                                GUI.Label(new Rect(_inGameLogPos.x + 10, _inGameLogPos.y + 30 + (10 * i), 150, 25), Quest.QuestDatabase.ReturnActiveQuestTitle(i), _skin.GetStyle("QuestLogQuest"));
                                if (!Quest.QuestDatabase.CheckQuestCompleteByID(_questLogQuestID[i]))
                                {
                                    GUI.Label(new Rect(_inGameLogPos.x + 150, _inGameLogPos.y + 30 + (10 * i), 150, 25), "(" + _currentCollected[i] + "/" + _totalToCollect[i] + ")", _skin.GetStyle("QuestLogQuest"));
                                }
                                if (Quest.QuestDatabase.CheckQuestCompleteByID(_questLogQuestID[i]))
                                {
                                    GUI.Label(new Rect(_inGameLogPos.x + 150, _inGameLogPos.y + 30 + (10 * i), 150, 25), "(Completed)", _skin.GetStyle("QuestLogQuest"));
                                }
                            }

                            if (_questTypes[i] == QuestType.Explore)
                            {

                                GUI.Label(new Rect(_inGameLogPos.x + 10, _inGameLogPos.y + 30 + (10 * i), 150, 25), Quest.QuestDatabase.ReturnActiveQuestTitle(i), _skin.GetStyle("QuestLogQuest"));
                                if (Quest.QuestDatabase.CheckQuestCompleteByID(_questLogQuestID[i]))
                                {
                                    GUI.Label(new Rect(_inGameLogPos.x + 150, _inGameLogPos.y + 30 + (10 * i), 150, 25), "(Completed)", _skin.GetStyle("QuestLogQuest"));
                                }
                            }
                        }

                    }
                }
            }

            if(_gameQuestLog.Contains(Event.current.mousePosition))
            {
                //CombatSystem.PlayerMovement.HoveringOverQuestLog(true);

                if(Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    //CombatSystem.PlayerMovement.SetDraggingQuestLog(true);
                }
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                {
                  //  CombatSystem.PlayerMovement.SetDraggingQuestLog(false);
                }

                if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && !_draggingGameLog)
                {
                    _draggingGameLog = true;
                    //CombatSystem.PlayerMovement.SetDraggingQuestLog(true);
                }

                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && _draggingGameLog)
                {
                    _draggingGameLog = false;
                    //CombatSystem.PlayerMovement.SetDraggingQuestLog(false);
                }
            }
            else
            {
                //CombatSystem.PlayerMovement.HoveringOverQuestLog(false);
            }

        }

        public static void SetQuestCollection(bool _set)
        {
            _questQuestCollection = _set;
        }

        public static void UpdateLog()
        {
            _queryDatabase = false;
        }

        public static void ClearAll()
        {
            _clearLists = true;
        }

        void Clear()
        {
            _questLogQuestID.Clear();
            _currentCollected.Clear();
            _totalToCollect.Clear();
            _questTitles.Clear();
            _questTypes.Clear();
            
        }

    }
}