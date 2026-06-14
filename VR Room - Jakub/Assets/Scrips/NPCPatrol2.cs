using System.Collections;
using UnityEngine;

public class NPCPatrol2 : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform crossingPoint;

    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float waitTime = 5f;

    private Transform target;
    private Animator animator;
    private Rigidbody rb;
    private bool isWaiting = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        target = crossingPoint;
    }

    void FixedUpdate()
    {
        if (isWaiting || target == null)
            return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + move);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            StartCoroutine(WaitAndChooseNextPoint());
        }
    }

    IEnumerator WaitAndChooseNextPoint()
    {
        isWaiting = true;

        if (animator != null)
            animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(waitTime);

        if (target == crossingPoint)
        {
            target = GetRandomPointExceptCrossing();
        }
        else
        {
            target = crossingPoint;
        }

        isWaiting = false;
    }

    Transform GetRandomPointExceptCrossing()
    {
        Transform randomPoint;

        do
        {
            int index = Random.Range(0, patrolPoints.Length);
            randomPoint = patrolPoints[index];
        }
        while (randomPoint == crossingPoint);

        return randomPoint;
    }
}
