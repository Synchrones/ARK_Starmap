Shader "Unlit/PlanetRingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexStart("Texture start", Float) = 1
        _TexEnd("Texture end", Float) = 1
        _Opacity("Opacity", Range(0.0, 1)) = 1
        _Color("Color", Color) = (1,1,1,1)
        _Threshold("Threshold", Range(0.0, 1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float distanceFromOrigin : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 objectOrigin;
            float3 vertexPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                objectOrigin = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                vertexPos = mul(unity_ObjectToWorld, float4(float3(v.vertex.x, v.vertex.y, v.vertex.z), 1)).xyz;
                o.distanceFromOrigin = distance(float3(objectOrigin.x - vertexPos.x, objectOrigin.y - vertexPos.y, objectOrigin.z - vertexPos.z), float3(0,0,0));
                return o;
            }

            float _TexStart;
            float _TexEnd;
            float _Opacity;
            float4 _Color;
            float _Sum;
            float _Threshold;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,0);
                if(i.distanceFromOrigin > _TexStart && i.distanceFromOrigin < _TexEnd)
                {
                    col = tex2D(_MainTex, i.uv);
                    col *= _Color;
                    _Sum = col.r + col.g + col.b;
                    col.a -= _Sum / 3 / _Threshold;
                    col.a *= _Opacity;
                    
                }
                return col;
            }
            ENDCG
        }
    }
}
