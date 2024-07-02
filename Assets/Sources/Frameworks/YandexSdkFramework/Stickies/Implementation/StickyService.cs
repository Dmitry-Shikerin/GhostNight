using Sources.Frameworks.YandexSdkFramework.com.agava.webutility_master.Runtime;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Advertising;
using Sources.Frameworks.YandexSdkFramework.Stickies.Interfaces;

namespace Sources.Frameworks.YandexSdkFramework.Stickies.Implementation
{
    public class StickyService : IStickyService
    {
        public void ShowSticky()
        {
            if (WebApplication.IsRunningOnWebGL == false)
                return;

            StickyAd.Show();
        }
    }
}