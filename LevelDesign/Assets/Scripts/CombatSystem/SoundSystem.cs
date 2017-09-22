using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

namespace CombatSystem
{

    public class SoundSystem : MonoBehaviour
    {
        
        private static UnityEngine.Object[] _allFoliageSounds;
        private static List<string> _foliageSounds = new List<string>();
       
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
