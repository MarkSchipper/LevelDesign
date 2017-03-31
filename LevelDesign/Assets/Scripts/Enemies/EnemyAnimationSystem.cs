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

        public void SetEnemyWalking()
        {
            _animator.SetBool("isWalk", true);
        }

        public void SetEnemyIdle()
        {

        }

        public void SetEnemyCombatIdle()
        {
            _animator.SetBool("isCombatIdle", true);
            _animator.SetBool("skipIdle", true);
            
            
        }

        public void SetEnemyRunning()
        {
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isRun", true);
            _animator.SetBool("skipIdle", true);
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

    }
}