using System;
using UnityEngine.Scripting;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Billing
{
    [Serializable]
    public class PurchaseProductResponse
    {
        [field: Preserve]
        public PurchasedProduct purchaseData;
        [field: Preserve]
        public string signature;
    }
}
