using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TankGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _infoText;
    [SerializeField] private Transform[] _playerStartPositions;
    [SerializeField] private float _timePerRoundInMinutes = 5; //in seconds
    [SerializeField] private Camera _mainCamera;

    [ReadOnly]
    [SerializeField] private float _remainingTime;

    private float TimePerRoundInSeconds => _timePerRoundInMinutes * 60;

    #region UNITY

    public override void OnEnable()
    {
        base.OnEnable();
        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

        Application.targetFrameRate = 30;
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

    private IEnumerator EndOfGame(string winners)
    {
        var timer = 5.0f;

        while (timer > 0.0f)
        {
            _infoText.text = $"{winners} won.\n\n\n" +
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckEndOfGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameSettings.PLAYER_SCORE))
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
        // on rejoin, we have to figure out if the spaceship exists or not
        // if this is a rejoin (the ship is already network instantiated and will be setup via event)
        // we don't need to call PN.Instantiate

        var myPlayerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        
        Debug.Log("instantiate player");
        // avoid this call on rejoin (ship was network instantiated before)
        
        
        PhotonNetwork.Instantiate("Player_Tank", _playerStartPositions[myPlayerNumber].position
            , Quaternion.identity);

       
        StartCoroutine(CheckTimeLimit());

        _mainCamera.enabled = false;
    }

    private IEnumerator CheckTimeLimit()
    {
        Debug.Log("start coroutine CheckTimeLimit");
        _remainingTime = TimePerRoundInSeconds;

        while (true)
        {
            if (_remainingTime <= 0)
            {
                _remainingTime = 0;
                
                if (PhotonNetwork.IsMasterClient)
                {
                    CheckEndOfGame();
                }
                
                yield break;
            }

            _infoText.text = ((int) _remainingTime).ToString();
            _remainingTime -= Time.deltaTime;
            yield return null;
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
        Debug.Log("CheckEndOfGame");
        var winnersText = "";
        var winners = new List<string>();
        var winnerScore = int.MinValue;
        

        Debug.Log($"PhotonNetwork.PlayerList {PhotonNetwork.PlayerList}" +
                  $", length {PhotonNetwork.PlayerList.Length}");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"analysing {player.NickName}");
            if (player.CustomProperties.TryGetValue(GameSettings.PLAYER_SCORE, out var score))
            {
                var scoreInt = (int) score;
                Debug.Log($"score {scoreInt}");

                if (scoreInt > winnerScore)
                {
                    Debug.Log($"current winner: ");
                    winnerScore = scoreInt;
                    winners.Clear();
                    winners.Add(player.NickName);
                }
                else if (scoreInt == winnerScore)
                {
                    winners.Add(player.NickName);
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StopAllCoroutines();
        }

        foreach (var winner in winners)
        {
            winnersText += $"{winner}  ";
        }
        
        StartCoroutine(EndOfGame(winnersText));
    }

    private void OnCountdownTimerIsExpired()
    {
        StartGame();
    }
}
