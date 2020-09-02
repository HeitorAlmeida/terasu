using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamiAnimator : BaseAnimator
{
	public const int STATUS_MANAGING = 0;

	public GameObject bodyObject;
	public GameObject shadowConfigPeriodic;
	public GameObject shadowConfigFullHeal;
	public GameObject[] shadowConfigsLevelUp;

	private bool side;
	private float speedAngle;
	private FlyingBodyAnimator bodyAnimator;
	private GameObject shadowPeriodicObject;
	private GameObject shadowFullHealObject;
	private GameObject[] shadowsLevelUpObjects;
	private ShadowConfiguration shadowPeriodic;
	private ShadowConfiguration shadowFullHeal;
	private ShadowConfiguration[] shadowsLevelUp;
	private float shadowCreationElapsed;

	protected override void Awake()
	{
		GameObject shadowConfigModel = null;
		GameObject shadowConfigObject = null;
		ShadowConfiguration shadowConfig = null;
		int shadowConfigsLength = 0;

		bodyAnimator = null;
		shadowPeriodicObject = null;
		shadowFullHealObject = null;
		shadowsLevelUpObjects = null;
		shadowPeriodic = null;
		shadowFullHeal = null;
		shadowsLevelUp = null;
		base.Awake();
		if( shadowConfigPeriodic != null )
		{
			shadowPeriodicObject = Instantiate( shadowConfigPeriodic ) as GameObject;
			if( shadowPeriodicObject != null )
			{
				shadowPeriodic = shadowPeriodicObject.GetComponent<ShadowConfiguration>();
			}
		}
		if( shadowConfigFullHeal != null )
		{
			shadowFullHealObject = Instantiate( shadowConfigFullHeal ) as GameObject;
			if( shadowFullHealObject != null )
			{
				shadowFullHeal = shadowFullHealObject.GetComponent<ShadowConfiguration>();
			}
		}
		if( shadowConfigsLevelUp != null )
		{
			shadowConfigsLength = shadowConfigsLevelUp.Length;
			if( shadowConfigsLength > 0 )
			{
				shadowsLevelUpObjects = new GameObject[shadowConfigsLength];
				shadowsLevelUp = new ShadowConfiguration[shadowConfigsLength];
				for( int i=0; i<shadowConfigsLength; i++ )
				{
					shadowConfigModel = shadowConfigsLevelUp[i];
					shadowConfigObject = null;
					shadowConfig = null;
					if( shadowConfigModel != null )
					{
						shadowConfigObject = Instantiate( shadowConfigModel ) as GameObject;
						if( shadowConfigObject != null )
						{
							shadowConfig = shadowConfigObject.GetComponent<ShadowConfiguration>();
						}
					}
					shadowsLevelUpObjects[i] = shadowConfigObject;
					shadowsLevelUp[i] = shadowConfig;
				}
			}
		}
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		side = SIDE_RIGHT;
		speedAngle = 0f;
		ClearShadowConfigurations();
		shadowCreationElapsed = 0f;
	}

	protected void ClearShadowConfigurations()
	{
		GameObject shadowObject = null;

		shadowPeriodic = null;
		shadowFullHeal = null;
		if( shadowsLevelUp != null )
		{
			for( int i=0; i<shadowsLevelUp.Length; i++ )
			{
				shadowsLevelUp[i] = null;
			}
			shadowsLevelUp = null;
		}
		if( shadowPeriodicObject != null )
		{
			Destroy( shadowPeriodicObject );
			shadowPeriodicObject = null;
		}
		if( shadowFullHealObject != null )
		{
			Destroy( shadowFullHealObject );
			shadowFullHealObject = null;
		}
		if( shadowsLevelUpObjects != null )
		{
			for( int i=0; i<shadowsLevelUpObjects.Length; i++ )
			{
				shadowObject = shadowsLevelUpObjects[i];
				if( shadowObject != null )
				{
					Destroy( shadowObject );
					shadowsLevelUpObjects[i] = null;
				}
			}
			shadowsLevelUpObjects = null;
		}
	}

	protected override void RecognizeBackChildAnimator()
	{
		if( bodyObject != null )
		{
			bodyAnimator = bodyObject.GetComponent<FlyingBodyAnimator>();
		}
		base.RecognizeBackChildAnimator();
	}

	protected override BaseAnimator ExtractChildAnimatorComponent( GameObject childObject )
	{
		if( childObject != null )
		{
			return childObject.GetComponent<KamiAnimator>();
		}
		return null;
	}

	public void SynchChildAnimator()
	{
		/*halmeida - the CharacterAnimator child of a CharacterAnimator is supposed to be equipment. The
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

	protected override void ClearChildAnimators()
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.Clear();
			bodyAnimator = null;
		}
		base.ClearChildAnimators();
	}

	protected override void ProgressChildAnimators( float timeStep )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.Progress( timeStep );
		}
		base.ProgressChildAnimators( timeStep );
	}

	protected override void ProgressShadows( float timeStep )
	{
		base.ProgressShadows( timeStep );
		if( !paused )
		{
			if( shadowPeriodic != null )
			{
				if( shadowPeriodic.creationInterval > 0f )
				{
					shadowCreationElapsed += timeStep;
					if( shadowCreationElapsed > shadowPeriodic.creationInterval )
					{
						CreatePeriodicShadow();
						shadowCreationElapsed = 0f;
					}
				}
			}
		}
	}

	public void InterpretMovementDirection( Vector2 normalizedDirection )
	{
		float angleWithRight = 0f;
		bool newSide = SIDE_RIGHT;

		if( normalizedDirection != Vector2.zero )
		{
			angleWithRight = UsefulFunctions.GetDirectionAngle( normalizedDirection );
			if( angleWithRight > 90f )
			{
				angleWithRight = -(180f - angleWithRight);
				newSide = SIDE_LEFT;
			}
			else if( angleWithRight < -90f )
			{
				angleWithRight = 180f + angleWithRight;
				newSide = SIDE_LEFT;
			}
			else
			{
				newSide = SIDE_RIGHT;
			}
			SetSide( newSide );
			SetSpeedAngleBody( true, angleWithRight );
		}
		else
		{
			SetSide( side );
			SetSpeedAngleBody( false, 0f );
		}
	}

	public void FaceDirection( Vector2 normalizedDirection )
	{
		float angleWithRight = 0f;
		bool newSide = SIDE_RIGHT;

		if( normalizedDirection != Vector2.zero )
		{
			angleWithRight = UsefulFunctions.GetDirectionAngle( normalizedDirection );
			if( (angleWithRight > 90f) || (angleWithRight < -90f) )
			{
				newSide = SIDE_LEFT;
			}
			else
			{
				newSide = SIDE_RIGHT;
			}
			SetSide( newSide );
		}
	}

	public override void SetSide( bool newSide )
	{
		side = newSide;
		if( bodyAnimator != null )
		{
			bodyAnimator.SetSide( side );
		}
		base.SetSide( newSide );
	}

	public override bool GetSide()
	{
		return side;
	}

	public override void SetSpeedAngleBody( bool pushingDirection, float angleWithRight )
	{
		speedAngle = angleWithRight;
		if( bodyAnimator != null )
		{
			bodyAnimator.SetSpeedAngleWithRight( pushingDirection, speedAngle, true );
		}
		base.SetSpeedAngleBody( pushingDirection, angleWithRight );
	}

	public override void Stabilize()
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.RequestTargetAngleReach();
		}
		base.Stabilize();
	}

	public override void TogglePause( bool pause )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.TogglePause( pause );
		}
		base.TogglePause( pause );
	}

	public override int GetPausedShadowsAmount()
	{
		if( bodyAnimator != null )
		{
			return bodyAnimator.GetPausedShadowsAmount();
		}
		return 0;
	}

	protected override void UpdateStatus()
	{
		if( (status == INVALID_STATUS) && !over )
		{
			if( bodyAnimator != null )
			{
				status = STATUS_MANAGING;
			}
		}
	}

	protected override bool RequiresNewSequence()
	{
		return false;
	}

	public override void StartAlphaFading( float newFadeSpeed, bool immediately, bool affectChildBehind, bool affectChildAhead )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.StartAlphaFading( newFadeSpeed, immediately, affectChildBehind, affectChildAhead );
		}
		base.StartAlphaFading( newFadeSpeed, immediately, affectChildBehind, affectChildAhead );
	}

	public override bool IsOpaque()
	{
		if( bodyAnimator != null )
		{
			return bodyAnimator.IsOpaque();
		}
		return base.IsOpaque();
	}

	public override bool IsTransparent()
	{
		if( bodyAnimator != null )
		{
			return bodyAnimator.IsTransparent();
		}
		return base.IsTransparent();
	}

	public void SetFakeLight( Color lightColor, Vector2 lightOffset, float lightRadius, float lightIntensity )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.SetFakeLight( lightColor, lightOffset, lightRadius, lightIntensity );
		}
	}

	public void RemoveFakeLight()
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.RemoveFakeLight();
		}
	}

	public override void StartDepthChain( float depth, bool inwards )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.StartDepthChain( depth, inwards );
		}
		base.StartDepthChain( depth, inwards );
	}

	public override void CreateShadow( ShadowConfiguration shadowConfig )
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.CreateShadow( shadowConfig );
		}
	}

	public override void ClearShadows()
	{
		if( bodyAnimator != null )
		{
			bodyAnimator.ClearShadows();
		}
	}

	public void ReconfigurePeriodicShadow( ShadowConfiguration newShadowConfiguration )
	{
		if( shadowPeriodic != null )
		{
			shadowPeriodic.Copy( newShadowConfiguration );
		}
	} 

	private void CreatePeriodicShadow()
	{
		CreateShadow( shadowPeriodic );
	}

	public void CreateFullHealShadow()
	{
		CreateShadow( shadowFullHeal );
	}

	public void CreateLevelUpShadows()
	{
		if( shadowsLevelUp != null )
		{
			for( int i=0; i<shadowsLevelUp.Length; i++ )
			{
				CreateShadow( shadowsLevelUp[i] );
			}
		}
	}
}
