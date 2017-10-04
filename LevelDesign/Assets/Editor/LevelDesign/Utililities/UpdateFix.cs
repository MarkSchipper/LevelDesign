using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UpdateFix : EditorWindow {

    private bool _updateFix;
    private bool _firstPerson;

    [MenuItem("Level Design/Update Helper")]
    static void ShowWindow()
    {
        UpdateFix window = EditorWindow.GetWindow<UpdateFix>(true, "Update Checker");
    }

	void OnGUI()
    {
        if(GUILayout.Button("Update Prefabs in scene"))
        {
            UpdatePrefabs();
        }
    }

    void UpdatePrefabs()
    {
        DestroyImmediate(GameObject.Find("GameManager"));
        DestroyImmediate(GameObject.Find("FirstPerson"));
        DestroyImmediate(GameObject.Find("Canvas"));
        DestroyImmediate(GameObject.Find("Camera_Target"));

        GameObject _player = Instantiate(Resources.Load("Characters/FirstPerson"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        _player.name = "FirstPerson";
        GameObject _GM = Instantiate(Resources.Load("SceneEditor/GameManager"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        _GM.name = "GameManager";
        GameObject _canvas = Instantiate(Resources.Load("SceneEditor/Canvas"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        _canvas.name = "Canvas";
        GameObject _CT = Instantiate(Resources.Load("SceneEditor/Camera_Target"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        _CT.name = "Camera_Target";

    }
}
