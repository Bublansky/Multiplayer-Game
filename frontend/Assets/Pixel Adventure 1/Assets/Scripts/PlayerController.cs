using System;
using Backend.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float jumpForce = 10.0f;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private PhotonView photonView;

        private bool _canJump = true;
        private int _jumpCount = 0;
    
        void Start()
        {
            // _rigidbody2D = GetComponent<Rigidbody2D>();
            // _animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                // Debug.Log(photonView.IsMine + gameObject.name);
                photonView.RPC(
                    "Move", 
                    RpcTarget.AllBuffered, 
                    PlayerInfoController.PlayerInfo.selectedCharacter,
                    Time.deltaTime,
                    Input.GetAxis("Horizontal")
                );
                // Move();
                // if(_canJump)
                photonView.RPC(
                    "Jump", 
                    RpcTarget.AllBuffered, 
                    PlayerInfoController.PlayerInfo.selectedCharacter,
                    Time.deltaTime,
                    Input.GetButtonDown("Jump"),
                    Input.GetAxis("Horizontal")
                );
                    // Jump();
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("entrouuuuuuuuu no colision");
            if (collision.gameObject.CompareTag("Ground"))
            {
                Debug.Log("entrou no if tambem");
                _canJump = true;
                _jumpCount = 0;
                animator.SetBool("jump", false);
            }
        }

        [PunRPC]
        private void Move(int objectIndex, float time, float horizontalMovement)
        {
            Debug.Log("name --> " + gameObject.name + "objectindex --> " + objectIndex + " ||   selectedcharacter --> " + GetComponent<AvatarSetupController>().GetSelectedCharacter());
            if (objectIndex == GetComponent<AvatarSetupController>().GetSelectedCharacter())
            {
                // float horizontalMovement = Input.GetAxis("Horizontal");
                
                if (horizontalMovement != 0)
                {
                    transform.eulerAngles = horizontalMovement > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
                    
                    Vector3 movement = new Vector3(horizontalMovement, 0.0f, 0.0f);
                    transform.position += movement * time * speed;
                    animator.SetBool("walk", true);    
                }
                else
                {
                    animator.SetBool("walk", false);
                }
            }
        }
    
        [PunRPC]
        private void Jump(int objectIndex, float time, bool isJumping, float horizontalMovement)
        {
            if (objectIndex == GetComponent<AvatarSetupController>().GetSelectedCharacter())
            {
                if (_jumpCount >= 2)
                    _canJump = false;
                
                if (isJumping)
                {
                    rigidbody2D.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                    animator.SetBool("jump", true);
                    _jumpCount++;
                }
                // Vector3 movement = new Vector3(horizontalMovement, 0.0f, 0.0f);
                // transform.position += movement * time * speed;
            }

        }

        public void SetUniqueName(string key)
        {
            gameObject.name = key;
        }
        
        public void SetAnimator(Animator anim)
        {
            animator = anim;
        }
        
        public void SetRigidBody(Rigidbody2D rigid)
        {
            rigidbody2D = rigid;
        }
    }
}
