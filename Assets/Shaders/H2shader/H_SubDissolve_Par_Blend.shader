// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.4527383,fgcb:0.4411765,fgca:1,fgde:0.01,fgrn:-43.8,fgrf:384.7,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4166,x:33882,y:32640,varname:node_4166,prsc:2|emission-3365-OUT,alpha-9679-OUT;n:type:ShaderForge.SFN_Tex2d,id:219,x:32391,y:32675,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8521,x:31427,y:32898,ptovrint:False,ptlb:Dssoled,ptin:_Dssoled,varname:node_8521,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:384,x:32007,y:32951,varname:node_384,prsc:2|A-6541-OUT,B-2310-OUT;n:type:ShaderForge.SFN_Power,id:877,x:32255,y:32951,varname:node_877,prsc:2|VAL-384-OUT,EXP-1214-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1214,x:32007,y:33177,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_1214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ComponentMask,id:8972,x:33164,y:32897,varname:node_8972,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-9936-OUT;n:type:ShaderForge.SFN_Multiply,id:9679,x:33526,y:32908,varname:node_9679,prsc:2|A-8972-OUT,B-7937-OUT;n:type:ShaderForge.SFN_OneMinus,id:6541,x:31774,y:32903,varname:node_6541,prsc:2|IN-1963-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7937,x:33316,y:33089,ptovrint:False,ptlb:ALPHA,ptin:_ALPHA,varname:node_7937,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:7613,x:31383,y:33043,varname:node_7613,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2310,x:31774,y:33094,varname:node_2310,prsc:2|A-7776-OUT,B-7916-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7916,x:31498,y:33276,ptovrint:False,ptlb:VertexColor_power,ptin:_VertexColor_power,varname:node_7916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Multiply,id:3365,x:33604,y:32685,varname:node_3365,prsc:2|A-219-RGB,B-2831-RGB,C-5454-OUT,D-7466-RGB;n:type:ShaderForge.SFN_Color,id:2831,x:33199,y:32446,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_2831,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5454,x:33317,y:32712,ptovrint:False,ptlb:Light,ptin:_Light,varname:node_5454,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_OneMinus,id:7776,x:31575,y:33126,varname:node_7776,prsc:2|IN-7613-A;n:type:ShaderForge.SFN_VertexColor,id:7466,x:33307,y:32774,varname:node_7466,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1963,x:31611,y:32888,varname:node_1963,prsc:2|A-8521-RGB,B-8521-A;n:type:ShaderForge.SFN_Desaturate,id:9936,x:32851,y:32900,varname:node_9936,prsc:2|COL-7177-OUT;n:type:ShaderForge.SFN_Subtract,id:7177,x:32634,y:32918,varname:node_7177,prsc:2|A-219-A,B-877-OUT;proporder:219-8521-1214-7937-7916-2831-5454;pass:END;sub:END;*/

Shader "H2/H_SubDissolve_Par_Blend" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Dssoled ("Dssoled", 2D) = "white" {}
        _Power ("Power", Float ) = 3
        _ALPHA ("ALPHA", Float ) = 1
        _VertexColor_power ("VertexColor_power", Float ) = 4
        _Color ("Color", Color) = (1,1,1,1)
        _Light ("Light", Float ) = 1
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
           // #pragma multi_compile_fog
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform sampler2D _Dssoled; uniform float4 _Dssoled_ST;
            uniform float _Power;
            uniform float _ALPHA;
            uniform float _VertexColor_power;
            uniform float4 _Color;
            uniform float _Light;
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
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(i.uv0, _TEX));
                float3 emissive = (_TEX_var.rgb*_Color.rgb*_Light*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _Dssoled_var = tex2D(_Dssoled,TRANSFORM_TEX(i.uv0, _Dssoled));
                fixed4 finalRGBA = fixed4(finalColor,(dot((_TEX_var.a-pow(((1.0 - (_Dssoled_var.rgb*_Dssoled_var.a))*((1.0 - i.vertexColor.a)*_VertexColor_power)),_Power)),float3(0.3,0.59,0.11)).r*_ALPHA));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
