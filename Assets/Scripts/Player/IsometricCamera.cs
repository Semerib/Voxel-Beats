using UnityEngine;

namespace VoxelBeat.Player
{
    public class IsometricCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 30f;
        [SerializeField] private float smoothSpeed = 0.125f;

        [Header("Rotation Settings")]
        [SerializeField] private float xRotation = 45f;
        [SerializeField] private float yRotation = 45f;

        private Vector3 _calculatedOffset;

        private void Start()
        {
            UpdateCameraConfiguration();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Posición deseada basada en el offset calculado
            Vector3 desiredPosition = target.position + _calculatedOffset;
            
            // Suavizado de cámara
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        [ContextMenu("Apply Isometric Configuration")]
        public void UpdateCameraConfiguration()
        {
            // Aplicar rotación
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

            // Calcular el offset basado en la rotación inversa para que siempre mire al target
            // Si la cámara mira hacia transform.forward, el offset debe ser -transform.forward * distancia
            _calculatedOffset = -transform.forward * distance;
        }

        private void OnValidate()
        {
            if (Application.isPlaying) UpdateCameraConfiguration();
        }
    }
}
