using UnityEngine;

namespace Character {
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] protected float jumpForce = 3f;
        [SerializeField] protected float acceleration = 20f;
        [SerializeField] protected float maxVelocity = 5f;
        protected Rigidbody rb;
        protected Vector2 moveInput;
        protected bool jumpInput; //protected lets physics controller use this

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public virtual void Move(Vector2 lateralInput)
        {
            moveInput = lateralInput;
        }

        public void Stop()
        {
            rb.linearVelocity = Vector3.zero;
            moveInput = Vector2.zero;
        }

        public virtual void Jump() 
        { 
            jumpInput = true;
        }

        public virtual Vector2 GetMovementVector()
        {
            return Vector2.ClampMagnitude(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z) / maxVelocity, 1f);
        }

        protected virtual void FixedUpdate()
        {
            SimpleMovement();
        }

        void SimpleMovement()
        {
            Vector3 movement = Vector3.zero;
            movement += transform.right * moveInput.x;
            movement += transform.forward * moveInput.y;
            movement.Normalize();
            movement *= Time.deltaTime * acceleration;
            //rb.MovePosition(rb.position + movement);
        }
    }
}