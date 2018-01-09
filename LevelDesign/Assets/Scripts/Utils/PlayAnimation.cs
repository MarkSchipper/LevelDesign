using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            GetComponent<Animator>().SetBool("playAnimation", true);
            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.DOOROPEN, transform.position, false);
        }
    }
}
