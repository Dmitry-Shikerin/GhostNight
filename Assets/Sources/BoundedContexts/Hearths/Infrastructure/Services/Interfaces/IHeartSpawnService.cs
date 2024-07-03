using Sources.BoundedContexts.Hearths.Presentation.Views.Interfaces;
using UnityEngine;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Services.Interfaces
{
    public interface IHeartSpawnService
    {
        IHearthView Spawn(Vector3 position);
    }
}