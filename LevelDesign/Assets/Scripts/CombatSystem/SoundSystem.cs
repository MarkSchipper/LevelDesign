using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

namespace CombatSystem
{

    public class SoundSystem : MonoBehaviour
    {
        [FMODUnity.EventRef]
        private static string _playerGrunt = "event:/grunts/female_grunts";

        [FMODUnity.EventRef]
        private static string _playerSpells = "event:/spells/fire_ball_cast";

        [FMODUnity.EventRef]
        private static string _playerBlink = "event:/spells/blink";

        [FMODUnity.EventRef]
        private static string _playerFootsteps = "event:/footsteps/footstep_materials_mix";

        [FMODUnity.EventRef]
        private static string _enemyCharge = "event:/grunts/Enemy_Infected_Grunt";

        [FMODUnity.EventRef]
        private static string _inCombatSwoosh = "event:/Swoosh/InCombat";

        [FMODUnity.EventRef]
        private static string _enemyDeath = "event:/Death/Enemy_Death";

        [FMODUnity.EventRef]
        private static string _enemySpawn = "event:/Enemies/EnemySpawn";


        [FMODUnity.EventRef]
        private static FMOD.Studio.EventDescription eventDescription = null;
        private static string Event = "";


        private static float m_Wood;
        private static float m_Water;
        private static float m_Dirt;
        private static float m_Sand;
        private static float m_Grass;
        private static float m_Stone;
        private static float m_Snow;

        private static UnityEngine.Object[] _allFoliageSounds;
        private static List<string> _foliageSounds = new List<string>();
        
        public static void PlaySpellCast(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerSpells);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();

        }

        public static void PlayerBlink(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerBlink);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        public static void PlayFootSteps(Vector3 _playerPos)
        {
            //Defaults
            m_Water = 0.0f;
            m_Dirt = 0.0f;
            m_Sand = 0.0f;
            m_Wood = 0.0f;
            m_Snow = 0.0f;
            m_Grass = 0.0f;
            m_Stone = 0.0f;

            RaycastHit hit;
            if (Physics.Raycast(_playerPos, Vector3.down, out hit, 1000.0f))
            {

                if (hit.collider.gameObject.layer == 9)
                {
                    m_Snow = 1.0f;
                    
                }
                if (hit.collider.gameObject.layer == 14)
                {
                    m_Stone = 1.0f;

                }

                if (hit.collider.gameObject.layer == 12)
                {
                    m_Dirt = 1.0f;

                }

                else
                {
                    m_Water = 0.0f;
                    m_Dirt = 0.0f;
                    m_Sand = 0.0f;
                    m_Wood = 0.0f;
                }
            }

           

            if (_playerFootsteps != null)
            {
                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

                SetParameter(e, "wood", m_Wood);
                SetParameter(e, "dirt", m_Dirt);
                //SetParameter(e, "sand", m_Sand);
                //SetParameter(e, "Water", m_Water);
                SetParameter(e, "snow", m_Snow);
                SetParameter(e, "grass", m_Grass);
                SetParameter(e, "stone", m_Stone);



                e.start();
                e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
            }
        }

        public static void PlayerHit(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerGrunt);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        public static void EnemyHit()
        {

        }

        public static void EnemyCharge(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyCharge);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        public static void EnemyDeath(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyDeath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        public static void InCombat()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_inCombatSwoosh);
            //e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        public static void EnemySpawn(Vector3 _enemyPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemySpawn);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_enemyPos));

            e.start();
            e.release();
        }

        static void SetParameter(FMOD.Studio.EventInstance e, string name, float value)
        {
            FMOD.Studio.ParameterInstance parameter;
            e.getParameter(name, out parameter);
            if (parameter == null)
            {
                return;
            }
            parameter.setValue(value);
        }

        public static void GetAllFoliage()
        {
            _foliageSounds.Clear();
            _allFoliageSounds = Resources.LoadAll("Audio/Foliage/");

            for (int i = 0; i < _allFoliageSounds.Length; i++)
            {
                
                if (_allFoliageSounds[i].GetType().ToString() == "UnityEngine.AudioClip")
                {
                    // Strip the length of the string of the objects in the folder
                    // By default it is :
                    //                      Plant ( UnityEngine.GameObject )
                    // Add it to a list
                    _foliageSounds.Add(_allFoliageSounds[i].ToString().Remove(_allFoliageSounds[i].ToString().Length - 24));
                    
                }
            }

        }

        public static List<string> ReturnAllFoliage()
        {
            return _foliageSounds;
        }
    }

}
