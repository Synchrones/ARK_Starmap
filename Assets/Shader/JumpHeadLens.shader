Shader "Custom/JumpHeadLens" {
    Properties {
        _MainTex ("Main texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 10)) = 3.020896
        _HoleSize ("Hole Size", Range(0, 1)) = 0.7030833
        _HoleEdgeSmoothness ("Hole Edge Smoothness", Range(0.001, 0.05)) = 0.007289694
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float _DistortionStrength;
            uniform float _HoleSize;
            uniform float _HoleEdgeSmoothness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            sampler2D _MainTex;
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_MainTex, sceneUVs);
////// Lighting:
////// Emissive:
                float node_9892 = (_HoleSize*-1.0+1.0);
                float node_1841 = smoothstep( (node_9892+_HoleEdgeSmoothness), (node_9892-_HoleEdgeSmoothness), (1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.15)) ); // Create the hole mask
                float node_3969 = (1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_DistortionStrength));
                float3 emissive = (node_1841*tex2D( _MainTex, ((pow(node_3969,6.0)*(sceneUVs.rg*-2.0+1.0))+sceneUVs.rg)).rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}