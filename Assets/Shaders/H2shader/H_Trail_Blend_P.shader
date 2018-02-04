// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33567,y:31923,varname:node_4795,prsc:2|emission-5480-OUT,alpha-8593-OUT;n:type:ShaderForge.SFN_Tex2d,id:3796,x:32635,y:31997,ptovrint:False,ptlb:MainTex_Par,ptin:_MainTex_Par,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8755-OUT;n:type:ShaderForge.SFN_Multiply,id:5480,x:32878,y:31997,varname:node_5480,prsc:2|A-3796-RGB,B-3450-RGB,C-7008-OUT,D-7305-RGB,E-5101-RGB;n:type:ShaderForge.SFN_Color,id:7305,x:32635,y:32450,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7008,x:32635,y:32346,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_6416,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_TexCoord,id:8296,x:31909,y:31982,varname:node_8296,prsc:2,uv:0;n:type:ShaderForge.SFN_VertexColor,id:5101,x:32635,y:32639,varname:node_5101,prsc:2;n:type:ShaderForge.SFN_VertexColor,id:378,x:31724,y:32077,varname:node_378,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:1228,x:31909,y:32160,varname:node_1228,prsc:2,frmn:0,frmx:1,tomn:1,tomx:-1|IN-378-A;n:type:ShaderForge.SFN_Panner,id:3220,x:32175,y:31987,varname:node_3220,prsc:2,spu:1,spv:0|UVIN-8296-UVOUT,DIST-1228-OUT;n:type:ShaderForge.SFN_Tex2d,id:3450,x:32627,y:32176,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_1741,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:3429,x:32175,y:32172,varname:node_3429,prsc:2,spu:0,spv:1|UVIN-8296-UVOUT,DIST-1228-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:8755,x:32426,y:31997,ptovrint:False,ptlb:UroV_Par,ptin:_UroV_Par,varname:node_8846,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3220-UVOUT,B-3429-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:1843,x:32955,y:32167,varname:node_1843,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-5790-OUT;n:type:ShaderForge.SFN_Multiply,id:8593,x:33248,y:32166,varname:node_8593,prsc:2|A-1843-OUT,B-3185-OUT,C-7305-A,D-5101-A,E-3796-A;n:type:ShaderForge.SFN_ValueProperty,id:3185,x:32878,y:32346,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_3185,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:5790,x:32817,y:32183,varname:node_5790,prsc:2|A-3450-RGB,B-3450-A;proporder:8755-3796-7305-3450-7008-3185;pass:END;sub:END;*/

Shader "H2/H_Trail_Blend_P" {
    Properties {
        [MaterialToggle] _UroV_Par ("UroV_Par", Float ) = 1
        _MainTex_Par ("MainTex_Par", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "white" {}
        _ZT ("ZT", Float ) = 2
        _Alpha ("Alpha", Float ) = 2
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
            //#pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 2.0
            uniform sampler2D _MainTex_Par; uniform float4 _MainTex_Par_ST;
            uniform float4 _TintColor;
            uniform float _ZT;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform fixed _UroV_Par;
            uniform float _Alpha;
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
                float node_1228 = (i.vertexColor.a*-2.0+1.0);
                float2 _UroV_Par_var = lerp( (i.uv0+node_1228*float2(1,0)), (i.uv0+node_1228*float2(0,1)), _UroV_Par );
                float4 _MainTex_Par_var = tex2D(_MainTex_Par,TRANSFORM_TEX(_UroV_Par_var, _MainTex_Par));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (_MainTex_Par_var.rgb*_Mask_var.rgb*_ZT*_TintColor.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,((_Mask_var.rgb*_Mask_var.a).r*_Alpha*_TintColor.a*i.vertexColor.a*_MainTex_Par_var.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
