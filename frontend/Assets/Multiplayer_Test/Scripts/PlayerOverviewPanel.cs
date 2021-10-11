using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverviewPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerOverviewEntryPrefab;

    private Dictionary<int, GameObject> _playerListEntries;

    #region UNITY

    public void Awake()
    {
        _playerListEntries = new Dictionary<int, GameObject>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var entry = Instantiate(_playerOverviewEntryPrefab);
            var entryTransform = entry.transform;
            entryTransform.SetParent(gameObject.transform);
            entryTransform.localScale = Vector3.one;
            entry.GetComponent<Text>().color = GameSettings.GetColor(player.GetPlayerNumber());
            entry.GetComponent<Text>().text =
                $"{player.NickName}\nScore: {player.GetScore()}\nLives: {GameSettings.PLAYER_MAX_LIVES}";

            _playerListEntries.Add(player.ActorNumber, entry);
        }
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_playerListEntries.TryGetValue(otherPlayer.ActorNumber, out _))
        {
            Destroy(_playerListEntries[otherPlayer.ActorNumber]);
            _playerListEntries.Remove(otherPlayer.ActorNumber);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out var entry))
        {
            entry.GetComponent<Text>().text =
                $"{targetPlayer.NickName}\nScore: {targetPlayer.GetScore()}\n" +
                $"Lives: {targetPlayer.CustomProperties[GameSettings.PLAYER_LIVES]}";
        }
    }

    #endregion
}
