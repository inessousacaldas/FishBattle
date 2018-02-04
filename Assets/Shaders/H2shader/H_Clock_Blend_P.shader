// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32660,y:32418,varname:node_4795,prsc:2|emission-9423-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Multiply,id:798,x:32462,y:32741,varname:node_798,prsc:2|A-6241-OUT,B-1697-OUT;n:type:ShaderForge.SFN_TexCoord,id:2257,x:29975,y:32634,varname:node_2257,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:5993,x:30197,y:32634,varname:node_5993,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2257-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:3461,x:30389,y:32634,varname:node_3461,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5993-OUT;n:type:ShaderForge.SFN_RemapRange,id:2472,x:30766,y:32650,varname:node_2472,prsc:2,frmn:-3.14,frmx:3.14,tomn:0,tomx:1|IN-4542-OUT;n:type:ShaderForge.SFN_ArcTan2,id:4542,x:30584,y:32650,varname:node_4542,prsc:2,attp:0|A-3461-G,B-3461-R;n:type:ShaderForge.SFN_Tex2d,id:9831,x:31362,y:32829,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_3202,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3763,x:31621,y:32806,varname:node_3763,prsc:2|A-8492-OUT,B-9831-RGB,C-1432-RGB,D-2304-OUT;n:type:ShaderForge.SFN_Color,id:4889,x:32179,y:32454,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:2304,x:31362,y:33175,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_861,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:9152,x:31036,y:32645,varname:node_9152,prsc:2|A-2472-OUT,B-9996-OUT;n:type:ShaderForge.SFN_Multiply,id:728,x:31242,y:32645,varname:node_728,prsc:2|A-9152-OUT,B-6058-OUT;n:type:ShaderForge.SFN_Clamp01,id:8492,x:31427,y:32645,varname:node_8492,prsc:2|IN-728-OUT;n:type:ShaderForge.SFN_RemapRange,id:9996,x:30766,y:32837,varname:node_9996,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1.3|IN-1139-A;n:type:ShaderForge.SFN_VertexColor,id:1139,x:30584,y:32816,varname:node_1139,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:6058,x:31036,y:32811,ptovrint:False,ptlb:Bian,ptin:_Bian,varname:node_9945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:30;n:type:ShaderForge.SFN_ComponentMask,id:6241,x:32010,y:32865,varname:node_6241,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3763-OUT;n:type:ShaderForge.SFN_VertexColor,id:1432,x:31362,y:33001,varname:node_1432,prsc:2;n:type:ShaderForge.SFN_Desaturate,id:2082,x:32036,y:32641,varname:node_2082,prsc:2|COL-3763-OUT;n:type:ShaderForge.SFN_Multiply,id:9423,x:32429,y:32502,varname:node_9423,prsc:2|A-4889-RGB,B-188-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1697,x:32255,y:32961,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_1697,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:188,x:32309,y:32656,ptovrint:False,ptlb:Color Switch,ptin:_ColorSwitch,varname:node_188,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3763-OUT,B-2082-OUT;proporder:9831-188-4889-2304-1697-6058;pass:END;sub:END;*/

Shader "H2/H_Clock_Blend_P" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        [MaterialToggle] _ColorSwitch ("Color Switch", Float ) = 0
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _ZT ("ZT", Float ) = 2
        _Alpha ("Alpha", Float ) = 1
        _Bian ("Bian", Float ) = 30
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
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform float4 _Color;
            uniform float _ZT;
            uniform float _Bian;
            uniform float _Alpha;
            uniform fixed _ColorSwitch;
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
                float2 node_3461 = (i.uv0*2.0+-1.0).rg;
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(i.uv0, _TEX));
                float3 node_3763 = (saturate((((atan2(node_3461.g,node_3461.r)*0.1592357+0.5)+(i.vertexColor.a*2.3+-1.0))*_Bian))*_TEX_var.rgb*i.vertexColor.rgb*_ZT);
                float3 emissive = (_Color.rgb*lerp( node_3763, dot(node_3763,float3(0.3,0.59,0.11)), _ColorSwitch ));
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_3763.r*_Alpha));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
