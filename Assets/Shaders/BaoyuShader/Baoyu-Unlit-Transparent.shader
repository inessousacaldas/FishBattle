// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/Unlit/Transparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "black" {}
    }

        //=========================================================================
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D	_MainTex;
            float4 _Color;

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
                return fcolor * _Color;
            }

            ENDCG
        }
    }
        //Fallback "VertexLit"
}
