using UnityEngine;

namespace Laliphat
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 12f;

        private Rigidbody rb;
        private Transform cam;
        private Vector3 moveDir;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            cam = Camera.main.transform;
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // เดินตามทิศที่กล้องมอง (ตัด แกน Y ออก)
            Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            moveDir = (forward * v + right * h).normalized;
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
