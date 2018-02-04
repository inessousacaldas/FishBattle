// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32455,y:32584,varname:node_3138,prsc:2|emission-1951-OUT,alpha-3903-OUT;n:type:ShaderForge.SFN_Tex2d,id:2426,x:31163,y:33115,ptovrint:False,ptlb:Ramp Texture,ptin:_RampTexture,varname:node_2426,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:db3f1119fe575ef489fa4787bc62a48e,ntxv:0,isnm:False|UVIN-1700-UVOUT;n:type:ShaderForge.SFN_NormalVector,id:2303,x:30428,y:33114,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:7569,x:30599,y:33114,varname:node_7569,prsc:2,tffrom:0,tfto:3|IN-2303-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2073,x:30782,y:33116,varname:node_2073,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7569-XYZ;n:type:ShaderForge.SFN_Multiply,id:6180,x:31377,y:33114,varname:node_6180,prsc:2|A-2426-RGB,B-6850-RGB;n:type:ShaderForge.SFN_Color,id:6850,x:31162,y:33289,ptovrint:False,ptlb:Ramp Texture Color,ptin:_RampTextureColor,varname:node_6850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Clamp,id:3746,x:31610,y:33108,varname:node_3746,prsc:2|IN-6180-OUT,MIN-6984-OUT,MAX-6366-OUT;n:type:ShaderForge.SFN_Vector1,id:6366,x:31374,y:33320,varname:node_6366,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6984,x:31376,y:33258,ptovrint:False,ptlb:Ramp Texture Color alpha,ptin:_RampTextureColoralpha,varname:node_6984,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Rotator,id:1700,x:30985,y:33114,varname:node_1700,prsc:2|UVIN-2073-OUT,ANG-3616-OUT;n:type:ShaderForge.SFN_Slider,id:3616,x:30596,y:33300,ptovrint:False,ptlb:light Directione,ptin:_lightDirectione,varname:node_3616,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Slider,id:1863,x:30678,y:32838,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_1863,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_ComponentMask,id:7801,x:30441,y:32630,varname:node_7801,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-4111-R;n:type:ShaderForge.SFN_Tex2d,id:4111,x:30090,y:32669,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4111,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4db30eecd487d6d4dbc05a51baf92de5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3356,x:31034,y:32776,varname:node_3356,prsc:2|A-9350-OUT,B-1863-OUT;n:type:ShaderForge.SFN_Power,id:9350,x:30684,y:32631,varname:node_9350,prsc:2|VAL-7801-OUT,EXP-290-OUT;n:type:ShaderForge.SFN_Add,id:3432,x:31899,y:32960,varname:node_3432,prsc:2|A-3356-OUT,B-293-OUT;n:type:ShaderForge.SFN_OneMinus,id:293,x:30863,y:32945,varname:node_293,prsc:2|IN-4111-RGB;n:type:ShaderForge.SFN_ComponentMask,id:3903,x:32228,y:32965,varname:node_3903,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3432-OUT;n:type:ShaderForge.SFN_ValueProperty,id:290,x:30461,y:32810,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_290,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:2789,x:31218,y:32463,ptovrint:False,ptlb:Base,ptin:_Base,varname:node_2789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ad40614d3ccb56b4fa8e72204951bf46,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5327,x:31686,y:32589,varname:node_5327,prsc:2|A-2789-RGB,B-3834-OUT;n:type:ShaderForge.SFN_Add,id:9692,x:31934,y:32591,varname:node_9692,prsc:2|A-5327-OUT,B-2205-OUT,C-5631-OUT;n:type:ShaderForge.SFN_Multiply,id:2205,x:31488,y:32769,varname:node_2205,prsc:2|A-2789-RGB,B-3356-OUT;n:type:ShaderForge.SFN_Multiply,id:1951,x:32213,y:32655,varname:node_1951,prsc:2|A-9692-OUT,B-3746-OUT;n:type:ShaderForge.SFN_RemapRange,id:6350,x:31210,y:32622,varname:node_6350,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-293-OUT;n:type:ShaderForge.SFN_Clamp01,id:3834,x:31486,y:32616,varname:node_3834,prsc:2|IN-6350-OUT;n:type:ShaderForge.SFN_Tex2d,id:7338,x:30802,y:31992,ptovrint:False,ptlb:Liuguang,ptin:_Liuguang,varname:node_7338,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2c17f7ec4c02c474bb443784fc32aa44,ntxv:0,isnm:False|UVIN-8824-UVOUT;n:type:ShaderForge.SFN_Multiply,id:5631,x:31739,y:32383,varname:node_5631,prsc:2|A-4217-OUT,B-9350-OUT,C-1029-RGB;n:type:ShaderForge.SFN_Panner,id:8824,x:30452,y:31951,varname:node_8824,prsc:2,spu:1,spv:1|UVIN-6780-UVOUT,DIST-8950-OUT;n:type:ShaderForge.SFN_Color,id:1029,x:31559,y:32449,ptovrint:False,ptlb:node_1029,ptin:_node_1029,varname:node_1029,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_NormalVector,id:5322,x:29125,y:31933,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:5534,x:29418,y:31936,varname:node_5534,prsc:2,tffrom:0,tfto:1|IN-5322-OUT;n:type:ShaderForge.SFN_ComponentMask,id:351,x:29665,y:31931,varname:node_351,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5534-XYZ;n:type:ShaderForge.SFN_RemapRange,id:7410,x:30101,y:31890,varname:node_7410,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-351-OUT;n:type:ShaderForge.SFN_Power,id:4217,x:31321,y:32079,varname:node_4217,prsc:2|VAL-3726-OUT,EXP-1388-OUT;n:type:ShaderForge.SFN_Vector1,id:6144,x:31051,y:32228,varname:node_6144,prsc:2,v1:1.5;n:type:ShaderForge.SFN_TexCoord,id:6780,x:29908,y:31797,varname:node_6780,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:5343,x:29879,y:32041,varname:node_5343,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8950,x:30153,y:32116,varname:node_8950,prsc:2|A-5343-TSL,B-6145-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6145,x:29924,y:32284,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_6145,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:3726,x:31035,y:32083,varname:node_3726,prsc:2|A-7338-RGB,B-3202-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3202,x:30794,y:32293,ptovrint:False,ptlb:liuguang_alpha,ptin:_liuguang_alpha,varname:node_3202,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:1388,x:31041,y:32349,ptovrint:False,ptlb:liuguang_power,ptin:_liuguang_power,varname:node_1388,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:2789-2426-6850-6984-3616-1863-290-4111-7338-1029-6145-3202-1388;pass:END;sub:END;*/

Shader "Shader Forge/Baoyu-Unlit-Toon_Freak" {
    Properties {
        _Base ("Base", 2D) = "white" {}
        _RampTexture ("Ramp Texture", 2D) = "white" {}
        _RampTextureColor ("Ramp Texture Color", Color) = (1,1,1,1)
        _RampTextureColoralpha ("Ramp Texture Color alpha", Float ) = 0.5
        _lightDirectione ("light Directione", Range(0, 2)) = 2
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Power ("Power", Float ) = 1
        _Mask ("Mask", 2D) = "white" {}
        _Liuguang ("Liuguang", 2D) = "white" {}
        _node_1029 ("node_1029", Color) = (0.5,0.5,0.5,1)
        _speed ("speed", Float ) = 2
        _liuguang_alpha ("liuguang_alpha", Float ) = 2
        _liuguang_power ("liuguang_power", Float ) = 1
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _RampTexture; uniform float4 _RampTexture_ST;
            uniform float4 _RampTextureColor;
            uniform float _RampTextureColoralpha;
            uniform float _lightDirectione;
            uniform float _Alpha;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Power;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform sampler2D _Liuguang; uniform float4 _Liuguang_ST;
            uniform float4 _node_1029;
            uniform float _speed;
            uniform float _liuguang_alpha;
            uniform float _liuguang_power;
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
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 node_293 = (1.0 - _Mask_var.rgb);
                float node_9350 = pow(_Mask_var.r.r,_Power);
                float node_3356 = (node_9350*_Alpha);
                float4 node_5343 = _Time + _TimeEditor;
                float2 node_8824 = (i.uv0+(node_5343.r*_speed)*float2(1,1));
                float4 _Liuguang_var = tex2D(_Liuguang,TRANSFORM_TEX(node_8824, _Liuguang));
                float node_1700_ang = _lightDirectione;
                float node_1700_spd = 1.0;
                float node_1700_cos = cos(node_1700_spd*node_1700_ang);
                float node_1700_sin = sin(node_1700_spd*node_1700_ang);
                float2 node_1700_piv = float2(0.5,0.5);
                float2 node_1700 = (mul(mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg-node_1700_piv,float2x2( node_1700_cos, -node_1700_sin, node_1700_sin, node_1700_cos))+node_1700_piv);
                float4 _RampTexture_var = tex2D(_RampTexture,TRANSFORM_TEX(node_1700, _RampTexture));
                float3 emissive = (((_Base_var.rgb*saturate((node_293*2.0+-1.0)))+(_Base_var.rgb*node_3356)+(pow((_Liuguang_var.rgb*_liuguang_alpha),_liuguang_power)*node_9350*_node_1029.rgb))*clamp((_RampTexture_var.rgb*_RampTextureColor.rgb),_RampTextureColoralpha,1.0));
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_3356+node_293).r);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
