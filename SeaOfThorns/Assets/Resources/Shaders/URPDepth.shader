Shader "Unlit/URPDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthMaxDistance("DepthMaxDistance", Float) = 1
        _DeepColor("DeepColor", Color) = (1,1,1,1)
        _ShallowColor("ShallowColor", Color) = (1,1,1,1)
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseCutoff("SurfaceNoiseCutoff", Range(0, 1)) = .777
        _FoamDistance("Foam Distance", Float) = 0.4
    }
    SubShader
    {
        Tags { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Opaque" 
            "Queue"="Geometry+0" 
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl" 
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl" 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float4 screenPosition :TEXCOORD1;
                float2 noiseUV : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial) 
            CBUFFER_END 

            TEXTURE2D(_MainTex);
            float4 _MainTex_ST;
            sampler2D _DepthNTexture;
            float _DepthMaxDistance;

            float4 _SurfaceNoise_ST;
            sampler2D _SurfaceNoise;
            float4 _DeepColor;
            float4 _ShallowColor;
            float _SurfaceNoiseCutoff;
            float _FoamDistance;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float existingDepth01 = tex2Dproj(_DepthNTexture, (i.screenPosition)/i.screenPosition.w).b;
                float4 waterCol = lerp(_ShallowColor, _DeepColor, existingDepth01 * 2);
                float depthLinear = LinearEyeDepth(existingDepth01,_ZBufferParams);
                float depthDiff = depthLinear - i.screenPosition.w;
                //depthDiff *= existingDepth01;

                float surfaceNoiseSample = tex2D(_SurfaceNoise, i.noiseUV).r;
                float foamDistanceDiff01 = saturate(depthDiff /  _FoamDistance);
                float surfaceNoiseCutoff = foamDistanceDiff01 * _SurfaceNoiseCutoff;
                surfaceNoiseSample = surfaceNoiseSample > _SurfaceNoiseCutoff ? 1: 0;
                return depthLinear*.1;
                //return waterCol + surfaceNoiseSample;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}
