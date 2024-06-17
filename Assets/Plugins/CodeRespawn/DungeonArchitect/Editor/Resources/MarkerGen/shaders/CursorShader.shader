Shader "DungeonArchitect/Editors/MarkerGen/Cursor"
{
    Properties
    {
        _BodyColor ("Body Color", Color) = (1, 1, 1, 1)
        _CullBodyColor ("Culled Body Color", Color) = (1, 1, 1, 1)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 0.75)
        _BorderThickness ("Border Thickness", Float) = 0.1
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            Cull off
            ZWrite Off
            ZTest Less
            Offset -1, -1 
            Blend SrcAlpha OneMinusSrcAlpha
                
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            float4 _BodyColor;
            
            float4 GetCursorBodyColor() {
                return _BodyColor;
            }
            
            #include "CursorShaderInclude.cginc"

            ENDCG
        }
        
        Pass
        {
            Cull off
            ZWrite Off
            ZTest GEqual
            Offset -1, -1 
            Blend SrcAlpha OneMinusSrcAlpha
                
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            float4 _CullBodyColor;
            
            float4 GetCursorBodyColor() {
                return _CullBodyColor;
            }
            
            #include "CursorShaderInclude.cginc"

            ENDCG
        }
    }
}
