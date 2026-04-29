using UnityEngine;

namespace VoxelBeat.Player
{
    public class IsometricCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(10f, 10f, -10f);
        [SerializeField] private float smoothSpeed = 0.125f;

        [Header("Rotation Settings")]
        [SerializeField] private float xRotation = 30f;
        [SerializeField] private float yRotation = 45f;

        private void Start()
        {
            // Configurar rotación inicial para perspectiva isométrica
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Posición deseada basada en el offset
            Vector3 desiredPosition = target.position + offset;
            
            // Suavizado de cámara
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        [ContextMenu("Apply Isometric Rotation")]
        public void ApplyRotation()
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }
}
