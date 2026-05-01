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
        [SerializeField] private float invulnerabilityExtraTime = 0.1f; // Tiempo extra tras el dash
        [SerializeField] private float hitInvulnerabilityDuration = 1.0f; // Tiempo tras ser golpeado
        
        [Header("Visual Feedback")]
        [SerializeField] private Renderer playerRenderer;
        [SerializeField] private TrailRenderer dashTrail;
        [SerializeField] private float flashFrequency = 0.1f;

        [Header("Isometric Settings")]
        [SerializeField] private float isoAngle = 45f;
        private Matrix4x4 _isoMatrix;

        private Vector2 _moveInput;
        private Vector3 _moveDirection;
        private Rigidbody _rb;
        
        private bool _isDashing;
        private float _dashTimeCounter;
        private float _dashCooldownCounter;
        private float _invulnerableTimeCounter;
        private float _hitInvulnerableCounter; // Nuevo: específico para daño
        private float _flashTimer;

        public bool IsInvulnerable => _isDashing || _invulnerableTimeCounter > 0 || _hitInvulnerableCounter > 0;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            // Si no se asignó manualmente, intentamos buscar el renderer en los hijos
            if (playerRenderer == null) playerRenderer = GetComponentInChildren<Renderer>();
            
            _rb.freezeRotation = true;
            _rb.useGravity = false; // Arena estática, movimiento en plano XZ
            
            // Inicializar matriz isométrica
            _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, isoAngle, 0));
        }

        // Se llama desde PlayerInput (Events)
        // Se llama automáticamente por PlayerInput (Send Messages)
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            if (_moveInput != Vector2.zero) Debug.Log($"Movimiento detectado: {_moveInput}");
        }

        // Se llama automáticamente por PlayerInput (Send Messages) al usar la acción "Sprint"
        public void OnSprint(InputValue value)
        {
            if (value.isPressed && !_isDashing && _dashCooldownCounter <= 0)
            {
                StartDash();
            }
        }

        private void Update()
        {
            if (_dashCooldownCounter > 0)
                _dashCooldownCounter -= Time.deltaTime;

            if (_invulnerableTimeCounter > 0)
                _invulnerableTimeCounter -= Time.deltaTime;

            if (_hitInvulnerableCounter > 0)
                _hitInvulnerableCounter -= Time.deltaTime;

            HandleVisualFeedback();

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
            // Movimiento usando linearVelocity (Unity 6+)
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
            
            // Aplicar velocidad de dash
            _rb.linearVelocity = dashDir * dashForce;
            
            if (dashTrail != null) dashTrail.emitting = true;
            
            Debug.Log("Player Dash!");
        }

        private void StopDash()
        {
            _isDashing = false;
            _rb.linearVelocity = Vector3.zero;
            _invulnerableTimeCounter = invulnerabilityExtraTime;

            if (dashTrail != null) dashTrail.emitting = false;
        }

        public void TriggerInvulnerability(float duration)
        {
            // Solo lo activamos si el nuevo tiempo es mayor al actual
            _invulnerableTimeCounter = Mathf.Max(_invulnerableTimeCounter, duration);
        }

        public void OnHit()
        {
            _hitInvulnerableCounter = hitInvulnerabilityDuration;
        }

        private void HandleVisualFeedback()
        {
            if (playerRenderer == null) return;

            // El parpadeo ahora solo depende del contador de daño
            if (_hitInvulnerableCounter > 0)
            {
                _flashTimer -= Time.deltaTime;
                if (_flashTimer <= 0)
                {
                    playerRenderer.enabled = !playerRenderer.enabled;
                    _flashTimer = flashFrequency;
                }
            }
            else
            {
                playerRenderer.enabled = true;
            }
        }
    }
}
