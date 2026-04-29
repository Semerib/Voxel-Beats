using UnityEngine;
using VoxelBeat.Environment;

namespace VoxelBeat.Spawners
{
    public class SimpleSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float spawnRate = 2f;
        [SerializeField] private float projectileSpeed = 5f;
        
        [Header("Pattern Settings")]
        [SerializeField] private bool useRandomDirection = true;
        [SerializeField] private Vector3 fixedDirection = Vector3.forward;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnRate)
            {
                SpawnProjectile();
                _timer = 0;
            }
        }

        private void SpawnProjectile()
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning("Spawner: Projectile Prefab no asignado.");
                return;
            }

            GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile proj = projGO.GetComponent<Projectile>();
            
            if (proj == null)
            {
                proj = projGO.AddComponent<Projectile>();
            }

            Vector3 direction = useRandomDirection ? 
                new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) : 
                fixedDirection;

            proj.Setup(projectileSpeed, direction);
        }
    }
}
