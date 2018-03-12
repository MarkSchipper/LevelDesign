using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCombat
{

    public class EnemySpawner : MonoBehaviour
    {

        private GameObject _rangedSpell;

        private bool _hasSpawned = false;

        private GameObject _deathParticles;
        private GameObject _hitParticles;
        private GameObject _specialAttack;

        private GameObject _enemy;

        void OnEnable()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider coll)
        {
            if(coll.tag == "Player")
            {
                if(!_hasSpawned)
                {
                    EnemySpawnFeedback();
                    
                    StartCoroutine(EnemySpawnDelay());
                    _hasSpawned = true;
                }
            }
        }

        void Start()
        {
            foreach (Transform t in transform.parent.gameObject.GetComponentInChildren<Transform>())
            {
                if (t.gameObject.GetComponent<EnemyBehaviour>() != null)
                {
                    _enemy = t.gameObject;
                    t.gameObject.SetActive(false);
                }
            }
        }

        void EnemySpawnFeedback()
        {
            GameObject _spawnFeedbackPosition = Instantiate(Resources.Load("Characters/Enemies/Feedback/Spawn/EnemySpawn")) as GameObject;
            _spawnFeedbackPosition.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.2f, this.transform.position.z);
           

            GameObject _spawnImpactParticles = Instantiate(Resources.Load("Characters/Enemies/Feedback/Spawn/EnemySpawn_ImpactParticles")) as GameObject;
            _spawnImpactParticles.transform.position = this.transform.position;
            _spawnImpactParticles.GetComponentInChildren<Animator>().SetBool("spawn", true);
            _spawnImpactParticles.GetComponentInChildren<ParticleSystem>().Play();
            StartCoroutine(DestroySpawnPosition(_spawnFeedbackPosition, _spawnImpactParticles));

        }

        void SpawnEnemy()
        {
            _enemy.SetActive(true);
        }

        IEnumerator EnemySpawnDelay()
        {
            yield return new WaitForSeconds(0.66f);
            CombatSystem.SoundManager.instance.PlaySound(CombatSystem.SOUNDS.ENEMYSPAWN, this.transform.position, true);
           // CombatSystem.CameraController.CameraShake(8, 1.5f);
            SpawnEnemy();
        }

        IEnumerator DestroySpawnPosition(GameObject _obj, GameObject _particles)
        {
            yield return new WaitForSeconds(2);
            Destroy(_obj);
            Destroy(_particles);
        }
    }
}