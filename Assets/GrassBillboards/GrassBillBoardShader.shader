Shader "Grass Billboard Shader" {
   Properties {
      _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
      _Color ("Overall Diffuse Color Filter", Color) = (1,1,1,1)
      _FadeStart ("Start.Fade Distance",float) = 100.0
      _FadeEnd ("End.Fade Distance",float) = 120.0
   }
   SubShader {
      Tags {"Queue" = "Transparent"} 
      Pass {   
         ZWrite On
         ZTest Lequal
         Blend SrcAlpha OneMinusSrcAlpha
         
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 

         #include "UnityCG.cginc" 
         
         uniform float4 _LightColor0;
         // color of light source (from "Lighting.cginc") 
         
         // User-specified uniforms            
         uniform sampler2D _MainTex; 
         uniform float4 _Color;          
         uniform float _FadeStart; 
         uniform float _FadeEnd; 
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tex : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            float4 diffuseColor : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            float3 lightDirection;
            float4 rootposition;
                        
            rootposition = mul(UNITY_MATRIX_P, 
              mul(UNITY_MATRIX_MV, float4(input.vertex.x, input.vertex.y, input.vertex.z, 1.0)));
            // I used the UV values of TEXCOORD0 to store the billboard offsets
            //with these I set and rotate each billboard vertice.
            output.pos = mul(UNITY_MATRIX_P, 
              mul(UNITY_MATRIX_MV, float4(input.vertex.x, input.vertex.y, input.vertex.z, 1.0))
              + float4(input.tex.x, input.tex.y, 0.0, 0.0));

            if (input.tex.y > 0) 
            {
            	output.pos.x += sin((_Time * 20.0f) + rootposition.x + rootposition.z) * 0.05f;
            }
  
            // Here I set the actual texture UV coordinates for the output.
            if (input.tex.x < 0) 
            	output.tex.x = 0;
            else if (input.tex.x > 0) 
            	output.tex.x = 1;
            if (input.tex.y == 0) 
            	output.tex.y = 0;
            else if (input.tex.y > 0) 
            	output.tex.y = 1;
            
            // Here we set the default Diffuse values (using Alpha, as a fade value based on distance)	
            output.diffuseColor = _Color;

            float3 normalDirection = normalize(mul(float4(input.normal, 0.0), _World2Object).xyz);            
            float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
            if (0.0 == _WorldSpaceLightPos0.w) // direcitonal light
            {
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
               output.diffuseColor.rgb = _Color.rgb * ((dot(normalDirection, lightDirection) + 1.0)/2.0);
            }

            float dist = distance(_WorldSpaceCameraPos, mul(_Object2World, input.vertex));
            //float dist = distance(_WorldSpaceCameraPos, output.pos);            
            output.diffuseColor.w = 1.0;
            
            if (dist > _FadeStart && dist < _FadeEnd)
          		output.diffuseColor.w = 1.0 - (dist - _FadeStart) / (_FadeEnd - _FadeStart);
            if (dist > _FadeEnd)
          		output.diffuseColor.w = 0.0f;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float4 rcol = tex2D(_MainTex, float2(input.tex.xy));   
             if (rcol.a > 0.5) // opaque face?
             	rcol.a = 1.0;
             else if (rcol.a < 0.1)
             	discard;
            return input.diffuseColor * rcol;
         }
 
         ENDCG
      }
   }
}