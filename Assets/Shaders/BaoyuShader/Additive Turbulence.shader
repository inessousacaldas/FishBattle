// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX PACK 1/Particles/Additive Turbulence"
{
Properties
{
	_MainTex("Main_Texture", 2D) = "white" {}
	_Color01("Color", Color) = (1,1,1,1)
	_Blend_Texture("Blend_Texture_01", 2D) = "white" {}
	_Color02("Color", Color) = (1,1,1,1)
	_Blend_Texture01("Blend_Texture_02", 2D) = "black" {}
	_Color03("Color", Color) = (1,1,1,1)
	_Speed01("Blend_Texture_01_Speed", Float) = 1
	_Speed02("Blend_Texture_02_Speed", Float) = 1
	_LightenMain("Brightness_Main", Float) = 1
	_Lighten("Brightness_Blend", Float) = 1
}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_particles

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color01;
			sampler2D _Blend_Texture;
			float4 _Blend_Texture_ST;

			float4 _Color02;
			sampler2D _Blend_Texture01;
			float4 _Blend_Texture01_ST;

			float4 _Color03;
			float _Speed01;
			float _Speed02;
			float _LightenMain;
			float _Lighten;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_Blend_Texture : TEXCOORD1;
				float2 uv_Blend_Texture01 : TEXCOORD2;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_Blend_Texture = TRANSFORM_TEX(v.texcoord, _Blend_Texture);
				o.uv_Blend_Texture01 = TRANSFORM_TEX(v.texcoord, _Blend_Texture01);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				float4 Tex2D0 = tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
				float4 Multiply5 = _Color01 * Tex2D0;
				float4 Multiply1 = _Time * _Speed01.xxxx;
				float4 UV_Pan0 = float4((IN.uv_Blend_Texture.xyxy).x,(IN.uv_Blend_Texture.xyxy).y + Multiply1.x,(IN.uv_Blend_Texture.xyxy).z + Multiply1.x,(IN.uv_Blend_Texture.xyxy).w);
				float4 Tex2D1 = tex2D(_Blend_Texture,UV_Pan0.xy);
				float4 Multiply6 = _Color02 * Tex2D1;
				float4 Multiply3 = _Time * _Speed02.xxxx;
				float4 UV_Pan2 = float4((IN.uv_Blend_Texture01.xyxy).x + Multiply3.x,(IN.uv_Blend_Texture01.xyxy).y + Multiply3.x,(IN.uv_Blend_Texture01.xyxy).z,(IN.uv_Blend_Texture01.xyxy).w);
				float4 Tex2D2 = tex2D(_Blend_Texture01,UV_Pan2.xy);
				float4 Multiply8 = _Color03 * Tex2D2;
				float4 Add1 = Multiply6 + Multiply8;
				float4 Multiply0 = Multiply6 * Multiply8;
				float4 Multiply10 = Add1 * Multiply0;
				float4 Multiply7 = Multiply5 * Multiply10;
				float4 Multiply9 = Multiply7 * _Lighten.xxxx;
				float4 Add0 = Multiply5 + Multiply9;
				float4 Multiply11 = _LightenMain.xxxx * Add0;
				return Multiply11 * IN.color;
			}
			ENDCG
		}
	}
	//Fallback "Diffuse"
}