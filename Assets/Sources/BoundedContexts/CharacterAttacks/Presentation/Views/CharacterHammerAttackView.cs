using Sources.BoundedContexts.CharacterMovements.Presentation.Views;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.BoundedContexts.CharacterAttacks.Presentation.Views
{
    public class CharacterHammerAttackView : View
    {
        [SerializeField] private Button _button;
        [SerializeField] private CharacterAnimation _characterAnimation;
        
        private void OnEnable() =>
            _button.onClick.AddListener(OnButtonClick);

        private void OnDisable() =>
            _button.onClick.RemoveListener(OnButtonClick);

        private void OnButtonClick() =>
            _characterAnimation.PlayHammerAttack();
    }
}