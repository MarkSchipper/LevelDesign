using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeedbackEditor
{

    public class Feedback : MonoBehaviour
    {
        [SerializeField]
        private int _feedbackID;
        [SerializeField]
        private string _feedbackType;
        [SerializeField]
        private float _feedbackTimer;
        [SerializeField]
        private float _feedbackIdleTimer;
        [SerializeField]
        private string _feedbackText;
        [SerializeField]
        private string _feedbackCondition;
        [SerializeField]
        private string _feedbackAchievement;

        [SerializeField]
        private string _feedbackTrigger;

        [SerializeField]
        private int _feedbackAchievementAmount;

        [SerializeField]
        private bool _showHintOnce = false;

        [SerializeField]
        private int _timesShown;

        // Use this for initialization
        void Start()
        {
            if (_feedbackTrigger == "Game_Start")
            {
                Dialogue.DialogueManager.instance.ShowHint(_feedbackText, true);
            }

            if(_feedbackTrigger == "Time")
            {
                StartCoroutine(StartTimer(_feedbackTimer));
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetValues(int _id, string _type, float _time, float _idleTime, string _text, string _condition, string _achievement, string _trigger, int _amount)
        {
            _feedbackID = _id;
            _feedbackType = _type;
            _feedbackTimer = _time;
            _feedbackIdleTimer = _idleTime;
            _feedbackText = _text;
            _feedbackCondition = _condition;
            _feedbackAchievement = _achievement;
            _feedbackTrigger = _trigger;
            _feedbackAchievementAmount = _amount;
        }

        void OnTriggerEnter(Collider coll)
        {
            if (_feedbackTrigger != "Game_Start")
            {
                if (coll.tag == "Player")
                {

                    // Credits to Alex Mazur for this contribution

                    if (_showHintOnce)
                    {
                        if (_timesShown == 0)
                        {
                            Dialogue.DialogueManager.instance.ShowHint(_feedbackText, true);
                            _timesShown++;
                        }
                        
                    }
                    else
                    { 

                        Dialogue.DialogueManager.instance.ShowHint(_feedbackText, true);
                    }
                }
            }
        }

        public int ReturnFeedbackID()
        {
            return _feedbackID;
        }

        public string ReturnFeedbackType()
        {
            return _feedbackType;
        }

        public string ReturnFeedbackText()
        {
            return _feedbackText;
        }

        public float ReturnFeedbackTimer()
        {
            return _feedbackTimer;
        }

        public float ReturnIdleTimer()
        {
            return _feedbackIdleTimer;
        }

        public string ReturnFeedbackCondition()
        {
            return _feedbackCondition;
        }

        public string ReturnFeedbackAchievement()
        {
            return _feedbackAchievement;
        }

        public string ReturnFeedbackTrigger()
        {
            return _feedbackTrigger;
        }

        public int ReturnAchievementAmount()
        {
            return _feedbackAchievementAmount;
        }

        IEnumerator StartTimer(float _time)
        {
            yield return new WaitForSeconds(_time);
            Dialogue.DialogueManager.instance.ShowHint(_feedbackText, true);
        }

    }
}
