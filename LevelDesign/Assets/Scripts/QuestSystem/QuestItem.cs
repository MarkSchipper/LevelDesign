using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Quest
{
    [System.Serializable]
    public class QuestItem : MonoBehaviour
    {
        private int _amountCollected;

        [SerializeField]
        private int _questID;

        [SerializeField]
        public bool _visibleInGame;

        // Use this for initialization
        void Start()
        {
            

            if (PlayerPrefs.GetString(this.gameObject.name) != "")
            {
                this.gameObject.SetActive(bool.Parse(PlayerPrefs.GetString(this.gameObject.name).ToLower()));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "Player")
            {
                if (Quest.QuestGameManager.ReturnQuestActive(_questID))
                {

                    _amountCollected = Quest.QuestGameManager.ReturnQuestItemsCollected(_questID);

                    if (_amountCollected < Quest.QuestGameManager.ReturnQuestAmount(_questID))
                    {
                        _amountCollected++;
                        Quest.QuestGameManager.AddQuestItemCollected(_questID, _amountCollected);

                        _visibleInGame = false;
                        this.gameObject.SetActive(_visibleInGame);
                        PlayerPrefs.SetString(this.gameObject.name, _visibleInGame.ToString().ToLower());

                        if (_amountCollected == Quest.QuestGameManager.ReturnQuestAmount(_questID))
                        {
                            Quest.QuestGameManager.SetQuestComplete(_questID);
                        }
                        Quest.QuestLog.SetQuestCollection(false);
                    }
                    else
                    {

                    }
                }
            }
        }

        public void SetQuestID(int _id)
        {
            _questID = _id;
        }

        public int ReturnQuestID()
        {
            return _questID;
        }

        public void ClearCache()
        {
            PlayerPrefs.SetString(this.gameObject.name, "True");
        }


    }
    /*
#if UNITY_EDITOR
    [CustomEditor(typeof(QuestItem))]
    public class QuestItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            QuestItem _item = (QuestItem)target;

            base.OnInspectorGUI();
            if(GUILayout.Button("CLEAR CACHE"))
            {
                _item.ClearCache();
            }
        }
    }
    */
//#endif
}
