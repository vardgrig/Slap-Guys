using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    StandartPlayerInput input;
    Camera cam;
    Rigidbody body;

    //Collision State
    List<Collider> colliding = new List<Collider>();
    Collider groundCollider = new Collider();
    Rigidbody groundRigidbody = new Rigidbody();
    Vector3 groundNormal = Vector3.down;
    Vector3 groundContactPoint = Vector3.zero;
    Vector3 groundVelocity = Vector3.zero;

    //Initialize variables
    void Start()
    {
        cam = Camera.main;
        body = GetComponent<Rigidbody>();
        input = GetComponent<StandartPlayerInput>();

    }

    //Movement Handling
    void FixedUpdate()
    {
        //Record the world-space walking movement
        Vector3 movement = transform.rotation * new Vector3(input.move.x, 0f, input.move.y).normalized;

        //If we're currently contacting a wall/ground like object
        if (groundCollider != null && Vector3.Dot(Vector3.up, groundNormal) > -0.3f)
        {
            //Subtract the ground's velocity
            if (groundRigidbody != null && groundRigidbody.isKinematic)
            {
                body.velocity -= groundVelocity;
            }

            //Walking along the ground movement
            if (Vector3.Dot(Vector3.up, groundNormal) > 0.5f)
            {
                if (movement != Vector3.zero)
                {
                    Vector2 XYVel = new(body.velocity.x, body.velocity.z);
                    XYVel = Mathf.Clamp(XYVel.magnitude, 0f, 5f) * XYVel.normalized;
                    body.velocity = new Vector3(XYVel.x, body.velocity.y, XYVel.y);
                    //Anim Run
                }
                //idle
                else
                {
                    //IdleAnimation();
                    body.velocity = new Vector3(body.velocity.x * 0.8f, body.velocity.y, body.velocity.z * 0.8f);
                }
                body.velocity += movement;
            }

            //Handle jumping
            if (input.jump && body.velocity.y <= 0.1f) 
            {
                //JumpAnimation();
                body.velocity += Vector3.Slerp(Vector3.up, groundNormal, 0.2f) * 6f; 
                input.jump = false;
            }

            //Draw some debug info
            Debug.DrawLine(groundContactPoint, groundContactPoint + groundNormal, Color.blue, 2f);

            //Add back the ground's velocity
            if (groundRigidbody != null && groundRigidbody.isKinematic)
            {
                groundVelocity = groundRigidbody.GetPointVelocity(groundContactPoint);
                body.velocity += groundVelocity;
            }
        }
        else
        {
            //FreeFallAnimation();
            body.velocity += movement * 0.1f;
            groundVelocity = Vector3.zero;
        }

        groundNormal = Vector3.down;
        groundCollider = null;
        groundRigidbody = null;
        groundContactPoint = (transform.position - Vector3.down * -0.5f);
    }


    void Update()
    {
        //Rotate the player
        transform.Rotate(0, (Input.GetAxis("Mouse X")) * 2f, 0);

        //Rotate the camera rig and prevent it from penetrating the environment
        RaycastHit hit;
        cam.transform.rotation *= Quaternion.Euler(-input.look.y * 2f, 0, 0);
        cam.transform.position = (Physics.SphereCast(cam.transform.position, cam.nearClipPlane * 0.5f, -cam.transform.forward, out hit, 3f) ? (Vector3.back * hit.distance) : Vector3.back * 3f);
    }


    //Ground Collision Handling
    void OnCollisionEnter(Collision collision)
    {
        colliding.Add(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.impulse.magnitude > float.Epsilon)
        {
            if (!colliding.Contains(collision.collider))
            {
                colliding.Add(collision.collider);
            }

            //Record ground telemetry
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Vector3.Dot(Vector3.up, collision.contacts[i].normal) > Vector3.Dot(Vector3.up, groundNormal))
                {
                    groundNormal = collision.contacts[i].normal;
                    groundCollider = collision.collider;
                    groundContactPoint = collision.contacts[i].point;
                    groundRigidbody = collision.rigidbody;
                    if (groundRigidbody != null && groundVelocity == Vector3.zero)
                    {
                        groundVelocity = groundRigidbody.GetPointVelocity(groundContactPoint);
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        colliding.Remove(collision.collider);
        if (colliding.Count == 0)
        {
            groundVelocity = Vector3.zero;
        }
    }
}