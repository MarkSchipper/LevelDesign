using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Health,
    Mana,
    QuestItem,
    Weapon,
    Armour,
    None,
}

[System.Serializable]
public class Item  {

    
    public string _itemName;
    public int _itemID;
    public string _itemDesc;
    public Texture2D _itemIcon;
    public int _itemPower;
    public int _itemStats;
    public ItemType _itemType;


    public Item(string _name, int _id, string _desc, int _power, int _stats, ItemType _type)
    {
        _itemName = _name;
        _itemID = _id;
        _itemDesc = _desc;
        _itemIcon = Resources.Load<Texture2D>("ItemIcons/" + _name);
        _itemPower = _power;
        _itemStats = _stats;
        _itemType = _type;
     }



    // EMPTY CONSTRUCTUR TO ADD AN EMPTY ITEM
    // WE FILL THE INITIAL INVENTORY WITH EMTPY ITEMS

    public Item()
    {
        _itemID = -1;
    }
}
