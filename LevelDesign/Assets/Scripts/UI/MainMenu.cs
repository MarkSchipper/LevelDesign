using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MainMenu : MonoBehaviour {

    [FMODUnity.EventRef]
    public string _click;

    void Start()
    {
        Cursor.SetCursor(Resources.Load("Icons/Cursor/Cursor_Normal") as Texture2D, Vector2.zero, CursorMode.Auto);
    }

    public void NewGame()
    {
        StartCoroutine(LoadAsynchronously(1));
        PlayClickSound();
        GameObject.Find("Canvas").SetActive(false);
    }

    public void ResumeGame()
    {
        PlayClickSound();
        CombatSystem.InteractionManager.instance.ResumeGame();
    }

    public void QuitGame()
    {
        PlayClickSound();
        Inventory.instance.SaveInventory();
        Application.Quit();
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    void PlayClickSound()
    {
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_click);

        e.start();
        e.release();
    }

}
