using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class NodeObject : MonoBehaviour
{

    [SerializeField]
    private int _charID;

    [SerializeField]
    private int _nodeID;
    [SerializeField]
    private int _inputID;
    [SerializeField]
    private int _outputID;

    [SerializeField]
    private string _anim;

    [SerializeField]
    private float _idleWait;

    [SerializeField]
    private float posX;

    [SerializeField]
    private float posY;

    private string windowTitle;
    [SerializeField]
    private GameObject _wayPoint;

    [SerializeField]
    private bool _completed;

    [SerializeField]
    private string _customAnimType;

    [SerializeField]
    private AnimationClip _customAnim;

    [SerializeField]
    private bool _isActive;

    [SerializeField]
    private AudioClip _audio;

    [SerializeField]
    private bool _autoPlay;

    [SerializeField]
    private float _fadeTimer;

    [SerializeField]
    private Color _fadeColour;

    [SerializeField]
    private float _solidTime;

    [SerializeField]
    private Texture2D _fadeTexture;

    [SerializeField]
    private string _fadeAction;

    [SerializeField]
    private string _fadeStart;


    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    private string _particleAction;

    [SerializeField]
    private Sprite _userImage;

    [SerializeField]
    private string _imageMode;

    [SerializeField]
    private float _imageTime;

    // CAMERA STUFF
    
    [SerializeField]
    private GameObject _camera;

    [SerializeField]
    private string _cameraEnd;

    [SerializeField]
    private float _cameraEndTime;



    public void setNodeID(int _id)
    {
        
        if (_id < 0)
        {
            Debug.Log("Setting node id");
            _nodeID += _id;
        }
        else {
            _nodeID = _id;
        }
    }

    public void setInputID(int _id)
    {
        _inputID = _id;
    }

    public void setOutputID(int _id)
    {
        if (_id < 0 && _outputID > 0)
        {
            Debug.Log("Setting output ID");
            _outputID += _id;
        }
        else {
            _outputID = _id;
        }
    }

    public void setAnimation(string _animation)
    {
        _anim = _animation;
    }

    public void setIdleWait(float _time)
    {
        _idleWait = _time;
    }

    public void setPosition(float x, float y)
    {
        posX = x;
        posY = y;
    }

    public float ReturnPosX()
    {
        return posX;
    }

    public float ReturnPosY()
    {
        return posY;
    }

    public void setTitle(string _title)
    {
        windowTitle = _title;
    }

    public string ReturnTitle()
    {
        return windowTitle;
    }

    public void SetWayPoint(GameObject _obj)
    {

        _wayPoint = _obj;
    }

    public GameObject ReturnWayPoint()
    {
        return _wayPoint;
    }

    public int ReturnOutputID()
    {
        return _outputID;
    }

    public bool ReturnComplete()
    {
        return _completed;
    }

    public void SetComplete()
    {
        _completed = true;
    }

    public string ReturnAnim()
    {
        return _anim;
    }

    public void SetActive()
    {
        _isActive = true;
    }

    public void SetInActive()
    {
        _isActive = false;
    }

    public bool ReturnActive()
    {
        return _isActive;
    }



    public float ReturnIdleWait()
    {
        return _idleWait;
    }

    public void SetCustomAnim(AnimationClip _cAnim)
    {
        _customAnim = _cAnim;
    }

    public AnimationClip ReturnCustomAnim()
    {
        return _customAnim;
    }

    

    public void SetCustomAnimType(string _cAnimType)
    {
        _customAnimType = _cAnimType;
    }

    public string ReturnCustomAnimType()
    {
        return _customAnimType;
    }

    public void SetAudio(AudioClip _sound)
    {
        _audio = _sound;
    }

    public AudioClip ReturnAudio()
    {
        return _audio;
    }

    public void SetCharID(int _id)
    {

        _charID = _id;

    }

    public int ReturnCharID()
    {
        return _charID;
    }

    public void SetName(int _id)
    {
        if (ReturnAnim() == "CameraAnimation")
        {
            this.name = "CameraNode" + (_nodeID - 1);
        }
        else
        {
            this.name = "Node" + (_nodeID - 1);
        }
    }

    public void SetCameraName(GameObject _obj)
    {
        
        _obj.name = "Cinematics_Camera" + (_nodeID);
        #if UNITY_EDITOR
        AssetDatabase.RenameAsset(_obj.GetComponent<Animator>().runtimeAnimatorController.name, "Cinematics_Controller" + _nodeID);
        #endif
    }

    public int ReturnNodeID()
    {
        return _nodeID;
    }

    public void AutoPlay(bool _play)
    {
        _autoPlay = _play;
    }

    public bool ReturnAutoPlay()
    {
        return _autoPlay;
    }

    public void SetFadeTime(float _fadeTime)
    {
        _fadeTimer = _fadeTime;
    }

    public void SetFadeTexture(Color _tex)
    {
        _fadeColour = _tex;

        
    }

    public float ReturnFadeTime()
    {
        return _fadeTimer;
    }

    public void SetSolidTime(float _solid)
    {
        _solidTime = _solid;
    }

    public void SetFadeAction(string _nwFadeAction)
    {
        _fadeAction = _nwFadeAction;
    }

    public string ReturnFadeAction()
    {
        return _fadeAction;
    }

    public float ReturnSolidTime()
    {
        return _solidTime;
    }

    public void SetFadeAnimStart(string _animStart)
    {
        _fadeStart = _animStart;
    }

    public string ReturnFadeAnimStart()
    {
        return _fadeStart;
    }

    public void SetParticleSystem(ParticleSystem _pSystem)
    {
        _particleSystem = _pSystem;
    }

    public void SetParticleAction(string _pAction)
    {
        _particleAction = _pAction;
    }

    public ParticleSystem ReturnParticleSystem()
    {
        return _particleSystem;
    }

    public string ReturnParticleAction()
    {
        return _particleAction;
    }

    public void SetUserImage(Sprite _img)
    {
        _userImage = _img;
    }

    public void SetImageMode(string _mode)
    {
        _imageMode = _mode;
    }

    public void SetImageTime(float _time)
    {
        _imageTime = _time;
    }

    public Sprite ReturnUserImage()
    {
        return _userImage;
    }

    public string ReturnImageMode()
    {
        return _imageMode;
    }

    public float ReturnImageTime()
    {
        return _imageTime;
    }

    public void CanvasCheck()
    {
        if(GameObject.Find("Canvas") != null)
        {
            if(GameObject.Find("Canvas").GetComponent<Image>() == null)
            {
                GameObject.Find("Canvas").AddComponent<Image>();
                GameObject.Find("Canvas").GetComponent<Image>().color = new Color(_fadeColour.r, _fadeColour.g, _fadeColour.b, 0);
            }
            else
            {
                GameObject.Find("Canvas").GetComponent<Image>().color = new Color(_fadeColour.r, _fadeColour.g, _fadeColour.b, 0);
            }
        } 
        else
        {
            GameObject _canvas = new GameObject();
            _canvas.name = "Canvas";
            _canvas.AddComponent<Canvas>();
            _canvas.AddComponent<CanvasGroup>();
            _canvas.AddComponent<CanvasRenderer>();

            _canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.GetComponent<RectTransform>().position = new Vector2(0, 0);
            _canvas.AddComponent<Image>();

            _canvas.GetComponent<Image>().color = new Color(_fadeColour.r, _fadeColour.g, _fadeColour.b, 0);
            
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                      //
    //                                          CAMERA STUFF                                                //
    //                                                                                                      //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////

   

    public void SetCamera(GameObject _cam)
    {
        _camera = _cam;
    }

    public GameObject ReturnCamera()
    {
        return _camera;
    }

    public void SetCameraEnd(string _mode)
    {
        _cameraEnd = _mode;
    }

    public string ReturnCameraEnd()
    {
        return _cameraEnd;
    }

    public void SetCameraEndTime(float _time)
    {
        _cameraEndTime = _time;
    }

    public float ReturnCameraEndTime()
    {
        return _cameraEndTime;
    }

  

}
