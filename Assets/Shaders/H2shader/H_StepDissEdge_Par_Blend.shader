// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-2393-OUT,alpha-1212-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:30436,y:32222,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32369,y:32667,varname:node_2393,prsc:2|A-7086-OUT,B-2053-RGB,C-7050-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:28535,y:32209,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31195,y:32493,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:3301,x:30790,y:32770,varname:node_3301,prsc:2|A-2233-OUT,B-669-OUT;n:type:ShaderForge.SFN_Tex2d,id:7566,x:29649,y:32558,ptovrint:False,ptlb:Diss_Tex,ptin:_Diss_Tex,varname:node_7566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6475569e57de23843b4c52854998a6af,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6181,x:29002,y:32748,ptovrint:False,ptlb:Diss,ptin:_Diss,varname:node_6181,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3689956,max:1;n:type:ShaderForge.SFN_Multiply,id:1212,x:32407,y:33002,varname:node_1212,prsc:2|A-9479-OUT,B-884-OUT,C-4945-R,D-4945-A;n:type:ShaderForge.SFN_ValueProperty,id:884,x:32112,y:33070,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_884,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Desaturate,id:8172,x:30701,y:32120,varname:node_8172,prsc:2|COL-6074-RGB;n:type:ShaderForge.SFN_ComponentMask,id:9479,x:31949,y:32993,varname:node_9479,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3301-OUT;n:type:ShaderForge.SFN_OneMinus,id:2233,x:29853,y:32561,varname:node_2233,prsc:2|IN-7566-RGB;n:type:ShaderForge.SFN_OneMinus,id:669,x:29668,y:32735,varname:node_669,prsc:2|IN-9239-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9239,x:29446,y:32731,ptovrint:False,ptlb:Diss_Swith,ptin:_Diss_Swith,varname:node_9239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2881-OUT,B-6181-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7339,x:30921,y:32201,ptovrint:False,ptlb:Color_Swith,ptin:_Color_Swith,varname:node_7339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8172-OUT,B-6074-RGB;n:type:ShaderForge.SFN_OneMinus,id:2881,x:29267,y:32586,varname:node_2881,prsc:2|IN-2053-A;n:type:ShaderForge.SFN_Tex2d,id:4945,x:32116,y:33189,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:7050,x:32133,y:32771,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_7050,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Step,id:1949,x:30431,y:32648,varname:node_1949,prsc:2|A-2233-OUT,B-7048-OUT;n:type:ShaderForge.SFN_Slider,id:7679,x:29722,y:32860,ptovrint:False,ptlb:Edge,ptin:_Edge,varname:node_7679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Subtract,id:3378,x:31176,y:32727,varname:node_3378,prsc:2|A-3301-OUT,B-1949-OUT;n:type:ShaderForge.SFN_Subtract,id:7048,x:30179,y:32783,varname:node_7048,prsc:2|A-669-OUT,B-7679-OUT;n:type:ShaderForge.SFN_Multiply,id:9715,x:31439,y:32319,varname:node_9715,prsc:2|A-7339-OUT,B-7178-OUT,C-797-RGB;n:type:ShaderForge.SFN_Add,id:7086,x:31947,y:32474,varname:node_7086,prsc:2|A-9715-OUT,B-8220-OUT;n:type:ShaderForge.SFN_Multiply,id:8220,x:31611,y:32721,varname:node_8220,prsc:2|A-7062-OUT,B-5759-RGB;n:type:ShaderForge.SFN_Color,id:5759,x:31343,y:32986,ptovrint:False,ptlb:EdgeColor,ptin:_EdgeColor,varname:node_5759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:2,c2:1,c3:3,c4:1;n:type:ShaderForge.SFN_Desaturate,id:7178,x:31035,y:32496,varname:node_7178,prsc:2|COL-1949-OUT;n:type:ShaderForge.SFN_Desaturate,id:7062,x:31393,y:32774,varname:node_7062,prsc:2|COL-3378-OUT;proporder:6074-7339-797-7679-5759-7050-884-9239-7566-6181-4945;pass:END;sub:END;*/

Shader "H2/H_StepDissEdge_Par_Blend" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _Color_Swith ("Color_Swith", Float ) = 0
        _TintColor ("Color", Color) = (1,1,1,1)
        _Edge ("Edge", Range(0, 0.1)) = 0
        _EdgeColor ("EdgeColor", Color) = (2,1,3,1)
        _ZT ("ZT", Float ) = 1
        _Alpha ("Alpha", Float ) = 1
        [MaterialToggle] _Diss_Swith ("Diss_Swith", Float ) = 0.3689956
        _Diss_Tex ("Diss_Tex", 2D) = "white" {}
        _Diss ("Diss", Range(0, 1)) = 0.3689956
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
            Colormask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Diss_Tex; uniform float4 _Diss_Tex_ST;
            uniform float _Diss;
            uniform float _Alpha;
            uniform fixed _Diss_Swith;
            uniform fixed _Color_Swith;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _ZT;
            uniform float _Edge;
            uniform float4 _EdgeColor;
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
                float3 node_2233 = (1.0 - _Diss_Tex_var.rgb);
                float node_669 = (1.0 - lerp( (1.0 - i.vertexColor.a), _Diss, _Diss_Swith ));
                float3 node_1949 = step(node_2233,(node_669-_Edge));
                float3 node_3301 = step(node_2233,node_669);
                float3 emissive = (((lerp( dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)), _MainTex_var.rgb, _Color_Swith )*dot(node_1949,float3(0.3,0.59,0.11))*_TintColor.rgb)+(dot((node_3301-node_1949),float3(0.3,0.59,0.11))*_EdgeColor.rgb))*i.vertexColor.rgb*_ZT);
                float3 finalColor = emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                fixed4 finalRGBA = fixed4(finalColor,(node_3301.r*_Alpha*_Mask_var.r*_Mask_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
