using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Runtime;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Tests.Runtime
{
    public class AdBlockTests
    {
        [Test]
        public void AdBlockEnabledShouldNotThrow()
        {
            Assert.DoesNotThrow(() =>
            {
                UnityEngine.Debug.Log($"{nameof(AdBlock)}.{nameof(AdBlock.Enabled)} = {AdBlock.Enabled}");
            });
        }
    }
}
