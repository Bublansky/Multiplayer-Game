using System;
using Photon.Pun;
using UnityEngine;

namespace Backend.Scripts
{
    public class AvatarSetupController : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject selectedCharacter;
        [SerializeField] private int characterSelectedIndex;

        private void Start()
        {
            if (photonView.IsMine)
            {
                photonView.RPC(
                    "RPC_AddCharacter", 
                    RpcTarget.AllBuffered, 
                    PlayerInfoController.PlayerInfo.selectedCharacter
                );
            }
        }

        [PunRPC]
        void RPC_AddCharacter(int character)
        {
            characterSelectedIndex = character;
            selectedCharacter = Instantiate(
                PlayerInfoController.PlayerInfo.allAvailableCharacters[character],
                transform.position,
                transform.rotation,
                transform
            );
        }
    }
}
