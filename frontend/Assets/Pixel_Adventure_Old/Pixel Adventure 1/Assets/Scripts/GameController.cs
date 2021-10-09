using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject gameOver;
        
        public static GameController GameInstance;
        
        private int _totalGameScore = 0;

        
        void Start()
        {
            GameInstance = this;
        }

        public void IncreaseScore(int score)
        {
            _totalGameScore += score;
            scoreText.text = _totalGameScore.ToString();
        }

        public void GameOver()
        {
            gameOver.SetActive(true);
        }

        public void RestartGame(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
