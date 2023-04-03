using UnityEngine;
using Photon.Pun;
public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InGameUIManager.instance.OnLevelStart();
        int i = Random.Range(0, spawnPoints.childCount);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints.GetChild(i).position, Quaternion.identity);
    }
}
