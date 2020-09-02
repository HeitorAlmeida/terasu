using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBodyAnimator : BaseAnimator
{
	public const int STATUS_IDLE = 0;
	public const int STATUS_IDLE_MOVING = 1;
	public const int STATUS_MOVING = 2;
	public const int STATUS_MOVING_IDLE = 3;
	public const int STATUS_IDLE_UP = 4;
	public const int STATUS_UP = 5;
	public const int STATUS_UP_IDLE = 6;
	public const int STATUS_IDLE_DOWN = 7;
	public const int STATUS_DOWN = 8;
	public const int STATUS_DOWN_IDLE = 9;

	//public const float DEFAULT_VERTICAL_ALIGNMENT_ANGLE = 65f;
	//public const float ANGLE_ATTENUATION = 0.7f;
	//public const float DEFAULT_LERP_FACTOR_MOVING = 3f;
	//public const float DEFAULT_LERP_FACTOR_IDLE = 2f;

	public GameObject sequenceIdleRight;
	public GameObject sequenceIdleMoving;
	public GameObject sequenceMoving;
	public GameObject sequenceMovingIdle;
	public GameObject sequenceIdleUp;
	public GameObject sequenceUp;
	public GameObject sequenceUpIdle;
	public GameObject sequenceIdleDown;
	public GameObject sequenceDown;
	public GameObject sequenceDownIdle;
	public float angleLerpFactorIdle;
	public float angleLerpFactorMoving;
	public float angleVerticalAlignStart;

	protected SpriteRenderer bodyRenderer;
	protected float targetBodyAngle;
	protected float currentBodyAngle;
	protected bool side;
	protected bool previousSide;
	protected bool movingFree;
	protected bool movingUp;
	protected bool movingDown;
	protected float lerpFactor;
	protected bool forceToTargetAngle;
	protected Material material;
	protected int lightColorID;
	protected int lightOffsetID;
	protected int lightRadiusID;
	protected int lightIntensityID;
	protected int normalMapID;
	protected int manualFlipID;

	protected override void Awake()
	{
		if( angleLerpFactorIdle <= 0f )
		{
			angleLerpFactorIdle = 1f;
		}
		if( angleLerpFactorMoving <= 0f )
		{
			angleLerpFactorMoving = 1f;
		}
		if( angleVerticalAlignStart < 0f )
		{
			angleVerticalAlignStart = 0f;
		}
		if( angleVerticalAlignStart > 90f )
		{
			angleVerticalAlignStart = 90f;
		}
		material = null;
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		targetBodyAngle = 0f;
		currentBodyAngle = 0f;
		side = SIDE_RIGHT;
		previousSide = SIDE_RIGHT;
		movingFree = false;
		movingUp = false;
		movingDown = false;
		lerpFactor = angleLerpFactorIdle;
		forceToTargetAngle = false;
		lightColorID = -1;
		lightOffsetID = -1;
		lightRadiusID = -1;
		lightIntensityID = -1;
		normalMapID = -1;
		manualFlipID = -1;
	}

	protected override void ClearRenderingComponent()
	{
		bodyRenderer = null;
		if( material != null )
		{
			Destroy( material );
			material = null;
		}
	}

	protected override bool ExtractRenderingComponent()
	{
		bodyRenderer = gameObject.GetComponent<SpriteRenderer>();
		if( bodyRenderer != null )
		{
			//Debug.Log("Debug : FlyingBodyAnimator : Materials before extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			material = bodyRenderer.material;
			//Debug.Log("Debug : FlyingBodyAnimator : Materials after extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			lightColorID = Shader.PropertyToID("_LightColor");
			lightOffsetID = Shader.PropertyToID("_LightOffset");
			lightRadiusID = Shader.PropertyToID("_LightRadius");
			lightIntensityID = Shader.PropertyToID("_LightIntensity");
			normalMapID = Shader.PropertyToID("_NormalTex");
			manualFlipID = Shader.PropertyToID("_ManualFlip");
			/*halmeida - let's make sure no light is currently being shown by the shader.*/
			RemoveFakeLight();
		}
		return (bodyRenderer != null);
	}

	public void SetFakeLight( Color lightColor, Vector2 lightOffset, float lightRadius, float lightIntensity )
	{
		Vector4 completeLightOffset = Vector4.zero;

		if( material != null )
		{
			//Debug.Log("Debug : FlyingBodyAnimator : received fake light with radius "+lightRadius+" and intensity "+lightIntensity+".");
			material.SetColor( lightColorID, lightColor );
			completeLightOffset.x = lightOffset.x;
			completeLightOffset.y = lightOffset.y;
			material.SetVector( lightOffsetID, completeLightOffset );
			material.SetFloat( lightRadiusID, lightRadius );
			material.SetFloat( lightIntensityID, lightIntensity ); 
		}
	}

	public void RemoveFakeLight()
	{
		if( material != null )
		{
			material.SetFloat( lightRadiusID, 0f );
		}
	}

	protected override void FeedRenderingComponent( Sprite newSprite, Sprite newSpriteNormal, bool newSpriteFlip )
	{
		Vector4 completeFlip = Vector4.one;

		if( bodyRenderer != null )
		{
			bodyRenderer.sprite = newSprite;
			bodyRenderer.enabled = (newSprite != null);
			bodyRenderer.flipX = newSpriteFlip;
			if( material != null )
			{
				if( newSpriteNormal != null )
				{
					material.SetTexture( normalMapID, newSpriteNormal.texture );
				}
				else
				{
					material.SetTexture( normalMapID, null );
				}
				completeFlip.x = newSpriteFlip ? -1f : 1f;
				material.SetVector( manualFlipID, completeFlip );
			}
		}
	}

	protected override Color ExtractRenderingColor()
	{
		if( bodyRenderer != null )
		{
			return bodyRenderer.color;
		}
		return base.ExtractRenderingColor();
	}

	protected override void FeedRenderingColor( Color newColor )
	{
		if( bodyRenderer != null )
		{
			bodyRenderer.color = newColor;
		}
	}

	protected override BaseAnimator ExtractChildAnimatorComponent( GameObject childObject )
	{
		if( childObject != null )
		{
			return childObject.GetComponent<FlyingBodyAnimator>();
		}
		return null;
	}

	public void SynchChildAnimator()
	{
		/*halmeida - the HeadAnimator child of a HeadAnimator is supposed to be equipment. The
		frames of the equipment have to be in perfect synch with the character's. To ensure that, we must
		force them into the same animation state.*/
		/*
		SetRightDirection( pressingRight );
		SetLeftDirection( pressingLeft );
		SetForcingUp( forcingUp );
		SetBreaking( breaking );
		ForceFadeTo( fading, faded );
		ForcePreviouslyGroundedTo( groundedDelayed );
		ForceStatusTo( status, previousStatus );
		ForceSequenceTo( sequenceIndex, firstIndex, lastIndex, speed, loop );
		ForceSpriteTo( spriteIndex, spriteFrame, focusOffsetOne, focusOffsetTwo );
		Progress();
		*/
	}

	public override void SetSide( bool newSide )
	{
		if( (side != newSide) && ( (status == STATUS_IDLE_UP) || (status == STATUS_IDLE_DOWN) ) )
		{
			keepSequenceProgress = true;
		}
		side = newSide;
		base.SetSide( newSide );
	}

	public override bool GetSide()
	{
		return side;
	}

	public override void SetSpeedAngleWithRight( bool isPushing, float newSpeedAngle, bool under90Transformed )
	{
		bool newSide = SIDE_RIGHT;

		if( !isPushing )
		{
			movingFree = false;
			movingUp = false;
			movingDown = false;
			/*halmeida - when the body is not moving to any direction its side should be
			preserved and it should try to be vertically aligned.*/
			targetBodyAngle = 0f;
			lerpFactor = angleLerpFactorIdle;
		}
		else
		{
			/*halmeida - when transforming an angle from under 180 to under 90, the side is
			obtained. So, this method can be called by someone who can provide only a brute
			under 180 angle, leaving the side determination to be done here, and can also
			be called by someone who already did the transformation and knows both the
			propper side and the under 90 angle, setting side and angle in individual separate
			calls.*/
			if( !under90Transformed )
			{
				if( newSpeedAngle > 90f )
				{
					newSpeedAngle = -(180f - newSpeedAngle);
					newSide = SIDE_LEFT;
				}
				else if( newSpeedAngle < -90f )
				{
					newSpeedAngle = 180f + newSpeedAngle;
					newSide = SIDE_LEFT;
				}
				else
				{
					newSide = SIDE_RIGHT;
				}
				SetSide( newSide );
			}
			/*halmeida - at this point the side is correctly set and the angle is within
			the interval [90, -90[.*/
			if( newSpeedAngle > angleVerticalAlignStart )
			{
				if( side )
				{
					movingUp = true;
					movingDown = false;
				}
				else
				{
					movingUp = false;
					movingDown = true;
				}
				movingFree = false;
				targetBodyAngle = 0f;
			}
			else if( newSpeedAngle < -angleVerticalAlignStart )
			{
				if( side )
				{
					movingDown = true;
					movingUp = false;
				}
				else
				{
					movingDown = false;
					movingUp = true;
				}
				movingFree = false;
				targetBodyAngle = 0f;
			}
			else
			{
				movingFree = true;
				movingUp = false;
				movingDown = false;
				targetBodyAngle = GetTargetAngleFromSpeedAngle( newSpeedAngle );
			}
			lerpFactor = angleLerpFactorMoving;
		}
		base.SetSpeedAngleWithRight( isPushing, newSpeedAngle, true );
	}

	protected virtual float GetTargetAngleFromSpeedAngle( float under90SpeedAngle )
	{
		return under90SpeedAngle;
	}

	public void RequestTargetAngleReach()
	{
		forceToTargetAngle = true;
	}

	protected override void UpdateStatus()
	{
		if( (status == INVALID_STATUS) && !over )
		{
			status = STATUS_IDLE;
		}
		if( movingFree )
		{
			if( (status != STATUS_IDLE_MOVING) && (status != STATUS_MOVING) )
			{
				status = STATUS_IDLE_MOVING;
			}
		}
		else if( movingUp )
		{
			if( (status != STATUS_IDLE_UP) && (status != STATUS_UP) )
			{
				status = STATUS_IDLE_UP;
			}
		}
		else if( movingDown )
		{
			if( (status != STATUS_IDLE_DOWN) && (status != STATUS_DOWN) )
			{
				status = STATUS_IDLE_DOWN;
			}
		}
		else
		{
			if( (status == STATUS_IDLE_MOVING) || (status == STATUS_MOVING) )
			{
				status = STATUS_MOVING_IDLE;
			}
			else if( (status == STATUS_IDLE_UP) || (status == STATUS_UP) )
			{
				status = STATUS_UP_IDLE;
			}
			else if( (status == STATUS_IDLE_DOWN) || (status == STATUS_DOWN) )
			{
				status = STATUS_DOWN_IDLE;
			}
		}
		//Debug.Log("Debug : KamiBodyAnimator : status became "+status+".");
	}

	protected override void UpdateTransform( float timeStep )
	{
		float lerpAmount = 0f;

		if( previousSide != side )
		{
			/*halmeida - get mirror angle for the current angle, so that the side change
			occurs more coherently.*/
			currentBodyAngle = -currentBodyAngle;
			targetBodyAngle = -targetBodyAngle;
		}
		if( forceToTargetAngle )
		{
			currentBodyAngle = targetBodyAngle;
			forceToTargetAngle = false;
		}
		else
		{
			lerpAmount = lerpFactor * timeStep;
			currentBodyAngle = Mathf.Lerp( currentBodyAngle, targetBodyAngle, lerpAmount );
		}
		transform.rotation = Quaternion.Euler( 0f, 0f, currentBodyAngle );
	}

	protected override void AdvanceToNextStatus()
	{
		switch( status )
		{
			case STATUS_IDLE:
				status = INVALID_STATUS;
				break;
			case STATUS_IDLE_MOVING:
				status = STATUS_MOVING;
				break;
			case STATUS_MOVING:
				status = INVALID_STATUS;
				break;
			case STATUS_MOVING_IDLE:
				status = STATUS_IDLE;
				break;
			case STATUS_IDLE_UP:
				status = STATUS_UP;
				break;
			case STATUS_UP:
				status = INVALID_STATUS;
				break;
			case STATUS_UP_IDLE:
				status = STATUS_IDLE;
				break;
			case STATUS_IDLE_DOWN:
				status = STATUS_DOWN;
				break;
			case STATUS_DOWN:
				status = INVALID_STATUS;
				break;
			case STATUS_DOWN_IDLE:
				status = STATUS_IDLE;
				break;
			default:
				status = INVALID_STATUS;
				break;
		}
	}

	protected override bool RequiresNewSequence()
	{ 
		return ( (previousStatus != status) || (previousSide != side) );
	}

	protected override void RecordChangeVerifiers()
	{
		base.RecordChangeVerifiers();
		previousSide = side;
	}

	protected override void FillSequencesArray()
	{
		totalSequences = 10;
		sequenceObjects = new GameObject[totalSequences];
		for( int i=0; i<totalSequences; i++ )
		{
			switch( i )
			{
				case 0:
					sequenceObjects[0] = sequenceIdleRight;
					break;
				case 1:
					sequenceObjects[1] = sequenceIdleMoving;
					break;
				case 2:
					sequenceObjects[2] = sequenceMoving;
					break;
				case 3:
					sequenceObjects[3] = sequenceMovingIdle;
					break;
				case 4:
					sequenceObjects[4] = sequenceIdleUp;
					break;
				case 5:
					sequenceObjects[5] = sequenceUp;
					break;
				case 6:
					sequenceObjects[6] = sequenceUpIdle;
					break;
				case 7:
					sequenceObjects[7] = sequenceIdleDown;
					break;
				case 8:
					sequenceObjects[8] = sequenceDown;
					break;
				case 9:
					sequenceObjects[9] = sequenceDownIdle;
					break;
			}
		}
	}

	protected override void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		newSpriteFlip = !side;
		switch( statusValue )
		{
			case STATUS_IDLE:
				newSequenceIndex = 0;
				break;
			case STATUS_IDLE_MOVING:
				newSequenceIndex = 1;
				break;
			case STATUS_MOVING:
				newSequenceIndex = 2;
				break;
			case STATUS_MOVING_IDLE:
				newSequenceIndex = 3;
				break;
			case STATUS_IDLE_UP:
				newSequenceIndex = 4;
				break;
			case STATUS_UP:
				newSequenceIndex = 5;
				break;
			case STATUS_UP_IDLE:
				newSequenceIndex = 6;
				break;
			case STATUS_IDLE_DOWN:
				newSequenceIndex = 7;
				break;
			case STATUS_DOWN:
				newSequenceIndex = 8;
				break;
			case STATUS_DOWN_IDLE:
				newSequenceIndex = 9;
				break;
			default:
				newSequenceIndex = -1;
				break;
		}
	}
}
