using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EndNode : BaseInputNode
{

    private string _response;
    private int _conversationID;
    private int _previousNode;
    private int _npcID;
    private bool _correctAnswer;
    private string _title;

    public EndNode()
    {
        windowTitle = "End Node";
        hasInputs = true;
        hasOutputs = false;
        _title = "End";
    }

    public override void DrawWindow()
    {
        base.DrawWindow();
        
        
    }


    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void Tick(float deltaTime)
    {

    }

    public override string ReturnTitle()
    {
        return _title;
    }

    public override bool ReturnHasInputs()
    {
        return hasInputs;
    }

    public override bool ReturnHasOutputs()
    {
        return hasOutputs;
    }

    public override void SetResponse(string _text)
    {
        _response = _text;
    }

    public override string ReturnResponse()
    {
        return _response;
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

    public override void SetAnswer(bool _set)
    {
        _correctAnswer = _set;
    }

    public override bool ReturnCorrectAnswer()
    {
        return _correctAnswer;
    }

    public override void SetNpcID(int _id)
    {
        _npcID = _id;
    }

    public override int ReturnNpcID()
    {
        return _npcID;
    }

    public override int ReturnQuestID()
    {
        return -1;
    }

}
