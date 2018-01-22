using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private bool _isSceneLoaded = false;

	void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            if (!_isSceneLoaded)
            {
                LevelManager.instance.SetIsNewScene(false);

                SceneManager.LoadSceneAsync("Dungeon_Lower", LoadSceneMode.Additive);
                //SceneManager.LoadScene("Dungeon_Lower", LoadSceneMode.Additive);
                _isSceneLoaded = true;
            }
        }
    }

}
