﻿using Sources.BoundedContexts.Tutorials.Services.Implementation;
using Sources.BoundedContexts.Tutorials.Services.Interfaces;
using Sources.Frameworks.GameServices.Linecasts.Implementation;
using Sources.Frameworks.GameServices.Linecasts.Interfaces;
using Sources.Frameworks.GameServices.Overlaps.Implementation;
using Sources.Frameworks.GameServices.Overlaps.Interfaces;
using Sources.Frameworks.GameServices.Prefabs.Implementation;
using Sources.Frameworks.GameServices.Prefabs.Interfaces;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITutorialService>().To<TutorialService>().AsSingle();
            Container.Bind<IOverlapService>().To<OverlapService>().AsSingle();
            Container.Bind<ILinecastService>().To<LinecastService>().AsSingle();
            Container.Bind<IPrefabLoader>().To<ResourcesPrefabLoader>().AsSingle();
        }
    }
}