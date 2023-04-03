using UnityEngine;
public class Arrow : MonoBehaviour, IPusher
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    [SerializeField] float power;
    public void ActivateProjectile()
    {
        lifetime = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        float movementSpeed = speed;
        transform.Translate(0, 0, movementSpeed);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
            gameObject.SetActive(false);
    }
    public float GetPower()
    {
        return power;
    }
}
