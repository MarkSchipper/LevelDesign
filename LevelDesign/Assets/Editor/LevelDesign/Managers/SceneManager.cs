using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class SceneManager : EditorWindow {

    private static GameObject _player;
    private static GameObject _gameManager;
    private static GameObject _spawnPoint;

    private static GameObject[] _allEnemies;
    private static GameObject[] _allNPC;
    private static int _missingComponents = 0;
    private static int _oldComponents = 0;

    private static List<string> _objectNames = new List<string>();

    private static bool _checkedEverything;

    public static void ShowNewScene()
    {
        if(GUILayout.Button("Set up new Scene"))
        {
            NewScene();
        }
        _checkedEverything = false;
    }

    public static void ShowUpdateScene()
    {
        if (!_checkedEverything)
        {
            CheckEverything();
            _checkedEverything = true;
        }
        if(_missingComponents > 0)
        {
            GUILayout.Label(_missingComponents + " objects have an empty component");
        }   
        if(_oldComponents > 0)
        {
            GUILayout.Label(_oldComponents + " objects are outdated");
        }

        if(_missingComponents > 0 || _oldComponents > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Please check the following GameObjects - If it is an Enemy, re add it to the game", EditorStyles.boldLabel);
            GUILayout.Space(15);
            for (int i = 0; i < _objectNames.Count; i++)
            {
                GUILayout.Label(_objectNames[i]);

            }
        }
        
    }

    static void NewScene()
    {
        DestroyImmediate(GameObject.FindGameObjectWithTag("MainCamera"));

        _player = Instantiate(Resources.Load("Characters/FirstPerson") as GameObject);
            _player.name = "FirstPerson";
        _gameManager = Instantiate(Resources.Load("SceneEditor/GameManager") as GameObject);
            _gameManager.name = "GameManager";

        _spawnPoint = Instantiate(Resources.Load("World_Building/PlayerSpawnPoint") as GameObject);
        _spawnPoint.name = "SpawnPoint";
    }

    static void CheckEverything()
    {

        _allEnemies = GameObject.FindGameObjectsWithTag("EnemyMelee");
        _allNPC = GameObject.FindGameObjectsWithTag("NPC");

        for (int i = 0; i < _allEnemies.Length; i++)
        {
            Component[] components = _allEnemies[i].GetComponents<Component>();
            for (int j = 0; j < components.Length; j++)
            {
                if(components[j] == null)
                {
                    _missingComponents++;
                    _objectNames.Add(_allEnemies[i].gameObject.name);
                }
            }
           
        }

        for (int k = 0; k < _allNPC.Length; k++)
        {
            Component[] components = _allNPC[k].GetComponents<Component>();
            for (int l = 0; l < components.Length; l++)
            {
                if(components[l] == null)
                {
                    _objectNames.Add(_allNPC[l].gameObject.name);
                    _missingComponents++;
                }
            }
        }

        if(GameObject.FindGameObjectWithTag("Player").GetComponent<Component>() == null)
        {
            _missingComponents++;
        }
    }

   
   
    public static void ResetCounters()
    {
        _oldComponents = 0;
        _missingComponents = 0;
        _objectNames.Clear();
        _checkedEverything = false;
    }

}
#endif