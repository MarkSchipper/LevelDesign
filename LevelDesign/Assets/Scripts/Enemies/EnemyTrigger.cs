using UnityEngine;
using System.Collections;

namespace EnemyCombat
{

    public class EnemyTrigger : MonoBehaviour
    {
        private bool _setOnce = false;
        private bool _isFrozen = false;

        void OnTriggerEnter(Collider coll)
        {

            if (transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnIsAlive())
            {
                if (!_isFrozen)
                {
                    if (!transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnLeashingBack())
                    {
                        if (coll.tag == "Player")
                        {
                            // If the enemy engages the player, the player get set in Combat
                            // we pass the parent gameobject to the player class so we can set the target in the interface to this enemy

                            CombatSystem.InteractionManager.instance.SetSelected(this.transform.parent.gameObject);
                            coll.GetComponent<CombatSystem.PlayerController>().SetPlayerInCombat(true);
                            coll.GetComponent<CombatSystem.PlayerController>().AddEnemyList(transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnGameID());

                            coll.GetComponent<CombatSystem.PlayerController>().SetEnemy(this.transform.parent.gameObject);

                            this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetAttack(true, coll.gameObject);
                            this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetLeashStart(transform.position);

                            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.INCOMBAT, Vector3.forward, true);

                        }
                    }
                }
                else
                {

                }
                
            }
        }

        void OnTriggerStay(Collider coll)
        {
            if (transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnIsAlive())
            {
                if (!_isFrozen)
                {
                    if (!transform.parent.GetComponent<EnemyCombat.EnemyBehaviour>().ReturnLeashingBack())
                    {
                        if (coll.tag == "Player")
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
                            if (!this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().MaxLeashDistanceMet())
                            {
                                this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetAttack(true, coll.gameObject);
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
                if (!_isFrozen)
                {
                    coll.GetComponent<CombatSystem.PlayerController>().SetPlayerInCombat(false);
                    this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyBehaviour>().SetAttack(false, null);
                }
            }
            
        }

        public void SetFrozen(bool _set)
        {
            _isFrozen = _set;
        }
    }

    
}