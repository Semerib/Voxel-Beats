using UnityEngine;

namespace VoxelBeat.Environment
{
    public class Projectile : MonoBehaviour
    {
        private float _speed;
        private Vector3 _direction;
        private float _lifeTime = 5f;

        public void Setup(float speed, Vector3 direction, float lifeTime = 5f)
        {
            _speed = speed;
            _direction = direction.normalized;
            _lifeTime = lifeTime;
            
            // Asegurarnos de que el proyectil mire a donde va
            if (_direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction);
                
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Obtenemos el controlador del jugador para chequear su estado
                VoxelBeat.Player.PlayerController player = other.GetComponent<VoxelBeat.Player.PlayerController>();
                
                if (player != null && player.IsInvulnerable)
                {
                    // El jugador es invulnerable, ignoramos el impacto
                    // Podríamos añadir un pequeño efecto visual aquí después
                    return; 
                }

                Debug.Log("<color=red>Player Hit!</color>");
                player.OnHit();
                Destroy(gameObject);
            }
            // Verificamos si el nombre empieza con "Wall" (así los nombra nuestro ArenaManager)
            else if (other.name.StartsWith("Wall"))
            {
                // Se destruye al chocar con la pared opuesta
                Destroy(gameObject);
            }
        }
    }
}
