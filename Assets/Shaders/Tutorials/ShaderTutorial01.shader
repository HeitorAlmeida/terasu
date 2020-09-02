Shader "Tutorial/Basic"
{
	Properties
	{
		mColor ("Main Color", Color) = (1.0, 0.5, 0.5, 1.0)
	}
	SubShader
	{
		Pass
		{
			Material
			{
				Diffuse [mColor]
			}
			Lighting On
		}
	}
}
