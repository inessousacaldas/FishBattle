// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Baoyu/Model/Outline"
{
    Properties
    {
        _ColorAlpha("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "black" {}
		_Outline ("Outline", Range(0,1)) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" }

		Pass {
			NAME "OUTLINE"
			
			ZWrite Off
			Cull Front
			Offset 1,1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float _Outline;
			fixed4 _OutlineColor;
			fixed4 _ColorAlpha;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f {
				half4 pos : SV_POSITION;
			};
			
			v2f vert (a2v v) {
				v2f o;
				o.pos =  UnityObjectToClipPos(v.vertex); 
				float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal); 
				float2 offset = TransformViewToProjection(normal.xy);
				o.pos.xy += offset * _Outline;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{ 
				return _OutlineColor * _ColorAlpha.a;               
			}
			
			ENDCG
		}

        Pass
        {
            Cull Back
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D	_MainTex;
            fixed4 _ColorAlpha;

            struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

            struct v2f
            {
                half4 pos    : SV_POSITION;
                half2 tc1    : TEXCOORD0;
            };

            v2f vert(VertInput  ad)
            {
                v2f v;

                v.pos = UnityObjectToClipPos(ad.vertex);
                v.tc1 = ad.texcoord;
                return v;
            }

            fixed4 frag(v2f v) :COLOR
            {
                fixed4 fcolor = tex2D(_MainTex, v.tc1);
                return fcolor * _ColorAlpha;
            }

            ENDCG
        }
    }
}
