using UnityEngine;

public class FireTrap : MonoBehaviour
{
    public float coldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    private float coldownTimer;

    private void Start()
    {
        coldown = Random.Range(2f, 4f);
    }
    private void Attack()
    {
        coldownTimer = 0;
        bullet.transform.position = firePoint.position;
        bullet.GetComponent<Arrow>().ActivateProjectile();
    }

    private void Update()
    {

        coldownTimer += Time.deltaTime;
        if (coldownTimer >= coldown)
        {
            Attack();
        }
    }
}
