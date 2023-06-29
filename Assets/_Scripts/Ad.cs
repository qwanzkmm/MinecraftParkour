using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ad : MonoBehaviour
{
    [SerializeField] private string Reward = "Reward";

    private void Start()
    {
        if (YandexSDK.YaSDK.instance.isInterstitialReady)
        {
            ShowInterstitial();
        }
        else
        {
            AudioListener.pause = false;
        }
    }

    private void OnEnable()
    {
        YandexSDK.YaSDK.onRewardedAdReward += UserGotReward;
    }

    private void OnDisable()
    {
        YandexSDK.YaSDK.onRewardedAdReward -= UserGotReward;
    }

    public void ShowRewarded()
    {
        YandexSDK.YaSDK.instance.ShowRewarded(Reward);

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.isCanPressEsc = false;
        playerInput.isPaused = true;

        Cursor.lockState = CursorLockMode.Confined;
        
        AudioListener.pause = true;
        Time.timeScale = 0f;
    }

    public void ShowInterstitial()
    {
        YandexSDK.YaSDK.instance.ShowInterstitial();

        AudioListener.pause = true;
        Time.timeScale = 0f;
    }

    public void UserGotReward(string reward)
    {
        if (this.Reward == reward)
        {
            PlayerInput playerInput = FindObjectOfType<PlayerInput>();
            playerInput.isCanPressEsc = true;
            playerInput.isPaused = false;
            
            YandexPlayerPrefs.SetBool($"isLevel{SceneManager.GetActiveScene().buildIndex}Passed", true);
        }
    }
}
