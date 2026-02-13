using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Consider the benefits of refactoring to namespace Movement
namespace Character
{
    
    
    public class PhysicsMovement : MovementController
    {
        
        struct PlayerState
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 linearVelocity;
            public Vector3 angularVelocity;
        }
    
    
        struct InputState
        {
            public Vector2 moveInput;
            public Vector2 rotateInput;
            public bool jumpInput;
        }
    
        struct JumpState
        {
            public bool canJump;
            public bool inAir;
            public bool onWall;
            public Vector3 wallNormal;
            public int jumpCount;
            public float lastJumpTime;
        }
        
        
        [SerializeField] float movementDrag = 0.5f;
        [SerializeField] float mouseSensitivity = 0.1f;
        [SerializeField] private float maxSpeed = 5.0f;
        [SerializeField] private float speed = 10.0f;
        [SerializeField] private float jumpForce = 5.0f;

        
        // Movement Input
        private InputState playerInputState = new InputState()
        {
            moveInput = Vector2.zero,
            rotateInput = Vector2.zero,
            jumpInput = false
        };
        
        // Player State
        private PlayerState playerState = new PlayerState
        {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            linearVelocity = Vector3.zero,
            angularVelocity = Vector3.zero
        };
        
        // Jump State
        private JumpState jumpState = new JumpState
        {
            canJump = true,
            inAir = false,
            onWall = false,
            wallNormal = Vector3.zero,
            jumpCount = 2
        };
        
        protected override void Start()
        {
            base.Start();
            rb.linearDamping = movementDrag;
        }

        public override float GetHorizontalSpeedPercent()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return Mathf.Clamp01(horizontalVelocity.magnitude / maxVelocity);;
        }

        public override void Jump() 
        { 
            playerInputState.jumpInput = true;
        }

        public override void Move(Vector2 input)
        {
            playerInputState.moveInput = input;
        }
        

        protected override void FixedUpdate()
        {
            ApplyMovement();
            ClampVelocity();
            ApplyRotation();
            ApplyJump();
            UpdatePlayerState();
        }
        
        void ApplyMovement()
        {
            ProcessMovementInput();
        }

        void ApplyJump()
        {
            ProcessJumpInput();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                jumpState.canJump = true;
                jumpState.inAir = false;
                jumpState.onWall = false;
                jumpState.jumpCount = 2;
            }
            else
            {
                jumpState.inAir = true;
            }

        }
        
        
        void OnCollisionStay(Collision collision)
        {

            if (Time.time < jumpState.lastJumpTime + 0.2f) return;
            
            ContactPoint contact = collision.GetContact(0);
            Vector3 collisionNormal = contact.normal.normalized;
            Vector3 v = rb.linearVelocity;
            
            
            if (Vector3.Dot(collisionNormal, Vector3.up) < .2)
            {
                var dotProd = Vector3.Dot(v, collisionNormal);
                if (dotProd < 0)
                {
                    var slide = v - (dotProd * collisionNormal);
                    rb.linearVelocity = slide;
                }
                
                jumpState.inAir = false;
                jumpState.onWall = true;
                jumpState.wallNormal = collisionNormal;
                jumpState.canJump = true;
                
            }
            else
            {
                jumpState.onWall = false;
            }

        }

        void OnCollisionExit(Collision collision)
        {
            if(jumpState.onWall)
            {
                jumpState.onWall = false;
            }
        }
        
        void ClampVelocity()
        {
            // Clamp horizontal velocity while preserving vertical (for jumping/falling)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude > maxVelocity)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        void ApplyRotation()
        {
            ProcessRotationInput();
        }
        
        private Vector3 GetOrientedMovement(Vector2 input)
        {
            // Create movement direction vector
            Vector3 inputDir = new Vector3(input.x, 0, input.y);
            
            // Rotate movement direction based on player orientation
            inputDir = transform.TransformDirection(inputDir);
            inputDir.Normalize();
            
            return inputDir;
        }
        
        private void ProcessMovementInput()
        {
            // Create movement direction vector
            Vector3 inputDir = GetOrientedMovement(playerInputState.moveInput);
            
            // Apply movement force to the Rigidbody
            rb.AddForce(inputDir*speed, ForceMode.Acceleration);
            
            // Slow down the player when no input is given (on the ground)
            if (playerInputState.moveInput.magnitude < .1f && !jumpState.inAir)
            {
                // Apply drag when no input is given
                rb.linearVelocity *= (1 - movementDrag);
            }
        }
        
        private void ProcessJumpInput()
        {
            // Handle jumping
            if (playerInputState.jumpInput && jumpState.canJump && jumpState.jumpCount > 0)
            {
                
                jumpState.lastJumpTime = Time.time;
                if (!jumpState.onWall)
                {
                    jumpState.jumpCount--;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    playerInputState.jumpInput = false; // Reset jump input after applying force
                    jumpState.canJump = false; // Prevent further jumps until grounded
                    jumpState.inAir = true;
                }
                else
                {
                    jumpState.jumpCount--;
                    rb.AddForce((Vector3.up + (jumpState.wallNormal*2.0f)) * jumpForce, ForceMode.Impulse);
                    playerInputState.jumpInput = false; // Reset jump input after applying force
                    jumpState.canJump = false; // Prevent further jumps until grounded
                    jumpState.inAir = true;
                    jumpState.onWall = false;
                }
            }
        }

        private void ProcessRotationInput()
        {
            // Rotate left/right by rotating the player object
            transform.Rotate(0.0f, playerInputState.rotateInput.x, 0.0f);
            
            // Look up/down by rotating the camera child object
            Transform cameraTransform = transform.GetChild(0);
            cameraTransform.Rotate(-playerInputState.rotateInput.y, 0.0f, 0.0f);
            
            // Clamp camera rotation to avoid flipping
            Vector3 cameraEuler = cameraTransform.localEulerAngles;
            if (cameraEuler.x > 180) cameraEuler.x -= 360; // Convert to -180 to 180 range
            cameraEuler.x = Mathf.Clamp(cameraEuler.x, -80, 80);
            cameraTransform.localEulerAngles = cameraEuler;
            
            playerInputState.rotateInput = Vector2.zero; // Reset rotation input after applying
            rb.angularVelocity = Vector3.zero;
        }

        private void UpdatePlayerState()
        {
            playerState.position = transform.position;
            playerState.rotation = transform.rotation;
            playerState.linearVelocity = rb.linearVelocity;
            playerState.angularVelocity = rb.angularVelocity;
        }
    }
}
