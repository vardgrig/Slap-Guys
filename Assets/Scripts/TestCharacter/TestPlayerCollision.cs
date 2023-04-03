using System.Collections;
using UnityEngine;

public class TestPlayerCollision : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float pushPower = 10f;
    [SerializeField] TestPlayerMovement player;
    private Vector3 startDir;
    private Vector3 endDir;

    IEnumerator PushPlayer(Vector3 dir)
    {
        player.canMove = false;
        rb.AddForce(dir, ForceMode.Acceleration);
        yield return new WaitForSeconds(1);
        player.canMove = true;
    }
    IEnumerator DisbaleCollider(Collider col) 
    {
        col.isTrigger = true;
        yield return new WaitForSeconds(1);
        col.isTrigger = false;
    }
    IEnumerator SlowDown()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1;
    }
    private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
        {
            Vector3 hitPoint = hit.contacts[0].point;
            Vector3 myPoint = transform.position;
            Vector3 direction = myPoint - hitPoint;
            direction.Normalize();
            direction.y = myPoint.y;
            Vector3 pointInDirection = pushPower * -direction;

            startDir = hitPoint;
            endDir = pointInDirection;
            
            StartCoroutine(PushPlayer(pointInDirection));
            StartCoroutine(SlowDown());
        }
    }
    private void OnCollisionExit(Collision hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(DisbaleCollider(hit.collider));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startDir, endDir);
        Gizmos.DrawSphere(startDir, 0.05f);
        Gizmos.DrawSphere(endDir, 0.1f);
    }
}