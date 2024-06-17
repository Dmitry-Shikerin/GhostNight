Shader "DungeonArchitect/Editors/MarkerGen/RuleObject"
{
    Properties
    {
        _BodyColor ("Body Color", Color) = (1, 1, 1, 1)
        _BodyHoverColor ("Body Hover Color", Color) = (1, 0.5, 0.5, 1)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 0.75)
        _BorderThickness ("Border Thickness", Float) = 0.05
        _LightDir ("Light Dir", Vector) = (-0.3, -0.8, 0.6)
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _Ambient ("Ambient", Float) = 0.1
        _Selected("Selected", Int) = 0
        _Hovered("Hovered", Int) = 0
        _Scale("Scale", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _BodyColor;
            float4 _BodyHoverColor;
            float4 _BorderColor;
            float _BorderThickness;
            float4 _LightDir;
            float4 _LightColor;
            float _Ambient;
            int _Selected;
            int _Hovered;
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
                float3 normal : TEXCOORD1;
                float3 localPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.color.xyz * 2 - float3(1, 1, 1);
                o.localPos = v.vertex;
                return o;
            }

            float2 AdjustThicknessToScale(float2 thickness, float3 normal) {
                if (abs(dot(normal, float3(1, 0, 0))) > 0.9) {
                    thickness.x /= _Scale.y;
                    thickness.y /= _Scale.z;
                }
                else if (abs(dot(normal, float3(0, 1, 0))) > 0.9) {
                    thickness.x /= _Scale.x;
                    thickness.y /= _Scale.z;
                }
                else if (abs(dot(normal, float3(0, 0, 1))) > 0.9) {
                    thickness.x /= _Scale.y;
                    thickness.y /= _Scale.x;
                }
                return thickness;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float dx = min(i.uv.x, 1 - i.uv.x);
                float dy = min(i.uv.y, 1 - i.uv.y);
                float2 d = float2(dx, dy);
                
                float4 col;
                float BorderThickness = clamp(_BorderThickness, 0, 1);
                if (_Selected != 0) {
                    BorderThickness *= 2;
                }
                
                float2 thickness = float2(BorderThickness, BorderThickness);
                thickness = AdjustThicknessToScale(thickness, i.normal);
                if (d.x < thickness.x || d.y < thickness.y) {
                    col = _BorderColor;

                    if (_Selected != 0) {
                        const int num_stripes = 5;
                        float3 pos = i.localPos * _Scale;
                        const float stripe_dist = pos.x + pos.y + pos.z;
                        const int stripe_idx = floor(stripe_dist * num_stripes);
                        if (stripe_idx % 2 == 0) {
                            col = float4(1, 0, 0, 1);
                        } 
                    }
                }
                else {
                    const float diffuse = max(0, dot(i.normal, _LightDir.xyz));
                    const float contrib = clamp(diffuse + _Ambient, 0, 1);
                    const float4 baseColor = _Hovered == 0 ? _BodyColor : _BodyHoverColor;
                    col = baseColor * _LightColor * contrib;
                    col.a = 1;
                }
                return col;
            }
            ENDCG
        }
    }
}
