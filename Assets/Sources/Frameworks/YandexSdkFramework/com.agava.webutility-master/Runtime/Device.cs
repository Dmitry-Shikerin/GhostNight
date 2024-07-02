using System.Runtime.InteropServices;
using UnityEngine;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Runtime
{
    public static class Device
    {
        public static bool IsMobile
        {
            get
            {
                if (WebApplication.IsRunningOnWebGL)
                    return GetDeviceIsMobile();

                return SystemInfo.deviceType == DeviceType.Handheld;
            }
        }

        [DllImport("__Internal")]
        private static extern bool GetDeviceIsMobile();
    }
}
