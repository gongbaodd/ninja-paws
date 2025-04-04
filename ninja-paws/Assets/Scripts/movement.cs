using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class movement : MonoBehaviour
{
    public Camera mainCamera;
    //public Animator catAnim;

    public LayerMask groundMask;
    //public GameObject cat;
    public float movementSpeed = 2f;
    //public float stoppingDistance = 0.1f;

    private NavMeshAgent catAgent;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        catAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToClickPoint();
            //OnMouseClicked();
        }

        // Check if the cat has reached its destination
        // if (!catAgent.pathPending && catAgent.remainingDistance <= stoppingDistance)
        // {
        //     catAnim.SetBool("isMoving", false);
        // }

        // if (isMoving)
        // {

        //     Vector3 moveDirection = (targetPosition - transform.position).normalized;
        //     rb.MovePosition(transform.position + moveDirection * movementSpeed * Time.fixedDeltaTime);

        //     //cat.transform.position = Vector3.Lerp(cat.transform.position, targetPosition, movementSpeed * Time.deltaTime);

        //     if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        //     {
        //         isMoving = false;
        //         //catAnim.SetBool("isMoving", false);
        //     }
        // }
    }

    void OnMouseClicked()
    {
        // Get the click position on screen
        Vector3 clickPosition = Input.mousePosition;

        // Create a ray starting at click point on screen and moves along the camera perspective
        Ray clickRay = mainCamera.ScreenPointToRay(clickPosition);

        // Declare a variable to store the Raycast hit information
        RaycastHit hit;

        // Physics.Raycast returns a bool (true/false) whether it hit a collider or not
        // Interaction mask allows us to filter what objects the ray should register
        if (Physics.Raycast(clickRay, out hit, 100f, groundMask))
        {
            print("Something clicked");

            // Do other logic with the object we clicked on
            //catAgent.SetDestination(hit.point);
            targetPosition = hit.point;
            isMoving = true;
            //catAnim.SetBool("isMoving", true);

        }
        else
        {
            print("Nothing clicked!");
        }




    }

    void MoveToClickPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Only move if we click on the ground
        if (Physics.Raycast(ray, out hit, 100f, groundMask))
        {
            catAgent.SetDestination(hit.point); // Move to clicked position
        }
    }

}