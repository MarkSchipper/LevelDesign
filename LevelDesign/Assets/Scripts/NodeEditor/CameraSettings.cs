using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraSettings : MonoBehaviour {

    [SerializeField]
    private bool _autoCreateCamera;

    [SerializeField]
    private bool _cameraName;

    [SerializeField]
    private bool _cameraSettings;

    [SerializeField]
    private GameObject _initialCamera;

    [SerializeField]
    private bool _initialCameraAnimation;

    [SerializeField]
    private GameObject _initialNode;

    public CameraSettings()
    {

    }

    public void SetInitialCamera(GameObject _go)
    {
        _initialCamera = _go;
    }
    
    public GameObject ReturnInitialCamera()
    {
        return _initialCamera;
    }

    public void SetAutoCreate(bool _auto)
    {
        _autoCreateCamera = _auto;
    }

    public void SetCameraName(bool _onScreen)
    {
        _cameraName = _onScreen;
    }

    public void SetCameraSettings(bool _settings)
    {
        _cameraSettings = _settings;
    }

    public bool ReturnAutoCreateCamera()
    {
        return _autoCreateCamera;
    }

    public bool ReturnCameraName()
    {
        return _cameraName;
    }

    public bool ReturnCameraSettings()
    {
        return _cameraSettings;
    }

    public void SetInitialCameraAnimation(bool _anim)
    {
        _initialCameraAnimation = _anim;
    }

    public bool ReturnInitialCameraAnimation()
    {
        return _initialCameraAnimation;
    }

    public void SetInitialNode(GameObject _go)
    {
        _initialNode = _go;
    }

    public GameObject ReturnInitialNode()
    {
        return _initialNode;
    }
}
