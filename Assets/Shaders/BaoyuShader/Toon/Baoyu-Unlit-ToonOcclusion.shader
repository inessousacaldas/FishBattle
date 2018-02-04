Shader "Baoyu/Unlit/ToonOcclusion" {  
    Properties {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _Ramp ("Ramp Texture", 2D) = "white" {}  
        _DirFactor("DirFactor", Range(0,1)) = 1
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_EdgeThickness ("Outline Thickness", Range(0.005, 0.02)) = 0.01
        _AmbientColor("AmbientColor", Color) = (1,1,1,1)
        //_RimColor ("边光颜色", Color) = (1,1,1,1)
        //_RimPow ("边光衰减", Range(1, 15)) = 1
        [HideInInspector]_LightDir ("LightDir", vector) = (0,0,0,0)

    }  
    SubShader {  
        Tags { "RenderType"="Opaque" "Queue" = "AlphaTest+20" }  

        Pass {
            Tags { "LightMode"="ForwardBase" }
            ZTest Greater
            ZWrite off
            Cull Back
            Blend SrcAlpha one
            CGPROGRAM  
            	
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
	            float4 vertex : POSITION;
	            float3 normal : NORMAL;

            };

            struct v2f
            {
	            float4 pos : SV_POSITION;
	            half3 worldNormal : TEXCOORD0;
	            half3 viewDir : TEXCOORD1;
            };
            
            #define _RimColor half4(1,1,1,1)
            #define _RimPow 1
            //half4 _RimColor;
            //half _RimPow;

            v2f vert (appdata v)
            {
	            v2f o;
	            o.pos = UnityObjectToClipPos(v.vertex);
	            o.worldNormal = UnityObjectToWorldNormal(v.normal);
	            o.viewDir = WorldSpaceViewDir(v.vertex);
	            return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
	            half3 normal = normalize(i.worldNormal);
	            half3 viewDir = normalize(i.viewDir);
	            half rimPower = pow(1.0 - saturate(dot(viewDir, normal) ), _RimPow);
	            half3 iRim = rimPower * _RimColor;

	            half4 col = half4(iRim, rimPower);
	            return col;
            }
            ENDCG  
        } 
        Pass
		{
			Cull Front
			ZTest Less
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Baoyu-Outline.cg"
            ENDCG

		}
        Pass 
        {  
            Tags { "LightMode"="ForwardBase" }  
            Cull Back   
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "Baoyu-Toon.cginc"
            ENDCG  
        }
    }  
}  