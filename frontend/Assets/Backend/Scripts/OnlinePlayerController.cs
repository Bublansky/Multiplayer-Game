using System;
using Photon.Pun;
using Pixel_Adventure_1.Assets.Scripts;
using Unity.Mathematics;
using UnityEngine;

namespace Backend.Scripts
{
    public class OnlinePlayerController : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject mySelectedAvatar;
        
        void Start()
        {
            if (photonView.IsMine)
            {
                int index = PhotonNetwork.CurrentRoom.PlayerCount - 1;
                Transform[] positions = GameSetupController.GameSetup.playersSpawnPoints;

                mySelectedAvatar = PhotonNetwork.Instantiate(
                    "PlayerCharacter",
                    positions[index].position,
                    positions[index].rotation,
                    0
                );
            }
        }
    }
}
