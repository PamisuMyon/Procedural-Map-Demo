Shader "Custom/Warp Screen Effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Twirl ("Twirl Angle", Float) = 0
        _Fade ("Fade", Range(0, 1)) = 0
        _FadeColor ("Fade Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        ZTest Always ZWrite Off Cull Off

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Twirl;
            float _Fade;
            fixed4 _FadeColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Twirl
                float2 uv = float2(i.uv.x - .5, i.uv.y - .5);
                float f = distance(uv, float2(0, 0));
                float s = sin(lerp(0, _Twirl, f));
                float c = cos(lerp(0, _Twirl, f));
                uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                uv = float2(uv.x + .5, uv.y + .5);
                fixed4 col = tex2D(_MainTex, uv);
                
                // Fade
                col = lerp(col, _FadeColor, _Fade);

                return col;
            }
            ENDCG
        }
    }
}
