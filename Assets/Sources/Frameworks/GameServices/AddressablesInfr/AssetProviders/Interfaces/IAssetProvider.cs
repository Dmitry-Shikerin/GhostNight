using Cysharp.Threading.Tasks;

namespace Sources.Frameworks.GameServices.AddressablesInfr.AssetProviders.Interfaces
{
    public interface IAssetProvider
    {
        UniTask LoadAsync();
        void Release();
    }
}