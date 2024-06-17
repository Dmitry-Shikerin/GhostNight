#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

float4 _BorderColor;
float _BorderThickness;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex + float4(0, 0.1, 0, 0));
    o.uv = v.uv;
    UNITY_TRANSFER_FOG(o,o.vertex);
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    const float2 uv = i.uv * 2 - float2(1, 1);
    const float d = length(uv);
    const float radius = 1.0f;

    float4 col;
    if (d > radius) {
        col = float4(0, 0, 0, 0);
    }
    else if (d > radius - _BorderThickness) {
        col = _BorderColor;
    }
    else {
        col = GetCursorBodyColor();
    }
    return col;
}
