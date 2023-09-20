using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class newmovement : MonoBehaviour
    {
        PlayerInput playerInput;
        CharacterController characterController;
        Animator animator;

        int isWalkingHash;
        int isRunningHash;

        Vector2 currentMovementInput;
        Vector3 currentMovement;
        Vector3 currentRunMovement;
        bool isMovementPressed;
        bool isRunPressed;

        float rotationFactorPerFrame = 10.0f;
        float runMultiplier = 7.6f;
        int zero = 0;

        float gravity = -3f;
        float groundedGravity = -1.05f;

        bool isJumpPressed = false;
        float initialJumpVelocity;
        float maxJumpHeight = 7.5f;
        float maxJumpTime = 3.75f;
        bool isJumping = false;

        private Camera mainCamera; // Reference to the main camera

        void Awake()
        {
            playerInput = new PlayerInput();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");

            playerInput.CharacterControls.Move.started += onMovementInput;
            playerInput.CharacterControls.Move.canceled += onMovementInput;
            playerInput.CharacterControls.Move.performed += onMovementInput;
            playerInput.CharacterControls.Run.started += onRun;
            playerInput.CharacterControls.Run.canceled += onRun;
            playerInput.CharacterControls.Jump.started += onJump;
            playerInput.CharacterControls.Jump.canceled += onJump;

            setupJumpVariables();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mainCamera = Camera.main; // Assign the main camera reference
        }

        void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        void handleJump()
        {
            if (!isJumping && characterController.isGrounded && isJumpPressed)
            {
                isJumping = true;
                currentMovement.y = initialJumpVelocity * .5f;
                currentRunMovement.y = initialJumpVelocity * .5f;
            }
            else if (!isJumpPressed && isJumping && characterController.isGrounded)
            {
                isJumping = false;
            }
        }

        void onJump(InputAction.CallbackContext context)
        {
            isJumpPressed = context.ReadValueAsButton();
        }

        void onRun(InputAction.CallbackContext context)
        {
            isRunPressed = context.ReadValueAsButton();
        }

        void handleRotation()
        {
            // Calculate the movement direction relative to the camera's forward direction
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f; // Ensure it's horizontal
            cameraForward.Normalize();

            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * currentMovementInput.y + cameraRight * currentMovementInput.x).normalized;

            if (isMovementPressed && moveDirection != Vector3.zero)
            {
                // Rotate the character to face the movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }

        void onMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();

            // Calculate the movement direction relative to the camera's forward direction
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f; // Ensure it's horizontal
            cameraForward.Normalize();

            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            currentMovement = (cameraForward * currentMovementInput.y + cameraRight * currentMovementInput.x).normalized;

            currentRunMovement.x = currentMovement.x * runMultiplier;
            currentRunMovement.z = currentMovement.z * runMultiplier;

            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void handleAnimation()
        {
            bool isWalking = animator.GetBool(isWalkingHash);
            bool isRunning = animator.GetBool(isRunningHash);

            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(isWalkingHash, true);
            }
            else if (!isMovementPressed && isWalking)
            {
                animator.SetBool(isWalkingHash, false);
            }

            if ((isMovementPressed && isRunPressed) && !isRunning)
            {
                animator.SetBool(isRunningHash, true);
            }
            else if ((!isMovementPressed || !isRunPressed) && isRunning)
            {
                animator.SetBool(isRunningHash, false);
            }
        }

        void handleGravity()
        {
            bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
            float fallMultiplier = 2.0f;

            if (characterController.isGrounded)
            {
                currentMovement.y = groundedGravity;
                currentRunMovement.y = groundedGravity;
            }
            else if (isFalling)
            {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
                float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
                currentMovement.y = nextYVelocity;
                currentRunMovement.y = nextYVelocity;
            }
            else
            {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
                float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
                currentMovement.y = nextYVelocity;
                currentRunMovement.y = nextYVelocity;
            }
        }

        void Update()
        {
            handleRotation();
            handleAnimation();

            if (isRunPressed)
            {
                characterController.Move(currentRunMovement * Time.deltaTime);
            }
            else
            {
                characterController.Move(currentMovement * Time.deltaTime);
            }
            handleGravity();
            handleJump();
        }

        void OnEnable()
        {
            playerInput.CharacterControls.Enable();
        }

        void OnDisable()
        {
            playerInput.CharacterControls.Disable();
        }
    }
}
