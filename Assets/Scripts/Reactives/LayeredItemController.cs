using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredItemController : ItemController
{
	public float[] opaqueAlphasFirstUpgrade;
	public float[] opaqueAlphasSecondUpgrade;
	public float alphaFadeSpeed;

	private SpinningLayersAnimator layersAnimator;
	private float fadeSpeed;

	protected override void Awake()
	{
		layersAnimator = null;
		fadeSpeed = (alphaFadeSpeed > 0f) ? alphaFadeSpeed : -alphaFadeSpeed;
		base.Awake();
	}

	protected override void ExtractItemAnimator()
	{
		layersAnimator = GetComponent<SpinningLayersAnimator>();
	}

	protected override void ActivateItemAnimator()
	{
		if( layersAnimator != null )
		{
			layersAnimator.SetActive( true );
		}
	}

	protected override void DeactivateItemAnimator()
	{
		if( layersAnimator != null )
		{
			layersAnimator.SetActive( false );
		}
	}

	protected override void ClearItemAnimator()
	{
		if( layersAnimator != null )
		{
			layersAnimator.Clear();
			layersAnimator = null;
		}
	}

	public override bool EvolveAppearance( float changeFactor )
	{
		VFXParticleController particleController = null;
		float valueOne = 0f;
		float valueTwo = 0f;

		if( base.EvolveAppearance( changeFactor ) && (layersAnimator != null) )
		{
			switch( appearanceLevel )
			{
				case APPEARANCE_LEVEL_SECOND:
					if( opaqueAlphasFirstUpgrade != null )
					{
						for( int i=0; i<opaqueAlphasFirstUpgrade.Length; i++ )
						{
							layersAnimator.SetLayerOpaqueAlpha( i, opaqueAlphasFirstUpgrade[i] );
							layersAnimator.StartLayerAlphaFading( i, fadeSpeed, false );
						}
					}
					break;
				case APPEARANCE_LEVEL_LAST:
					if( opaqueAlphasSecondUpgrade != null )
					{
						for( int i=0; i<opaqueAlphasSecondUpgrade.Length; i++ )
						{
							layersAnimator.SetLayerOpaqueAlpha( i, opaqueAlphasSecondUpgrade[i] );
							layersAnimator.StartLayerAlphaFading( i, fadeSpeed, false );
						}
					}
					particleController = layersAnimator.GetParticleController();
					if( (particleController != null) && (changeFactor > 1f) )
					{
						valueOne = particleController.GetWindowDuration();
						valueOne *= (2f - changeFactor);
						particleController.SetWindowDuration( valueOne );
						valueOne = particleController.GetFocusMaximumRadius() * changeFactor;
						valueTwo = particleController.GetFocusMinimumRadius() * changeFactor;
						particleController.SetFocusOffsetRadiusRange( valueOne, valueTwo );
					}
					break;
			}
			return true;
		}
		return false;
	}

	protected override void ProgressItemAnimator( float timeStep )
	{
		if( layersAnimator != null )
		{
			layersAnimator.Progress( timeStep );
		}
	}

	public override void CorrectDepthChain()
	{
		if( layersAnimator != null )
		{
			layersAnimator.StartDepthChain( gameObject.transform.position.z, true );
		}
	}
}
