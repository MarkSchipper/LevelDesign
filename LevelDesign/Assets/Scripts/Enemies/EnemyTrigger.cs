using UnityEngine;
using System.Collections;

namespace CombatSystem
{

    public class EnemyTrigger : MonoBehaviour
    {
        private bool _setOnce = false;


        void OnTriggerEnter(Collider coll)
        {

            if (EnemyCombat.EnemyCombatSystem.ReturnIsAlive())
            {

                if (coll.tag == "Player")
                {
                    // If the enemy engages the player, the player get set in Combat
                    // we pass the parent gameobject to the player class so we can set the target in the interface to this enemy

                    CombatSystem.GameInteraction.SetSelectedUI(this.transform.parent.gameObject);
                    coll.GetComponent<CombatSystem.PlayerMovement>().PlayerInCombat(true);
                    coll.GetComponent<CombatSystem.PlayerMovement>().SetEnemy(this.transform.parent.gameObject);


                    this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().SetTarget(coll.gameObject);
                    this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().SetAttack(true);

                    CombatSystem.SoundSystem.InCombat();

                }
            }
        }

        void OnTriggerStay(Collider coll)
        {
            if (coll.tag == "Player")
            {
                // If the enemy engages the player, the player get set in Combat
                // we pass the parent gameobject to the player class so we can set the target in the interface to this enemy
                if (!_setOnce)
                {
                    CombatSystem.GameInteraction.SetSelectedUI(this.transform.parent.gameObject);
                    coll.GetComponent<CombatSystem.PlayerMovement>().PlayerInCombat(true);
                    coll.GetComponent<CombatSystem.PlayerMovement>().SetEnemy(this.transform.parent.gameObject);


                    
                    this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().SetAttack(true);

                    CombatSystem.SoundSystem.InCombat();

                    _setOnce = true;
                }
                this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().SetTarget(coll.gameObject);
            }
        }

        void OnTriggerExit(Collider coll)
        {

            coll.GetComponent<CombatSystem.PlayerMovement>().PlayerInCombat(false);
            this.transform.parent.transform.parent.GetComponentInChildren<EnemyCombat.EnemyCombatSystem>().SetAttack(false);
            
            
        }
    }

    
}