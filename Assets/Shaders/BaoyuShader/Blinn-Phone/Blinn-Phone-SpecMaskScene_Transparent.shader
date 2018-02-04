Shader "Blinn/Blinn-Phone-SpecMaskScene (Transparent)"
{
	Properties
	{
		 _MainTex("Mask", 2D) = "white" {}
         _BaseText("BaseColor", 2D) = "white"{}
         _SpecMask("Spec", 2D) = "black"{}
        [Space(20)]
        [Toggle(ENABLE_NORMALMAP)] _EnableNormalMap("EnableNormalMap", int) = 0
        _NormalMap ("NormalMap", 2D) = "bump" {}
        [Space(20)]
        _DiffFactor ("DiffFactor", Range(0,1)) = 1
        _ExpoFactor("ExpoFactor", Range(0, 20)) = 1
        _SpecFactor("SpecFactor", Range(0, 128)) = 1
        _SpecTint("_SpecTint", Range(0, 1)) = 1
        _SellColor("SellIlluminateColor", Color) = (0,0,0,0)
        _SellIntensity("SellIlluminateIntensity", Range(0, 10)) = 1

        [HideInInspector]_SpecDir("SpecDir", vector) = (0,0,0,0)
        [HideInInspector]_SpecLightColor("SpecLightColor", vector) = (0,0,0,0)

	}
	SubShader
	{
		Pass
		{
            Name "FORWARD"
		    Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
            Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma shader_feature ENABLE_NORMALMAP
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
            #include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
				float3 normal : NORMAL;
                #if ENABLE_NORMALMAP
                float4 tangent : TANGENT;
                #endif

			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 lmap : TEXCOORD3;
                UNITY_FOG_COORDS(4)
                #if ENABLE_NORMALMAP
                float3 tSpace0 : TEXCOORD5;
				float3 tSpace1 : TEXCOORD6;
				float3 tSpace2 : TEXCOORD7;
                #else
                float3 normal : TEXCOORD5;
                #endif
			};

			sampler2D _BaseText;
			float4 _BaseText_ST;

            sampler2D _NormalMap;
            sampler2D _SpecMask;
            sampler2D _MainTex;

            float _SpecFactor;
            float3 _SpecDir;
            float3 _SpecLightColor;
            float _SpecTint;
            half _ExpoFactor;
            float _DiffFactor;
            float3 _SellColor;
            float _SellIntensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BaseText);
                o.lightDir = WorldSpaceLightDir(v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                #if ENABLE_NORMALMAP
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
                o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, 0);
                o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, 0);
                o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, 0);
                #else
                o.normal = worldNormal;
                #endif
                
                #ifndef DYNAMICLIGHTMAP_OFF
                o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                #ifndef LIGHTMAP_OFF
                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 difftex = tex2D(_BaseText, i.uv);
                half4 col = 0;
                fixed spec = tex2D(_SpecMask, i.uv);
                half3 normal;
                
                #if ENABLE_NORMALMAP
                half3 normaltex = UnpackNormal(tex2D(_NormalMap, i.uv));
                normal.x = dot(i.tSpace0.xyz, normaltex);
                normal.y = dot(i.tSpace1.xyz, normaltex);
                normal.z = dot(i.tSpace2.xyz, normaltex);
                normal = normalize(normal);
                #else
                normal = normalize(i.normal);
                #endif

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

                half3 viewDir = normalize(i.viewDir);
                half3 halfDir = normalize(viewDir + _SpecDir);
	            half nh = dot(normal, halfDir);
                half3 iSpec =  lerp(difftex.rgb, _SpecLightColor, _SpecTint) * (pow(max(0, nh), spec * _SpecFactor) * _ExpoFactor * spec);
                col.rgb +=  max(0, iSpec);
                col.rgb += saturate(1 - tex2D(_MainTex, i.uv).a) * _SellIntensity * difftex.rgb *_SellColor;
                #ifndef LIGHTMAP_OFF
                    half nl = dot(normal, i.lightDir);
                    half3 iDiff = difftex.rgb * _LightColor0 * (nl * _DiffFactor);
                    col.rgb += iDiff;
                #endif

                #ifndef LIGHTMAP_OFF
                #ifdef SHADOWS_SCREEN
                    #if defined(UNITY_NO_RGBM)
                    col.rgb += difftex.rgb * min(lm, atten*2);
                    #else
                    col.rgb += difftex.rgb * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
                    #endif
                #else // SHADOWS_SCREEN
                    col.rgb += difftex.rgb * lm;
                #endif // SHADOWS_SCREEN
                #endif // LIGHTMAP_OFF

                #ifndef DYNAMICLIGHTMAP_OFF
                fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, i.lmap.zw);
                col.rgb += difftex.rgb * DecodeRealtimeLightmap (dynlmtex);
                #endif
                UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}

        Pass
		{
            Name "FORWARD"
		    Tags { "RenderType"="Opaque" "LightMode"="ForwardAdd"}
		    ZWrite Off 
            Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma shader_feature ENABLE_NORMALMAP

			#include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
                #if ENABLE_NORMALMAP
                float4 tangent : TANGENT;
                #endif

			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                #if ENABLE_NORMALMAP
                float3 tSpace0 : TEXCOORD3;
				float3 tSpace1 : TEXCOORD4;
				float3 tSpace2 : TEXCOORD5;
                #else
                float3 normal : TEXCOORD3;
                #endif
			};

			sampler2D _BaseText;
			float4 _BaseText_ST;

            sampler2D _NormalMap;

            sampler2D _SpecMask;
            float _SpecFactor;
            float _ExpoFactor;
            float _DiffFactor;


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BaseText);
                o.lightDir = WorldSpaceLightDir(v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                #if ENABLE_NORMALMAP
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
                o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, 0);
                o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, 0);
                o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, 0);
                #else
                o.normal = worldNormal;
                #endif
                o.normal = worldNormal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 difftex = tex2D(_BaseText, i.uv);
                half4 col = difftex;
                fixed spec = tex2D(_SpecMask, i.uv);
                half3 normal;
                
                #if ENABLE_NORMALMAP
                half3 normaltex = UnpackNormal(tex2D(_NormalMap, i.uv));
                normal.x = dot(i.tSpace0.xyz, normaltex);
                normal.y = dot(i.tSpace1.xyz, normaltex);
                normal.z = dot(i.tSpace2.xyz, normaltex);
                normal = normalize(normal);
                #else
                normal = normalize(i.normal);
                #endif

                half nl = dot(normal, i.lightDir);
                half3 iDiff = difftex.rgb * _LightColor0 * (nl * _DiffFactor);
                col.rgb = iDiff;
				return col;
			}
			ENDCG
		}


        Pass
	    {
            Name "Meta"
	        Tags { "RenderType"="Opaque" "LightMode"="Meta"}
            Cull Back
		    CGPROGRAM
            #pragma vertex vert_surf
            #pragma fragment frag_surf
            #pragma shader_feature ENABLE_NORMALMAP
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
            sampler2D _BaseText;
			float4 _BaseText_ST;
            sampler2D _NormalMap;
            sampler2D _SpecMask;
            sampler2D _MainTex;

            float _SpecFactor;
            float _ExpoFactor;
            float _DiffFactor;
            float3 _SellColor;
            float _SellIntensity;
            v2f vert_surf (appdata_full v) 
            {
                v2f o;
			    o.vertex = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
			    o.uv = TRANSFORM_TEX(v.texcoord, _BaseText);
                
			    return o;
            }

            fixed4 frag_surf (v2f IN) : SV_Target {
              UnityMetaInput metaIN;
              UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
              float4 albedo = tex2D(_BaseText, IN.uv);
              metaIN.Albedo = albedo;
              metaIN.Emission = (1 - tex2D(_MainTex, IN.uv).a) * _SellIntensity *_SellColor * albedo;
              return UnityMetaFragment(metaIN);
            }

            ENDCG
        }
	}

}
