using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    private GameObject _solidMesh;
    private GameObject _toFracture;
    private bool _fractured;

	// Use this for initialization
	void Start () {

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).childCount > 1)
            {
                
                _toFracture = transform.GetChild(i).gameObject;
            }
            else if(transform.GetChild(i).childCount < 1)
            {
                _solidMesh = transform.GetChild(i).gameObject;

            }
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if (!_fractured)
            {
                Debug.Log("Player");
                _solidMesh.SetActive(false);
                for (int i = 0; i < _toFracture.transform.childCount; i++)
                {
                    _toFracture.transform.GetChild(i).gameObject.AddComponent<Rigidbody>().AddExplosionForce(200, transform.position, 10);
                    
                }
                CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.CRATE_BRAKE, transform.position, false);
                Destroy(_toFracture, 5f);
                _fractured = true;
            }
        }
    }
}
