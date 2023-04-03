using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints instance;
    [SerializeField] List<GameObject> CheckPoints;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    
    public GameObject GetCurrentCheckPoint()
    {
        return CheckPoints[0];
    }
    public bool GetCheckPointState(GameObject checkpoint)
    {
        bool isActive = checkpoint.GetComponent<CheckPoint>().GetCheckPoint();
        return isActive;
    }

    public void SetCheckPointState()
    {
        CheckPoints.Remove(CheckPoints[0]);
        CheckPoints[0].SetActive(true);
    }
}
