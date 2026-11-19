Shader "Kicklings/Characters/Kid Color Fill"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] _ZWrite ("ZWrite", Float) = 0
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment ColorFillFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
            CBUFFER_END

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings output = CommonUnlitVertex(input);
                output.color = input.color * _Color * unity_SpriteColor;
                return output;
            }

            half4 ColorFillFragment(Varyings input) : SV_Target
            {
                half3 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;
                // The authored black pixels are supplied by the separate shadow renderer.
                // Test before tinting, keeping dark skin, hair and clothing in the color layer.
                clip(max(originalColor.r, max(originalColor.g, originalColor.b)) - 0.0001h);
                return CommonUnlitFragment(input, input.color);
            }
            ENDHLSL
        }
    }
}
