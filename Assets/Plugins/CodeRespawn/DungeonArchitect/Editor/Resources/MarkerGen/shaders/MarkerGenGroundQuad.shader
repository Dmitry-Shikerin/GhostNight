Shader "DungeonArchitect/Editors/MarkerGen/GroundQuad"
{
    Properties
    {
        _BodyColor ("Body Color", Color) = (1, 1, 1, 1)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 0.75)
        _BorderThickness ("Border Thickness", Float) = 0.05
        _Scale("Scale", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _BodyColor;
            float4 _BorderColor;
            float _BorderThickness;
            float4 _Scale;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localPos = v.vertex;
                return o;
            }

            float2 AdjustThicknessToScale(float2 thickness) {
                thickness.x /= _Scale.x;
                thickness.y /= _Scale.z;
                return thickness;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float dx = min(i.uv.x, 1 - i.uv.x);
                float dy = min(i.uv.y, 1 - i.uv.y);
                float2 d = float2(dx, dy);
                
                const float BorderThickness = clamp(_BorderThickness, 0, 1);
                float2 thickness = float2(BorderThickness, BorderThickness);
                thickness = AdjustThicknessToScale(thickness);
                return (d.x < thickness.x || d.y < thickness.y) ? _BorderColor : _BodyColor;
            }
            ENDCG
        }
    }
}
