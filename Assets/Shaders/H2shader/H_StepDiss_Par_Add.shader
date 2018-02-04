// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32758,y:32675,varname:node_4795,prsc:2|emission-6068-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31559,y:32659,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:732c95a8d3a922543bdeb51bf025adc0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32343,y:32779,varname:node_2393,prsc:2|A-7339-OUT,B-2053-RGB,C-797-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:30765,y:32918,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32091,y:32823,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:3301,x:31707,y:32990,varname:node_3301,prsc:2|A-2233-OUT,B-9239-OUT;n:type:ShaderForge.SFN_Tex2d,id:7566,x:31323,y:32991,ptovrint:False,ptlb:Diss_Tex,ptin:_Diss_Tex,varname:node_7566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6475569e57de23843b4c52854998a6af,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6181,x:30670,y:33238,ptovrint:False,ptlb:Diss,ptin:_Diss,varname:node_6181,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:1212,x:32262,y:32988,varname:node_1212,prsc:2|A-9479-OUT,B-884-OUT,C-6074-A;n:type:ShaderForge.SFN_ValueProperty,id:884,x:32050,y:33036,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_884,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Desaturate,id:8172,x:31885,y:32527,varname:node_8172,prsc:2|COL-6074-RGB;n:type:ShaderForge.SFN_ComponentMask,id:9479,x:31896,y:33003,varname:node_9479,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3301-OUT;n:type:ShaderForge.SFN_OneMinus,id:2233,x:31510,y:32988,varname:node_2233,prsc:2|IN-7566-RGB;n:type:ShaderForge.SFN_OneMinus,id:669,x:31637,y:33141,varname:node_669,prsc:2|IN-9239-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9239,x:31230,y:33224,ptovrint:False,ptlb:Diss_Swith,ptin:_Diss_Swith,varname:node_9239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2053-A,B-6181-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7339,x:32119,y:32686,ptovrint:False,ptlb:Color_Swith,ptin:_Color_Swith,varname:node_7339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8172-OUT,B-6074-RGB;n:type:ShaderForge.SFN_Multiply,id:6068,x:32560,y:32836,varname:node_6068,prsc:2|A-2393-OUT,B-1212-OUT,C-3744-RGB,D-3744-A;n:type:ShaderForge.SFN_Tex2d,id:3744,x:32479,y:33176,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_3744,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:6074-797-7339-7566-9239-6181-884-3744;pass:END;sub:END;*/

Shader "H2/H_StepDiss_Par_Add" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (1,1,1,1)
        [MaterialToggle] _Color_Swith ("Color_Swith", Float ) = 0
        _Diss_Tex ("Diss_Tex", 2D) = "white" {}
        [MaterialToggle] _Diss_Swith ("Diss_Swith", Float ) = 1
        _Diss ("Diss", Range(0, 1)) = 1
        _Alpha ("Alpha", Float ) = 1
        _Mask ("Mask", 2D) = "white" {}
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
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
           // #pragma multi_compile_fog
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
                float4 _Diss_Tex_var = tex2D(_Diss_Tex,TRANSFORM_TEX(i.uv0, _Diss_Tex));
                float _Diss_Swith_var = lerp( i.vertexColor.a, _Diss, _Diss_Swith );
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = ((lerp( dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)), _MainTex_var.rgb, _Color_Swith )*i.vertexColor.rgb*_TintColor.rgb)*(step((1.0 - _Diss_Tex_var.rgb),_Diss_Swith_var).r*_Alpha*_MainTex_var.a)*_Mask_var.rgb*_Mask_var.a);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
