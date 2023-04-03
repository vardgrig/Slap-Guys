using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerCollision : MonoBehaviourPunCallbacks
{
    [Tooltip("What layers the character uses as death zone")]
    [SerializeField] LayerMask DeathZoneLayer;
    [SerializeField] LayerMask FinsihLayer;
    PlayerManager playerManager;
    PlayerSpawnDespawnSystem playerSpawn;
    PlayerPushSystem pushSystem;
    [HideInInspector]
    public bool playerIsOverLapping = false;
    bool isSpawned = true;
    void Start()
    {
        if (!photonView.IsMine)
            return;

        playerManager = GetComponent<PlayerManager>();
        playerSpawn = GetComponent<PlayerSpawnDespawnSystem>();
        pushSystem = GetComponent<PlayerPushSystem>();
        isSpawned = true;
    }

    private IEnumerator BrokenLegs()
    {
        GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(1f);
        GetComponent<PlayerInput>().enabled = true;
    }
    IEnumerator DespawnAndSpawn()
    {
        StartCoroutine(playerSpawn.PlayerDespawn());
        yield return new WaitForSeconds(2f);
        StartCoroutine(playerSpawn.PlayerSpawn());
        isSpawned = true;
    }
    IEnumerator Teleport()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Rigidbody>().isKinematic = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!photonView.IsMine)
            return;

        if (hit.gameObject.CompareTag("Trampoline"))
        {
            if (hit.gameObject.GetComponent<Trampoline>().GetChargedStatus())
            {
                var trampoline = hit.gameObject.GetComponent<Trampoline>();
                StartCoroutine(pushSystem.PushPlayer(Vector3.up, trampoline.GetPower(),hit.collider));
                StartCoroutine(trampoline.Reloading());
            }
        }
        if (hit.gameObject.layer == LayerMask.NameToLayer("WrongCubeLayer"))
        {
            Debug.Log("Wrong Path");
            PhotonView pv = hit.gameObject.GetComponent<PhotonView>();
            pv.RPC("DestroyCube", RpcTarget.All, pv.ViewID);
        }
        if (hit.gameObject.CompareTag("PathCube"))
        {
            PhotonView pv = hit.gameObject.GetComponent<PhotonView>();
            pv.RPC("ChangeCubeColor", RpcTarget.All, pv.ViewID);
        }
        if (hit.gameObject.CompareTag("Wall"))
        {
            var power = hit.gameObject.GetComponent<Wall>().GetPower();
            var _targetDir = transform.position - hit.point;
            _targetDir.Normalize();
            _targetDir.y = transform.position.y;
            StartCoroutine(pushSystem.PushPlayer(_targetDir, power, hit.collider));
            StartCoroutine(BrokenLegs());
        }
        if (hit.gameObject.layer == Mathf.Log(FinsihLayer.value, 2))
        {
            StartCoroutine(playerSpawn.PlayerDespawn());
            StartCoroutine(playerManager.OnPlayerFinished());
            hit.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        if (hit.gameObject.layer == LayerMask.NameToLayer("FallLayer"))
        {
            Debug.Log("Dead?");
            if (isSpawned)
            {
                Debug.Log("Dead");
                isSpawned = false;
                StartCoroutine(DespawnAndSpawn());
            }
        }
        if (hit.gameObject.CompareTag("Portal"))
        {
            playerIsOverLapping = true;
            StartCoroutine(Teleport());
            var portal = hit.gameObject.GetComponent<Portal>();
            if(portal)
                    portal.OnPortalEnter(gameObject);
        }
        if (hit.gameObject.CompareTag("CheckPoint") && !SpawnPoints.instance.GetCheckPointState(hit.gameObject))
        {
            var go = hit.gameObject.transform;
            playerSpawn.SetSpawnPoint(go.position.x, go.position.z);
            SpawnPoints.instance.SetCheckPointState();
        }
        //if (hit.gameObject.CompareTag("Enemy"))
        //{
        //    Debug.Log("Arrow Hit!");

        //    var power = hit.gameObject.GetComponent<Arrow>().GetPower();
        //    var _targetDir = hit.point - transform.position;
        //    _targetDir.y = 0;
        //    StartCoroutine(pushSystem.PushPlayer(-_targetDir.normalized, power, hit.collider));
        //    StartCoroutine(BrokenLegs());
        //}
    }
}
