using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // called zero
    void Awake()
    {
    }

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CombatSystem.PlayerController.instance.SpawnPlayer();
        CombatSystem.CameraController.instance.SetPostProfile(scene.name);
        CombatSystem.SoundManager.instance.SetInWater(false);
        CombatSystem.InteractionManager.instance.LoadLevel(false);
        PlayerSpawner.instance.LoadRespawnsPoints();
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
}