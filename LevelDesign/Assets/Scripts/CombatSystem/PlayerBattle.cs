using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{

    public class PlayerBattle : MonoBehaviour
    {
        // bools
        private bool SPELL_BARRIER = false;

        // gameobjects
        private GameObject _barrierGameObject;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(SPELL_BARRIER)
            {
                if(_barrierGameObject != null)
                {
                    _barrierGameObject.transform.position = transform.position;
                }
            }
        }

        public void CreateBarrier(float _time)
        {
            _barrierGameObject = Instantiate(Resources.Load("PlayerSpells/Ability/Barrier_Spell"), transform.position, Quaternion.identity) as GameObject;
            SPELL_BARRIER = true;
            StartCoroutine(BarrierRise(_time));

            SoundManager.instance.PlaySound(SOUNDS.PLAYERBARRIER, transform.position, true);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          Barrier IENumerators                                            //
        //                                                                                                          //
        //  When the barrier spell is cast it starts the coroutine BarrierRise with the duration time               //
        //                                                                                                          //
        //  In 1 second the barrier dissolve goes from 1 ( fully dissolved ) to 0 to create a growing effect        //
        //      We set the float _SliceAmount defined in the shader using the _timer ( 1 - Time.deltaTime )         //
        //          If the timer is ~0                                                                              //
        //              We wait for the duration time                                                               //
        //              Play the Barrier dissolve sound                                                             //
        //                  Start the coroutine BarrierDecay which is the same but inverted                         //
        //                      Break the coroutine ( stop it )                                                     //
        //                  The 'inverted coroutine' does the same but when the _timer > 1 we destroy the object    //
        //                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IEnumerator BarrierRise(float _waitTime)
        {
            float _timer = 1f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer -= Time.deltaTime;
                _barrierGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);

                if (_timer <= 0)
                {
                    yield return new WaitForSeconds(_waitTime);
                    SoundManager.instance.PlaySound(SOUNDS.PLAYERBARRIERDECAY, transform.position, true);
                    StartCoroutine(BarrierDecay());
                    yield break;
                }
            }
        }

        IEnumerator BarrierDecay()
        {
            float _timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;
                if (_barrierGameObject != null)
                {
                    _barrierGameObject.GetComponentInChildren<Renderer>().material.SetFloat("_SliceAmount", _timer);
                }
                if (_timer >= 1)
                {
                    Destroy(_barrierGameObject);
                    SPELL_BARRIER = false;
                    yield break;
                }
            }
        }

    }
}