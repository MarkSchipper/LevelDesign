using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

namespace LevelEditor
{
    public class LevelTrigger : MonoBehaviour
    {
        [SerializeField]
        private string _levelToLoad;

        [SerializeField]
        private string _loadingScreen;

        
        private Sprite[] _loadAllScreenImg;

        private Sprite _loadingScreenImage;
        private Texture2D _loadBar;

        private AsyncOperation _async = null;

        private bool _isLoading = false;
        private GameObject _canvasImage;

        private void Start()
        {
            _loadAllScreenImg = Resources.LoadAll<Sprite>("Scenes/LoadingScreens/");
            _loadBar = Resources.Load("Scenes/LoadingScreens/LoadingBar") as Texture2D;


            for (int i = 0; i < _loadAllScreenImg.Length; i++)
            {

                if (_loadAllScreenImg[i].ToString() == _loadingScreen + "(UnityEngine.Sprite)") 
                {
                    _loadingScreenImage = _loadAllScreenImg[i];
                }

               
            }
            
        }

        public void SetLevel(string _level, string _img)
        {
            _levelToLoad = _level;
            _loadingScreen = _img;
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "Player")
            {
                StartCoroutine(LoadLevel());
                CombatSystem.InteractionManager.instance.LoadLevel(true);
            }
        }

        private IEnumerator LoadLevel()
        {

            _async = SceneManager.LoadSceneAsync(_levelToLoad);

            while (!_async.isDone)
            {
                float progress = Mathf.Clamp01(_async.progress / 0.9f);
                CombatSystem.InteractionManager.instance.SetLoadingProgress(progress);
                yield return null;
            }
        }
    }
}
