using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DoorHandler : MonoBehaviour {

    [SerializeField]
    private string _keyRequired;

    public GameObject _GateToOpen;

    [FMODUnity.EventRef]
    public string _sound;

    private bool _isDoorOpen = false;

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
            if (!_isDoorOpen)
            {
                if (Inventory.instance.ItemInInventory(_keyRequired))
                {
                    StartCoroutine(OpenDoor());
                    FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_sound);
                    e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

                    e.start();
                    e.release();

                }
                else
                {
                    Dialogue.DialogueManager.instance.ShowMessage("This gate is locked", true);
                }
            }
        }
    }

    IEnumerator OpenDoor()
    {
        float _timer = 0f;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            _timer += Time.deltaTime;

            _GateToOpen.transform.position = new Vector3(transform.position.x, transform.position.y + _timer, transform.position.z);

            //this.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

            if (_timer >= 5f)
            {
                _isDoorOpen = true;
                yield break;
            }
        }
    }
}
