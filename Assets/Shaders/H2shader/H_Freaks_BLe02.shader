// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32927,y:32526,varname:node_4795,prsc:2|emission-2393-OUT,alpha-798-OUT,clip-548-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31293,y:32506,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32657,y:32578,varname:node_2393,prsc:2|A-9774-OUT,B-2053-RGB,C-4781-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31949,y:32730,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31645,y:32845,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:798,x:32327,y:32865,varname:node_798,prsc:2|A-1680-OUT,B-2053-A;n:type:ShaderForge.SFN_Fresnel,id:2488,x:31274,y:32075,varname:node_2488,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9737,x:31933,y:32293,varname:node_9737,prsc:2|A-9886-OUT,B-2534-OUT,C-2978-RGB;n:type:ShaderForge.SFN_ValueProperty,id:2534,x:31544,y:32289,ptovrint:False,ptlb:Fresnel_ZT,ptin:_Fresnel_ZT,varname:node_2534,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ValueProperty,id:1111,x:31266,y:32229,ptovrint:False,ptlb:Fresnel_Power,ptin:_Fresnel_Power,varname:node_1111,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Add,id:9774,x:32313,y:32479,varname:node_9774,prsc:2|A-44-OUT,B-5316-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4781,x:32440,y:32756,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_4781,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:2978,x:31539,y:32367,ptovrint:False,ptlb:Fresnel_Color,ptin:_Fresnel_Color,varname:node_2978,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:5316,x:32017,y:32568,varname:node_5316,prsc:2|A-6074-RGB,B-797-RGB;n:type:ShaderForge.SFN_ValueProperty,id:1680,x:32114,y:32789,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_1680,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:9886,x:31560,y:32112,varname:node_9886,prsc:2|VAL-2488-OUT,EXP-1111-OUT;n:type:ShaderForge.SFN_Tex2d,id:9054,x:32029,y:33046,ptovrint:False,ptlb:DISS_TEX,ptin:_DISS_TEX,varname:node_9054,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f32d8cbdfa3df5e48a885c48e037a62b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Step,id:548,x:32336,y:32993,varname:node_548,prsc:2|A-9054-RGB,B-1095-OUT;n:type:ShaderForge.SFN_Slider,id:1095,x:31948,y:33249,ptovrint:False,ptlb:DISS,ptin:_DISS,varname:node_1095,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_SwitchProperty,id:44,x:32152,y:32395,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_44,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9737-OUT,B-6858-OUT;n:type:ShaderForge.SFN_Vector1,id:6858,x:31898,y:32497,varname:node_6858,prsc:2,v1:0;proporder:6074-797-4781-44-2978-2534-1111-1680-9054-1095;pass:END;sub:END;*/

Shader "H2/H_Freaks_BLe02" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _ZT ("ZT", Float ) = 1
        [MaterialToggle] _Fresnel ("Fresnel", Float ) = 0
        _Fresnel_Color ("Fresnel_Color", Color) = (0.5,0.5,0.5,1)
        _Fresnel_ZT ("Fresnel_ZT", Float ) = 3
        _Fresnel_Power ("Fresnel_Power", Float ) = 5
        _Alpha ("Alpha", Float ) = 1
        _DISS_TEX ("DISS_TEX", 2D) = "white" {}
        _DISS ("DISS", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _Fresnel_ZT;
            uniform float _Fresnel_Power;
            uniform float _ZT;
            uniform float4 _Fresnel_Color;
            uniform float _Alpha;
            uniform sampler2D _DISS_TEX; uniform float4 _DISS_TEX_ST;
            uniform float _DISS;
            uniform fixed _Fresnel;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _DISS_TEX_var = tex2D(_DISS_TEX,TRANSFORM_TEX(i.uv0, _DISS_TEX));
                clip(step(_DISS_TEX_var.rgb,_DISS) - 0.5);
////// Lighting:
////// Emissive:
                float node_9886 = pow((1.0-max(0,dot(normalDirection, viewDirection))),_Fresnel_Power);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((lerp( (node_9886*_Fresnel_ZT*_Fresnel_Color.rgb), 0.0, _Fresnel )+(_MainTex_var.rgb*_TintColor.rgb))*i.vertexColor.rgb*_ZT);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Alpha*i.vertexColor.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
