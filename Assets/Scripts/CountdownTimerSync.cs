using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using static CountdownTimerSync;

public class CountdownTimerSync : MonoBehaviourPunCallbacks
{
    public static CountdownTimerSync instance;
    public delegate void CountdownTimerHasExpired();
    public delegate void CountdownTimerHasStarted();

    public const string CountdownStartTime = "StartTime";

    [Header("Countdown time in seconds")]
    [SerializeField] float Countdown = 5.0f;

    private bool isTimerRunning;

    private int startTime;

    [Header("Reference to a Text component for visualizing the countdown")]
    [SerializeField] TMP_Text Text;

    public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;
    public static event CountdownTimerHasStarted OnCountdownTimerHasStarted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }
    public void Start()
    {
        if (Text == null) 
            Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }


    public void Update()
    {
        if (!isTimerRunning) 
            return;
        float countdown = TimeRemaining();
        Text.text = string.Format(countdown.ToString("n0"));
        Debug.Log(Text.text);

        if (countdown > 0.0f) 
            return;

        OnTimerEnds();
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        Initialize();
    }

    private void OnTimerRuns()
    {
        OnCountdownTimerHasStarted?.Invoke();
        isTimerRunning = true;
        enabled = true;
    }

    private void OnTimerEnds()
    {
        isTimerRunning = false;
        enabled = false;
        Text.text = string.Empty;

        OnCountdownTimerHasExpired?.Invoke();
    }

    public void Initialize()
    {
        int propStartTime;
        if (TryGetStartTime(out propStartTime))
        {
            startTime = propStartTime;
            isTimerRunning = TimeRemaining() > 0;

            if (isTimerRunning)
                OnTimerRuns();
            else
                OnTimerEnds();
        }
    }


    private float TimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - startTime;
        return Countdown - timer / 1000f;
    }


    public static bool TryGetStartTime(out int startTimestamp)
    {
        startTimestamp = PhotonNetwork.ServerTimestamp;

        object startTimeFromProps;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
        {
            Debug.Log("There is customprop");
            startTimestamp = (int)startTimeFromProps;
            return true;
        }
        else
        {
            Debug.Log("There isn't customprop");
        }

        return false;
    }


    public static void SetStartTime()
    {
        int startTime = 0;
        bool wasSet = TryGetStartTime(out startTime);

        Hashtable props = new()
        {
                {CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
        };
        bool a = PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Debug.Log($"SetCustom is {a}");
        Debug.Log("Set Custom Props for Time: " + props.ToStringFull() + " wasSet: " + wasSet);
    }
}
