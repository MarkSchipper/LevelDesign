using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestNode : BaseInputNode
{

    private string _response;
    private int _conversationID;
    private int _previousNode;
    private int _npcID;
    private bool _correctAnswer;
    private string _title;

    private int _selectIndex;
    private List<string> _questNames;
    private int _qID;
    private string _questText;
    private string _questComplete;

    public QuestNode(int _id)
    {

        Quest.QuestDatabaseManager.ClearAll();

        windowTitle = "Quest Node";
        hasInputs = true;
        hasOutputs = true;
        _title = "Add Quest";
        _npcID = _id;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();
        
            // Fetch all the quests
            Quest.QuestDatabaseManager.GetAllQuests();

        _selectIndex = EditorGUILayout.Popup(_selectIndex, Quest.QuestDatabaseManager.ReturnAllQuestTitles().ToArray());

            _qID = Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectIndex];

            // Display the Title of the quest
            GUILayout.Label("Quest Title", EditorStyles.boldLabel);
            _questNames = Quest.QuestDatabaseManager.ReturnAllQuestTitles();

            // Create the popup in the window to select the quest
            _selectIndex = EditorGUILayout.Popup(_selectIndex, _questNames.ToArray());
            
            // Display the text of the quest
            GUILayout.Label("Quest Text", EditorStyles.boldLabel);
            _questText = EditorGUILayout.TextArea(Quest.QuestDatabaseManager.ReturnAllQuestTexts()[_selectIndex], GUILayout.Width(230), GUILayout.Height(60));

            // Display the quest complete text
            GUILayout.Label("Quest Complete Text", EditorStyles.boldLabel);
            _questComplete = EditorGUILayout.TextArea(Quest.QuestDatabaseManager.ReturnAllQuestCompleteTexts()[_selectIndex], GUILayout.Width(230), GUILayout.Height(60));

            if (GUILayout.Button("Save Changes"))
            {
                Quest.QuestDatabaseManager.SaveQuestFromNode(Quest.QuestDatabaseManager.ReturnAllQuestID()[_selectIndex], Quest.QuestDatabaseManager.ReturnAllQuestTitles()[_selectIndex], Quest.QuestDatabaseManager.ReturnAllQuestTexts()[_selectIndex], Quest.QuestDatabaseManager.ReturnAllQuestCompleteTexts()[_selectIndex]);
            }
        

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
        return _qID;
    }

}
