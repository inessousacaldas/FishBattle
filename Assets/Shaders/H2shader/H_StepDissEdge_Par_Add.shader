// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32857,y:32659,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31170,y:32461,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32448,y:32807,varname:node_2393,prsc:2|A-7086-OUT,B-2053-RGB,C-7050-OUT,D-9479-OUT,E-6074-A;n:type:ShaderForge.SFN_VertexColor,id:2053,x:29678,y:32720,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31754,y:32733,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:3301,x:31093,y:33069,varname:node_3301,prsc:2|A-2233-OUT,B-669-OUT;n:type:ShaderForge.SFN_Tex2d,id:7566,x:30120,y:32782,ptovrint:False,ptlb:Diss_Tex,ptin:_Diss_Tex,varname:node_7566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6475569e57de23843b4c52854998a6af,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6181,x:29739,y:33061,ptovrint:False,ptlb:Diss,ptin:_Diss,varname:node_6181,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3689956,max:1;n:type:ShaderForge.SFN_Desaturate,id:8172,x:31592,y:32422,varname:node_8172,prsc:2|COL-8995-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9479,x:31902,y:33075,varname:node_9479,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3301-OUT;n:type:ShaderForge.SFN_OneMinus,id:2233,x:30742,y:32828,varname:node_2233,prsc:2|IN-8991-OUT;n:type:ShaderForge.SFN_OneMinus,id:669,x:30393,y:33012,varname:node_669,prsc:2|IN-9239-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9239,x:30171,y:33008,ptovrint:False,ptlb:Diss_Swith,ptin:_Diss_Swith,varname:node_9239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2881-OUT,B-6181-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7339,x:31806,y:32500,ptovrint:False,ptlb:Color_Swith,ptin:_Color_Swith,varname:node_7339,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8172-OUT,B-8995-OUT;n:type:ShaderForge.SFN_OneMinus,id:2881,x:29893,y:32918,varname:node_2881,prsc:2|IN-2053-A;n:type:ShaderForge.SFN_ValueProperty,id:7050,x:32212,y:32911,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_7050,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Step,id:1949,x:30982,y:32822,varname:node_1949,prsc:2|A-2233-OUT,B-7048-OUT;n:type:ShaderForge.SFN_Slider,id:7679,x:30433,y:33292,ptovrint:False,ptlb:Edge,ptin:_Edge,varname:node_7679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.2;n:type:ShaderForge.SFN_Subtract,id:3378,x:31368,y:32908,varname:node_3378,prsc:2|A-3301-OUT,B-1949-OUT;n:type:ShaderForge.SFN_Subtract,id:7048,x:30730,y:32957,varname:node_7048,prsc:2|A-669-OUT,B-7679-OUT;n:type:ShaderForge.SFN_Multiply,id:9715,x:31998,y:32559,varname:node_9715,prsc:2|A-7339-OUT,B-7178-OUT,C-797-RGB;n:type:ShaderForge.SFN_Add,id:7086,x:32242,y:32682,varname:node_7086,prsc:2|A-9715-OUT,B-8220-OUT;n:type:ShaderForge.SFN_Multiply,id:8220,x:31807,y:32931,varname:node_8220,prsc:2|A-7062-OUT,B-5759-RGB;n:type:ShaderForge.SFN_Color,id:5759,x:31539,y:33196,ptovrint:False,ptlb:EdgeColor,ptin:_EdgeColor,varname:node_5759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Desaturate,id:7178,x:31594,y:32736,varname:node_7178,prsc:2|COL-1949-OUT;n:type:ShaderForge.SFN_Desaturate,id:7062,x:31598,y:32917,varname:node_7062,prsc:2|COL-3378-OUT;n:type:ShaderForge.SFN_Multiply,id:8991,x:30493,y:32829,varname:node_8991,prsc:2|A-7566-RGB,B-7566-A;n:type:ShaderForge.SFN_Multiply,id:8995,x:31383,y:32522,varname:node_8995,prsc:2|A-6074-RGB,B-6074-A;proporder:6074-7339-797-7679-5759-7050-9239-7566-6181;pass:END;sub:END;*/

Shader "H2/H_StepDissEdge_Par_Add" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _Color_Swith ("Color_Swith", Float ) = 0
        _TintColor ("Color", Color) = (1,1,1,1)
        _Edge ("Edge", Range(0, 0.2)) = 0
        _EdgeColor ("EdgeColor", Color) = (1,1,1,1)
        _ZT ("ZT", Float ) = 1
        [MaterialToggle] _Diss_Swith ("Diss_Swith", Float ) = 0.3689956
        _Diss_Tex ("Diss_Tex", 2D) = "white" {}
        _Diss ("Diss", Range(0, 1)) = 0.3689956
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
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Diss_Tex; uniform float4 _Diss_Tex_ST;
            uniform float _Diss;
            uniform fixed _Diss_Swith;
            uniform fixed _Color_Swith;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_8995 = (_MainTex_var.rgb*_MainTex_var.a);
                float4 _Diss_Tex_var = tex2D(_Diss_Tex,TRANSFORM_TEX(i.uv0, _Diss_Tex));
                float3 node_2233 = (1.0 - (_Diss_Tex_var.rgb*_Diss_Tex_var.a));
                float node_669 = (1.0 - lerp( (1.0 - i.vertexColor.a), _Diss, _Diss_Swith ));
                float3 node_1949 = step(node_2233,(node_669-_Edge));
                float3 node_3301 = step(node_2233,node_669);
                float3 emissive = (((lerp( dot(node_8995,float3(0.3,0.59,0.11)), node_8995, _Color_Swith )*dot(node_1949,float3(0.3,0.59,0.11))*_TintColor.rgb)+(dot((node_3301-node_1949),float3(0.3,0.59,0.11))*_EdgeColor.rgb))*i.vertexColor.rgb*_ZT*node_3301.r*_MainTex_var.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
