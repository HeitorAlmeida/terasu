using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBirdAnimator : FlyingBodyAnimator
{
	public float angleRateSpeedToBody;
	public float angleBonusSpeedToBody;

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
		if( side )
		{
			return (angleRateSpeedToBody * under90SpeedAngle) + angleBonusSpeedToBody;
		}
		else
		{
			return (angleRateSpeedToBody * under90SpeedAngle) - angleBonusSpeedToBody;
		}
	}
}
