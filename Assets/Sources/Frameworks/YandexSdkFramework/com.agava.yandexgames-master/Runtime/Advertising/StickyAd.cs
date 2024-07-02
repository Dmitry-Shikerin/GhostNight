using System.Runtime.InteropServices;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Advertising
{
    /// <summary>
    /// Proxy for ysdk.adv.showBannerAdv() and ysdk.adv.hideBannerAdv() methods in YandexGames SDK.
    /// </summary>
    public static class StickyAd
    {
        public static void Show() => StickyAdShow();

        [DllImport("__Internal")]
        private static extern void StickyAdShow();

        public static void Hide() => StickyAdHide();

        [DllImport("__Internal")]
        private static extern void StickyAdHide();
    }
}
