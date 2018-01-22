using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootGenerator : MonoBehaviour {

    private  string _lootTable;
    private  List<LootTypes> _lootTypeList = new List<LootTypes>();
    private  List<int> _lootValueList = new List<int>();
    private  List<int> _lootItemID = new List<int>();

    private  List<float> _chance = new List<float>();

    public LootGenerator(string _table, int _gameID)
    {
        ClearAll();
        _lootTable = _table + _gameID.ToString();
        
        LootDatabase.GetLootTable(_table);

        for (int i = 0; i < LootDatabase.ReturnLootIdByTable().Count; i++)
        {
            if (LootDatabase.ReturnLootTypeByTable()[i] == LootTypes.Gold)
            {
                _lootTypeList.Add(LootTypes.Gold);
                _lootValueList.Add(LootDatabase.ReturnLootValueByTable()[i]);
                _lootItemID.Add(0);
            }
            if (LootDatabase.ReturnLootTypeByTable()[i] == LootTypes.Items)
            {
                if (Random.Range(0, (100 / LootDatabase.ReturnLootWeightByTable()[i])) == 0)
                {
                    _lootTypeList.Add(LootTypes.Items);
                    _lootItemID.Add(LootDatabase.ReturnItemIDByTable()[i]);
                    _lootValueList.Add(0);
                }
                else
                {

                }
            }
        }

    }

    public void DeleteEntry(LootTypes _gold, string _table)
    {
        LootDatabase.GetLootTable(_table);

        for (int i = 0; i < LootDatabase.ReturnLootIdByTable().Count; i++)
        {
            if(LootDatabase.ReturnLootTypeByTable()[i] == LootTypes.Gold)
            {
                _lootTypeList.RemoveAt(i);
                _lootValueList.RemoveAt(i);
                _lootItemID.RemoveAt(i);
            }
        }
    }

    public void DeleteEntry(LootTypes _item, int _id, string _table)
    {
        LootDatabase.GetLootTable(_table);

        for (int i = 0; i < LootDatabase.ReturnLootIdByTable().Count; i++)
        {
            if (LootDatabase.ReturnLootTypeByTable()[i] == LootTypes.Gold && i == _id)
            {
                _lootTypeList.RemoveAt(i);
                _lootValueList.RemoveAt(i);
                _lootItemID.RemoveAt(i);
            }
        }
    }

    public  string ReturnLootTable()
    {
        return _lootTable;
    }

    public  List<LootTypes> ReturnLootType()
    {
        return _lootTypeList;
    }

    public  List<int> ReturnValueList()
    {
        return _lootValueList;
    }

    public  List<int> ReturnItemIDList()
    {
        return _lootItemID;
    }

    void ClearAll()
    {
        _lootItemID.Clear();
        _lootTable = "";
        _lootTypeList.Clear();
        _lootValueList.Clear();
    }


}
