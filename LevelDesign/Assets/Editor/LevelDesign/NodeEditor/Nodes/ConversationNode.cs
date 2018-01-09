using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConversationNode : BaseInputNode
{

    public string _question;
    private int _conversationID;
    private int _previousNode;
    private int _npcID;
    private bool _correctAnswer;

    public ConversationNode()
    {
        windowTitle = "Question to ask";
        hasInputs = true;
        hasOutputs = false;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();
        
        Event e = Event.current;
        GUILayout.Label("Question:");
        _question = GUILayout.TextArea(_question, GUILayout.Height(60));

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

    public override void SetQuestion(string _text)
    {
        _question = _text;

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

 
}
