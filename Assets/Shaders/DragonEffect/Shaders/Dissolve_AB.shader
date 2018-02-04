Shader "Dragon/Dissolve/Dissolve Alpha Blend" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125
		_Amount("Amount", Range(0, 1)) = 0.5
		_StartAmount("StartAmount", float) = 0.1
		_Illuminate("Illuminate", Range(0, 1)) = 0.5
		_Tile("Tile", float) = 1
		_DissColor("DissColor", Color) = (1,1,1,1)
		_ColorAnimate("ColorAnimate", vector) = (1,1,1,1)
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_DissolveSrc("DissolveSrc", 2D) = "white" {}
	}
		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 400
			Cull Off

			CGPROGRAM
			#pragma target 3.0
			#pragma surface surf BlinnPhong addshadow alpha exclude_path:deferred exclude_path:prepass noambient novertexlights nolightmap nodirlightmap nofog nometa noforwardadd 

			sampler2D _MainTex;
			sampler2D _DissolveSrc;

			fixed4 _Color;
			half4 _DissColor;
			half _Shininess;
			half _Amount;
			static half3 Color = float3(1,1,1);
			half4 _ColorAnimate;
			half _Illuminate;
			half _Tile;
			half _StartAmount;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb * _Color.rgb;

				float ClipTex = tex2D(_DissolveSrc, IN.uv_MainTex / _Tile).r;
				float ClipAmount = ClipTex - _Amount;
				float Clip = 0;

				if (_Amount > 0)
				{
					if (ClipAmount < 0)
					{
						Clip = 1; //clip(-0.1);
					}
					 else
					 {
						if (ClipAmount < _StartAmount)
						{
							float3 fator = float3(ceil(_ColorAnimate.x), ceil(_ColorAnimate.y), ceil(_ColorAnimate.z));
							Color = (1 - fator)*_DissColor.rgb + fator*(ClipAmount / _StartAmount);

							o.Albedo = (o.Albedo *((Color.x + Color.y + Color.z))* Color*((Color.x + Color.y + Color.z))) / (1 - _Illuminate);
						}
					 }
				}

				if (Clip == 1)
				{
					clip(-0.1);
				}


				//////////////////////////////////
				//
				o.Gloss = tex.a;
				o.Alpha = tex.a * _Color.a;
				o.Specular = _Shininess;
			}
			ENDCG
	}

		//Fallback "Mobile/Diffuse" 

}
