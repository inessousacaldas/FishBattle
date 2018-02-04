Shader "Baoyu/DiffuseSkyReflection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Cube("SkyBox", Cube) = "balck"{}
		_reflection ("reflection", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
            #include "Lighting.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
                half3 normal : NORMAL;
				half2 uv : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                half2 texcoord2 : TEXCOORD2;

			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
                half3 worldRef : TEXCOORD2;
                half4 lmap : TEXCOORD3;
				half4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			half _reflection;
            samplerCUBE _Cube;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
                o.worldRef = reflect(-worldViewDir, worldNormal);
                #ifndef DYNAMICLIGHTMAP_OFF
                o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                #ifndef LIGHTMAP_OFF
                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 difftex = tex2D(_MainTex, i.uv);
                fixed3 refCol = texCUBE(_Cube, i.worldRef).rgb ;
                half4 col = 0;
                 // lightmaps
                #ifndef LIGHTMAP_OFF
                #if DIRLIGHTMAP_COMBINED
                    // directional lightmaps
                    fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);
                    fixed4 lmIndTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, INilmap.xy);
                    half3 lm = DecodeDirectionalLightmap (DecodeLightmap(lmtex), lmIndTex, o.Normal);
                #elif DIRLIGHTMAP_SEPARATE
                    // directional with specular - no support
                    half4 lmtex = 0;
                    half3 lm = 0;
                #else
                    // single lightmap
                    fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);
                    fixed3 lm = DecodeLightmap (lmtex);
                #endif

                #endif // LIGHTMAP_OFF
                half3 iDiff = 0;
                #ifdef LIGHTMAP_OFF
                    half nl = dot(normal, i.lightDir);
                    iDiff = difftex.rgb * _LightColor0 * (nl * _DiffFactor);
                #endif

				

                #ifndef LIGHTMAP_OFF
                #ifdef SHADOWS_SCREEN
                    #if defined(UNITY_NO_RGBM)
                    iDiff += difftex.rgb * min(lm, atten*2);
                    #else
                    iDiff += difftex.rgb * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
                    #endif
                #else // SHADOWS_SCREEN
                    iDiff += difftex.rgb * lm;
                #endif // SHADOWS_SCREEN
                #endif // LIGHTMAP_OFF

                #ifndef DYNAMICLIGHTMAP_OFF
                fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, i.lmap.zw);
                iDiff += difftex.rgb * DecodeRealtimeLightmap (dynlmtex);
                #endif

                col.rgb = lerp(iDiff, iDiff * refCol, _reflection);

				UNITY_APPLY_FOG(i.fogCoord, col);
                col.a = 1;
				return col;
			}
			ENDCG
		}
        Pass
	    {
            Name "Meta"
	        Tags { "RenderType"="Opaque" "LightMode"="Meta"}
            Cull Off
		    CGPROGRAM
            #pragma vertex vert_surf
            #pragma fragment frag_surf
            #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma skip_variants INSTANCING_ON
            #define UNITY_PASS_META
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityMetaPass.cginc"

            struct v2f
		    {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
		    };
            sampler2D _MainTex;
			float4 _MainTex_ST;

            sampler2D _NormalMap;

            sampler2D _SpecMask;
            float _SpecFactor;
            float _ExpoFactor;
            float _DiffFactor;

            v2f vert_surf (appdata_full v) 
            {
                v2f o;
			    o.vertex = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
			    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
			    return o;
            }

            fixed4 frag_surf (v2f IN) : SV_Target {
                UnityMetaInput metaIN;
                UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
                metaIN.Albedo = tex2D(_MainTex, IN.uv);
                metaIN.Emission = fixed4(0,0,0,0);
                return UnityMetaFragment(metaIN);
            }

            ENDCG
        }
	}
}
