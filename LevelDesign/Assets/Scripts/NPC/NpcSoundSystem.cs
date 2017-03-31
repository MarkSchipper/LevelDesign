using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPCSystem
{

    public class NpcSoundSystem : MonoBehaviour
    {

        [FMODUnity.EventRef]
        private static string _playerFootsteps = "event:/footsteps/footstep_materials_mix";


        public static void PlayFootSteps(Vector3 _pos)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_pos));

            e.start();
            e.release();
        }
    }
}
