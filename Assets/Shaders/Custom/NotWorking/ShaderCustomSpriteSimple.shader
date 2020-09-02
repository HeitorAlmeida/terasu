Shader "Custom/SpriteSimple"
{
	//halmeida - this shader does not work properly. It should, cause it is the simplest
	//texturization shader, but it still doesn't. My guess is the SpriteRenderer component
	//uses the material in a weird way that requires the shader to have all those macros
	//and appendices of the Sprite/Default shader.
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;

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
			}
			ENDCG
		}
	}
}
