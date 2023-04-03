using Cinemachine;
using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerSpawnDespawnSystem : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    public GameObject SpawnTransform;
    CinemachineVirtualCamera _cinVirtualCam;

    [Tooltip("Character's Mesh Renderer GameObject")]
    [SerializeField] GameObject _characterMeshGO;

    [Tooltip("Character's Body Material")]
    [SerializeField] Material _characterMaterial;

    CharacterController _controller;

    bool spawn = true;
    bool death = false;
    [SerializeField] float Mixvalue = 0;
    public new SkinnedMeshRenderer renderer = new();

    public void OnStartLocalPlayer()
    {
        if (!photonView.IsMine)
            return;

        SpawnTransform = GameObject.FindGameObjectWithTag("CheckPoints");
    }

    void Start()
    {
        if (!photonView.IsMine)
            return;

        SetNewMaterial();
        //photonView.RPC("SetNewMaterialRPC", RpcTarget.AllBuffered, photonView.ViewID);
        player = GetComponent<PlayerManager>();
        _controller = player.Controller;
        _cinVirtualCam = player.CinVirtualCam;
        StartCoroutine(PlayerSpawn());
    }

    
    public void SetNewMaterial()
    {
        renderer = _characterMeshGO.GetComponent<SkinnedMeshRenderer>();
        var originalMaterial = renderer.material;
        renderer.material = new Material(originalMaterial);
    }

    //[PunRPC]
    //public void SetNewMaterialRPC(int viewId)
    //{
    //    PhotonView pv = PhotonView.Find(viewId);
    //    pv.GetComponent<PlayerSpawnDespawnSystem>()._characterMeshGO.GetComponent<SkinnedMeshRenderer>().material = new Material(pv.GetComponent<PlayerSpawnDespawnSystem>()._characterMeshGO.GetComponent<SkinnedMeshRenderer>().material);
    //}

    [PunRPC]
    public void UpdateDissolveValue(float value, int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            Material material = view.gameObject.GetComponent<PlayerSpawnDespawnSystem>()._characterMeshGO.GetComponent<SkinnedMeshRenderer>().material;
            material.SetFloat("_Dissolve", value);
        }
    }
    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (spawn)
        {
            Mixvalue -= Time.deltaTime;
            if (Mixvalue < 0)
            {
                Mixvalue = 0;
                spawn = false;
            }
            renderer.material.SetFloat("_Dissolve", Mixvalue);
            photonView.RPC("UpdateDissolveValue", RpcTarget.AllBuffered, Mixvalue, photonView.ViewID);
        }
        else if (death)
        {
            Mixvalue += Time.deltaTime;
            if (Mixvalue > 1)
            {
                Mixvalue = 1;
                death = false;
            }
            renderer.material.SetFloat("_Dissolve", Mixvalue);
            photonView.RPC("UpdateDissolveValue", RpcTarget.AllBuffered, Mixvalue, photonView.ViewID);
        }
    }
    public IEnumerator PlayerSpawn()
    {
        if (!photonView.IsMine)
            yield break;

        CheckPointPositionSpawner();
        _characterMeshGO.SetActive(false);
        _cinVirtualCam.enabled = false;
        _cinVirtualCam.transform.position = transform.position;
        yield return new WaitForSeconds(0.2f);
        _cinVirtualCam.enabled = true;
        _controller.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _characterMeshGO.SetActive(true);
        renderer.material.SetFloat("_Dissolve", 1f);
        Mixvalue = 1f;
        spawn = true;
    }
    public IEnumerator PlayerDespawn()
    {
        if (!photonView.IsMine)
            yield return null;

        renderer.material.SetFloat("_Dissolve", 0f);
        Mixvalue = 0f;
        death = true;
        _controller.enabled = false;
        yield return null;
    }
    void CheckPointPositionSpawner()
    {
        if (!photonView.IsMine)
            return;

        var spawn = SpawnPoints.instance;
        var point = spawn.GetCurrentCheckPoint().transform;
        transform.SetPositionAndRotation(point.GetChild(Random.Range(0, point.childCount)).position, Quaternion.identity);
    }
    public void SetSpawnPoint(float x, float z)
    {
        SpawnTransform.transform.position = new Vector3(x, SpawnTransform.transform.position.y, z);
    }
}
