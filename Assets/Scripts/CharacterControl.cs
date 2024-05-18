using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CharacterControl : MonoBehaviour
{

    public static CharacterControl instance;

    // Player based events
    public event Action OnDead;
    public event Action OnSprint;
    public event Action OnCrouch;
    public event Action OnWalk;
    public event Action OnSlide;
    public event Action OnShoot;
    public event Action OnAim;
    public event Action OnStand;
    public event Action OnJump;

    public bool CanMove { get; private set; } = true;
    // Check player can sprint, and sprint key is held
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    // Check player's on the ground, and jump key is pressed
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    // Check player's on the ground, not during their crouch animation and also crouch key is pressed
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnim && characterController.isGrounded;

    [Header("Functional options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlope = true;
    [SerializeField] private int health = 100;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement parameters")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float slopeSpeed = 6.0f;

    [Header("Look parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouching parameters")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.1f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnim;

    [Header("Headbob parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.02f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.09f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.015f;
    private float defaultYPos = 0;
    private float timer;

    // Sliding params
    private Vector3 hitPointNormal;

    // Check if angle > slope limit with Raycast
    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0f;
    private int SavedHealth;

    void Awake()
    {
        // Set sprint speed to double the walk speed
        sprintSpeed = walkSpeed * 2;
        // Set params
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        // Set cursor to locked and invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SavedHealth = health;
        instance = this;
    }

    void Update()
    {
        // If player can move (not locked in cut scene or 'snare')
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            if (canJump) HandleJump();
            if (canCrouch) HandleCrouch();
            if (canUseHeadBob) HandleHeadBob();
            ApplyFinalMovements();
        }

        if (health <= 0)
            OnDead?.Invoke();

        if (Input.GetMouseButtonDown(0)) OnShoot?.Invoke();
        if (Input.GetMouseButtonDown(1)) OnAim?.Invoke();
    }

    private void HandleMovementInput()
    {
        // Get the current input angle as a Vector2
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        // Set direction to move towards
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0) OnWalk?.Invoke();
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 && IsSprinting) OnSprint?.Invoke();
    }

    private void HandleMouseLook()
    {
        // Rotate at speed
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        // Clamp limits (up/down)
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        // Rotate camera
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        // Rotate transform
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        // Just go up
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
            OnJump?.Invoke();
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        // Check player is actually moving
        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            // Get the timer based on whether player is walking/sprinting/crouching
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            // Local pos
            playerCamera.transform.localPosition = new Vector3( // Move Camera up and down
                playerCamera.transform.localPosition.x, // Camera's local position X
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount), // change y pos based on state
                playerCamera.transform.localPosition.z); // Camera's local position Y
        }
    }

    private void ApplyFinalMovements()
    {
        // Gravity check
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // Slope-slide check
        if (willSlideOnSlope && IsSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
            OnSlide?.Invoke();
        }


        // Move player
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {

        // Ensure the player's not got anything above their head when crouched, if something is above, do nothing
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        // Set during
        duringCrouchAnim = true;

        // Params
        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            // Move character height and center down, based on amount of crouch per timeElapsed
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;

            yield return null; // Move next
        }

        // Set the height to get the absolute value
        // "Sanity check"
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        // Swap isCrouching
        isCrouching = !isCrouching;

        if (isCrouching) OnCrouch?.Invoke();
        else if (!isCrouching) OnStand?.Invoke();

        // Set during
        duringCrouchAnim = false;
    }
}
