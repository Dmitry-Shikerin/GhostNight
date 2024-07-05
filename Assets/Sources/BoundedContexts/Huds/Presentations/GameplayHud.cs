using Sirenix.OdinInspector;
using Sources.BoundedContexts.Healths.Presentation.Implementation;
using Sources.BoundedContexts.PlayerWallets.Presentation.Implementation;
using UnityEngine;

namespace Sources.BoundedContexts.Huds.Presentations
{
    public class GameplayHud : MonoBehaviour
    {
        [FoldoutGroup("Wallet")]
        [Required] [SerializeField] private PlayerWalletView _playerWalletView;
        
        [FoldoutGroup("Health")]
        [Required] [SerializeField] private HealthView _characterHealthView;
        
        public PlayerWalletView PlayerWalletView => _playerWalletView;
        
        public HealthView CharacterHealthView => _characterHealthView;
    }
}