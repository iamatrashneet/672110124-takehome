using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: เปลี่ยน YourName เป็นชื่อภาษาอังกฤษของตัวเอง (ข้อกำหนดข้อ 7)
namespace YourName
{
    // เก็บข้อมูลที่ต้องส่งข้ามฉาก (static จึงไม่หายตอนเปลี่ยนฉาก)
    public static class GameSession
    {
        public static string LastStage = "Stage1";
        public static int FinalScore = 0;
    }

    public class SceneLoader : MonoBehaviour
    {
        // ปุ่มทั่วไป: ไปฉากตามชื่อ (เช่น MainMenu, StageSelection, Credits)
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        // ปุ่ม Stage 1 / Stage 2: จำไว้ว่าเล่นด่านไหน เพื่อใช้กับ Try Again
        public void LoadStage(string stageSceneName)
        {
            GameSession.LastStage = stageSceneName;
            SceneManager.LoadScene(stageSceneName);
        }

        // ปุ่ม Try Again: เล่นด่านเดิมซ้ำ
        public void TryAgain()
        {
            SceneManager.LoadScene(GameSession.LastStage);
        }

        // ใช้กับ Options ถ้าทำเป็น Additive (ซ้อนบน Main Menu)
        public void OpenAdditive(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void CloseAdditive(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        // เรียกจากระบบเกม (ภายหลัง) เมื่อชนะ/แพ้ พร้อมส่งคะแนน
        public void GoToWin(int score)
        {
            GameSession.FinalScore = score;
            SceneManager.LoadScene("Win");
        }

        public void GoToGameOver(int score)
        {
            GameSession.FinalScore = score;
            SceneManager.LoadScene("GameOver");
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
