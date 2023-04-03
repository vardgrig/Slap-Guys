using UnityEngine;
using Photon.Pun;

public class FallCubeProperties : MonoBehaviour
{
    public Material materialRed;
    public Material materialGreen;

    private void Start()
    {
        materialRed = GetComponent<MeshRenderer>().sharedMaterial;
        
    }
    [PunRPC]
    public void ChangeProperties(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);

        pv.gameObject.GetComponent<BoxCollider>().enabled = true;
        pv.gameObject.layer = LayerMask.NameToLayer("GroundLayer");
        pv.gameObject.tag = "PathCube";
    }

    [PunRPC]
    public void ChangeCubeColor(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        pv.gameObject.GetComponent<MeshRenderer>().sharedMaterial = materialGreen;
    }

    [PunRPC]
    public void DestroyCube(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);

        Destroy(pv.gameObject);
    }
}
