using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Feedback_Game : MonoBehaviour {

    private static int _feedbackSelectIndex;
    private static bool _isDataLoaded;
    private static FeedbackType _type = FeedbackType.None;
    private static FeedbackTrigger _trigger = FeedbackTrigger.None;
    private static TriggerShape _shape = TriggerShape.None;
    private static float _timer;
    private static int _triggerSize;
    private static int _achievementAmount;
    private static string _feedbackText;

    public static void ShowAddGame()
    {

        FeedbackEditor.FeedbackDB.ClearAll();
        FeedbackEditor.FeedbackDB.GetAllFeedback();

        _feedbackSelectIndex = EditorGUILayout.Popup(_feedbackSelectIndex, FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().ToArray());

        if (GUILayout.Button("Add " + FeedbackEditor.FeedbackDB.ReturnFeedbackText(_feedbackSelectIndex) + " to the game"))
        {

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Hint")
            {
                _type = FeedbackType.Hint;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackType(_feedbackSelectIndex) == "Achievement")
            {
                _type = FeedbackType.Achievement;
            }


            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Game_Start")
            {
                _trigger = FeedbackTrigger.Game_Start;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Time")
            {
                _trigger = FeedbackTrigger.Time;
            }
            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTrigger(_feedbackSelectIndex) == "Trigger")
            {
                _trigger = FeedbackTrigger.Trigger;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Capsule")
            {
                _shape = TriggerShape.Capsule;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Sphere")
            {
                _shape = TriggerShape.Sphere;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_feedbackSelectIndex) == "Square")
            {
                _shape = TriggerShape.Square;
            }


            //AddToGame(FeedbackDB.ReturnFeedbackID(_feedbackSelectIndex), _type, _trigger, FeedbackDB.ReturnFeedbackTimer(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackIdleTimer(_feedbackSelectIndex), _shape, FeedbackDB.ReturnFeedbackText(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackCondition(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackAchievement(_feedbackSelectIndex), FeedbackDB.ReturnFeedbackTriggerSize(_feedbackSelectIndex), FeedbackDB.ReturnAchievementAmount(_feedbackSelectIndex));
        }
    }

    static void AddToGame(int _id, FeedbackType _type, FeedbackTrigger _trigger, float _time, float _idleTime, TriggerShape _shape, string _text, string _condition, string _achievement, int _triggerSize, int _amount)
    {

        if (GameObject.Find("PLAYERFEEDBACK") == null)
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

        Transform _feedbackChild = _feedback.GetComponentInChildren<Transform>(true);

        if (_trigger == FeedbackTrigger.Trigger)
        {

            foreach (Transform child in _feedbackChild)
            {

                child.transform.localScale = new Vector3(_triggerSize * _triggerSize, _triggerSize * _triggerSize, _triggerSize * _triggerSize);
                if (_shape == TriggerShape.Capsule)
                {
                    child.gameObject.AddComponent<CapsuleCollider>();
                    child.GetComponent<CapsuleCollider>().radius = _triggerSize;
                    child.GetComponent<CapsuleCollider>().isTrigger = true;
                    child.gameObject.layer = 2;

                }
                if (_shape == TriggerShape.Sphere)
                {
                    child.gameObject.AddComponent<SphereCollider>();
                    child.GetComponent<SphereCollider>().radius = _triggerSize;
                    child.GetComponent<SphereCollider>().isTrigger = true;
                    child.gameObject.layer = 2;
                }

                if (_shape == TriggerShape.Square)
                {
                    child.gameObject.AddComponent<BoxCollider>();
                    child.GetComponent<BoxCollider>().isTrigger = true;
                    child.gameObject.layer = 2;
                }
            }
        }

        _feedback.GetComponentInChildren<FeedbackEditor.Feedback>().SetValues(_id, _type.ToString(), _time, _idleTime, _text, _condition, _achievement, _trigger.ToString(), _amount);

    }
    
    public static void ShowEditGame()
    {

        FeedbackEditor.Feedback[] _editAllFeedback = GameObject.FindObjectsOfType<FeedbackEditor.Feedback>();
        List<string> _editAllFeedbackText = new List<string>();

        if (_editAllFeedback.Length > 0)
        {
            for (int i = 0; i < _editAllFeedback.Length; i++)
            {
                _editAllFeedbackText.Add(_editAllFeedback[i].ReturnFeedbackText());
            }

            _feedbackSelectIndex = EditorGUILayout.Popup(_feedbackSelectIndex, _editAllFeedbackText.ToArray());
            if (!_isDataLoaded)
            {
                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackType() == "Hint")
                {
                    _type = FeedbackType.Hint;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackType() == "Achievement")
                {
                    _type = FeedbackType.Achievement;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Game_Start")
                {
                    _trigger = FeedbackTrigger.Game_Start;
                }

                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Time")
                {
                    _trigger = FeedbackTrigger.Time;
                }


                if (_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackTrigger() == "Trigger")
                {
                    _trigger = FeedbackTrigger.Trigger;
                }




                if (_trigger == FeedbackTrigger.Time)
                {
                    _timer = EditorGUILayout.FloatField("After how much time: ", _timer);
                }

                if (_trigger == FeedbackTrigger.Trigger)
                {
                    _shape = (TriggerShape)EditorGUILayout.EnumPopup("Shape of Trigger: ", _shape);
                    _triggerSize = EditorGUILayout.IntField(_triggerSize);
                }

                _achievementAmount = _editAllFeedback[_feedbackSelectIndex].ReturnAchievementAmount();

                _feedbackText = _editAllFeedback[_feedbackSelectIndex].ReturnFeedbackText();
                _isDataLoaded = true;
            }

            _type = (FeedbackType)EditorGUILayout.EnumPopup("Type of Feedback: ", _type);
            _trigger = (FeedbackTrigger)EditorGUILayout.EnumPopup("Trigger Hint by: ", _trigger);

            GUILayout.Label("The Hint text", EditorStyles.boldLabel);
            _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("SAVE"))
            {
                FeedbackEditor.FeedbackDB.UpdateFeedback(_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackID(), _type.ToString(), _trigger.ToString(), _timer, 0, _shape.ToString(), _feedbackText, null, _type.ToString(), _triggerSize, _achievementAmount);
                _editAllFeedback[_feedbackSelectIndex].SetValues(_editAllFeedback[_feedbackSelectIndex].ReturnFeedbackID(), _type.ToString(), _timer, 0, _feedbackText, null, _type.ToString(), _trigger.ToString(), _achievementAmount);
            }


            EditorGUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("No Feedback in Game");
        }
    }
    
}
