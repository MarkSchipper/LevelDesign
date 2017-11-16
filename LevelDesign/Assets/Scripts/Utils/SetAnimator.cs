using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem
{

    public class SetAnimator : MonoBehaviour
    {

        private static SetAnimator instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void SetPlayerController()
        {
            CombatSystem.AnimationSystem.SetController(GetComponent<Animator>());
        }

    }
}
