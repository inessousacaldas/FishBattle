// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32870,y:32684,varname:node_3138,prsc:2|emission-9612-OUT,alpha-7540-OUT;n:type:ShaderForge.SFN_Tex2d,id:4077,x:31820,y:32730,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4077,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:876,x:31820,y:32415,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_876,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9612,x:32464,y:32562,varname:node_9612,prsc:2|A-876-RGB,B-2156-RGB,C-4077-RGB,D-1726-OUT;n:type:ShaderForge.SFN_VertexColor,id:2156,x:31820,y:32581,varname:node_2156,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:1726,x:32218,y:32785,ptovrint:False,ptlb:MainTex_Power,ptin:_MainTex_Power,varname:node_1726,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Step,id:9142,x:32137,y:32956,varname:node_9142,prsc:2|A-3392-OUT,B-312-OUT;n:type:ShaderForge.SFN_OneMinus,id:8999,x:32316,y:32986,varname:node_8999,prsc:2|IN-9142-OUT;n:type:ShaderForge.SFN_Multiply,id:931,x:32523,y:32943,varname:node_931,prsc:2|A-7175-OUT,B-8999-OUT;n:type:ShaderForge.SFN_VertexColor,id:5452,x:31277,y:33296,varname:node_5452,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:3623,x:31526,y:33296,varname:node_3623,prsc:2|IN-5452-A;n:type:ShaderForge.SFN_Slider,id:9849,x:31540,y:33163,ptovrint:False,ptlb:diss_amount,ptin:_diss_amount,varname:node_9849,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.01,cur:-0.01,max:1;n:type:ShaderForge.SFN_SwitchProperty,id:312,x:31912,y:33239,ptovrint:False,ptlb:particle_control(A),ptin:_particle_controlA,varname:node_312,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9849-OUT,B-2223-OUT;n:type:ShaderForge.SFN_Tex2d,id:3615,x:31511,y:32926,ptovrint:False,ptlb:Dissolve_Tex,ptin:_Dissolve_Tex,varname:node_3615,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Desaturate,id:3447,x:31717,y:32926,varname:node_3447,prsc:2|COL-3615-RGB;n:type:ShaderForge.SFN_RemapRange,id:2223,x:31697,y:33296,varname:node_2223,prsc:2,frmn:0,frmx:1,tomn:-0.01,tomx:1|IN-3623-OUT;n:type:ShaderForge.SFN_Multiply,id:7175,x:32075,y:32803,varname:node_7175,prsc:2|A-4077-A,B-1726-OUT;n:type:ShaderForge.SFN_Clamp01,id:7540,x:32692,y:32943,varname:node_7540,prsc:2|IN-931-OUT;n:type:ShaderForge.SFN_Multiply,id:3392,x:31884,y:33041,varname:node_3392,prsc:2|A-3447-OUT,B-3615-A;proporder:876-4077-1726-9849-312-3615;pass:END;sub:END;*/

Shader "G/G_dissolve_blend2" {
    Properties {
        _MainColor ("MainColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _MainTex_Power ("MainTex_Power", Float ) = 3
        _diss_amount ("diss_amount", Range(-0.01, 1)) = -0.01
        [MaterialToggle] _particle_controlA ("particle_control(A)", Float ) = -0.01
        _Dissolve_Tex ("Dissolve_Tex", 2D) = "white" {}
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
            uniform float _MainTex_Power;
            uniform float _diss_amount;
            uniform fixed _particle_controlA;
            uniform sampler2D _Dissolve_Tex; uniform float4 _Dissolve_Tex_ST;
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
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_MainColor.rgb*i.vertexColor.rgb*_MainTex_var.rgb*_MainTex_Power);
                float3 finalColor = emissive;
                float4 _Dissolve_Tex_var = tex2D(_Dissolve_Tex,TRANSFORM_TEX(i.uv0, _Dissolve_Tex));
                return fixed4(finalColor,saturate(((_MainTex_var.a*_MainTex_Power)*(1.0 - step((dot(_Dissolve_Tex_var.rgb,float3(0.3,0.59,0.11))*_Dissolve_Tex_var.a),lerp( _diss_amount, ((1.0 - i.vertexColor.a)*1.01+-0.01), _particle_controlA ))))));
            }
            ENDCG
        }
    }
    //Fallback "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
