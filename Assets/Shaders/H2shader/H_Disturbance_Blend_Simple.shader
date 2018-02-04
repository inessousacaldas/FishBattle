// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.4527383,fgcb:0.4411765,fgca:1,fgde:0.01,fgrn:-43.8,fgrf:384.7,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32545,y:32925,varname:node_4795,prsc:2|emission-2393-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31668,y:33129,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1540-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32237,y:32994,varname:node_2393,prsc:2|A-4903-OUT,B-2053-RGB,C-797-RGB,D-6074-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31708,y:32798,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31704,y:32983,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:798,x:32254,y:33216,varname:node_798,prsc:2|A-2053-A,B-797-A,C-6074-A,D-461-OUT,E-5356-OUT;n:type:ShaderForge.SFN_Tex2d,id:9704,x:30559,y:33123,ptovrint:False,ptlb:Noise 01,ptin:_Noise01,varname:node_9704,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2778-OUT;n:type:ShaderForge.SFN_Append,id:2778,x:30369,y:33092,varname:node_2778,prsc:2|A-8813-OUT,B-6750-OUT;n:type:ShaderForge.SFN_TexCoord,id:8230,x:29993,y:33131,varname:node_8230,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:8813,x:30189,y:33017,varname:node_8813,prsc:2|A-2506-OUT,B-8230-U;n:type:ShaderForge.SFN_Multiply,id:2506,x:29957,y:32957,varname:node_2506,prsc:2|A-9982-T,B-5869-OUT;n:type:ShaderForge.SFN_Time,id:9982,x:29724,y:32897,varname:node_9982,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5869,x:29755,y:33120,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_5869,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.3;n:type:ShaderForge.SFN_Add,id:6750,x:30161,y:33294,varname:node_6750,prsc:2|A-8230-V,B-9086-OUT;n:type:ShaderForge.SFN_Multiply,id:9086,x:29910,y:33293,varname:node_9086,prsc:2|A-9955-T,B-3622-OUT;n:type:ShaderForge.SFN_Time,id:9955,x:29700,y:33233,varname:node_9955,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3622,x:29737,y:33460,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:_node_5869_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.2;n:type:ShaderForge.SFN_Multiply,id:3853,x:31064,y:33166,varname:node_3853,prsc:2|A-8424-OUT,B-3517-OUT;n:type:ShaderForge.SFN_Add,id:1540,x:31291,y:33140,varname:node_1540,prsc:2|A-6037-UVOUT,B-3853-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4903,x:31671,y:32741,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_4903,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ComponentMask,id:8424,x:30825,y:33187,varname:node_8424,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-9704-RGB;n:type:ShaderForge.SFN_TexCoord,id:6037,x:31029,y:32964,varname:node_6037,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:3517,x:30700,y:33400,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_3517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4607778,max:3;n:type:ShaderForge.SFN_Slider,id:461,x:31539,y:33334,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_461,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.060438,max:2;n:type:ShaderForge.SFN_Tex2d,id:8936,x:31620,y:33473,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:node_8936,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5356,x:31974,y:33464,varname:node_5356,prsc:2|A-8936-R,B-8936-A;proporder:6074-797-4903-461-3517-9704-5869-3622-8936;pass:END;sub:END;*/

Shader "H2/H_Disturbance_Blend_Simple" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (1,1,1,1)
        _ZT ("ZT", Float ) = 1
        _Alpha ("Alpha", Range(0, 2)) = 1.060438
        _Disturbance ("Disturbance", Range(0, 3)) = 0.4607778
        _Noise01 ("Noise 01", 2D) = "white" {}
        _U_Speed ("U_Speed", Float ) = -0.3
        _V_Speed ("V_Speed", Float ) = -0.2
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise01; uniform float4 _Noise01_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
            uniform float _ZT;
            uniform float _Disturbance;
            uniform float _Alpha;
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
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_9982 = _Time + _TimeEditor;
                float4 node_9955 = _Time + _TimeEditor;
                float2 node_2778 = float2(((node_9982.g*_U_Speed)+i.uv0.r),(i.uv0.g+(node_9955.g*_V_Speed)));
                float4 _Noise01_var = tex2D(_Noise01,TRANSFORM_TEX(node_2778, _Noise01));
                float2 node_1540 = (i.uv0+(_Noise01_var.rgb.r*_Disturbance));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1540, _MainTex));
                float3 emissive = (_ZT*i.vertexColor.rgb*_TintColor.rgb*_MainTex_var.rgb);
                float3 finalColor = emissive;
                float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
                return fixed4(finalColor,(i.vertexColor.a*_TintColor.a*_MainTex_var.a*_Alpha*(_MASK_var.r*_MASK_var.a)));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
