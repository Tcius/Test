using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    public bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool canHeal = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode healKey = KeyCode.E;


    [Header("Movement parameters")]
    [SerializeField] private float walkSpeed = 6.0f;
    [SerializeField] private float sprintSpeed = 12.0f;

    [Header("Look parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    [Header("Headbob parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float staminaValueIncrement = 0.1f;
    float defaultYPos = 0;
    float timer = 0;

    [Header("Health parameters")]
    [SerializeField] private int maxArrowCount = 3;
    [SerializeField] private float timeToHeal = 0.5f;
    private int currentArrowCount = 3;
    private Coroutine regeneratingHealth;
    public static Action<int> OnTakeDamage;
    public static Action<int, int> OnHeal;
    float time = 0;
    [SerializeField]float health = 3;

    [Header("Stamina parameters")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaUseMultiplicator = 0.05f;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 2f;
    [SerializeField] private float staminaTimeIncerement = 0.1f;
    float currentStamina;
    Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;
    void Awake()
    {

        currentStamina = maxStamina;

        //Caching
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        defaultYPos = playerCamera.transform.localPosition.y;

        OnHeal?.Invoke(currentArrowCount, maxArrowCount);
        OnStaminaChange?.Invoke(currentStamina);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            MovementInput();
            CameraMovement();

            if (canJump)
                HandleJump();

            if (canHeadbob)
                HandleHeadbob();

            if (canHeal)
                HandleHeal();

            HandleStamina();

            ApplyFinalMovements();
        }
    }


    void HandleStamina()
    {
        if (IsSprinting && moveDirection != Vector3.zero)
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }

            currentStamina -= staminaUseMultiplicator * Time.deltaTime;

            if (currentStamina < 0)
                currentStamina = 0;

            if (currentStamina <= 0f)
                canSprint = false;

        }
        if (!IsSprinting && currentStamina < maxStamina && regeneratingStamina != null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncerement);
        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }
        regeneratingStamina = null;
    }


    IEnumerator RegenerateHealth()
    {
        if (currentArrowCount <= 0)
            yield break;

        if (Input.GetKey(healKey))
        {
            if (time < timeToHeal)
            {
                time += Time.deltaTime;
                yield return new WaitForSeconds(timeToHeal);
            }
            currentArrowCount--;
            OnHeal?.Invoke(currentArrowCount, maxArrowCount);
            time = 0.0f;
        }
        regeneratingHealth = null;
    }
    void HandleHeal()
    {
        if (regeneratingHealth != null)
        {
            StopCoroutine(regeneratingHealth);
        }
        regeneratingHealth = StartCoroutine(RegenerateHealth());
    }
    void HandleHeadbob()
    {
        if (!characterController.isGrounded)
        {
            return;
        }
        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (IsSprinting ? sprintBobSpeed : walkBobSpeed); //Turnary operator
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
        else
        {
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos,
                playerCamera.transform.localPosition.z);
        }
    }
    void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    void MovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                                   (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
            (transform.TransformDirection(Vector3.right) * currentInput.y);

        moveDirection.y = moveDirectionY;
    }
    void CameraMovement()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    public void Hit(float damage)
    {
        health =health - damage;
    }
}
