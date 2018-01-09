using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnemyCombat
{
    public class EnemyAnimationSystem 
    {

        private Animator _animator;

        private float _rangedAttackNormalizedTime;

        public EnemyAnimationSystem(Animator _anim)
        {
            _animator = _anim;
        }

        public void StartEnemyClimb()
        {
            _animator.SetBool("isClimbing", true);
        }

        public void SetEnemyClimb(bool _set)
        {
            _animator.SetBool("isClimbing", _set);
        }

        public bool ClimbFinished()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Climb"))
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    _animator.SetBool("isClimbing", false);
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

        public void SetEnemyWalking(bool _set)
        {
            _animator.SetBool("isWalk", _set);
        }

        public void SetEnemyIdle()
        {
            _animator.SetBool("isIdle", true);
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isRun", false);
        }

        public void SetEnemyCombatIdle()
        {
            _animator.SetBool("isCombatIdle", true);
            _animator.SetBool("skipIdle", true);
            _animator.SetBool("isIdle", false);
            
            
        }

        public void StopEnemyCombatIdle()
        {
            _animator.SetBool("isCombatIdle", false);
        }

        public void SetEnemyRunning(float direction)
        {
            _animator.SetFloat("Direction", direction);
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isRun", true);
            _animator.SetBool("skipIdle", true);
        }

        public void StopEnemyWalking()
        {
            _animator.SetBool("isWalk", false);
            
        }

        public void StopEnemyRunning()
        {
            _animator.SetBool("isRun", false);
        }

        public void SetAttackPlayer()
        {

            _animator.SetBool("isCombatIdle", false);
            _animator.SetBool("isMeleeAttack", true);
            _animator.SetBool("skipIdle", true);
        }

        public void SetRangedAttackPlayer()
        {
            _animator.SetBool("isRangedAttack", true);
            _animator.SetBool("isCombatIdle", false);
            _animator.SetBool("skipIdle", true);

            
        }

        public void SetEnemyDeath()
        {
            _animator.SetBool("isIdle", false);
            _animator.SetBool("isMeleeAttack", false);
            _animator.SetBool("isCombatIdle", false);
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isRun", false);
            _animator.SetBool("skipIdle", true);
            _animator.SetBool("isDeath", true);
            _animator.SetBool("skipCombatIdle", true);
         }
        
        public void SetAttackFalse()
        {
           
                _animator.SetBool("isAttack", false);
                _animator.SetBool("isRangedAttack", false);
        }

        public void CancelAttackBool()
        {
            _animator.SetBool("isMeleeAttack", false); 
            _animator.SetBool("isAttack", false);
            _animator.SetBool("isRangedAttack", false);
        }

        public void SetSpecialAttack()
        {
            _animator.SetBool("isCombatIdle", false);
            _animator.SetBool("isRangedAttack", false);
            _animator.SetBool("isSpecialAttack", true);

        }

        public void StopSpecialAttack()
        {
            _animator.SetBool("isSpecialAttack", false);
            _animator.SetBool("isCombatIdle", true);
        }

        public void SetEnemyFrozen()
        {
            _animator.SetFloat("Direction", 0);
        }

        public void SetEnemyUnFrozen()
        {
            _animator.SetFloat("Direction", 1);
        }

        public void SetDirectionFloat(float _dir)
        {
            _animator.SetFloat("Direction", _dir);
        } 

    }
}