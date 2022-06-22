Shader "Unlit/DiscMaskShader"
{
     Properties
    {
        _GreenBandColor("Green Band Color", Color) = (0,1,0,0.1)
        _FrostBandColor("Frost Band Color", Color) = (0,0,1,0.1)
        _GreenBandStart("Green Band Start", Float) = 1
        _GreenBandEnd("Green Band End", Float) = 2
        _FrostBandPos("Frost Band Pos", Float) = 5
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

            float4 _GreenBandColor;
            float4 _FrostBandColor;

            float _GreenBandStart;
            float _GreenBandEnd;
            float _FrostBandPos;

            float _FrostBandEnd;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,0);
                _FrostBandEnd = clamp(_FrostBandPos / 3, 5, 50);
                if(i.distanceFromOrigin > _GreenBandStart && i.distanceFromOrigin < _GreenBandEnd)
                {
                    col = _GreenBandColor;
                    col.a = 0.05;
                }
                else if(i.distanceFromOrigin > _GreenBandStart - 0.2 && i.distanceFromOrigin < _GreenBandStart)
                {
                    col = _GreenBandColor;
                    col.a = (i.distanceFromOrigin - (_GreenBandStart - 0.2)) / 4;
                }
                else if(i.distanceFromOrigin > _GreenBandEnd && i.distanceFromOrigin < _GreenBandEnd + 0.2)
                {
                    col = _GreenBandColor;
                    col.a = (_GreenBandEnd + 0.2 - i.distanceFromOrigin) / 4;
                }
                else if(i.distanceFromOrigin > _FrostBandPos && i.distanceFromOrigin < _FrostBandPos + 0.1)
                {
                    col = _FrostBandColor;
                    col.a = 0.05;
                }
                else if(i.distanceFromOrigin > _FrostBandPos - 0.2 && i.distanceFromOrigin < _FrostBandPos )
                {
                    col = _FrostBandColor;
                    col.a = (i.distanceFromOrigin - (_FrostBandPos - 0.2)) / 4;
                }
                else if(i.distanceFromOrigin > _FrostBandPos && i.distanceFromOrigin < _FrostBandPos + _FrostBandEnd)
                {
                    col = _FrostBandColor;
                    col.a = (_FrostBandPos + _FrostBandEnd - i.distanceFromOrigin) / (20 * _FrostBandEnd);
                }
                return col;
            }
            ENDCG
        }
    }
}
