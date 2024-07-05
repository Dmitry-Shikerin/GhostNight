using System.Threading;
using Cysharp.Threading.Tasks;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views;
using UnityEngine;

namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.UI.Images
{
    public interface IImageView : IView
    {
        float FillAmount { get; }
        
        void SetSprite(Sprite sprite);
        void SetFillAmount(float fillAmount);
        UniTask FillMoveTowardsAsync(float duration, CancellationToken cancellationToken);
        void ShowImage();
        void HideImage();
    }
}