using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CountdownTimer : MonoBehaviourPunCallbacks
{
    [Header("Reference to a Text component for visualizing the countdown")]
    [SerializeField] private Text _text;

    private bool _isTimerRunning;
    private int _startTime;

    /// <summary>
    ///     OnCountdownTimerHasExpired delegate.
    /// </summary>
    public delegate void CountdownTimerHasExpired();
    public const string CountdownStartTime = "StartTime";
    [Header("Countdown time in seconds")] 
    public float Countdown = 5.0f;

    /// <summary>
    ///     Called when the timer has expired.
    /// </summary>
    public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;

    public override void OnEnable()
    {
        Debug.Log("OnEnable CountdownTimer");
        base.OnEnable();

        // the starttime may already be in the props. look it up.
        Initialize();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Debug.Log("OnDisable CountdownTimer");
    }

    public void Update()
    {
        if (!_isTimerRunning) return;

        var countdown = TimeRemaining();
        _text.text = $"Game starts in {countdown:n0} seconds";

        if (countdown > 0.0f) return;

        OnTimerEnds();
    }

    private void OnTimerRuns()
    {
        _isTimerRunning = true;
        enabled = true;
    }

    private void OnTimerEnds()
    {
        _isTimerRunning = false;
        enabled = false;

        Debug.Log("Emptying info text.", _text);
        _text.text = string.Empty;

        OnCountdownTimerHasExpired?.Invoke();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
        Initialize();
    }

    private void Initialize()
    {
        if (TryGetStartTime(out var propStartTime))
        {
            _startTime = propStartTime;
            Debug.Log("Initialize sets StartTime " + _startTime + " server time now: " 
                      + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());

            _isTimerRunning = TimeRemaining() > 0;

            if (_isTimerRunning)
            {
                OnTimerRuns();
            }
            else
            {
                OnTimerEnds();
            }
        }
    }

    private float TimeRemaining()
    {
        var timer = PhotonNetwork.ServerTimestamp - _startTime;
        return Countdown - timer / 1000f;
    }

    public static bool TryGetStartTime(out int startTimestamp)
    {
        startTimestamp = PhotonNetwork.ServerTimestamp;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out var startTimeFromProps))
        {
            startTimestamp = (int)startTimeFromProps;
            return true;
        }

        return false;
    }

    public static void SetStartTime()
    {
        var wasSet = TryGetStartTime(out _);

        var props = new Hashtable
        {
            {CountdownStartTime, PhotonNetwork.ServerTimestamp}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Debug.Log("Set Custom Props for Time: "+ props.ToStringFull() + " wasSet: "+wasSet);
    }
}