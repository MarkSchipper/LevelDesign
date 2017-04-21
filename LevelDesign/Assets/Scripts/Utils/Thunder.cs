using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour {

    private Light _thunder;
    private bool _isThunder;
    private bool _setOnce = false;
	// Use this for initialization
	void Start () {


        // Get the directional light which will act as our lighting
        _thunder = GetComponentInChildren<Light>();

        // by default set the intensity to 0
        _thunder.intensity = 0;

        // Start the coroutine for the initial thunderstrike
        StartCoroutine(SetThunder(Random.Range(2, 10)));

        // Call the SoundSystem to start playing the Rain
        CombatSystem.SoundSystem.Rain();
	}
	
	// Update is called once per frame
	void Update () {
        
        // if _isThunder is true make the light flicker
        if(_isThunder)
        {
            _thunder.intensity = Random.Range(0f, 3f);

            // Start the coroutine to kill the light ( set the intensity to 0 and set _isThunder to false )
            StartCoroutine(KillLight());
        }

	}

    IEnumerator KillLight()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1);

        // intensity to 0 and _isThunder to false
        _thunder.intensity = 0;
        _isThunder = false;
        
        // stop this coroutine        
        StopCoroutine(KillLight());

        // We want to call the SetThunder coroutine only once
        if(!_setOnce)
        {
            StartCoroutine(SetThunder(Random.Range(7, 20)));
            _setOnce = true;
        }
    }

    IEnumerator SetThunder(float _time)
    {
        // Wait for _time 
        yield return new WaitForSeconds(_time);

        // setOnce is false so we can call this coroutine again
        _setOnce = false;

        // Play the thunder sound
        CombatSystem.SoundSystem.Thunder();

        // _isThunder is true ( see Update )
        _isThunder = true;
    }
}
