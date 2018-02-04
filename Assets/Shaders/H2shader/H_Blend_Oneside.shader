// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:1,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:721,x:33720,y:32797,varname:node_721,prsc:2|emission-8625-OUT,alpha-3826-OUT;n:type:ShaderForge.SFN_Tex2d,id:6209,x:32768,y:32641,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_6209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:22b694789706b2d41888c16d8c7b2c85,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8625,x:33272,y:32811,varname:node_8625,prsc:2|A-6209-RGB,B-161-RGB,C-3739-RGB,D-1459-OUT,E-5900-RGB;n:type:ShaderForge.SFN_Multiply,id:3826,x:33231,y:33048,varname:node_3826,prsc:2|A-6209-A,B-161-A,C-3739-A,D-1482-OUT,E-5900-A;n:type:ShaderForge.SFN_VertexColor,id:161,x:32763,y:32818,varname:node_161,prsc:2;n:type:ShaderForge.SFN_Color,id:3739,x:32746,y:32975,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3739,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1482,x:32782,y:33270,ptovrint:False,ptlb:ALPHA,ptin:_ALPHA,varname:node_1482,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:1459,x:32774,y:33176,ptovrint:False,ptlb:light,ptin:_light,varname:node_1459,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2d,id:5900,x:32790,y:33391,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:node_5900,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:6209-3739-1482-1459-5900;pass:END;sub:END;*/

Shader "H2/H_Blend_Oneside" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _ALPHA ("ALPHA", Float ) = 2
        _light ("light", Float ) = 2
        _MASK ("MASK", 2D) = "white" {}
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
            Cull Front
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform float4 _Color;
            uniform float _ALPHA;
            uniform float _light;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
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
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(i.uv0, _TEX));
                float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
                float3 emissive = (_TEX_var.rgb*i.vertexColor.rgb*_Color.rgb*_light*_MASK_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_TEX_var.a*i.vertexColor.a*_Color.a*_ALPHA*_MASK_var.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
