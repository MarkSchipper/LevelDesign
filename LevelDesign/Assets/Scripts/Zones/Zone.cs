using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{

    public class Zone : MonoBehaviour
    {

       // [HideInInspector]
        [SerializeField]
        private string _zoneName;

        //[HideInInspector]
        [SerializeField]
        private string _zoneDescription;

        //[HideInInspector]
        [SerializeField]
        private int _questID;

        [SerializeField]
        private bool _playerVisited;

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
                if (!_playerVisited)
                {
                    Dialogue.DialogueManager.instance.SetShowZone(true, _zoneName, _zoneDescription);
                }
                _playerVisited = true;

                if (_questID > 0)
                {
                    if (Quest.QuestGameManager.ReturnQuestActive(_questID))
                    {
                        if (Quest.QuestGameManager.ReturnZoneAutoComplete(_questID))
                        {
                            // END QUEST
                            Quest.QuestGameManager.SetQuestComplete(_questID);

                            // 

                            Quest.QuestGameManager.FinishQuest(_questID);
                        }
                        else
                        {
                            Quest.QuestGameManager.SetQuestComplete(_questID);
                        }
                    }
                }
            }
        }

        public void SetQuestID(int _id)
        {
            _questID = _id;

        }

        public bool ReturnPlayerVisited()
        {
            return _playerVisited;
        }
    }
}