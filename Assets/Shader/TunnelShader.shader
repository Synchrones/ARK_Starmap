Shader "Custom/TunnelShader"
{
    Properties
    {
        _MainColor ("Color", Color) = (1,1,1,1)
        _Color ("Color", Color) = (1,1,1,1)
        _Tex1 ("Albedo (RGB)", 2D) = "white" {}
        _Tex2 ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags{"RenderType"="Transparent" "Queue"="Transparent"}
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
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
			};

			fixed4 _Color;
            fixed4 _MainColor;

            float4 _Tex1_ST;
            float4 _Tex2_ST;
            sampler2D _Tex1;
            sampler2D _Tex2;

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _Tex1);
                OUT.uv2 = TRANSFORM_TEX(IN.uv, _Tex2);
				return OUT;
			};
			

			fixed4 frag(v2f IN) : SV_Target
			{
                float4 mainColor = _MainColor;
				float4 firstColor = tex2D(_Tex1, IN.uv);
                float4 secondColor = tex2D(_Tex2, IN.uv2);
                mainColor += (firstColor + secondColor) * _Color;
                return mainColor;
			}


			ENDCG
		}
    }
}
