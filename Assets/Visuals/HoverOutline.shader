Shader"Custom/URP/InvertedHullOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 0.03
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" }

        Pass
        {
Name"Outline"
            Cull
Front
            ZWrite
Off
            ZTest
LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
};

float _OutlineThickness;
float4 _OutlineColor;

Varyings vert(Attributes IN)
{
    Varyings OUT;

    float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);

    positionWS += normalWS * _OutlineThickness;

    OUT.positionHCS = TransformWorldToHClip(positionWS);

    return OUT;
}

half4 frag() : SV_Target
{
    return _OutlineColor;
}

            ENDHLSL
        }
    }
}