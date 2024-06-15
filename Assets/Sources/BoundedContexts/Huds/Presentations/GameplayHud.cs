using Sirenix.OdinInspector;
using Sources.BoundedContexts.PlayerWallets.Presentation.Implementation;
using UnityEngine;

namespace Sources.BoundedContexts.Huds.Presentations
{
    public class GameplayHud : MonoBehaviour
    {
        [FoldoutGroup("Wallet")]
        [Required] [SerializeField] private PlayerWalletView _playerWalletView;
        
        public PlayerWalletView PlayerWalletView => _playerWalletView;
    }
}