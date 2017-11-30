using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{

    public enum ActorBehaviour
    {
        Idle,
        Patrol,
    }

    public class NpcSystem : MonoBehaviour
    {


        [SerializeField] private int _npcID;
        [SerializeField] private string _npcName;
        [SerializeField] private bool _hasMetPlayer;
        [SerializeField] private float _patrolSpeed;
        [SerializeField] private bool STATE_IDLE;
        [SerializeField] private bool STATE_PATROL;
        [SerializeField] private string _prefab;
        [SerializeField] private ActorBehaviour _initialBehaviour;

        private bool STATE_CONVERSATION;
        private bool _isSelected;
        private bool _setInteraction;

        [SerializeField] private List<Transform> _wayPoints = new List<Transform>();
        private Vector3 _wayPointTarget;
        private int _currentWayPoint;
        private Vector3 _moveDirection;
        private Vector3 _oldPosition;
        private float _distanceTraveled;

        private NpcAnimationSystem _npcAnimator;
        private CharacterController _characterController;

        private GameObject _prevHighlighted;
        private GameObject _hasQuestVFX;

        private int _nodeID;



        // Use this for initialization
        void Start()
        {
            _npcAnimator = new NpcAnimationSystem(GetComponent<Animator>(), _prefab, _patrolSpeed);
            _characterController = GetComponent<CharacterController>();

            CheckForQuest();

            SwitchOnNpcCamera(false);

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (STATE_IDLE)
            {
                Idle();
            }
            if (STATE_PATROL)
            {
                Patrol();
            }
            if (STATE_CONVERSATION)
            {
            }
            if (_isSelected)
            {
                HighlightNPC(true, this.gameObject);
                CheckDistance();

            }
            if (!_isSelected)
            {
                HighlightNPC(false, null);
            }
        }

        public void SetData(int id, string name, ActorBehaviour behaviour, float patrol, string prefab)
        {
            _npcID = id;
            _npcName = name;


            if (behaviour == ActorBehaviour.Idle)
            {
                STATE_IDLE = true;
                STATE_PATROL = false;
            }
            if (behaviour == ActorBehaviour.Patrol)
            {
                STATE_PATROL = true;
                STATE_IDLE = false;
            }
            _initialBehaviour = behaviour;
            _patrolSpeed = patrol;
            _prefab = prefab;
        }

        public void SetWaypoints(Transform t)
        {
            _wayPoints.Add(t);
        }

        void Idle()
        {
            _npcAnimator.SetIdle();
        }

        void Patrol()
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
                else
                {
                    _currentWayPoint = 0;
                }
                _oldPosition = transform.position;

                Vector3 _dir = _wayPointTarget - transform.position;                                        // get the Vector we are going to move to
                _dir.y = 0f;                                                                                // we dont want to move up
                Quaternion _targetRot = Quaternion.LookRotation(_dir);                                      // get the rotation in which we should look at
                Vector3 _forward = transform.TransformDirection(Vector3.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 2);

                _characterController.SimpleMove(_forward * _patrolSpeed);
                _npcAnimator.SetWalking();

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

        public void IsSelected(bool _set)
        {
            _isSelected = _set;
        }

        public void SetInteraction(bool _set)
        {
            _setInteraction = _set;
        }

        public void PlayerHasMetNPC()
        {
            _hasMetPlayer = true;
        }

        public bool ReturnIsSelected()
        {
            return _isSelected;
        }

        public void HighlightNPC(bool _set, GameObject _sel)
        {
            if (_set)
            {
                for (int i = 0; i < _sel.GetComponentsInChildren<SkinnedMeshRenderer>().Length; i++)
                {
                    if (_sel.GetComponentsInChildren<SkinnedMeshRenderer>()[i].material.GetTexture("_EmissionMap") != null)
                    {
                        _sel.GetComponentsInChildren<SkinnedMeshRenderer>()[i].material.SetColor("_EmissionColor", Color.white);
                        _prevHighlighted = _sel;
                    }
                }
            }
            if (!_set)
            {
                if (_prevHighlighted != null)
                {
                    for (int i = 0; i < _prevHighlighted.GetComponentsInChildren<SkinnedMeshRenderer>().Length; i++)
                    {
                        _prevHighlighted.GetComponentsInChildren<SkinnedMeshRenderer>()[i].material.SetColor("_EmissionColor", Color.black);
                    }
                    _prevHighlighted = null;
                }
            }
        }

        public string ReturnNpcName()
        {
            return _npcName;
        }

        public int ReturnID()
        {
            return _npcID;
        }

        public ActorBehaviour ReturnNpcBehaviour()
        {
            if (STATE_PATROL)
            {
                return ActorBehaviour.Patrol;
            }
            else
            {
                return ActorBehaviour.Idle;
            }
        }

        public void SetNpcBehaviour(ActorBehaviour _behaviour)
        {
            if (_behaviour == ActorBehaviour.Idle)
            {
                STATE_IDLE = true;
                STATE_PATROL = false;
            }
            if (_behaviour == ActorBehaviour.Patrol)
            {
                STATE_PATROL = true;
                STATE_IDLE = false;
            }
        }

        public int ReturnWaypointAmount()
        {
            return _wayPoints.Count;
        }

        public float ReturnPatrolSpeed()
        {
            return _patrolSpeed;
        }

        public void SetPatrolSpeed(float _speed)
        {
            _patrolSpeed = _speed;
        }

        public void UpdateQuestNPC(bool _set)
        {

        }

        void CheckDistance()
        {
            if (Vector3.Distance(transform.position, CombatSystem.PlayerController.instance.ReturnPlayerPosition()) < 5f)
            {
                STATE_PATROL = false;
                STATE_IDLE = true;
                STATE_CONVERSATION = true;
                _npcAnimator.StopWalking();

                CreateDialogue();
            }
            else
            {

                if (_initialBehaviour == ActorBehaviour.Idle)
                {
                    STATE_IDLE = true;
                    STATE_CONVERSATION = false;
                    _npcAnimator.SetIdle();
                }
                if (_initialBehaviour == ActorBehaviour.Patrol)
                {
                    STATE_IDLE = false;
                    STATE_PATROL = true;
                    _npcAnimator.SetWalking();
                }
                STATE_CONVERSATION = false;
            }
        }

        void CreateDialogue()
        {
            if (STATE_CONVERSATION)
            {
                if (_setInteraction)
                {

                    Dialogue.DialogueManager.InitiateDialogue(_npcID, false, this.gameObject);
                    _setInteraction = false;
                    IsSelected(false);

                }
            }
        }

        public bool ReturnHasConversation()
        {
            if (Dialogue.DialogueDatabase.GetInitialQuestionFromNPC(_npcID) != string.Empty)
            {
                return true;
            }
            else {
                return false;
            }
        }
        public void CheckForQuest()
        {
            if(Quest.QuestDatabase.NPCHasNewQuest(_npcID))
            {
                if (_hasQuestVFX == null)
                {
                    _hasQuestVFX = Instantiate(Resources.Load("VFX/PS_NPC_Quest") as GameObject);
                    _hasQuestVFX.GetComponent<FollowObject>().SetObject(this.gameObject);
                }
            }
            else
            {
                if(_hasQuestVFX != null)
                {
                    Destroy(_hasQuestVFX);
                }
            }
        }

        public void SwitchOnNpcCamera(bool set)
        {
            GetComponentInChildren<Camera>().enabled = set;
            if (transform.parent.gameObject.GetComponentInChildren<Light>() != null)
            {
                transform.parent.gameObject.GetComponentInChildren<Light>().enabled = set;
            }
        }

        IEnumerator StopConversation()
        {
            yield return new WaitForSeconds(4);
            Dialogue.DialogueManager.CancelDialogue();
            SwitchOnNpcCamera(false);
            CombatSystem.PlayerController.instance.SetInputBlock(false);
        }

    }

}
