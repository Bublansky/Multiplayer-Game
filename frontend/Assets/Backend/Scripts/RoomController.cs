using System;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Backend.Scripts
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject[] gamePrefabs;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private int multiplayerScene;

        private int currentScene;
        
        public static RoomController GameRoomController;

        private void Awake()
        {
            LoadPrefabsToResourcesCash();
            InitializeRoomComponent();
        }

        private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            currentScene = scene.buildIndex;
            if (currentScene == multiplayerScene)
            {
                CreatePlayer();
            }
        }

        private void CreatePlayer()
        {
            PhotonNetwork.Instantiate(
                "OnlinePlayerReference",
                transform.position,
                quaternion.identity,
                0
            );
        }

        private void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            PhotonNetwork.LoadLevel(multiplayerScene);
        }

        private void LoadPrefabsToResourcesCash()
        {
            DefaultPool poolPrefab = PhotonNetwork.PrefabPool as DefaultPool;
            
            if ((poolPrefab != null) && (gamePrefabs != null))
            {
                foreach(GameObject prefab in gamePrefabs)
                {
                    poolPrefab.ResourceCache.Add(prefab.name, prefab);
                }
            }
        }

        private void InitializeRoomComponent()
        {
            if (RoomController.GameRoomController == null)
            {
                RoomController.GameRoomController = this;
            }
            else
            {
                if (RoomController.GameRoomController != this)
                {
                    Destroy(RoomController.GameRoomController.gameObject);
                    RoomController.GameRoomController = this;
                }
            }
            DontDestroyOnLoad(this.gameObject);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }
        
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("Joined room successfully");
            StartGame();
        }
    }
}
