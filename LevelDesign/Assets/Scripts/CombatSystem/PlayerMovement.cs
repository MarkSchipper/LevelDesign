using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class PlayerMovement : MonoBehaviour
    {

        private bool STATE_WALKING = false;
        private bool STATE_RUNNING = false;
        private bool STATE_BACKWARDS = false;
        private bool STATE_IDLE = false;
        private bool STATE_JUMPING = false;

        private CharacterController _charController;

        private Vector3 _moveDirection = new Vector3(0, 0, 0);
        private float _runSpeed;
        private float _walkSpeed;
        private float _rotateAngle = 0f;
        private float _rotatedAngle = 0f;
        private float _playerDistanceTraveled = 0.0f;
        private float _playerFallingDistance = 0.0f;
        private float _blinkMaxDistance;
        private bool _setOnce;

        private GameObject _playerMesh;

        private float _playerMass = 3.0f;

        private Vector3 _impact = Vector3.zero;

        void OnEnable()
        {
            foreach (Animator anim in GetComponentsInChildren<Animator>())
            {
                if (anim.name == "PlayerMesh")
                {
                    _playerMesh = anim.gameObject;
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            _charController = this.GetComponent<CharacterController>();
        }

        void Update()
        {
            if(_impact.magnitude > 0.2f)
            {
                _charController.Move(_impact * Time.deltaTime);
                _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.deltaTime);
            }
        }

        public void SetValues(float _run, float _walk)
        {
            _runSpeed = _run;
            _walkSpeed = _walk;
        }
      
        public void CheckInputs()
        {
            if (!PlayerController.instance.ReturnPlayerDead())
            {
                #region MOVING THE CHARACTER       
                if (_charController.isGrounded)
                {
                    _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    _playerFallingDistance = 0;
                    // If we pressed the Left Shift we multiply the movedirection with the walking speed
                    // Else by the run speed
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _moveDirection *= _walkSpeed * Time.deltaTime;
                        STATE_IDLE = false;
                        PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                    }
                    else
                    {
                        _moveDirection *= _runSpeed * Time.deltaTime;
                    }

                    if (_moveDirection != Vector3.zero)
                    {
                        if (InteractionManager.instance.ReturnIsSpellCasting())
                        {
                            InteractionManager.instance.CancelSpellCasting();
                        }
                    }

                    // If _moveDirection.z is not 0
                    // If we are moving FORWARD
                    if (_moveDirection.z != 0)
                    {

                        // If the z direction is negative we are walking backwards
                        if (_moveDirection.z < 0)
                        {
                            PlayerController.instance.SetPlayerState("STATE_BACKWARDS", true);
                            STATE_BACKWARDS = true;
                        }
                        else
                        {
                            PlayerController.instance.SetPlayerState("STATE_BACKWARDS", false);
                            STATE_BACKWARDS = false;
                        }
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            PlayerController.instance.SetPlayerState("STATE_WALKING", true);
                            PlayerController.instance.SetPlayerState("STATE_RUNNING", false);
                            CombatSystem.AnimationSystem.SetSkipIdle(true);

                            STATE_WALKING = true;
                            STATE_RUNNING = false;

                        }
                        else
                        {
                            PlayerController.instance.SetPlayerState("STATE_RUNNING", true);
                            PlayerController.instance.SetPlayerState("STATE_WALKING", false);
                            PlayerController.instance.SetPlayerState("STATE_INCOMBAT", false);

                            STATE_RUNNING = true;
                            STATE_WALKING = false;
                        }

                        // If we are moving FORWARD and have pressed the A or D key we rotate the player 45 degrees to simulate strafing
                        // If we are NOT moving sideways rotate the player to 0,0,0 again
                        // NOTE: We are rotating the player mesh which is a CHILD of the PlayerController object since we don't want to rotate the parent object
                        if (_moveDirection.x > 0 && _moveDirection.z != 0)
                        {
                            _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, 45, 0)), Time.deltaTime * 20f);
                            PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                            STATE_IDLE = false;
                        }
                        if (_moveDirection.x < 0 && _moveDirection.z != 0)
                        {
                            _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, -45, 0)), Time.deltaTime * 20f);
                            PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                            STATE_IDLE = false;
                        }
                        if (_moveDirection.x == 0)
                        {
                            _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime * 20f);
                            PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                            STATE_IDLE = false;
                        }

                        PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                        STATE_IDLE = false;
                    }

                    // If we are NOT moving forward but have pressed A or D
                    // Rotate the player by 90 degrees so it runs sideways
                    if (_moveDirection.x != 0 && _moveDirection.z == 0)
                    {
                        if (_moveDirection.x > 0)
                        {
                            _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), Time.deltaTime * 20f);
                        }
                        if (_moveDirection.x < 0)
                        {
                            _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, -90, 0)), Time.deltaTime * 20f);

                        }
                        PlayerController.instance.SetPlayerState("STATE_RUNNING", true);
                        PlayerController.instance.SetPlayerState("STATE_IDLE", false);
                        PlayerController.instance.SetPlayerState("STATE_INCOMBAT", false);

                        STATE_RUNNING = true;
                        STATE_IDLE = false;

                    }

                    // If we havent pressed anything
                    // Rotate the player to 0,0,0 
                    if (_moveDirection.z == 0 && _moveDirection.x == 0)
                    {
                        _playerMesh.transform.localRotation = Quaternion.Slerp(_playerMesh.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime * 20f);

                        PlayerController.instance.SetPlayerState("STATE_RUNNING", false);
                        PlayerController.instance.SetPlayerState("STATE_WALKING", false);

                        STATE_RUNNING = false;
                        STATE_WALKING = false;

                        if (!PlayerController.instance.ReturnInCombat())
                        {
                            PlayerController.instance.SetPlayerState("STATE_IDLE", true);
                            STATE_IDLE = true;
                            CombatSystem.AnimationSystem.SetPlayerIdle();
                        }

                    }
                    if (Input.GetKey(KeyCode.Space))
                    {
                        _moveDirection.y = 0.1f;
                        PlayerController.instance.SetPlayerState("STATE_JUMPING", true);
                        STATE_JUMPING = true;
                        SoundManager.instance.PlaySound(SOUNDS.PLAYERJUMP, transform.position, true);
                    }

                    // Convert the _moveDirection to world space
                    _moveDirection = transform.TransformDirection(_moveDirection);

                }
                if (!_charController.isGrounded)
                {
                    _playerFallingDistance += Time.deltaTime;

                    _moveDirection.y -= 0.25f * Time.deltaTime;

                    if (_playerFallingDistance > 2.0f)
                    {
                        if (!_setOnce)
                        {
                            PlayerController.instance.SetPlayerState("STATE_DEAD", true);
                            PlayerController.instance.PlayerDeath(true);
                            _setOnce = true;
                        }
                    }

                    _charController.Move(_moveDirection * Time.deltaTime);
                }
                #endregion
            }
        }

        // If the player is walking, move the player and play the walking animation
        public void PlayerWalking()
        {
            CombatSystem.AnimationSystem.StopPlayerIdle();
            CombatSystem.AnimationSystem.SetSkipIdle(true);

            Vector3 _oldPosition = this.transform.position;

            if (!STATE_BACKWARDS)
            {
                _charController.Move(_moveDirection * _walkSpeed);
            }
            if (STATE_BACKWARDS)
            {
                _charController.Move(_moveDirection * (_walkSpeed * 0.5f));
            }
            CombatSystem.AnimationSystem.SetPlayerWalking(STATE_BACKWARDS);

            _playerDistanceTraveled += (transform.position - _oldPosition).magnitude;

            if (_playerDistanceTraveled > 0.5f)
            {
                if (_charController.isGrounded)
                {
                    SoundManager.instance.PlaySound(SOUNDS.FOOTSTEPS, transform.position, true);
                    _playerDistanceTraveled = 0.0f;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                  PlayerRunning()                                         //
        // If the player is running, move the player and play the running animation                                 //
        //  Check the distance the player has traveled, if it is greater than 1.75f                                 //
        //      1.75f is an estimation of the distance between footsteps                                            //
        //  Call the SoundSystem and play the Footstep sounds                                                       //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PlayerRunning()
        {

            CombatSystem.AnimationSystem.StopPlayerIdle();

            Vector3 _oldPosition = this.transform.position;
            if (!STATE_BACKWARDS)
            {
                _charController.Move(_moveDirection * _runSpeed);
            }
            if (STATE_BACKWARDS)
            {
                _charController.Move(_moveDirection * (_runSpeed * 0.5f));
            }
            CombatSystem.AnimationSystem.SetPlayerRunning(STATE_BACKWARDS);

            _playerDistanceTraveled += (transform.position - _oldPosition).magnitude;

            if (_playerDistanceTraveled > 2.25f)
            {
                if (_charController.isGrounded)
                {
                    CombatSystem.SoundManager.instance.PlaySound(SOUNDS.FOOTSTEPS, transform.position, true);
                    _playerDistanceTraveled = 0.0f;
                }
            }
        }

        public void PlayerJump()
        {

            CombatSystem.AnimationSystem.SetPlayerJumping();
            if (CombatSystem.AnimationSystem.ReturnJumpingFinished())
            {
                CombatSystem.AnimationSystem.StopPlayerJumping();
                PlayerController.instance.SetPlayerState("STATE_JUMPING", false);
                STATE_JUMPING = false;
            }
        }

        public void MovePlayerToPosition(Vector3 _pos)
        {

        }

        public void Knockback(Vector3 dir, float force)
        {
            
            dir = transform.TransformDirection(dir);
             _impact += dir.normalized * force / _playerMass;
        }

    }
}
