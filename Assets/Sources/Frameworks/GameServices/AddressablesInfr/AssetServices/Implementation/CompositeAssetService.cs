using Cysharp.Threading.Tasks;
using Sources.Frameworks.GameServices.AddressablesInfr.AssetProviders.Interfaces;
using Sources.Frameworks.GameServices.AddressablesInfr.AssetServices.Interfaces;

namespace Sources.Frameworks.GameServices.AddressablesInfr.AssetServices.Implementation
{
    public class CompositeAssetService : ICompositeAssetService
    {
        private readonly IAssetProvider[] _assetProviders;
        
        public CompositeAssetService()
        {
            _assetProviders = new IAssetProvider[]
            {
                
            };
        }

        public async UniTask LoadAsync()
        {
            foreach (IAssetProvider assetProvider in _assetProviders)
                await assetProvider.LoadAsync();
        }

        public void Release()
        {
            foreach (var assetProvider in _assetProviders)
                assetProvider.Release();
        }
    }
}