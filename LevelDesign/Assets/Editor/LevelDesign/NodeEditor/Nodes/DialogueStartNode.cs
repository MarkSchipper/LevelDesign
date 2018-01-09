using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class DialogueStartNode : BaseInputNode {

    private string _title;
    private int _previousNode = -1;
    private int _conversationID;

    public DialogueStartNode()
    {
        windowTitle = "Dialogue Start Node";
        hasInputs = false;
        hasOutputs = true;

        //_conversationID = 1;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();
        Event e = Event.current;
        GUILayout.Label("Title");
        _title = EditorGUILayout.TextField(_title);

    }


    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void SetConversationID(int _id)
    {
        _conversationID = _id;
    }

    public override bool ReturnHasInputs()
    {
        return hasInputs;
    }

    public override bool ReturnHasOutputs()
    {
        return hasOutputs;
    }

    public override int ReturnConversationID()
    {
        return _conversationID;
    }

    public override void SetTitle(string _text)
    {
        _title = _text;

    }

    public override string ReturnTitle()
    {
        return _title;
    }

    public override int ReturnQuestID()
    {
        return -1;
    }

    public override int ReturnPreviousNode()
    {
        return _previousNode;
    }



}
