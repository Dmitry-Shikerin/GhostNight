using System;
using UnityEngine.Scripting;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Billing
{
    [Serializable]
    public class GetProductCatalogResponse
    {
        [field: Preserve]
        public CatalogProduct[] products;
    }
}
