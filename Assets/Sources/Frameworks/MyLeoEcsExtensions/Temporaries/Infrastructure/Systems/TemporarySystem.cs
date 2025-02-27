﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.Frameworks.MyLeoEcsExtensions.Temporaries.Domain.Components.Interfaces;
using UnityEngine;

namespace Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Infrastructure.Systems
{
    public class TemporarySystem<T> : IEcsRunSystem
        where T : struct, ITemporaryComponent
    {
        private readonly EcsFilterInject<Inc<T>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref T component = ref _filter.Pools.Inc1.Get(entity);

                component.CurrentTime += Time.deltaTime;

                if (component.CurrentTime < component.Duration)
                    continue;
                
                _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}