using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMobileController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Transform cam;
    private PlayerController playerController;
    private PlayerInput playerInput;

    private bool isPressed;

    float rotationX = 0;
    float lookXLimit = 89.0f;
    float Lookhorizontal;
    float Lookvertical;

    Quaternion LastRotation;

    void Start()
    {
        cam = FindObjectOfType<Camera>().transform;
        playerController = FindObjectOfType<PlayerController>();
        playerInput = playerController.GetComponentInChildren<PlayerInput>();
    }

    void Update()
    {
        rotationX += Lookvertical * playerController.lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
        LastRotation = Quaternion.Euler(rotationX, 0, 0);
        playerController.transform.rotation *= Quaternion.Euler(0, Lookhorizontal * playerController.lookSpeed, 0);

        if (isPressed)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Moved)
                {
                    Lookvertical = touch.deltaPosition.y / 20;
                    Lookhorizontal = -touch.deltaPosition.x / 20;
                }
                if (touch.phase == TouchPhase.Stationary)
                {
                    Lookvertical = 0;
                    Lookhorizontal = 0;
                }
            }
        }
        else
        {
            Lookvertical = 0;
            Lookhorizontal = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            isPressed = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
