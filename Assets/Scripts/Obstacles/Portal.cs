using UnityEngine;

public class Portal : MonoBehaviour
{
    Transform reciver;
    [SerializeField] Transform[] PossiblePortals;
    [SerializeField] int count;
    
    public void OnPortalEnter(GameObject player)
    {
        reciver = PossiblePortals[Random.Range(0, PossiblePortals.Length)];
        if (player.GetComponent<PlayerCollision>().playerIsOverLapping && count > 0)
        {
            count--;
            player.transform.position = reciver.GetChild(0).transform.position;
            Debug.Log(reciver.transform.name);
        }
    }
}
