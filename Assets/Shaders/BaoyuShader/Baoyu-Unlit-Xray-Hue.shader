// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/Unlit/Xray-Hue"
{
	Properties 
	{
		_MainTex ("Texture", 2D)								= "black" {} 
		_MutateTex ("Mutate", 2D)                               = "black"{}
		_MaskTex ("Mask", 2D)                                   = "black"{}
		_blendFactorR ("Blend Factor",Range(0,1.0))			= 0
		_blendFactorG ("Blend Factor",Range(0,1.0))			= 0
		_blendFactorB ("Blend Factor",Range(0,1.0))			= 0

        _RHueShift1("_RHueShift1", vector) = (1,0,0,0)
        _RHueShift2("_RHueShift2", vector) = (0,1,0,0)
        _RHueShift3("_RHueShift3", vector) = (0,0,1,0)

        _GHueShift1("_GHueShift1", vector) = (1,0,0,0)
        _GHueShift2("_GHueShift2", vector) = (0,1,0,0)
        _GHueShift3("_GHueShift3", vector) = (0,0,1,0)

        _BHueShift1("_BHueShift1", vector) = (1,0,0,0)
        _BHueShift2("_BHueShift2", vector) = (0,1,0,0)
        _BHueShift3("_BHueShift3", vector) = (0,0,1,0)
 	}

	//=========================================================================
	SubShader 
	{
 		Tags {"Queue"="AlphaTest+10" "IgnoreProjector"="True" }
		
//		Pass{
//			ZWrite Off
//			ZTest Off
//			Blend SrcAlpha OneMinusSrcAlpha
//			
//			CGPROGRAM
//			
// 			#pragma vertex vert
//			#pragma fragment frag
//			#include "UnityCG.cginc"
//
//			struct VertInput
//            {
//                float4 vertex	: POSITION;
//                float2 texcoord	: TEXCOORD0;
//            };
//
//           	struct v2f {
//                half4  pos : SV_POSITION;
//                half2  uv : TEXCOORD0;
//            };
//
//			//=============================================
//			v2f vert (VertInput v)
//            {
//                v2f o;
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//                o.uv = v.texcoord;
//                return o;
//            }
//			
//			sampler2D _MainTex;
//			sampler2D _MutateTex;
//	        sampler2D _MaskTex;
//	        
//	        fixed _blendFactorR;
//	        fixed _blendFactorG;
//	        fixed _blendFactorB;
//            
//           	uniform fixed4x4 _RHueShift;
//           	uniform fixed4x4 _GHueShift;
//           	uniform fixed4x4 _BHueShift;
// 	
//			//=============================================
//			fixed4 frag ( v2f i ):COLOR
//			{
////				return fixed4(0.2,0.8,0.2,0.4);
//                fixed4 origin = tex2D(_MainTex, i.uv);
//                fixed4 mask = tex2D(_MaskTex, i.uv);
//                fixed4 mutate = tex2D(_MutateTex, i.uv);
//               	
//               	fixed4 mutateColor = mul(_RHueShift,lerp(origin,mutate,_blendFactorR))*mask.r
//					               	+mul(_GHueShift,lerp(origin,mutate,_blendFactorG))*mask.g
//					               	+mul(_BHueShift,lerp(origin,mutate,_blendFactorB))*mask.b;
//				
//				origin = mutateColor+origin*(1-mask.r-mask.g-mask.b);  
//				return fixed4(origin.rgb,0.4);
//			}
//
//			ENDCG
//		}
		
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

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct v2f {
                half4  pos : SV_POSITION;
                half2  uv : TEXCOORD0;
            };

			//=============================================
			v2f vert (VertInput v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                return o;
            }
			
			sampler2D _MainTex;
			sampler2D _MutateTex;
	        sampler2D _MaskTex;
	        
	        fixed _blendFactorR;
	        fixed _blendFactorG;
	        fixed _blendFactorB;
            
            fixed4 _RHueShift1;
            fixed4 _RHueShift2;
            fixed4 _RHueShift3;

            fixed4 _GHueShift1;
            fixed4 _GHueShift2;
            fixed4 _GHueShift3;

            fixed4 _BHueShift1;
            fixed4 _BHueShift2;
            fixed4 _BHueShift3;
 	
			//=============================================
			fixed4 frag ( v2f i ):COLOR
			{
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);
                fixed4 mutate = tex2D(_MutateTex, i.uv);
               	fixed4 temp = fixed4(0,0,0,1);
                fixed4x4 _RHueShift = fixed4x4(
                    _RHueShift1,
                    _RHueShift2,
                    _RHueShift3,
                    temp
                );
                fixed4x4 _GHueShift = fixed4x4(
                    _GHueShift1,
                    _GHueShift2,
                    _GHueShift3,
                    temp
                );
                fixed4x4 _BHueShift = fixed4x4(
                    _BHueShift1,
                    _BHueShift2,
                    _BHueShift3,
                    temp
                );

               	fixed4 mutateColor = mul(_RHueShift,lerp(origin,mutate,_blendFactorR))*mask.r
					               	+mul(_GHueShift,lerp(origin,mutate,_blendFactorG))*mask.g
					               	+mul(_BHueShift,lerp(origin,mutate,_blendFactorB))*mask.b
					               	+origin*(1-mask.r-mask.g-mask.b);
					               
				return fixed4(mutateColor.rgb,1);
//				return lerp(mutateColor,origin,1-mask.r-mask.g-mask.b);
			}

			ENDCG
		}
 	}
 	//Fallback "VertexLit"
 }
