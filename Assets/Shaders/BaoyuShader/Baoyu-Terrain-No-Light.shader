// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Baoyu/Baoyu-Terrain-No-Light"
{
	Properties
	{
		_Control("Control (RGBA)", 2D) = "bump" {}
		_Splat0Color("Main Color", Color) = (1,1,1,1)
		_Splat0("Layer 0 (R)", 2D) = "white" {}
		_Splat1Color("Main Color", Color) = (1,1,1,1)
		_Splat1("Layer 1 (G)", 2D) = "white" {}
		_Splat2Color("Main Color", Color) = (1,1,1,1)
		_Splat2("Layer 2 (B)", 2D) = "white" {}
		_Splat3Color("Main Color", Color) = (1,1,1,1)
		_Splat3("Layer 3 (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers xbox360 ps3
			#pragma vertex vert
			#pragma fragment frag		
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma multi_compile_fwdbase		    	
			#include "UnityCG.cginc"

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			fixed4 _Splat0Color, _Splat1Color, _Splat2Color, _Splat3Color;

			float4 _Control_ST, _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
#ifndef LIGHTMAP_OFF
			// sampler2D   unity_Lightmap;
			// float4   	unity_LightmapST;
#endif

			struct VertInput
			{
				float4 vertex	: POSITION;
				float2 texcoord	: TEXCOORD0;
#ifndef LIGHTMAP_OFF
				float4 texcoord1: TEXCOORD1;
#endif
			};


			struct v2f
			{
				float4 pos   : SV_POSITION;
				float2 uv_Control : TEXCOORD0;
				float2 uv_Splat0 : TEXCOORD1;
				float2 uv_Splat1 : TEXCOORD2;
				float2 uv_Splat2 : TEXCOORD3;
				float2 uv_Splat3 : TEXCOORD4;
#ifndef LIGHTMAP_OFF
				half2 lmapuv : TEXCOORD5;
#endif													
			};

			v2f vert(VertInput v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv_Control = TRANSFORM_TEX(v.texcoord, _Control);
				o.uv_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);

#ifndef LIGHTMAP_OFF			    
				o.lmapuv = (unity_LightmapST.xy * v.texcoord1.xy) + unity_LightmapST.zw;
#endif			    
				return o;
			}


			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 splat_control = tex2D(_Control, IN.uv_Control);
				fixed3 col;
				col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb * _Splat0Color;
				col += splat_control.g * tex2D(_Splat1, IN.uv_Splat1).rgb * _Splat1Color;
				col += splat_control.b * tex2D(_Splat2, IN.uv_Splat2).rgb * _Splat2Color;
				col += splat_control.a * tex2D(_Splat3, IN.uv_Splat3).rgb * _Splat3Color;
#ifndef LIGHTMAP_OFF				
				return fixed4(col * DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmapuv)), 1.0);
#else				
				return fixed4(col, 1.0);
#endif				
			}
			ENDCG
		}
	}
	//Fallback "Diffuse"
}
