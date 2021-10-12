using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Backend.Scripts
{
    public class LobbyController : MonoBehaviourPunCallbacks
    {
        public static LobbyController GameLobbyController;

        public GameObject enterGameButton;
        public GameObject cancelButton;

        private void Awake()
        {
            // Creates the singleton that lives within the main menu scene.
            GameLobbyController = this;
        }

        void Start()
        {
            // Connects to master photon server.
            PhotonNetwork.ConnectUsingSettings();
        }

        public void OnEnterGameButtonClicked()
        {
            Debug.Log("Entering room...");
            enterGameButton.SetActive(false);
            cancelButton.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
        }
        
        public void OnCancelButtonClicked()
        {
            Debug.Log("Leaving room...");
            enterGameButton.SetActive(true);
            cancelButton.SetActive(false);
            PhotonNetwork.LeaveRoom();
        }

        private void CreateNewRoom()
        {
            var randomRoomId = DateTime.Now.ToFileTime();

            Debug.Log("Creating new room: Room" + randomRoomId);

            RoomOptions roomOps = new RoomOptions()
            {
                IsVisible = true, 
                IsOpen = true, 
                MaxPlayers = 4
            };
            PhotonNetwork.CreateRoom("Room" + randomRoomId, roomOps);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to create room failed, trying again...");
            CreateNewRoom();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Players has connected to the photon master server.");
            PhotonNetwork.AutomaticallySyncScene = true;
            enterGameButton.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Tried to join a random room failed. There are no open game room available");
            CreateNewRoom();
        }
    }
}
