using System.Collections;
using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime;
using UnityEngine.TestTools;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Tests.Runtime
{
    public class SdkTests
    {
        private static bool s_isInitializationSuccessCallbackReceived = false;

        [UnitySetUp]
        public IEnumerator InitializeSdk()
        {
            if (!YandexGamesSdk.IsInitialized)
                yield return YandexGamesSdk.Initialize(TrackSuccessCallback);
        }

        [Test]
        public void ShouldReturnEnvironment()
        {
            Assert.IsNotEmpty(YandexGamesSdk.Environment.browser.lang);
        }

        [Test]
        public void ShouldReceiveInitializationSuccessCallback()
        {
            Assert.IsTrue(s_isInitializationSuccessCallbackReceived);
        }

        [Test]
        public void IsRunningOnYandexShouldReturnFalse()
        {
            Assert.IsFalse(YandexGamesSdk.IsRunningOnYandex);
        }

        public static void TrackSuccessCallback()
        {
            s_isInitializationSuccessCallbackReceived = true;
        }
    }
}
