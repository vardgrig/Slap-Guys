using UnityEngine;
using Photon.Pun;

public class GridPathMaker : MonoBehaviourPunCallbacks
{
    GameObject[,] grid = new GameObject[11,5];
    [SerializeField] Transform gridParent;

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FillGrid();
            CreatePath();
            Test();
        }
    }

    void FillGrid()
    {
        for(int i = 0; i < 11; ++i)
            for(int j = 0; j < 5; ++j)
                grid[i, j] = gridParent.GetChild(i * 5 + j).gameObject;
    }
    void CreatePath()
    {
        int iLength = grid.GetLength(0);
        int jLength = grid.GetLength(1);

        int j = Random.Range(0, jLength);

        for(int i = 0; i < iLength; ++i)
        {
            PhotonView pv = grid[i,j].GetComponent<PhotonView>();
            pv.RPC("ChangeProperties", RpcTarget.All, pv.ViewID);
            if (j != 0 && j != jLength - 1)
                j = Random.Range(j - 1, j + 2);
            else if (j == 0)
                j = Random.Range(j, j + 2);
            else if (j == jLength - 1)
                j = Random.Range(j - 1, j + 1);
        }
    }
    void Test()
    {
        for(int i = 0; i < gridParent.childCount; ++i)
        {
            if (gridParent.GetChild(i).gameObject.layer == LayerMask.NameToLayer("WrongCube"))
                gridParent.GetChild(i).gameObject.SetActive(false);
        }
    }
}
