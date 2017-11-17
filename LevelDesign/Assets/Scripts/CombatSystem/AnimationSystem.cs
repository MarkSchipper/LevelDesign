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
        private static Animator _staffAnimator;

        public static void SetController(Animator _animator)
        {
            _playerAnimator = _animator;
        }

        public static void SetStaffController(Animator _staff)
        {
            _staffAnimator = _staff;

        }

        public static bool ReturnRunningAnim()
        {
            return _playerAnimator.GetBool("isRunning");
            
            
        }

        public static void SetPlayerRunning(bool _backwards)
        {
            if (!_backwards)
            {
                _playerAnimator.SetFloat("Direction", 1f);
                _staffAnimator.SetFloat("Direction", 1f);
            }
            if(_backwards)
            {
                _playerAnimator.SetFloat("Direction", -0.5f);
                _staffAnimator.SetFloat("Direction", -0.5f);

            }
            
            _playerAnimator.SetBool("isRunning", true);
            _staffAnimator.SetBool("isRunning", true);
            _staffAnimator.SetBool("isCombatIdle", false);
            _playerAnimator.SetBool("isCombatIdle", false);
        }

        public static void SetPlayerWalking(bool _backwards)
        {
            if (!_backwards)
            {
                _playerAnimator.SetFloat("Direction", 1f);
                _staffAnimator.SetFloat("Direction", 1f);
            }
            if(_backwards)
            {
                _playerAnimator.SetFloat("Direction", -0.5f);
                _staffAnimator.SetFloat("Direction", -0.5f);
            }

            _playerAnimator.SetBool("isWalking", true);
            _staffAnimator.SetBool("isWalking", true);
        }

        public static void SetPlayerJumping()
        {
            _playerAnimator.SetBool("isJumping", true);
            _staffAnimator.SetBool("isJumping", true);
            

        }

        public static void StopPlayerJumping()
        {
            if (_playerAnimator.GetBool("isJumping"))
            {
                _playerAnimator.SetBool("isJumping", false);
                _staffAnimator.SetBool("isJumping", false);
            }
        }

        public static bool ReturnJumpingFinished()
        {

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
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

        public static void StopPlayerWalking()
        {
            _playerAnimator.SetBool("isWalking", false);
            _staffAnimator.SetBool("isWalking", false);
        }

        public static void StopPlayerRunning()
        {
            _playerAnimator.SetBool("isRunning", false);
            _staffAnimator.SetBool("isRunning", false);
        }

        public static void SetPlayerIdle()
        {
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isCombatIdle", false);
            _playerAnimator.SetBool("skipIdle", false);
            _playerAnimator.SetBool("isIdle", true);

            _staffAnimator.SetBool("isRunning", false);
            _staffAnimator.SetBool("isWalking", false);
            _staffAnimator.SetBool("isCombatIdle", false);
            _staffAnimator.SetBool("skipIdle", false);
            _staffAnimator.SetBool("isIdle", true);

            _playerAnimator.speed = 1;
        }

        public static void StopPlayerIdle()
        {
            _playerAnimator.SetBool("isIdle", false);
            _staffAnimator.SetBool("isIdle", false);
        }

        public static bool ReturnInCombatAnim()
        {
            return _playerAnimator.GetBool("isCombatIdle");
        }

        public static void SetCombatIdle()
        {
            _playerAnimator.SetBool("isCombatIdle", true);
            _staffAnimator.SetBool("isCombatIdle", true);
            _playerAnimator.speed = 1;
        }

        public static void StopCombatIdle()
        {
            _playerAnimator.SetBool("isCombatIdle", false);
            _staffAnimator.SetBool("isCombatIdle", false);
        }

        public static bool ReturnSpellCastAnim()
        {
            return _playerAnimator.GetBool("isRanged");
        }

        public static void SetRangedSpell()
        {
            _playerAnimator.SetBool("isRanged", true);
            _staffAnimator.SetBool("isRanged", true);

            

        }

        public static void CastRangedSpell()
        {
            _playerAnimator.SetBool("isRanged", false);
            _staffAnimator.SetBool("isRanged", false);

            _playerAnimator.SetBool("isSpellCast", true);
            _staffAnimator.SetBool("isSpellCast", true);

        }

        public static void StopRangedSpell()
        {

                _playerAnimator.SetBool("isSpellCast", false);
                _staffAnimator.SetBool("isSpellCast", false);
        }

        public static void CastHealingSpell()
        {
            _playerAnimator.SetBool("isRanged", false);
            _staffAnimator.SetBool("isRanged", false);

            _playerAnimator.SetBool("isSpellCast_two", true);
            _staffAnimator.SetBool("isSpellCast_two", true);
        }

        public static void StopHealingSpell()
        {
            _playerAnimator.SetBool("isSpellCast_two", false);
            _staffAnimator.SetBool("isSpellCast_two", false);
        }

        public static bool RangedSpellFinished()
        {
            
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spell casting"))
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

        public static bool HealingSpellFinished()
        {

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spell cast two"))
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
            _staffAnimator.SetBool("skipIdle", _set);
        }
    }
}