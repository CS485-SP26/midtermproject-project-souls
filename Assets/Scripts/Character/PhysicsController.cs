using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsController : MovementController
    {
        #region 1. Settings & State
        
        [Header("Movement Settings")]
        [SerializeField] private float speed = 10.0f;
        [SerializeField] private float maxSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 15.0f; // New: Controls how fast they turn
        [SerializeField] private float movementDrag = 0.5f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private int maxJumps = 2;

        // Internal State Structures
        private struct InputState
        {
            public Vector2 moveInput; // WASD
            public bool jumpInput;    // Space
        }

        private struct JumpState
        {
            public bool isGrounded;
            public bool onWall;
            public Vector3 wallNormal;
            public int jumpsRemaining;
            public float lastJumpTime;
        }

        private InputState _input;
        private JumpState _jumpState;
        
        #endregion

        #region 2. Unity Lifecycle

        protected override void Start()
        {
            base.Start();
            
            rb.linearDamping = movementDrag;
            rb.freezeRotation = true; // Physics shouldn't rotate player, our code will.

            _jumpState.jumpsRemaining = maxJumps;
            _jumpState.isGrounded = true;
        }

        protected override void FixedUpdate()
        {
            // 1. Movement & Rotation
            HandleMovement();
            
            // 2. Physics Cleanup
            ClampVelocity();
            ApplyDrag();
            
            // 3. Actions
            HandleJump();
        }

        #endregion

        #region 3. Input Handling (Public API)

        public override void Move(Vector2 input)
        {
            _input.moveInput = input;
        }

        public override void Jump()
        {
            _input.jumpInput = true;
        }

        // Only used for Animation logic now
        public Vector2 GetHorizontalSpeedPercent()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return new Vector2(horizontalVelocity.magnitude / maxSpeed, 0); 
        }

        #endregion

        #region 4. Movement Logic

        private void HandleMovement()
        {
            // Get raw input direction (World Space)
            // WASD directly maps to X/Z direction
            Vector3 targetDir = new Vector3(_input.moveInput.x, 0, _input.moveInput.y);

            // 1. Handle Rotation
            // Only rotate if we are actually moving
            if (targetDir.sqrMagnitude > 0.01f)
            {
                // Calculate target rotation (Face the direction we are pressing)
                Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                
                // Smoothly rotate towards it
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }

            // 2. Apply Movement Force
            if (targetDir.sqrMagnitude > 0.01f)
            {
                rb.AddForce(targetDir * speed, ForceMode.Acceleration);
            }
        }

        private void HandleJump()
        {
            if (_input.jumpInput)
            {
                if (_jumpState.jumpsRemaining > 0)
                {
                    PerformJump();
                }
                _input.jumpInput = false;
            }
        }

        private void PerformJump()
        {
            _jumpState.lastJumpTime = Time.time;
            _jumpState.jumpsRemaining--;
            _jumpState.isGrounded = false;

            // Reset vertical velocity for consistent jump height
            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0;
            rb.linearVelocity = currentVel;

            // Calculate Direction (Standard Up or Wall Bounce)
            Vector3 jumpDir = Vector3.up;

            if (_jumpState.onWall)
            {
                jumpDir = (Vector3.up + (_jumpState.wallNormal * 2.0f)).normalized;
                _jumpState.onWall = false; 
            }

            rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
        }

        #endregion

        #region 5. Physics Corrections

        private void ClampVelocity()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        private void ApplyDrag()
        {
            // Apply extra friction when stopping to prevent "sliding on ice" feel
            if (_jumpState.isGrounded && _input.moveInput.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 5f);
            }
        }

        #endregion

        #region 6. Collision & Ground Detection

        private void OnCollisionEnter(Collision collision) => EvaluateCollision(collision);
        private void OnCollisionStay(Collision collision) => EvaluateCollision(collision);
        private void OnCollisionExit(Collision collision)
        {
            _jumpState.isGrounded = false;
            _jumpState.onWall = false;
        }

        private void EvaluateCollision(Collision collision)
        {
            if (Time.time < _jumpState.lastJumpTime + 0.1f) return;

            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 normal = contact.normal;

                // FLOOR CHECK
                if (Vector3.Dot(normal, Vector3.up) > 0.7f)
                {
                    _jumpState.isGrounded = true;
                    _jumpState.onWall = false;
                    _jumpState.jumpsRemaining = maxJumps;
                    return; 
                }
                
                // WALL CHECK
                if (Vector3.Dot(normal, Vector3.up) < 0.1f)
                {
                    _jumpState.onWall = true;
                    _jumpState.wallNormal = normal;
                    _jumpState.jumpsRemaining = Mathf.Max(_jumpState.jumpsRemaining, 1);
                }
            }
        }

        #endregion
    }
}