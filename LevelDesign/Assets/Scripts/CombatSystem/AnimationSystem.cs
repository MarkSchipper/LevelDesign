using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                      The Animation System                                            //
    //                                                                                                      //
    // Called by other classes to set booleans in the Player Character Controller                           //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////


    public class AnimationSystem : MonoBehaviour
    {
        private static Animator _playerAnimator;

        public static void SetController(Animator _animator)
        {
            _playerAnimator = _animator;
        }

        public static bool ReturnRunningAnim()
        {
            return _playerAnimator.GetBool("isRunning");
            
        }

        public static void SetPlayerRunning()
        {
            _playerAnimator.SetBool("isRunning", true);
            
        }

        public static void SetPlayerWalking()
        {
            _playerAnimator.SetBool("isWalking", true);
        }

        public static void StopPlayerWalking()
        {
            _playerAnimator.SetBool("isWalking", false);
        }

        public static void StopPlayerRunning()
        {
            _playerAnimator.SetBool("isRunning", false);
        }

        public static void SetPlayerIdle()
        {
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isCombatIdle", false);
            _playerAnimator.SetBool("skipIdle", false);
            _playerAnimator.speed = 1;
        }

        public static bool ReturnInCombatAnim()
        {
            return _playerAnimator.GetBool("isCombatIdle");
        }

        public static void SetCombatIdle()
        {
            _playerAnimator.SetBool("isCombatIdle", true);
            _playerAnimator.speed = 1;
        }

        public static void StopCombatIdle()
        {
            _playerAnimator.SetBool("isCombatIdle", false);
        }

        public static bool ReturnSpellCastAnim()
        {
            return _playerAnimator.GetBool("isRanged");
        }

        public static void SetRangedSpell(float _casttime)
        {
            _playerAnimator.SetBool("isRanged", true);
            _playerAnimator.speed = (1 / _casttime);
            
        }

        public static void StopRangedSpell()
        {
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Ranged"))
            {
                _playerAnimator.SetBool("isRanged", false);
            }
        }

        public static bool RangedSpellFinished()
        {
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Ranged"))
            {
                if (_playerAnimator.IsInTransition(0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SetSkipIdle(bool _set)
        {
            _playerAnimator.SetBool("skipIdle", _set);
        }
    }
}