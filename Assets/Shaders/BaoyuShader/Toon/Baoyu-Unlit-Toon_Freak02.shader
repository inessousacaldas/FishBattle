// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2543252,fgcg:0.3470349,fgcb:0.6176471,fgca:1,fgde:0.01,fgrn:11.3,fgrf:39,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32547,y:32779,varname:node_3138,prsc:2|emission-1951-OUT,olwid-515-OUT,olcol-9941-RGB;n:type:ShaderForge.SFN_Tex2d,id:2426,x:31323,y:33203,ptovrint:False,ptlb:Ramp Texture,ptin:_RampTexture,varname:node_2426,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:db3f1119fe575ef489fa4787bc62a48e,ntxv:0,isnm:False|UVIN-1700-UVOUT;n:type:ShaderForge.SFN_NormalVector,id:2303,x:30588,y:33202,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:7569,x:30759,y:33202,varname:node_7569,prsc:2,tffrom:0,tfto:3|IN-2303-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2073,x:30942,y:33204,varname:node_2073,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7569-XYZ;n:type:ShaderForge.SFN_Multiply,id:6180,x:31537,y:33202,varname:node_6180,prsc:2|A-2426-RGB,B-6850-RGB;n:type:ShaderForge.SFN_Color,id:6850,x:31322,y:33377,ptovrint:False,ptlb:Ramp Texture Color,ptin:_RampTextureColor,varname:node_6850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Clamp,id:3746,x:31770,y:33196,varname:node_3746,prsc:2|IN-6180-OUT,MIN-6984-OUT,MAX-6366-OUT;n:type:ShaderForge.SFN_Vector1,id:6366,x:31534,y:33408,varname:node_6366,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6984,x:31536,y:33346,ptovrint:False,ptlb:Ramp Texture Color alpha,ptin:_RampTextureColoralpha,varname:node_6984,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Rotator,id:1700,x:31145,y:33202,varname:node_1700,prsc:2|UVIN-2073-OUT,ANG-3616-OUT;n:type:ShaderForge.SFN_Slider,id:3616,x:30756,y:33388,ptovrint:False,ptlb:light Directione,ptin:_lightDirectione,varname:node_3616,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Tex2d,id:4111,x:31157,y:33001,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4111,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2789,x:31208,y:32817,ptovrint:False,ptlb:Base,ptin:_Base,varname:node_2789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ad40614d3ccb56b4fa8e72204951bf46,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:9692,x:31853,y:32856,varname:node_9692,prsc:2|A-5296-OUT,B-6700-OUT;n:type:ShaderForge.SFN_Multiply,id:1951,x:32166,y:32850,varname:node_1951,prsc:2|A-9692-OUT,B-3746-OUT,C-7590-OUT;n:type:ShaderForge.SFN_NormalVector,id:5322,x:29070,y:31927,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:5534,x:29248,y:31928,varname:node_5534,prsc:2,tffrom:0,tfto:1|IN-5322-OUT;n:type:ShaderForge.SFN_ComponentMask,id:351,x:29439,y:31927,varname:node_351,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5534-XYZ;n:type:ShaderForge.SFN_RemapRange,id:7410,x:29621,y:31929,varname:node_7410,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-351-OUT;n:type:ShaderForge.SFN_Multiply,id:5296,x:31554,y:32711,varname:node_5296,prsc:2|A-2343-RGB,B-4111-RGB;n:type:ShaderForge.SFN_OneMinus,id:1143,x:31425,y:33022,varname:node_1143,prsc:2|IN-4111-RGB;n:type:ShaderForge.SFN_Multiply,id:6700,x:31617,y:32966,varname:node_6700,prsc:2|A-2789-RGB,B-1143-OUT;n:type:ShaderForge.SFN_Tex2d,id:2343,x:31275,y:32629,ptovrint:False,ptlb:Base02,ptin:_Base02,varname:node_2343,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9340-OUT;n:type:ShaderForge.SFN_TexCoord,id:5520,x:29620,y:32692,varname:node_5520,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:1427,x:30557,y:32568,ptovrint:False,ptlb:node_1427,ptin:_node_1427,varname:node_1427,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f32d8cbdfa3df5e48a885c48e037a62b,ntxv:0,isnm:False|UVIN-3219-OUT;n:type:ShaderForge.SFN_Add,id:9340,x:31007,y:32603,varname:node_9340,prsc:2|A-4447-OUT,B-5520-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4447,x:30764,y:32591,varname:node_4447,prsc:2|A-1427-R,B-4641-OUT;n:type:ShaderForge.SFN_Slider,id:4641,x:30347,y:32889,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_4641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.2;n:type:ShaderForge.SFN_Tex2d,id:6751,x:30697,y:33363,ptovrint:False,ptlb:Noise 04,ptin:_Noise04,varname:_Noise04,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9b173c0d6ac89c44898e6fd7b99a15f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:3219,x:30242,y:32694,varname:node_3219,prsc:2|A-5876-OUT,B-2288-OUT;n:type:ShaderForge.SFN_Add,id:5876,x:29987,y:32749,varname:node_5876,prsc:2|A-5520-U,B-7048-OUT;n:type:ShaderForge.SFN_Multiply,id:7048,x:29785,y:32869,varname:node_7048,prsc:2|A-5764-OUT,B-1758-TSL;n:type:ShaderForge.SFN_Time,id:1758,x:29497,y:32944,varname:node_1758,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5764,x:29527,y:32853,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_5764,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9128,x:29780,y:33028,varname:node_9128,prsc:2|A-1758-TSL,B-1685-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1685,x:29510,y:33169,ptovrint:False,ptlb:V_speed,ptin:_V_speed,varname:node_1685,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:2288,x:30040,y:32977,varname:node_2288,prsc:2|A-5520-V,B-9128-OUT;n:type:ShaderForge.SFN_Multiply,id:515,x:32267,y:33093,varname:node_515,prsc:2|A-53-OUT,B-2913-OUT;n:type:ShaderForge.SFN_Vector1,id:2913,x:32058,y:33187,varname:node_2913,prsc:2,v1:0.08;n:type:ShaderForge.SFN_Slider,id:53,x:31880,y:33063,ptovrint:False,ptlb:Outline width,ptin:_Outlinewidth,varname:node_53,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.03418804,max:1;n:type:ShaderForge.SFN_Color,id:9941,x:32372,y:33337,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_9941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1443,x:30761,y:33427,ptovrint:False,ptlb:Noise 05,ptin:_Noise05,varname:_Noise05,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9b173c0d6ac89c44898e6fd7b99a15f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:7987,x:30571,y:33396,varname:node_7987,prsc:2|A-6244-OUT,B-8325-OUT;n:type:ShaderForge.SFN_TexCoord,id:8830,x:30207,y:33343,varname:node_8830,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:6244,x:30391,y:33321,varname:node_6244,prsc:2|A-8974-OUT,B-8830-U;n:type:ShaderForge.SFN_Multiply,id:8974,x:30159,y:33261,varname:node_8974,prsc:2|A-2643-TSL,B-6738-OUT;n:type:ShaderForge.SFN_Time,id:2643,x:29734,y:33455,varname:node_2643,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:6738,x:29957,y:33424,ptovrint:False,ptlb:U_Speed_copy_copy_copy,ptin:_U_Speed_copy_copy_copy,varname:_U_Speed_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:8325,x:30375,y:33506,varname:node_8325,prsc:2|A-8830-V,B-6863-OUT;n:type:ShaderForge.SFN_Multiply,id:6863,x:30124,y:33505,varname:node_6863,prsc:2|A-2643-TSL,B-8238-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8238,x:29951,y:33672,ptovrint:False,ptlb:V_Speed_copy_copy_copy_copy,ptin:_V_Speed_copy_copy_copy_copy,varname:_V_Speed_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ValueProperty,id:7590,x:32061,y:33383,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_7590,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.1;proporder:2789-2426-6850-6984-3616-4111-2343-1427-4641-5764-1685-53-9941-7590;pass:END;sub:END;*/

Shader "Baoyu/Unlit/Toon_Freak02" {
    Properties {
        _Base ("Base", 2D) = "white" {}
        _RampTexture ("Ramp Texture", 2D) = "white" {}
        _RampTextureColor ("Ramp Texture Color", Color) = (1,1,1,1)
        _RampTextureColoralpha ("Ramp Texture Color alpha", Float ) = 0.5
        _lightDirectione ("light Directione", Range(0, 2)) = 2
        _Mask ("Mask", 2D) = "white" {}
        _Base02 ("Base02", 2D) = "white" {}
        _node_1427 ("node_1427", 2D) = "white" {}
        _Disturbance ("Disturbance", Range(0, 0.2)) = 0
        _U_Speed ("U_Speed", Float ) = 1
        _V_speed ("V_speed", Float ) = 1
        _Outlinewidth ("Outline width", Range(0, 1)) = 0.03418804
        _OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1)
        _ZT ("ZT", Float ) = 1.1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
           // #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           //  #pragma target 3.0
            uniform float _Outlinewidth;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*(_Outlinewidth*0.08),1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
          //  #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           /// #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _RampTexture; uniform float4 _RampTexture_ST;
            uniform float4 _RampTextureColor;
            uniform float _RampTextureColoralpha;
            uniform float _lightDirectione;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform sampler2D _Base02; uniform float4 _Base02_ST;
            uniform sampler2D _node_1427; uniform float4 _node_1427_ST;
            uniform float _Disturbance;
            uniform float _U_Speed;
            uniform float _V_speed;
            uniform float _ZT;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_1758 = _Time + _TimeEditor;
                float2 node_3219 = float2((i.uv0.r+(_U_Speed*node_1758.r)),(i.uv0.g+(node_1758.r*_V_speed)));
                float4 _node_1427_var = tex2D(_node_1427,TRANSFORM_TEX(node_3219, _node_1427));
                float2 node_9340 = ((_node_1427_var.r*_Disturbance)+i.uv0);
                float4 _Base02_var = tex2D(_Base02,TRANSFORM_TEX(node_9340, _Base02));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float node_1700_ang = _lightDirectione;
                float node_1700_spd = 1.0;
                float node_1700_cos = cos(node_1700_spd*node_1700_ang);
                float node_1700_sin = sin(node_1700_spd*node_1700_ang);
                float2 node_1700_piv = float2(0.5,0.5);
                float2 node_1700 = (mul(mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg-node_1700_piv,float2x2( node_1700_cos, -node_1700_sin, node_1700_sin, node_1700_cos))+node_1700_piv);
                float4 _RampTexture_var = tex2D(_RampTexture,TRANSFORM_TEX(node_1700, _RampTexture));
                float3 emissive = (((_Base02_var.rgb*_Mask_var.rgb)+(_Base_var.rgb*(1.0 - _Mask_var.rgb)))*clamp((_RampTexture_var.rgb*_RampTextureColor.rgb),_RampTextureColoralpha,1.0)*_ZT);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
