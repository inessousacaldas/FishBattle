// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33223,y:32335,varname:node_4795,prsc:2|emission-6081-OUT;n:type:ShaderForge.SFN_Tex2d,id:914,x:32405,y:32425,ptovrint:False,ptlb:MainTex Par,ptin:_MainTexPar,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8846-OUT;n:type:ShaderForge.SFN_Multiply,id:6081,x:32983,y:32428,varname:node_6081,prsc:2|A-9671-OUT,B-5747-OUT,C-8399-OUT,D-8842-RGB,E-6084-RGB;n:type:ShaderForge.SFN_Color,id:8842,x:32441,y:32923,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:8399,x:32441,y:32836,ptovrint:False,ptlb:Lighting Levels,ptin:_LightingLevels,varname:node_6416,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_TexCoord,id:2008,x:31679,y:32410,varname:node_2008,prsc:2,uv:0;n:type:ShaderForge.SFN_VertexColor,id:6084,x:32441,y:33079,varname:node_6084,prsc:2;n:type:ShaderForge.SFN_VertexColor,id:7762,x:31488,y:32588,varname:node_7762,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:9033,x:31679,y:32588,varname:node_9033,prsc:2,frmn:0,frmx:1,tomn:1,tomx:-1|IN-7762-A;n:type:ShaderForge.SFN_Panner,id:4691,x:31945,y:32415,varname:node_4691,prsc:2,spu:1,spv:0|UVIN-2008-UVOUT,DIST-9033-OUT;n:type:ShaderForge.SFN_Tex2d,id:1741,x:32378,y:32639,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_1741,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:1613,x:31945,y:32600,varname:node_1613,prsc:2,spu:0,spv:1|UVIN-2008-UVOUT,DIST-9033-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:8846,x:32196,y:32425,ptovrint:False,ptlb:UroV Par,ptin:_UroVPar,varname:node_8846,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4691-UVOUT,B-1613-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8744,x:31933,y:32038,ptovrint:False,ptlb:Tex2,ptin:_Tex2,varname:node_8744,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1280-OUT;n:type:ShaderForge.SFN_Multiply,id:9671,x:32726,y:32396,varname:node_9671,prsc:2|A-5627-OUT,B-914-RGB,C-914-A;n:type:ShaderForge.SFN_Multiply,id:3767,x:31232,y:31980,varname:node_3767,prsc:2|A-3162-T,B-3439-OUT;n:type:ShaderForge.SFN_Time,id:3162,x:30939,y:31955,varname:node_3162,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3439,x:30952,y:32116,ptovrint:False,ptlb:Sudu U,ptin:_SuduU,varname:node_496,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_TexCoord,id:3074,x:31214,y:32105,varname:node_3074,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:5231,x:31460,y:32001,varname:node_5231,prsc:2|A-3767-OUT,B-3074-U;n:type:ShaderForge.SFN_Add,id:2758,x:31460,y:32170,varname:node_2758,prsc:2|A-3074-V,B-8902-OUT;n:type:ShaderForge.SFN_Multiply,id:8902,x:31200,y:32265,varname:node_8902,prsc:2|A-1933-T,B-6405-OUT;n:type:ShaderForge.SFN_Time,id:1933,x:30963,y:32247,varname:node_1933,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:6405,x:30963,y:32412,ptovrint:False,ptlb:Sudu V,ptin:_SuduV,varname:_Sudu_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Append,id:1280,x:31687,y:32021,varname:node_1280,prsc:2|A-5231-OUT,B-2758-OUT;n:type:ShaderForge.SFN_Multiply,id:9950,x:32275,y:32166,varname:node_9950,prsc:2|A-5014-RGB,B-8744-RGB,C-6711-OUT,D-8744-A;n:type:ShaderForge.SFN_Color,id:5014,x:31964,y:31862,ptovrint:False,ptlb:Tex2_Color,ptin:_Tex2_Color,varname:node_5014,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6711,x:31905,y:32241,ptovrint:False,ptlb:Tex2 Lighting Levels,ptin:_Tex2LightingLevels,varname:node_6711,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_SwitchProperty,id:5627,x:32570,y:32124,ptovrint:False,ptlb:Tex2_Switch,ptin:_Tex2_Switch,varname:node_5627,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6910-OUT,B-9950-OUT;n:type:ShaderForge.SFN_Vector1,id:6910,x:32275,y:31985,varname:node_6910,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5747,x:32604,y:32621,varname:node_5747,prsc:2|A-1741-RGB,B-1741-A;proporder:8846-914-8399-8842-5627-8744-6711-5014-3439-6405-1741;pass:END;sub:END;*/

Shader "H2/H_Trail_Add_P" {
    Properties {
        [MaterialToggle] _UroVPar ("UroV Par", Float ) = 1
        _MainTexPar ("MainTex Par", 2D) = "white" {}
        _LightingLevels ("Lighting Levels", Float ) = 2
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _Tex2_Switch ("Tex2_Switch", Float ) = 1
        _Tex2 ("Tex2", 2D) = "white" {}
        _Tex2LightingLevels ("Tex2 Lighting Levels", Float ) = 2
        _Tex2_Color ("Tex2_Color", Color) = (1,1,1,1)
        _SuduU ("Sudu U", Float ) = 0.1
        _SuduV ("Sudu V", Float ) = 0.1
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
            //#pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTexPar; uniform float4 _MainTexPar_ST;
            uniform float4 _TintColor;
            uniform float _LightingLevels;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform fixed _UroVPar;
            uniform sampler2D _Tex2; uniform float4 _Tex2_ST;
            uniform float _SuduU;
            uniform float _SuduV;
            uniform float4 _Tex2_Color;
            uniform float _Tex2LightingLevels;
            uniform fixed _Tex2_Switch;
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
            float4 frag(VertexOutput i) : COLOR {
                float4 node_3162 = _Time + _TimeEditor;
                float4 node_1933 = _Time + _TimeEditor;
                float2 node_1280 = float2(((node_3162.g*_SuduU)+i.uv0.r),(i.uv0.g+(node_1933.g*_SuduV)));
                float4 _Tex2_var = tex2D(_Tex2,TRANSFORM_TEX(node_1280, _Tex2));
                float node_9033 = (i.vertexColor.a*-2.0+1.0);
                float2 _UroVPar_var = lerp( (i.uv0+node_9033*float2(1,0)), (i.uv0+node_9033*float2(0,1)), _UroVPar );
                float4 _MainTexPar_var = tex2D(_MainTexPar,TRANSFORM_TEX(_UroVPar_var, _MainTexPar));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = ((lerp( 1.0, (_Tex2_Color.rgb*_Tex2_var.rgb*_Tex2LightingLevels*_Tex2_var.a), _Tex2_Switch )*_MainTexPar_var.rgb*_MainTexPar_var.a)*(_Mask_var.rgb*_Mask_var.a)*_LightingLevels*_TintColor.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
