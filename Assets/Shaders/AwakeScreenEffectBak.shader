Shader "Custom/Awake Screen Effect bak"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 1
        _ArchHeight ("Arch Height", Range (0, .5)) = .2
        _BlurSize ("Blur Size", Float) = 1
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
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[5] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Progress;
            float _ArchHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv[0] = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 uv = i.uv[0];
                fixed4 col = tex2D(_MainTex, uv);

                // 上边界
                float upBorder = .5 + _Progress * (1 + _ArchHeight - .5);
                float downBorder = .5 - _Progress * (.5 + _ArchHeight);
                upBorder -=  _ArchHeight * pow(uv.x - .5, 2);
                downBorder += _ArchHeight * pow(uv.x - .5, 2);

                float visibleV = (1 - step(upBorder, uv.y)) * (step(downBorder, uv.y));
                col *= visibleV;
                col *= _Progress;
                return col;
            }
            ENDCG
        }

        UsePass "Custom/Gaussian Blur/GAUSSIAN_BLUR_VERTICAL"

        UsePass "Custom/Gaussian Blur/GAUSSIAN_BLUR_HORIZONTAL"

    }

    Fallback Off
}
