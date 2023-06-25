using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingPassedLevels : MonoBehaviour
{

    [HideInInspector] public bool isLevel1Passed = false;
    [HideInInspector] public bool isLevel2Passed = false;
    [HideInInspector] public bool isLevel3Passed = false;
    [HideInInspector] public bool isLevel4Passed = false;
    [HideInInspector] public bool isLevel5Passed = false;
    [HideInInspector] public bool isLevel6Passed = false;
    [HideInInspector] public bool isLevel7Passed = false;
    [HideInInspector] public bool isLevel8Passed = false;
    [HideInInspector] public bool isLevel9Passed = false;

    public GameObject _2LevelLocker;
    public GameObject _3LevelLocker;
    public GameObject _4LevelLocker;
    public GameObject _5LevelLocker;
    public GameObject _6LevelLocker;
    public GameObject _7LevelLocker;
    public GameObject _8LevelLocker;
    public GameObject _9LevelLocker;
    public GameObject _10LevelLocker;

    private void Start()
    {
        if (YandexPlayerPrefs.HasKey("isLevel1Passed"))
        {
            isLevel1Passed = YandexPlayerPrefs.GetBool("isLevel1Passed");
            if (isLevel1Passed) { _2LevelLocker.SetActive(false); } 
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel2Passed"))
        {
            isLevel2Passed = YandexPlayerPrefs.GetBool("isLevel2Passed");
            if (isLevel2Passed) { _3LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel3Passed"))
        {
            isLevel3Passed = YandexPlayerPrefs.GetBool("isLevel3Passed");
            if (isLevel3Passed) { _4LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel4Passed"))
        {
            isLevel4Passed = YandexPlayerPrefs.GetBool("isLevel4Passed");
            if (isLevel4Passed) { _5LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel5Passed"))
        {
            isLevel5Passed = YandexPlayerPrefs.GetBool("isLevel5Passed");
            if (isLevel5Passed) { _6LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel6Passed"))
        {
            isLevel6Passed = YandexPlayerPrefs.GetBool("isLevel6Passed");
            if (isLevel6Passed) { _7LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel7Passed"))
        {
            isLevel7Passed = YandexPlayerPrefs.GetBool("isLevel7Passed");
            if (isLevel7Passed) { _8LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel8Passed"))
        {
            isLevel8Passed = YandexPlayerPrefs.GetBool("isLevel8Passed");
            if (isLevel8Passed) { _9LevelLocker.SetActive(false); }
        }
        
        if (YandexPlayerPrefs.HasKey("isLevel9Passed"))
        {
            isLevel9Passed = YandexPlayerPrefs.GetBool("isLevel9Passed");
            if (isLevel9Passed) { _10LevelLocker.SetActive(false); }
        }
    }
}
