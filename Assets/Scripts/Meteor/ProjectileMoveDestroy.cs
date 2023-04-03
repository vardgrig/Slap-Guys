using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class ProjectileMoveDestroy : MonoBehaviour
{
    public float speed;
    public GameObject impactPrefab;
    public List<GameObject> trails;
    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Instantiate(trails[0], transform);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("GroundLayer"));
    }

    private void FixedUpdate()
    {
        if (speed != 0 && rb != null)
        {
            rb.position += transform.forward * (speed * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WalkableLayer"))
        {
            var tmp = speed;
            speed = 0;
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (impactPrefab != null)
            {
                var impactVFX = Instantiate(impactPrefab, pos, rot);
                Destroy(impactVFX, 5);
            }
            collision.gameObject.SetActive(false);
            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    var child = transform.GetChild(i);
                    child.transform.parent = null;
                    child.transform.localScale = Vector3.one;
                    if (child.TryGetComponent<ParticleSystem>(out var ps))
                    {
                        ps.Stop();
                        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                }
            }

            gameObject.SetActive(false);
            speed = tmp;
        }
    }
}
