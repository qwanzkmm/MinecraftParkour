using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isHold = false;
    public bool isContinueButton = false;
    public bool isMenuButton = false;
    public bool isJumpButton = false;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
    }


    public void OnPointerDown(PointerEventData pointerEventData)
    {
        isHold = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isHold = false;
    }
    
    private void Update()
    {
        if (isHold && isContinueButton)
        {
            playerInput.ContinueGame();
        }


        if (isHold && isMenuButton)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.SetScene(0);
        }

        if (isHold && isJumpButton)
        {
            playerController.Jump();
        }
    }

    void OnDisable()
    {
        isHold = false;
    }
}