using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationSystem : MonoBehaviour {

    private Animator _animator;
    private string _prefab;
    private float _movementSpeed;
    private float[] _defaultSpeed = new float[] { 1.85f, 2.0f, 1.6f };

    public NpcAnimationSystem(Animator _anim, string prefab, float _speed)
    {
        _animator = _anim;
        _prefab = prefab;
        _movementSpeed = _speed;
    }

    public void SetIdle()
    {
        _animator.SetBool("isIdle", true);
        if(_animator.GetBool("isWalk"))
        {
            _animator.SetBool("isWalk", false);
        }
    }

    public void StopIdle()
    {
        _animator.SetBool("isIdle", false);
    }

    public void SetWalking()
    {
        _animator.SetBool("isWalk", true);

        if(_prefab == "Male_Smith")
        {
            _animator.speed = _movementSpeed / _defaultSpeed[0];
        }
        
        if (_animator.GetBool("isIdle"))
        {
            _animator.SetBool("isIdle", false);
        }
    }

    public void StopWalking()
    {
        _animator.SetBool("isWalk", false);
    }
	
}
