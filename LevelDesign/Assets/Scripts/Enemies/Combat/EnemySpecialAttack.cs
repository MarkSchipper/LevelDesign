using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{

    public enum SpecialAttackType
    {
        None,
        KNOCKBACK,
        STUN,
        INCAPACITATE,
    }

    public class EnemySpecialAttack : MonoBehaviour
    {

        public SpecialAttackType _type;

        // Use this for initialization
        void Start()
        {
            //_type = SpecialAttackType.KNOCKBACK;
            StartCoroutine(KillSwitch());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public SpecialAttackType ReturnAttackType()
        {
            return _type;
        }

        IEnumerator KillSwitch()
        {
            yield return new WaitForSeconds(2);
            if (_type == SpecialAttackType.INCAPACITATE)
            {
                StartCoroutine(DissolveWeb(1f));
            }
            Destroy(this.gameObject, 1f);
        }

        IEnumerator DissolveWeb(float _time)
        {
            Debug.Log("dissolve");
            float _timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;

                CombatSystem.PlayerController.instance.ReturnPlayerWebbed().GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_SliceAmount", _timer);
                //this.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer >= _time)
                {
                    yield break;
                }
            }
        }

    }
}