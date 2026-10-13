using UnityEngine;
using TMPro;

namespace Laliphat
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Configuration")]
        [SerializeField] private float interactRadius = 2.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private LayerMask interactableLayers = ~0;

        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI promptText;

        private StageItem currentTarget;

        private void Start()
        {
            if (promptText == null)
            {
                var promptObj = GameObject.Find("Interaction_Prompt");
                if (promptObj != null)
                {
                    promptText = promptObj.GetComponent<TextMeshProUGUI>();
                }
            }

            HidePrompt();
        }

        private void Update()
        {
            DetectInteractable();

            if (currentTarget != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    InteractWithTarget();
                }
            }
        }

        private void DetectInteractable()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayers);
            StageItem closest = null;
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var item = hit.GetComponentInParent<StageItem>();
                if (item != null)
                {
                    float dist = Vector3.Distance(transform.position, item.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = item;
                    }
                }
            }

            if (closest != null)
            {
                currentTarget = closest;
                ShowPrompt($"Press [{interactKey}] to Interact");
            }
            else
            {
                currentTarget = null;
                HidePrompt();
            }
        }

        private void InteractWithTarget()
        {
            if (currentTarget != null)
            {
                currentTarget.CollectItem();
                currentTarget = null;
                HidePrompt();
            }
        }

        private void ShowPrompt(string message)
        {
            if (promptText != null)
            {
                promptText.text = message;
                promptText.gameObject.SetActive(true);
            }
        }

        private void HidePrompt()
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }
}
