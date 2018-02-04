// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33199,y:32755,varname:node_4795,prsc:2|emission-3084-OUT,amdfl-4847-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:29629,y:32672,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8842-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:31293,y:32758,varname:node_2393,prsc:2|A-3185-OUT,B-797-RGB;n:type:ShaderForge.SFN_Color,id:797,x:31010,y:32964,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:849,x:31821,y:32949,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:_MASK,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:8842,x:29368,y:32717,varname:node_8842,prsc:2|A-5135-OUT,B-2668-OUT;n:type:ShaderForge.SFN_Tex2d,id:1963,x:29321,y:33157,ptovrint:False,ptlb:Tex_Dis,ptin:_Tex_Dis,varname:node_1963,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7acc33f24a06fcd46baa112415b42195,ntxv:0,isnm:False|UVIN-584-OUT;n:type:ShaderForge.SFN_Multiply,id:2668,x:29600,y:33161,varname:node_2668,prsc:2|A-1963-R,B-5425-OUT,C-3813-OUT,D-1963-A;n:type:ShaderForge.SFN_Vector1,id:3813,x:29312,y:33419,varname:node_3813,prsc:2,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:5425,x:29319,y:33336,ptovrint:False,ptlb:Disturbance,ptin:_Disturbance,varname:node_5425,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:9234,x:30388,y:32735,varname:node_9234,prsc:2|A-4809-OUT,B-4926-OUT,C-6074-A;n:type:ShaderForge.SFN_ValueProperty,id:4926,x:30167,y:32905,ptovrint:False,ptlb:ZT,ptin:_ZT,varname:node_4926,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:4302,x:30576,y:32784,varname:node_4302,prsc:2|VAL-9234-OUT,EXP-4624-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4624,x:30307,y:32905,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_4624,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:2499,x:31782,y:33138,varname:node_2499,prsc:2;n:type:ShaderForge.SFN_Append,id:5135,x:29103,y:32648,varname:node_5135,prsc:2|A-5898-OUT,B-1565-OUT;n:type:ShaderForge.SFN_TexCoord,id:4396,x:28717,y:32556,varname:node_4396,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3224,x:28728,y:32423,varname:node_3224,prsc:2|A-8387-TSL,B-7592-OUT;n:type:ShaderForge.SFN_Time,id:8387,x:28515,y:32400,varname:node_8387,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7592,x:28525,y:32516,ptovrint:False,ptlb:U_Sapeed,ptin:_U_Sapeed,varname:node_7592,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:5898,x:28905,y:32487,varname:node_5898,prsc:2|A-3224-OUT,B-4396-U;n:type:ShaderForge.SFN_Multiply,id:7290,x:28728,y:32722,varname:node_7290,prsc:2|A-6634-TSL,B-4956-OUT;n:type:ShaderForge.SFN_Time,id:6634,x:28514,y:32633,varname:node_6634,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4956,x:28514,y:32803,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:_U_Sapeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:1565,x:28917,y:32702,varname:node_1565,prsc:2|A-4396-V,B-7290-OUT;n:type:ShaderForge.SFN_Append,id:584,x:29065,y:33195,varname:node_584,prsc:2|A-420-OUT,B-1638-OUT;n:type:ShaderForge.SFN_TexCoord,id:9952,x:28679,y:33103,varname:node_9952,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:753,x:28690,y:32970,varname:node_753,prsc:2|A-6168-TSL,B-8874-OUT;n:type:ShaderForge.SFN_Time,id:6168,x:28477,y:32947,varname:node_6168,prsc:2;n:type:ShaderForge.SFN_Add,id:420,x:28867,y:33034,varname:node_420,prsc:2|A-753-OUT,B-9952-U;n:type:ShaderForge.SFN_Multiply,id:5606,x:28690,y:33269,varname:node_5606,prsc:2|A-9212-TSL,B-6296-OUT;n:type:ShaderForge.SFN_Time,id:9212,x:28476,y:33180,varname:node_9212,prsc:2;n:type:ShaderForge.SFN_Add,id:1638,x:28879,y:33249,varname:node_1638,prsc:2|A-9952-V,B-5606-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8874,x:28465,y:33116,ptovrint:False,ptlb:Dis_U_Speed,ptin:_Dis_U_Speed,varname:node_8874,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:6296,x:28468,y:33352,ptovrint:False,ptlb:Dis_V_Speed,ptin:_Dis_V_Speed,varname:_Dis_V_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Desaturate,id:2897,x:29934,y:32685,varname:node_2897,prsc:2|COL-6074-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:4809,x:30154,y:32640,ptovrint:False,ptlb:Color_Sweitch,ptin:_Color_Sweitch,varname:node_4809,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6074-RGB,B-2897-OUT;n:type:ShaderForge.SFN_Posterize,id:3694,x:30725,y:32932,varname:node_3694,prsc:2|IN-4302-OUT,STPS-4780-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4780,x:30477,y:33141,ptovrint:False,ptlb:Posterize,ptin:_Posterize,varname:node_4780,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_SwitchProperty,id:3185,x:30990,y:32775,ptovrint:False,ptlb:Posterize_switch,ptin:_Posterize_switch,varname:node_3185,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4302-OUT,B-3694-OUT;n:type:ShaderForge.SFN_Fresnel,id:6919,x:30391,y:32188,varname:node_6919,prsc:2|EXP-2539-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2539,x:30209,y:32204,ptovrint:False,ptlb:Fresnel_EXP,ptin:_Fresnel_EXP,varname:node_4149,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:6376,x:30698,y:32192,varname:node_6376,prsc:2|A-6919-OUT,B-9511-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9511,x:30459,y:32334,ptovrint:False,ptlb:Fresnel_ZT,ptin:_Fresnel_ZT,varname:node_7072,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Color,id:3784,x:30871,y:32433,ptovrint:False,ptlb:Fresnel_Color,ptin:_Fresnel_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Power,id:2136,x:31009,y:32261,varname:node_2136,prsc:2|VAL-6376-OUT,EXP-6018-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6018,x:30726,y:32340,ptovrint:False,ptlb:Fresnel_POWER,ptin:_Fresnel_POWER,varname:node_3567,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4847,x:31211,y:32295,varname:node_4847,prsc:2|A-2136-OUT,B-3784-RGB,C-7419-RGB;n:type:ShaderForge.SFN_Add,id:3080,x:31627,y:32638,varname:node_3080,prsc:2|A-4847-OUT,B-2393-OUT;n:type:ShaderForge.SFN_Multiply,id:1772,x:32267,y:32957,varname:node_1772,prsc:2|A-8731-OUT,B-849-RGB,C-849-A,D-2499-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:8731,x:31841,y:32718,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_8731,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2393-OUT,B-3080-OUT;n:type:ShaderForge.SFN_Tex2d,id:2199,x:31836,y:33280,ptovrint:False,ptlb:water_DISE,ptin:_water_DISE,varname:node_2199,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:8892,x:32563,y:33044,varname:node_8892,prsc:2|A-1772-OUT,B-5806-OUT;n:type:ShaderForge.SFN_Multiply,id:5806,x:32221,y:33244,varname:node_5806,prsc:2|A-2199-RGB,B-7975-RGB,C-2499-RGB;n:type:ShaderForge.SFN_Color,id:7975,x:31854,y:33529,ptovrint:False,ptlb:TEX_Color,ptin:_TEX_Color,varname:node_7975,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:7419,x:30891,y:32621,varname:node_7419,prsc:2;n:type:ShaderForge.SFN_SwitchProperty,id:3084,x:32810,y:32999,ptovrint:False,ptlb:water_dise,ptin:_water_dise,varname:node_3084,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1772-OUT,B-8892-OUT;proporder:6074-4809-797-4624-4926-7592-4956-1963-5425-8874-6296-849-3185-4780-8731-2539-9511-3784-6018-3084-2199-7975;pass:END;sub:END;*/

Shader "H2/H_Disturbance_Add_Double" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _Color_Sweitch ("Color_Sweitch", Float ) = 0
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Power ("Power", Float ) = 1
        _ZT ("ZT", Float ) = 1
        _U_Sapeed ("U_Sapeed", Float ) = 0
        _V_Speed ("V_Speed", Float ) = 0
        _Tex_Dis ("Tex_Dis", 2D) = "white" {}
        _Disturbance ("Disturbance", Float ) = 2
        _Dis_U_Speed ("Dis_U_Speed", Float ) = 4
        _Dis_V_Speed ("Dis_V_Speed", Float ) = 0
        _MASK ("MASK", 2D) = "white" {}
        [MaterialToggle] _Posterize_switch ("Posterize_switch", Float ) = 0
        _Posterize ("Posterize", Float ) = 2
        [MaterialToggle] _Fresnel ("Fresnel", Float ) = 0
        _Fresnel_EXP ("Fresnel_EXP", Float ) = 0.2
        _Fresnel_ZT ("Fresnel_ZT", Float ) = 0.5
        _Fresnel_Color ("Fresnel_Color", Color) = (1,1,1,1)
        _Fresnel_POWER ("Fresnel_POWER", Float ) = 1
        [MaterialToggle] _water_dise ("water_dise", Float ) = 0
        _water_DISE ("water_DISE", 2D) = "white" {}
        _TEX_Color ("TEX_Color", Color) = (0.5,0.5,0.5,1)
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
            #pragma multi_compile_fog
            ///#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
           // #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform sampler2D _Tex_Dis; uniform float4 _Tex_Dis_ST;
            uniform float _Disturbance;
            uniform float _ZT;
            uniform float _Power;
            uniform float _U_Sapeed;
            uniform float _V_Speed;
            uniform float _Dis_U_Speed;
            uniform float _Dis_V_Speed;
            uniform fixed _Color_Sweitch;
            uniform float _Posterize;
            uniform fixed _Posterize_switch;
            uniform float _Fresnel_EXP;
            uniform float _Fresnel_ZT;
            uniform float4 _Fresnel_Color;
            uniform float _Fresnel_POWER;
            uniform fixed _Fresnel;
            uniform sampler2D _water_DISE; uniform float4 _water_DISE_ST;
            uniform float4 _TEX_Color;
            uniform fixed _water_dise;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_8387 = _Time + _TimeEditor;
                float4 node_6634 = _Time + _TimeEditor;
                float4 node_6168 = _Time + _TimeEditor;
                float4 node_9212 = _Time + _TimeEditor;
                float2 node_584 = float2(((node_6168.r*_Dis_U_Speed)+i.uv0.r),(i.uv0.g+(node_9212.r*_Dis_V_Speed)));
                float4 _Tex_Dis_var = tex2D(_Tex_Dis,TRANSFORM_TEX(node_584, _Tex_Dis));
                float2 node_8842 = (float2(((node_8387.r*_U_Sapeed)+i.uv0.r),(i.uv0.g+(node_6634.r*_V_Speed)))+(_Tex_Dis_var.r*_Disturbance*0.1*_Tex_Dis_var.a));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8842, _MainTex));
                float3 node_4302 = pow((lerp( _MainTex_var.rgb, dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)), _Color_Sweitch )*_ZT*_MainTex_var.a),_Power);
                float3 node_2393 = (lerp( node_4302, floor(node_4302 * _Posterize) / (_Posterize - 1), _Posterize_switch )*_TintColor.rgb);
                float3 node_4847 = (pow((pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel_EXP)*_Fresnel_ZT),_Fresnel_POWER)*_Fresnel_Color.rgb*i.vertexColor.rgb);
                float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
                float3 node_1772 = (lerp( node_2393, (node_4847+node_2393), _Fresnel )*_MASK_var.rgb*_MASK_var.a*i.vertexColor.rgb);
                float4 _water_DISE_var = tex2D(_water_DISE,TRANSFORM_TEX(i.uv0, _water_DISE));
                float3 emissive = lerp( node_1772, (node_1772+(_water_DISE_var.rgb*_TEX_Color.rgb*i.vertexColor.rgb)), _water_dise );
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
