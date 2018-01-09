using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyAOE : MonoBehaviour {

	private float _cooldown;
	private bool _repeater;
	private string _type;
	private float _timer;
	private bool _inRange;
	private float _damageDone;

	public Text _debug;

	// Use this for initialization
	void Start () {
		_repeater = false;
		_timer = _cooldown;
	}
	
	// Update is called once per frame
	void Update () {
		//_debug.text = _timer.ToString ();
		RunTimer ();

		if (_inRange) {
			
			if (_repeater) {
				
				if (_type == "HellBlast") {	
					if (Mathf.RoundToInt (_timer) == _cooldown) {
						
						DoHellBlast ();
						_timer = 0;
					}
				}
			
			}


		}
		if (!_inRange) {


			_timer = 0;

		}

	}

	void RunTimer() {

		_timer += Time.deltaTime;


	}

	public void DoAOE(string _AOEType, float _damage, float _nwCooldown, bool _repeatHellBlast, ParticleSystem _particles) {
		
		if (_AOEType == "HellBlast") {
			_type = _AOEType;
			_repeater = _repeatHellBlast;
			_cooldown = _nwCooldown;
			_damageDone = _damage;
		}
		if (_AOEType == "EarthQuake") {
			_type = _AOEType;
			_cooldown = _nwCooldown;
			_damageDone = _damage;
			DoEarthQuake ();
            _particles.Play();
		}

	}

	void DoHellBlast() {

		GetComponent<Animator> ().Play ("Anim_HellBlast");
        
	}

	void DoEarthQuake() {

		GetComponent<Animator> ().Play ("Anim_EarthQuake");

	}

	public void SetInAOERange(bool _inrange) {

		_inRange = _inrange;

	}

	public float ReturnDamage() {
		return _damageDone;
	}
}
