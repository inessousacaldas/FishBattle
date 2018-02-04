// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/Unlit/DoubleSide"
{
	Properties 
	{
//		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {} 
 	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }

    	Pass 
		{    
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
//			fixed4 _Color;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct v2f
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
            };

			// ============================================= 	
			v2f vert ( VertInput  ad )
			{
				v2f v;

    			v.pos = UnityObjectToClipPos( ad.vertex );
   				v.tc1 = TRANSFORM_TEX(ad.texcoord,_MainTex);

				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( v2f v ):COLOR
			{
//				fixed4 col = tex2D(_MainTex,v.tc1) * _Color;
//    			return col;
				return tex2D(_MainTex,v.tc1);
			}

			ENDCG
		}
 	}
 	
 	//Fallback "Diffuse"
 }
