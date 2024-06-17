using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.SxEngine;
using DungeonArchitect.SxEngine.Utils;
using DungeonArchitect.Utils;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors.Grid.Actors
{
    public class SxGridPatternRuleActor : SxMeshActor
    {
        private readonly SxGridRuleObjectMaterial material;
        private bool selected = false;
        private bool hovered = false;
        private SxTextComponent textComponent1;
        private SxTextComponent textComponent2;
        private SmoothValueVector3 animPosition;
        private SmoothValueVector3 animScale;

        public GridMarkerGenRule Rule { get; private set; }
        public float EdgeSize { get; private set; }
        public float TileSize { get; private set; }
        
        public SxGridPatternRuleActor()
        {
            SetMesh(new SxCubeBaseMesh());
            
            material = new SxGridRuleObjectMaterial();
            SetMaterial(material);
            UpdateMaterialParameters();
        }

        public void Initialize(GridMarkerGenRule rule, float tileSize, float edgeSize)
        {
            Rule = rule;
            TileSize = tileSize;
            EdgeSize = edgeSize;
            selected = false;
            animPosition = new SmoothValueVector3(Position);
            animScale = new SmoothValueVector3(Scale);

            var textSettings = new SxTextComponentSettings()
            {
                Font = Resources.Load<Font>("MarkerGen/fonts/roboto/RobotoCondensed-Bold"),
                Color = Color.black,
                Scale = 0.10f,
                HAlign = SxTextHAlign.Center,
                VAlign = SxTextVAlign.Center,
                DepthBias = -2,
                CustomMaterial = new SxGridRuleTextMaterial(),
                WordWrap = true,
                WordWrapWidth = 0.9f,
                WordWrapHeight = 0.9f
            };

            textComponent1 = AddComponent<SxTextComponent>();
            textComponent1.Initialize(textSettings);
            
            textComponent2 = AddComponent<SxTextComponent>();
            textComponent2.Initialize(textSettings);

            UpdateRuleText();
            UpdateTransform(false);
            UpdateMaterial();
        }

        public void UpdateMaterial()
        {
            if (material != null && Rule != null)
            {
                var color = Rule.color;
                if (!Rule.visuallyDominant)
                {
                    color = GridMarkerGenEditorUtils.CreatePaleColor(color);
                }
                material.SetBodyColor(color);
                material.SetBodyHoverColor(GridMarkerGenEditorUtils.CreateHoverColor(color));
            }
        }

        public bool Animating => (animPosition != null && !animPosition.HasArrived) || (animScale != null && !animScale.HasArrived);
        
        public override void Tick(SxRenderContext context, float deltaTime)
        {
            base.Tick(context, deltaTime);

            if (Animating)
            {
                animPosition.Update(deltaTime);
                animScale.Update(deltaTime);
                ApplyTransform();
            }
        }

        public void UpdateRuleText()
        {
            var lines = new SxTextLineList();
            if (Rule != null)
            {
                if (Rule.previewTextCondition.Trim().Length > 0)
                {
                    lines.AddLine("CONDITION:", 0.6f);
                    lines.AddLine(Rule.previewTextCondition);
                }

                if (Rule.previewTextActions.Length > 0)
                {
                    lines.AddLine("ACTIONS:", 0.6f, 30);
                    foreach (var actionText in Rule.previewTextActions)
                    {
                        lines.AddLine(actionText);
                    }
                }
            }
            textComponent1.SetLines(lines);
            textComponent2.SetLines(lines);
        }
        
        float GetHeightScale()
        {
            if (Rule == null) return 1;
            switch (Rule.ruleType)
            {
                case GridMarkerGenRuleType.Ground:
                    return TileSize * 0.1f;
                
                case GridMarkerGenRuleType.EdgeX:
                case GridMarkerGenRuleType.EdgeZ:
                case GridMarkerGenRuleType.Corner:
                    return Rule.visuallyDominant ? TileSize : TileSize * 0.3f;
                
                default:
                    return 1;
                    
            }
        }

        public void UpdateTransform(bool animate)
        {
            if (Rule == null) return;
            
            float offset = TileSize + EdgeSize;
            float padding = TileSize / 10.0f;
            
            var position2D = Vector2.zero;
            var scale2D = Vector2.one;

            if (Rule.ruleType == GridMarkerGenRuleType.Ground)
            {
                var coord2D = MathUtils.ToVector2(Rule.coord); 
                position2D = coord2D * offset;
                scale2D = new Vector2(TileSize, TileSize);
            }
            else if (Rule.ruleType == GridMarkerGenRuleType.EdgeX)
            {
                var coord2D = MathUtils.ToVector2(Rule.coord) + new Vector2(0, -0.5f); 
                position2D = coord2D * offset;
                scale2D = new Vector2(TileSize, EdgeSize);
            }
            else if (Rule.ruleType == GridMarkerGenRuleType.EdgeZ)
            {
                var coord2D = MathUtils.ToVector2(Rule.coord) + new Vector2(-0.5f, 0); 
                position2D = coord2D * offset;
                scale2D = new Vector2(EdgeSize, TileSize);
            }
            else if (Rule.ruleType == GridMarkerGenRuleType.Corner)
            {
                var coord2D = MathUtils.ToVector2(Rule.coord) + new Vector2(-0.5f, -0.5f); 
                position2D = coord2D * offset;
                scale2D = new Vector2(EdgeSize, EdgeSize);
            }

            var position = new Vector3(position2D.x, 0, position2D.y);
            var scale = new Vector3(scale2D.x - padding, GetHeightScale(), scale2D.y - padding);

            if (position != Position || Scale != scale)
            {
                if (animate)
                {
                    animPosition.Value = position;
                    animScale.Value = scale;
                }
                else
                {
                    animPosition.Set(position);
                    animScale.Set(scale);
                    ApplyTransform();
                }
            }
            
            UpdateTextTransform();
        }

        private void ApplyTransform()
        {
            WorldTransform = new SxTransform(animPosition.Value, Quaternion.identity, animScale.Value);
            material.SetScale(Scale);
        }

        private void UpdateTextTransform()
        {
            if (Rule.visuallyDominant)
            {
                if (Rule.ruleType == GridMarkerGenRuleType.Ground)
                {
                    textComponent1.Visible = true;
                    textComponent2.Visible = false;
                    textComponent1.RelativeTransform = Matrix4x4.TRS(new Vector3(0, 1.01f, 0), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), Vector3.one);
                }
                else if (Rule.ruleType == GridMarkerGenRuleType.EdgeX)
                {
                    textComponent1.Visible = true;
                    textComponent2.Visible = true;
                    textComponent1.RelativeTransform = Matrix4x4.TRS(new Vector3(0, 0.5f, -0.51f), Quaternion.AngleAxis(0, Vector3.up), Vector3.one);
                    textComponent2.RelativeTransform = Matrix4x4.TRS(new Vector3(0, 0.5f, 0.51f), Quaternion.AngleAxis(180, Vector3.up), Vector3.one);
                } 
                else if (Rule.ruleType == GridMarkerGenRuleType.EdgeZ)
                {
                    textComponent1.Visible = true;
                    textComponent2.Visible = true;
                    textComponent1.RelativeTransform = Matrix4x4.TRS(new Vector3(-0.51f, 0.5f, 0), Quaternion.AngleAxis(90, Vector3.up), Vector3.one);
                    textComponent2.RelativeTransform = Matrix4x4.TRS(new Vector3(0.51f, 0.5f, 0), Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
                } 
                else if (Rule.ruleType == GridMarkerGenRuleType.Corner)
                {
                    textComponent1.Visible = false;
                    textComponent2.Visible = false;
                } 
            }
            else
            {
                // Not visually dominant and should fade into the background (not get in the way).  Hide the text on everything except the ground tiles
                if (Rule.ruleType == GridMarkerGenRuleType.Ground)
                {
                    textComponent1.Visible = true;
                    textComponent2.Visible = false;
                    textComponent1.RelativeTransform = Matrix4x4.TRS(new Vector3(0, 1.01f, 0), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), Vector3.one);
                }
                else
                {
                    textComponent1.Visible = false;
                    textComponent2.Visible = false;
                }
            }
        }

        public Bounds GetBounds()
        {
            var min = Position + Vector3.Scale(new Vector3(-0.5f, 0, -0.5f), Scale);
            var max = Position + Vector3.Scale(new Vector3(0.5f, 1, 0.5f), Scale);
            return new Bounds((min + max) * 0.5f, max - min);
        }

        public void SetHovered(bool newHoveredState)
        {
            if (hovered != newHoveredState)
            {
                hovered = newHoveredState;
                material.SetHovered(hovered);
            }
            
        }
        
        public void SetSelected(bool newSelectState)
        {
            if (selected != newSelectState)
            {
                selected = newSelectState;
                material.SetSelected(selected);
            }
        }

        private void UpdateMaterialParameters()
        {
            material.SetSelected(selected);
            material.SetHovered(hovered);
            material.SetScale(Scale);
        }
    }

    
    public class SxGridRuleObjectMaterial : SxUnityResourceCopyMaterial
    {
        private readonly int idxSelected;
        private readonly int idxHovered;
        private readonly int idxScale;
        private readonly int idxBodyColor;
        private readonly int idxBodyHoverColor;
        
        public SxGridRuleObjectMaterial() : base("MarkerGen/materials/MatRuleObject")
        {
            DepthBias = -2;
            idxSelected = Shader.PropertyToID("_Selected");
            idxHovered = Shader.PropertyToID("_Hovered");
            idxScale = Shader.PropertyToID("_Scale");
            idxBodyColor = Shader.PropertyToID("_BodyColor");
            idxBodyHoverColor = Shader.PropertyToID("_BodyHoverColor");
        }
        
        public void SetSelected(bool selected)
        {
            UnityMaterial.SetInt(idxSelected, selected ? 1 : 0);
        }
        
        public void SetHovered(bool hovered)
        {
            UnityMaterial.SetInt(idxHovered, hovered ? 1 : 0);
        }

        public void SetScale(Vector3 scale)
        {
            UnityMaterial.SetVector(idxScale, scale);
        }
        
        public void SetBodyColor(Color color)
        {
            UnityMaterial.SetColor(idxBodyColor, color);
        }
        
        public void SetBodyHoverColor(Color color)
        {
            UnityMaterial.SetColor(idxBodyHoverColor, color);
        }
    }

    
    public class SxGridRuleTextMaterial : SxUnityResourceCopyMaterial
    {
        public SxGridRuleTextMaterial() : base("MarkerGen/materials/MatRuleText")
        {
            DepthBias = -2;
        }
     
    }
    
}