using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public CharacterController Controller { get; private set; }
    public StandartPlayerInput Input { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public CinemachineVirtualCamera CinVirtualCam { get; private set; }
    public Camera MainCamera { get; private set; }
    public bool CanMove => GameManager.PlayersCanMove;

    public bool IsCurrentDeviceGamepad
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return PlayerInput.currentControlScheme == "Gamepad";
#else
				return false;
#endif
        }
    }

    void Awake()
    {
        if (!photonView.IsMine)
            return;

        Controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        MainCamera = Camera.main;
        CinVirtualCam = GameObject.FindGameObjectWithTag("CinemachineTarget").GetComponent<CinemachineVirtualCamera>();
        PlayerInput.enabled = true;
        Input = GetComponent<StandartPlayerInput>();
    }
    private void OnValidate()
    {
        if (!photonView.IsMine)
            return;

        if (Controller == null)
            Controller = GetComponent<CharacterController>();
        if (PlayerInput == null)
            PlayerInput = GetComponent<PlayerInput>();
    }


    public void OnStartLocalPlayer()
    {
        if (!photonView.IsMine)
            return;
        Controller.enabled = true;
        PlayerInput.enabled = true;
        Input = GetComponent<StandartPlayerInput>();
    }
    public IEnumerator DelayedSetMasterClient(Player newPlayer)
    {
        yield return new WaitForSeconds(0.1f);
        if (!PhotonNetwork.SetMasterClient(newPlayer))
        {
            Debug.LogError("Can't Set New MasterClient");
        }
    }

    [PunRPC]
    public void RequestMasterClientChange(Player newPlayer)
    {
        StartCoroutine(DelayedSetMasterClient(newPlayer));
    }

    public IEnumerator OnPlayerFinished()
    {
        if (!photonView.IsMine)
            yield break;

        if(PhotonNetwork.CurrentRoom.FinishedPlayersCount == 0)
        {
            photonView.RPC("RequestMasterClientChange", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        StartCoroutine(GameManager.instance.CheckFinishState());
    }
}