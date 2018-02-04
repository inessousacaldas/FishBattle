// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Baoyu/UI/Particles/Additive-Turbulence"{
	Properties 
	{
		_MainTex("Main_Texture", 2D) = "white" {}
		_TintColor("Color", Color) = (1,1,1,1)
		_Blend_Texture_01("Blend_Texture_01", 2D) = "white" {}
		_Blend_Color_01("Color", Color) = (1,1,1,1)
		_Blend_Texture_02("Blend_Texture_02", 2D) = "black" {}
		_Blend_Color_02("Color", Color) = (1,1,1,1)
//		_MaskTex("Mask_Texture", 2D) = "black" {}
		_Speed00("Main_Texture_Speed", Float) = 0
		_Speed01("Blend_Texture_01_Speed", Float) = 1
		_Speed02("Blend_Texture_02_Speed", Float) = 1
		_LightenMain("Brightness_Main", Float) = 1
		_Lighten("Brightness_Blend", Float) = 1
		_ClipRange ("Clip Range",Vector) = (-1,-1,1,1)
	}

	Category {
		Tags { "Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
	//	AlphaTest Greater .01
	//	ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		SubShader {
			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				//#pragma multi_compile_particles

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _TintColor;

				sampler2D _Blend_Texture_01;
				float4 _Blend_Texture_01_ST;
				float4 _Blend_Color_01;

				sampler2D _Blend_Texture_02;
				float4 _Blend_Texture_02_ST;
				float4 _Blend_Color_02;

//				sampler2D _MaskTex;
				float _Speed00;
				float _Speed01;
				float _Speed02;
				float _LightenMain;
				float _Lighten;
				float4 _ClipRange;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 mainTexUV : TEXCOORD0;
					float2 blendTexUV1 : TEXCOORD1;
					float2 blendTexUV2 : TEXCOORD2;
					float2 worldPos : TEXCOORD3;
				};
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.mainTexUV = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.blendTexUV1 = TRANSFORM_TEX(v.texcoord,_Blend_Texture_01);
					o.blendTexUV2 = TRANSFORM_TEX(v.texcoord,_Blend_Texture_02);
					o.worldPos = o.vertex.xy;
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					if( i.worldPos.x >= _ClipRange.x
						&& i.worldPos.x <= _ClipRange.z
						&& i.worldPos.y >= _ClipRange.y
						&& i.worldPos.y <= _ClipRange.w){

						fixed4 MainCol = _TintColor * tex2D(_MainTex,i.mainTexUV + float2(0,_Time.x*_Speed00));
						fixed4 BlendCol01= _Blend_Color_01 * tex2D(_Blend_Texture_01,i.blendTexUV1+_Time*_Speed01);
						fixed4 BlendCol02 = _Blend_Color_02 * tex2D(_Blend_Texture_02,i.blendTexUV2 + float2(0,_Time.x*_Speed02));
//						float4 MaskCol = tex2D(_MaskTex,i.texcoord);

						fixed4 col = MainCol + MainCol * (BlendCol01 + BlendCol02) * BlendCol01 * BlendCol02 * _Lighten;
						col = _LightenMain * col * i.color;
						return col;
					}else{
						return fixed4(0,0,0,0);
					}
				}
				ENDCG 
			}
		}	
	}
}