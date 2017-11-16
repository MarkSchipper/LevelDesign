using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

public class Feedback_DB : MonoBehaviour {

    private static FeedbackTrigger _feedbackTrigger;
    private static TriggerShape _triggerShape;
    private static FeedbackType _feedbackType;
    private static AchievementType _achievementType;
    private static int _triggerSize;
    private static string _feedbackText;
    private static float _timer;
    private static int _achievementAmount;
    private static bool _isDataLoaded;
    private static bool _isHintDataLoaded;
    private static bool _isAchievementDataLoaded;
    private static int _feedbackSelectIndex;
    private static bool _deleteConfirmation;
    private static int _deleteIndex;
    private static bool _deletedFeedback;

    public static void ShowAddFeedback()
    {
        _isDataLoaded = false;
        _deleteConfirmation = false;
        _deletedFeedback = false;
        _feedbackType = (FeedbackType)EditorGUILayout.EnumPopup("Feedback Type: ", _feedbackType);
        switch (_feedbackType)
        {
            case FeedbackType.None:
                break;
            case FeedbackType.Hint:
                ShowAddHint();
                break;
            case FeedbackType.Achievement:
                ShowAddAchievement();
                break;
            default:
                break;
        }
    }

    static void ShowAddHint()
    {
        _feedbackType = FeedbackType.Hint;
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
                FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, null, _triggerSize, 0);
            }

            EditorGUILayout.EndHorizontal();

        }
    }

    static void ShowAddAchievement()
    {
        _achievementType = (AchievementType)EditorGUILayout.EnumPopup("Achievement for: ", _achievementType);

        if (_achievementType != AchievementType.None)
        {
            _achievementAmount = EditorGUILayout.IntField("Amount needed: ", _achievementAmount);

            _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to Database"))
            {
                FeedbackEditor.FeedbackDB.SaveFeedback(_feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, _achievementAmount);
            }
            EditorGUILayout.EndHorizontal();

        }

    }

    public static void ShowEditFeedback()
    {
        _deleteConfirmation = false;
        _deletedFeedback = false;
        GUILayout.Label("Edit Feedback", EditorStyles.boldLabel);

        if (FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().Count > 0)
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
            if (_feedbackType == FeedbackType.Achievement)
            {
                EditAchievement(_feedbackSelectIndex);
            }
        }
        else
        {
            GUILayout.Label("No records found!");
        }
        
    }

    static void EditHint(int _id)
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

                if (FeedbackEditor.FeedbackDB.ReturnFeedbackTriggerShape(_id) == "Capsule")
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
        if (GUI.changed)
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
                FeedbackEditor.FeedbackDB.UpdateFeedback(FeedbackEditor.FeedbackDB.ReturnFeedbackID(_id), _feedbackType.ToString(), _feedbackTrigger.ToString(), _timer, 0, _triggerShape.ToString(), _feedbackText, null, _achievementType.ToString(), _triggerSize, 0);
            }


            EditorGUILayout.EndHorizontal();

        }
    }

    static void EditAchievement(int _id)
    {
        if (!_isAchievementDataLoaded)
        {

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackAchievement(_id) == "Gold_Collected")
            {
                _achievementType = AchievementType.Gold_Collected;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackAchievement(_id) == "Level_Gained")
            {
                _achievementType = AchievementType.Level_Gained;
            }

            if (FeedbackEditor.FeedbackDB.ReturnFeedbackAchievement(_id) == "Quests_Completed")
            {
                _achievementType = AchievementType.Quests_Completed;
            }

            _achievementAmount = FeedbackEditor.FeedbackDB.ReturnAchievementAmount(_id);
            _feedbackText = FeedbackEditor.FeedbackDB.ReturnFeedbackText(_id);
            _isAchievementDataLoaded = true;

        }

        _achievementType = (AchievementType)EditorGUILayout.EnumPopup("Achievement for: ", _achievementType);

        _achievementAmount = EditorGUILayout.IntField("Amount needed: ", _achievementAmount);

        _feedbackText = EditorGUILayout.TextArea(_feedbackText, GUILayout.Width(400), GUILayout.Height(100));

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save to Database"))
        {
            FeedbackEditor.FeedbackDB.UpdateFeedback(FeedbackEditor.FeedbackDB.ReturnFeedbackID(_id), _feedbackType.ToString(), "None", 0, 0, "None", _feedbackText, null, _achievementType.ToString(), 0, _achievementAmount);
            _isDataLoaded = false;
            _isAchievementDataLoaded = false;
            ClearAll();
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void ShowDeleteFeedback()
    {
        GUILayout.Label("Delete Feedback");

        if(!_deleteConfirmation)
        {
            for (int i = 0; i < FeedbackEditor.FeedbackDB.ReturnAllFeedbackText().Count; i++)
            {
                if (GUILayout.Button("Delete " + FeedbackEditor.FeedbackDB.ReturnFeedbackText(i)))
                {
                    _deleteConfirmation = true;
                    _deleteIndex = i;
                }
            }
        }
        if(_deleteConfirmation && !_deletedFeedback)
        {
            if(GUILayout.Button("Are you sure you want to delete " + FeedbackEditor.FeedbackDB.ReturnFeedbackText(_deleteIndex)))
            {
                _deletedFeedback = true;
            }
        }
        if(_deletedFeedback)
        {
            FeedbackEditor.FeedbackDB.DeleteFeedback(FeedbackEditor.FeedbackDB.ReturnFeedbackID(_deleteIndex));
            GUILayout.Label("Feedback deleted");
            FeedbackEditor.FeedbackDB.ClearAll();

            FeedbackEditor.FeedbackDB.GetAllFeedback();
        }
    }

    public static void ClearAll()
    {
        _feedbackType = FeedbackType.None;
        _feedbackTrigger = FeedbackTrigger.None;
     //   _hintCondition = HintCondition.None;
        _achievementType = AchievementType.None;
        _triggerShape = TriggerShape.None;
        _timer = 0.0f;
        _triggerSize = 0;
        _feedbackText = "";

        _isDataLoaded = false;
        _isAchievementDataLoaded = false;
    }

}
