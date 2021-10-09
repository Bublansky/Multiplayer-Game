using UnityEngine;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class FlagController : MonoBehaviour
    {
        [SerializeField] private int levelNumber;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameController.GameInstance.RestartGame("Nivel_"+levelNumber.ToString());
            }
        }
    }
}
