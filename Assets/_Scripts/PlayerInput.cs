using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")]
    Camera cam;
    public bool isPaused = false;
    [HideInInspector] public bool isCanPressEsc = true;
    
    [Header("Pause")]
    [SerializeField] public GameObject PauseWindow;
    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject MobileCameraManager;
    
    
    [DllImport("__Internal")] private static extern void SetToLeaderboard(int value);

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 70f;
        
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        isPaused = false;
        PauseWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isCanPressEsc)
        {
            Pause();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Ad ad = FindObjectOfType<Ad>();
            ad.ShowRewarded();
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
#if !UNITY_EDITOR
        if(YandexSDKControllerCS.instance.CurrentDeviceType == Assets.Scripts.DeviceTypeWEB.Desktop)    
            Cursor.lockState = CursorLockMode.Locked;
        else 
            MobileCameraManager.SetActive(true);
#else 
        Cursor.lockState = CursorLockMode.Locked;
#endif

        PauseWindow.SetActive(false);
        SettingsWindow.SetActive(false);
        AudioListener.pause = false;
    }
    public void Pause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            
#if !UNITY_EDITOR
            if(YandexSDKControllerCS.instance.CurrentDeviceType == Assets.Scripts.DeviceTypeWEB.Desktop)    
                Cursor.lockState = CursorLockMode.Locked;
            else
                MobileCameraManager.SetActive(true);
#else 
            Cursor.lockState = CursorLockMode.Locked;
#endif

            PauseWindow.SetActive(false);
            SettingsWindow.SetActive(false);
            AudioListener.pause = false;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;

#if !UNITY_EDITOR
            if(YandexSDKControllerCS.instance.CurrentDeviceType == Assets.Scripts.DeviceTypeWEB.Desktop)    
                Cursor.lockState = CursorLockMode.Confined;
            else
                MobileCameraManager.SetActive(false);
#else 
            Cursor.lockState = CursorLockMode.Confined;
#endif

            PauseWindow.SetActive(true);
            AudioListener.pause = true;
        }
    }
}