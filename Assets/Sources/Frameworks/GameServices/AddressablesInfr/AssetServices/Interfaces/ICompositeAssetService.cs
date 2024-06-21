using Cysharp.Threading.Tasks;

namespace Sources.Frameworks.GameServices.AddressablesInfr.AssetServices.Interfaces
{
    public interface ICompositeAssetService
    {
        UniTask LoadAsync();
        void Release();
    }
}