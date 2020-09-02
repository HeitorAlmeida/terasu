using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingHumanAnimator : FlyingBodyAnimator
{
	public float angleRateSpeedToBody;

	protected override void Awake()
	{
		angleRateSpeedToBody *= ((angleRateSpeedToBody < 0f) ? -1f : 1f);
		if( angleRateSpeedToBody > 1f )
		{
			angleRateSpeedToBody = 1f;
		}
		base.Awake();
	}

	protected override float GetTargetAngleFromSpeedAngle( float under90SpeedAngle )
	{
		return angleRateSpeedToBody * under90SpeedAngle;
	}
}
