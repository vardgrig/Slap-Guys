using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public float maxY;
    public float minY;
    public float posSpeed;
    Vector3 direction;
    bool isGoingDown = true;

    private void Start()
    {
        direction = new Vector3(transform.localPosition.x, -1, transform.localPosition.z);
        posSpeed = Random.Range(1f, 3f);
    }

    void Update()
    {
        StartCoroutine(Fly());
        transform.localPosition += direction * posSpeed * Time.deltaTime;
    }

    public IEnumerator Fly()
    {
        if (transform.localPosition.y >= maxY && !isGoingDown)
        {
            direction *= -1;
            isGoingDown = true;
        }
        else if (transform.localPosition.y < minY && isGoingDown)
        {
            direction *= -1;
            isGoingDown = false;
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            //direction *= -1;
        }
    }
}
