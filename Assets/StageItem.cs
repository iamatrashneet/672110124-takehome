using UnityEngine;

namespace Laliphat
{
    public enum ItemType
    {
        ScoreBonus,    // +500 points
        ScorePenalty,  // -500 points
        TimeBonus,     // +3 seconds
        TimePenalty    // -5 seconds
    }

    public class StageItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] private ItemType itemType = ItemType.ScoreBonus;
        [SerializeField] private int scoreValue = 500;
        [SerializeField] private float timeValue = 3f;

        [Header("Animation")]
        [SerializeField] private float rotateSpeed = 90f;
        [SerializeField] private float bobSpeed = 3f;
        [SerializeField] private float bobHeight = 0.25f;

        [Header("Interaction Settings")]
        [SerializeField] private bool requireInteraction = false;

        private Vector3 startPosition;
        private bool isCollected = false;

        public ItemType Type => itemType;
        public bool RequireInteraction
        {
            get => requireInteraction;
            set => requireInteraction = value;
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            // Continuous rotation
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

            // Floating bobbing motion
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;
            if (requireInteraction) return; // In Stage 2, collection requires explicit interaction!

            if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
            {
                CollectItem();
            }
        }

        public void CollectItem()
        {
            if (isCollected) return;
            isCollected = true;

            // Apply gameplay changes
            if (GameplayManager.Instance != null)
            {
                switch (itemType)
                {
                    case ItemType.ScoreBonus:
                        GameplayManager.Instance.AddScore(scoreValue > 0 ? scoreValue : 500);
                        break;
                    case ItemType.ScorePenalty:
                        GameplayManager.Instance.AddScore(scoreValue < 0 ? scoreValue : -500);
                        break;
                    case ItemType.TimeBonus:
                        GameplayManager.Instance.AddTime(timeValue > 0 ? timeValue : 3f);
                        break;
                    case ItemType.TimePenalty:
                        GameplayManager.Instance.AddTime(timeValue < 0 ? timeValue : -5f);
                        break;
                }
            }

            // Audio feedback hook
            if (AudioManager.Instance != null)
            {
                if (itemType == ItemType.ScoreBonus || itemType == ItemType.TimeBonus)
                {
                    AudioManager.Instance.PlayItemSFX();
                }
                else
                {
                    AudioManager.Instance.PlayObstacleSFX();
                }
            }

            Destroy(gameObject);
        }
    }
}
