using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Assets.Scripts;

public class YandexSDKControllerCS : MonoBehaviour
{
    public static YandexSDKControllerCS instance;
    [DllImport("__Internal")] private static extern void GetTypePlatformDevice();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    
    private void Start()
    {
        //GetPlatformDevice();
    }
    
    public DeviceTypeWEB CurrentDeviceType { get; private set; }// = DeviceTypeWEB.Mobile;

    public void SetTargetDeviceType(string typeDevice)
    {
        switch (typeDevice)
        {
            case "desktop": CurrentDeviceType = DeviceTypeWEB.Desktop; break;
            case "mobile": CurrentDeviceType = DeviceTypeWEB.Mobile; break;

            default: CurrentDeviceType = DeviceTypeWEB.Mobile; break;
        }
    }
    

    public void GetPlatformDevice() => GetTypePlatformDevice();
}