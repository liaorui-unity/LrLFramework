Shader "Custom/Particles/MaskedAlpha"
{
    Properties
    {
        _MainTex ("粒子纹理(Particle Texture)", 2D) = "white" {}
        _MaskTex ("遮罩纹理(Mask Texture)", 2D) = "black" {}
        [HDR]_TintColor ("颜色调整(Tint Color)", Color) = (1,1,1,1)
        _DiscardThreshold ("丢弃阈值(Discard Threshold)", Range(0,1)) = 0.1
        _Alpha ("透明度(Alpha)", Range(0,1)) = 0.1
      
         _UVMainX("主UV X", Float) = 0.0
        _UVMainY("主UV Y", Float) = 0.0

        _UVMaskX("遮罩UV X", Float) = 0.0
        _UVMaskY("遮罩UV Y", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _TintColor;
            float _DiscardThreshold;
            float _Alpha;
     

            float _UVMainX;
            float _UVMainY;

            float _UVMaskX;
            float _UVMaskY;

            struct appdata_t
            {
                float4 vertex  : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float2 maskTexcoord : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord + float2(_Time.y * _UVMainX, _Time.y * _UVMainY); 
                o.maskTexcoord = v.texcoord+ float2(_Time.y*_UVMaskX, _Time.y*_UVMaskY);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.texcoord) * _TintColor;
                fixed4 maskColor = tex2D(_MaskTex, i.maskTexcoord);

                bool isBlack = maskColor.r < _DiscardThreshold && maskColor.g < _DiscardThreshold && maskColor.b < _DiscardThreshold;

                if (isBlack)
                {
                    discard; 
                }

                mainColor.a *= _Alpha;

                return mainColor;
            }
            ENDCG
        }
    }
}