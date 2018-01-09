using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public int slotsX, slotsY;
    private int _slotSize = 50;

    [SerializeField]
    public List<Item> _inventory = new List<Item>();
    private ItemDatabase _itemDB;
    public List<Item> _slots = new List<Item>();

    [SerializeField]
    public Texture2D _background;

    private List<Item> _loot = new List<Item>();
    private List<Item> _lootSlots = new List<Item>();

    private bool _showInventory;
    private bool _showLootWindow;
    private bool _loadedItemDatabase;

    private bool _showTooltip;
    private string _toolTip;

    private bool _draggingItem;
    private Item _draggedItem;

    private bool _draggingInventory;
    private bool _draggingLootWindow;

    private int _prevIndex;

    public GUISkin _skin;

    private string _enemyLootTable;
    private GameObject _enemyToLoot;

    private bool _mouseOverLootWindow;

    // inventory positions

    private int _inventoryMain_width = 400;
    private int _inventoryMain_height = 400;

    // Create a new Vector2 for the position
    private Vector2 _inventoryPos = new Vector2(600, 600);

    private Vector2 _lootPos = new Vector2(214, 346);

    public static Inventory instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        _itemDB = GameObject.FindObjectOfType<ItemDatabase>().GetComponent<ItemDatabase>();
    
        
        for (int i = 0; i < (slotsX * slotsY); i++)
        {
            _slots.Add(new Item());
            _inventory.Add(new Item());
            
        }

        if (PlayerPrefs.GetFloat("InventoryPosX") > 0)
        {
            _inventoryPos = new Vector2(PlayerPrefs.GetFloat("InventoryPosX"), PlayerPrefs.GetFloat("InventoryPosY"));
        }

        //_player = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatSystem.PlayerController>();

    }

    void Update()
    {
        if(Input.GetKeyDown("x"))
        {
            _showInventory = !_showInventory;
            if(_showInventory)
            {                
                LoadInventory();
                if (CombatSystem.CameraController.ReturnFirstPerson())
                {
                    Cursor.visible = true;
                }
            }
            else
            {
                SaveInventory();
            }
        }
    }

    void OnGUI()
    {
        _toolTip = "";

        GUI.skin = _skin;

        if(_showInventory)
        {
            DrawInventory();

            if (_showTooltip)
            {
                GUI.Box(new Rect(Event.current.mousePosition.x + 15, Event.current.mousePosition.y, 200, 200), _toolTip);
            }

            if(_draggingItem)
            {
                GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 50, 50), _draggedItem._itemIcon);
            }

            if(_draggingInventory)
            {
                _inventoryPos.x = Event.current.mousePosition.x;
                _inventoryPos.y = Event.current.mousePosition.y;

                PlayerPrefs.SetFloat("InventoryPosX", _inventoryPos.x);
                PlayerPrefs.SetFloat("InventoryPosY", _inventoryPos.y);

            }
        }
        if(_showLootWindow)
        {
            DrawLootWindow();

            if (_draggingLootWindow)
            {
                _lootPos.x = Event.current.mousePosition.x;
                _lootPos.y = Event.current.mousePosition.y;

                PlayerPrefs.SetFloat("LootWindowPosX", _lootPos.x);
                PlayerPrefs.SetFloat("LootWindowPosY", _lootPos.y);
            }

        }
    }

    void DrawInventory()
    {
        Event e = Event.current;
        int i = 0;

        GUI.color = Color.white;
        // Create the main "bag" for the Inventory
        Rect _inventoryRect = new Rect(_inventoryPos.x - 50, _inventoryPos.y - 100, _inventoryMain_width, _inventoryMain_height);
        GUI.Box(_inventoryRect, "", _skin.GetStyle("Main"));
        //GUI.DrawTexture(_inventoryRect, _background);

        // Create the slots for the inventory
        Rect _inventoryClose = new Rect(_inventoryPos.x + (_inventoryMain_width - 16) - 100, _inventoryPos.y, 16, 16);


        // If the Close button is pressed
        // Close the Inventory and save the current inventory in the PlayerPrefs
        if (GUI.Button(_inventoryClose, "Close",_skin.GetStyle("CloseInventory")))
        {
            _showInventory = !_showInventory;
            SaveInventory();
        }

        // If the mouse is hovering over the main 'bag'
        if(_inventoryRect.Contains(e.mousePosition))
        {

            // Set the HoveringOverUI in the Player Class to prevent clicking and setting a target and therefor moving the player
         //   CombatSystem.PlayerMovement.HoveringOverInventory(true);

            // If we clicked the mouse and using the Left mousebutton
            if(e.button == 0 && e.type == EventType.MouseDown)
            {
                // Set the SetDraggingUI function in the Player class to True so we cancel all essential player control functions
           //     CombatSystem.PlayerMovement.SetDraggingUI(true);
            }
            if(e.button == 0 && e.type == EventType.MouseUp)
            {
             //   CombatSystem.PlayerMovement.SetDraggingUI(false);
            }

            if(e.button == 0 && e.type == EventType.MouseDrag && !_draggingInventory)
            {
                _draggingInventory = true;
               // CombatSystem.PlayerMovement.SetDraggingUI(true);

            }
            if(e.button == 0 && e.type == EventType.MouseUp && _draggingInventory)
            {
                _draggingInventory = false;
                //CombatSystem.PlayerMovement.SetDraggingUI(false);
            }
        }

        else
        {
            //CombatSystem.PlayerMovement.HoveringOverInventory(false);
        }

        for (int y = 0; y < slotsY; y++)
        {

            for (int x = 0; x < slotsX; x++)
            {
                Rect slotRect = new Rect(_inventoryPos.x + 15 + x * _slotSize + (5 * x), _inventoryPos.y + 15 + y * _slotSize + (5 * y), _slotSize, _slotSize);
                GUI.Box(slotRect, "", _skin.GetStyle("Slot"));
                _slots[i] = _inventory[i];
                
                if (_slots[i]._itemName != null)
                {
                    // Draw the texture of the slots and potential Icon
                    GUI.Box(slotRect, _slots[i]._itemName);
                    GUI.DrawTexture(slotRect, _slots[i]._itemIcon);
                    


                    // If the mouse is over the Slot 
                    if (slotRect.Contains(e.mousePosition))
                    {
                        // Create a tooltip
                        _toolTip = CreateTooltip(_slots[i]);
                        _showTooltip = true;
                        if (e.button == 0 && e.type == EventType.MouseDrag && !_draggingItem)
                        {
                            _draggingItem = true;
                            _prevIndex = i;
                            _draggedItem = _slots[i];
                            _inventory[i] = new Item();

                        }
                        if (e.type == EventType.MouseUp && _draggingItem)
                        {
                            _inventory[_prevIndex] = _inventory[i];
                            _inventory[i] = _draggedItem;
                            _draggingItem = false;
                            _draggedItem = null;
                        }
                    }

                    // If we pressed the Right Mouse Button
                    if (e.isMouse && e.button == 1 && e.type == EventType.MouseDown && !_draggingItem)
                    {

                        // Filter: If the itemType is Health
                        if (_inventory[i]._itemType == ItemType.Health)
                        {
                            // Call the player script to add Health to the player based on the item stats
                            CombatSystem.PlayerController.instance.AddPlayerHealth(_inventory[i]._itemStats);
                            _inventory[i] = new Item();
                        }

                        // Filter: If the itemType is Mana
                        if (_inventory[i]._itemType == ItemType.Mana)
                        {
                            // Call the player script to add Mana to the player based on the item stats
                            CombatSystem.PlayerController.instance.AddPlayerMana(_inventory[i]._itemStats);
                            _inventory[i] = new Item();
                        }
                    }

                    
                }
                else
                {
                    // Swapping of items in the inventory
                    if(slotRect.Contains(e.mousePosition))
                    {
                        if(e.type == EventType.MouseUp && _draggingItem)
                        {
                            _inventory[i] = _draggedItem;
                            _draggingItem = false;
                            _draggedItem = null;
                        }
                    }
                }
                if (_toolTip == "")
                {
                    _showTooltip = false;
                }
                i++;
            }
        }
        //SaveInventory();
    }

    string CreateTooltip(Item _item)
    {
        _toolTip = _item._itemName;
        return _toolTip;
    }

    public void RemoveItem(int _id)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i]._itemID == _id)
            {
                _inventory[i] = new Item();
                break;
            }
        }
    }

    public void AddItem(int _id)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i]._itemName == null)
            {
                for (int j = 0; j < _itemDB.ReturnItemList().Count; j++)
                {
                    if(_itemDB.ReturnItemList()[j]._itemID == _id)
                    {
                        _inventory[i] = _itemDB.ReturnItemList()[j];
                    }
                }

                break;
            }
        }
    }

    bool InventoryContains(int _id)
    {
        bool _result = false;

        for (int i = 0; i < _inventory.Count; i++)
        {
            _result =  _inventory[i]._itemID == _id;
            if(_result)
            {
                break;
            }
        }

        return _result;       
    }

    void SaveInventory()
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            PlayerPrefs.SetInt("Inventory " + i, _inventory[i]._itemID);
        }
    }

    void LoadInventory()
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (PlayerPrefs.GetInt("Inventory " + i) >= 0)
            {
                if (PlayerPrefs.GetInt("Inventory " + i) == _itemDB.ReturnItemList()[PlayerPrefs.GetInt("Inventory " + i)]._itemID)
                {
                    _inventory[i] = _itemDB.ReturnItemList()[PlayerPrefs.GetInt("Inventory " + i)];
                }
            }
        }
    }

    public void DrawLootWindow()
    {

        if (_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootTypes().Count > 0)
        {

            Rect _lootRect = new Rect(_lootPos.x - 50, _lootPos.y - 100, 214, 346);
            GUI.Box(_lootRect, "", _skin.GetStyle("LootWindow"));

            if (!_loadedItemDatabase)
            {
                LootDatabase.GetLootTable(_enemyLootTable);
                ItemDatabase.GetAllItems();
                _loadedItemDatabase = true;
            }

            Rect _inventoryClose = new Rect(_lootPos.x + 105, _lootPos.y - 23, 16, 16);

            

            // If the Close button is pressed
            // Close the Inventory and save the current inventory in the PlayerPrefs
            if (GUI.Button(_inventoryClose, "", _skin.GetStyle("CloseInventory")))
            {
                _showLootWindow = !_showLootWindow;

            }

            for (int i = 0; i < _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootTypes().Count; i++)
            {

                Rect _slot = new Rect(_lootPos.x + 5, _lootPos.y - 15 + (i * 40), 110, 50);
                Rect _lootSlot = new Rect(_lootPos.x + 20, _lootPos.y - 8 + (i * 40), 32, 32);
                Rect _lootSlotText = new Rect(_lootPos.x + 50, _lootPos.y + (i * 50), 25, 25);

                GUI.Box(_slot, "", _skin.GetStyle("LootSlot"));
                if (_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootTypes()[i] == LootTypes.Gold)
                {
                    GUI.Box(_lootSlotText, _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootValues()[i].ToString());
                    GUI.DrawTexture(_lootSlot, Resources.Load("ItemIcons/Gold") as Texture2D);
                }



                if (_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootTypes()[i] == LootTypes.Items)
                {

                    if (Resources.Load("ItemIcons/" + ItemDatabase.ReturnItemName(_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootItemID()[i])) != null)
                    {
                        GUI.DrawTexture(_lootSlot, Resources.Load("ItemIcons/" + ItemDatabase.ReturnItemName(_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootItemID()[i])) as Texture2D);
                    }
                    else
                    {
                        GUI.DrawTexture(_lootSlot, Resources.Load("ItemIcons/Quest Item") as Texture2D);
                    }
                }

                if (_slot.Contains(Event.current.mousePosition))
                {
                    if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
                    {
                        if (_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootTypes()[i] == LootTypes.Gold)
                        {
                            CombatSystem.CombatDatabase.AddGold(CombatSystem.CombatDatabase.ReturnPlayerGold() + _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootValues()[i]);
                            _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().RemoveFromLoot(LootTypes.Gold);

                        }
                        else
                        {
                            AddItem(ItemDatabase.ReturnItemID(_enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().ReturnLootItemID()[i]));
                            _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().RemoveFromLoot(LootTypes.Items, i);
                        }
                    }
                }
                else
                {
                }

            }

            if (_lootRect.Contains(Event.current.mousePosition))
            {
                _mouseOverLootWindow = true;

                if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && !_draggingLootWindow)
                {
                    _draggingLootWindow = true;
                    // CombatSystem.PlayerMovement.SetDraggingUI(true);

                }
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && _draggingLootWindow)
                {
                    _draggingLootWindow = false;
                    //CombatSystem.PlayerMovement.SetDraggingUI(false);
                }

            }
            else
            {
                _mouseOverLootWindow = false;
            }
        }
        else
        {
            if(_enemyToLoot != null)
            {
                _enemyToLoot.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().DestroyEnemy();
            }
            _showLootWindow = false;
        }
    }

    public void ShowLootWindow(GameObject _enemy)
    {
        _showLootWindow = !_showLootWindow;
       if(_showLootWindow)
        {
            _enemyToLoot = _enemy;
        }
       else
        {
            _loadedItemDatabase = false;
            _enemyToLoot = null;
        }
        
    }

    public bool ReturnShowLootWindow()
    {
        return _showLootWindow;
    }
}
