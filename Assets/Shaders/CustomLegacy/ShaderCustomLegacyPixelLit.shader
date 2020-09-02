// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomLegacy/PixelLit"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess("Shininess", Float) = 10
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _LightColor0;
			//uniform float3 _WorldSpaceCameraPos;
			//uniform float4 _WorldSpaceLightPos0;
			
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			void vert( float4 position : POSITION,
				float3 normal : NORMAL,
				out float4 oPosition : POSITION,
				out float4 worldPosition : TEXCOORD0,
				out float3 worldNormal : TEXCOORD1 )
			{
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
			
				oPosition = UnityObjectToClipPos(position);
				worldPosition = mul(modelMatrix, position);
				worldNormal = normalize(mul(float4(normal, 0.0), modelMatrixInverse));
			}
			
			void frag( float4 worldPosition : TEXCOORD0,
				float3 worldNormal : TEXCOORD1,
				out float4 color : COLOR )
			{
				float3 ambientReflection = 2.0 * UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
			
				float3 vertexToLight = _WorldSpaceLightPos0.xyz - (_WorldSpaceLightPos0.w * worldPosition.xyz);
				float3 lightDirection = normalize(vertexToLight);
				float3 normalDirection = normalize(worldNormal);
				float diffuseAngleFactor = max(0.0, dot(normalDirection, lightDirection));
				float attenuation = max(1.0, _WorldSpaceLightPos0.w * length(vertexToLight));
				attenuation = 1.0 / attenuation;
				float3 diffuseReflection = diffuseAngleFactor * _Color.rgb * _LightColor0.rgb * attenuation;
				
				float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition.xyz);
				float3 specularDirection = reflect(-lightDirection, normalDirection);
				float specularAngleFactor = max(0.0, dot(viewDirection, specularDirection));
				specularAngleFactor = pow(specularAngleFactor, _Shininess);
				specularAngleFactor *= ceil(diffuseAngleFactor);
				float3 specularReflection = specularAngleFactor * _SpecColor.rgb * _LightColor0.rgb * attenuation;
				
				color = float4(ambientReflection + diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardAdd"
			}
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _LightColor0;
			//uniform float3 _WorldSpaceCameraPos;
			//uniform float4 _WorldSpaceLightPos0;
			
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			void vert( float4 position : POSITION,
				float3 normal : NORMAL,
				out float4 oPosition : POSITION,
				out float4 worldPosition : TEXCOORD0,
				out float3 worldNormal : TEXCOORD1 )
			{
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
			
				oPosition = UnityObjectToClipPos(position);
				worldPosition = mul(modelMatrix, position);
				worldNormal = normalize(mul(float4(normal, 0.0), modelMatrixInverse));
			}
			
			void frag( float4 worldPosition : TEXCOORD0,
				float3 worldNormal : TEXCOORD1,
				out float4 color : COLOR )
			{
				float3 normalDirection = normalize(worldNormal);
				float3 vertexToLight = _WorldSpaceLightPos0.xyz - (_WorldSpaceLightPos0.w * worldPosition.xyz);
				float3 lightDirection = normalize(vertexToLight);
				float attenuation = max(1.0, _WorldSpaceLightPos0.w * length(vertexToLight));
				attenuation = 1.0 / attenuation;
				float diffuseAngleFactor = max(0.0, dot(normalDirection, lightDirection));
				float3 diffuseReflection = diffuseAngleFactor * _Color.rgb * _LightColor0.rgb * attenuation;
				
				float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition.xyz);
				float3 specularDirection = reflect(-lightDirection, normalDirection);
				float specularAngleFactor = max(0.0, dot(viewDirection, specularDirection));
				specularAngleFactor = pow(specularAngleFactor, _Shininess);
				specularAngleFactor *= ceil(diffuseAngleFactor);
				float3 specularReflection = specularAngleFactor * _SpecColor.rgb * _LightColor0.rgb * attenuation;
				
				color = float4(diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
