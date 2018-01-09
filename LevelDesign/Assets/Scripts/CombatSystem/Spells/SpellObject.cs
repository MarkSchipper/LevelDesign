using UnityEngine;
using System.Collections;

public class SpellObject : MonoBehaviour
{

    private float _spellDamage;
    private float _timer;
    private bool _fromPlayer;

    private float _lifeSpan = 2f;

    private GameObject _spellCaster;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(KillSwitch());
    }

    // Update is called once per frame
    void Update()
    {
     
      
    }


    public void SetDamage(float _damage)
    {
        _spellDamage = _damage;
    }

    public float ReturnDamage()
    {
        return _spellDamage;
    }

    public void SetFromPlayer(bool _set)
    {
        _fromPlayer = _set;
    }

    public void SetSpellCaster(GameObject _obj)
    {
        _spellCaster = _obj;
    }

    public bool ReturnFromPlayer()
    {
        return _fromPlayer;
    }

    public GameObject ReturnSpellCaster()
    {
        return _spellCaster;
    }

    IEnumerator KillSwitch()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
