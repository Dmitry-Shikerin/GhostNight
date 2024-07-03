using UnityEngine;

namespace Sources.BoundedContexts.Layers.Domain.Const
{
    public static class LayerConst
    {
        public static int Enemy = 1 << LayerMask.NameToLayer("Enemy");
    }
}