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
    public string isInDDRParameterName = "IsInDDR";
    private bool puedeCaminar = true;

    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (PermanentHUDManager.Instance != null && PermanentHUDManager.Instance.IsGameplayLocked)
        {
            velocity = Vector3.zero;

            if (animator != null)
            {
                animator.SetBool(isInDDRParameterName, GameManager.Instance != null && GameManager.Instance.IsDDRActive);
                animator.SetFloat("Speed", 0f);
                animator.SetBool("isSprinting", false);
                animator.SetBool("IsWalkingBackward", false);
            }

            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsDDRActive)
        {
            velocity = Vector3.zero;

            if (animator != null)
            {
                animator.SetBool(isInDDRParameterName, true);
                animator.SetFloat("Speed", 0f);
                animator.SetBool("isSprinting", false);
                animator.SetBool("IsWalkingBackward", false);
            }

            return;
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool(isInDDRParameterName, false);
            }
        }

        // INPUT
        float x = puedeCaminar ? Input.GetAxis("Horizontal") : 0f;
        float z = puedeCaminar ? Input.GetAxis("Vertical") : 0f;
        
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

    public void desactivarMoviemiento()
    {
        puedeCaminar = false;
    }

    public void activarMovimiento()
    {
        puedeCaminar = true;
    }
    
}