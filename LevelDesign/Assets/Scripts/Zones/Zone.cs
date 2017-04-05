using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{

    public class Zone : MonoBehaviour
    {

        [HideInInspector]
        [SerializeField]
        private string _zoneName;

        [HideInInspector]
        [SerializeField]
        private string _zoneDescription;

        [HideInInspector]
        [SerializeField]
        private int _questID;

        

        void OnEnable()
        {
        
        }

        public void SetNames(string _name, string _desc)
        {
            _zoneName = _name;
            _zoneDescription = _desc;
        }

        public string ReturnName()
        {
            return _zoneName;
        }

        public string ReturnDescription()
        {
            return _zoneDescription;
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "Player")
            {
                Dialogue.DialogueManager.SetShowZone(true, _zoneName, _zoneDescription);

                if (_questID > 0)
                {
                    if (Quest.QuestDatabase.ReturnQuestActive(_questID))
                    {
                        if (Quest.QuestDatabase.ReturnZoneAutoComplete(_questID))
                        {
                            // END QUEST
                            Quest.QuestDatabase.SetQuestComplete(_questID);

                            // 

                            Quest.QuestDatabase.FinishQuest(_questID);
                        }
                        else
                        {
                            Quest.QuestDatabase.SetQuestComplete(_questID);
                        }
                    }
                }

            }
        }

        public void SetQuestID(int _id)
        {
            _questID = _id;
        }
    }
}