using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YandexSDK;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text skipLvlText;

    private void Start()
    {
        skipLvlText.text = YandexSDK.YaSDK.instance.currentPlatform == Platform.desktop ? "Пропустить уровень [R]" : "Пропустить уровень";
    }
}
