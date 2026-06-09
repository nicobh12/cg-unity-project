using System;
using Unity.VectorGraphics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float rotationSpeed = 12f;
    public float gravity = -9.81f;
    public float backwardInputThreshold = -0.1f;
    public float sprintMultiplier = 1.8f;
    private bool puedeCaminar = true;
    

    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (puedeCaminar)
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

            // SPRINT (Shift) - only when moving forward
            bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && z > 0.1f;
            float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

            animator.SetBool("isSprinting", isSprinting);

            // MOVE CHARACTER
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);

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

    public void desactivarMoviemiento()
    {
        puedeCaminar = false;
    }

    public void activarMovimiento()
    {
        puedeCaminar = true;
    }
    
}