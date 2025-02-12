using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private PlayerControls controls;
    private Animator animator;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = 9.8f;

    private bool isGrounded;
    private bool isJumping;

    void Awake() 
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing! Add one to the Player.");
            enabled = false;
            return;
        }

        // Input system event subscription
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Movement.Jump.performed += ctx => Jump();
        controls.Enable();
    }

    void OnEnable() => controls.Movement.Enable();
    void OnDisable() => controls.Movement.Disable();
   
    void Update()
    {
        HandleMovement();
        Debug.Log($"Move Input: {moveInput}, IsGrounded: {characterController.isGrounded}");
    }

    void HandleMovement()
    {
        // Read movement input and normalize
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y);

        // Only move if input is detected
        if (moveInput.magnitude > 0.01f)
        {
            moveDirection.x = move.x * speed;
            moveDirection.z = move.z * speed;
        }
        else
        {
            moveDirection.x = 0f; // Force zero movement when input stops
            moveDirection.z = 0f;
        }

        // Handle jumping and gravity
        if (isGrounded)
        {
            if (isJumping)
            {
                moveDirection.y = jumpForce;
                isJumping = false;
                animator.SetTrigger("Jump");
            }
            else
            {
                moveDirection.y = -0.5f; // Prevent floating
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        float movementMagnitude = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude;
        animator.SetFloat("Speed", movementMagnitude > 0.01f ? movementMagnitude : 0f);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void Jump()
    {
        if (isGrounded)
        {
            moveDirection.y = jumpForce;
            animator.SetTrigger("Jump");
        }
    }

}
