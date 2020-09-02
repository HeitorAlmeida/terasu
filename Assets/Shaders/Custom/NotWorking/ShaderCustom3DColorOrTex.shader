Shader "Custom/ColorOrTexture"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Fill Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;

			void vert( float4 position : POSITION,
				float2 texCoord : TEXCOORD0,
				out float4 oPosition : POSITION,
				out float2 oTexCoord : TEXCOORD0 )
			{
				oPosition = UnityObjectToClipPos(position);
				oTexCoord = texCoord;
			}

			void frag( float2 texCoord : TEXCOORD0,
				out float4 oColor : COLOR )
			{
				oColor = tex2D(_MainTex, texCoord);
				if( oColor.a == 0.0 )
				{
					oColor = _Color;
				}
			}
			ENDCG
		}
	}
}
