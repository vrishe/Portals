Shader "Unlit/Checker"
{
    Properties
    {
        _BgColor("Background Color", Color) = (1,1,1,1)
        _FgColor("Foreground Color", Color) = (0,0,0,1)

        _Size("Square Size", Float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull OFf

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            float _Size;

            #pragma vertex vert
            v2f vert (appdata v)
            {
                float3 unit = mul(unity_ObjectToWorld, float3(1, 1, 1));
                float scale = max(max(unit.x, unit.y), unit.z) / _Size;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = scale * v.uv;

                return o;
            }

            fixed4 _BgColor;
            fixed4 _FgColor;

            #pragma fragment frag
            fixed4 frag(v2f i) : SV_Target
            {
                float v = step(.5, frac(.5
                    * (floor(i.uv.x) + floor(i.uv.y))));

                return lerp(_BgColor, _FgColor, v);
            }
            ENDCG
        }
    }
}
