using UnityEngine;

namespace Laliphat
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -6f);
        [SerializeField] private float smooth = 8f;

        private void Start()
        {
            FindTarget();
        }

        private void FindTarget()
        {
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                FindTarget();
                if (target == null) return;
            }
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 1f);
        }
    }
}
