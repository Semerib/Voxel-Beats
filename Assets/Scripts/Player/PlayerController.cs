using UnityEngine;
using UnityEngine.InputSystem;

namespace VoxelBeat.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float dashForce = 20f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 0.8f;
        [SerializeField] private float rotationSpeed = 15f;

        [Header("Isometric Settings")]
        [SerializeField] private float isoAngle = 45f;
        private Matrix4x4 _isoMatrix;

        private Vector2 _moveInput;
        private Vector3 _moveDirection;
        private Rigidbody _rb;
        
        private bool _isDashing;
        private float _dashTimeCounter;
        private float _dashCooldownCounter;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.useGravity = false; // Arena estática, movimiento en plano XZ
            
            // Inicializar matriz isométrica
            _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, isoAngle, 0));
        }

        // Se llama desde PlayerInput (Events)
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        // Se llama desde PlayerInput (Events) - Asumiendo que usamos 'Jump' o 'Sprint' para Dash
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && !_isDashing && _dashCooldownCounter <= 0)
            {
                StartDash();
            }
        }

        private void Update()
        {
            if (_dashCooldownCounter > 0)
                _dashCooldownCounter -= Time.deltaTime;

            if (_isDashing)
            {
                _dashTimeCounter -= Time.deltaTime;
                if (_dashTimeCounter <= 0)
                {
                    StopDash();
                }
            }
            else
            {
                CalculateMovement();
            }
        }

        private void FixedUpdate()
        {
            if (!_isDashing)
            {
                ApplyMovement();
            }
        }

        private void CalculateMovement()
        {
            Vector3 input = new Vector3(_moveInput.x, 0, _moveInput.y);
            _moveDirection = _isoMatrix.MultiplyPoint3x4(input).normalized;
        }

        private void ApplyMovement()
        {
            // Movimiento usando velocity para mayor control en bullet hell
            _rb.linearVelocity = _moveDirection * moveSpeed;
            
            if (_moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
                _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }

        private void StartDash()
        {
            _isDashing = true;
            _dashTimeCounter = dashDuration;
            _dashCooldownCounter = dashCooldown;
            
            Vector3 dashDir = _moveDirection != Vector3.zero ? _moveDirection : transform.forward;
            
            // Deshabilitar fricción temporalmente o simplemente setear velocidad
            _rb.linearVelocity = dashDir * dashForce;
            
            // Visual feedback (opcional por ahora)
            Debug.Log("Player Dash!");
        }

        private void StopDash()
        {
            _isDashing = false;
            _rb.linearVelocity = Vector3.zero;
        }
    }
}
