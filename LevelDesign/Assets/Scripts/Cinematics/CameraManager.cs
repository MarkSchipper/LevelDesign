using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using UnityEngine.UI;
using System;

public class CameraManager : MonoBehaviour {

    private GameObject[] _cameraNodes;
    private float _cameraTimer = 0f;
    private bool _timerEnded = false;
    private int _activeNode;

    private GameObject _camSettings;

    private bool _cameraName;
    private bool _cameraSettings;

    private string _cameraNameText = "test";
    private Text _cameraCanvasName;
    private Text _cameraCanvasSettings;

	// Use this for initialization
	void Start () {

        _cameraNodes = GameObject.FindGameObjectsWithTag("CameraNode").OrderBy(go => int.Parse(go.name.Substring(10))).ToArray();
        _camSettings = GameObject.Find("CameraSettings");

        _cameraName = _camSettings.GetComponent<CameraSettings>().ReturnCameraName();
        _cameraSettings = _camSettings.GetComponent<CameraSettings>().ReturnCameraSettings();

        if(_cameraName)
        {
            CameraNameSetup();
        }
        else
        {
            _cameraCanvasName.text = "";
        }

        if(_cameraSettings)
        {
            CameraSettingsSetup();
        }
        else
        {
            _cameraCanvasSettings.text = "";
        }

        if (_camSettings.GetComponent<CameraSettings>().ReturnInitialCameraAnimation())
        {

            GameObject.Find("CameraSettings").GetComponent<CameraSettings>().ReturnInitialNode().GetComponent<NodeObject>().SetActive();

            for (int i = 0; i < _cameraNodes.Length; i++)
            {

                if (_cameraNodes[i].name != _camSettings.GetComponent<CameraSettings>().ReturnInitialNode().name)
                {
                    _cameraNodes[i].GetComponent<NodeObject>().ReturnCamera().SetActive(false);


                }
                else
                {
                    _activeNode = i;
                    _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(true);
                    ChangeCameraName(_cameraNodes[_activeNode].name);
                    ChangeCameraSettings(_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera());
                }
            }
        }

        else
        {
            for (int i = 0; i < _cameraNodes.Length; i++)
            {
                if(_cameraNodes[i].GetComponent<NodeObject>().ReturnAnim() == "CameraAnimation")
                {
                    _activeNode = i;
                    _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(true);
                    _cameraNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                    ChangeCameraName(_cameraNodes[_activeNode].name);
                    ChangeCameraSettings(_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera());
                }
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

       // Debug.Log("Current Active Node: " + _cameraNodes[_activeNode] + " which has " + _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCameraEnd());

        if (!_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnComplete())
        {
            if (_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnActive())
            {
                if (_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCameraEnd() == "Time")
                {
                  

                    if(_cameraTimer < _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCameraEndTime())
                    {
                        _cameraTimer += Time.deltaTime;
                    }

                    else
                    {
                        _timerEnded = true;
                    }

                    if (_timerEnded)
                    {
                        _timerEnded = false;
                        _cameraTimer = 0;
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                        Debug.Log("Completed " + _cameraNodes[_activeNode]);
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(false);
                        _activeNode = _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(true);

                        ChangeCameraName(_cameraNodes[_activeNode].name);
                        ChangeCameraSettings(_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera());


                    }
                 }
                else
                {
                    if(_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().SetComplete();
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(false);
                        _activeNode = _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnOutputID();
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().SetActive();
                        _cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera().SetActive(true);
                        
                    }
                }

                ChangeCameraName(_cameraNodes[_activeNode].name);
                ChangeCameraSettings(_cameraNodes[_activeNode].GetComponent<NodeObject>().ReturnCamera());
            }
        }
	}

    void ChangeCameraName(string _name)
    {
        _cameraCanvasName.text = _name;
    }

    void ChangeCameraSettings(GameObject _go)
    {
       // _cameraCanvasSettings.text = " FOV: [ " + _go.GetComponent<Camera>().fov.ToString() + " ] Focal Length: [ " + _go.GetComponent<UnityEngine.PostProcessing.Utilities.PostProcessor>().ReturnFocalLength() + " ] Focal Distance: [ " + _go.GetComponent<UnityEngine.PostProcessing.Utilities.PostProcessor>().ReturnFocusDistance() + " ] Aperture: [ " + _go.GetComponent<UnityEngine.PostProcessing.Utilities.PostProcessor>().ReturnAperture() + " ] ";
    }

    void CameraNameSetup()
    {
        CanvasCheck();

        _cameraCanvasName = GameObject.Find("CameraNameUI").GetComponent<Text>();     

    }

    void CameraSettingsSetup()
    {
        CanvasCheck();
        _cameraCanvasSettings = GameObject.Find("CameraSettingsUI").GetComponent<Text>();
    }

    void CanvasCheck()
    {
        if(GameObject.Find("Canvas") == null)
        {
            GameObject _myCanvas = new GameObject();
            _myCanvas.name = "Canvas";
            _myCanvas.AddComponent<Canvas>();
            _myCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            if (GameObject.Find("LowerEpicBar") == null)
            {

                GameObject _myBars = new GameObject();
                _myBars.name = "CameraBar";
                _myBars.transform.parent = _myCanvas.transform;

                _myBars.AddComponent<Image>();
                _myBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _myBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _myBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 50);
            }


        }

        else
        {
            GameObject _myCanvas = GameObject.Find("Canvas");
            if (GameObject.Find("LowerEpicBar") == null)
            {

                GameObject _myBars = new GameObject();
                _myBars.name = "CameraBar";
                _myBars.transform.parent = _myCanvas.transform;

                _myBars.AddComponent<Image>();
                _myBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _myBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _myBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 50);
            }
            else
            {
                
            }
        }
    }
}
