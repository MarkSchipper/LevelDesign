using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffSpell : MonoBehaviour {

    private string _debuff;
    private float _duration;
    private bool _fromPlayer;
    private GameObject _spellCaster;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDebuff(string _nwDebuff, float _time, GameObject _caster)
    {
        _debuff = _nwDebuff;
        _duration = _time;
        _spellCaster = _caster;
    }

    public string ReturnDebuff()
    {
        return _debuff;
    }

    public float ReturnDuration()
    {
        return _duration;
    }

    public void SetFromPlayer(bool _set)
    {
        _fromPlayer = _set;
    }

    public void SetSpellCaster(GameObject _obj)
    {
        _spellCaster = _obj;
    }

    public bool ReturnFromPlayer()
    {
        return _fromPlayer;
    }

    public GameObject ReturnSpellCaster()
    {
        return _spellCaster;
    }
}
