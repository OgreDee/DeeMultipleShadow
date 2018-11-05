// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Dee/PlanarShadow Role"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            Material {Diffuse(0,0,1,1)}
            //Lighting On
            
            SetTexture[_MainTex]
            {
                Combine texture*primary
            }
        }
        
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            Offset -1,0
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            uniform float4x4 _World2Ground;
            uniform float4x4 _Ground2World;
            
            struct a2v
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert(a2v i)
            {
                v2f o = (v2f)0;
                
                float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
                
                float3 gsLDir = mul((float3x3)_World2Ground, _WorldSpaceLightPos0);
                gsLDir = normalize(gsLDir);
                
                float4 glPos = mul(_World2Ground, worldPos);
                
                glPos.xz = glPos.xz - (glPos.y / gsLDir.y) * gsLDir.xz;
                glPos.y = min(glPos.y, 0);
                
                worldPos = mul(_Ground2World, glPos);
                
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return float4(0.3,0.3,0.3,1);
            } 
            
            
            ENDCG
            
            
        }
    }
}
