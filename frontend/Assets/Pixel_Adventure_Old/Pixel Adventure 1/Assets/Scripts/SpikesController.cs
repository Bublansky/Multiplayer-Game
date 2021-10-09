using UnityEngine;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class SpikesController : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameController.GameInstance.GameOver();
                Destroy(collision.gameObject);
            }
        }
    }
}
