using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Theme
{

    public class Triggers : MonoBehaviour
    {
        private static int _soundSelectIndex;
        private static bool _playSoundOnce;
        private static int _soundTriggerSize = 5;
        private static float _soundVolume = 0.5f;

        
        private static GameObject _objectToAdd;

        private static string[] _gameplayTriggerSelectType;
        private static int _gameplayTriggerIndex;

        void OnEnable()
        {
            _gameplayTriggerSelectType = new string[] { "Instant Death", "Level Up" };
        }

        public static void AudioTrigger()
        {

            CombatSystem.SoundManager.GetAllFoliage();

            List<string> _sounds = CombatSystem.SoundManager.ReturnAllFoliage();

            GUILayout.Label("Which Sound");
            _soundSelectIndex = EditorGUILayout.Popup(_soundSelectIndex, _sounds.ToArray());

            _playSoundOnce = EditorGUILayout.Toggle("Play Once?: ", _playSoundOnce);
            _soundVolume = EditorGUILayout.FloatField("Volume: ", _soundVolume);

            _soundTriggerSize = EditorGUILayout.IntField("Size of Trigger: ", _soundTriggerSize);

            if (GUILayout.Button("Add Sound Trigger"))
            {
                _objectToAdd = Instantiate(Resources.Load("World_Building/GamePlay/SoundTrigger")) as GameObject;
                _objectToAdd.GetComponent<Transform>().localScale = new Vector3(_soundTriggerSize, _soundTriggerSize, _soundTriggerSize);
                _objectToAdd.GetComponentInChildren<SoundTrigger>().SetData(_sounds[_soundSelectIndex], _playSoundOnce, _soundVolume);
                _objectToAdd.name = "SoundTrigger-" + _sounds[_soundSelectIndex];

                if (GameObject.Find("AUDIO") != null)
                {
                    _objectToAdd.transform.SetParent(GameObject.FindGameObjectWithTag("Audio").transform);
                }
                else
                {
                    GameObject _obj = new GameObject();
                    _obj.name = "AUDIO";
                    _obj.tag = "Audio";

                    _objectToAdd.transform.SetParent(_obj.transform);
                }

                LevelEditor.ObjectPainter.SetAddingTriggersToScene(true);
                LevelEditor.ObjectPainter.SetAddingToScene();
            }
        }

        public static void GameplayTrigger()
        {
            _gameplayTriggerIndex = EditorGUILayout.Popup(_gameplayTriggerIndex, _gameplayTriggerSelectType);

            if (_gameplayTriggerSelectType[_gameplayTriggerIndex] == "Instant Death")
            {
                // add cube that kills the player
            }
            if (_gameplayTriggerSelectType[_gameplayTriggerIndex] == "Level Up")
            {

            }
        }


        public static GameObject ReturnObjectToAdd()
        {
            return _objectToAdd;
        }
    }
}