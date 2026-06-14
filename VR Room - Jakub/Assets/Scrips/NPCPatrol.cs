using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float rotationSpeed = 5f;

    private Transform target;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        target = pointB;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
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

            animator.SetBool("isWalking", true);
        }
        else
        {
            target = target == pointA ? pointB : pointA;
            animator.SetBool("isWalking", false);
        }
    }
}
