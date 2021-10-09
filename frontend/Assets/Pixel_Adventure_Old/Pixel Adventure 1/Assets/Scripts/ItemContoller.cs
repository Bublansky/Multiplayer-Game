using UnityEngine;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class ItemContoller : MonoBehaviour
    {
        [SerializeField] private GameObject itemCollectedGameObject;
        [SerializeField] private int score;
    
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _polygonCollider2D;
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                _polygonCollider2D.enabled = false;
                _spriteRenderer.enabled = false;
                
                GameController.GameInstance.IncreaseScore(score);
                
                itemCollectedGameObject.SetActive(true);
                Destroy(gameObject, 0.25f);
            }
        }
    }
}
