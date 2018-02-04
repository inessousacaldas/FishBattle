// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.4527383,fgcb:0.4411765,fgca:1,fgde:0.01,fgrn:-43.8,fgrf:384.7,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32697,y:32959,varname:node_4795,prsc:2|emission-4926-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31200,y:33215,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1540-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32233,y:33039,varname:node_2393,prsc:2|A-4578-OUT,B-2053-RGB,C-797-RGB,D-6199-OUT,E-461-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31571,y:32868,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31574,y:33069,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9704,x:29908,y:33025,ptovrint:False,ptlb:Noise 01,ptin:_Noise01,varname:node_9704,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2778-OUT;n:type:ShaderForge.SFN_Append,id:2778,x:29718,y:32994,varname:node_2778,prsc:2|A-8813-OUT,B-6750-OUT;n:type:ShaderForge.SFN_TexCoord,id:8230,x:29342,y:33033,varname:node_8230,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:8813,x:29538,y:32919,varname:node_8813,prsc:2|A-2506-OUT,B-8230-U;n:type:ShaderForge.SFN_Multiply,id:2506,x:29306,y:32859,varname:node_2506,prsc:2|A-9982-T,B-5869-OUT;n:type:ShaderForge.SFN_Time,id:9982,x:29073,y:32799,varname:node_9982,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5869,x:29104,y:33022,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_5869,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:6750,x:29510,y:33196,varname:node_6750,prsc:2|A-8230-V,B-9086-OUT;n:type:ShaderForge.SFN_Multiply,id:9086,x:29259,y:33195,varname:node_9086,prsc:2|A-9955-T,B-3622-OUT;n:type:ShaderForge.SFN_Time,id:9955,x:29049,y:33135,varname:node_9955,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3622,x:29086,y:33362,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:_node_5869_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:3853,x:30685,y:33237,varname:node_3853,prsc:2|A-8424-OUT,B-8125-OUT;n:type:ShaderForge.SFN_Add,id:1540,x:30912,y:33211,varname:node_1540,prsc:2|A-6037-UVOUT,B-3853-OUT;n:type:ShaderForge.SFN_Tex2d,id:5494,x:29905,y:33457,ptovrint:False,ptlb:Noise 02,ptin:_Noise02,varname:_node_9704_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False|UVIN-9617-OUT;n:type:ShaderForge.SFN_Append,id:9617,x:29694,y:33477,varname:node_9617,prsc:2|A-3085-OUT,B-1401-OUT;n:type:ShaderForge.SFN_TexCoord,id:4987,x:29318,y:33516,varname:node_4987,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:3085,x:29514,y:33402,varname:node_3085,prsc:2|A-9194-OUT,B-4987-U;n:type:ShaderForge.SFN_Multiply,id:9194,x:29282,y:33342,varname:node_9194,prsc:2|A-1896-T,B-5069-OUT;n:type:ShaderForge.SFN_Time,id:1896,x:28950,y:33236,varname:node_1896,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5069,x:29080,y:33505,ptovrint:False,ptlb:U_Speed_copy,ptin:_U_Speed_copy,varname:_U_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:1401,x:29486,y:33679,varname:node_1401,prsc:2|A-4987-V,B-8884-OUT;n:type:ShaderForge.SFN_Multiply,id:8884,x:29235,y:33678,varname:node_8884,prsc:2|A-8589-T,B-4099-OUT;n:type:ShaderForge.SFN_Time,id:8589,x:29025,y:33618,varname:node_8589,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4099,x:29033,y:33841,ptovrint:False,ptlb:V_Speed_copy,ptin:_V_Speed_copy,varname:_V_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1243,x:30169,y:33158,varname:node_1243,prsc:2|A-9704-RGB,B-5494-RGB;n:type:ShaderForge.SFN_ComponentMask,id:8424,x:30446,y:33258,varname:node_8424,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-1243-OUT;n:type:ShaderForge.SFN_TexCoord,id:6037,x:30650,y:33035,varname:node_6037,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:461,x:31542,y:33603,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_461,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.060438,max:2;n:type:ShaderForge.SFN_ValueProperty,id:7268,x:30344,y:33455,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_7268,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8125,x:30553,y:33414,varname:node_8125,prsc:2|A-7268-OUT,B-6256-OUT;n:type:ShaderForge.SFN_Vector1,id:6256,x:30354,y:33736,varname:node_6256,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:3460,x:31420,y:33200,varname:node_3460,prsc:2|A-6074-RGB,B-6074-A;n:type:ShaderForge.SFN_Power,id:6199,x:31741,y:33259,varname:node_6199,prsc:2|VAL-3460-OUT,EXP-5376-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5376,x:31607,y:33494,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_5376,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4578,x:31924,y:33049,varname:node_4578,prsc:2|A-2053-A,B-797-A;n:type:ShaderForge.SFN_Multiply,id:4926,x:32516,y:33086,varname:node_4926,prsc:2|A-2393-OUT,B-9588-RGB,C-8057-OUT;n:type:ShaderForge.SFN_Tex2d,id:9588,x:32213,y:33272,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_9588,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:8057,x:32323,y:33683,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_8057,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:6074-797-8057-5376-461-9704-5869-3622-5494-5069-4099-7268-9588;pass:END;sub:END;*/

Shader "H2/H_Disturbance_ADD02" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (1,1,1,1)
        _ZT ("ZT", Float ) = 1
        _Power ("Power", Float ) = 1
        _Alpha ("Alpha", Range(0, 2)) = 1.060438
        _Noise01 ("Noise 01", 2D) = "white" {}
        _U_Speed ("U_Speed", Float ) = 0
        _V_Speed ("V_Speed", Float ) = 0
        _Noise02 ("Noise 02", 2D) = "white" {}
        _U_Speed_copy ("U_Speed_copy", Float ) = 0
        _V_Speed_copy ("V_Speed_copy", Float ) = 0
        _Disturbance ("Disturbance", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise01; uniform float4 _Noise01_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform sampler2D _Noise02; uniform float4 _Noise02_ST;
            uniform float _U_Speed_copy;
            uniform float _V_Speed_copy;
            uniform float _Alpha;
            uniform float _Disturbance;
            uniform float _Power;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_9982 = _Time + _TimeEditor;
                float4 node_9955 = _Time + _TimeEditor;
                float2 node_2778 = float2(((node_9982.g*_U_Speed)+i.uv0.r),(i.uv0.g+(node_9955.g*_V_Speed)));
                float4 _Noise01_var = tex2D(_Noise01,TRANSFORM_TEX(node_2778, _Noise01));
                float4 node_1896 = _Time + _TimeEditor;
                float4 node_8589 = _Time + _TimeEditor;
                float2 node_9617 = float2(((node_1896.g*_U_Speed_copy)+i.uv0.r),(i.uv0.g+(node_8589.g*_V_Speed_copy)));
                float4 _Noise02_var = tex2D(_Noise02,TRANSFORM_TEX(node_9617, _Noise02));
                float2 node_1540 = (i.uv0+((_Noise01_var.rgb*_Noise02_var.rgb).r*(_Disturbance*0.1)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1540, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (((i.vertexColor.a*_TintColor.a)*i.vertexColor.rgb*_TintColor.rgb*pow((_MainTex_var.rgb*_MainTex_var.a),_Power)*_Alpha)*_Mask_var.rgb*_ZT);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
