Shader "Baoyu/Unlit/Shadow"
{
	Properties
	{
		_ShadowColor ("ShadowColor", Color) = (0,0,0,1)
        _Terrain ("Terrain", float) = 0
        _ColorAlpha("ColorAlpha", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Shadow" "Queue" = "AlphaTest+21"}
		LOD 100
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite On
        ZTest Less
        Cull Back
        Stencil{
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Greater
                Pass Replace   
            }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
            
			struct appdata
			{
				half4 vertex : POSITION;
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half _Terrain;
            half4 _ShadowColor;
            half4 _WorldShadowDir;
			half4 _ColorAlpha;

			v2f vert (appdata v)
			{
				v2f o;
                half4 vertex = mul(unity_ObjectToWorld, v.vertex);
                half3 forward = _WorldShadowDir;
                half scale = (_Terrain - vertex.y) / forward.y;
                vertex.xyz = vertex.xyz + scale * forward;
				o.vertex = mul(UNITY_MATRIX_VP, vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _ShadowColor * _ColorAlpha;
			}
			ENDCG
		}
	}
}
