// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32890,y:32657,varname:node_4795,prsc:2|emission-633-OUT,alpha-7479-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31945,y:32506,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5814-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32205,y:32698,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31945,y:32677,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31945,y:32835,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:31945,y:32986,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_UVTile,id:5814,x:31728,y:32630,varname:node_5814,prsc:2|UVIN-8092-OUT,WDT-145-OUT,HGT-7814-OUT,TILE-4712-OUT;n:type:ShaderForge.SFN_Append,id:8092,x:31515,y:32529,varname:node_8092,prsc:2|A-3736-U,B-5527-OUT;n:type:ShaderForge.SFN_TexCoord,id:3736,x:31028,y:32427,varname:node_3736,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:5527,x:31267,y:32540,varname:node_5527,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-3736-V;n:type:ShaderForge.SFN_ValueProperty,id:145,x:31226,y:32728,ptovrint:False,ptlb:X,ptin:_X,varname:node_145,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:7814,x:31182,y:32825,ptovrint:False,ptlb:Y,ptin:_Y,varname:node_7814,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Trunc,id:4712,x:31436,y:32931,varname:node_4712,prsc:2|IN-282-OUT;n:type:ShaderForge.SFN_Multiply,id:282,x:31256,y:32957,varname:node_282,prsc:2|A-8886-T,B-5320-OUT;n:type:ShaderForge.SFN_Time,id:8886,x:30988,y:32884,varname:node_8886,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5320,x:31030,y:33088,ptovrint:False,ptlb:Sheet_Speed,ptin:_Sheet_Speed,varname:node_5320,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:633,x:32539,y:32753,varname:node_633,prsc:2|A-2393-OUT,B-4083-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4083,x:32157,y:33032,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_4083,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:7479,x:32656,y:32926,varname:node_7479,prsc:2|A-6074-A,B-2053-A,C-797-A;proporder:6074-797-145-7814-5320-4083;pass:END;sub:END;*/

Shader "H2/H_TexSheetAni_Ble" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _X ("X", Float ) = 4
        _Y ("Y", Float ) = 4
        _Sheet_Speed ("Sheet_Speed", Float ) = 10
        _ZT ("ZT", Float ) = 2
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
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _X;
            uniform float _Y;
            uniform float _Sheet_Speed;
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
                float4 node_8886 = _Time + _TimeEditor;
                float node_4712 = trunc((node_8886.g*_Sheet_Speed));
                float2 node_5814_tc_rcp = float2(1.0,1.0)/float2( _X, _Y );
                float node_5814_ty = floor(node_4712 * node_5814_tc_rcp.x);
                float node_5814_tx = node_4712 - _X * node_5814_ty;
                float2 node_5814 = (float2(i.uv0.r,(i.uv0.g*-1.0+1.0)) + float2(node_5814_tx, node_5814_ty)) * node_5814_tc_rcp;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5814, _MainTex));
                float3 emissive = ((_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0)*_ZT);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a*_TintColor.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
