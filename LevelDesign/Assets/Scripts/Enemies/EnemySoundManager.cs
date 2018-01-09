using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace EnemyCombat
{

    public enum EnemySound
    {
        ATTACK,
        FOOTSTEPS,
        DEATH,
        CHARGE,
        SPAWN,
        CLIMB,
    }

    public class EnemySoundManager : MonoBehaviour
    {

        [FMODUnity.EventRef]
        public string _enemyAttack;

        [FMODUnity.EventRef]
        public string _enemyFootsteps;

        [FMODUnity.EventRef]
        public string _enemyDeath;

        [FMODUnity.EventRef]
        public string _enemyCharge;

        [FMODUnity.EventRef]
        public string _enemySpawn;

        [FMODUnity.EventRef]
        public string _enemyClimb;

        public EnemySoundManager()
        {

        }

        public void PlaySound(EnemySound sound, Vector3 pos)
        {
            switch(sound)
            {
                case EnemySound.ATTACK:
                    PlayEnemyAttack(pos);
                    break;
                case EnemySound.CHARGE:
                    PlayEnemyCharge(pos);
                    break;
                case EnemySound.CLIMB:
                    PlayEnemyClimb(pos);
                    break;
                case EnemySound.DEATH:
                    PlayEnemyDeath(pos);
                    break;
                case EnemySound.FOOTSTEPS:
                    PlayEnemyFootsteps(pos);
                    break;
                case EnemySound.SPAWN:
                    PlayEnemySpawn(pos);
                    break;
                default:
                    break;
            }
        }

        void PlayEnemyAttack(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyAttack);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

        void PlayEnemyCharge(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyCharge);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

        void PlayEnemyClimb(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyClimb);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

        void PlayEnemyDeath(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyDeath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

        void PlayEnemyFootsteps(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyFootsteps);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

        void PlayEnemySpawn(Vector3 pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemySpawn);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));

            e.start();
            e.release();
        }

    }
}
