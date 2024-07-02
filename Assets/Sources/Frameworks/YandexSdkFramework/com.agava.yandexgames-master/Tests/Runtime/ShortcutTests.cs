using System.Collections;
using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Tools;
using UnityEngine.TestTools;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Tests.Runtime
{
    public class ShortcutTests
    {
        [UnitySetUp]
        public IEnumerator InitializeSdk()
        {
            if (!YandexGamesSdk.IsInitialized)
                yield return YandexGamesSdk.Initialize(SdkTests.TrackSuccessCallback);
        }

        [Test]
        public void CanSuggestShouldNotThrow()
        {
            Assert.DoesNotThrow(() => Shortcut.CanSuggest(result => {}));
        }

        [Test]
        public void SuggestShouldNotThrow()
        {
            Assert.DoesNotThrow(() => Shortcut.Suggest());
        }
    }
}
