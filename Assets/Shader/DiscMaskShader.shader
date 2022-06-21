// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DiscMaskShader"
{
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float distanceFromOrigin : TEXCOORD0;
            };

            float3 objectOrigin;
            float3 vertexPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                objectOrigin = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                vertexPos = mul(unity_ObjectToWorld, float4(float3(v.vertex.x, v.vertex.y, v.vertex.z), 1)).xyz;
                o.distanceFromOrigin = distance(float3(objectOrigin.x - vertexPos.x, objectOrigin.y - vertexPos.y, objectOrigin.z - vertexPos.z), float3(0,0,0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                if(i.distanceFromOrigin > 10 && i.distanceFromOrigin < 20)
                {
                    col = fixed4(1,1,1,1);
                }
                else
                {
                    col = fixed4(1,1,1,0);
                }
                return col;
            }
            ENDCG
        }
    }
}
