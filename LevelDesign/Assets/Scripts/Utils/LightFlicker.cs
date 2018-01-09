using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    private Light _light;
    private float m_Rnd;

    // Use this for initialization
    void Start () {
        _light = GetComponent<Light>();
        m_Rnd = Random.Range(0.2f, 0.9f);
        //InvokeRepeating("Flicker", 0.2f, 0.4f);
    }
	
	// Update is called once per frame
	void Update () {
        _light.intensity =  Mathf.PerlinNoise(m_Rnd + Time.time, m_Rnd + 1 + Time.time * 1);
    }

    void Flicker()
    {
        _light.intensity = Mathf.Lerp(Random.Range(0.7f, 1f), Random.Range(0.5f, 1f), Time.deltaTime);
    }
}
