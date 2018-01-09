using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffRuntime : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
        if(GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = false;

            if (GetComponentInChildren<MeshRenderer>() != null)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            if(GetComponentInChildren<MeshRenderer>() != null)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
        	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
