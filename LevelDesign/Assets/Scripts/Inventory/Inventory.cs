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

    private bool _showInventory;

    private bool _showTooltip;
    private string _toolTip;

    private bool _draggingItem;
    private Item _draggedItem;

    private bool _draggingInventory;

    private int _prevIndex;

    public GUISkin _skin;



    // inventory positions

    private int _inventoryMain_width = 400;
    private int _inventoryMain_height = 400;

    // Create a new Vector2 for the position
    private Vector2 _inventoryPos = new Vector2(600, 600);

    
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

    }

    void Update()
    {
        if(Input.GetKeyDown("x"))
        {
            _showInventory = !_showInventory;
            if(_showInventory)
            {                
                LoadInventory();
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
            CombatSystem.PlayerMovement.HoveringOverUI(true);

            // If we clicked the mouse and using the Left mousebutton
            if(e.button == 0 && e.type == EventType.mouseDown)
            {
                // Set the SetDraggingUI function in the Player class to True so we cancel all essential player control functions
                CombatSystem.PlayerMovement.SetDraggingUI(true);
            }
            if(e.button == 0 && e.type == EventType.mouseUp)
            {
                CombatSystem.PlayerMovement.SetDraggingUI(false);
            }

            if(e.button == 0 && e.type == EventType.mouseDrag && !_draggingInventory)
            {
                _draggingInventory = true;
                CombatSystem.PlayerMovement.SetDraggingUI(true);

            }
            if(e.button == 0 && e.type == EventType.mouseUp && _draggingInventory)
            {
                _draggingInventory = false;
                CombatSystem.PlayerMovement.SetDraggingUI(false);
            }
        }

        else
        {
            CombatSystem.PlayerMovement.HoveringOverUI(false);
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
                        if (e.button == 0 && e.type == EventType.mouseDrag && !_draggingItem)
                        {
                            _draggingItem = true;
                            _prevIndex = i;
                            _draggedItem = _slots[i];
                            _inventory[i] = new Item();

                        }
                        if (e.type == EventType.mouseUp && _draggingItem)
                        {
                            _inventory[_prevIndex] = _inventory[i];
                            _inventory[i] = _draggedItem;
                            _draggingItem = false;
                            _draggedItem = null;
                        }
                    }

                    // If we pressed the Right Mouse Button
                    if (e.isMouse && e.button == 1 && e.type == EventType.mouseDown && !_draggingItem)
                    {

                        // Filter: If the itemType is Health
                        if (_inventory[i]._itemType == ItemType.Health)
                        {
                            // Call the player script to add Health to the player based on the item stats
                            CombatSystem.PlayerMovement.AddPlayerHealth(_inventory[i]._itemStats);
                            _inventory[i] = new Item();
                        }

                        // Filter: If the itemType is Mana
                        if (_inventory[i]._itemType == ItemType.Mana)
                        {
                            // Call the player script to add Mana to the player based on the item stats
                            CombatSystem.PlayerMovement.AddPlayerMana(_inventory[i]._itemStats);
                            _inventory[i] = new Item();
                        }
                    }

                    
                }
                else
                {
                    // Swapping of items in the inventory
                    if(slotRect.Contains(e.mousePosition))
                    {
                        if(e.type == EventType.mouseUp && _draggingItem)
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
                for (int j = 0; j < _itemDB._itemList.Count; j++)
                {
                    if(_itemDB._itemList[j]._itemID == _id)
                    {
                        _inventory[i] = _itemDB._itemList[j];
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
                if (PlayerPrefs.GetInt("Inventory " + i) == _itemDB._itemList[PlayerPrefs.GetInt("Inventory " + i)]._itemID)
                {
                    _inventory[i] = _itemDB._itemList[PlayerPrefs.GetInt("Inventory " + i)];
                }
            }
        }
    }
}
