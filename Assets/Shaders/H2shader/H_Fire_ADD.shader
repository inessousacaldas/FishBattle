// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.4527383,fgcb:0.4411765,fgca:1,fgde:0.01,fgrn:-43.8,fgrf:384.7,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32358,y:32566,varname:node_4795,prsc:2|emission-5197-OUT;n:type:ShaderForge.SFN_Tex2d,id:8015,x:31288,y:32756,ptovrint:False,ptlb:TEX,ptin:_TEX,varname:node_8015,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6676-OUT;n:type:ShaderForge.SFN_Tex2d,id:6584,x:30409,y:32703,ptovrint:False,ptlb:Noise_01,ptin:_Noise_01,varname:node_6584,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5acee2e05cb523d4f826d8704520e3b4,ntxv:0,isnm:False|UVIN-3966-OUT;n:type:ShaderForge.SFN_Append,id:3966,x:30102,y:32669,varname:node_3966,prsc:2|A-2344-OUT,B-7554-OUT;n:type:ShaderForge.SFN_TexCoord,id:8191,x:29610,y:32677,varname:node_8191,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:3703,x:29352,y:32512,varname:node_3703,prsc:2;n:type:ShaderForge.SFN_Time,id:9347,x:29358,y:32729,varname:node_9347,prsc:2;n:type:ShaderForge.SFN_Add,id:2344,x:29934,y:32620,varname:node_2344,prsc:2|A-3383-OUT,B-8191-U;n:type:ShaderForge.SFN_Add,id:7554,x:29920,y:32779,varname:node_7554,prsc:2|A-8191-V,B-7600-OUT;n:type:ShaderForge.SFN_Multiply,id:3383,x:29757,y:32555,varname:node_3383,prsc:2|A-3703-T,B-3811-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3811,x:29375,y:32632,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_3811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:7600,x:29749,y:32850,varname:node_7600,prsc:2|A-9347-T,B-5059-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5059,x:29381,y:32895,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:_U_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.4;n:type:ShaderForge.SFN_TexCoord,id:1056,x:30858,y:32967,varname:node_1056,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:6676,x:31102,y:32761,varname:node_6676,prsc:2|A-4832-OUT,B-1056-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5439,x:30422,y:32954,ptovrint:False,ptlb:Noise_02,ptin:_Noise_02,varname:_Noise_02,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:91d436b0b02556e409adff0387c82528,ntxv:0,isnm:False|UVIN-631-OUT;n:type:ShaderForge.SFN_Append,id:631,x:30028,y:33113,varname:node_631,prsc:2|A-2283-OUT,B-4742-OUT;n:type:ShaderForge.SFN_TexCoord,id:1579,x:29606,y:33205,varname:node_1579,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:5386,x:29382,y:33029,varname:node_5386,prsc:2;n:type:ShaderForge.SFN_Time,id:2604,x:29350,y:33299,varname:node_2604,prsc:2;n:type:ShaderForge.SFN_Add,id:2283,x:29823,y:33099,varname:node_2283,prsc:2|A-1989-OUT,B-1579-U;n:type:ShaderForge.SFN_Add,id:4742,x:29816,y:33283,varname:node_4742,prsc:2|A-1579-V,B-9859-OUT;n:type:ShaderForge.SFN_Multiply,id:1989,x:29638,y:33069,varname:node_1989,prsc:2|A-5386-T,B-1284-OUT;n:type:ShaderForge.SFN_Multiply,id:9859,x:29645,y:33355,varname:node_9859,prsc:2|A-2604-T,B-4969-OUT;n:type:ShaderForge.SFN_Multiply,id:2499,x:30683,y:32763,varname:node_2499,prsc:2|A-6584-R,B-5439-R;n:type:ShaderForge.SFN_ValueProperty,id:1284,x:29349,y:33197,ptovrint:False,ptlb:U_Speed02,ptin:_U_Speed02,varname:node_1284,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:4969,x:29347,y:33489,ptovrint:False,ptlb:V_Speef02,ptin:_V_Speef02,varname:node_4969,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:4832,x:30878,y:32790,varname:node_4832,prsc:2|A-2499-OUT,B-6226-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6226,x:30512,y:33177,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_6226,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_OneMinus,id:3663,x:31521,y:32741,varname:node_3663,prsc:2|IN-8015-RGB;n:type:ShaderForge.SFN_Multiply,id:9729,x:31821,y:32658,varname:node_9729,prsc:2|A-1217-RGB,B-3663-OUT,C-3667-RGB,D-433-RGB,E-500-OUT;n:type:ShaderForge.SFN_Tex2d,id:1217,x:31402,y:32516,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:node_1217,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3667,x:31503,y:32916,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Clamp01,id:5197,x:32058,y:32707,varname:node_5197,prsc:2|IN-9729-OUT;n:type:ShaderForge.SFN_VertexColor,id:433,x:31501,y:33130,varname:node_433,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:500,x:31773,y:33226,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_500,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;proporder:8015-3667-6226-6584-3811-5059-5439-1284-4969-1217-500;pass:END;sub:END;*/

Shader "H/H_Fire_ADD" {
    Properties {
        _TEX ("TEX", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Disturbance ("Disturbance", Float ) = 1
        _Noise_01 ("Noise_01", 2D) = "white" {}
        _U_Speed ("U_Speed", Float ) = 0
        _V_Speed ("V_Speed", Float ) = 0.4
        _Noise_02 ("Noise_02", 2D) = "white" {}
        _U_Speed02 ("U_Speed02", Float ) = 0
        _V_Speef02 ("V_Speef02", Float ) = 0.7
        _MASK ("MASK", 2D) = "white" {}
        _ZT ("ZT", Float ) = 2
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
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
         //   #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _TEX; uniform float4 _TEX_ST;
            uniform sampler2D _Noise_01; uniform float4 _Noise_01_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform sampler2D _Noise_02; uniform float4 _Noise_02_ST;
            uniform float _U_Speed02;
            uniform float _V_Speef02;
            uniform float _Disturbance;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float4 _Color;
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
                float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
                float4 node_3703 = _Time + _TimeEditor;
                float4 node_9347 = _Time + _TimeEditor;
                float2 node_3966 = float2(((node_3703.g*_U_Speed)+i.uv0.r),(i.uv0.g+(node_9347.g*_V_Speed)));
                float4 _Noise_01_var = tex2D(_Noise_01,TRANSFORM_TEX(node_3966, _Noise_01));
                float4 node_5386 = _Time + _TimeEditor;
                float4 node_2604 = _Time + _TimeEditor;
                float2 node_631 = float2(((node_5386.g*_U_Speed02)+i.uv0.r),(i.uv0.g+(node_2604.g*_V_Speef02)));
                float4 _Noise_02_var = tex2D(_Noise_02,TRANSFORM_TEX(node_631, _Noise_02));
                float2 node_6676 = (((_Noise_01_var.r*_Noise_02_var.r)*_Disturbance)+i.uv0);
                float4 _TEX_var = tex2D(_TEX,TRANSFORM_TEX(node_6676, _TEX));
                float3 emissive = saturate((_MASK_var.rgb*(1.0 - _TEX_var.rgb)*_Color.rgb*i.vertexColor.rgb*_ZT));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
