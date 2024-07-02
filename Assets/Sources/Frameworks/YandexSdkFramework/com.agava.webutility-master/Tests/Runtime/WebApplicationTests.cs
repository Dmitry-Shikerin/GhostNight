using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Runtime;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Tests.Runtime
{
    public class WebApplicationTests
    {
        [Test]
        public void InBackgroundShouldReturnFalse()
        {
            Assert.IsFalse(WebApplication.InBackground);
        }
    }
}
