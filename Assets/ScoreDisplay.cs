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
                if (tmp.text.Contains("score :") || tmp.text.Contains("Score :") || tmp.text.Contains("99999"))
                {
                    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    tmp.text = $"{sceneName}\n\n\nScore : {GameSession.FinalScore}";
                }
                else
                {
                    tmp.text = $"{prefix}{GameSession.FinalScore}";
                }
            }
        }
    }
}
