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
            // En la fase 1 solo detectamos el impacto
            if (other.CompareTag("Player"))
            {
                Debug.Log("<color=red>Player Hit!</color>");
                // Aquí iría la lógica de daño en la Fase 4
                Destroy(gameObject);
            }
        }
    }
}
