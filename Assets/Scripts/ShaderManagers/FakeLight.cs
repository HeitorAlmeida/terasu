
using UnityEngine;

public class FakeLight
{
	public Color color;

	private float radius;
	private float radiusSpeed;
	private float intensity;
	private float intensitySpeed;
	private bool stretchesValid;
	private int stretchIndex;
	private float[] stretchRadiusSpeeds;
	private float[] stretchIntensitySpeeds;
	private float[] stretchDurations;
	private float stretchDuration;
	private float stretchElapsed;

	public FakeLight( Color lightColor, float startingRadius, float startingIntensity )
	{
		color = lightColor;
		radius = (startingRadius > 0f) ? startingRadius : 0f;
		radiusSpeed = 0f;
		intensity = (startingIntensity > 0f) ? startingIntensity : 0f;
		intensitySpeed = 0f;
		stretchesValid = false;
		stretchIndex = -1;
		stretchRadiusSpeeds = null;
		stretchIntensitySpeeds = null;
		stretchDurations = null;
		stretchDuration = 0f;
		stretchElapsed = 0f;
	}

	public bool ConfigureStretches( float[] radiusSpeedPerStretch, float[] intensitySpeedPerStretch, float[] durationPerStretch )
	{
		if( (radiusSpeedPerStretch != null) && (intensitySpeedPerStretch != null) && (durationPerStretch != null) )
		{
			stretchesValid = (radiusSpeedPerStretch.Length == intensitySpeedPerStretch.Length);
			if( stretchesValid )
			{
				stretchesValid = (intensitySpeedPerStretch.Length == durationPerStretch.Length);
			}
			if( stretchesValid )
			{
				stretchRadiusSpeeds = (float[]) radiusSpeedPerStretch.Clone();
				stretchIntensitySpeeds = (float[]) intensitySpeedPerStretch.Clone();
				stretchDurations = (float[]) durationPerStretch.Clone();
				SetCurrentStretch( 0 );
				return true;
			}
		}
		return false;
	}

	private void SetCurrentStretch( int newStretchIndex )
	{
		bool indexValid = false;

		if( stretchesValid )
		{
			if( (newStretchIndex > -1) && (newStretchIndex < stretchDurations.Length) )
			{
				stretchIndex = newStretchIndex;
				radiusSpeed = stretchRadiusSpeeds[stretchIndex];
				intensitySpeed = stretchIntensitySpeeds[stretchIndex];
				stretchDuration = stretchDurations[stretchIndex];
				stretchElapsed = 0f;
				indexValid = true;
			}
		}
		if( !indexValid )
		{
			stretchIndex = -1;
			radiusSpeed = 0f;
			intensitySpeed = 0f;
			stretchDuration = 0f;
			stretchElapsed = 0f;
			radius = 0f;
		}
	}

	public void Progress( float timeStep )
	{
		if( stretchIndex > -1 )
		{
			radius += radiusSpeed * timeStep;
			intensity += intensitySpeed * timeStep;
			stretchElapsed += timeStep;
			if( stretchElapsed > stretchDuration )
			{
				SetCurrentStretch( stretchIndex + 1 );
			}
		}
	}

	public float GetRadius()
	{
		return radius;
	}

	public float GetIntensity()
	{
		return intensity;
	}

	public void Clear()
	{
		stretchIndex = -1;
		stretchRadiusSpeeds = null;
		stretchIntensitySpeeds = null;
		stretchDurations = null;
	}
}
