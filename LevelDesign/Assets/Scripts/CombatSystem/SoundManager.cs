using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;

namespace CombatSystem
{
    public enum SOUNDS
    {
        FOOTSTEPS,
        PLAYERHIT,
        PLAYERSPELLWARMUP,
        PLAYERSPELLCAST,
        PLAYERBLINK,
        PLAYERJUMP,
        INCOMBAT,
        ENEMYCHARGE,
        ENEMYDEATH,
        ENEMYSPAWN,
        ENEMYHIT,
        HEALING,
        LEVELUP,
        RAIN,
        THUNDER,
        
    }

    public class SoundManager : MonoBehaviour
    {

        [FMODUnity.EventRef]
        public string _playerGrunt;

        [FMODUnity.EventRef]
        public string _playerSpells;

        [FMODUnity.EventRef]
        public string _spellCasting;

        private static FMOD.Studio.EventInstance _spellCast;

        [FMODUnity.EventRef]
        public string _playerBlink;

        [FMODUnity.EventRef]
        public string _playerJump;

        [FMODUnity.EventRef]
        public string _playerFootsteps;

        [FMODUnity.EventRef]
        public string _enemyCharge;

        [FMODUnity.EventRef]
        public string _enemyHit;

        [FMODUnity.EventRef]
        public string _inCombatSwoosh;

        [FMODUnity.EventRef]
        public string _enemyDeath;

        [FMODUnity.EventRef]
        public string _enemySpawn;

        [FMODUnity.EventRef]
        public string _levelUp;

        [FMODUnity.EventRef]
        public string _healing;

        [FMODUnity.EventRef]
        public string _thunder;

        [FMODUnity.EventRef]
        public string _rain;

        [FMODUnity.EventRef]
        public string _dungeonAmbience;

        private static UnityEngine.Object[] _allFoliageSounds;
        private static List<string> _foliageSounds = new List<string>();

        private float m_Wood;
        private float m_Dirt;
        private float m_Grass;
        private float m_Stone;
        private float m_Snow;

        public static SoundManager instance;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }

            DontDestroyOnLoad(instance);
        }

        // Use this for initialization
        void Start()
        {
            PlayAmbience();
            _spellCast = FMODUnity.RuntimeManager.CreateInstance(_spellCasting);
        }

        // Method overloading
        public void PlaySound(SOUNDS _sound)
        {
            PlaySound(_sound, Vector3.zero, true);
        }

        public void PlaySound(SOUNDS _sound, Vector3 _playerPos, bool _play)
        {
            switch (_sound)
            {
                case SOUNDS.FOOTSTEPS:
                    PlayFootsteps(_playerPos);
                    break;
                case SOUNDS.PLAYERHIT:
                    PlayPlayerHit(_playerPos);
                    break;
                case SOUNDS.PLAYERSPELLWARMUP:
                    PlayPlayerSpellWarmup(_playerPos, _play);
                    break;
                case SOUNDS.PLAYERSPELLCAST:
                    PlayPlayerSpellCast(_playerPos);
                    break;
                case SOUNDS.PLAYERBLINK:
                    PlayPlayerBlink(_playerPos);
                    break;

                case SOUNDS.PLAYERJUMP:
                    PlayPlayerJump(_playerPos);
                    break;
                case SOUNDS.INCOMBAT:
                    PlayInCombat();
                    break;
                case SOUNDS.ENEMYCHARGE:
                    PlayEnemyCharge(_playerPos);
                    break;
                case SOUNDS.ENEMYDEATH:
                    PlayEnemyDeath(_playerPos);
                    break;
                case SOUNDS.ENEMYSPAWN:
                    PlayEnemySpawn(_playerPos);
                    break;
                case SOUNDS.HEALING:
                    PlayPlayerHealing(_playerPos);
                    break;
                case SOUNDS.LEVELUP:
                    PlayLevelUp(_playerPos);
                    break;
                case SOUNDS.RAIN:
                    PlayRain();
                    break;
                case SOUNDS.THUNDER:
                    PlayThunder();
                    break;
                case SOUNDS.ENEMYHIT:
                    PlayEnemyHit(_playerPos);
                    break;
                default:
                    break;
            }
        }

        void PlayFootsteps(Vector3 _playerPos)
        {
            m_Wood = 0.0f;
            m_Dirt = 0.0f;
            m_Grass = 0.0f;
            m_Stone = 0.0f;
            m_Snow = 0.0f;

            RaycastHit hit;

            if(Physics.Raycast(_playerPos, Vector3.down, out hit))
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
                    m_Dirt = 0.0f;
                    m_Wood = 0.0f;
                }

                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

                SetParameter(e, "wood", m_Wood);
                SetParameter(e, "dirt", m_Dirt);
                SetParameter(e, "snow", m_Snow);
                SetParameter(e, "grass", m_Grass);
                SetParameter(e, "stone", m_Stone);

                e.start();
                e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
            }
    }

        void PlayPlayerHit(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerGrunt);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerSpellWarmup(Vector3 _playerPos, bool _play)
        {

            if (_play)
            {
                _spellCast.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));
                _spellCast.start();
            }
            if (!_play)
            {
                _spellCast.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        void PlayPlayerSpellCast(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerSpells);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerBlink(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerBlink);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerJump(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerJump);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayInCombat()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_inCombatSwoosh);

            e.start();
            e.release();
        }

        void PlayEnemyCharge(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyCharge);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayEnemyHit(Vector3 _playerPos)
        {
                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyHit);
                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

                e.start();
                e.release();
        }

        void PlayEnemyDeath(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemyDeath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayEnemySpawn(Vector3 enemyPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_enemySpawn);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(enemyPos));

            e.start();
            e.release();
        }

        void PlayPlayerHealing(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_healing);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayLevelUp(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_levelUp);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayThunder()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_thunder);
            e.start();
            e.release();
        }

        void PlayRain()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_rain);
            e.start();
            e.release();
        }

        void PlayAmbience()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_dungeonAmbience);
            e.start();
        }

        void SetParameter(FMOD.Studio.EventInstance e, string name, float value)
        {
            FMOD.Studio.ParameterInstance parameter;
            e.getParameter(name, out parameter);

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
