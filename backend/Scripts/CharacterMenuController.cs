using UnityEngine;

namespace Backend.Scripts
{
    public class CharacterMenuController : MonoBehaviour
    {
        public void OnCharacterPick(int selectedCharacter)
        {
            if (PlayerInfoController.PlayerInfo != null)
            {
                PlayerInfoController.PlayerInfo.selectedCharacter = selectedCharacter;
                PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
            }
        }
    }
}
