using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEngine.UI;

public enum CameraMode
{
    AnimationEnd,
    Time,
}
#if UNITY_EDITOR
public class CameraSetupNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;
    
    private bool _autoCreateCamera = false;
    private bool _cameraOnScreen = false;
    private bool _cameraSettings = false;

    private GameObject _initialCamera;
    private bool _initialCameraAnimation;

    private float _cameraEndTime;

    private CameraMode _camMode;

    public CameraSetupNode()
    {
        windowTitle = "Camera Setup Node";
        hasInputs = false;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Auto Create Camera?", GUILayout.MinWidth(200));
        _autoCreateCamera = EditorGUILayout.Toggle(_autoCreateCamera);
        GUILayout.EndHorizontal();
              
        

        GUILayout.BeginHorizontal();
        GUILayout.Label("Display Camera Name?", GUILayout.MinWidth(200));
        _cameraOnScreen = EditorGUILayout.Toggle(_cameraOnScreen);
        GUILayout.EndHorizontal();
                

        GUILayout.BeginHorizontal();
        GUILayout.Label("Display Camera Settings?", GUILayout.MinWidth(200));
        _cameraSettings = EditorGUILayout.Toggle(_cameraSettings);
        GUILayout.EndHorizontal();

        if(_autoCreateCamera)
        {
            GUILayout.Label("Which Camera to clone?", GUILayout.MinWidth(200));
            _initialCamera = (GameObject)EditorGUILayout.ObjectField(_initialCamera, typeof(GameObject), true);

            if(_cameraOnScreen)
            {
                if (GameObject.Find("CameraNameUI") == null)
                {
                    GameObject _myCanvas = GameObject.Find("Canvas");
                    GameObject _camName = new GameObject();
                    _camName.name = "CameraNameUI";
                    _camName.transform.parent = _myCanvas.transform;

                    _camName.AddComponent<Text>();
                    _camName.GetComponent<Text>().color = new Color(1, 1, 1, 1);
                    _camName.GetComponent<Text>().fontSize = 25;
                    _camName.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
                    _camName.GetComponent<RectTransform>().anchoredPosition = new Vector2(((_myCanvas.GetComponent<RectTransform>().rect.width / 2) * -1) + 200, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 25);
                }
            }

            if(_cameraSettings)
            {
                if (GameObject.Find("CameraSettingsUI") == null)
                {
                    GameObject _myCanvas = GameObject.Find("Canvas");
                    GameObject _camSettings = new GameObject();
                    _camSettings.name = "CameraSettingsUI";
                    _camSettings.transform.parent = _myCanvas.transform;

                    _camSettings.AddComponent<Text>();
                    _camSettings.GetComponent<Text>().color = new Color(1, 1, 1, 1);
                    _camSettings.GetComponent<Text>().fontSize = 25;
                    _camSettings.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 100);
                    _camSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(((_myCanvas.GetComponent<RectTransform>().rect.width / 2) * -1) + 850, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 25);
                }
            }

            if(_initialCamera != null)
            {
                GameObject.Find("CameraSettings").GetComponent<CameraSettings>().SetInitialCamera(_initialCamera);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Has animation?", GUILayout.MinWidth(200));
                _initialCameraAnimation = EditorGUILayout.Toggle(_initialCameraAnimation);
                GUILayout.EndHorizontal();
                if(_initialCameraAnimation)
                {
                    GUILayout.Label("When to switch camera?");
                    _camMode = (CameraMode)EditorGUILayout.EnumPopup("Action:", _camMode);

                    //Debug.Log("BaseID: " + base.ReturnID());

                    GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCameraEnd(_camMode.ToString());
                    GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCamera(_initialCamera);

                    if(_camMode == CameraMode.Time)
                    {
                        GUILayout.Label("End after: ");
                        float.TryParse(EditorGUILayout.TextField("Animate for: ", _cameraEndTime.ToString()), out _cameraEndTime);

                        if(_cameraEndTime > 0)
                        {
                            GameObject.Find("CameraNode" + base.ReturnID()).GetComponent<NodeObject>().SetCameraEndTime(_cameraEndTime);
                        }
                        
                    }
                    GameObject.Find("CameraSettings").GetComponent<CameraSettings>().SetInitialNode(GameObject.Find("CameraNode" + base.ReturnID()).gameObject);

                }
            }
        }



        if (GameObject.Find("CameraSettings") == null)
        {

            GameObject _camSettings = new GameObject();
            _camSettings.name = "CameraSettings";
            _camSettings.transform.parent = GameObject.FindGameObjectWithTag("NodeParent").transform;
            _camSettings.AddComponent<CameraSettings>();

        }
        else
        {
            GameObject _findSettings = GameObject.Find("CameraSettings");
            _findSettings.GetComponent<CameraSettings>().SetAutoCreate(_autoCreateCamera);
            _findSettings.GetComponent<CameraSettings>().SetCameraName(_cameraOnScreen);
            _findSettings.GetComponent<CameraSettings>().SetCameraSettings(_cameraSettings);
            _findSettings.GetComponent<CameraSettings>().SetInitialCameraAnimation(_initialCameraAnimation);
        }

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }


    }

    public void SetInitialCamera(GameObject _cam)
    {
        _initialCamera = _cam;
    }

    public void SetAutoCreateCamera(bool _auto)
    {
        _autoCreateCamera = _auto;
    }

    public void SetCameraName(bool _name)
    {
        _cameraOnScreen = _name;
    }

    public void SetCameraSettings(bool _settings)
    {
        _cameraSettings = _settings;
    }

    public void SetCameraMode(string _mode)
    {
        if(_mode == "AnimationEnd")
        {
            _camMode = CameraMode.AnimationEnd;
        }
        else
        {
            _camMode = CameraMode.Time;
        }
    }

    public void SetHasAnimation(bool _set)
    {
        _initialCameraAnimation = _set;
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