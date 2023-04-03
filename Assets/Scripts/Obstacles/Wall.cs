using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour, IPusher
{
    [SerializeField] float power;
    public float GetPower()
    {
        return power;
    }
}
