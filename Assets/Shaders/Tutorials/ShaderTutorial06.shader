// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Tutorial/WindowCoordinates/CirclesMask"
{
	Properties
	{
		mCirclesX("Circles in X", Float) = 20.0
		mCirclesY("Circles in Y", Float) = 10.0
		mFade("Fade", Range( 0.1, 1.0 )) = 0.5
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			uniform float mCirclesX;
			uniform float mCirclesY;
			uniform float mFade;
			
			float4 vert( appdata_base v ) : POSITION
			{
				return UnityObjectToClipPos( v.vertex );
			}
			
			float4 frag( float4 sp : WPOS ) : COLOR
			{
				float2 wcoord = sp.xy / _ScreenParams.xy;
				float4 color;
				
				if( length( fmod( float2( mCirclesX*wcoord.x, mCirclesY*wcoord.y ), 2.0 ) - 1.0 ) < mFade )
				{
					color = float4( sp.xy / _ScreenParams.xy, 0.0, 1.0 );
				}
				else
				{
					color = float4( 0.3, 0.3, 0.3, 1.0 );
				}
				return color;
			}
			
			ENDCG
		}
	}
}
