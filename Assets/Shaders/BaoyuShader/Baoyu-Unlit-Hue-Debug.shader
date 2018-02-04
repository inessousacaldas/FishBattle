
//用于检查Mask变色区域贴图边缘
Shader "Baoyu/Unlit/Hue-Debug"
{
	Properties 
	{
		_MainTex ("Texture", 2D)								= "black" {} 
		_MaskTex ("Mask", 2D)                                   = "black"{}
		_maskThreshold ("Mask Edge Threshold ",Range(0,1)) 		= 1
 	}

	//=========================================================================
	SubShader 
	{
 		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" }
		
    	Pass 
		{    
      		Cull Off
      		ZTest LEqual
      		ZWrite On
      		Blend Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct v2f {
                half4  pos : SV_POSITION;
                half2  uv : TEXCOORD0;
            };

			//=============================================
			v2f vert (VertInput v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord;
                return o;
            }
			
			sampler2D _MainTex;
            sampler2D _MaskTex;
            fixed _maskThreshold;
            
			//=============================================
			fixed4 frag ( v2f i ):COLOR
			{
				fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);
               
				return lerp(mask,origin,1-(mask.r+mask.g+mask.b)*_maskThreshold);
			}
			ENDCG
		}
 	}
 	//Fallback "VertexLit"
 }
