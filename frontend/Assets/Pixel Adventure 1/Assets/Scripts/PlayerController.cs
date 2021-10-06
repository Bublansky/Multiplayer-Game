using System;
using UnityEngine;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float jumpForce = 10.0f;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private bool _canJump = true;
        private int _jumpCount = 0;
    
        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            Move();
            if(_canJump)
                Jump();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                 _canJump = true;
                _jumpCount = 0;
                _animator.SetBool("jump", false);
            }
        }

        private void Move()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            
            if (horizontalMovement != 0)
            {
                transform.eulerAngles = horizontalMovement > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
                
                Vector3 movement = new Vector3(horizontalMovement, 0.0f, 0.0f);
                transform.position += movement * Time.deltaTime * speed;
                _animator.SetBool("walk", true);    
            }
            else
            {
                _animator.SetBool("walk", false);
            }
        }
    
        private void Jump()
        {
            if (_jumpCount >= 2)
                _canJump = false;
            
            if (Input.GetButtonDown("Jump"))
            {
                _rigidbody2D.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                _animator.SetBool("jump", true);
                _jumpCount++;
            }
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
            transform.position += movement * Time.deltaTime * speed;

        }
    }
}
