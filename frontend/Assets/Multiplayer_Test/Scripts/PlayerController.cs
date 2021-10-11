using System;
using System.Collections;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _myRigidbody;
    [SerializeField] private Collider2D _myCollider;
    [SerializeField] private Renderer _myRenderer;
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private Transform _myTransform;
    [SerializeField] private PlayerFeet _playerFeet;
    [SerializeField] private PhotonView _photonView;
    
    [Header("Parameters")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _jumpForce = 10.0f;
    [SerializeField] private bool _isInputEnabled;

    private bool _canJump = true;
    private int _jumpCount;
    
    private bool _controllable = true;
    private static readonly int JumpTrigger = Animator.StringToHash("jump");
    private static readonly int WalkTrigger = Animator.StringToHash("walk");

    [SerializeField] private bool IsJumping => _myRigidbody.velocity.y < 0;

    public Action OnDying;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        _myRenderer.material.color = GameSettings.GetColor(photonView.Owner.GetPlayerNumber());
        
        _playerFeet.OnJumpPlayerHead += HandleJumpPlayerHead;
        
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!photonView.AmOwner || !_controllable)
        {
            return;
        }

        // we don't want the master client to apply input to remote ships while the remote player is inactive
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        if (photonView.IsMine)
        {
            if (_controllable)
            {
                Move();
                if(_canJump)
                    Jump();
            }
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

    #region COROUTINES

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(GameSettings.PLAYER_RESPAWN_TIME);

        photonView.RPC("RespawnSpaceship", RpcTarget.AllViaServer);
    }

    #endregion

    private void HandleJumpPlayerHead(PlayerFeet.JumpPlayerHeadArgs args)
    {
        Debug.Log($"HandleJumpPlayerHead");
        if (IsJumping)
        {
            if (photonView.IsMine)
            {
                var playerKilledGameObject = args.Collider.gameObject; 
                Debug.Log($"{gameObject.name} killed {playerKilledGameObject.name}");

                playerKilledGameObject.GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
            }
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
            _myRigidbody.AddForce(new Vector2(0.0f, _jumpForce), ForceMode2D.Impulse);
            _myAnimator.SetBool(JumpTrigger, true);
            _jumpCount++;
        }
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        transform.position += movement * (Time.deltaTime * _speed);
    }
    
    #region PUN CALLBACKS

    [PunRPC]
    [UsedImplicitly]
    public void DestroyPlayer()
    {
        Debug.Log($"{gameObject.name} destroyed");
        _myCollider.enabled = false;
        _myRenderer.enabled = false;
        _myRigidbody.isKinematic = true;

        _controllable = false;

        if (photonView.IsMine)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameSettings.PLAYER_LIVES, out var lives))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(
                    new Hashtable
                    {
                        {
                            GameSettings.PLAYER_LIVES, 
                            (int) lives <= 1 ? 0 : (int) lives - 1
                        }
                    });

                if ((int) lives > 1)
                {
                    StartCoroutine(nameof(WaitForRespawn));
                }
            }
        }
    }

    [PunRPC]
    [UsedImplicitly]
    public void RespawnSpaceship()
    {
        _myCollider.enabled = true;
        _myRenderer.enabled = true;
        _myRigidbody.velocity = Vector2.zero;
        _myRigidbody.angularVelocity = 0;
        _myRigidbody.Sleep();
        _myRigidbody.isKinematic = false;

        _controllable = true;
    }

    #endregion
}