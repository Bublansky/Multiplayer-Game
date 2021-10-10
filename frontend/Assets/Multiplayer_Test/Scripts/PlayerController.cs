using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _myRigidbody2D;
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private Transform _myTransform;
    [SerializeField] private PlayerFeet _playerFeet;

    [Header("Parameters")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _jumpForce = 10.0f;
    [SerializeField] private bool _isInputEnabled;

    private bool _canJump = true;
    private int _jumpCount;
    private static readonly int JumpTrigger = Animator.StringToHash("jump");
    private static readonly int WalkTrigger = Animator.StringToHash("walk");

    private bool IsJumping => !_canJump && _jumpCount >= 2; 

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        _playerFeet.OnJumpPlayerHead += HandleJumpPlayerHead;
    }

    private void Update()
    {
        if (_isInputEnabled)
        {
            Move();
            if(_canJump)
                Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
             _canJump = true;
            _jumpCount = 0;
            _myAnimator.SetBool(JumpTrigger, false);
        }
    }

    #endregion

    private void HandleJumpPlayerHead(PlayerFeet.JumpPlayerHeadArgs args)
    {
        if (IsJumping)
        {
            Debug.Log("jump player head");
        }
    }

    private void Move()
    {
        var horizontalMovement = Input.GetAxis("Horizontal");
        
        if (horizontalMovement != 0)
        {
            transform.eulerAngles = horizontalMovement > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

            var movement = new Vector3(horizontalMovement, 0.0f, 0.0f);
            _myTransform.position += movement * (Time.deltaTime * _speed);
            _myAnimator.SetBool(WalkTrigger, true);    
        }
        else
        {
            _myAnimator.SetBool(WalkTrigger, false);
        }
    }

    private void Jump()
    {
        if (_jumpCount >= 2)
            _canJump = false;
        
        if (Input.GetButtonDown("Jump"))
        {
            _myRigidbody2D.AddForce(new Vector2(0.0f, _jumpForce), ForceMode2D.Impulse);
            _myAnimator.SetBool(JumpTrigger, true);
            _jumpCount++;
        }
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        transform.position += movement * (Time.deltaTime * _speed);
    }
}