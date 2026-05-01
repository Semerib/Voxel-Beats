using UnityEngine;
using VoxelBeat.Environment;

namespace VoxelBeat.Spawners
{
    public enum SpawnPattern
    {
        TargetCenter,
        TargetPlayer,
        Perpendicular,
        RandomTarget
    }

    public class SimpleSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ArenaManager arenaManager;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform targetPlayer; // Requerido para TargetPlayer

        [Header("Spawn Settings")]
        [SerializeField] private float spawnRate = 2f;
        [SerializeField] private float projectileSpeed = 5f;
        
        [Header("Pattern Settings")]
        [SerializeField] private SpawnPattern currentPattern = SpawnPattern.TargetCenter;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnRate)
            {
                SpawnFromEdge();
                _timer = 0;
            }
        }

        [ContextMenu("Test Spawn")]
        private void SpawnFromEdge()
        {
            if (projectilePrefab == null || arenaManager == null)
            {
                Debug.LogWarning("Spawner: Falta asignar el Prefab o el Arena Manager.");
                return;
            }

            // 1. Calcular punto aleatorio en el borde y saber de qué lado salió
            int side;
            Vector3 localSpawnPoint = GetRandomEdgePoint(out side);
            
            // 2. Convertir a posición mundial (esto respetará si la arena está rotada 45 grados)
            Vector3 spawnPosition = arenaManager.transform.TransformPoint(localSpawnPoint);
            spawnPosition.y = 1f; 

            // 3. Crear el proyectil
            GameObject projGO = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile proj = projGO.GetComponent<Projectile>();
            if (proj == null) proj = projGO.AddComponent<Projectile>();

            // 4. Calcular dirección de disparo según el patrón elegido
            Vector3 direction = Vector3.forward;

            switch (currentPattern)
            {
                case SpawnPattern.TargetCenter:
                    Vector3 targetCenter = arenaManager.transform.position;
                    targetCenter.y = spawnPosition.y;
                    direction = (targetCenter - spawnPosition).normalized;
                    break;
                
                case SpawnPattern.TargetPlayer:
                    if (targetPlayer != null)
                    {
                        Vector3 targetPos = targetPlayer.position;
                        targetPos.y = spawnPosition.y;
                        direction = (targetPos - spawnPosition).normalized;
                    }
                    else
                    {
                        Debug.LogWarning("TargetPlayer Pattern seleccionado pero no hay Player asignado.");
                    }
                    break;

                case SpawnPattern.Perpendicular:
                    // Dispara en línea recta cruzando la arena (ej: del Norte al Sur)
                    Vector3 localDirPerp = Vector3.zero;
                    if (side == 0) localDirPerp = Vector3.back;       // Norte -> Sur
                    else if (side == 1) localDirPerp = Vector3.forward; // Sur -> Norte
                    else if (side == 2) localDirPerp = Vector3.left;    // Este -> Oeste
                    else if (side == 3) localDirPerp = Vector3.right;   // Oeste -> Este
                    
                    direction = arenaManager.transform.TransformDirection(localDirPerp);
                    break;

                case SpawnPattern.RandomTarget:
                    // Dispara hacia un punto aleatorio dentro de la arena
                    float w = arenaManager.width / 2f;
                    float d = arenaManager.depth / 2f;
                    Vector3 randomLocalPoint = new Vector3(Random.Range(-w, w), 0, Random.Range(-d, d));
                    
                    // Convertir a posición mundial y ajustar altura
                    Vector3 randomWorldPoint = arenaManager.transform.TransformPoint(randomLocalPoint);
                    randomWorldPoint.y = spawnPosition.y;
                    
                    direction = (randomWorldPoint - spawnPosition).normalized;
                    break;
            }

            // 5. ¡Fuego!
            proj.Setup(projectileSpeed, direction);
        }

        private Vector3 GetRandomEdgePoint(out int side)
        {
            // Añadimos un pequeño margen (inset) para que nazcan ligeramente dentro y no choquen inmediatamente con su propia pared
            float inset = 1.5f; 
            float w = (arenaManager.width / 2f) - inset;
            float d = (arenaManager.depth / 2f) - inset;

            // Elegir uno de los 4 lados al azar (0=Norte, 1=Sur, 2=Este, 3=Oeste)
            side = Random.Range(0, 4);
            Vector3 point = Vector3.zero;

            switch (side)
            {
                case 0: point = new Vector3(Random.Range(-w, w), 0, d); break;
                case 1: point = new Vector3(Random.Range(-w, w), 0, -d); break;
                case 2: point = new Vector3(w, 0, Random.Range(-d, d)); break;
                case 3: point = new Vector3(-w, 0, Random.Range(-d, d)); break;
            }
            return point;
        }
    }
}
