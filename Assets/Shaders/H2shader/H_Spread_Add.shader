// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2975,x:33345,y:32658,varname:node_2975,prsc:2|emission-5042-OUT;n:type:ShaderForge.SFN_Tex2d,id:8590,x:32245,y:32586,ptovrint:False,ptlb:Spread_TEX,ptin:_Spread_TEX,varname:node_8590,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5628-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7893,x:30714,y:32479,varname:node_7893,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:5498,x:30658,y:32731,varname:node_5498,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:5826,x:31655,y:32586,varname:node_5826,prsc:2|A-3842-OUT,B-5498-OUT,T-3734-A;n:type:ShaderForge.SFN_Add,id:3842,x:31463,y:32511,varname:node_3842,prsc:2|A-7893-UVOUT,B-1410-OUT;n:type:ShaderForge.SFN_Subtract,id:801,x:30959,y:32579,varname:node_801,prsc:2|A-7893-UVOUT,B-5498-OUT;n:type:ShaderForge.SFN_Divide,id:1410,x:31195,y:32561,varname:node_1410,prsc:2|A-801-OUT,B-3734-A;n:type:ShaderForge.SFN_Clamp01,id:10,x:31857,y:32586,varname:node_10,prsc:2|IN-5826-OUT;n:type:ShaderForge.SFN_VertexColor,id:3734,x:30710,y:32835,varname:node_3734,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7579,x:32705,y:32777,varname:node_7579,prsc:2|A-8876-OUT,B-3584-RGB,C-4409-RGB,D-1158-RGB,E-8590-A;n:type:ShaderForge.SFN_Tex2d,id:3584,x:32292,y:32870,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_3584,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4409,x:32292,y:33057,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4409,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5046,x:32907,y:32992,ptovrint:False,ptlb:Lighting Levels,ptin:_LightingLevels,varname:node_5046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:1158,x:32292,y:33235,varname:node_1158,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5042,x:33135,y:32879,varname:node_5042,prsc:2|A-7579-OUT,B-3584-A,C-5046-OUT;n:type:ShaderForge.SFN_Rotator,id:5628,x:32037,y:32586,varname:node_5628,prsc:2|UVIN-10-OUT,SPD-1670-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1670,x:31783,y:32791,ptovrint:False,ptlb:Rotate speed,ptin:_Rotatespeed,varname:node_1670,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Power,id:8876,x:32521,y:32621,varname:node_8876,prsc:2|VAL-8590-RGB,EXP-9104-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9104,x:32279,y:32776,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_9104,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:3584-4409-5046-9104-8590-1670;pass:END;sub:END;*/

Shader "H2/H_Spread_Add_P" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _LightingLevels ("Lighting Levels", Float ) = 1
        _Power ("Power", Float ) = 1
        _Spread_TEX ("Spread_TEX", 2D) = "white" {}
        _Rotatespeed ("Rotate speed", Float ) = 0
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
           // #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Spread_TEX; uniform float4 _Spread_TEX_ST;
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform float4 _Color;
            uniform float _LightingLevels;
            uniform float _Rotatespeed;
            uniform float _Power;
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
                float4 node_5362 = _Time + _TimeEditor;
                float node_5628_ang = node_5362.g;
                float node_5628_spd = _Rotatespeed;
                float node_5628_cos = cos(node_5628_spd*node_5628_ang);
                float node_5628_sin = sin(node_5628_spd*node_5628_ang);
                float2 node_5628_piv = float2(0.5,0.5);
                float node_5498 = 0.5;
                float2 node_5628 = (mul(saturate(lerp((i.uv0+((i.uv0-node_5498)/i.vertexColor.a)),float2(node_5498,node_5498),i.vertexColor.a))-node_5628_piv,float2x2( node_5628_cos, -node_5628_sin, node_5628_sin, node_5628_cos))+node_5628_piv);
                float4 _Spread_TEX_var = tex2D(_Spread_TEX,TRANSFORM_TEX(node_5628, _Spread_TEX));
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(i.uv0, _TEX));
                float3 emissive = ((pow(_Spread_TEX_var.rgb,_Power)*_TEX_var.rgb*_Color.rgb*i.vertexColor.rgb*_Spread_TEX_var.a)*_TEX_var.a*_LightingLevels);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
