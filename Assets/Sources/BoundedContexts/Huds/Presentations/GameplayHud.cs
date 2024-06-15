using Sirenix.OdinInspector;
using Sources.BoundedContexts.PlayerWallets.Presentation.Implementation;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.UI.Huds;
using Sources.Frameworks.UiFramework.Views.Presentations.Implementation;
using UnityEngine;

namespace Sources.BoundedContexts.Huds.Presentations
{
    public class GameplayHud : MonoBehaviour, IHud
    {
        [FoldoutGroup("UiFramework")]
        [Required] [SerializeField] private UiCollector _uiCollector;
        
        [FoldoutGroup("Wallet")]
        [Required] [SerializeField] private PlayerWalletView _playerWalletView;
        
        public UiCollector UiCollector => _uiCollector;

        public PlayerWalletView PlayerWalletView => _playerWalletView;
    }
}