using UnityEngine;
using System.Collections;

namespace Quest
{

    public class NPC_Trigger : MonoBehaviour
    {
        
        private NPCSystem.NPC _npc;

        void Start()
        {
            
            _npc = this.GetComponentInChildren<NPCSystem.NPC>();
            
        }

        void OnTriggerEnter(Collider coll)
        {


            // IF THE PLAYER HAS SELECTED THE NPC
            if (_npc.ReturnIsSelected())
            {
                if (coll.tag == "Player")
                {
                    Debug.Log(coll.tag);
                    _npc.PlayerInteraction(coll.gameObject, false);

                    if (!_npc.ReturnMetBefore())
                    {
                        // If the NPC is not a quest giver
                        if (!_npc.ReturnQuestGiver())
                        {
                            Debug.Log("test");
                            Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue1(), false, -1, -1);
                        }

                        if (_npc.ReturnQuestGiver() && !Quest.QuestDatabase.GetActiveFromNPC(_npc.ReturnNpcID()))
                        {
                            // IF THE NPC HAS A QUEST
                            Quest.QuestDatabase.GetQuestFromNpc(_npc.ReturnNpcID());
                            Dialogue.DialogueManager.SetDialogue(Quest.QuestDatabase.ReturnQuestTitle(), Quest.QuestDatabase.ReturnQuestText(), true, _npc.ReturnNpcID(), Quest.QuestDatabase.ReturnQuestID());
                        }
                        else
                        {
                            Debug.Log("QUEST IS ACTIVE");
                        }
                        _npc.HasMetPlayer(true);
                        PlayerPrefs.SetString("MetNPC_" + _npc.ReturnNpcName(), "True");
                    }
                    if (_npc.ReturnMetBefore())
                    {

                        //Debug.Log(Quest.QuestDatabase.CheckQuestCompleteNpc(_npc.ReturnNpcID()));

                        if (!_npc.ReturnQuestGiver())
                        {
                            Debug.Log("dialogue2");
                            Dialogue.DialogueManager.SetDialogue("", _npc.ReturnDialogue2(), false, -1, -1);
                            
                        }

                        if (_npc.ReturnQuestGiver() && Quest.QuestDatabase.GetActiveFromNPC(_npc.ReturnNpcID()))
                        {
                            Debug.Log("WE HAVE QUEST");
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
                    }
                }

           }
           
        }

        void OnTriggerExit(Collider coll)
        {
            if (_npc.ReturnPatrol())
            {
                _npc.PlayerInteraction(coll.gameObject, true);
            }
            if(!_npc.ReturnPatrol())
            {
                _npc.PlayerInteraction(coll.gameObject, false);
            }
            _npc.IsSelected(false);
            Dialogue.DialogueManager.ExitDialogue(false);
        }


    }
}