using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FeedbackEditor
{

    public enum FeedbackType
    {
        None,
        Hint,
        Achievement,
    }

    public enum FeedbackTrigger
    {
        None,
        Trigger,
        Time,
        Game_Start,
    }

    public enum HintCondition
    {
        None,
        Trigger,
        Time,
        IdleTime,
    }
        

    public enum AchievementType
    {
        None,
        Level_Gained,
  //      Monsters_Slain,
        Gold_Collected,
        Quests_Completed,
    }

    public enum TriggerShape
    {
        None,
        Square,
        Sphere,
        Capsule,
    }

    public class FeedbackManager : EditorWindow
    {

        private FeedbackType _feedbackType;
        private FeedbackTrigger _feedbackTrigger;
        private HintCondition _hintCondition;
        private AchievementType _achievementType;
        private TriggerShape _triggerShape;
        private float _timer = 0.0f;
        private int _triggerSize;
        private int _achievementAmount;

        private string _feedbackText;

        private bool _addFeedback = false;
        private bool _editFeedback = false;
        private bool _deleteFeedback = false;
        private bool _addGameFeedback = false;
        private bool _editGameFeedback = false;

        private bool _isSelectionSet = false;
        private bool _isDataLoaded = false;
        private bool _isHintDataLoaded = false;
        private bool _isAchievementDataLoaded = false;
        private bool[] _feedbackSelection;

        private int _feedbackSelectIndex;
        private int _achievementSelectIndex;

        private GUISkin _skin;

        [MenuItem("Level Design/Feedback/Feedback Manager")]

        static void ShowEditor()
        {
            FeedbackManager _fm = EditorWindow.GetWindow<FeedbackManager>(true, "Feedback Manager");
        }

        void OnEnable()
        {
            FeedbackEditor.FeedbackDB.ClearAll();

            // Get all the feedback
            FeedbackEditor.FeedbackDB.GetAllFeedback();

            _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
            
            
        }

        void OnGUI()
        {
            GUI.skin = _skin;

            GUI.Label(new Rect(0, 0, 700, 100), "Welcome to the Feedback Manager");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!_addFeedback && !_editFeedback && !_deleteFeedback && !_addGameFeedback && !_editGameFeedback)
            {

                if (GUILayout.Button("Add Feedback"))
                {
                    ClearAll();
                    _addFeedback = true;
                }

                if (GUILayout.Button("Edit Feedback"))
                {
                    ClearAll();
                    _editFeedback = true;
                }

                if (GUILayout.Button("Delete Feedback"))
                {
                    ClearAll();
                    _deleteFeedback = true;
                }

                if(GUILayout.Button("Add Feedback to Game"))
                {
                    ClearAll();
                    _addGameFeedback = true;
                }
                if(GUILayout.Button("Edit Feedback Currently in Game"))
                {
                    ClearAll();
                    _editGameFeedback = true;
                }

                
            }

            if (_addFeedback)
            {
                AddFeedback();
            }

            if(_deleteFeedback)
            {
                DeleteFeedback();
            }
            if(_editFeedback)
            {
                EditFeedback();
            }

            if(_editGameFeedback)
            {
                EditGameFeedback();
            }
            if(_addGameFeedback)
            {
                AddGameFeedback();
            }


        }

        void AddFeedback()
        {
            _feedbackType = (FeedbackType)EditorGUILayout.EnumPopup("Type of Feedback: ", _feedbackType);

            if (_feedbackType == FeedbackType.Hint)
            {
                ShowHint();
            }

            //if (_feedbackType == FeedbackType.ConditionalHint)
            //{
            //    ConditionalHint();
            //}

            if (_feedbackType == FeedbackType.Achievement)
            {
                Achievement();
            }

            if(GUILayout.Button("BACK"))
            {
                _addFeedback = false;
            }

        }

        void ShowHint()
        {
            _feedbackTrigger = (FeedbackTrigger)EditorGUILayout.EnumPopup("Trigger Hint by: ", _feedbackTrigger);

            if (_feedbackTrigger != FeedbackTrigger.None)
            {
                if (_feedbackTrigger == FeedbackTrigger.Time)
                {
                    _timer = EditorGUILayout.FloatField("After how much time: ", _timer);
                }

                if (_feedbackTrigger == FeedbackTrigger.Trigger)
                {
                    _triggerShape = (TriggerShape)EditorGUILayout.EnumPopup("Shape of Trigger: ", _triggerShape);
                    _triggerSize = EditorGUILayout.IntField(_triggerSize);
                }
                GUILayout.Label("The Hint text", EditorStyles.boldLabel);
                _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Save to Database"))
                {
                    FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer,0, _triggerShape.ToString(), _feedbackText, null, null, _triggerSize, 0);
                    _addFeedback = false;
                }
                if (GUILayout.Button("Save and Add to Game"))
                {
                    FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, null, _triggerSize, 0);
                    AddToGame(FeedbackEditor.FeedbackDB.ReturnLastID(), _feedbackType, _feedbackTrigger, _timer, 0, _triggerShape, _feedbackText, null, null, _triggerSize, 0);
                    _addFeedback = false;
                }

                EditorGUILayout.EndHorizontal();

            }
        }

        void ConditionalHint()
        {

        }

        void Achievement()
        {
            _achievementType = (AchievementType) EditorGUILayout.EnumPopup("Achievement for: ", _achievementType);

            if(_achievementType != AchievementType.None)
            {
                _achievementAmount = EditorGUILayout.IntField("Amount needed: ", _achievementAmount);

                _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save to Database"))
                {
                    FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, _achievementAmount);
                }
                if (GUILayout.Button("Save and Add to Game"))
                {
                    FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, _achievementAmount);
                    AddToGame(FeedbackEditor.FeedbackDB.ReturnLastID(), _feedbackType, _feedbackTrigger, _timer, 0, _triggerShape, _feedbackText, null, _achievementType.ToString(), _triggerSize, _achievementAmount);
                    _addFeedback = false;
                }
                EditorGUILayout.EndHorizontal();

            }
            
        }

        void AddToGame(int _id, FeedbackType _type, FeedbackTrigger _trigger, float _time, float _idleTime, TriggerShape _shape, string _text, string _condition, string _achievement, int _triggerSize, int _amount)
        {

            if(GameObject.Find("PLAYERFEEDBACK") == null)
            {
                GameObject _parent = new GameObject();
                _parent.name = "PLAYERFEEDBACK";
            }
            else
            {
                
            }
            GameObject _feedback = Instantiate(Resources.Load("World_Building/GamePlay/HintTrigger")) as GameObject;
            _feedback.name = "FeedbackTrigger_" + _id;
            _feedback.GetComponent<Transform>().localScale = new Vector3(_triggerSize, _triggerSize, _triggerSize);
            _feedback.transform.SetParent(GameObject.Find("PLAYERFEEDBACK").transform);
            _feedback.layer = 2;

            if (_trigger == FeedbackTrigger.Trigger)
            {
                if (_triggerShape == TriggerShape.Capsule)
                {
                    _feedback.AddComponent<CapsuleCollider>();
                    _feedback.GetComponent<CapsuleCollider>().radius = _triggerSize;
                    _feedback.GetComponent<CapsuleCollider>().isTrigger = true;

                }
                if (_triggerShape == TriggerShape.Sphere)
                {
                    _feedback.AddComponent<SphereCollider>();
                    _feedback.GetComponent<SphereCollider>().radius = _triggerSize;
                    _feedback.GetComponent<SphereCollider>().isTrigger = true;
                }

                if(_triggerShape == TriggerShape.Square)
                {
                    _feedback.AddComponent<BoxCollider>();
                    _feedback.GetComponent<BoxCollider>().size = new Vector3(_triggerSize, _triggerSize, _triggerSize);
                    _feedback.GetComponent<BoxCollider>().isTrigger = true;
                }
            }

            _feedback.GetComponentInChildren<FeedbackEditor.Feedback>().SetValues(_id, _type.ToString(), _time, _idleTime, _text, _condition, _achievement, _trigger.ToString(), _amount);

        }

        void AddGameFeedback()
        {
            FeedbackType _type = FeedbackType.None;
            FeedbackTrigger _trigger = FeedbackTrigger.None;
            TriggerShape _shape = TriggerShape.None;

            FeedbackEditor.FeedbackDB.ClearAll();
            FeedbackEditor.FeedbackDB.GetAllFeedback();

            _feedbackSelectIndex = EditorGUILayout.Popup(_feedbackSelectIndex, FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().ToArray());

            if(GUILayout.Button("Add " + FeedbackDB.ReturnFeedbackText(_feedbackSelectIndex) + " to the game"))
            {

                if(FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Hint")
                {
                    _type = FeedbackType.Hint;
                }

                if (FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Achievement")
                {
                    _type = FeedbackType.Achievement;
                }
                

                if(FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Game_Start")
                {
                    _trigger = FeedbackTrigger.Game_Start;
                }

                if (FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Time")
                {
                    _trigger = FeedbackTrigger.Time;
                }
                if (FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Trigger")
                {
                    _trigger = FeedbackTrigger.Trigger;
                }

                if(FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Capsule")
                {
                    _shape = TriggerShape.Capsule;
                }

                if (FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Sphere")
                {
                    _shape = TriggerShape.Sphere;
                }

                if (FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Square")
                {
                    _shape = TriggerShape.Square;
                }
                

                AddToGame(FeedbackDB.ReturnFeedbackID(_feedbackSelectIndex), _type, _trigger, FeedbackDB.ReturnFeedbackTimer(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackIdleTimer(_feedbackSelectIndex), _shape, FeedbackDB.ReturnFeedbackText(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackCondition(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackAchievement(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackTriggerSize(_feedbackSelectIndex), FeedbackDB.ReturnAchievementAmount(_feedbackSelectIndex));
            }

            if (GUILayout.Button("BACK"))
            {
                _addGameFeedback = false;
            }
        }

        void DeleteFeedback()
        {
            // Make sure all the lists are clear
            FeedbackEditor.FeedbackDB.ClearAll();

            // Get all the feedback
            FeedbackEditor.FeedbackDB.GetAllFeedback();

            if (!_isSelectionSet)
            {
                _feedbackSelection = new bool[FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().Count];
                _isSelectionSet = true;
            }


            for (int i = 0; i < FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().Count; i++)
            {

                GUILayout.BeginHorizontal(GUILayout.Width(250));
                    GUILayout.Label("ID: " + FeedbackEditor.FeedbackDB.ReturnFeedbackID(i), GUILayout.Width(50));
                    GUILayout.Label("Text: " + FeedbackEditor.FeedbackDB.ReturnFeedbackText(i), GUILayout.Width(150));
                    _feedbackSelection[i] = EditorGUILayout.Toggle(_feedbackSelection[i]);
                GUILayout.EndHorizontal();


            }
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("DELETE SELECTED"))
            {
                for (int i = 0; i < FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().Count; i++)
                {
                    if(_feedbackSelection[i])
                    {
                        FeedbackEditor.FeedbackDB.DeleteFeedback(FeedbackEditor.FeedbackDB.ReturnFeedbackID(i));
                    }
                }

                FeedbackEditor.FeedbackDB.ClearAll();

                // Get all the feedback
                FeedbackEditor.FeedbackDB.GetAllFeedback();

            }
            if(GUILayout.Button("BACK"))
            {
                _deleteFeedback = false;
                _isSelectionSet = false;
            }

            GUILayout.EndHorizontal();
        }

        void EditFeedback()
        {

            

            GUILayout.Label("Edit Feedback", EditorStyles.boldLabel);

            if (FeedbackDB.ReturnAllFeedbackText().Count > 0)
            {
                _feedbackSelectIndex = EditorGUILayout.Popup(_feedbackSelectIndex, FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().ToArray());

                if (!_isDataLoaded)
                {

                    FeedbackEditor.FeedbackDB.ClearAll();
                    FeedbackEditor.FeedbackDB.GetAllFeedback();

                    if (FeedbackEditor.FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Hint")
                    {
                        _feedbackType = FeedbackType.Hint;
                    }
                    
                    if (FeedbackEditor.FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Achievement")
                    {
                        _feedbackType = FeedbackType.Achievement;
                    }

                    _isDataLoaded = true;
                }

                if (GUI.changed)
                {
                    _isDataLoaded = false;
                }

                _feedbackType = (FeedbackType)EditorGUILayout.EnumPopup("Type of Feedback: ", _feedbackType);

                if (_feedbackType == FeedbackType.Hint)
                {
                    EditHint(_feedbackSelectIndex);

                }
                if(_feedbackType == FeedbackType.Achievement)
                {
                    EditAchievement(_feedbackSelectIndex);
                }
            }
            else
            {
                GUILayout.Label("No records found!");
            }
            if(GUILayout.Button("BACK"))
            {
                _editFeedback = false;
                _isDataLoaded = false;
            }
        }

        void EditHint(int _id)
        {
            if (!_isHintDataLoaded)
            {
                if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_id) == "Game_Start")
                {
                    _feedbackTrigger = FeedbackTrigger.Game_Start;
                }

                if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_id) == "Time")
                {
                    _feedbackTrigger = FeedbackTrigger.Time;
                    _timer = FeedbackEditor.FeedbackDB.ReturnFeedbackTimer(_id);
                }

                if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_id) == "Trigger")
                {
                    _feedbackTrigger = FeedbackTrigger.Trigger;
                    
                    if(FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_id) == "Capsule")
                    {
                        _triggerShape = TriggerShape.Capsule;
                    }

                    if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_id) == "Sphere")
                    {
                        _triggerShape = TriggerShape.Sphere;
                    }

                    if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_id) == "Square")
                    {
                        _triggerShape = TriggerShape.Square;
                    }

                    _triggerSize = FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerSize(_id);

                    Debug.Log(_feedbackTrigger);

                }
               

                _feedbackText = FeedbackEditor.FeedbackDB.ReturnFeedbackText(_id);
                _isHintDataLoaded = true;

            }
            if(GUI.changed)
            {
                _isHintDataLoaded = false;
            }
            _feedbackTrigger = (FeedbackTrigger)EditorGUILayout.EnumPopup("Trigger Hint by: ", _feedbackTrigger);

            if (_feedbackTrigger != FeedbackTrigger.None)
            {
                if (_feedbackTrigger == FeedbackTrigger.Time)
                {
                    _timer = EditorGUILayout.FloatField("After how much time: ", _timer);
                }

                if (_feedbackTrigger == FeedbackTrigger.Trigger)
                {
                    _triggerShape = (TriggerShape)EditorGUILayout.EnumPopup("Shape of Trigger: ", _triggerShape);
                    _triggerSize = EditorGUILayout.IntField(_triggerSize);
                }

                GUILayout.Label("The Hint text", EditorStyles.boldLabel);
                _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Save to Database"))
                {
                    FeedbackEditor.FeedbackDB.UpdateFeedback(FeedbackDB.ReturnFeedbackID(_id), _feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, 0);
                    _editFeedback = false;
                }
                

                EditorGUILayout.EndHorizontal();

            }
        }

        void EditConditionalHint(int _id)
        {

        }

        void EditAchievement(int _id)
        {
            if(!_isAchievementDataLoaded)
            {
                
                if(FeedbackDB.ReturnFeedbackAchievement(_id) == "Gold_Collected")
                {
                    _achievementType = AchievementType.Gold_Collected;
                }

                if (FeedbackDB.ReturnFeedbackAchievement(_id) == "Level_Gained")
                {
                    _achievementType = AchievementType.Level_Gained;
                }

                if (FeedbackDB.ReturnFeedbackAchievement(_id) == "Quests_Completed")
                {
                    _achievementType = AchievementType.Quests_Completed;
                }

                _achievementAmount = FeedbackDB.ReturnAchievementAmount(_id);
                _feedbackText = FeedbackDB.ReturnFeedbackText(_id);
                _isAchievementDataLoaded = true;
                     
            }

            _achievementType = (AchievementType)EditorGUILayout.EnumPopup("Achievement for: ", _achievementType);

            _achievementAmount = EditorGUILayout.IntField("Amount needed: ", _achievementAmount);

            _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save to Database"))
            {
                FeedbackEditor.FeedbackDB.UpdateFeedback(FeedbackDB.ReturnFeedbackID(_id), _feedbackType.ToString(), "None", 0, 0, "None", _feedbackText, null, _achievementType.ToString(), 0, _achievementAmount);
                _editFeedback = false;
                _isDataLoaded = false;
                _isAchievementDataLoaded = false;
                ClearAll();
            }
            if(GUILayout.Button("BACK"))
            {
                _editFeedback = false;
                _isDataLoaded = false;
                _isAchievementDataLoaded = false;
                ClearAll();
            }

            EditorGUILayout.EndHorizontal();
        }

        void EditGameFeedback()
        {

            Feedback[] _editAllFeedback = GameObject.FindObjectsOfType<Feedback>();
            List<string> _editAllFeedbackText = new List<string>();

            for (int i = 0; i < _editAllFeedback.Length; i++)
            {
                _editAllFeedbackText.Add(_editAllFeedback[i].ReturnFeedbackText());
            }

            _feedbackSelectIndex = EditorGUILayout.Popup(_feedbackSelectIndex, _editAllFeedbackText.ToArray());
            if (!_isDataLoaded)
            {
                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackType() == "Hint")
                {
                    _feedbackType = FeedbackType.Hint;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackType() == "Achievement")
                {
                    _feedbackType = FeedbackType.Achievement;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Game_Start")
                {
                    _feedbackTrigger = FeedbackTrigger.Game_Start;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Time")
                {
                    _feedbackTrigger = FeedbackTrigger.Time;
                }


                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Trigger")
                {
                    _feedbackTrigger = FeedbackTrigger.Trigger;
                }


                

                if (_feedbackTrigger == FeedbackTrigger.Time)
                {
                    _timer = EditorGUILayout.FloatField("After how much time: ", _timer);
                }

                if (_feedbackTrigger == FeedbackTrigger.Trigger)
                {
                    _triggerShape = (TriggerShape)EditorGUILayout.EnumPopup("Shape of Trigger: ", _triggerShape);
                    _triggerSize = EditorGUILayout.IntField(_triggerSize);
                }

                _achievementAmount = _editAllFeedback[_feedbackSelectIndex].ReturnAchievementAmount();

                _feedbackText = _editAllFeedback[_feedbackSelectIndex].ReturnFeedbackText();
                _isDataLoaded = true;
            }

            _feedbackType = (FeedbackType)EditorGUILayout.EnumPopup("Type of Feedback: ", _feedbackType);
            _feedbackTrigger = (FeedbackTrigger)EditorGUILayout.EnumPopup("Trigger Hint by: ", _feedbackTrigger);

            GUILayout.Label("The Hint text", EditorStyles.boldLabel);
            _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("SAVE"))
            {
                FeedbackEditor.FeedbackDB.UpdateFeedback(_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackID(), _feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, _achievementAmount);
                _editAllFeedback[_feedbackSelectIndex].SetValues(_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackID(), _feedbackType.ToString(), _timer, 0, _feedbackText, null, _achievementType.ToString(), _feedbackTrigger.ToString(), _achievementAmount);
                _editGameFeedback = false;
            }

            if (GUILayout.Button("BACK"))
            {
                _editGameFeedback = false;
            }
            EditorGUILayout.EndHorizontal();
        }

        void ClearAll()
        {
            _feedbackType = FeedbackType.None;
            _feedbackTrigger = FeedbackTrigger.None;
            _hintCondition = HintCondition.None;
            _achievementType = AchievementType.None;
            _triggerShape = TriggerShape.None;
            _timer = 0.0f;
            _triggerSize = 0;
            _feedbackText = "";

            _isDataLoaded = false;
            _isAchievementDataLoaded = false;
        }

    }
}
