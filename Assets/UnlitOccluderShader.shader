Shader "Unlit/Occluder"
{
    SubShader
    {
        //ColorMask 0
        Cull      Off
        ZWrite    On

        Tags 
        {
            "Queue" = "Geometry+1" 
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            float4 vert(appdata v) : SV_POSITION
            {
                float3 c = 2 * v.vertex;
                float3 p = UnityObjectToViewPos(v.vertex);
                float3 n = UnityObjectToViewPos(v.normal);

                n /= length(n);
                c.z = -((n.x*(p.x-c.x) + n.y*(p.y-c.y)) / n.z + p.z);

                return float4(c.x, c.y, c.z, 1);
            }

            fixed4 frag(UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
                return fixed4(0, 1, 0, 1);
            }
            ENDCG
        }
    }
}