using System.Collections;
using UnityEngine;

public class Udarylka : MonoBehaviour, IPusher
{
    [SerializeField] float power;
    [SerializeField] bool charged;
    Animator animator;
    public Collider col;
    private void Start()
    {
        animator = GetComponent<Animator>();
        charged = true;
    }
    private void Update()
    {
        if(charged)
            StartCoroutine(Reloading());
    }
    public IEnumerator Reloading()
    {
        animator.SetBool("Charged", true);
        col.enabled = false;
        charged = false;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        animator.SetBool("Charged", false);
        col.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        charged = true;
    }
    public float GetPower()
    {
        return power;
    }
    public bool GetChargedStatus()
    {
        return charged;
    }
}
