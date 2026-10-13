using UnityEngine;

namespace Laliphat
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private float jumpForce = 5f;

        private Rigidbody rb;
        private Transform cam;
        private Vector3 moveDir;
        private bool isGrounded = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            if (Camera.main != null) cam = Camera.main.transform;
        }

        private void Update()
        {
            if (cam == null)
            {
                if (Camera.main != null) cam = Camera.main.transform;
                else return;
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // เดินตามทิศที่กล้องมอง (ตัด แกน Y ออก)
            Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            moveDir = (forward * v + right * h).normalized;

            // Ground check via raycast
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private void FixedUpdate()
        {
            Vector3 vel = moveDir * moveSpeed;
            vel.y = rb.linearVelocity.y; // คงแรงโน้มถ่วงไว้
            rb.linearVelocity = vel;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
            }
        }
    }
}
