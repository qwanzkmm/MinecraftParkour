using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class YandexSDKControllerCS : MonoBehaviour
{
    public static YandexSDKControllerCS instance;
    [DllImport("__Internal")] private static extern void GetTypePlayformDevice();

    
    public enum DeviceTypeWeb
    {
        Mobile,
        Desktop
    }
    
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
        GetPlatformDevice();
    }
    
    public DeviceTypeWeb CurrentDeviceType { get; private set; }// = DeviceTypeWEB.Mobile;

    public void SetTargetDeviceType(string typeDevice)
    {
        switch (typeDevice)
        {
            case "desktop": CurrentDeviceType = DeviceTypeWeb.Desktop; break;
            case "mobile": CurrentDeviceType = DeviceTypeWeb.Mobile; break;

            default: CurrentDeviceType = DeviceTypeWeb.Mobile; break;
        }
    }
    

    public void GetPlatformDevice() => GetTypePlayformDevice();
}