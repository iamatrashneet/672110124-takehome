using UnityEngine;
using TMPro;

namespace Laliphat
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private string prefix = "Score : ";

        private void Start()
        {
            var tmp = GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = $"{prefix}{GameSession.FinalScore}";
            }
        }
    }
}
