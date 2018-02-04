Shader "Baoyu/Unlit/Toon" {  
    Properties {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _Ramp ("Ramp Texture", 2D) = "white" {}  
        _DirFactor("DirFactor", Range(0,1)) = 1
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_EdgeThickness ("Outline Thickness", Range(0, 1)) = 0.01
        _AmbientColor("AmbientColor", Color) = (1,1,1,1)
        [HideInInspector]_LightDir ("LightDir", vector) = (0,0,0,0)

    }  
    SubShader {  
        Tags { "RenderType"="Opaque" "Queue" = "AlphaTest+20" }  
      
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
          
        Pass {  
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