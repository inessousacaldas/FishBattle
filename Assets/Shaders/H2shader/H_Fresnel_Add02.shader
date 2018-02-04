// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33225,y:32506,varname:node_4795,prsc:2|emission-3612-OUT;n:type:ShaderForge.SFN_Fresnel,id:4021,x:32527,y:32483,varname:node_4021,prsc:2|EXP-7721-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7721,x:32336,y:32532,ptovrint:False,ptlb:Fresnel EXP,ptin:_FresnelEXP,varname:node_7721,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:2388,x:32716,y:32607,varname:node_2388,prsc:2|A-4021-OUT,B-5200-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5200,x:32445,y:32639,ptovrint:False,ptlb:Fresnel Bright,ptin:_FresnelBright,varname:node_5200,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Color,id:4910,x:32516,y:32795,ptovrint:False,ptlb:Fresnel Color,ptin:_FresnelColor,varname:node_4910,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.3,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3612,x:33000,y:32616,varname:node_3612,prsc:2|A-2388-OUT,B-4910-RGB,C-9647-RGB,D-5357-OUT,E-1934-OUT;n:type:ShaderForge.SFN_VertexColor,id:9647,x:32515,y:32964,varname:node_9647,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5357,x:32733,y:33188,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_5357,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1934,x:32881,y:32809,varname:node_1934,prsc:2|A-4910-A,B-9647-A;proporder:7721-5200-4910-5357;pass:END;sub:END;*/

Shader "H2/H_Fresnel_Add02" {
    Properties {
        _FresnelEXP ("Fresnel EXP", Float ) = 3
        _FresnelBright ("Fresnel Bright", Float ) = 3
        _FresnelColor ("Fresnel Color", Color) = (0.5,0.3,1,1)
        _Alpha ("Alpha", Float ) = 1
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform float _FresnelEXP;
            uniform float _FresnelBright;
            uniform float4 _FresnelColor;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = ((pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelEXP)*_FresnelBright)*_FresnelColor.rgb*i.vertexColor.rgb*_Alpha*(_FresnelColor.a*i.vertexColor.a));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
