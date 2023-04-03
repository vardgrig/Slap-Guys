using System.Collections;
using UnityEngine;

public class Trampoline : MonoBehaviour,IPusher
{
    [SerializeField] float power;
    [SerializeField] bool charged;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        charged = true;
    }
    public IEnumerator Reloading()
    {
        yield return new WaitForSeconds(0.05f);
        charged = false;
        animator.SetBool("Charged", true);
        yield return new WaitForSeconds(0.2f);
        charged = true;
        animator.SetBool("Charged", false);
    }
    public bool GetChargedStatus()
    {
        return charged;
    }
    public float GetPower()
    {
        return power;
    }
}
