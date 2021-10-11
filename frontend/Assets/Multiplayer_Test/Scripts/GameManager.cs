using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance = null;

    [SerializeField] private Text _infoText;

    #region UNITY

    public void Awake()
    {
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
    }

    public void Start()
    {
        var props = new Hashtable
        {
            {GameSettings.PLAYER_LOADED_LEVEL, true}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
    }

    #endregion

    #region COROUTINES

    private IEnumerator EndOfGame(string winner, int score)
    {
        var timer = 5.0f;

        while (timer > 0.0f)
        {
            _infoText.text = $"Player {winner} won with {score} points.\n\n\n" +
                            $"Returning to login screen in {timer:n2} seconds.";

            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }

        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            //StartCoroutine(SpawnAsteroid());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckEndOfGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameSettings.PLAYER_LIVES))
        {
            CheckEndOfGame();
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // if there was no countdown yet, the master client (this one) waits
        // until everyone loaded the level and sets a timer start
        var startTimeIsSet = CountdownTimer.TryGetStartTime(out _);

        if (changedProps.ContainsKey(GameSettings.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                if (!startTimeIsSet)
                {
                    CountdownTimer.SetStartTime();
                }
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ", _infoText);
                _infoText.text = "Waiting for other players...";
            }
        }
    }

    #endregion

    
    // called by OnCountdownTimerIsExpired() when the timer ended
    private void StartGame()
    {
        Debug.Log("StartGame!");

        // on rejoin, we have to figure out if the spaceship exists or not
        // if this is a rejoin (the ship is already network instantiated and will be setup via event)
        // we don't need to call PN.Instantiate

        //-5.5 to 5.5
        const float roomWidth = 11f, roomStart = -roomWidth/2;

        var indexOffset = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        var clampedIndex = Mathf.Clamp(indexOffset, 1, int.MaxValue);
        var positionStepX = roomWidth / clampedIndex;
        var positionX = roomStart + positionStepX * PhotonNetwork.LocalPlayer.GetPlayerNumber();
        var position = new Vector3(positionX, 2, 0);

        // avoid this call on rejoin (ship was network instantiated before)
        PhotonNetwork.Instantiate("Player_Base", position, Quaternion.identity, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            //StartCoroutine(SpawnAsteroid());
        }
    }

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(GameSettings.PLAYER_LOADED_LEVEL, out var playerLoadedLevel))
            {
                if ((bool) playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    private void CheckEndOfGame()
    {
        var allDestroyed = true;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(GameSettings.PLAYER_LIVES, out var lives))
            {
                if ((int) lives > 0)
                {
                    allDestroyed = false;
                    break;
                }
            }
        }

        if (allDestroyed)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StopAllCoroutines();
            }

            var winner = "";
            var score = -1;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.GetScore() > score)
                {
                    winner = player.NickName;
                    score = player.GetScore();
                }
            }

            StartCoroutine(EndOfGame(winner, score));
        }
    }

    private void OnCountdownTimerIsExpired()
    {
        StartGame();
    }
}