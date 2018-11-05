Shader "Dee/Receiver" {
    Properties
    {
        _depthBias("depth bias", float) = 0.001
    }
        
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 300 

        Pass {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            CGPROGRAM
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 shadowCoord : TEXCOORD0;
            };

            uniform float4x4 _gWorldToShadow;
            uniform sampler2D _gShadowMapTexture;
            uniform float _gShadowStrength;
            float _depthBias;
            
            v2f vert (appdata_full v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.shadowCoord = mul(_gWorldToShadow, worldPos);

                return o; 
            }

            fixed4 frag (v2f i) : COLOR0 
            {            
                //计算光空间深度
                float depth = i.shadowCoord.z / i.shadowCoord.w;
                #if defined (SHADER_API_MOBILE) 
                depth = depth*0.5 + 0.5; //(-1, 1)-->(0, 1)
                #elif defined (UNITY_REVERSED_Z)
                depth = 1 - depth;       //(1, 0)-->(0, 1)
                #endif
            
                //计算shadow贴图uv
                i.shadowCoord.xy = i.shadowCoord.xy/i.shadowCoord.w;
                float2 uv = i.shadowCoord.xy;
                uv = uv*0.5 + 0.5; //(-1, 1)-->(0, 1)
                #if USESOFTSHADOW
                float sampleDepth = PCFSample(depth, uv);
                #else
                float sampleDepth = DecodeFloatRGBA(tex2D(_gShadowMapTexture, uv));
                #endif
                
                float shadow = lerp(_gShadowStrength, 1, step(depth - _depthBias, sampleDepth));
                
                //col.rgb *= shadow;

                return 1-shadow;
            }    

            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest  
            ENDCG
        }
    }
}