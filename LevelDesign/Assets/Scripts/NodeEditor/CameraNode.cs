using UnityEngine;
using System.Collections;
using UnityEditor;
using System;


#if UNITY_EDITOR

using UnityEditor.Animations;


public class CameraNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    private Camera _camera;

    private CameraMode _mode;

    private bool _autoCreate;
    private bool _createdCamera = false;

    private GameObject _cinematicsCamera;

    private bool _turnedOff = false;

    private bool _isActive;

    private float _cameraEndTime;
    private string _cameraNote = "Note here";

    public CameraNode()
    {
        windowTitle = "Camera Node";
        hasInputs = true;

        
    }

    void OnEnable()
    {
        

        if (GameObject.Find("CameraSettings") != null)
        {
            _autoCreate = GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnAutoCreateCamera();

            
        }
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        if (_autoCreate && !_createdCamera && !_isActive && GameObject.Find("Cinematics_Camera" + base.ReturnID()) == null)
        {

            _cinematicsCamera =  Instantiate(GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnInitialCamera(), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            _cinematicsCamera.name = "Cinematics_Camera" + base.ReturnID();
            AnimatorController _controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Cinematic_controller" + base.ReturnID() + ".controller");

            _cinematicsCamera.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Cinematic_controller" + base.ReturnID()) as RuntimeAnimatorController;
                    
            GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCamera(_cinematicsCamera);
            _isActive = true;

            _createdCamera = true;
        }

        if(GameObject.Find("Cinematics_Camera" + base.ReturnID()) != null)
        {
            GameObject.Find("Cinematics_Camera" + base.ReturnID()).GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Cinematic_controller" + base.ReturnID()) as RuntimeAnimatorController;
        }

        GUILayout.Label("You are now editing ");
        GUILayout.Label("[Cinematics_Camera" + base.ReturnID() + "]");
        GUILayout.Space(10);

        if (GameObject.Find("CameraNode" + base.ReturnID()) != null)
        {

            _cinematicsCamera = (GameObject)EditorGUILayout.ObjectField(GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().ReturnCamera(), typeof(GameObject), false);

        }
        if (!_turnedOff)
        {
            if (GUILayout.Button("Click to turn the camera OFF"))
            {
                _cinematicsCamera.SetActive(false);
                _turnedOff = true;
            }
        }
        else
        {
            if (GUILayout.Button("Click to turn the camera ON"))
            {
                _cinematicsCamera.SetActive(true);
                _turnedOff = false;
            }
        }

        GUILayout.Label("When to switch camera?");
        _mode = (CameraMode)EditorGUILayout.EnumPopup("Action:", _mode);

        if (GameObject.Find("CameraNode" + base.ReturnID()) != null)
        {
            GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCameraEnd(_mode.ToString());
        }

        else
        {
            Debug.Log("Cant find Camera: CameraNode" + base.ReturnID());
        }

        if(_mode == CameraMode.Time)
        {
            GUILayout.Label("End after: ");
            float.TryParse(EditorGUILayout.TextField("Animate for: ", _cameraEndTime.ToString()), out _cameraEndTime);

            if (_cameraEndTime > 0)
            {
                GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCameraEndTime(_cameraEndTime);
            }
        }
        
        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }
    }


    public void SetAutoCreate(bool _auto)
    {
        _autoCreate = _auto;
    }


    public void SetCamera(GameObject _cam)
    {
        _cinematicsCamera = _cam;
    }

    public void SetCameraMode(string _camMode)
    {
        if(_camMode == "AnimationEnd")
        {
            _mode = CameraMode.AnimationEnd;
        }
        else
        {
            _mode = CameraMode.Time;
        }
    }

    public void SetCameraEndTime(float _time)
    {
        _cameraEndTime = _time;
    }

    public override void Tick(float deltaTime)
    {

    }

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {

            input1 = input;

        }

    }

    public override void DrawCurves()
    {
        if (input1)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y;
            rect.width = 1;
            rect.height = 1;

            NodeEditor.DrawNodeCurve(input1.windowRect, rect);
        }
    }
}
#endif