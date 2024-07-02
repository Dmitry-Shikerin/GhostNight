using System.Collections;
using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Advertising;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Tests.Runtime
{
    public class InterstitialAdTests
    {
        [UnitySetUp]
        public IEnumerator InitializeSdk()
        {
            if (!YandexGamesSdk.IsInitialized)
                yield return YandexGamesSdk.Initialize(SdkTests.TrackSuccessCallback);
        }

        [UnityTest]
        public IEnumerator ShowShouldInvokeErrorCallback()
        {
            bool callbackInvoked = false;
            InterstitialAd.Show(onErrorCallback: (message) =>
            {
                callbackInvoked = true;
            });

            yield return new WaitForSecondsRealtime(1);

            Assert.IsTrue(callbackInvoked);
        }
    }
}
