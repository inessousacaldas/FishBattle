// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5211,x:33076,y:32629,varname:node_5211,prsc:2|emission-3652-OUT;n:type:ShaderForge.SFN_TexCoord,id:7126,x:30742,y:32655,varname:node_7126,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:9842,x:30974,y:32655,varname:node_9842,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7126-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:4247,x:31156,y:32655,varname:node_4247,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9842-OUT;n:type:ShaderForge.SFN_RemapRange,id:3007,x:31564,y:32671,varname:node_3007,prsc:2,frmn:-3.14,frmx:3.14,tomn:0,tomx:1|IN-3908-OUT;n:type:ShaderForge.SFN_ArcTan2,id:3908,x:31382,y:32671,varname:node_3908,prsc:2,attp:0|A-4247-G,B-4247-R;n:type:ShaderForge.SFN_Tex2d,id:3202,x:32233,y:32774,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_3202,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3652,x:32774,y:32731,varname:node_3652,prsc:2|A-7426-OUT,B-2663-OUT,C-3236-RGB,D-861-OUT;n:type:ShaderForge.SFN_Color,id:3236,x:32233,y:32958,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:861,x:32233,y:33150,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_861,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:8178,x:31842,y:32647,varname:node_8178,prsc:2|A-3007-OUT,B-4991-OUT;n:type:ShaderForge.SFN_Multiply,id:4788,x:32030,y:32704,varname:node_4788,prsc:2|A-8178-OUT,B-9945-OUT;n:type:ShaderForge.SFN_Clamp01,id:7426,x:32233,y:32630,varname:node_7426,prsc:2|IN-4788-OUT;n:type:ShaderForge.SFN_RemapRange,id:4991,x:31686,y:32949,varname:node_4991,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:-1|IN-2956-A;n:type:ShaderForge.SFN_VertexColor,id:2956,x:31382,y:32846,varname:node_2956,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9945,x:31842,y:32796,ptovrint:False,ptlb:Bian,ptin:_Bian,varname:node_9945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:15;n:type:ShaderForge.SFN_Multiply,id:2663,x:32502,y:32775,varname:node_2663,prsc:2|A-3202-RGB,B-3202-A,C-2956-RGB;proporder:3202-3236-861-9945;pass:END;sub:END;*/

Shader "H2/H_Clock_add" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _ZT ("ZT", Float ) = 2
        _Bian ("Bian", Float ) = 15
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
           // #pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform float4 _Color;
            uniform float _ZT;
            uniform float _Bian;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float2 node_4247 = (i.uv0*2.0+-1.0).rg;
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(i.uv0, _TEX));
                float3 emissive = (saturate((((atan2(node_4247.g,node_4247.r)*0.1592357+0.5)+(i.vertexColor.a*-1.5+0.5))*_Bian))*(_TEX_var.rgb*_TEX_var.a*i.vertexColor.rgb)*_Color.rgb*_ZT);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    //Fallback "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
