// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Baoyu/UI/Particles/Additive" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
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
				float4 _ClipRange;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
				};
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.worldPos = o.vertex.xy;
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord);
					col.a *= i.worldPos.x >= _ClipRange.x;
					col.a *= i.worldPos.x <= _ClipRange.z;
					col.a *= i.worldPos.y >= _ClipRange.y;
					col.a *= i.worldPos.y <= _ClipRange.w;
					return col;
				}
				ENDCG 
			}
		}	
	}
}
