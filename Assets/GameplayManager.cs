using UnityEngine;
using TMPro;

namespace Laliphat
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [Header("Gameplay Settings")]
        [SerializeField] private float totalTime = 30f;
        [SerializeField] private int targetWinningScore = 10000;
        [SerializeField] private float fallThresholdY = -8f;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI hudText;

        [Header("Player Reference")]
        [SerializeField] private Transform playerTransform;

        private float timeRemaining;
        private int currentScore;
        private bool isGameOver;
        private SceneLoader sceneLoader;

        public int CurrentScore => currentScore;
        public float TimeRemaining => timeRemaining;

        private void Awake()
        {
            Instance = this;
            timeRemaining = totalTime;
            currentScore = 0;
            isGameOver = false;

            sceneLoader = GetComponent<SceneLoader>();
            if (sceneLoader == null)
            {
                sceneLoader = FindFirstObjectByType<SceneLoader>();
            }
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            if (hudText == null)
            {
                var hudObj = GameObject.Find("HUD_Overlay");
                if (hudObj != null) hudText = hudObj.GetComponent<TextMeshProUGUI>();
            }

            UpdateHUD();
        }

        private void Update()
        {
            if (isGameOver) return;

            // Timer countdown
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;

            UpdateHUD();

            // Fall check
            if (playerTransform != null && playerTransform.position.y < fallThresholdY)
            {
                TriggerGameOver();
                return;
            }

            // Time attack end check
            if (timeRemaining <= 0f)
            {
                if (currentScore >= targetWinningScore)
                {
                    TriggerWin();
                }
                else
                {
                    TriggerGameOver();
                }
            }
        }

        public void AddScore(int amount)
        {
            if (isGameOver) return;
            currentScore += amount;
            if (currentScore < 0) currentScore = 0;
            UpdateHUD();
        }

        public void AddTime(float seconds)
        {
            if (isGameOver) return;
            timeRemaining += seconds;
            if (timeRemaining < 0f) timeRemaining = 0f;
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            if (hudText != null)
            {
                hudText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s\nScore: {currentScore}";
            }
        }

        private void TriggerWin()
        {
            if (isGameOver) return;
            isGameOver = true;
            GameSession.FinalScore = currentScore;
            if (sceneLoader != null)
            {
                sceneLoader.GoToWin(currentScore);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Win");
            }
        }

        private void TriggerGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            GameSession.FinalScore = currentScore;
            if (sceneLoader != null)
            {
                sceneLoader.GoToGameOver(currentScore);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }
    }
}
