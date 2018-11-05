// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/ShadowMap_Role"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _depthBias("depth bias", float) = 0.001
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			    float4 shadowCoord : TEXCOORD3;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _depthBias;
            
			uniform float4x4 _gWorldToShadow;
            uniform sampler2D _gShadowMapTexture;
            uniform float _gShadowStrength;
            uniform float4 _gSunColor;
            
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.shadowCoord = mul(_gWorldToShadow, mul(unity_ObjectToWorld, v.vertex));
                
                return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
                
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
                
                col.rgb *= shadow;
                
				return col;
			}
			ENDCG
		}
	}
}
