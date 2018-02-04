// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5211,x:33076,y:32629,varname:node_5211,prsc:2|emission-3652-OUT;n:type:ShaderForge.SFN_Tex2d,id:3202,x:32215,y:32766,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_3202,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:286d61729fd1ed04a8a1656bd6d14055,ntxv:0,isnm:False|UVIN-2161-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3652,x:32774,y:32731,varname:node_3652,prsc:2|A-861-OUT,B-2663-OUT,C-3236-RGB;n:type:ShaderForge.SFN_Color,id:3236,x:32233,y:32958,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:861,x:32233,y:33150,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_861,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:2663,x:32484,y:32767,varname:node_2663,prsc:2|A-3202-RGB,B-3202-A;n:type:ShaderForge.SFN_Rotator,id:2161,x:31985,y:32794,varname:node_2161,prsc:2|UVIN-1941-UVOUT,SPD-5650-OUT;n:type:ShaderForge.SFN_TexCoord,id:1941,x:31720,y:32742,varname:node_1941,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:5650,x:31502,y:32925,ptovrint:False,ptlb:Rota_Speed,ptin:_Rota_Speed,varname:node_5650,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-1;proporder:3202-3236-861-5650;pass:END;sub:END;*/

Shader "H2/H_Rotation_Add01" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Alpha ("Alpha", Float ) = 1
        _Rota_Speed ("Rota_Speed", Float ) = -1
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
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform float4 _Color;
            uniform float _Alpha;
            uniform float _Rota_Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_5523 = _Time + _TimeEditor;
                float node_2161_ang = node_5523.g;
                float node_2161_spd = _Rota_Speed;
                float node_2161_cos = cos(node_2161_spd*node_2161_ang);
                float node_2161_sin = sin(node_2161_spd*node_2161_ang);
                float2 node_2161_piv = float2(0.5,0.5);
                float2 node_2161 = (mul(i.uv0-node_2161_piv,float2x2( node_2161_cos, -node_2161_sin, node_2161_sin, node_2161_cos))+node_2161_piv);
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(node_2161, _TEX));
                float3 emissive = (_Alpha*(_TEX_var.rgb*_TEX_var.a)*_Color.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
