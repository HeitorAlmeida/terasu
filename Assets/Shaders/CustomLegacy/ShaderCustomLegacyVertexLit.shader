// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomLegacy/VertexLit"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (1, 1, 1, 1)
		_SpecColor("Specular Color", Color) = (1, 1, 1, 1)
		_Shininess("Shininess", Float) = 10
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				//halmeida - this pass tag makes sure that all uniforms
				//will be correct for ambient light and first light source.
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//halmeida - unity-defined properties.
			uniform float4 _LightColor0;
			//uniform float3 _WorldSpaceCameraPos;
			//uniform float4 _WorldSpaceLightPos0;
			
			//halmeida - user-defined properties.
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			void vert( float4 position : POSITION,
				float3 normal : NORMAL,
				out float4 oPosition : POSITION,
				out float4 color : COLOR )
			{
				oPosition = UnityObjectToClipPos(position);
				
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				
				//halmeida - useful knowledge:
				//Points as float4 have last element 1.
				//Directions as float4 have last element 0.
				//Normals must be transformed with the transposed inverse matrix
				//of the matrix that transforms a direction or point to which
				//they are orthogonal. This ensures they remain orthogonal.
				//By changing the order of the multiplication, we can transpose
				//the matrix that transforms the normals. By doing this, we just
				//need the inverse of the original point transforming matrix.
				
				float3 worldVertex = mul(modelMatrix, position).xyz;
				float3 normalDirection = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - worldVertex);
				float3 vertexToLightSource;
				float3 lightDirection;
				float attenuation;
				
				//halmeida - Unity provides the light position according to the following rule:
				//if the light is directional, the float4 provided contains the direction towards
				//the light in xyz and 0 in w. Otherwise, the float4 provided contains the world
				//space position of the light in xyz and 1 in w. We could use an if command to
				//properly deal with the two cases, but an if command is too expensive within a
				//shader. So, the following commands remove the need for the if through calculations.
				//Moreover, if the light is directional, there is no attenuation with distance. But
				//if the light is not directional, we must account for an attenuation of the light's
				//effect with the distance from its emitting point.
				vertexToLightSource = _WorldSpaceLightPos0.xyz - (_WorldSpaceLightPos0.w * worldVertex);
				lightDirection = normalize(vertexToLightSource);
				attenuation = max(1.0, _WorldSpaceLightPos0.w * length(vertexToLightSource));
				attenuation = 1.0 / attenuation;
				
				//halmeida - The ambient light reflection term of the final color.
				//halmeida - Unity provides am ambient light value that is actually
				//half of the color value set in the Render Settings.
				float3 ambientLighting = 2.0 * UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
				
				//halmeida - The diffuse light reflection term of the final color.
				float diffuseAngleFactor = max(dot(normalDirection, lightDirection), 0.0);
				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * diffuseAngleFactor;
				
				//halmeida - The specular light reflection term of the final color.
				float3 specularDirection = reflect(-lightDirection, normalDirection);
				float specularAngleFactor = max(dot(specularDirection, viewDirection), 0.0);
				specularAngleFactor = pow(specularAngleFactor, _Shininess);
				float3 specularReflection = float3(0.0, 0.0, 0.0);
				//halmeida - we only show the specular term inside the diffuse term lit area. This means
				//that if the diffuse angle factor is 0 we have no specular, because that means the surface
				//does not face the light, not even slightly. It is parallel to the light rays or faces the
				//other direction. Again, we must avoid the if command.
				specularReflection += ceil(diffuseAngleFactor) * attenuation * _LightColor0.rgb * _SpecColor.rgb * specularAngleFactor;
				
				color = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
			}
			
			void frag( float4 color : COLOR,
				out float4 oColor : COLOR )
			{
				oColor = color;
			}
			ENDCG
		}
		
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardAdd"
				//halmeida - this pass tag makes sure that all uniforms will
				//be correct for additional light sources.
			}
			Blend One One
			//hameida - configuring blending equation for additive blending,
			//so that the color in this pass will be summed to the color obtained
			//in the previous step.
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//halmeida - unity-defined properties
			uniform float4 _LightColor0;
			//uniform float3 _WorldSpaceCameraPos;
			//uniform float4 _WorldSpaceLightPos0;
			
			//halmeida - user-defined properties
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			void vert( float4 position : POSITION,
				float3 normal : NORMAL,
				out float4 oPosition : POSITION,
				out float4 color : COLOR )
			{
				oPosition = UnityObjectToClipPos(position);
			
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				
				float3 worldVertex = mul(modelMatrix, position).xyz;
				float3 normalDirection = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - worldVertex);
				float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - (_WorldSpaceLightPos0.w * worldVertex);
				float3 lightDirection = normalize(vertexToLightSource);
				float attenuation = 1.0 / max(1.0, _WorldSpaceLightPos0.w * length(vertexToLightSource));
				
				float diffuseAngleFactor = max(0.0, dot(normalDirection, lightDirection));
				float3 diffuseReflection = attenuation * _Color.rgb * _LightColor0.rgb * diffuseAngleFactor;
				
				float3 specularDirection = reflect(-lightDirection, normalDirection);
				float specularAngleFactor = max(0.0, dot(viewDirection, specularDirection));
				specularAngleFactor = pow(specularAngleFactor, _Shininess);
				float3 specularReflection = ceil(diffuseAngleFactor) * attenuation * _SpecColor.rgb * _LightColor0.rgb * specularAngleFactor;
				
				color = float4(diffuseReflection + specularReflection, 1.0);
			}
			
			void frag( float4 color : COLOR,
				out float4 oColor : COLOR )
			{
				oColor = color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
