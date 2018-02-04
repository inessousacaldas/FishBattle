// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/Unlit/Xray"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "black" {}
    }

        //=========================================================================
    SubShader
    {
        Tags {"Queue" = "AlphaTest+10" "IgnoreProjector" = "True" }

        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite On
            Blend Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile BLEND_OFF BLEND_ON

            sampler2D	_MainTex;
#ifdef BLEND_ON
            float4 _Color;
#endif

            struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

            struct Varys
            {
                half4 pos    : SV_POSITION;
                half2 tc1    : TEXCOORD0;
            };

            //=============================================
            Varys vert(VertInput  ad)
            {
                Varys v;

                v.pos = UnityObjectToClipPos(ad.vertex);
                v.tc1 = ad.texcoord;
                return v;
            }

            //=============================================
            fixed4 frag(Varys v) :COLOR
            {
                fixed4 fcolor = tex2D(_MainTex, v.tc1);
#ifdef BLEND_ON
                return fcolor * _Color;
#else
                return fcolor;
#endif
            }

            ENDCG
        }
    }
    //Fallback "VertexLit"
}
