using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;

#if UNITY_EDITOR

public class SceneManager : EditorWindow {

    private static GameObject _player;
    private static GameObject _gameManager;
    private static GameObject _spawnPoint;
  
    public static void ShowNewScene()
    {
        if(GUILayout.Button("Create a new Scene"))
        {
            NewScene();
        }
    }

    public static void ShowUpdateScene()
    {
       
        if(GUILayout.Button("Restore Defaults"))
        {
            UpdateScene();
        }
    }

    static void NewScene()
    {

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        DestroyImmediate(GameObject.FindGameObjectWithTag("MainCamera"));

        _player = Instantiate(Resources.Load("Characters/FirstPerson") as GameObject);
        _player.name = "FirstPerson";
        _gameManager = Instantiate(Resources.Load("SceneEditor/GameManager") as GameObject);
        _gameManager.name = "GameManager";

        _spawnPoint = Instantiate(Resources.Load("World_Building/PlayerSpawnPoint") as GameObject);
        _spawnPoint.name = "SpawnPoint";

        GameObject _sceneSettings = new GameObject();
        _sceneSettings.name = "LevelSettings";
        _sceneSettings.AddComponent<LevelSettings>();

    }

    static void UpdateScene()
    {
        DestroyImmediate(GameObject.FindGameObjectWithTag("Player"));
        DestroyImmediate(GameObject.FindGameObjectWithTag("GameManager"));
        DestroyImmediate(GameObject.FindGameObjectWithTag("SpawnPoint"));
        DestroyImmediate(GameObject.Find("LevelSettings"));

        _player = Instantiate(Resources.Load("Characters/FirstPerson") as GameObject);
        _player.name = "FirstPerson";
        _gameManager = Instantiate(Resources.Load("SceneEditor/GameManager") as GameObject);
        _gameManager.name = "GameManager";

        _spawnPoint = Instantiate(Resources.Load("World_Building/PlayerSpawnPoint") as GameObject);
        _spawnPoint.name = "SpawnPoint";

        GameObject _sceneSettings = new GameObject();
        _sceneSettings.name = "LevelSettings";
        _sceneSettings.AddComponent<LevelSettings>();
    }
}
#endif