﻿Shader "Custom/SpriteWave"
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
		_Color ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		[MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0.0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1.0, 1.0, 1.0, 1.0)
        [HideInInspector] _Flip ("Flip", Vector) = (1.0, 1.0, 1.0, 1.0)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _WaveColor ("Wave Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WaveSource ("Wave Source", Vector) = (0.0, 0.0, 0.0, 0.0)
        _WaveLengths ("Wave Lengths", Vector) = (0.0, 0.0, 0.0, 0.0)
        _WaveAlphas ("Wave Alphas", Vector) = (0.0, 0.0, 0.0, 0.0)
        _WaveScalars ("Wave Scalars", Vector) = (1.0, 1.0, 1.0, 1.0)
        //halmeida - the scalars are "waveFront", "solidifyWave", and "fadeWithWave". The last value is unused.
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
			#pragma target 3.0
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
			uniform fixed4 _Color;
			uniform fixed4 _WaveColor;
			uniform float4 _WaveSource;
			uniform float4 _WaveLengths;
			uniform fixed4 _WaveAlphas;
			uniform float4 _WaveScalars;

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
				float2 objectSpaceVertex : TEXCOORD1;
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
				OUT.color = IN.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				//halmeida - end of original vertex shader instructions.

				OUT.objectSpaceVertex = IN.vertex;

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

				float2 vertexFromSource = IN.objectSpaceVertex - _WaveSource.xy;
				float distFromSource = length(vertexFromSource);
				float subWaveFront = _WaveScalars.x;
				float solidifyWave = _WaveScalars.y;
				float fadeWithWave = _WaveScalars.z;
				//halmeida - to discover if this pixel is beyond a certain distance, I could divide it by the distance
				//and see if the result is bigger than 1.0. However, this distance, which is the wave front, might be
				//0.0. To avoid dividing by zero, I came up with another way to check. The "if" command must be avoided
				//in shader code cause it breaks the pipelining.
				float beyondWaveFront = min(1.0, ceil(max(0.0, (distFromSource - subWaveFront))));
				subWaveFront -= _WaveLengths.x;
				float beyondWaveBack = min(1.0, ceil(max(0.0, (distFromSource - subWaveFront))));
				subWaveFront -= _WaveLengths.y;
				float beyondSecondBack = min(1.0, ceil(max(0.0, (distFromSource - subWaveFront))));
				subWaveFront -= _WaveLengths.z;
				float beyondThirdBack = min(1.0, ceil(max(0.0, (distFromSource - subWaveFront))));
				subWaveFront -= _WaveLengths.w;
				float beyondFourthBack = min(1.0, ceil(max(0.0, (distFromSource - subWaveFront))));
				//halmeida - if this pixel is beyond the wave front, it should not be considered by any of the waves.
				beyondWaveBack -= beyondWaveFront;
				beyondSecondBack -= beyondWaveFront;
				beyondThirdBack -= beyondWaveFront;
				beyondFourthBack -= beyondWaveFront;
				//halmeida - if beyond the wave back, it should not be considered by the other waves.
				beyondSecondBack -= beyondWaveBack;
				beyondThirdBack -= beyondWaveBack;
				beyondFourthBack -= beyondWaveBack;
				//halmeida - if beyond the second back, it should not be considered by the third and fourth.
				beyondThirdBack -= beyondSecondBack;
				beyondFourthBack -= beyondSecondBack;
				//halmeida - if beyond the third back, it should not be considered by the fourth.
				beyondFourthBack -= beyondThirdBack;
				float waveAlpha = (beyondFourthBack * _WaveAlphas.w) + (beyondThirdBack * _WaveAlphas.z);
				waveAlpha += (beyondSecondBack * _WaveAlphas.y) + (beyondWaveBack * _WaveAlphas.x);
				c.rgb += waveAlpha * _WaveColor.rgb;

				//halmeida - bring the fadeWithWave to the range of [0.0,1.0]. This is how much I will ignore the original
				//alpha value of the pixel and impose the alpha value of the wave itself.
				fadeWithWave = max(0.0, min(1.0, fadeWithWave));
				c.a = max( (1-fadeWithWave) * c.a, fadeWithWave * min(c.a, (beyondWaveFront * c.a) + ((1-beyondWaveFront) * waveAlpha)) );

				//halmeida - bring the solidifyWave to the range of [0.0,1.0]. This is how much I will ignore the original
				//alpha value of the pixel and impose the opaque alpha value.
				solidifyWave = max(0.0, min(1.0, solidifyWave));
				c.a = ceil(c.a) * max(c.a, solidifyWave * waveAlpha);

				c.rgb *= c.a;

				return c;
			}
			ENDCG
		}
	}
}
