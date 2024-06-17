using DungeonArchitect.SxEngine;
using DungeonArchitect.SxEngine.Utils;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors.Grid.Actors
{
    public class SxCursorActor : SxMeshActor
    {
        private readonly SxCursorMaterial material;
        private SmoothValueVector3 animPosition;
        public bool Animating => (animPosition != null && !animPosition.HasArrived);
        
        public SxCursorActor()
        {
            SetMesh(SxMeshRegistry.Get<SxMGGroundTileMesh>());
            
            material = new SxCursorMaterial();
            SetMaterial(material);
            animPosition = new SmoothValueVector3(Vector3.zero);
            animPosition.TimeToArrive = 0.05f;
        }

        public override void Tick(SxRenderContext context, float deltaTime)
        {
            base.Tick(context, deltaTime);
            if (!animPosition.HasArrived)
            {
                animPosition.Update(deltaTime);
                Position = animPosition.Value;
            }
        }

        public void SetSmoothPosition(Vector3 pos, bool immediate = false)
        {
            if (immediate)
            {
                animPosition.Set(pos);
            }
            else
            {
                animPosition.Value = pos;
            }
        }
    }

    public class SxCursorMaterial : SxUnityResourceCopyMaterial
    {
        public SxCursorMaterial() : base("MarkerGen/materials/MatCursor")
        {
            DepthBias = -2;
        }
    }
}