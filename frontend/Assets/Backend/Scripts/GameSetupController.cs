using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Backend.Scripts
{
    public class GameSetupController : MonoBehaviour
    {
        public static GameSetupController GameSetup;

        public Transform[] playersSpawnPoints;
        
        private void OnEnable()
        {
            if (GameSetupController.GameSetup == null)
            {
                GameSetupController.GameSetup = this;
            }
        }
    }
}
