using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("PlayerController")]
    [SerializeField] public Transform Camera;
    [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
    [Range(0.1f, 5)] public float CroughSpeed = 1.0f;
    [SerializeField, Range(2, 20)] float RuningSpeed = 4.0f;
    [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
    [Range(0.5f, 10)] public float lookSpeed = 2.0f;
    [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;

    [Space(20)]
    [Header("Advance")]
    [SerializeField] float CroughHeight = 1.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float timeToRunning = 2.0f;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool CanRunning = true;


    [Space(20)]
    [Header("Input")]
    [SerializeField] KeyCode CroughKey = KeyCode.LeftControl;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    bool isCrough = false;
    float InstallCroughHeight;
    float rotationX = 0;
    [HideInInspector] public bool isRunning = false;
    Vector3 InstallCameraMovement;
    float InstallFOV;
    Camera cam;
    [HideInInspector] public bool Moving;
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float Lookvertical;
    [HideInInspector] public float Lookhorizontal;
    float RunningValue;
    float installGravity;
    bool WallDistance;
    [HideInInspector] public float WalkingValue;
    
    private PlayerInput playerInput;

    [Space(10)]
    [Header("Joystick")]
    public FixedJoystick fixedJoystick;
    public float horizontalFixedJoystick;
    public float verticalFixedJoystick;
    public float lerpMult;
    private bool isOnPs = true;
    public GameObject PhoneCanvas;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        
        InstallCroughHeight = characterController.height;
        InstallCameraMovement = Camera.localPosition;
        InstallFOV = cam.fieldOfView;
        RunningValue = RuningSpeed;
        installGravity = gravity;
        WalkingValue = walkingSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        

        playerInput = GetComponentInChildren<PlayerInput>();

#if UNITY_EDITOR

        isOnPs = true;
#else
        if(YandexSDKControllerCS.instance.CurrentDeviceType ==  YandexSDKControllerCS.DeviceTypeWeb.Desktop)
        {
            isOnPs = true;
            PhoneCanvas.SetActive(false);
        }
        else
        {
            isOnPs = false;
            PhoneCanvas.SetActive(true);
        }
#endif
    }

    void Update()
    {
        if(isOnPs)
        {
            RaycastHit CroughCheck;

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftControl) : false : false;
            //cam.fieldOfView = isRunning ? 90f : 70f; 
            if (isRunning)
            {
                StartRunningFOV();
            }
            else
            {
                EndRunningFOV();
            } 
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
            horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Horizontal") : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) + (right * horizontal);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;


            if (canMove && playerInput.isPaused == false)
            {
                Lookvertical = -Input.GetAxis("Mouse Y");
                Lookhorizontal = Input.GetAxis("Mouse X");

                rotationX += Lookvertical * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);
            }

            if (Input.GetKey(CroughKey))
            {
                isCrough = true;
                float Height = Mathf.Lerp(characterController.height, CroughHeight, 5 * Time.deltaTime);
                characterController.height = Height;
                WalkingValue = Mathf.Lerp(WalkingValue, CroughSpeed, 6 * Time.deltaTime);
            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (characterController.height != InstallCroughHeight)
                {
                    isCrough = false;
                    float Height = Mathf.Lerp(characterController.height, InstallCroughHeight, 6 * Time.deltaTime);
                    characterController.height = Height;
                    WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
                }
            }
        }
        else
        {
            horizontalFixedJoystick = Mathf.Lerp(horizontalFixedJoystick, fixedJoystick.Horizontal, Time.deltaTime * lerpMult);
            verticalFixedJoystick = Mathf.Lerp(verticalFixedJoystick, fixedJoystick.Vertical, Time.deltaTime * lerpMult);

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * verticalFixedJoystick : 0;
            horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * horizontalFixedJoystick : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) + (right * horizontal);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;

        }
    }
    
    
    public void Jump()
    {
        if(canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            float movementDirectionY = moveDirection.y;
            moveDirection.y = movementDirectionY;
        }
    }
    
    public void StartRunningFOV()
    {
        float timer = 0;
        float timeToLoad = 1f;

        if (cam.fieldOfView <= 80f)
        {
            timer += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(timer / timeToLoad);
            cam.fieldOfView += fillAmount * 75f;

            if (cam.fieldOfView > 80)
            {
                cam.fieldOfView = 80f;
            }
        }
    }

    public void EndRunningFOV()
    {
        float timer = 0;
        float timeToLoad = 1f;

        if (cam.fieldOfView >= 70f)
        {
            timer += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(timer / timeToLoad);
            cam.fieldOfView -= fillAmount * 75f;

            if (cam.fieldOfView < 70)
            {
                cam.fieldOfView = 70f;
            }
        }
    }
}