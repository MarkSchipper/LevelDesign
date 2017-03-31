using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;


public class Cinematic_Movement : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////
    //
    // The cinematic movement script
    // In this class we do all the movements the Actor can do
    //
    //////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // All Animation functions have the following lines                                                                 //
    //                                                                                                                  //
    //      _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();                                            //
    //      _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();                                            //
    //      _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();                           //
    //                                                                                                                  //
    //      The SetComplete() is to set the current Node to be complete                                                 //
    //      The SetInActive() is to set the current node to inactive                                                    //
    //      Then we get the next node in line based on the ReturnOutputID() ( the connection made by the user           //
    //                                                                                                                  //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Store all the current nodes in the scenes
    private GameObject[] _allNodes;

    // Animation states
    private bool _isWalking = false;
    private bool _isIdle = false;
    private bool _isRunning = false;
    private bool _isCombatIdle = false;
    private bool _setOnce = false;
    private bool _isRanged = false;
    private bool _isMelee = false;
    private bool _isTeleport = false;
    private bool _isFade = false;
    private bool _isParticle = false;
    private bool _isImage = false;

    // If there is a custom animation
    private bool _isCustom = false;

    // which custom animation
    private bool _isCustomIdle = false;
    private bool _isCustomWalk = false;
    private bool _isCustomRun = false;
    private bool _isCustomRanged = false;
    private bool _isCustomGesture = false;

    // Waypoints set by the user ( where to move to )
    private GameObject _wayPoint;

    // A simple int to keep track of which node is active -- directly corresponds with the name of the Nodes
    private int _activeNode;

    // The Animator for the player so we can fire up animations
    private Animator _animator;

    // simple counter
    private float _counter;

    // The Character Controller
    // Without it we don't have 'physics', so no collisions
    private CharacterController _playerMovement;

    // The sounds 
    private AudioSource _soundManager;
    private bool _isPlaying;

    private AudioSource _soundTrack;

    // The character ID which is used in the Node Editor
    public int CharacterID;

    [SerializeField]
    private float _fadeTimer = 0f;
    private bool _countUp = true;

    private float _setFadeTime;

    private float _solidTime = 0f;


    // timer

    private GameObject _screenTimer;
    private float _onScreenTimer = 0f;

    private bool _imageComplete = false;
    private float _imageFader = 0f;


    public void Start()
    {

        // Fetch the Animator component
        _animator = GetComponentInChildren<Animator>();

        // Fetch the CharacterController component
        _playerMovement = GetComponent<CharacterController>();

        // Fetch the AudioSource component
        _soundManager = GetComponent<AudioSource>();
        _soundTrack = GetComponent<AudioSource>();

        // Find ALL the Nodes in the scene and order them based on the 4th 'thing' in the name, in this case the integer
        _allNodes = GameObject.FindGameObjectsWithTag("Node").OrderBy(go => int.Parse(go.name.Substring(4))).ToArray();

      
        for (int i = 0; i < _allNodes.Length; i++)
        {

            // Check if we are using the right CharacterID
            if (_allNodes[i].GetComponent<NodeObject>().ReturnCharID() == CharacterID)
            {
                // IF the current node is NOT complete
                if (!_allNodes[i].GetComponent<NodeObject>().ReturnComplete())
                {
                    // If there is a Start node
                    if (_allNodes[i].GetComponent<NodeObject>().ReturnAnim() == "Start")
                    {
                        // Start is set to complete to fire up the next nodes
                        _allNodes[i].GetComponent<NodeObject>().SetComplete();

                        // Move the Actor in the start position
                        transform.position = _allNodes[i].GetComponent<NodeObject>().ReturnWayPoint().transform.position;
                        transform.rotation = _allNodes[i].GetComponent<NodeObject>().ReturnWayPoint().transform.rotation;

                        // Get the next node in line
                        _activeNode = _allNodes[i].GetComponent<NodeObject>().ReturnOutputID();

                        // Set the next node to active
                        _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();

                    }

                    // IF there is a SOUNDTRACK node, Play it!
                    if(_allNodes[i].GetComponent<NodeObject>().ReturnAutoPlay())
                    {

                        GameObject _soundGO = new GameObject();
                        _soundGO.name = "SoundTrack";

                        _soundGO.AddComponent<AudioSource>();
                        _soundGO.GetComponent<AudioSource>().clip = _allNodes[i].GetComponent<NodeObject>().ReturnAudio();
                        _soundGO.GetComponent<AudioSource>().Play();
                        
                        _soundTrack.Play();
                        if (!_soundTrack.isPlaying)
                        {
                            _isPlaying = false;
                        }
                        else
                        {
                            _isPlaying = true;
                        }
                    }
                }
                else
                {
                    // Error catching
                }
            }
        }

        if (GameObject.Find("timeCounter") != null)
        {
            _screenTimer = GameObject.Find("timeCounter");
        }

    }

    public void Update()
    {

        SequenceTimer();

        // Again check if we are using the correct character
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCharID() == CharacterID)
        {
          // Check if there are actual nodes ( safety check )
          if (_allNodes.Length > 0)
            {
                // If the current active node actually exists ( safety check )
                if (_allNodes[_activeNode] != null)
                {
                    // If the active node is actually active ( safety check )                
                    if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnActive())
                    {

                        //////////////////////////////////////////////////////////////////////////////////////////
                        //                                                                                      //
                        // Check what the Animation is ( ReturnAnim() ) and set the corresponding bool to true  //
                        // If there is a WayPoint set in the animation, set this classes waypoint to match it   //
                        //                                                                                      //
                        //////////////////////////////////////////////////////////////////////////////////////////

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Walk")
                        {
                            _wayPoint = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint();

                            _isWalking = true;
                        }

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Run")
                        {
                            _wayPoint = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint();

                            _isRunning = true;
                        }

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Idle")
                        {
                            _isIdle = true;
                        }
                        if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "JumpTo")
                        {
                            _isTeleport = true;
                        }
                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "CombatIdle")
                        {
                            _isCombatIdle = true;
                        }

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Start")
                        {
                            _isIdle = true;
                        }
                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Ranged")
                        {
                            _isRanged = true;
                        }
                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Melee")
                        {
                            _isMelee = true;
                        }
                        if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Fade")
                        {
                            _isFade = true;
                            _setFadeTime = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnFadeTime();
                        }

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "ParticleSystem")
                        {
                            _isParticle = true;
                            
                        }

                        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Image")
                        {
                            _isImage = true;
                        }

                            //////////////////////////////////////////////////////
                            //                  CUSTOM NODES                    //
                            //////////////////////////////////////////////////////


                            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "CustomNode")
                        {
                            _isCustom = true;
                            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnimType() == "Idle")
                            {
                                _isCustomIdle = true;
                                
                            }
                            else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnimType() == "Walk")
                            {
                                _isCustomWalk = true;
                            }
                            else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnimType() == "Run")
                            {
                                _isCustomRun = true;
                            }
                            else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnimType() == "Ranged")
                            {
                                _isCustomRanged = true;
                            }
                            else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnimType() == "Gesture")
                            {
                                _isCustomGesture = true;
                            }

                            
                            
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////////////
            // Here we set the Animator states and fire up the corresponding function       //
            //////////////////////////////////////////////////////////////////////////////////

            if (_isWalking)
            {
                _animator.SetBool("isWalking", true);
                Walk();
            }

            if (_isRunning)
            {
                _animator.SetBool("isRunning", true);
                Run();
            }
            if (_isIdle)
            {
                //Debug.Log("Idle");
                Idle();
            }

            if(_isTeleport)
            {
                Teleport();
            }

            if(_isCombatIdle)
            {
                _animator.SetBool("isCombatIdle", true);
                CombatIdle();
            }

            if (_isRanged)
            {
                _animator.SetBool("isRanged", true);
                Ranged();
                
            }

            if (_isMelee)
            {
                _animator.SetBool("isMelee", true);

                Melee();
            }
            if(_isCustom)
            {
                if(_isCustomWalk)
                {
                   // _animator.SetBool("isCustom", true);
                    CustomWalk();
                }
                else if (_isCustomIdle)
                {
                    _animator.SetBool("isCustom", true);
                    CustomIdle();
                    
                }
                else if (_isCustomGesture)
                {
                    _animator.SetBool("isCustomGesture", true);
                    CustomGesture();
                }
                
            }
            if(_isFade)
            {
                FadeScreen();
               
            }
            if(_isParticle)
            {
                Particles();
            }

            if(_isImage)
            {
                DoImage();
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////
    // Nothing but setting the Actors position to the Waypoint set by the user  //
    //////////////////////////////////////////////////////////////////////////////

    public void AnimStart(GameObject _wayPoint)
    {
        this.transform.position = _wayPoint.transform.position;
        
    }

    // Set the Waypoint

    public void SetWayPoint(GameObject _nwWayPoint)
    {
        _wayPoint = _nwWayPoint;
       
    }

    //////////////////////////////////////////////////////////////////////////////
    //                          Walk like an Egyptian                           //
    //////////////////////////////////////////////////////////////////////////////

    void Walk()
    {

       
        // Has the user set a sound to be played with this Node
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {
                
                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }

        // The actual interesting stuff

        Vector3 _dir = _wayPoint.transform.position - transform.position;                       // get the Vector we are going to move to
        _dir.y = 0f;                                                                            // we dont want to move up
        Quaternion _targetRot = Quaternion.LookRotation(_dir);                                  // get the rotation in which we should look at

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 2);  // Slerp the rotations

        //transform.rotation = _targetRot;                                                      // rotate the player

        Vector3 _forward = transform.TransformDirection(Vector3.forward);                       // create a forward Vector3

        if (_dir.magnitude > 0.1f)
        {                                                        
            _playerMovement.SimpleMove(_forward * 2.65f);                                       // move the actual player

        }
        else
        {
            _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
            _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
            _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
            if (_allNodes[_activeNode] != null)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
            }

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
            {

                _animator.SetBool("skipIdle", true);

            }
            if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Idle")
            {
                _animator.SetBool("skipIdle", false);
            }



            _isWalking = false;
            _animator.SetBool("isWalking", false);
        }

            

    }

    public void Idle()
    {
        
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait() > 0)
        {

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
            {
                if (!_isPlaying)
                {
                   // Debug.Log(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio());
                    _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                    _soundManager.Play();
                    if(!_soundManager.isPlaying)
                    {
                        _isPlaying = false;
                    }
                    else
                    {
                        _isPlaying = true;
                    }

                }
            }

            if (!_setOnce)
            {
                _counter = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait();
                _setOnce = true;
            }
            _counter -= Time.deltaTime;
            
           

            if (_counter < 0)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
                _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                
                if (_allNodes[_activeNode] != null)
                {
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                }
                _isIdle = false;
                _setOnce = false;
            }
        }
        else
        {
            _isWalking = false;
            _isRunning = false;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("skipIdle", false);
        }
        
    }

    public void Run()
    {


        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {
                
                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }

        Vector3 _dir = _wayPoint.transform.position - transform.position;                                    // get the Vector we are going to move to
        _dir.y = 0f;                                                                    // we dont want to move up
        Quaternion _targetRot = Quaternion.LookRotation(_dir);                          // get the rotation in which we should look at
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 2);  // Slerp the rotations

        Vector3 _forward = transform.TransformDirection(Vector3.forward);               // create a forward Vector3

        if (_dir.magnitude > 0.1f)
        {                                                    // if the magnitude of the vector is greater than
            _playerMovement.SimpleMove(_forward * 3f);        // move the actual player

        }
        else
        {
            _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
            _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
            _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
            if (_allNodes[_activeNode] != null)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
            }

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
            {

                _animator.SetBool("skipIdle", true);

            }



            _isRunning = false;
            _animator.SetBool("isRunning", false);
        }


    }

    public void Teleport()
    {
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint() != null)
        {
            transform.position = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint().transform.position;


            _isTeleport = false;


            _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
            _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
            _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();

            if (_allNodes[_activeNode] != null)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
            }
            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
            {

                _animator.SetBool("skipIdle", true);

            }

            else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Idle")
            {

                _animator.SetBool("skipIdle", false);

            }

        }
    }


    public void CombatIdle()
    {
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait() > 0)
        {

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
            {
                if (!_isPlaying)
                {
                    
                    _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                    _soundManager.Play();
                    if (!_soundManager.isPlaying)
                    {
                        _isPlaying = false;
                    }
                    else
                    {
                        _isPlaying = true;
                    }

                }
            }


            if (!_setOnce)
            {
                _counter = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait();
                _setOnce = true;
            }
            _counter -= Time.deltaTime;



            if (_counter < 0)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
                _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();

                _isCombatIdle = false;
                _animator.SetBool("isCombatIdle", false);

                if (_allNodes[_activeNode] != null)
                {
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                }
                else if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                {

                    _animator.SetBool("skipIdle", true);

                }
               
            }
        }
        else
        {
            _isWalking = false;
            _isRunning = false;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("skipIdle", false);
        }
    }

    void Ranged()
    {

        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {
             
                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }


            _animator.SetBool("isRanged", true);
        
           

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Ranged"))
            {
            
;            }
            else {

            

                _isRanged = false;
                _animator.SetBool("isRanged", false);

               _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();

                _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                if (_allNodes[_activeNode] != null)
                {
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                }

                if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                {

                    _animator.SetBool("skipIdle", true);

                }
            }



    }

    void Melee()
    {

    }

    void CustomIdle()
    {

        _isIdle = false;
     
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait() > 0)
        {
            
            _animator.Play(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnim().ToString());
            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
            {
                if (!_isPlaying)
                {
                    
                    _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                    _soundManager.Play();
                    if (!_soundManager.isPlaying)
                    {
                        _isPlaying = false;
                    }
                    else
                    {
                        _isPlaying = true;
                    }

                }
            }

            if (!_setOnce)
            {
                _counter = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnIdleWait();
                _setOnce = true;
            }
            _counter -= Time.deltaTime;
           


            if (_counter < 0)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
                _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                _isCustomIdle = false;
                if (_allNodes[_activeNode] != null)
                {
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                }
                _setOnce = false;

             

                
                _animator.SetBool("isCustom", false);
            }
        }
        else
        {
            _isWalking = false;
            _isRunning = false;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("skipIdle", false);
        }
    }


    void CustomWalk()
    {
        _animator.Play(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnCustomAnim().ToString());
        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {
                
                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }

        Vector3 _dir = _wayPoint.transform.position - transform.position;                                    // get the Vector we are going to move to
        _dir.y = 0f;                                                                    // we dont want to move up
        Quaternion _targetRot = Quaternion.LookRotation(_dir);                          // get the rotation in which we should look at
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, Time.deltaTime * 2);  // Slerp the rotations

        Vector3 _forward = transform.TransformDirection(Vector3.forward);               // create a forward Vector3

        if (_dir.magnitude > 0.1f)
        {                                                    // if the magnitude of the vector is greater than
            _playerMovement.SimpleMove(_forward * 2.65f);        // move the actual player

        }
        else
        {
            _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
            _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
            _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
            if (_allNodes[_activeNode] != null)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
            }

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
            {

                _animator.SetBool("skipIdle", true);

            }



            _isCustomWalk = false;
            //_animator.SetBool("isWalking", false);
        }



    }


    void CustomGesture()
    {
        
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isIdle", false);

        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {
                
                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }
            
        // Bit dirty!
        // We check if the current normalized time of the animation is greater than 0, if we are NOT in transition mode and if the current normalized time is smaller than 2
        // If we don't check for greater than 1 and smaller than 2 it does some weird ass shit

        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animator.IsInTransition(0) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 2)
        {
            

            _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
            _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();
            _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
            _isCustomGesture = false;

            _animator.SetBool("isCustomGesture", false);

            if (_allNodes[_activeNode] != null)
            {
                _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
            }

            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
            {

                _animator.SetBool("skipIdle", true);

            }
            if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Idle")
            {

                _animator.SetBool("skipIdle", false);

            }

        }
        
    }

    //////////////////////////////////////////////////////
    //                                                  //
    //                Hacky fade to color               //
    //                                                  //
    //////////////////////////////////////////////////////

    void FadeScreen()
    {
        Image _fadeImage = GameObject.Find("Canvas").GetComponent<Image>();

  //      _animator.SetBool("isWalking", false);
  //      _animator.SetBool("isRunning", false);
  //      _animator.SetBool("skipIdle", false);

        // If we are fading ( bool )

        if (_isFade)
        {
            
            // Bool to see if we are in the transition or not          
            if (_countUp)
            {

                // New timer and check if its smaller than HALF of the fadetime in the Fade node
                if (_fadeTimer <= (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnFadeTime() / 2))
                {

                    // We make a new Color with the RGB which we set but create a new alpha based on the timer divided by half of the fade time
                    // NOTE we are using Color and not Color32 so all values go between 0 and 1

                    _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, (_fadeTimer / (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnFadeTime() / 2)));
                    _fadeTimer += Time.deltaTime;

                    // IF the fadetimer is HALF of the Fade Time stated by the user
                    if (_fadeTimer >= _allNodes[_activeNode].GetComponent<NodeObject>().ReturnFadeTime() / 2)
                    {
                       

                        // IF we have a 'solid time', meaning the alpha is set to 1              
                        if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnSolidTime() > 0)
                        {
                            // Start a new Coroutine which is nothing but a timer
                            StartCoroutine(SolidTimer());
                            
                            // Set the alpha to 1
                            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1);
                        }

                        // IF there is no solid time then just fade back to 0
                        else
                        {
                            _countUp = false;

                            // set the _fadeTimer to 1 so we get a solid image
                            _fadeTimer = 1;
                        }
                       
                        // IF the user added a teleport to the node
                        if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint() != null && _allNodes[_activeNode].GetComponent<NodeObject>().ReturnFadeAction() == "Teleport")
                        {
                            // Teleport!
                            transform.position = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnWayPoint().transform.position;
                            
                        }
                       
                    }

                }
                
            }
            
           
            // IF we are counting down
            else  {

                // Same thing here as above but in this case we want to go from 1 to 0
                _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, (_fadeTimer / (_setFadeTime / 2)));
                _fadeTimer -= Time.deltaTime;


                // If the fadeTimer is zero or lower the transition has ended
                if (_fadeTimer < 0)
                {
                    _isFade = false;
                    _fadeTimer = 0f;
                    _setOnce = true;
                }

                if (_setOnce)
                {
                    // Set the current node to Complete
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();

                    // Set the current node to inactive
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();

                    // Get the next node in line
                    _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                    

                    // Check if the next node actually exists
                    if (_allNodes[_activeNode] != null)
                    {
                        _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                    }

                    if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                    {

                        _animator.SetBool("skipIdle", true);

                    }
                    _setOnce = false;
                }
                
            }
        }
        
    }

    void Particles()
    {

        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {

                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }

        ParticleSystem _pSystem = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnParticleSystem();
        if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnParticleAction() == "Enable")
        {
            _pSystem.Play();
        }
        else if(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnParticleAction() == "Disable")
        {
            _pSystem.Stop();
        }
        

                    // Set the current node to Complete
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();

                    // Set the current node to inactive
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();

                    // Get the next node in line
                    _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();


                    // Check if the next node actually exists
                    if (_allNodes[_activeNode] != null)
                    {
                        _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                    }

                    if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                    {

                        _animator.SetBool("skipIdle", true);

                    }
                    if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() == "Idle")
                    {

                        _animator.SetBool("skipIdle", false);

                    }

        _isParticle = false;

    }

    // Simple timer for the Fade Node
    IEnumerator SolidTimer()
    {

        // Wait for the seconds the user set
        yield return new WaitForSeconds(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnSolidTime());

        // _Countup = false --- we can start counting down
        _countUp = false;
        _fadeTimer = 1;
    }


    // Simple timer for the Image Node
    IEnumerator SolidImageTimer()
    {
        
        yield return new WaitForSeconds(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnImageTime());
        _imageComplete = true;
        _imageFader = 1;

        
    }

    void DoImage()
    {


        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio() != null)
        {
            if (!_isPlaying)
            {

                _soundManager.clip = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnAudio();

                _soundManager.Play();
                if (!_soundManager.isPlaying)
                {
                    _isPlaying = false;
                }
                else
                {
                    _isPlaying = true;
                }

            }
        }

        GameObject _userImage = GameObject.Find("userImage");

        if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnImageMode() == "Appear")
        {
            if (!_imageComplete)
            {
                _userImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _userImage.GetComponent<Image>().sprite = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage();
                _userImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.width, _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.height);

                StartCoroutine(SolidImageTimer());
            }
            if (_imageComplete)
            {

                //_userImage.active = false;
                _userImage.GetComponent<Image>().sprite = null;
                _userImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                // Set the current node to Complete
                _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();

                // Set the current node to inactive
                _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();

                // Get the next node in line
                _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();


                // Check if the next node actually exists
                if (_allNodes[_activeNode] != null)
                {
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                }

                if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                {

                    _animator.SetBool("skipIdle", true);

                }

                _isImage = false;
                _soundManager.Stop();

            }

        }
        else
        {

            if(!_imageComplete)
            {
                _userImage.GetComponent<Image>().color = new Color(1, 1, 1, _imageFader);

                    _imageFader += Time.deltaTime;

                _userImage.GetComponent<Image>().sprite = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage();
                _userImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.width, _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.height);

                

                if(_imageFader > 1)
                {
                    StartCoroutine(SolidImageTimer());
                }

            }
            if (_imageComplete)
            {

                //_userImage.active = false;
                //_imageFader -= Time.deltaTime;

                InvokeRepeating("ImageCountdown", 0, 0.05f);
                _userImage.GetComponent<Image>().color = new Color(1, 1, 1, _imageFader);
                              
                

               


                if (_imageFader < 0)
                {
                    CancelInvoke();
                    _userImage.GetComponent<Image>().sprite = null;

                    // Set the current node to Complete
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetComplete();

                    // Set the current node to inactive
                    _allNodes[_activeNode].GetComponent<NodeObject>().SetInActive();

                    // Get the next node in line
                    _activeNode = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();


                    // Check if the next node actually exists
                    if (_allNodes[_activeNode] != null)
                    {
                        _allNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                    }

                    if (_allNodes[_activeNode].GetComponent<NodeObject>().ReturnAnim() != "Idle")
                    {

                        _animator.SetBool("skipIdle", true);

                    }

                    _isImage = false;
                    _soundManager.Stop();
                }

            }

            //     _userImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            //     _userImage.GetComponent<Image>().sprite = _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage();
            //     _userImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.width, _allNodes[_activeNode].GetComponent<NodeObject>().ReturnUserImage().textureRect.height);

        }



    }

    void ImageCountdown()
    {
        _imageFader -= 0.05f;
    }

    void SequenceTimer()
    {        
        
        if(_screenTimer != null)
        {
            _onScreenTimer += Time.deltaTime;
            _screenTimer.GetComponent<Text>().text = _onScreenTimer.ToString("F2");
            
        }
    }


}