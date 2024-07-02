using System;
using UnityEngine.Scripting;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Billing
{
    [Serializable]
    public class GetPurchasedProductsResponse
    {
        [field: Preserve]
        public PurchasedProduct[] purchasedProducts;
        [field: Preserve]
        public string signature;
    }
}
