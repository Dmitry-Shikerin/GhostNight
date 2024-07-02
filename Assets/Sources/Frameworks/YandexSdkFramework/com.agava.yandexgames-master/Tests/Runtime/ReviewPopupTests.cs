using System.Collections;
using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Tools;
using UnityEngine.TestTools;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Tests.Runtime
{
    public class ReviewPopupTests
    {
        [UnitySetUp]
        public IEnumerator InitializeSdk()
        {
            if (!YandexGamesSdk.IsInitialized)
                yield return YandexGamesSdk.Initialize(SdkTests.TrackSuccessCallback);
        }

        [Test]
        public void CanReviewShouldNotThrow()
        {
            Assert.DoesNotThrow(() => ReviewPopup.CanOpen((result, reason) => { }));
        }

        [Test]
        public void ReviewShouldNotThrow()
        {
            Assert.DoesNotThrow(() => ReviewPopup.Open());
        }
    }
}
