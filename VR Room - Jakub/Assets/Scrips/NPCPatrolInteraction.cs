using System.Collections;
using UnityEngine;

public class NPCPatrolInteraction : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform crossingPoint;
    public Transform player;

    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float waitTime = 5f;
    public float detectionDistance = 4f;
    public float waveDuration = 2f;

    private Transform target;
    private Animator animator;
    private Rigidbody rb;

    private bool isWaiting = false;
    private bool isInteracting = false;
    private bool hasWaved = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.freezeRotation = true;

        target = crossingPoint;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionDistance)
            {
                LookAtPlayer();

                if (!isInteracting)
                    StartCoroutine(InteractWithPlayer());

                return;
            }
            else
            {
                hasWaved = false;
            }
        }

        if (isWaiting || target == null)
            return;

        MoveToTarget();
    }

    void MoveToTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + move);

            RotateTowards(direction);

            animator.SetBool("isWalking", true);
            animator.SetBool("isWaving", false);
        }
        else
        {
            StartCoroutine(WaitAndChooseNextPoint());
        }
    }

    IEnumerator InteractWithPlayer()
    {
        isInteracting = true;

        animator.SetBool("isWalking", false);

        if (!hasWaved)
        {
            hasWaved = true;

            animator.SetBool("isWaving", true);
            yield return new WaitForSeconds(waveDuration);
            animator.SetBool("isWaving", false);
        }

        while (player != null && Vector3.Distance(transform.position, player.position) <= detectionDistance)
        {
            LookAtPlayer();
            animator.SetBool("isWalking", false);
            animator.SetBool("isWaving", false);

            yield return null;
        }

        isInteracting = false;
    }

    IEnumerator WaitAndChooseNextPoint()
    {
        isWaiting = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isWaving", false);

        yield return new WaitForSeconds(waitTime);

        if (target == crossingPoint)
            target = GetRandomPointExceptCrossing();
        else
            target = crossingPoint;

        isWaiting = false;
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
            RotateTowards(direction);
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
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
