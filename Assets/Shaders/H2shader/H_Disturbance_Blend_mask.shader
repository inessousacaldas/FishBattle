// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.4527383,fgcb:0.4411765,fgca:1,fgde:0.01,fgrn:-43.8,fgrf:384.7,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32707,y:32733,varname:node_4795,prsc:2|spec-9978-OUT,emission-2393-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31677,y:33141,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ca82661e8ff17d34f9cc276699281f86,ntxv:0,isnm:False|UVIN-1540-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32164,y:33014,varname:node_2393,prsc:2|A-4903-OUT,B-2053-RGB,C-797-RGB,D-6074-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31509,y:32823,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31509,y:32981,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:798,x:32249,y:33182,varname:node_798,prsc:2|A-2053-A,B-797-A,C-6074-A,D-5988-OUT,E-8399-R;n:type:ShaderForge.SFN_Tex2d,id:9704,x:30057,y:32723,ptovrint:False,ptlb:Noise 01,ptin:_Noise01,varname:node_9704,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9b173c0d6ac89c44898e6fd7b99a15f,ntxv:0,isnm:False|UVIN-2778-OUT;n:type:ShaderForge.SFN_Append,id:2778,x:29867,y:32692,varname:node_2778,prsc:2|A-8813-OUT,B-6750-OUT;n:type:ShaderForge.SFN_TexCoord,id:8230,x:29503,y:32639,varname:node_8230,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:8813,x:29687,y:32617,varname:node_8813,prsc:2|A-2506-OUT,B-8230-U;n:type:ShaderForge.SFN_Multiply,id:2506,x:29455,y:32557,varname:node_2506,prsc:2|A-9982-TSL,B-5869-OUT;n:type:ShaderForge.SFN_Time,id:9982,x:29030,y:32751,varname:node_9982,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5869,x:29253,y:32720,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_5869,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:6750,x:29671,y:32802,varname:node_6750,prsc:2|A-8230-V,B-9086-OUT;n:type:ShaderForge.SFN_Multiply,id:9086,x:29420,y:32801,varname:node_9086,prsc:2|A-9982-TSL,B-3622-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3622,x:29247,y:32968,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:_node_5869_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:3853,x:30834,y:32935,varname:node_3853,prsc:2|A-8424-OUT,B-3094-OUT;n:type:ShaderForge.SFN_Add,id:1540,x:31071,y:32897,varname:node_1540,prsc:2|A-6037-UVOUT,B-3853-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4903,x:31503,y:32781,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_4903,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:5494,x:30054,y:33155,ptovrint:False,ptlb:Noise 02,ptin:_Noise02,varname:_node_9704_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False|UVIN-9617-OUT;n:type:ShaderForge.SFN_Append,id:9617,x:29843,y:33175,varname:node_9617,prsc:2|A-3085-OUT,B-1401-OUT;n:type:ShaderForge.SFN_TexCoord,id:4987,x:29467,y:33214,varname:node_4987,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:3085,x:29663,y:33100,varname:node_3085,prsc:2|A-9194-OUT,B-4987-U;n:type:ShaderForge.SFN_Multiply,id:9194,x:29442,y:33074,varname:node_9194,prsc:2|A-9982-TSL,B-5069-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5069,x:29240,y:33237,ptovrint:False,ptlb:U_Speed_copy,ptin:_U_Speed_copy,varname:_U_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:1401,x:29635,y:33377,varname:node_1401,prsc:2|A-4987-V,B-8884-OUT;n:type:ShaderForge.SFN_Multiply,id:8884,x:29384,y:33376,varname:node_8884,prsc:2|A-9982-TSL,B-4099-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4099,x:29182,y:33539,ptovrint:False,ptlb:V_Speed_copy,ptin:_V_Speed_copy,varname:_V_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:1243,x:30318,y:32856,varname:node_1243,prsc:2|A-9704-RGB,B-5494-RGB;n:type:ShaderForge.SFN_ComponentMask,id:8424,x:30595,y:32956,varname:node_8424,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-1243-OUT;n:type:ShaderForge.SFN_TexCoord,id:6037,x:30809,y:32721,varname:node_6037,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:8399,x:31786,y:33482,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_8399,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d74d58de69767bc4c96e00b40bec1fbb,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:5988,x:31759,y:33323,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_5988,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:3094,x:30702,y:33145,varname:node_3094,prsc:2|A-8296-OUT,B-6709-OUT;n:type:ShaderForge.SFN_Vector1,id:6709,x:30563,y:33438,varname:node_6709,prsc:2,v1:0.01;n:type:ShaderForge.SFN_ValueProperty,id:8296,x:30464,y:33358,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_8296,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:20;n:type:ShaderForge.SFN_ValueProperty,id:9978,x:32408,y:32581,ptovrint:False,ptlb:Normal_specular,ptin:_Normal_specular,varname:node_9978,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;proporder:6074-797-4903-5988-8296-9704-5869-3622-5494-5069-4099-8399-9978;pass:END;sub:END;*/

Shader "H2/H_Disturbance_Blend_Mask" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (1,1,1,1)
        _ZT ("ZT", Float ) = 1
        _Alpha ("Alpha", Float ) = 1
        _Disturbance ("Disturbance", Float ) = 20
        _Noise01 ("Noise 01", 2D) = "white" {}
        _U_Speed ("U_Speed", Float ) = 2
        _V_Speed ("V_Speed", Float ) = 3
        _Noise02 ("Noise 02", 2D) = "white" {}
        _U_Speed_copy ("U_Speed_copy", Float ) = 1
        _V_Speed_copy ("V_Speed_copy", Float ) = 2
        _Mask ("Mask", 2D) = "white" {}
        _Normal_specular ("Normal_specular", Float ) = 0.2
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
          //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise01; uniform float4 _Noise01_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float _ZT;
            uniform sampler2D _Noise02; uniform float4 _Noise02_ST;
            uniform float _U_Speed_copy;
            uniform float _V_Speed_copy;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Alpha;
            uniform float _Disturbance;
            uniform float _Normal_specular;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Normal_specular,_Normal_specular,_Normal_specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
////// Emissive:
                float4 node_9982 = _Time + _TimeEditor;
                float2 node_2778 = float2(((node_9982.r*_U_Speed)+i.uv0.r),(i.uv0.g+(node_9982.r*_V_Speed)));
                float4 _Noise01_var = tex2D(_Noise01,TRANSFORM_TEX(node_2778, _Noise01));
                float2 node_9617 = float2(((node_9982.r*_U_Speed_copy)+i.uv0.r),(i.uv0.g+(node_9982.r*_V_Speed_copy)));
                float4 _Noise02_var = tex2D(_Noise02,TRANSFORM_TEX(node_9617, _Noise02));
                float2 node_1540 = (i.uv0+((_Noise01_var.rgb*_Noise02_var.rgb).r*(_Disturbance*0.01)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1540, _MainTex));
                float3 emissive = (_ZT*i.vertexColor.rgb*_TintColor.rgb*_MainTex_var.rgb);
/// Final Color:
                float3 finalColor = specular + emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                fixed4 finalRGBA = fixed4(finalColor,(i.vertexColor.a*_TintColor.a*_MainTex_var.a*_Alpha*_Mask_var.r));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise01; uniform float4 _Noise01_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float _ZT;
            uniform sampler2D _Noise02; uniform float4 _Noise02_ST;
            uniform float _U_Speed_copy;
            uniform float _V_Speed_copy;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Alpha;
            uniform float _Disturbance;
            uniform float _Normal_specular;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Normal_specular,_Normal_specular,_Normal_specular);
                float3 directSpecular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/// Final Color:
                float3 finalColor = specular;
                float4 node_9982 = _Time + _TimeEditor;
                float2 node_2778 = float2(((node_9982.r*_U_Speed)+i.uv0.r),(i.uv0.g+(node_9982.r*_V_Speed)));
                float4 _Noise01_var = tex2D(_Noise01,TRANSFORM_TEX(node_2778, _Noise01));
                float2 node_9617 = float2(((node_9982.r*_U_Speed_copy)+i.uv0.r),(i.uv0.g+(node_9982.r*_V_Speed_copy)));
                float4 _Noise02_var = tex2D(_Noise02,TRANSFORM_TEX(node_9617, _Noise02));
                float2 node_1540 = (i.uv0+((_Noise01_var.rgb*_Noise02_var.rgb).r*(_Disturbance*0.01)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1540, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                fixed4 finalRGBA = fixed4(finalColor * (i.vertexColor.a*_TintColor.a*_MainTex_var.a*_Alpha*_Mask_var.r),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
