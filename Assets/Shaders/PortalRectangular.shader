Shader "Custom/Portal Rectangular"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "white" {}
        _UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
        _VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
        _Tiling ("Tiling", Float) = 1
        _Speed ("Speed", Float) = 1
        _FlowStrength ("Flow Strength", Float) = 1
        _FlowOffset ("Flow Offset", Float) = 0
    }
    SubShader
    {
        
        Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutOut" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        // Blend One One

        CGPROGRAM
        #pragma surface surf Unlit noambient keepalpha
        #pragma target 3.0
        #include "Flow.cginc"

        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
        {
            return fixed4(s.Albedo, s.Alpha);
        }

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        sampler2D _FlowMap;
        float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 flow = tex2D (_FlowMap, IN.uv_MainTex).rg;
            flow *= _FlowStrength;
            float noise = tex2D (_FlowMap, IN.uv_MainTex).a;   
            float time = _Time.y * _Speed + noise;
            float2 jump = float2(_UJump, _VJump);
            float3 uvwA = FlowUVW(IN.uv_MainTex, flow, jump, _FlowOffset, _Tiling, time, false);
            float3 uvwB = FlowUVW(IN.uv_MainTex, flow, jump, _FlowOffset, _Tiling, time, true);

            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

            fixed4 c = ((texA + texB) + .2) * _Color;
            c.rgb = lerp(fixed3(.5, .5, .5), c.rgb, 1.1);
            o.Albedo = c.rgb;
            o.Alpha = _Color.a;
            o.Emission = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
