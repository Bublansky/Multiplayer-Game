using System.Collections;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerTankController : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private Rigidbody _myRigidbody;
    [SerializeField] private Collider _myCollider;
    [SerializeField] private Renderer _myRenderer;
    [SerializeField] private Transform _myTransform;
    [SerializeField] private Camera _myCamera;

    [Header("Parameters")]
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _rotationSpeed = 30f;
    
    private bool _controllable = true;

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        //_myRenderer.material.color = GameSettings.GetColor(photonView.Owner.GetPlayerNumber());
        
        //_playerFeet.OnJumpPlayerHead += HandleJumpPlayerHead;
        
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization
        // , thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
        if (photonView.IsMine)
        {
            _myCamera.enabled = true;
            IncreaseScore(1);
        }
    }

    private void Update()
    {
        Move();
        Shoot();

        return;

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
                //Move();
            }
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

    private void Move()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");

        var verticalMovement = _myTransform.position + _myTransform.forward *
            (Time.deltaTime * _movementSpeed * verticalAxis);

        var angle = Time.deltaTime * _rotationSpeed * horizontalAxis;

        var rotation = (angle + _myTransform.rotation.eulerAngles.y) * Vector3.up;
        
        _myRigidbody.MovePosition(verticalMovement);
        _myRigidbody.MoveRotation(Quaternion.Euler(rotation));
    }

    private void Shoot()
    {
        
        //_myTransform.LookAt(_lookAt);
        return;
        var cameraRay = _myCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out var hit, 1000))
        {
            var distance = Vector3.Distance(_myTransform.position, hit.point);

            if (distance > 1)
            {
                //_myTransform.LookAt(hit.point);
            }
        }
    }

    private void IncreaseScore(int amount)
    {
        Debug.Log($"IncreaseScore {amount}");
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameSettings.PLAYER_SCORE, out var score))
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new Hashtable
                {
                    {
                        GameSettings.PLAYER_SCORE
                        , (int) score + amount
                    }
                });
        }
    }

    #region PUN CALLBACKS

    [PunRPC]
    public void DestroyPlayer()
    {
        //_myCollider.enabled = false;
        //_myRenderer.enabled = false;
        _myRigidbody.isKinematic = true;

        _controllable = false;

        if (photonView.IsMine)
        {
            StartCoroutine(nameof(WaitForRespawn));
        }
    }

    [PunRPC]
    public void RespawnTank()
    {
        //_myCollider.enabled = true;
        //_myRenderer.enabled = true;
        _myRigidbody.velocity = Vector2.zero;
        _myRigidbody.angularVelocity = Vector3.zero;
        _myRigidbody.Sleep();
        _myRigidbody.isKinematic = false;

        _controllable = true;
    }

    #endregion
}