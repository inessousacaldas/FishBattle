// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33064,y:32490,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32688,y:32544,varname:node_2393,prsc:2|A-249-OUT,B-2053-RGB,C-797-RGB,D-9210-OUT,E-601-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32179,y:32668,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32179,y:32826,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4110,x:31693,y:32296,ptovrint:False,ptlb:TEX1,ptin:_TEX1,varname:node_4110,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:2,isnm:False|UVIN-6586-OUT;n:type:ShaderForge.SFN_Multiply,id:5020,x:30828,y:31990,varname:node_5020,prsc:2|A-1627-TSL,B-9558-OUT;n:type:ShaderForge.SFN_Time,id:1627,x:30541,y:31954,varname:node_1627,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9558,x:30509,y:32266,ptovrint:False,ptlb:Tex1_Uspeed,ptin:_Tex1_Uspeed,varname:node_9558,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:1599,x:31639,y:32579,ptovrint:False,ptlb:TEX2,ptin:_TEX2,varname:_TEX2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9b173c0d6ac89c44898e6fd7b99a15f,ntxv:2,isnm:False|UVIN-6679-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9210,x:32179,y:32994,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2d,id:5476,x:32182,y:33095,ptovrint:False,ptlb:MASk,ptin:_MASk,varname:node_5476,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9bbd19cdbaea81441a3343972b4a94fc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4946,x:31885,y:32358,varname:node_4946,prsc:2|A-4110-RGB,B-4110-A;n:type:ShaderForge.SFN_Multiply,id:1689,x:31878,y:32549,varname:node_1689,prsc:2|A-1599-RGB,B-1599-A;n:type:ShaderForge.SFN_Multiply,id:601,x:32465,y:33035,varname:node_601,prsc:2|A-5476-RGB,B-5476-A;n:type:ShaderForge.SFN_Multiply,id:3421,x:32126,y:32428,varname:node_3421,prsc:2|A-4946-OUT,B-1689-OUT;n:type:ShaderForge.SFN_Append,id:6586,x:31447,y:32198,varname:node_6586,prsc:2|A-3385-OUT,B-985-OUT;n:type:ShaderForge.SFN_Add,id:3385,x:31117,y:32052,varname:node_3385,prsc:2|A-5020-OUT,B-330-U;n:type:ShaderForge.SFN_TexCoord,id:330,x:30832,y:32125,varname:node_330,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7204,x:30859,y:32349,varname:node_7204,prsc:2|A-1627-TSL,B-7440-OUT;n:type:ShaderForge.SFN_Add,id:985,x:31148,y:32345,varname:node_985,prsc:2|A-330-V,B-7204-OUT;n:type:ShaderForge.SFN_Multiply,id:4206,x:30841,y:32466,varname:node_4206,prsc:2|A-1627-TSL,B-9335-OUT;n:type:ShaderForge.SFN_Add,id:6792,x:31130,y:32529,varname:node_6792,prsc:2|A-4206-OUT,B-2044-U;n:type:ShaderForge.SFN_TexCoord,id:2044,x:30825,y:32627,varname:node_2044,prsc:2,uv:0;n:type:ShaderForge.SFN_Append,id:6679,x:31359,y:32603,varname:node_6679,prsc:2|A-6792-OUT,B-1747-OUT;n:type:ShaderForge.SFN_Multiply,id:5616,x:30804,y:32819,varname:node_5616,prsc:2|A-1627-TSL,B-8254-OUT;n:type:ShaderForge.SFN_Add,id:1747,x:31095,y:32837,varname:node_1747,prsc:2|A-2044-V,B-5616-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9335,x:30502,y:32439,ptovrint:False,ptlb:Tex2_Uspeed,ptin:_Tex2_Uspeed,varname:node_9335,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8254,x:30497,y:32564,ptovrint:False,ptlb:Tex2_Vspeed,ptin:_Tex2_Vspeed,varname:_Tex2_Uspeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7440,x:30515,y:32369,ptovrint:False,ptlb:Tex1_Vspeed,ptin:_Tex1_Vspeed,varname:node_7440,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Power,id:249,x:32381,y:32456,varname:node_249,prsc:2|VAL-3421-OUT,EXP-4595-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4595,x:32117,y:32594,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_4595,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:797-9210-4595-4110-9558-7440-1599-9335-8254-5476;pass:END;sub:END;*/

Shader "H2/H_liuguang_M_Add" {
    Properties {
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Alpha ("Alpha", Float ) = 2
        _Power ("Power", Float ) = 1
        _TEX1 ("TEX1", 2D) = "black" {}
        _Tex1_Uspeed ("Tex1_Uspeed", Float ) = 0
        _Tex1_Vspeed ("Tex1_Vspeed", Float ) = 0
        _TEX2 ("TEX2", 2D) = "black" {}
        _Tex2_Uspeed ("Tex2_Uspeed", Float ) = 0
        _Tex2_Vspeed ("Tex2_Vspeed", Float ) = 0
        _MASk ("MASk", 2D) = "white" {}
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
            colormask rgb

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
           // #pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _TintColor;
            uniform sampler2D _TEX1; uniform float4 _TEX1_ST;
            uniform float _Tex1_Uspeed;
            uniform sampler2D _TEX2; uniform float4 _TEX2_ST;
            uniform float _Alpha;
            uniform sampler2D _MASk; uniform float4 _MASk_ST;
            uniform float _Tex2_Uspeed;
            uniform float _Tex2_Vspeed;
            uniform float _Tex1_Vspeed;
            uniform float _Power;
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
                float4 node_1627 = _Time + _TimeEditor;
                float2 node_6586 = float2(((node_1627.r*_Tex1_Uspeed)+i.uv0.r),(i.uv0.g+(node_1627.r*_Tex1_Vspeed)));
                float4 _TEX1_var = tex2D(_TEX1,TRANSFORM_TEX(node_6586, _TEX1));
                float2 node_6679 = float2(((node_1627.r*_Tex2_Uspeed)+i.uv0.r),(i.uv0.g+(node_1627.r*_Tex2_Vspeed)));
                float4 _TEX2_var = tex2D(_TEX2,TRANSFORM_TEX(node_6679, _TEX2));
                float4 _MASk_var = tex2D(_MASk,TRANSFORM_TEX(i.uv0, _MASk));
                float3 emissive = (pow(((_TEX1_var.rgb*_TEX1_var.a)*(_TEX2_var.rgb*_TEX2_var.a)),_Power)*i.vertexColor.rgb*_TintColor.rgb*_Alpha*(_MASk_var.rgb*_MASk_var.a));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    //fallback "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
