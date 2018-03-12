using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueQuestComplete : MonoBehaviour
    {
        public static void QuestComplete(int _npcID, int _nodeID, GameObject _selectedNPC, int _answer)
        {
            Quest.QuestGameManager.GetAllQuests();
            // check if quest is active
            if (Quest.QuestGameManager.ReturnQuestComplete(Quest.QuestGameManager.ReturnQuestID()[int.Parse(Dialogue.Game.DialogueGameDatabase.GetConditionValue(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]))]))
            {
                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) Correct answer
                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[_answer], "True");

                // Set the Question
                Dialogue.DialogueManager.instance.SetQuestText(_selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetCurrentQuestion(_npcID, _nodeID, "True"));

                // Fetch the corresponding answers
                Dialogue.DialogueManager.instance.SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, _nodeID));
                Dialogue.DialogueManager.instance.SetNodeID(_nodeID);
            }
            else
            {
                // Set the _nodeID to the NodeID of the Condition to get the ( in this case ) False answer
                _nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[_answer], "False");
                if (Dialogue.Game.DialogueGameDatabase.GetNextQuestion(_npcID, _nodeID) == string.Empty)
                {
                    Dialogue.DialogueManager.instance.SetQuestText(_selectedNPC.GetComponent<NPC.NpcSystem>().ReturnNpcName() + ": " + Dialogue.Game.DialogueGameDatabase.GetCurrentQuestion(_npcID, _nodeID, "False"));
                }
                // Set the Question


                // Fetch the corresponding answers
                //_nodeID = Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, Dialogue.Game.DialogueGameDatabase.GetNodeIDsByAnswer(_npcID, _nodeID)[0]);
                Dialogue.DialogueManager.instance.SetAnswers(Dialogue.Game.DialogueGameDatabase.GetAnswersByQuestion(_npcID, _nodeID));
                //Debug.Log(Dialogue.Game.DialogueGameDatabase.GetNextNodeID(_npcID, _nodeID));
                Dialogue.DialogueManager.instance.SetNodeID(_nodeID);
            }
        }
    }
}
