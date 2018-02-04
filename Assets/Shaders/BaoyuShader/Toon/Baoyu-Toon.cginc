
            #include "UnityCG.cginc"  
            #include "Lighting.cginc"  
            #include "AutoLight.cginc"  
            #include "UnityShaderVariables.cginc"  
              
  
            sampler2D _MainTex;  
            sampler2D _Ramp;  
            float4 _MainTex_ST;  
            float4 _AmbientColor;
            float4 _LightDir;
            float _DirFactor;
            struct a2v  
            {  
                float4 vertex : POSITION;  
                float3 normal : NORMAL;  
                float4 texcoord : TEXCOORD0;  
            };   
  
            struct v2f  
            {  
                float4 pos : POSITION;  
                float2 uv : TEXCOORD0;  
                float3 normal : TEXCOORD1;  
                float3 lightDir : TEXCOORD2;
                //LIGHTING_COORDS(2,3)  
            };  
              
            v2f vert (a2v v)  
            {  
                v2f o;  
                o.pos = mul( UNITY_MATRIX_MVP, v.vertex);   
                o.normal  = mul(UNITY_MATRIX_IT_MV, float4(v.normal, 0));// UnityObjectToWorldNormal ( v.normal);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);    
                o.lightDir.xyz = _LightDir;
                //TRANSFER_VERTEX_TO_FRAGMENT(o);  
                return o;  
            }  
              
            half4 frag(v2f i) : COLOR    
            {   
                half4 c = tex2D (_MainTex, i.uv);    
                half3 lightColor = _AmbientColor.xyz;  
                half diff =  dot(normalize(i.normal), normalize(i.lightDir));    
                diff = tex2D(_Ramp, half2(diff, 0.5));  
                //float atten = LIGHT_ATTENUATION(i);  
                lightColor += diff* _DirFactor/* * atten*/;   
                c.rgb = lightColor * c.rgb ;  
                return c;   
            }   
  