using System;
using UnityEngine.Scripting;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Billing
{
    [Serializable]
    public class PurchasedProduct
    {
        [field: Preserve]
        public string developerPayload;
        [field: Preserve]
        public string productID;
        [field: Preserve]
        public string purchaseTime;
        [field: Preserve]
        public string purchaseToken;
    }
}
