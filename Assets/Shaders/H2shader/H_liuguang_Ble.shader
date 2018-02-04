// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32475,y:32453,varname:node_4795,prsc:2|emission-2393-OUT,alpha-8720-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31680,y:32956,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32304,y:32628,varname:node_2393,prsc:2|A-99-OUT,B-2053-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31669,y:32629,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Append,id:2664,x:29429,y:32414,varname:node_2664,prsc:2|A-9290-OUT,B-37-OUT;n:type:ShaderForge.SFN_TexCoord,id:6184,x:28959,y:32454,varname:node_6184,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:9290,x:29219,y:32330,varname:node_9290,prsc:2|A-8940-OUT,B-6184-U;n:type:ShaderForge.SFN_Multiply,id:8940,x:28959,y:32305,varname:node_8940,prsc:2|A-2703-TSL,B-961-OUT;n:type:ShaderForge.SFN_Time,id:2703,x:28285,y:32415,varname:node_2703,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:961,x:28642,y:32442,ptovrint:False,ptlb:U_Speed01,ptin:_U_Speed01,varname:node_961,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Add,id:37,x:29210,y:32590,varname:node_37,prsc:2|A-6184-V,B-1653-OUT;n:type:ShaderForge.SFN_Tex2d,id:3191,x:29625,y:32414,ptovrint:False,ptlb:Liuguang01,ptin:_Liuguang01,varname:node_3191,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9b173c0d6ac89c44898e6fd7b99a15f,ntxv:0,isnm:False|UVIN-2664-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2826,x:28654,y:32655,ptovrint:False,ptlb:V_Speed01,ptin:_V_Speed01,varname:_node_961_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1653,x:28959,y:32644,varname:node_1653,prsc:2|A-2703-TSL,B-2826-OUT;n:type:ShaderForge.SFN_Multiply,id:8903,x:30573,y:32493,varname:node_8903,prsc:2|A-2233-OUT,B-8396-OUT;n:type:ShaderForge.SFN_Append,id:908,x:29446,y:32856,varname:node_908,prsc:2|A-2253-OUT,B-8936-OUT;n:type:ShaderForge.SFN_TexCoord,id:8053,x:28969,y:33035,varname:node_8053,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:2253,x:29229,y:32876,varname:node_2253,prsc:2|A-72-OUT,B-8053-U;n:type:ShaderForge.SFN_Multiply,id:72,x:28969,y:32886,varname:node_72,prsc:2|A-2703-TSL,B-8448-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8448,x:28673,y:32975,ptovrint:False,ptlb:U_Speed02,ptin:_U_Speed02,varname:_U_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:8936,x:29245,y:33106,varname:node_8936,prsc:2|A-8053-V,B-5279-OUT;n:type:ShaderForge.SFN_Tex2d,id:4055,x:29697,y:32868,ptovrint:False,ptlb:Liuguang02,ptin:_Liuguang02,varname:_LiuGuang_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False|UVIN-908-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5518,x:28678,y:33250,ptovrint:False,ptlb:V_Speed02,ptin:_V_Speed02,varname:_V_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Multiply,id:5279,x:28969,y:33208,varname:node_5279,prsc:2|A-2703-TSL,B-5518-OUT;n:type:ShaderForge.SFN_Multiply,id:99,x:31353,y:32514,varname:node_99,prsc:2|A-1085-OUT,B-7787-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7787,x:31024,y:32644,ptovrint:False,ptlb:Liuguang_ZT,ptin:_Liuguang_ZT,varname:node_7787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Power,id:1085,x:31012,y:32490,varname:node_1085,prsc:2|VAL-9373-OUT,EXP-5850-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5850,x:30775,y:32674,ptovrint:False,ptlb:Liuguang_power,ptin:_Liuguang_power,varname:node_5850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.3;n:type:ShaderForge.SFN_Multiply,id:3344,x:31961,y:32870,varname:node_3344,prsc:2|A-6074-R,B-6074-A,C-7316-OUT,D-2053-A;n:type:ShaderForge.SFN_ValueProperty,id:9541,x:31818,y:33180,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9541,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:2233,x:29980,y:32498,varname:node_2233,prsc:2|A-3191-RGB,B-142-RGB;n:type:ShaderForge.SFN_Color,id:142,x:29721,y:32654,ptovrint:False,ptlb:Color01,ptin:_Color01,varname:node_142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:8396,x:30153,y:32866,varname:node_8396,prsc:2|A-4055-RGB,B-6067-RGB;n:type:ShaderForge.SFN_Color,id:6067,x:29820,y:33066,ptovrint:False,ptlb:Color02,ptin:_Color02,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:1705,x:30496,y:32777,varname:node_1705,prsc:2|A-2233-OUT,B-8396-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9373,x:30819,y:32532,ptovrint:False,ptlb:Add Or Mul,ptin:_AddOrMul,varname:node_9373,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8903-OUT,B-1705-OUT;n:type:ShaderForge.SFN_Multiply,id:8720,x:32268,y:32801,varname:node_8720,prsc:2|A-3344-OUT,B-9541-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7316,x:31673,y:32768,varname:node_7316,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-99-OUT;proporder:6074-9541-7787-5850-9373-142-3191-961-2826-6067-4055-8448-5518;pass:END;sub:END;*/

Shader "H2/H_liuguang_Ble" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Alpha ("Alpha", Float ) = 2
        _Liuguang_ZT ("Liuguang_ZT", Float ) = 4
        _Liuguang_power ("Liuguang_power", Float ) = 1.3
        [MaterialToggle] _AddOrMul ("Add Or Mul", Float ) = 0
        _Color01 ("Color01", Color) = (1,1,1,1)
        _Liuguang01 ("Liuguang01", 2D) = "white" {}
        _U_Speed01 ("U_Speed01", Float ) = 10
        _V_Speed01 ("V_Speed01", Float ) = 1
        _Color02 ("Color02", Color) = (1,1,1,1)
        _Liuguang02 ("Liuguang02", 2D) = "white" {}
        _U_Speed02 ("U_Speed02", Float ) = 2
        _V_Speed02 ("V_Speed02", Float ) = 5
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
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _U_Speed01;
            uniform sampler2D _Liuguang01; uniform float4 _Liuguang01_ST;
            uniform float _V_Speed01;
            uniform float _U_Speed02;
            uniform sampler2D _Liuguang02; uniform float4 _Liuguang02_ST;
            uniform float _V_Speed02;
            uniform float _Liuguang_ZT;
            uniform float _Liuguang_power;
            uniform float _Alpha;
            uniform float4 _Color01;
            uniform float4 _Color02;
            uniform fixed _AddOrMul;
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
                float4 node_2703 = _Time + _TimeEditor;
                float2 node_2664 = float2(((node_2703.r*_U_Speed01)+i.uv0.r),(i.uv0.g+(node_2703.r*_V_Speed01)));
                float4 _Liuguang01_var = tex2D(_Liuguang01,TRANSFORM_TEX(node_2664, _Liuguang01));
                float3 node_2233 = (_Liuguang01_var.rgb*_Color01.rgb);
                float2 node_908 = float2(((node_2703.r*_U_Speed02)+i.uv0.r),(i.uv0.g+(node_2703.r*_V_Speed02)));
                float4 _Liuguang02_var = tex2D(_Liuguang02,TRANSFORM_TEX(node_908, _Liuguang02));
                float3 node_8396 = (_Liuguang02_var.rgb*_Color02.rgb);
                float3 node_99 = (pow(lerp( (node_2233*node_8396), (node_2233+node_8396), _AddOrMul ),_Liuguang_power)*_Liuguang_ZT);
                float3 emissive = (node_99*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                return fixed4(finalColor,((_MainTex_var.r*_MainTex_var.a*node_99.r*i.vertexColor.a)*_Alpha));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
