using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerSpawner : MonoBehaviour {

    private List<GameObject> _respawnPoints = new List<GameObject>();
    

    public static PlayerSpawner instance;
     
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("RespawnPoint"))
        {
            _respawnPoints.Add(point);
        }
	}

    public void LoadRespawnsPoints()
    {
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("RespawnPoint"))
        {
            _respawnPoints.Add(point);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerSpawn(Vector3 _spawnPosition)
    {
        CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.PLAYERSPAWN, _spawnPosition, false);

        GameObject _respawn = Instantiate(Resources.Load("VFX/VFX_Respawn"), _spawnPosition, Quaternion.identity) as GameObject;

        CombatSystem.PlayerController.instance.SetInputBlock(false);
        CombatSystem.PlayerController.instance.RespawnPlayer(_spawnPosition);
        CombatSystem.InteractionManager.instance.ShowUI(true);

        Destroy(_respawn, 4);

    }

    public void PlayerRespawn(Vector3 _playerPos)
    {
        float minDistance = float.MaxValue;
        int nearestIndex = -1;
        for (int i = 0; i < _respawnPoints.Count; i++)
        {
            float dist = (_respawnPoints[i].transform.position - _playerPos).magnitude;
            if(dist < minDistance)
            {
                nearestIndex = i;
                minDistance = dist;
            }
        }

        PlayerSpawn(_respawnPoints[nearestIndex].transform.position);
    }

   
}
