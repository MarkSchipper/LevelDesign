using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


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

            Debug.Log(_loadAllScreenImg.Length);
            for (int i = 0; i < _loadAllScreenImg.Length; i++)
            {
                Debug.Log(_loadAllScreenImg[i].ToString());
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
                _isLoading = true;


                GameObject _canvasImage = new GameObject();
                _canvasImage.name = "LoadImage";
                _canvasImage.transform.SetParent(GameObject.Find("Canvas").transform);

                _canvasImage.AddComponent<Image>();
                _canvasImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _canvasImage.GetComponent<Image>().sprite = _loadingScreenImage;
                _canvasImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                _canvasImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_loadingScreenImage.textureRect.width, _loadingScreenImage.textureRect.height);

            }
        }

        private IEnumerator LoadLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_levelToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);
            yield return _async;
        }

        private void OnGUI()
        {
            if(_isLoading)
            {
                if(_async != null)
                {
                    GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, 100 * _async.progress, 50), _loadBar);
                }
            }
        }

    }
}
