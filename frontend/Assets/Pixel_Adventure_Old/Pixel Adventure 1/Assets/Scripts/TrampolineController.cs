using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class TrampolineController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float jumpForce = 20;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("jump");
            }
        }
    }
}
