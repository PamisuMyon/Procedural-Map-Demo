Shader "Custom/Status Slider"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Value ("Slider Value", Range(0, 1)) = .5
        _FillColor ("Slider Value Fill Color", Color) = (0, 0, 0, 0)
        _Value2 ("Slider Second Value", Range(0, 1)) = .5
        _FillColor2 ("Slider Second Fill Color", Color) = (0, 0, 0, 0)
        _Offset ("Offset", Range(0, .3)) = .1
        _EmptyAreaAlpha ("Empty Area Alpha", Range(0, 1)) = .5
        // _Noise ("Flow Noise (R)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        ZTest Always ZWrite Off Cull Back
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Value;
            fixed4 _FillColor;
            float _Value2;
            fixed4 _FillColor2;
            float _Offset;
            fixed _EmptyAreaAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;

                float value = _Offset + _Value * (1 - _Offset * 2);
                float value2 = _Offset + _Value2 * (1 - _Offset * 2);

                fixed4 fill =  _FillColor * (1 - step(value, i.uv.x));
                fixed4 fill2 = _FillColor2 * (1 - step(value2, i.uv.x) - (1 - step(value, i.uv.x)));
                fixed4 empty = col * step(value2, i.uv.x);
                empty.a *= _EmptyAreaAlpha;

                col = col * fill + col * fill2;
                col += empty;
                return col;
            }
            ENDCG
        }
    }
}
