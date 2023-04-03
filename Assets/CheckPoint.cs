using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] GameObject Point;
    
    public bool GetCheckPoint()
    {
        return Point.activeInHierarchy;
    }
}
