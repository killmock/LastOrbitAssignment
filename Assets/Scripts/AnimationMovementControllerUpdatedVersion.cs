using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class AnimationMovementControllerUpdatedVersion : MonoBehaviour
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
        float walkingSpeed = 4.0f;
        int zero = 0;

        float gravity = -3f;
        float groundedGravity = -1.05f;

        bool isJumpPressed = false;
        float initialJumpVelocity;
        float maxJumpHeight = 8f;
        float maxJumpTime = 3.75f;
        bool isJumping = false;

        //[SerializeField] Cinemachine.CinemachineFreeLook mainCamera;

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
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
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
            // Calculate the movement direction based on the input
            Vector3 moveDirection = new Vector3(currentMovementInput.x, 0, currentMovementInput.y).normalized;

            // Check if there's any movement input
            if (moveDirection != Vector3.zero)
            {
                // Calculate the camera's forward direction in the XZ plane
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                // Calculate the input direction relative to the camera
                Vector3 inputRelativeToCamera = Quaternion.LookRotation(cameraForward) * moveDirection;

                // Use the input relative to the camera for character movement
                currentMovement.x = inputRelativeToCamera.x;
                currentMovement.z = inputRelativeToCamera.z;

                // Calculate the rotation angle based on the movement direction
                float targetRotationAngle = Mathf.Atan2(inputRelativeToCamera.x, inputRelativeToCamera.z) * Mathf.Rad2Deg;
                float rotationSpeed = 10.0f; // Adjust the rotation speed as needed

                // Smoothly rotate the character towards the target rotation angle
                Quaternion targetRotation = Quaternion.Euler(0, targetRotationAngle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // No movement input, keep the character's rotation
                currentMovement.x = 0;
                currentMovement.z = 0;
            }

            // Use the transformed moveDirection for character movement
            currentRunMovement.x = currentMovement.x * runMultiplier;
            currentRunMovement.z = currentMovement.z * runMultiplier;
        }

        // handler function to set the player input values
        void onMovementInput(InputAction.CallbackContext context)

        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * walkingSpeed;
            currentMovement.z = currentMovementInput.y * walkingSpeed;

            currentRunMovement.x = currentMovementInput.x * runMultiplier;
            currentRunMovement.z = currentMovementInput.y * runMultiplier;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void handleAnimation()
        {
            // parameter values from animator
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
                float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
                currentMovement.y = nextYVelocity;
                currentRunMovement.y = nextYVelocity;
            }
            else
            {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
                float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
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
