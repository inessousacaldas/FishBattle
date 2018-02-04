// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/Unlit/AlphaTest"
{
    Properties
    {
		_CutOff("Alpha CutOff", Range(0, 1)) = 0.5
        _MainTex("Texture", 2D) = "black" {}
    }

        //=========================================================================
    SubShader
    {
        Tags {"Queue" = "AlphaTest+10" "IgnoreProjector" = "True" }

        Pass
        {
			Cull Off
			// Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D	_MainTex;
			half _CutOff;

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
				clip(fcolor.a - _CutOff);

                return fcolor;
            }

            ENDCG
        }
    }
        //Fallback "VertexLit"
}
