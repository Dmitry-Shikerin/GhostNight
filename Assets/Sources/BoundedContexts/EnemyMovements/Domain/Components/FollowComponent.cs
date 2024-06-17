using System;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyMovements.Domain.Components
{
    [Serializable]
    public struct FollowComponent
    {
        public Transform Target;
    }
}