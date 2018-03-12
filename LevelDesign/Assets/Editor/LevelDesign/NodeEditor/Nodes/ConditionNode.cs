using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Condition
{
    None,
    If,
    IfNot,
}

public enum ConditionTerms
{
    None,
    Greater,
    Less,
    Equal,
}

public enum ConditionStatements
{
    None,
    QuestActive,
    QuestCompleted,
    PlayerLevel,
    ZoneVisited,
}

public class ConditionNode : BaseInputNode {

    public string _question;
    private string _title;
    private int _conversationID;
    private int _previousNode;
    private int _npcID;
    private bool _correctAnswer;
    private ConditionStatements _statementIndex;
    private Condition _conditionIndex;
    private ConditionTerms _termsIndex;

    private string _value;

    private bool _loadedQuests;
    private bool _loadedZones;

    private int _questIndex;
    private int _zoneIndex;

    private List<int> _questID = new List<int>();
    private List<string> _questTitle = new List<string>();
    private List<string> _zones = new List<string>();

    public ConditionNode()
    {
        windowTitle = "Condition Node";
        hasInputs = true;
        hasOutputs = true;
        _title = "ConditionNode";
        
    }

    void OnEnable()
    {

    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        if (!_loadedQuests)
        {
            if (_questID.Count > 0)
            {
                _questID.Clear();
                _questTitle.Clear();
            }
            Quest.QuestDatabaseManager.GetAllQuests();
            for (int i = 0; i < Quest.QuestDatabaseManager.ReturnAllQuestTitles().Count; i++)
            {
                _questID.Add(Quest.QuestDatabaseManager.ReturnAllQuestID()[i]);
                _questTitle.Add(Quest.QuestDatabaseManager.ReturnAllQuestTitles()[i]);
            }
            _loadedQuests = true;
        }

        if (!_loadedZones)
        {
            foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Zone"))
            {
                _zones.Add(zone.name);
            }
        }
        Event e = Event.current;

        _conditionIndex = (Condition)EditorGUILayout.EnumPopup(_conditionIndex);

        if (_conditionIndex != Condition.None)
        {
            _statementIndex = (ConditionStatements)EditorGUILayout.EnumPopup(_statementIndex);
            if (_statementIndex == ConditionStatements.QuestActive || _statementIndex == ConditionStatements.QuestCompleted)
            {
                GUILayout.Label("Which Quest?");
                _questIndex = EditorGUILayout.Popup(_questIndex, _questTitle.ToArray());
            }

            if (_statementIndex == ConditionStatements.ZoneVisited)
            {
                GUILayout.Label("Which Zone?");
                _zoneIndex = EditorGUILayout.Popup(_zoneIndex, _zones.ToArray());
            }

            if (_statementIndex == ConditionStatements.PlayerLevel)
            {
                _termsIndex = (ConditionTerms)EditorGUILayout.EnumPopup(_termsIndex);

                if (_termsIndex != ConditionTerms.None)
                {
                    _value =  EditorGUILayout.TextField("Value: ", _value, GUILayout.Width(60));
                }
            }
        }
    }

    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void Tick(float deltaTime)
    {

    }

    public override bool ReturnHasInputs()
    {
        return hasInputs;
    }

    public override bool ReturnHasOutputs()
    {
        return hasOutputs;
    }

    public override void SetConversationID(int _id)
    {
        _conversationID = _id;
    }

    public override int ReturnConversationID()
    {
        return _conversationID;
    }

    public override void SetPreviousNode(int _id)
    {
        _previousNode = _id;
    }

    public override int ReturnPreviousNode()
    {
        return _previousNode;
    }

    public override void SetNpcID(int _id)
    {
        _npcID = _id;
    }

    public override int ReturnNpcID()
    {
        return _npcID;
    }

    public override void SetTitle(string _text)
    {
        _title = _text;
    }

    public override string ReturnTitle()
    {
        return _title;
    }

    public override void SetAnswer(bool _set)
    {
        _correctAnswer = _set;
    }

    public override string ReturnQuestion()
    {
        return _question;
    }

    public override bool ReturnCorrectAnswer()
    {
        return _correctAnswer;
    }

    public override int ReturnQuestID()
    {
        return -1;
    }

    public override void SetCondition(string _nwCondition)
    {
        switch (_nwCondition)
        {
            case "If":
                _conditionIndex = Condition.If;
                break;
            case "IfNot":
                _conditionIndex = Condition.IfNot;
                break;
            case "None":
                _conditionIndex = Condition.None;
                break;
            default:
                break;
        }
    }

    public override void SetConditionStatement(string _statement)
    {
        switch (_statement)
        {
            case "None":
                _statementIndex = ConditionStatements.None;
                break;
            case "QuestActive":
                _statementIndex = ConditionStatements.QuestActive;
                break;
            case "QuestCompleted":
                _statementIndex = ConditionStatements.QuestCompleted;
                break;
            case "PlayerLevel":
                _statementIndex = ConditionStatements.PlayerLevel;
                break;
            case "ZoneVisited":
                _statementIndex = ConditionStatements.ZoneVisited;
                break;
            default:
                break;
        }
    }

    public override void SetConditionTerm(string _term)
    {
        switch (_term)
        {
            case "None":
                _termsIndex = ConditionTerms.None;
                break;
            case "Greater":
                _termsIndex = ConditionTerms.Greater;
                break;
            case "Less":
                _termsIndex = ConditionTerms.Less;
                break;
            case "Equal":
                _termsIndex = ConditionTerms.Equal;
                break;
            default:
                break;
        }
    }

    public override void SetConditionValue(string _nwValue)
    {
        _value = _nwValue;
    }

    public override string ReturnCondition()
    {
        return _conditionIndex.ToString();
    }

    public override string ReturnConditionStatement()
    {
        return _statementIndex.ToString();
    }

    public override string ReturnConditionTerm()
    {
        return _termsIndex.ToString();
    }

    public override string ReturnConditionValue()
    {
        if (_statementIndex == ConditionStatements.QuestActive || _statementIndex == ConditionStatements.QuestCompleted)
        {
            return _questIndex.ToString();
        }
        else if(_statementIndex == ConditionStatements.ZoneVisited)
        {
            return _zones[_zoneIndex];
        }
        else
        {
            return _value.ToString();
        }
        
    }

}
