// Upgrade NOTE: 只支持3张贴图

Shader "Baoyu/Baoyu-Terrain-Light" {
	Properties{
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

		SubShader{
			Tags{
				"SplatCount" = "4"
				"Queue" = "Geometry-100"
				"RenderType" = "Opaque"
			}
			CGPROGRAM
			#pragma surface surf Lambert exclude_path:deferred exclude_path:prepass noshadow noambient novertexlights nofog nometa noforwardadd 
            #pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			fixed4 _Splat0Color, _Splat1Color, _Splat2Color, _Splat3Color;

			struct Input {
				float2 uv_Control : TEXCOORD0;  
				float2 uv_Splat0 : TEXCOORD1;
				float2 uv_Splat1 : TEXCOORD2;
				float2 uv_Splat2 : TEXCOORD3; 
				float2 uv_Splat3 : TEXCOORD4;  
			};


			void surf(Input IN, inout SurfaceOutput o) 
			{
				fixed4 splat_control = tex2D(_Control, IN.uv_Control);
				fixed3 col;
				col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb * _Splat0Color;
				col += splat_control.g * tex2D(_Splat1, IN.uv_Splat1).rgb * _Splat1Color;
				col += splat_control.b * tex2D(_Splat2, IN.uv_Splat2).rgb * _Splat2Color;
				o.Albedo = col;
				o.Alpha = 1.0;
			}
			ENDCG
	}
	//Fallback "Diffuse"
}
