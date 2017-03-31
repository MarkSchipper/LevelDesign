using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemCollectable : MonoBehaviour {

    [SerializeField]
    private int _itemID;

    [SerializeField]
    private string _itemName;

    [SerializeField]
    private string _itemType;

    [SerializeField]
    private int _itemStats;

    private Inventory _inventory;

    void OnEnable()
    {
        _inventory = GameObject.FindObjectOfType<Inventory>().GetComponent<Inventory>();
    }

    public void SetValues(int _id, string _name, string _type, int _stats)
    {
        _itemID = _id;
        _itemName = _name;
        _itemType = _type;
        _itemStats = _stats;
    }

    public string ReturnName()
    {
        return _itemName;
    }

    public string ReturnItemType()
    {
        return _itemType;
    }
    public int ReturnItemStats()
    {
        return _itemStats;
    }


    void OnTriggerEnter(Collider coll)
    {
        if(coll.name == "Player")
        {
            _inventory.AddItem(_itemID);
            Destroy(this.gameObject);
        }
    }

}
