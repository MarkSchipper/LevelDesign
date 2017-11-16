using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelDesignManager : EditorWindow {

    private int _tabIndex = -1;
    private int _playerTabIndex = -1;
    private int _playerSpellTabIndex = -1;
    private int _npcTabIndex = -1;
    private int _npcDBTabIndex = -1;
    private int _npcGameTabIndex = -1;
    private int _enemyTabIndex = -1;
    private int _enemyDBTabIndex = -1;
    private int _enemyGameTabIndex = -1;
    private int _worldTabIndex = -1;
    private int _zoneTabIndex = -1;
    private int _feedbackTabIndex = -1;
    private int _feedbackDBTabIndex = -1;
    private int _feedbackGameTabIndex = -1;
    private int _sceneTabIndex = -1;
    private int _itemTabIndex = -1;
    private int _questTabIndex = -1;
    private int _lootTabIndex = -1;

    private bool _npcTabSwitch;
    private bool _enemyTabSwitch;

    private GUIStyle _style;

    [MenuItem("Level Design/Level Design Tool")]
    static void ShowEditor()
    {
        LevelDesignManager _levelDesignManager = EditorWindow.GetWindow<LevelDesignManager>();
        _levelDesignManager.Init();
    }

    void Init()
    {
        //_style = new GUIStyle();
    }
    void OnGUI()
    {
        GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);

        _tabIndex = GUILayout.Toolbar(_tabIndex, new string[] { "Player", "NPC", "Enemies", "World", "Scene Setup", "Quests" });
        switch (_tabIndex)
        {
            case 0:
                GUI.backgroundColor = Color.cyan;
                ShowPlayer();
                break;
            case 1:
                GUI.backgroundColor = new Color(0.5f, 0.7f, 0.2f, 1f);
                ShowNPC();
                break;
            case 2:
                GUI.backgroundColor = new Color(0.2f, 0.7f, 0.5f, 1);
                ShowEnemies();
                break;
            case 3:
                GUI.backgroundColor = new Color(0.7f, 0.5f, 0.4f, 1);
                ShowWorld();
                break;
            case 4:
                GUI.backgroundColor = new Color(0.4f, 0.5f, 0.6f, 1);
                ShowSceneManager();
                break;
            case 5:
                GUI.backgroundColor = new Color(0.5f, 0.7f, 0.4f, 1);
                ShowQuestManager();
                break;
            default:
                break;
        }
    }

    void ShowPlayer()
    {
        _playerTabIndex = GUILayout.Toolbar(_playerTabIndex, new string[] { "Player Settings", "Player Spells", "Player Statistics", "Item Manager" });
        switch (_playerTabIndex)
        {
            case 0:
                GUI.backgroundColor = new Color(0.8f, 0.8f,0.8f,1);
                PlayerSetup.ShowPlayerSettings();
                break;
            case 1:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowPlayerSpells();
                break;
            case 2:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                PlayerStats.ShowPlayerStatistics();
                break;
            case 3:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowItemManager();
                break;
            default:
                break;  
        }
    }

    void ShowPlayerSpells()
    {
        _playerSpellTabIndex = GUILayout.Toolbar(_playerSpellTabIndex, new string[] {"Add Spell", "Edit Spell", "Delete Spell" });

        switch (_playerSpellTabIndex)
        {
            case 0:
                CombatSystem.PlayerSpells.GetResources();
                CombatSystem.PlayerSpells.ShowAddPlayerSpells();
                break;
            case 1:
                CombatSystem.PlayerSpells.GetResources();
                CombatSystem.CombatDatabase.GetAllSpells();
                CombatSystem.PlayerSpells.ShowEditPlayerSpells();
                break;
            case 2:
                
                CombatSystem.PlayerSpells.ShowDeletePlayerSpells();
                break;
            default:
                break;
        }
    }

    void ShowItemManager()
    {
        _itemTabIndex = GUILayout.Toolbar(_itemTabIndex, new string[] { "Add Item", "Edit Item", "Delete Item" });
        switch (_itemTabIndex)
        {
            case 0:
                ItemManager.ShowAddItem();
                break;
            case 1:
                ItemManager.ClearValues();
                ItemManager.LoadResources();
                ItemManager.ShowEditItem();
                break;
            case 2:
                ItemManager.ClearValues();
                ItemManager.ShowDeleteItem();
                break;
            default:
                break;
        }
    }

    void ShowNPC()
    {
        _npcTabIndex = GUILayout.Toolbar(_npcTabIndex, new string[] { "Database Operations", "In Game Operations", "Dialogue Editor" });
        switch (_npcTabIndex)
        {
            case 0:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowNpcDatabase();
                break;
            case 1:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowNPCGame();
                break;
            case 2:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                DialogueSystem.NodeEditor.GetWindow<DialogueSystem.NodeEditor>();
                _npcTabIndex = -1;
                break;
            default:
                break;
        }
    }

    void ShowNpcDatabase()
    {
        _npcDBTabIndex = GUILayout.Toolbar(_npcDBTabIndex, new string[] { "Add NPC", "Edit NPC", "Delete NPC" });
        switch (_npcDBTabIndex)
        {
            case 0:
                NPC_Database.LoadActors();
                if (!_npcTabSwitch)
                {
                    NPC_Database.ClearValues();
                    _npcTabSwitch = true;
                }
                NPC_Database.AddNPC();
                break;
            case 1:
                _npcTabSwitch = false;
                NPC_Database.EditNPC();
                break;
            case 2:
                _npcTabSwitch = false;
                NPC_Database.DeleteNPC();
                break;
            default:
                break;
        }
    }

    void ShowNPCGame()
    {
        _npcGameTabIndex = GUILayout.Toolbar(_npcGameTabIndex, new string[] { "Add NPC to Game", "Edit NPC in Game" });
        switch (_npcGameTabIndex)
        {
            case 0:
                NPC_Game.GetAllActors();
                NPC_Game.ShowAddGame();
                break;
            case 1:
                NPC_Game.ShowEditGame();
                break;
            default:
                break;
        }
    }

    void ShowEnemies()
    {
        _enemyTabIndex = GUILayout.Toolbar(_enemyTabIndex, new string[] { "Database Operations", "In Game Operations" });
        switch (_enemyTabIndex)
        {
            case 0:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowEnemyDatabase();
                break;
            case 1:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowEnemyGame();
                break;
            default:
                break;
        }
    }

    void ShowEnemyDatabase()
    {
        _enemyDBTabIndex = GUILayout.Toolbar(_enemyDBTabIndex, new string[] { "Add Enemy", "Edit Enemy", "Delete Enemy", "Loot Tables" });
        switch (_enemyDBTabIndex)
        {
            case 0:
                if(!_enemyTabSwitch)
                {
                    Enemy_DB.ClearValues();
                    _enemyTabSwitch = true;

                }
                Enemy_DB.LoadEnemies();
                Enemy_DB.AddEnemy();
                break;
            case 1:
                _enemyTabSwitch = false;
                EnemyCombat.EnemyDatabase.ClearLists();
                EnemyCombat.EnemyDatabase.GetAllEnemies();
                Enemy_DB.EditEnemy();
                break;
            case 2:
                _enemyTabSwitch = false;
                Enemy_DB.ClearValues();
                EnemyCombat.EnemyDatabase.ClearLists();
                EnemyCombat.EnemyDatabase.GetAllEnemies();
                Enemy_DB.DeleteEnemy();
                break;

            case 3:
                ShowEnemyLootTable();
                break;
            default:
                break;
        }
    }

    void ShowEnemyGame()
    {
        _enemyGameTabIndex = GUILayout.Toolbar(_enemyGameTabIndex, new string[] { "Add Enemy to Game", "Edit Enemy in Game" });
        switch (_enemyGameTabIndex)
        {
            case 0:
                EnemyCombat.EnemyDatabase.ClearLists();
                EnemyCombat.EnemyDatabase.GetAllEnemies();
                Enemy_Game.ShowAddEnemyToGame();
                Enemy_Game.ClearLists();
                break;
            case 1:

                Enemy_Game.GetAllFeedback();
                Enemy_Game.ShowEditEnemy();
                break;
            default:
                break;
        }
    }

    void ShowEnemyLootTable()
    {
        
        _lootTabIndex = GUILayout.Toolbar(_lootTabIndex, new string[] { "Add Loot Table", "Edit Loot Table", "Delete Loot Table" });
        switch (_lootTabIndex)
        {
            case 0:
                LootTable.ClearAll();
                
                LootTable.ShowAddLootTable();
                break;
            case 1:
                LootTable.ShowEditLootTable();
                break;
            case 2:
                LootTable.ShowDeleteLootTable();
                break;
            default:
                break;
        }

        
    }

    void ShowWorld()
    {
        _worldTabIndex = GUILayout.Toolbar(_worldTabIndex, new string[] { "Level Editor", "Object Painter", "Zones", "Player Feedback" });
        switch (_worldTabIndex)
        {
            case 0:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                LevelEditor.ObjectPainter.GetWindow<LevelEditor.ObjectPainter>();
                _worldTabIndex = -1;
                break;
            case 1:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                LevelEditor.FoliagePainter.GetWindow<LevelEditor.FoliagePainter>();
                _worldTabIndex = -1;
                break;
            case 2:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowZoneEditor();
                break;
            case 3:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                ShowFeedbackEditor();
                break;
            default:
                break;
        }
    }

    void ShowZoneEditor()
    {
        _zoneTabIndex = GUILayout.Toolbar(_zoneTabIndex, new string[] { "Add Zone", "Edit Zone", "Delete Zone" });
        switch (_zoneTabIndex)
        {
            case 0:
                ZoneEditor.ClearValues();
                ZoneEditor.ShowAddZone();
                break;
            case 1:
                ZoneEditor.ShowEditZone();
                break;
            case 2:
                ZoneEditor.ShowDeleteZone();
                break;
            default:
                break;
        }
    }

    void ShowFeedbackEditor()
    {
        _feedbackTabIndex = GUILayout.Toolbar(_feedbackTabIndex, new string[] { "Database Operations", "In Game Operations" });
        switch (_feedbackTabIndex)
        {
            case 0:
                ShowFeedbackDatabase();
                break;
            case 1:
                ShowFeedbackGame();
                break;
            default:
                break;
        }
    }

    void ShowFeedbackDatabase()
    {
        _feedbackDBTabIndex = GUILayout.Toolbar(_feedbackDBTabIndex, new string[] { "Add Feedback", "Edit Feedback", "Delete Feedback" });
        switch (_feedbackDBTabIndex)
        {
            case 0:
                Feedback_DB.ClearAll();
                Feedback_DB.ShowAddFeedback();
                break;
            case 1:
                FeedbackEditor.FeedbackDB.ClearAll();
                FeedbackEditor.FeedbackDB.GetAllFeedback();
                Feedback_DB.ShowEditFeedback();
                break;
            case 2:
                FeedbackEditor.FeedbackDB.ClearAll();
                FeedbackEditor.FeedbackDB.GetAllFeedback();
                Feedback_DB.ShowDeleteFeedback();
                break;
            default:
                break;
        }
    }

    void ShowFeedbackGame()
    {
        _feedbackGameTabIndex = GUILayout.Toolbar(_feedbackGameTabIndex, new string[] { "Add Feedback to Game", "Edit Feedback in Game" });
        switch (_feedbackGameTabIndex)
        {
            case 0:
                Feedback_Game.ShowAddGame();
                break;
            case 1:
                Feedback_Game.ShowEditGame();
                break;
            default:
                break;
        }
    }
    
    void ShowSceneManager()
    {
        _sceneTabIndex = GUILayout.Toolbar(_sceneTabIndex, new string[] { "Set up new scene", "Update Manager"});
        switch (_sceneTabIndex)
        {
            case 0:
                SceneManager.ShowNewScene();
                break;
            case 1:
                SceneManager.ResetCounters();
                SceneManager.ShowUpdateScene();
                break;
            default:
                break;
        }
    }

    void ShowQuestManager()
    {
        _questTabIndex = GUILayout.Toolbar(_questTabIndex, new string[] { "Add Quest", "Edit Quest", "Delete Quest", "View Active Quests", "View Quest Chains","Reset Active Quests" });
        switch (_questTabIndex)
        {
            case 0:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
                Quest.QuestSystem.ShowAddQuest();
                break;
            case 1:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);

                Quest.QuestSystem.SetRetrievedQuests(false);
                Quest.QuestSystem.ShowEditQuest();
                break;
            case 2:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);

                Quest.QuestSystem.SetRetrievedQuests(false);
                Quest.QuestSystem.ShowDeleteQuests();
                break;
            case 3:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);

                Quest.QuestSystem.SetRetrievedQuests(false);
                Quest.QuestSystem.ShowActiveQuests();
                break;
            case 4:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);

                Quest.QuestSystem.SetRetrievedQuests(false);
                Quest.QuestSystem.ShowQuestChains();
                break;
            case 5:
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);

                Quest.QuestSystem.SetRetrievedQuests(false);
                Quest.QuestSystem.ShowResetQuest();
                break;
            default:
                break;
        }
    }
}
