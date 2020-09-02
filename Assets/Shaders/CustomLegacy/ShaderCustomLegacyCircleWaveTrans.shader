// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomLegacy/CircleWaveTrans"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_BaseAlpha ("Base Alpha", Range(0.0, 1.0)) = 1.0
		_WaveColor ("Wave Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_WaveLength ("Wave Length", Float) = 0.0
		_WaveFront ("Wave Front", Float) = 0.0
		_Transforming ("Transforming Bool", Float) = 0.0
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}
		Pass
		{
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			uniform sampler2D _MainTex;
			uniform half _BaseAlpha;
			uniform half4 _WaveColor;
			uniform float _WaveLength;
			uniform float _WaveFront;
			uniform half _Transforming;
			
			void vert( float4 position : POSITION,
				float2 texCoord : TEXCOORD0,
				out float4 oPosition : POSITION,
				out float2 oTexCoord : TEXCOORD0,
				out float2 magnitude : TEXCOORD1 )
			{
				oPosition = UnityObjectToClipPos( position );
				oTexCoord = texCoord;
				magnitude = float2( length( position ), 0.0 );
			}
			
			void frag( float2 texCoord : TEXCOORD0,
				float2 magnitude : TEXCOORD1,
				out float4 color : COLOR )
			{
				float realMagnitude = magnitude.x;
				float remainder = 0.0;
				float extraAlpha = 0.0;
				float thisWave = 0.0;
				float4 originalColor = float4(1.0, 1.0, 1.0, 1.0);
				
				originalColor = tex2D( _MainTex, texCoord );
				color.rgb = originalColor.rrr;
				color.a = _BaseAlpha;
				if( (realMagnitude <= _WaveFront) && (_WaveLength > 0.0) )
				{
					thisWave = (_WaveFront - realMagnitude) / _WaveLength;
					remainder = thisWave - floor( thisWave );
					remainder = 1.0 - remainder;
					if( (_Transforming > 0.0) && (thisWave >= _Transforming) )
					{
						color.rgb = originalColor.rgb;
						color.a = 1.0;
						if( floor( thisWave ) == floor( _Transforming ) )
						{
							color.rgb += _WaveColor.rgb * remainder;
						}
					}
					else
					{
						extraAlpha = ( 1.0 - _BaseAlpha ) * remainder;
						color.rgb += _WaveColor.rgb * extraAlpha;
						color.a += extraAlpha / 2.0;
					}
				}
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
