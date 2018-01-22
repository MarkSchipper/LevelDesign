using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private bool _isNewScene = true;

    // called zero
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_isNewScene)
        {
            CombatSystem.PlayerController.instance.SpawnPlayer();
            if (GameObject.FindObjectOfType<LevelSettings>() != null)
            {
                if (GameObject.FindObjectOfType<LevelSettings>().GetComponent<LevelSettings>().ReturnProfile() == null)
                {
                    CombatSystem.CameraController.instance.SetPostProfile(scene.name);
                }
            }
            else
            {
                CombatSystem.CameraController.instance.SetPostProfile(GameObject.FindObjectOfType<LevelSettings>().GetComponent<LevelSettings>().ReturnProfile().ToString());
            }
            CombatSystem.CameraController.instance.SetLevelSettings(GameObject.FindObjectOfType<LevelSettings>().GetComponent<LevelSettings>().ReturnFarClipPlane());

            CombatSystem.SoundManager.instance.SetInWater(false);
            CombatSystem.InteractionManager.instance.LoadLevel(false);
            PlayerSpawner.instance.LoadRespawnsPoints();
        }
    }

    // called third
    void Start()
    {
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetIsNewScene(bool _set)
    {
        _isNewScene = _set;
    }
}