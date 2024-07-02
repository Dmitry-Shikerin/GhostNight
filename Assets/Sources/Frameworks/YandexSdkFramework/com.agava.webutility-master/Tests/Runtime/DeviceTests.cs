using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Runtime;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Tests.Runtime
{
    public class DeviceTests
    {
        [Test]
        public void IsMobileShouldReturnFalse()
        {
            Assert.IsFalse(Device.IsMobile);
        }
    }
}
