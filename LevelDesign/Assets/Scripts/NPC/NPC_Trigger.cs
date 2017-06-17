using UnityEngine;
using System.Collections;

namespace Quest
{

    public class NPC_Trigger : MonoBehaviour
    {
        
        private NPCSystem.NPC _npc;
        

        void Start()
        {
            
            _npc = this.GetComponentInParent<NPCSystem.NPC>();
            
        }

        void OnTriggerEnter(Collider coll)
        {


            // IF THE PLAYER HAS SELECTED THE NPC
            if (_npc.ReturnIsSelected())
            {
                if (coll.tag == "Player")
                {

                    CombatSystem.PlayerMovement.StopMoving();

                    _npc.PlayerInteraction(coll.gameObject, false);

                    if (!_npc.ReturnMetBefore())
                    {
                        // If the NPC is not a quest giver
                        if (!_npc.ReturnQuestGiver())
                        {
                            Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue1(), false, -1, -1);
                        }

                        if (_npc.ReturnQuestGiver() && !Quest.QuestDatabase.GetActiveFromNPC(_npc.ReturnNpcID()))
                        {
                            if (Quest.QuestDatabase.ReturnQuestTitle() == "")
                            {
                                Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue1(), false, -1, -1);
                            }
                            else {
                                // IF THE NPC HAS A QUEST
                                Quest.QuestDatabase.GetQuestFromNpc(_npc.ReturnNpcID());
                                Dialogue.DialogueManager.SetDialogue(Quest.QuestDatabase.ReturnQuestTitle(), Quest.QuestDatabase.ReturnQuestText(), true, _npc.ReturnNpcID(), Quest.QuestDatabase.ReturnQuestID());
                            }
                        }
                        else
                        {
                            
                        }
                        
                        PlayerPrefs.SetString("MetNPC_" + _npc.ReturnNpcName(), "True");
                    }
                    if (_npc.ReturnMetBefore())
                    {

                        if (!_npc.ReturnQuestGiver())
                        {

                            Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue2(), false, -1, -1);
                            
                        }

                        if (_npc.ReturnQuestGiver() && Quest.QuestDatabase.GetActiveFromNPC(_npc.ReturnNpcID()))
                        {
                            
                            if (!Quest.QuestDatabase.CheckQuestCompleteNpc(_npc.ReturnNpcID()))
                            {

                                
                                Quest.QuestDatabase.GetQuestFromNpc(_npc.ReturnNpcID());
                                Dialogue.DialogueManager.SetDialogue(Quest.QuestDatabase.ReturnQuestTitle(), Quest.QuestDatabase.ReturnQuestText(), false, _npc.ReturnNpcID(), Quest.QuestDatabase.ReturnQuestID());
                            }
                            if (Quest.QuestDatabase.CheckQuestCompleteNpc(_npc.ReturnNpcID()))
                            {

                                Quest.QuestDatabase.GetQuestFromNpc(_npc.ReturnNpcID());
                                Dialogue.DialogueManager.SetDialogue(Quest.QuestDatabase.ReturnQuestTitle(), Quest.QuestDatabase.ReturnQuestCompleteText(), true, _npc.ReturnNpcID(), Quest.QuestDatabase.ReturnQuestID());
                            }
                        }
                        if(_npc.ReturnQuestGiver() && !Quest.QuestDatabase.GetActiveFromNPC(_npc.ReturnNpcID()))
                        {
                            Quest.QuestDatabase.GetQuestFromNpc(_npc.ReturnNpcID());
                            if(Quest.QuestDatabase.ReturnQuestTitle() != null)
                            {
                                
                                Dialogue.DialogueManager.SetDialogue(Quest.QuestDatabase.ReturnQuestTitle(), Quest.QuestDatabase.ReturnQuestText(), true, _npc.ReturnNpcID(), Quest.QuestDatabase.ReturnQuestID());
                            }

                            // If the NPC is a questgiver but the ReturnQuestTitle() == nothing it means that the quest is no longer enabled ( completed ), therefor show the regular text
                            if (Quest.QuestDatabase.ReturnQuestTitle() == null)
                            {
                                Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue2(), false, -1, -1);
                            }
                            // IF THE NPC HAS A QUEST
                            
                        }
                    }
                }

           }
           
        }

        void OnTriggerExit(Collider coll)
        {
            if (coll.tag == "Player")
            {

                if (_npc.ReturnBehaviour() == NPCSystem.ActorBehaviour.Patrol)
                {
                    _npc.PlayerInteraction(null, true);
                    Debug.Log("RETURN TO PATROL");
                }
                if (_npc.ReturnBehaviour() == NPCSystem.ActorBehaviour.Idle)
                {
                    _npc.PlayerInteraction(null, false);
                }

                if (!_npc.ReturnMetBefore())
                {
                    _npc.HasMetPlayer(true);
                }

                _npc.IsSelected(false);
                Dialogue.DialogueManager.ExitDialogue(false);
            }
        }


    }
}