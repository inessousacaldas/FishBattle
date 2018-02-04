// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4166,x:33882,y:32640,varname:node_4166,prsc:2|emission-3365-OUT,alpha-6802-OUT;n:type:ShaderForge.SFN_Tex2d,id:219,x:32538,y:32659,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5dd2fdc55ddf3ad43894f9710d5a6b2e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8521,x:31824,y:32958,ptovrint:False,ptlb:Dissoled,ptin:_Dissoled,varname:node_8521,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:384,x:32276,y:33006,varname:node_384,prsc:2|A-6541-OUT,B-2840-OUT;n:type:ShaderForge.SFN_Power,id:877,x:32524,y:33006,varname:node_877,prsc:2|VAL-384-OUT,EXP-1214-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1214,x:32276,y:33232,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_1214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_OneMinus,id:6541,x:32043,y:32958,varname:node_6541,prsc:2|IN-8521-RGB;n:type:ShaderForge.SFN_ValueProperty,id:7937,x:33321,y:32938,ptovrint:False,ptlb:ALPHA,ptin:_ALPHA,varname:node_7937,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:7613,x:31480,y:33005,varname:node_7613,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2310,x:31852,y:33095,varname:node_2310,prsc:2|A-7776-OUT,B-7916-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7916,x:31494,y:33222,ptovrint:False,ptlb:VertexColor_power,ptin:_VertexColor_power,varname:node_7916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Multiply,id:3365,x:33604,y:32685,varname:node_3365,prsc:2|A-219-RGB,B-2831-RGB,C-5454-OUT,D-7466-RGB;n:type:ShaderForge.SFN_Color,id:2831,x:33199,y:32446,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_2831,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5454,x:33279,y:32712,ptovrint:False,ptlb:Light,ptin:_Light,varname:node_5454,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_OneMinus,id:7776,x:31672,y:33088,varname:node_7776,prsc:2|IN-7613-A;n:type:ShaderForge.SFN_VertexColor,id:7466,x:33316,y:32783,varname:node_7466,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6802,x:33662,y:32982,varname:node_6802,prsc:2|A-7937-OUT,B-4374-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4374,x:33195,y:33012,varname:node_4374,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-5493-OUT;n:type:ShaderForge.SFN_Slider,id:890,x:31686,y:33263,ptovrint:False,ptlb:DISS,ptin:_DISS,varname:node_890,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8080663,max:6;n:type:ShaderForge.SFN_SwitchProperty,id:2840,x:32117,y:33156,ptovrint:False,ptlb:Diss_Switch,ptin:_Diss_Switch,varname:node_2840,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2310-OUT,B-890-OUT;n:type:ShaderForge.SFN_Desaturate,id:5493,x:33001,y:33015,varname:node_5493,prsc:2|COL-4712-OUT;n:type:ShaderForge.SFN_Subtract,id:4712,x:32781,y:33009,varname:node_4712,prsc:2|A-219-A,B-877-OUT;proporder:219-8521-1214-7937-7916-2831-5454-890-2840;pass:END;sub:END;*/

Shader "H2/H_SubDissolve_Par_Blend02" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Dissoled ("Dissoled", 2D) = "white" {}
        _Power ("Power", Float ) = 3
        _ALPHA ("ALPHA", Float ) = 1
        _VertexColor_power ("VertexColor_power", Float ) = 4
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Light ("Light", Float ) = 2
        _DISS ("DISS", Range(0, 6)) = 0.8080663
        [MaterialToggle] _Diss_Switch ("Diss_Switch", Float ) = 0.8080663
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
           // #pragma multi_compile_fwdbase
           // #pragma multi_compile_fog
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform sampler2D _Dissoled; uniform float4 _Dissoled_ST;
            uniform float _Power;
            uniform float _ALPHA;
            uniform float _VertexColor_power;
            uniform float4 _Color;
            uniform float _Light;
            uniform float _DISS;
            uniform fixed _Diss_Switch;
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
                float4 _Dissoled_var = tex2D(_Dissoled,TRANSFORM_TEX(i.uv0, _Dissoled));
                fixed4 finalRGBA = fixed4(finalColor,(_ALPHA*dot((_TEX_var.a-pow(((1.0 - _Dissoled_var.rgb)*lerp( ((1.0 - i.vertexColor.a)*_VertexColor_power), _DISS, _Diss_Switch )),_Power)),float3(0.3,0.59,0.11)).r));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
