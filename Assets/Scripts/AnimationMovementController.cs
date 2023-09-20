using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Platformer
{
    public class AnimationMovementController: MonoBehaviour
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
        }

        void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        void handleJump()
        {
            if (!isJumping && characterController.isGrounded && isJumpPressed) {
                isJumping = true;
                currentMovement.y = initialJumpVelocity * .5f;
                currentRunMovement.y = initialJumpVelocity * .5f; 
            } else if (!isJumpPressed && isJumping && characterController.isGrounded) {
                isJumping = false;
            }
        }

        void onJump (InputAction.CallbackContext context)
        {
            isJumpPressed = context.ReadValueAsButton();
        }
            
        void onRun (InputAction.CallbackContext context)
        {
            isRunPressed = context.ReadValueAsButton();
        }
   
       
        void handleRotation()
        {
            Vector3 positionToLookAt;
            // changes in position character should point to
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = zero;
            positionToLookAt.z = currentMovement.z;
            // current rotation of character
            Quaternion currentRotation = transform.rotation;

            if (isMovementPressed) {
                // creates new rotation based on where the player is pressing
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

                // rotate the character to face posiyionToLookAt
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }  
        }
       

        // handler function to set the player input values
        void onMovementInput(InputAction.CallbackContext context)

        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
          
            currentRunMovement.x = currentMovementInput.x * runMultiplier;
            currentRunMovement.z = currentMovementInput.y * runMultiplier;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void handleAnimation()
        {
            // parameter values from animator
            bool isWalking = animator.GetBool(isWalkingHash);
            bool isRunning = animator.GetBool(isRunningHash);

            if (isMovementPressed && !isWalking) {
                animator.SetBool(isWalkingHash, true);
            }

            else if (!isMovementPressed && isWalking) {
                animator.SetBool(isWalkingHash, false);
            }

            if ((isMovementPressed && isRunPressed) && !isRunning)
            {
                animator.SetBool(isRunningHash, true);
            }

            else if ((!isMovementPressed || !isRunPressed) && isRunning) {
                animator.SetBool(isRunningHash, false);
            }
        }

        void handleGravity()
        {
            bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
            float fallMultiplier = 2.0f;
            // apply gravity if player is grounded or not
            if (characterController.isGrounded) {
                currentMovement.y = groundedGravity;
                currentRunMovement.y = groundedGravity;

            } else if (isFalling) {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
                float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
                currentMovement.y = nextYVelocity;
                currentRunMovement.y = nextYVelocity;

            } else {
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
            
            if (isRunPressed) {
                characterController.Move(currentRunMovement * Time.deltaTime);
            } else {
                characterController.Move(currentMovement * Time.deltaTime);
            }
            handleGravity();
            handleJump();

        }


        void OnEnable()
        {
            // enable the character controls action map
            playerInput.CharacterControls.Enable();
        }

        void OnDisable()
        {
            playerInput.CharacterControls.Disable();
        }
    }
}

