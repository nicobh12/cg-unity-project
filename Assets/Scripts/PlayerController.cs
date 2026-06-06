using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float rotationSpeed = 12f;
    public float gravity = -9.81f;
    public float backwardInputThreshold = -0.1f;

    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // INPUT
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector2 input = new Vector2(x, z);

        // PLAYER DIRECTION (flattened)
        Vector3 moveForward = transform.forward;
        Vector3 moveRight = transform.right;

        moveForward.y = 0;
        moveRight.y = 0;

        moveForward.Normalize();
        moveRight.Normalize();

        // FINAL MOVE DIRECTION
        Vector3 moveDirection = moveForward * z + moveRight * x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // MOVE CHARACTER
        controller.Move(moveDirection * speed * Time.deltaTime);

        // ANIMATION (smooth value)
        float speedPercent = Mathf.Clamp01(input.magnitude);
        animator.SetFloat("Speed", speedPercent);
        animator.SetBool("IsWalkingBackward", z < backwardInputThreshold && speedPercent > 0.1f);

        // ROTATION (ONLY if moving)
        if (moveDirection.magnitude > 0.1f && z >= 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // GRAVITY
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}