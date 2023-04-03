using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

public enum FinishState
{
    Qualified,
    Eliminated,
    Win
}
public class InGameUIManager : MonoBehaviourPunCallbacks
{
    public static InGameUIManager instance;
    [SerializeField] GameObject playersCountInfoPanel;
    [SerializeField] GameObject finishInfoPanel;
    [SerializeField] TMP_Text TimerText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void SetupInstance()
    {
        HidePanels();
        GameManager.instance.OnPlayerFinished += PlayerCountInfoPanel;
        GameManager.instance.OnGameFinish += OnPlayerFinish;
        TextInStart();
        CountdownTimerSync.OnCountdownTimerHasExpired += HideTimer;
        CountdownTimerSync.OnCountdownTimerHasStarted += ShowTimer;
    }
    private void HideTimer()
    {
        TimerText.gameObject.SetActive(false);
    }
    private void ShowTimer()
    {
        TimerText.gameObject.SetActive(true);
    }
    public void OnPlayerFinish(FinishState state)
    {
        StartCoroutine(FinishedPanel(state));
    }
    private IEnumerator FinishedPanel(FinishState state)
    {
        playersCountInfoPanel.SetActive(false);
        finishInfoPanel.SetActive(true);
        switch (state)
        {
            case FinishState.Qualified:
                finishInfoPanel.transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(2.5f);
                finishInfoPanel.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case FinishState.Eliminated:
                finishInfoPanel.transform.GetChild(1).gameObject.SetActive(true);
                yield return new WaitForSeconds(2.5f);
                finishInfoPanel.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case FinishState.Win:
                finishInfoPanel.transform.GetChild(2).gameObject.SetActive(true);
                yield return new WaitForSeconds(2.5f);
                finishInfoPanel.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
        finishInfoPanel.SetActive(false);
    }
    public void HidePanels()
    {
        playersCountInfoPanel.SetActive(false);
        finishInfoPanel.SetActive(false);
    }
    public void OnLevelStart()
    {
        playersCountInfoPanel.SetActive(true);
    }
    private void TextInStart()
    {
        string text = $"{PhotonNetwork.CurrentRoom.FinishedPlayersCount} / {PhotonNetwork.CurrentRoom.ExceptedFinishCount}";
        playersCountInfoPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
    }
    public void PlayerCountInfoPanel()
    {
        string text = $"{PhotonNetwork.CurrentRoom.FinishedPlayersCount} / {PhotonNetwork.CurrentRoom.ExceptedFinishCount}";
        playersCountInfoPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        photonView.RPC("PlayerCountInfoPanelRPC", RpcTarget.OthersBuffered, text);
    }
    [PunRPC]
    public void PlayerCountInfoPanelRPC(string text)
    {
        SetText(text);
    }
    private void SetText(string text)
    {
        instance.playersCountInfoPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
    }
}
