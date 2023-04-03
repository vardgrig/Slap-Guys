using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class NetworkManagerGUI : MonoBehaviourPunCallbacks
{
    #region Variables
    [SerializeField] GameObject _loginPanel;
    [SerializeField] GameObject _startPanel;
    [SerializeField] GameObject _waitingPanel;

    [SerializeField] Button startBtn;
    [SerializeField] Text userNickname;
    [SerializeField] InputField userNicknameInput;

    [SerializeField] Text roomInfoText;

    public List<Player> playerList = new();
    #endregion

    #region PUN_CallBacks
    public override void OnJoinedRoom()
    {
        playerList.Add(PhotonNetwork.LocalPlayer);
        int playersCount = playerList.Count;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        int remPlayers = maxPlayers - playersCount;
        roomInfoText.text = playersCount + " / " + maxPlayers + $"\n{remPlayers} Player" + (remPlayers > 1 ? "s" : "") + " Remaining";
        if (playersCount == maxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen= false;
            PhotonNetwork.LoadLevel("Level1-1");
        }
        else if(!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Not master client");
        }
        SetActivePanel(_waitingPanel.name);
    }
    public override void OnLeftRoom()
    {
        playerList.Remove(PhotonNetwork.LocalPlayer);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        SetActivePanel(_startPanel.name);
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        startBtn.interactable = true;
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Failed");
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 1
        };
        
        string roomName = "Room" + Random.Range(0, 100);
        PhotonNetwork.CreateRoom(roomName,roomOptions,TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} entered the room");
        int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        int remPlayers = maxPlayers - playersCount;
        roomInfoText.text = playersCount + " / " + maxPlayers + $"\n{remPlayers} Player" + (remPlayers > 1 ? "s" : "") + " Remaining";
        if (playersCount == maxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("Level3-1");
        }
    }
    #endregion

    #region Private voids

    private void SetActivePanel(string activePanel)
    {
        _loginPanel.SetActive(activePanel.Equals(_loginPanel.name));
        _startPanel.SetActive(activePanel.Equals(_startPanel.name));
        _waitingPanel.SetActive(activePanel.Equals(_waitingPanel.name));
    }

    public void OnLoginButtonClicked()
    {
        string playerName = userNicknameInput.text;

        if(!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString("Nickname", playerName);
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
            SetActivePanel(_startPanel.name);
            userNickname.text = playerName;
            userNickname.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
    private void Start()
    {
        startBtn.onClick.AddListener(StartGame);
        userNicknameInput.text = PlayerPrefs.GetString("Nickname");
        userNickname.gameObject.SetActive(false);
        SetActivePanel(_loginPanel.name);
        startBtn.interactable = false;
    }

    private void StartGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion
}