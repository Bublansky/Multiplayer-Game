using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Backend.Scripts
{
    public class PlayerInfoController : MonoBehaviour
    {
        public static PlayerInfoController PlayerInfo;

        public int selectedCharacter;

        public GameObject[] allAvailableCharacters;

        private void OnEnable()
        {
            InitializePlayerInfoComponent();
        }

        void Start()
        {
            if (PlayerPrefs.HasKey("SelectedCharacter"))
            {
                selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
            }
            else
            {
                selectedCharacter = 0;
                PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
            }
        }

        void Update()
        {
        
        }
        
        private void InitializePlayerInfoComponent()
        {
            if (PlayerInfoController.PlayerInfo == null)
            {
                PlayerInfoController.PlayerInfo = this;
            }
            else
            {
                if (PlayerInfoController.PlayerInfo != this)
                {
                    Destroy(PlayerInfoController.PlayerInfo.gameObject);
                    PlayerInfoController.PlayerInfo = this;
                }
            }
        }
    }
}
