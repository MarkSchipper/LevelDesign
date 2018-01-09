using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect windowRect;

    public bool hasInputs = false;
    public bool hasOutputs = false;

    public string windowTitle = "";

    private string title;

    private int _nodeID;
    private int conversationID;
    private int previousNode;
    private string question;
    private string response;
    private int npcID;
    private int questID;
    private string _condition;
    private string _conditionStatement;
    private string _conditionTerm;
    private string _conditionValue;

    private bool correctAnswer;

    public virtual void DrawWindow()
    {
        //windowTitle = EditorGUILayout.TextField("Title", windowTitle);

    }

    public abstract void DrawCurves();

    public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
    {

    }

    public virtual void  NodeDeleted(BaseNode node)
    {

    }

    public virtual BaseInputNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }

    public virtual bool ReturnHasInputs()
    {
        return hasInputs;
    }

    public virtual bool ReturnHasOutputs()
    {
        return hasOutputs;
    }

    public abstract void Tick(float deltaTime);

    public virtual void SetID(int _id)
    {
        if(_id < 0 && _nodeID > 0)
        {
            
            _nodeID += _id;
        }
        _nodeID = _id;
    }

    public virtual int ReturnID()
    {
        return _nodeID;
    }

    public virtual void SetConversationID(int _id)
    {
        conversationID = _id;
    }

    public virtual int ReturnConversationID()
    {
        return conversationID;
    }

    public virtual void SetPreviousNode(int _id)
    {
        previousNode = _id;
    }

    public virtual int ReturnPreviousNode()
    {
        return previousNode;
    }

    public virtual void SetAnswer(bool _set)
    {
        correctAnswer = _set;
    }

    public virtual void SetTitle(string _text)
    {
        title = _text;
    }

    public virtual string ReturnTitle()
    {
        return title;
    }

    public virtual void SetResponse(string _text)
    {
        response = _text;
    }

    public virtual string ReturnResponse()
    {
        return response;
    }

    public virtual void SetNpcID(int _id)
    {
        npcID = _id;
    }

    public virtual int ReturnNpcID()
    {
        return npcID;
    }

    public virtual void SetQuestion(string _text)
    {
        question = _text;
    }

    public virtual string ReturnQuestion()
    {
        return question;
    }

    public virtual bool ReturnCorrectAnswer()
    {
        return correctAnswer;
    }

    public virtual void SetQuestID(int _id)
    {
        questID = _id;
    }

    public virtual int ReturnQuestID()
    {
        return questID;
    }

    public virtual void SetCondition(string _nwCondition)
    {
        _condition = _nwCondition;
    }

    public virtual string ReturnCondition()
    {
        return _condition;
    }

    public virtual void SetConditionStatement(string _statement)
    {
        _conditionStatement = _statement;
    }

    public virtual string ReturnConditionStatement()
    {
        return _conditionStatement;
    }

    public virtual void SetConditionTerm(string _term)
    {
        _conditionTerm = _term;
    }

    public virtual string ReturnConditionTerm()
    {
        return _conditionTerm;
    }

    public virtual void SetConditionValue(string _value)
    {
        _conditionValue = _value;
    }

    public virtual string ReturnConditionValue()
    {
        return _conditionValue;
    }
}
