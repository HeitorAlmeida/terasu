// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomLegacy/WaterAlpha"
{
	Properties
	{
		_DecalMap("Decal Map", 2D) = "white" {}
		_NormalMapForward("Forward Waves Normal", 2D) = "white" {}
		_NormalMapBackwards("Backwards Waves Normal", 2D) = "white" {}
		_EnvironmentMap("Environment Map", Cube) = "white" {}
		_WaveFrontX("Wave Front X", Float) = 0.0
		_WaveFrontY("Wave Front Y", Float) = 0.0
		//_Alpha("Alpha", Float) = 0.0
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
		}
		Pass
		{
			Zwrite off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			//uniform float3 _WorldSpaceCameraPos;
			//uniform float4x4 _Object2World;
			//uniform float4x4 _World2Object;
			
			uniform sampler2D _DecalMap;
			uniform sampler2D _NormalMapForward;
			uniform sampler2D _NormalMapBackwards;
			uniform samplerCUBE _EnvironmentMap;
			uniform float _WaveFrontX;
			uniform float _WaveFrontY;
			//uniform float _Alpha;
			
			void vert( float4 position : POSITION,
				float3 normal : NORMAL,
				float3 tangent : TANGENT,
				float2 texCoord : TEXCOORD0,
				out float4 oPosition : POSITION,
				out float2 oTexCoord : TEXCOORD0,
				out float4 oObjectSpacePosition : TEXCOORD1,
				out float3 oNormal : TEXCOORD2,
				out float3 oTangent : TEXCOORD3 )
			{
				oObjectSpacePosition = position;
				oPosition = UnityObjectToClipPos(position);
				oTexCoord = texCoord;
				oNormal = normal;
				oTangent = tangent;
			}
			
			float3 expand( float3 compressed )
			{
				return (compressed - 0.5) * 2.0;
			}
			
			void frag( float2 texCoord : TEXCOORD0,
				float4 position : TEXCOORD1,
				float3 normal : TEXCOORD2,
				float3 tangent : TEXCOORD3,
				out float4 color : COLOR )
			{
				normal = normalize(normal);
				tangent = normalize(tangent);
				float3 binormal = cross(tangent, normal);
				float3x3 objectToTexture = float3x3(tangent, binormal, normal);
				float waveFrontXCapped = max(0, min(1, _WaveFrontX));
				float waveFrontYCapped = max(0, min(1, _WaveFrontY));
				float newX = texCoord.x - waveFrontXCapped;
				float newY = texCoord.y - waveFrontYCapped;
				newX = newX + (min(0.0, newX) / newX);
				newY = newY + (min(0.0, newY) / newY);
				float2 newTexCoord = float2(newX, newY);
				float3 normalCompressed = tex2D(_NormalMapForward, newTexCoord).xyz;
				float3 normalFromTex = expand(normalCompressed);
				float3 firstNewNormal = mul(normalFromTex, objectToTexture);
				firstNewNormal.xy = -1 * firstNewNormal.xy;
				newX = texCoord.x + waveFrontXCapped;
				newY = texCoord.y + waveFrontYCapped;
				newX -= floor(newX);
				newY -= floor(newY);
				newTexCoord = float2(newX, newY);
				normalCompressed = tex2D(_NormalMapBackwards, newTexCoord).xyz;
				normalFromTex = expand(normalCompressed);
				float3 secondNewNormal = mul(normalFromTex, objectToTexture);
				secondNewNormal.xy = -1 * secondNewNormal.xy;
				//float3 resultingNormal = normalize(firstNewNormal + secondNewNormal);
				secondNewNormal = normalize(firstNewNormal + secondNewNormal);
				float4 objectSpaceCameraPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 incidentRay = (position - objectSpaceCameraPos).xyz;
				//float3 reflectedRay = reflect(incidentRay, resultingNormal);
				float3 reflectedRay = reflect(incidentRay, secondNewNormal);
				float3 worldSpaceReflectedRay = mul(unity_ObjectToWorld, float4(reflectedRay, 0.0)).xyz;
				float4 reflectedColor = texCUBE(_EnvironmentMap, worldSpaceReflectedRay);
				float4 decalColor = tex2D(_DecalMap, texCoord);
				color = float4(reflectedColor.rgb, decalColor.r);
			}
			
			ENDCG
		}
	}
	//FallBack "Diffuse"
}
