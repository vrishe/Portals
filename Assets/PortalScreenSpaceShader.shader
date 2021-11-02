Shader "Unlit/PortalScreenSpaceShader"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;

            float4 vert(appdata v) : SV_POSITION
            {
                return UnityObjectToClipPos(v.vertex);
            }

            fixed4 frag(UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
                return tex2D(_MainTex, vpos / _ScreenParams.xy);
            }
            ENDCG
        }
    }
}
