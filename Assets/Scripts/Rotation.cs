using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float rotX;
    [Range(0f, 1f)]
    [SerializeField] float rotY;
    [Range(0f, 1f)]
    [SerializeField] float rotZ;

    [SerializeField] float rotSpeed;
    Vector3 direction;
    private void Start()
    {
        direction = new Vector3(rotX, rotY, rotZ);
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(direction, rotSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
