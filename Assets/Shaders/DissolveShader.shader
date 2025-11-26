shader "Custom/URP/DissolveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _DissolveTexture ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveEdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05
        _DissolveEdgeColor ("Edge Color", Color) = (1,0.5,0,1)
        _DissolveEdgeIntensity ("Edge Intensity", Range(0, 5)) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 dissolveUV : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DissolveTexture);
            SAMPLER(sampler_DissolveTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _DissolveTexture_ST;
                half4 _Color;
                half _DissolveAmount;
                half _DissolveEdgeWidth;
                half4 _DissolveEdgeColor;
                half _DissolveEdgeIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.dissolveUV = TRANSFORM_TEX(input.uv, _DissolveTexture);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample textures
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                half dissolveNoise = SAMPLE_TEXTURE2D(_DissolveTexture, sampler_DissolveTexture, input.dissolveUV).r;
                
                // Calculate dissolve
                half dissolveThreshold = dissolveNoise - _DissolveAmount;
                
                // Discard pixels below threshold
                clip(dissolveThreshold);
                
                // Calculate edge glow
                half edgeFactor = smoothstep(0, _DissolveEdgeWidth, dissolveThreshold);
                half3 edgeGlow = _DissolveEdgeColor.rgb * _DissolveEdgeIntensity * (1 - edgeFactor);
                
                // Combine base color and edge glow
                half3 finalColor = mainColor.rgb + edgeGlow;
                
                // Fade alpha near dissolve edge
                half alpha = mainColor.a * edgeFactor;
                
                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}