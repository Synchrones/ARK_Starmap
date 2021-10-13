Shader "Custom/Distortion" 
{
    Properties 
	{
        _MainTex ("Main texture", 2D) = "white" {}
        _NoiseTex ("Noise texture", 2D) = "grey" {}
 
        _Mitigation ("Distortion mitigation", Range(1, 30)) = 1
        _SpeedX("Speed along X", Range(0, 5)) = 1
        _SpeedY("Speed along Y", Range(0, 5)) = 1

		_Color("Color", Color) = (1,1,1,1)
		_Transparency("Transparency", Range(0.0, 0.5)) = 0.25
		_Glow("Glow", Range(0,10)) = 3
    }
 
    SubShader 
	{
        Tags {"RenderType"="Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
	
        Pass 
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _SpeedX;
            float _SpeedY;
            float _Mitigation;
 
            struct v2f 
			{
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
            };
 
            fixed4 _MainTex_ST;
 
            v2f vert(appdata_base v) 
			{
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
 
            half4 frag(v2f i) : COLOR 
			{
                half2 uv = i.uv;
                half noiseVal = tex2D(_NoiseTex, uv).r;
                uv.x = uv.x + noiseVal * (0.25 * sin(_Time.y * _SpeedX) + 0.5) / _Mitigation;
                uv.y = uv.y + noiseVal * (0.25 * sin(_Time.y * _SpeedY) + 0.5) / _Mitigation;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
		GrabPass{"_GrabTexture"}

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
				float4 grabPos : TEXCOORD0;
			};

			fixed4 _Color;
			float _Transparency;
			float _Glow;

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.grabPos = ComputeGrabScreenPos(OUT.position);
				return OUT;
			};

			sampler2D _GrabTexture;

			fixed4 frag(v2f IN) : SV_Target
			{
				float4 pixelColor = tex2Dproj(_GrabTexture, IN.grabPos);
				pixelColor.a = _Transparency;
				return pixelColor * _Color + _Color / _Glow; 
			}


			ENDCG
		}
    }
    FallBack "Diffuse"
}