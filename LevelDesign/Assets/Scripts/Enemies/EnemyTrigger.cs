using UnityEngine;
using System.Collections;

namespace EnemyCombat
{

    public class EnemyTrigger : MonoBehaviour
    {
        private bool _setOnce = false;
        private bool _isFrozen = false;

        [SerializeField]
        private bool _groundTrigger = false;

        void OnTriggerEnter(Collider coll)
        {
            if (!_groundTrigger)
            {
                if (transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnAliveState())
                {
                    if (!_isFrozen)
                    {
                        if (!transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnLeashingBack())
                        {
                            if (transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnClimbFinished())
                            {
                                if (coll.tag == "Player")
                                {
                                    if (!coll.GetComponent<CombatSystem.PlayerController>().ReturnPlayerDead())
                                    {
                                        // If the enemy engages the player, the player get set in Combat
                                        // we pass the parent gameobject to the player class so we can set the target in the interface to this enemy

                                        CombatSystem.InteractionManager.instance.SetSelected(this.transform.parent.gameObject);
                                        coll.GetComponent<CombatSystem.PlayerController>().SetPlayerInCombat(true);
                                        coll.GetComponent<CombatSystem.PlayerController>().AddEnemyList(transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnGameID(), transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>());
                                        coll.GetComponent<CombatSystem.PlayerController>().SetEnemy(this.transform.parent.gameObject);
                                        this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyMotor>().SetAttack(true, coll.gameObject);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                if (!transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnClimbFinished())
                {
                    if (coll.tag == "Player")
                    {
                        if (transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnClimbGround())
                        {
                            transform.parent.GetComponent<EnemyCombat.EnemyMotor>().EnemyStartClimb();
                            transform.parent.GetComponent<EnemyCombat.EnemySoundManager>().PlaySound(EnemySound.CLIMB, transform.position);
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        void OnTriggerStay(Collider coll)
        {
            if (!_groundTrigger)
            {
                if (transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnAliveState())
                {
                    if (!_isFrozen)
                    {
                        if (!transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnLeashingBack())
                        {
                            if (transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnClimbFinished())
                            {
                                if (coll.tag == "Player")
                                {
                                    if (!coll.GetComponent<CombatSystem.PlayerController>().ReturnPlayerDead())
                                    {
                                        // If the enemy engages the player, the player get set in Combat
                                        // we pass the parent gameobject to the player class so we can set the target in the interface to this enemy
                                        if (!_setOnce)
                                        {
                                            CombatSystem.InteractionManager.instance.SetSelected(this.transform.parent.gameObject);
                                            coll.GetComponent<CombatSystem.PlayerController>().SetPlayerInCombat(true);
                                            //coll.GetComponent<CombatSystem.PlayerController>().AddEnemyList(transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnGameID());
                                            coll.GetComponent<CombatSystem.PlayerController>().SetEnemy(this.transform.parent.gameObject);
                                            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.INCOMBAT, Vector3.forward, true);
                                            _setOnce = true;
                                        }
                                        if (!this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyMotor>().ReturnLeashingBack())
                                        {
                                            this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyMotor>().SetAttack(true, coll.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void OnTriggerExit(Collider coll)
        {
            if (coll.tag == "Player")
            {
                if (!transform.parent.GetComponent<EnemyCombat.EnemyMotor>().ReturnLeashingBack())
                {
                    if (!_isFrozen)
                    {
                        coll.GetComponent<CombatSystem.PlayerController>().SetPlayerInCombat(false);
                        this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyMotor>().SetAttack(false, null);
                    }
                }
            }
            
        }

        public void SetFrozen(bool _set)
        {
            _isFrozen = _set;
        }
    }

    
}