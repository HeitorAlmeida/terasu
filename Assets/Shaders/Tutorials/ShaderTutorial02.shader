Shader "Tutorial/VertexLit"
{
	Properties
	{
		mColor("Main Color", Color) = (1, 1, 1, 0.5)
		mSpecColor("Spec Color", Color) = (1, 1, 1, 1)
		mEmission("Emissive Color", Color) = (0, 0, 0, 0)
		mShininess("Shininess", Range(0.01, 1)) = 0.7
		mMainTex("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Pass
		{
			Material
			{
				Diffuse [mColor]
				Ambient [mColor]
				Shininess [mShininess]
				Specular [mSpecColor]
				Emission [mEmission]
			}
			Lighting On
			SeparateSpecular On
			SetTexture [mMainTex]
			{
				constantColor [mColor]
				Combine texture * primary DOUBLE, texture * constant
			}
		}
	}
}
