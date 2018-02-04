// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Baoyu/UI/CloudEffect"
{
    Properties{
        _MainTex("MainTex", 2D) = "white" {}
        _GTex_ST("G Channel Tiling & Offset",Vector) = (1,1,0,0)
        _BTex_ST("B Channel Tiling & Offset", Vector) = (1, 1, 0, 0)
        _Color1("Color1", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (1,1,1,1)
        _Speed1_U("_Speed1_U", Float) = 1
        _Speed1_V("_Speed1_V", Float) = 1
        _Speed2_U("_Speed2_U", Float) = 1
        _Speed2_V("_Speed2_V", Float) = 1
        _Speed3_U("_Speed3_U", Float) = 1
        _Speed3_V("_Speed3_V", Float) = 1
    }

        SubShader{
            Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    half2 uv1 : TEXCOORD0;
                    half2 uv2 : TEXCOORD1;
                    half2 uv3 : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _GTex_ST;
                float4 _BTex_ST;

                float _Speed1_U;
                float _Speed1_V;
                float _Speed2_U;
                float _Speed2_V;
                float _Speed3_U;
                float _Speed3_V;

                float4 _Color1;
                float4 _Color2;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.uv2 = TRANSFORM_TEX(v.texcoord, _GTex);
                    o.uv3 = TRANSFORM_TEX(v.texcoord, _BTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 rChl = tex2D(_MainTex, i.uv1 + float2(_Speed1_U,_Speed1_V)*_Time);
                    fixed4 gChl = tex2D(_MainTex, i.uv2 + float2(_Speed2_U,_Speed2_V)*_Time);
                    fixed4 bChl = tex2D(_MainTex, i.uv3 + float2(_Speed3_U, _Speed3_V)*_Time);

                    float alpha = rChl.r + gChl.g + bChl.b;
                    float3 finalColor = lerp(_Color1.rgb, _Color2.rgb, alpha);
                    return fixed4(finalColor, alpha);
                }
                ENDCG
            }
        }
}
