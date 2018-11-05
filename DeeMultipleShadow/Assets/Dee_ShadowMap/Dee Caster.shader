Shader "Dee/Caster"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"}
		LOD 100

		Pass
		{
        Cull front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "HLSLSupport.cginc"
            
            #pragma only_renderers d3d9 d3d11 glcore gles3 metal
            
            
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float2 depth:TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float _gShadowBias;
            
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex.z += _gShadowBias;
                o.depth = o.vertex.zw;
                
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float depth = i.depth.x/i.depth.y;
            #if defined (SHADER_API_MOBILE) 
                depth = depth*0.5 + 0.5; //(-1, 1)-->(0, 1)
            #elif defined (UNITY_REVERSED_Z)
                depth = 1 - depth;       //(1, 0)-->(0, 1)
            #endif

                return EncodeFloatRGBA(depth);
			}
			ENDCG
		}
	}
}
