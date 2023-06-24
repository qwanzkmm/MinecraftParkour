using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ad : MonoBehaviour
{
    [SerializeField] private string Reward = "Reward";

    private void Start()
    {
        if (YandexSDK.YaSDK.instance.isInterstitialReady)
        {
            ShowInterstitial();
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
            Time.timeScale = 0f;
        }
    }
}
