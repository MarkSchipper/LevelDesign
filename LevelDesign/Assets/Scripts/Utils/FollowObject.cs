using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

    private GameObject _follow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_follow != null)
        {
            this.transform.position = new Vector3(_follow.transform.position.x, _follow.transform.position.y + 0.1f, _follow.transform.position.z);
        }
        
	}

    public void SetObject(GameObject _object)
    {
        _follow = _object;
    }
}
