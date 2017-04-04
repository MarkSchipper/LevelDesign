﻿using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace NPCSystem
{
    [System.Serializable]

    public enum ActorBehaviour
    {
        Idle,
        Patrol,
    }

    public class NPC : MonoBehaviour
    {

        private GameObject _player;

        [SerializeField]
        private int _npcID;

        [SerializeField]
        private string _nameToDisplay;
        private float _health;

        [SerializeField]
        private string _npcProfession;

        [SerializeField]
        private bool _interaction;

        [SerializeField]
        private string _dialogue1;

        [SerializeField]
        private string _dialogue2;

        [SerializeField]
        private bool _questGiver;


        [Header("Conversation")]
        public bool _AutoCommunicate;
        private bool _communicate;


        public bool _patrol;
        public float _patrolSpeed;

        private int _currentWayPoint;

        [SerializeField]
        private List<Transform> _wayPoints = new List<Transform>();

        private Vector3 _wayPointTarget;
        private Vector3 _moveDirection;
        private Vector3 _oldPosition;
        private float _distanceTraveled;

        [SerializeField]
        private bool _haveMetPlayer;


        private Animator _npcAnimator;


        // Use this for initialization
        void Start()
        {
            _communicate = false;


            if (PlayerPrefs.GetString("MetNPC_" + ReturnNpcName()) == "True")
            {
                _haveMetPlayer = true;
            }
            else
            {
                _haveMetPlayer = false;
            }

            _npcAnimator = GetComponent<Animator>();

        }

        // Update is called once per frame
        void Update()
        {

            if (_player != null && _communicate)
            {

                Vector3 _dir = _player.transform.position - transform.position;                       // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 4);  // rotate the player
                _npcAnimator.SetBool("isWalk", false);
                _npcAnimator.SetBool("isIdle", true);

            }

            if (_patrol)
            {
                Patrol();
                _npcAnimator.SetBool("isWalk", true);
                _npcAnimator.SetBool("isIdle", false);
            }

            for (int i = 0; i < _wayPoints.Count; i++)
            {
                if (i + 1 < _wayPoints.Count)
                {
                    Debug.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
                }
                else
                {

                }
            }

        }



        public void SetConversation(GameObject _playerTarget, bool _moving)
        {

            _player = _playerTarget;
            _patrol = _moving;

            if (!_moving)
            {
                //      GetComponentInChildren<NPC_Dialog>().Converse();
            }
            if (_moving)
            {

                //    GetComponentInChildren<NPC_Dialog>().StopConversation();
            }

        }

        void Patrol()
        {
            if (_patrol)
            {
                if (_wayPoints.Count > 0)
                {

                    if (_currentWayPoint < _wayPoints.Count)
                    {
                        _wayPointTarget = _wayPoints[_currentWayPoint].position;
                        _moveDirection = _wayPointTarget - transform.position;

                        if (_moveDirection.magnitude < 1)
                        {
                            _currentWayPoint++;
                        }
                        else
                        {
                        }

                    }
                    else {

                        _currentWayPoint = 0;


                    }
                    _oldPosition = transform.position;

                    Vector3 _dir = _wayPointTarget - transform.position;                                        // get the Vector we are going to move to
                    _dir.y = 0f;                                                                                // we dont want to move up
                    Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at

                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 2);
                    transform.position = Vector3.MoveTowards(transform.position, _wayPointTarget, _patrolSpeed / 10);

                    _distanceTraveled += (transform.position - _oldPosition).magnitude;

                    // Check if the distance traveled is greater than 2
                    // In this case 2f is the distance between each step! ( FOR RUNNING )

                    if (_distanceTraveled >= 2f)
                    {
                        // Call the PlayFootstepSound in the SoundSystem Class
                        NpcSoundSystem.PlayFootSteps(this.transform.position);
                        _distanceTraveled = 0.0f;
                    }
                }

            }
        }

        public void IsSelected(bool _set)
        {
            _communicate = _set;

        }

        public bool ReturnIsSelected()
        {
            return _communicate;
        }

        public void PlayerInteraction(GameObject _playerTarget, bool _shouldMove)
        {
            _player = _playerTarget;
            _patrol = _shouldMove;

        }

        public int ReturnNpcID()
        {
            return _npcID;
        }

        public bool ReturnPatrol()
        {
            return _patrol;
        }

        public void SetNpcID(int _id)
        {
            _npcID = _id;
        }
        public void SetNPCName(string _name)
        {
            _nameToDisplay = _name;
        }

        public void SetProfession(string _prof)
        {
            _npcProfession = _prof;
        }

        public void SetInteraction(bool _inter)
        {
            _interaction = _inter;
        }

        public void SetDialogues(string _dialog1, string _dialog2)
        {
            _dialogue1 = _dialog1;
            _dialogue2 = _dialog2;
        }

        public void SetQuestGiver(string _quest)
        {
            if (_quest == "True")
            {
                _questGiver = true;

            }
            else
            {
                _questGiver = false;
                Debug.Log(_questGiver);
            }
        }


        public void SetNpcBehaviour(ActorBehaviour _behaviour)
        {
            if (_behaviour == ActorBehaviour.Idle)
            {
                _patrol = false;
            }
            if (_behaviour == ActorBehaviour.Patrol)
            {
                _patrol = true;
            }
        }

        public void SetWayPoints(Transform _wp)
        {
            _wayPoints.Add(_wp);
        }


        public void SetPatrolSpeed(float _speed)
        {
            _patrolSpeed = _speed;
        }


        public void HasMetPlayer(bool _met)
        {
            _haveMetPlayer = _met;

        }

        public ActorBehaviour ReturnBehaviour()
        {
            if (_patrol)
            {
                return ActorBehaviour.Patrol;
            }
            else
            {
                return ActorBehaviour.Idle;
            }
        }

        public string ReturnNpcName()
        {
            return _nameToDisplay;
        }

        public float ReturnNpcHealth()
        {
            return _health;
        }


        public string ReturnDialogue1()
        {
            return _dialogue1;
        }

        public string ReturnDialogue2()
        {
            return _dialogue2;
        }

        public bool ReturnMetBefore()
        {
            return _haveMetPlayer;
        }

        public bool ReturnQuestGiver()
        {
            return _questGiver;
        }

        public int ReturnWaypointAmount()
        {
            return _wayPoints.Count;
        }

        public float ReturnPatrolSpeed()
        {
            return _patrolSpeed;
        }

        public string ReturnProfession()
        {
            return _npcProfession;
        }


    }
}
