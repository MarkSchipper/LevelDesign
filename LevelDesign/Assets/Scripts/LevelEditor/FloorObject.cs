using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject : MonoBehaviour {

    [SerializeField]
    private bool _isActive = false;

    [SerializeField]
    private int _location;

	// Use this for initialization
	void Start () {

        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<MeshCollider>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetObjectActive(bool _set)
    {
        _isActive = _set;
        if(!_set)
        {
            this.gameObject.layer = 2;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            this.gameObject.layer = 0;
            this.GetComponent<MeshRenderer>().enabled = true;
        }

    }

    public bool ReturnObjectActive()
    {
        return _isActive;
    }

    public void SetLocation(int _loc)
    {
        _location = _loc;
    }

    public int ReturnLocation()
    {
        return _location;
    }

}
