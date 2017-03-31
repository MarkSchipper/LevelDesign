using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour {

    [SerializeField]
    private string _audioClip;

    [SerializeField]
    private bool _oneShot;

    [SerializeField]
    private float _soundVolume;

    private AudioClip _soundClip;

    private AudioSource _audioSource;

	// Use this for initialization
	void Start () {

        _soundClip = Resources.Load("Audio/Foliage/" + _audioClip) as AudioClip;
        _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(string _clip, bool _set, float _volume)
    {
        _audioClip = _clip;
        _oneShot = _set;
        _soundVolume = _volume;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player")
        {
            if (_oneShot)
            {
                _audioSource.PlayOneShot(_soundClip);
                _audioSource.volume = _soundVolume;
            }
            else
            {
                _audioSource.PlayOneShot(_soundClip);
                _audioSource.loop = true;
            }
        }
    }
}
