using UnityEngine;

public class OwnerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        animator.SetBool("Grounded", true);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
