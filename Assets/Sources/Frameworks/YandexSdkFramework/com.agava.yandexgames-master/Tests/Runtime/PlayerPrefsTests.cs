using NUnit.Framework;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Utility;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Tests.Runtime
{
    public class PlayerPrefsTests
    {
        [Test]
        public void ShouldNotThrowOnEmptyData()
        {
            Assert.DoesNotThrow(() => PlayerPrefs.ParseAndApplyData(""));
        }

        [Test]
        public void ShouldNotCorruptData()
        {
            // {"abc":"1232","s":"5","somestring":"herpthederp"}
            PlayerPrefs.ParseAndApplyData("{\"abc\":\"1232\",\"s\":\"5\",\"somestring\":\"herpthederp\"}");
            Assert.AreEqual(PlayerPrefs.GetInt("abc"), 1232);
            Assert.AreEqual(PlayerPrefs.GetFloat("s"), 5f);
            Assert.AreEqual(PlayerPrefs.GetString("somestring"), "herpthederp");
        }
    }
}
