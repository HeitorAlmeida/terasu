Shader "Custom/SpriteColorSum"
{
	//halmeida - the understanding of this shader is very problematic. I tried to build it
	//very simple by subtracting parts of the Sprites/Default shader that I found unnecessary.
	//The thing is, anything I removed from it made it stop working. I couldn't even remove
	//the structures and turn them into vert and frag parameters, because within the structures
	//there are properties declared through macros and these macros will not work as function
	//parameters. Even the instancing parts that seem to be so optional are necessary for it
	//to work. In the end I had to practically copy the entire original shader code.
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0.0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1.0, 1.0, 1.0, 1.0)
        [HideInInspector] _Flip ("Flip", Vector) = (1.0, 1.0, 1.0, 1.0)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _ColorToAdd ("Color To Add", Vector) = (0.0, 0.0, 0.0, 0.0)
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

			#ifdef UNITY_INSTANCING_ENABLED
				UNITY_INSTANCING_CBUFFER_START(PerDrawSprite)
				// SpriteRenderer.Color while Non-Batched/Instanced.
				fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
				// this could be smaller but that's how bit each entry is regardless of type
				float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
				UNITY_INSTANCING_CBUFFER_END

				#define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
				#define _Flip unity_SpriteFlipArray[unity_InstanceID]

			#endif // instancing

			CBUFFER_START(UnityPerDrawSprite)
			#ifndef UNITY_INSTANCING_ENABLED
				fixed4 _RendererColor;
				float4 _Flip;
			#endif
			float _EnableExternalAlpha;
			CBUFFER_END

			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _ColorToAdd;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				//halmeida - start of original vertex shader instructions.
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				#ifdef UNITY_INSTANCING_ENABLED
					IN.vertex.xy *= _Flip.xy;
				#endif

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _RendererColor;

				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				//halmeida - end of original vertex shader instructions.

				return OUT;
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
					fixed4 alpha = tex2D(_AlphaTex, uv);
					color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
				#endif

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				//halmeida - start of original fragment shader instructions.
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				//c.rgb *= c.a;
				//halmeida - end of original fragment shader instructions.

				//halmeida - the original shader is so fucked up that the final color rgb components
				//have to be multiplied by the alpha component of the color to be displayed correctly.
				//In any other correctly applied shader, multiplying the final components by the alpha
				//component would make semitransparent pixels darker. If you think of a black background
				//for the object, it is ok to darken the pixels for an accelerated fade out, but on a
				//white background this would create problems. This unorthodox procedure is, I believe,
				//a consequence of how the SpriteRenderer component deals with the shader.

				//halmeida - regardless of the cause of this mess, I have to deal with it, and that means
				//I need to, after finding the final color for the pixel, multiply its components by the
				//alpha component of that color.

				c += _ColorToAdd;

				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
