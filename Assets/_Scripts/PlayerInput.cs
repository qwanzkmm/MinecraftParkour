using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using YandexSDK;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")]
    Camera cam;
    public bool isPaused = false;
    [HideInInspector] public bool isCanPressEsc = true;
    
    [Header("Pause")]
    [SerializeField] public GameObject PauseWindow;
    [SerializeField] private GameObject SettingsWindow;
    
    
    [DllImport("__Internal")] private static extern void SetToLeaderboard(int value);
    private YandexSDKControllerCS YandexSDKControllerCs;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
        Time.timeScale = 1f;

        if (YandexPlayerPrefs.HasKey("///////////////"))
        {
            
        }
    }

    private void Start()
    {
        cam.fieldOfView = 70f;
        Debug.Log(Cursor.lockState);
        isPaused = false;
        Time.timeScale = 1f;
        PauseWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PauseWindow.SetActive(false);
        AudioListener.pause = false;
    }

    public void Pause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;

            PauseWindow.SetActive(false);
            SettingsWindow.SetActive(false);
            AudioListener.pause = false;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;

            PauseWindow.SetActive(true);
            AudioListener.pause = true;
        }
    }
}