// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomLegacy/UnlitTransWithColor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}
		LOD 100

		Pass
		{
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform half4 _Color;

			void vert( float4 position : POSITION,
				float2 texCoord : TEXCOORD0,
				out float4 oPosition : POSITION,
				out float2 oTexCoord : TEXCOORD0 )
			{
				oPosition = UnityObjectToClipPos( position );
				oTexCoord = texCoord;
			}

			void frag( float2 texCoord : TEXCOORD0,
				out float4 color : COLOR )
			{
				float4 colorFromTex = float4( 1.0, 1.0, 1.0, 1.0);

				colorFromTex = tex2D( _MainTex, texCoord );
				color = colorFromTex * _Color;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
