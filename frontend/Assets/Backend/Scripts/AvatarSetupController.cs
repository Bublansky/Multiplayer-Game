using System;
using Photon.Pun;
using Pixel_Adventure_1.Assets.Scripts;
using UnityEngine;

namespace Backend.Scripts
{
    public class AvatarSetupController : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject selectedCharacter;
        [SerializeField] private int characterSelectedIndex;

        private void Awake()
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
            
            // selectedCharacter.gameObject.name += DateTime.Now.ToFileTime();
            GetComponent<PlayerController>().SetAnimator(selectedCharacter.gameObject.GetComponent<Animator>());
            GetComponent<PlayerController>().SetRigidBody(selectedCharacter.gameObject.GetComponent<Rigidbody2D>());
            // selectedCharacter.gameObject.GetComponent<PlayerController>().SetAnimator();
        }

        public int GetSelectedCharacter()
        {
            return characterSelectedIndex;
        }
    }
}
