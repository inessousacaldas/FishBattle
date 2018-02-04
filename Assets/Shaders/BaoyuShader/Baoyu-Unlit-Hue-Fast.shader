// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Baoyu/Unlit/Hue-Fast"
{
 Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black"{}

        _RHueShift1("_RHueShift1", vector) = (1,0,0,0)
        _RHueShift2("_RHueShift2", vector) = (0,1,0,0)
        _RHueShift3("_RHueShift3", vector) = (0,0,1,0)

        _GHueShift1("_GHueShift1", vector) = (1,0,0,0)
        _GHueShift2("_GHueShift2", vector) = (0,1,0,0)
        _GHueShift3("_GHueShift3", vector) = (0,0,1,0)

        _BHueShift1("_BHueShift1", vector) = (1,0,0,0)
        _BHueShift2("_BHueShift2", vector) = (0,1,0,0)
        _BHueShift3("_BHueShift3", vector) = (0,0,1,0)
    }
    SubShader {
 
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
 
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
 
 			sampler2D _MainTex;
            half4 _MainTex_ST;
 
            v2f vert (VertInput v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
 
            sampler2D _MaskTex;
            
            fixed4 _RHueShift1;
            fixed4 _RHueShift2;
            fixed4 _RHueShift3;

            fixed4 _GHueShift1;
            fixed4 _GHueShift2;
            fixed4 _GHueShift3;

            fixed4 _BHueShift1;
            fixed4 _BHueShift2;
            fixed4 _BHueShift3;

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);
                
                fixed4 temp = fixed4(0,0,0,1);
                fixed4x4 _RHueShift = fixed4x4(
                    _RHueShift1,
                    _RHueShift2,
                    _RHueShift3,
                    temp
                );
                fixed4x4 _GHueShift = fixed4x4(
                    _GHueShift1,
                    _GHueShift2,
                    _GHueShift3,
                    temp
                );
                fixed4x4 _BHueShift = fixed4x4(
                    _BHueShift1,
                    _BHueShift2,
                    _BHueShift3,
                    temp
                );

                fixed3 R_Color = mul(_RHueShift,texColor);
                fixed3 G_Color = mul(_GHueShift,texColor);
                fixed3 B_Color = mul(_BHueShift,texColor);
				
            	return fixed4(R_Color*mask.r+G_Color*mask.g+B_Color*mask.b+texColor*(1-mask.r-mask.g-mask.b),texColor.a);
//            	return fixed4(R_Color*mask.r+G_Color*mask.g+B_Color*mask.b+texColor*mask.a,texColor.a);
            }
            ENDCG
        }
    }
    //Fallback "VertexLit" 
}
