using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour {

    public float farClipPlane = 100;
    public UnityEngine.PostProcessing.PostProcessingProfile _customProfile;

    public float ReturnFarClipPlane()
    {
        return farClipPlane;
    }

    public UnityEngine.PostProcessing.PostProcessingProfile ReturnProfile()
    {
        return _customProfile;
    }

}
