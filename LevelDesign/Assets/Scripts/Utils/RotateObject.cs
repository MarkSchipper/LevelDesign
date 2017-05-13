using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

    public float _speed;

    public bool _x;
    public bool _y;
    public bool _z = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (_z)
        {
            this.transform.Rotate(0, 0, Time.deltaTime * _speed);
        }
        if(_x)
        {
            this.transform.Rotate(Time.deltaTime * _speed, 0, 0);
        }

        if (_y)
        {
            this.transform.Rotate(0, Time.deltaTime * _speed, 0);
        }
    }
}
