using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPush : MonoBehaviour
{
    public float power;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) 
        {
            rb.AddForce(Vector3.forward * power);
        }
    }
}
