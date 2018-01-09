using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    private Transform _target;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 destination = Vector3.zero;

    // Use this for initialization
    void Start () {
        SetCameraTarget(this.transform.parent.transform);
	}
	
	// Update is called once per frame
	void Update () {
        LookAtTarget();
        MoveToTarget();
	}


    void SetCameraTarget(Transform t)
    {
        _target = t;
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    void MoveToTarget()
    {
        //targetPos = _target.position
        //destination += targetPos;
        transform.position = _target.transform.position;
    }
}
