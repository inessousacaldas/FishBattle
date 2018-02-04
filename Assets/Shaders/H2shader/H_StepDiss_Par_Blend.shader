// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-2393-OUT,alpha-1212-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31549,y:32598,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:732c95a8d3a922543bdeb51bf025adc0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32411,y:32779,varname:node_2393,prsc:2|A-7339-OUT,B-2053-RGB,C-797-RGB,D-7050-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31184,y:32901,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31916,y:32818,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:3301,x:31845,y:33011,varname:node_3301,prsc:2|A-2233-OUT,B-669-OUT;n:type:ShaderForge.SFN_Tex2d,id:7566,x:31461,y:33012,ptovrint:False,ptlb:Diss_Tex,ptin:_Diss_Tex,varname:node_7566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6475569e57de23843b4c52854998a6af,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6181,x:31042,y:33274,ptovrint:False,ptlb:Diss,ptin:_Diss,varname:node_6181,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1212,x:32407,y:33002,varname:node_1212,prsc:2|A-9479-OUT,B-884-OUT,C-6074-A,D-4945-R,E-4945-A;n:type:ShaderForge.SFN_ValueProperty,id:884,x:32150,y:33076,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_884,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Desaturate,id:8172,x:31789,y:32495,varname:node_8172,prsc:2|COL-6074-RGB;n:type:ShaderForge.SFN_ComponentMask,id:9479,x:32017,y:32997,varname:node_9479,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3301-OUT;n:type:ShaderForge.SFN_OneMinus,id:2233,x:31648,y:33009,varname:node_2233,prsc:2|IN-7566-RGB;n:type:ShaderForge.SFN_OneMinus,id:669,x:31709,y:33189,varname:node_669,prsc:2|IN-9239-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9239,x:31473,y:33255,ptovrint:False,ptlb:Diss_Swith,ptin:_Diss_Swith,varname:node_9239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2881-OUT,B-6181-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7339,x:32023,y:32654,ptovrint:False,ptlb:Color_Swith,ptin:_Color_Swith,varname:node_7339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8172-OUT,B-6074-RGB;n:type:ShaderForge.SFN_OneMinus,id:2881,x:31318,y:33111,varname:node_2881,prsc:2|IN-2053-A;n:type:ShaderForge.SFN_Tex2d,id:4945,x:32116,y:33189,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:7050,x:32255,y:32917,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_7050,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:6074-7339-797-7050-884-9239-7566-6181-4945;pass:END;sub:END;*/

Shader "H2/H_StepDiss_Par_Blend" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _Color_Swith ("Color_Swith", Float ) = 0
        _TintColor ("Color", Color) = (1,1,1,1)
        _ZT ("ZT", Float ) = 1
        _Alpha ("Alpha", Float ) = 1
        [MaterialToggle] _Diss_Swith ("Diss_Swith", Float ) = 0
        _Diss_Tex ("Diss_Tex", 2D) = "white" {}
        _Diss ("Diss", Range(0, 1)) = 0
        _Mask ("Mask", 2D) = "white" {}
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
            //#pragma multi_compile_fog
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Diss_Tex; uniform float4 _Diss_Tex_ST;
            uniform float _Diss;
            uniform float _Alpha;
            uniform fixed _Diss_Swith;
            uniform fixed _Color_Swith;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _ZT;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (lerp( dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)), _MainTex_var.rgb, _Color_Swith )*i.vertexColor.rgb*_TintColor.rgb*_ZT);
                float3 finalColor = emissive;
                float4 _Diss_Tex_var = tex2D(_Diss_Tex,TRANSFORM_TEX(i.uv0, _Diss_Tex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                fixed4 finalRGBA = fixed4(finalColor,(step((1.0 - _Diss_Tex_var.rgb),(1.0 - lerp( (1.0 - i.vertexColor.a), _Diss, _Diss_Swith ))).r*_Alpha*_MainTex_var.a*_Mask_var.r*_Mask_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
