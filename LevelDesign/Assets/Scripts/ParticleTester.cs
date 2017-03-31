using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTester : MonoBehaviour {

    public ParticleSystem _ps;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            _ps.Play();
            _ps.GetComponent<Animator>().SetBool("spawn", true);
        }
    }
}
