using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    [SerializeField] TMP_Text nicknameText;
    Camera _mainCamera;

    void Start()
    {
        if (!photonView.IsMine)
        {
            nicknameText.color = Color.red;
            return;
        }

        nicknameText.color = Color.cyan;
        player = GetComponent<PlayerManager>();
        _mainCamera = player.MainCamera;
        var pv = GetComponent<PhotonView>();
        pv.RPC("SetNicknameRPC", RpcTarget.AllBuffered, pv.ViewID, PhotonNetwork.LocalPlayer.NickName);
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        NicknameRotation();
    }

    void NicknameRotation()
    {
        foreach (var playerUIManager in FindObjectsOfType<PlayerUIManager>())
        {
            playerUIManager.nicknameText.transform.rotation = _mainCamera.transform.rotation;
        }
    }
    [PunRPC]
    public void SetNicknameRPC(int viewID, string nickname)
    {
        PhotonView pv = PhotonView.Find(viewID);
        nicknameText.text = nickname;
        pv.GetComponent<PlayerUIManager>().nicknameText.text = nicknameText.text;
    }
}
