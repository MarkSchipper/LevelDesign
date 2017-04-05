using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HelpMenu : EditorWindow {

    private TextAsset _mainHelp;
    private TextAsset _managerHelp;
    private TextAsset _actorManagerHelp;
    private TextAsset _itemManagerHelp;
    private TextAsset _sceneManagerHelp;
    private TextAsset _zoneManagerHelp;
    private TextAsset _playerSettingsHelp;
    private TextAsset _playerSpellsHelp;
    private TextAsset _enemiesHelp;
    private TextAsset _worldBuilderHelp;
    private TextAsset _questSystemHelp;

    private string _mainHelpContent;
    private string _managersContent;
    private string _actorManagerContent;
    private string _itemManagerContent;
    private string _sceneMangerContent;
    private string _zoneManagerContent;

    private string _playerSettingsContent;
    private string _playerSpellsContent;
    private string _enemyManagerContent;

    private string _worldBuilderContent;
    private string _questSystemContent;

    private bool _isManagers = false;
    private bool _isPlayer = false;
    private bool _isEnemies = false;
    private bool _isWorldBuilder = false;
    private bool _isQuestSystem = false;

    private bool _isActorManager = false;
    private bool _isItemManager = false;
    private bool _isSceneManager = false;
    private bool _isZoneManager = false;

    private bool _isPlayerSettings = false;
    private bool _isSpellManager = false;

    private GUISkin _skin;

    private Vector2 _scrollPos;

    [MenuItem("Level Design/Help")]

    static void ShowWindow()
    {
        HelpMenu _help = EditorWindow.GetWindow<HelpMenu>();
    }

    void OnEnable()
    {
        _mainHelp = Resources.Load("Text/Help") as TextAsset;
        _actorManagerHelp = Resources.Load("Text/ActorManager") as TextAsset;
        _itemManagerHelp = Resources.Load("Text/ItemManager") as TextAsset;
        _sceneManagerHelp = Resources.Load("Text/SceneManager") as TextAsset;
        _zoneManagerHelp = Resources.Load("Text/ZoneManager") as TextAsset;
        _playerSettingsHelp = Resources.Load("Text/PlayerSettings") as TextAsset;
        _playerSpellsHelp = Resources.Load("Text/SpellManager") as TextAsset;
        _enemiesHelp = Resources.Load("Text/EnemyManager") as TextAsset;
        _worldBuilderHelp = Resources.Load("Text/WorldBuilder") as TextAsset;
        _questSystemHelp = Resources.Load("Text/QuestManager") as TextAsset;

        _skin = Resources.Load("Skins/LevelDesign") as GUISkin;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUI.skin = _skin;
        if (!_isManagers && !_isPlayer && !_isEnemies && !_isWorldBuilder && !_isQuestSystem)
        {

            if (GUILayout.Button("Managers"))
            {
                _isManagers = true;
            }

            if (GUILayout.Button("Player"))
            {
                _isPlayer = true;
            }

            if (GUILayout.Button("Enemies"))
            {
                _isEnemies = true;
            }

            if (GUILayout.Button("World Builder"))
            {
                _isWorldBuilder = true;
            }

            if (GUILayout.Button("Quest System"))
            {
                _isQuestSystem = true;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _mainHelpContent = GUILayout.TextArea(_mainHelp.text);

            EditorGUILayout.EndScrollView();
        }

        if(_isManagers)
        {
            if(GUILayout.Button("Actor Manager"))
            {
                _isActorManager = true;
            }

            if (GUILayout.Button("Item Manager"))
            {
                _isItemManager = true;
            }

            if (GUILayout.Button("Scene Manager"))
            {
                _isSceneManager = true;
            }

            if (GUILayout.Button("Zone Manager"))
            {
                _isZoneManager = true;
            }
            GUILayout.Space(50);
            if(GUILayout.Button("BACK"))
            {
                _isManagers = false;
            }
        }

        if(_isManagers)
        {
            if(_isActorManager)
            {
                ActorManager();
            }
            if(_isItemManager)
            {
                ItemManager();
            }
            if(_isSceneManager)
            {
                SceneManager();
            }
            if(_isZoneManager)
            {
                ZoneManager();
            }
        }

        if(_isPlayer)
        {
            if(GUILayout.Button("Player Settings"))
            {
                _isPlayerSettings = true;
            }
            if(GUILayout.Button("Player Spell Manager"))
            {
                _isSpellManager = true;
            }

            if(_isPlayerSettings)
            {
                PlayerSettings();
            }
            if(_isSpellManager)
            {
                PlayerSpellManager();
            }

            GUILayout.Space(50);

            if(GUILayout.Button("BACK"))
            {
                _isPlayer = false;
            }
        }

        if(_isEnemies)
        {
            Enemies();
        }

        if(_isWorldBuilder)
        {
            WorldBuilder();
        }

        if(_isQuestSystem)
        {
            QuestSystem();
        }

    }

    void ActorManager()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _actorManagerContent = GUILayout.TextArea(_actorManagerHelp.text);

        EditorGUILayout.EndScrollView();

        if(GUILayout.Button("BACK"))
        {
            _isActorManager = false;
            
        }
    }

    void ItemManager()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _itemManagerContent = GUILayout.TextArea(_itemManagerHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isItemManager = false;

        }
    }

    void SceneManager()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _sceneMangerContent = GUILayout.TextArea(_sceneManagerHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isSceneManager = false;

        }

    }

    void ZoneManager()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _zoneManagerContent = GUILayout.TextArea(_zoneManagerHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isZoneManager = false;

        }
    }

    void PlayerSettings()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _playerSettingsContent = GUILayout.TextArea(_playerSettingsHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isPlayerSettings = false;

        }
    }

    void PlayerSpellManager()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _playerSpellsContent = GUILayout.TextArea(_playerSpellsHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isSpellManager = false;

        }
    }

    void Enemies()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _enemyManagerContent = GUILayout.TextArea(_enemiesHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isEnemies = false;

        }
    }

    void WorldBuilder()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _worldBuilderContent = GUILayout.TextArea(_worldBuilderHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isWorldBuilder = false;

        }

    }

    void QuestSystem()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _questSystemContent = GUILayout.TextArea(_questSystemHelp.text);

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("BACK"))
        {
            _isQuestSystem = false;

        }
    }
}
