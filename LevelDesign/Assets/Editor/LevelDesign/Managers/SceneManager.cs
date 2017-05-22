using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class SceneManager : EditorWindow {

    private GameObject _player;
    private GameObject _sceneCanvas;
    private GameObject _camSetup;
    private GameObject _gameManager;

    private GUISkin _skin;

    [MenuItem("Level Design/Managers/Scene Mananger")]
      
    static void ShowEditor()
    {
        SceneManager _sceneManager = EditorWindow.GetWindow<SceneManager>();
    }

    void OnEnable()
    {
        _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
    }

    void OnGUI()
    {
        GUI.skin = _skin;

        GUILayout.Label("Welcome to the Scene Manager", EditorStyles.boldLabel);

        if(GUILayout.Button("Set up new Scene"))
        {
            NewScene();
        }
    }

    void NewScene()
    {
        DestroyImmediate(GameObject.FindGameObjectWithTag("MainCamera"));

        _player = Instantiate(Resources.Load("Characters/PlayerCharacter") as GameObject);
            _player.name = "PlayerCharacter";
        _sceneCanvas = Instantiate(Resources.Load("SceneEditor/Canvas") as GameObject);
            _sceneCanvas.name = "Canvas";
        _camSetup = Instantiate(Resources.Load("SceneEditor/Camera_Target") as GameObject);
            _camSetup.name = "Camera_Target";
        _gameManager = Instantiate(Resources.Load("SceneEditor/GameManager") as GameObject);
            _gameManager.name = "GameManager";
    }
}
#endif