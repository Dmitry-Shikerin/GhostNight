using System.Collections.Generic;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.UI.Images;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.UI.Images;
using UnityEngine;

namespace Sources.BoundedContexts.Healths.Presentation.Implementation
{
    public class HealthView : View
    {
        [SerializeField] private List<ImageView> _images;
        
        public IReadOnlyList<IImageView> Images => _images;
    }
}