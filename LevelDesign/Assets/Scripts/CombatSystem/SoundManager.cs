using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;

namespace CombatSystem
{
    public enum SOUNDS
    {
        FOOTSTEPS,
        PLAYERSPAWN,
        PLAYERHIT,
        PLAYERDEATH,
        PLAYERSPELLWARMUP,
        PLAYERSPELLCAST,
        PLAYERBLINK,
        PLAYERJUMP,
        PLAYERBARRIER,
        PLAYERBARRIERDECAY,
        PLAYERICETHRONE,
        INCOMBAT,
        ENEMYCHARGE,
        ENEMYDEATH,
        ENEMYSPAWN,
        ENEMYHIT,
        HEALING,
        LEVELUP,
        THUNDER,
        CRATE_BRAKE,
        UICLICK,
        MUSIC,
        DOOROPEN,
        MUSHROOMCLIMB,
        MUSHROOMAGGRO,
        MUSHROOMATTACK,
        MUSHROOMDEATH,
        
    }


    public class SoundManager : MonoBehaviour
    {

        [FMODUnity.EventRef]
        public string _playerGrunt;

        [FMODUnity.EventRef]
        public string _playerDeath;

        [FMODUnity.EventRef]
        public string _playerSpells;

        [FMODUnity.EventRef]
        public string _spellCasting;

        private static FMOD.Studio.EventInstance _spellCast;
        private static FMOD.Studio.EventInstance _musicInstance;

        [FMODUnity.EventRef]
        public string _playerBlink;

        [FMODUnity.EventRef]
        public string _barrierRise;

        [FMODUnity.EventRef]
        public string _barrierDecay;

        [FMODUnity.EventRef]
        public string _iceThrone;

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
        public string _mushroomClimb;

        [FMODUnity.EventRef]
        public string _mushroomAggro;

        [FMODUnity.EventRef]
        public string _mushroomAttack;

        [FMODUnity.EventRef]
        public string _mushroomDeath;

        [FMODUnity.EventRef]
        public string _levelUp;

        [FMODUnity.EventRef]
        public string _healing;

        [FMODUnity.EventRef]
        public string _thunder;
  
        [FMODUnity.EventRef]
        public string _dungeonAmbience;

        [Header("Sound Effects")]

        [FMODUnity.EventRef]
        public string _crateBrake;

        [FMODUnity.EventRef]
        public string _doorOpen;


        [FMODUnity.EventRef]
        public string _playerSpawn;

        [FMODUnity.EventRef]
        public string _uiClick;

        [Header("Music")]
        [FMODUnity.EventRef]
        public string _music;

        private static UnityEngine.Object[] _allFoliageSounds;
        private static List<string> _foliageSounds = new List<string>();

        private float m_Wood;
        private float m_Dirt;
        private float m_Grass;
        private float m_Stone;
        private float m_Snow;
        private float m_Water;
        private bool _isInWater = false;

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
           // PlayAmbience();
            _spellCast = FMODUnity.RuntimeManager.CreateInstance(_spellCasting);
            _musicInstance = FMODUnity.RuntimeManager.CreateInstance(_music);
            
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
                case SOUNDS.MUSIC:
                    PlayMusic();
                    break;
                case SOUNDS.FOOTSTEPS:
                    PlayFootsteps(_playerPos);
                    break;
                case SOUNDS.PLAYERSPAWN:
                    PlayPlayerSpawn(_playerPos);
                    break;
                case SOUNDS.PLAYERDEATH:
                    PlayPlayerDeath(_playerPos);
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
                case SOUNDS.THUNDER:
                    PlayThunder();
                    break;
                case SOUNDS.ENEMYHIT:
                    PlayEnemyHit(_playerPos);
                    break;
                case SOUNDS.PLAYERBARRIER:
                    PlayPlayerBarrier(_playerPos);
                    break;
                case SOUNDS.PLAYERBARRIERDECAY:
                    PlayPlayerBarrierDecay(_playerPos);
                    break;
                case SOUNDS.PLAYERICETHRONE:
                    PlayPlayerIceThrone(_playerPos);
                    break;
                case SOUNDS.CRATE_BRAKE:
                    PlayCrateBrake(_playerPos);
                    break;
                case SOUNDS.UICLICK:
                    PlayUIClick();
                    break;
                case SOUNDS.DOOROPEN:
                    PlayDoorOpen(_playerPos);
                    break;
                case SOUNDS.MUSHROOMAGGRO:
                    PlayMushroomAggro(_playerPos);
                    break;
                case SOUNDS.MUSHROOMATTACK:
                    PlayMushroomAttack(_playerPos);
                    break;
                case SOUNDS.MUSHROOMDEATH:
                    PlayMushroomDeath(_playerPos);
                    break;
                default:
                    break;
            }
        }

        public void StopSound(SOUNDS _sound)
        {
            switch(_sound)
            {
                   default:
                    break;
            }
        }

        public void PlayMusic()
        {
            FMOD.Studio.PLAYBACK_STATE _playing;
            _musicInstance.getPlaybackState(out _playing);
            if (_playing != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                _musicInstance.start();
            }
        }

        public void StopMusic()
        {
            SetParameter(_musicInstance, "Battle", 1);
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

       

        public void SetInWater(bool _set)
        {
            _isInWater = _set;
            
        }

        void PlayFootsteps(Vector3 _playerPos)
        {
            if (!_isInWater)
            {
                m_Wood = 0.0f;
                m_Dirt = 0.0f;
                m_Grass = 0.0f;
                m_Stone = 0.0f;
                m_Snow = 0.0f;
                m_Water = 0.0f;
                RaycastHit hit;

                if (Physics.Raycast(_playerPos, Vector3.down, out hit))
                {

                    if (hit.collider.gameObject.layer == 9)
                    {
                        m_Snow = 1.0f;

                    }
                    if (hit.collider.gameObject.layer == 13)
                    {
                        m_Wood = 1.0f;

                    }

                    if (hit.collider.gameObject.layer == 14)
                    {
                        m_Stone = 1.0f;

                    }

                    if (hit.collider.gameObject.layer == 12)
                    {
                        m_Dirt = 1.0f;

                    }



                    FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
                    e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

                    SetParameter(e, "wood", m_Wood);
                    SetParameter(e, "dirt", m_Dirt);
                    SetParameter(e, "snow", m_Snow);
                    SetParameter(e, "grass", m_Grass);
                    SetParameter(e, "stone", m_Stone);
                    SetParameter(e, "water", m_Water);
                    e.start();
                    e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
                }
            }
            else
            {
                m_Wood = 0.0f;
                m_Dirt = 0.0f;
                m_Grass = 0.0f;
                m_Stone = 0.0f;
                m_Snow = 0.0f;
                m_Water = 1.0f;


                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

                SetParameter(e, "wood", m_Wood);
                SetParameter(e, "dirt", m_Dirt);
                SetParameter(e, "snow", m_Snow);
                SetParameter(e, "grass", m_Grass);
                SetParameter(e, "stone", m_Stone);
                SetParameter(e, "water", m_Water);

                e.start();
                e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 

            }
        }

        void PlayPlayerSpawn(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerSpawn);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerDeath(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerDeath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
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

        void PlayPlayerBarrier(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_barrierRise);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerBarrierDecay(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_barrierDecay);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayPlayerIceThrone(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_iceThrone);
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

        void PlayMushroomAggro(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_mushroomAggro);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayMushroomAttack(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_mushroomAttack);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayMushroomDeath(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_mushroomDeath);
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

        void PlayCrateBrake(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_crateBrake);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));

            e.start();
            e.release();
        }

        void PlayDoorOpen(Vector3 _playerPos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_doorOpen);
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

        // UI

        void PlayUIClick()
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_uiClick);

            e.start();
            e.release();
        }

    }
}
