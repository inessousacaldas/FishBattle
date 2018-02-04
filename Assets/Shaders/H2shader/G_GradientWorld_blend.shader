// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32790,y:32615,varname:node_3138,prsc:2|emission-8418-OUT,alpha-1567-OUT,olwid-288-OUT,olcol-9827-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:31725,y:32435,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:2768,x:31725,y:32631,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_2768,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:8418,x:32197,y:32664,varname:node_8418,prsc:2|A-2768-RGB,B-7241-RGB,T-8119-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4995,x:31116,y:32869,varname:node_4995,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:1020,x:31312,y:32869,varname:node_1020,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-4995-XYZ;n:type:ShaderForge.SFN_Clamp01,id:8119,x:31953,y:32854,varname:node_8119,prsc:2|IN-7616-OUT;n:type:ShaderForge.SFN_Multiply,id:5979,x:31502,y:32932,varname:node_5979,prsc:2|A-1020-OUT,B-138-OUT;n:type:ShaderForge.SFN_ValueProperty,id:138,x:31250,y:33084,ptovrint:False,ptlb:Gradient_Offset,ptin:_Gradient_Offset,varname:node_138,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:7616,x:31744,y:33011,varname:node_7616,prsc:2|VAL-5979-OUT,EXP-6712-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6712,x:31484,y:33142,ptovrint:False,ptlb:Gradient_Power,ptin:_Gradient_Power,varname:node_6712,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:9827,x:32355,y:33119,ptovrint:False,ptlb:OutlineColor,ptin:_OutlineColor,varname:node_9827,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:2085,x:32355,y:32822,varname:node_2085,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1567,x:32583,y:32839,varname:node_1567,prsc:2|A-2085-A,B-9827-A;n:type:ShaderForge.SFN_ValueProperty,id:288,x:32342,y:33043,ptovrint:False,ptlb:Outline,ptin:_Outline,varname:node_288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.0075;proporder:7241-2768-138-6712-9827-288;pass:END;sub:END;*/

Shader "G/G_GradientWorld_blend" {
    Properties {
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (0.5,0.5,0.5,1)
        _Gradient_Offset ("Gradient_Offset", Float ) = 1
        _Gradient_Power ("Gradient_Power", Float ) = 1
        _OutlineColor ("OutlineColor(alpha control)", Color) = (0.5,0.5,0.5,1)
        _Outline ("Outline", Float ) = 0.0075
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            //#pragma multi_compile_shadowcaster
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _Outline;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
				float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*_Outline,1) );
				o.vertexColor = v.vertexColor;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_OutlineColor.rgb,i.vertexColor.a*_OutlineColor.a);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 3.0
            uniform float4 _Color1;
            uniform float4 _Color2;
            uniform float _Gradient_Offset;
            uniform float _Gradient_Power;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = lerp(_Color2.rgb,_Color1.rgb,saturate(pow((i.posWorld.rgb.g*_Gradient_Offset),_Gradient_Power)));
                float3 finalColor = emissive;
                return fixed4(finalColor,(i.vertexColor.a*_OutlineColor.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
