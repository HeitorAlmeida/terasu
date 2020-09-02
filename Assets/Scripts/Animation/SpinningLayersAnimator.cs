using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLayersAnimator : TwoStateAnimator
{
	public GameObject[] layersParentToChild;
	public float[] layerAngleSpeeds;

	private Transform[] layerTransforms;
	private float[] layerSpeeds;
	private TwoStateAnimator[] layerAnimators;

	protected override void Awake()
	{
		layerTransforms = null;
		layerSpeeds = null;
		layerAnimators = null;
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
	}

	protected override void RecognizeBackChildAnimator()
	{
		int totalLayers = 0;
		GameObject layerObject = null;
		Transform layerTransform = null;
		float layerSpeed = 0f;
		TwoStateAnimator layerAnimator = null;

		if( layersParentToChild != null )
		{
			totalLayers = layersParentToChild.Length;
			layerTransforms = new Transform[totalLayers];
			layerSpeeds = new float[totalLayers];
			layerAnimators = new TwoStateAnimator[totalLayers];
			for( int i=0; i<totalLayers; i++ )
			{
				layerObject = layersParentToChild[i];
				layerTransform = null;
				layerSpeed = 0f;
				layerAnimator = null;
				if( layerObject != null )
				{
					layerTransform = layerObject.transform;
					layerAnimator = layerObject.GetComponent<TwoStateAnimator>();
				}
				if( layerAngleSpeeds != null )
				{
					if( layerAngleSpeeds.Length > i )
					{
						layerSpeed = layerAngleSpeeds[i];
					}
				}
				layerTransforms[i] = layerTransform;
				layerSpeeds[i] = layerSpeed;
				layerAnimators[i] = layerAnimator;
			}
		}
	}

	public override void SetChildAnimator( GameObject newChildObject, bool behindParent )
	{
		return;
	}

	protected override void StartAlphaFadingChildAnimators( float newFadeSpeed, bool immediately, bool behindParent )
	{
		TwoStateAnimator layerAnimator = null;

		if( behindParent )
		{
			if( layerAnimators != null )
			{
				for( int i=0; i<layerAnimators.Length; i++ )
				{
					layerAnimator = layerAnimators[i];
					if( layerAnimator != null )
					{
						layerAnimator.StartAlphaFading( newFadeSpeed, immediately, true, false );
						break;
					}
				}
			}
		}
	}

	protected override void ClearChildAnimators()
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			for( int i=0; i<layerAnimators.Length; i++ )
			{
				layerAnimator = layerAnimators[i];
				if( layerAnimator != null )
				{
					layerAnimator.Clear();
				}
			}
		}
		layerTransforms = null;
		layerSpeeds = null;
		layerAnimators = null;
	}

	protected override void ProgressChildAnimators( float timeStep )
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			for( int i=0; i<layerAnimators.Length; i++ )
			{
				layerAnimator = layerAnimators[i];
				if( layerAnimator != null )
				{
					layerAnimator.Progress( timeStep );
				}
			}
		}
	}

	public override void SetActive( bool newActive )
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			for( int i=0; i<layerAnimators.Length; i++ )
			{
				layerAnimator = layerAnimators[i];
				if( layerAnimator != null )
				{
					layerAnimator.SetActive( newActive );
				}
			}
		}
		base.SetActive( newActive );
	}

	protected override void UpdateTransform( float timeStep )
	{
		/*halmeida - I don't want to create a subclass specific for the layers to include transform spinning, and I also
		don't want to include the spinning into the TwoStateAnimator class. Therefore, I cannot use the UpdateTransform
		of the animator of each layer to make them turn. I use this layer managing class instead, to make them spin.*/
		Transform layerTransform = null;
		float layerAngleChange = 0f;

		if( layerTransforms != null )
		{
			for( int i=0; i<layerTransforms.Length; i++ )
			{
				layerTransform = layerTransforms[i];
				if( layerTransform != null )
				{
					layerAngleChange = layerSpeeds[i] * timeStep;
					layerTransform.Rotate( new Vector3( 0f, 0f, layerAngleChange ) );
					/*halmeida - since rotating the parent rotates the children, I gotta remove this rotation from the children.
					The only layer that has no children is the last one. All the others before it apply their rotations to their
					children and that rotation of the children has to be undone.*/
					layerTransform = null;
					for( int j=(i+1); j<layerTransforms.Length; j++ )
					{
						layerTransform = layerTransforms[j];
						if( layerTransform != null )
						{
							layerTransform.Rotate( new Vector3( 0f, 0f, -layerAngleChange ) );
							break;
						}
					}
				}
			}
		}
	}

	public void SetLayerOpaqueAlpha( int layerIndex, float newOpaqueAlpha )
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			if( (layerIndex > -1) && (layerIndex < layerAnimators.Length) )
			{
				layerAnimator = layerAnimators[layerIndex];
				if( layerAnimator != null )
				{
					layerAnimator.SetOpaqueAlpha( newOpaqueAlpha );
				}
			}
		}
	}

	public void StartLayerAlphaFading( int layerIndex, float newFadeSpeed, bool immediately )
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			if( (layerIndex > -1) && (layerIndex < layerAnimators.Length) )
			{
				layerAnimator = layerAnimators[layerIndex];
				if( layerAnimator != null )
				{
					layerAnimator.StartAlphaFading( newFadeSpeed, immediately, false, false );
				}
			}
		}
	}

	protected override void StartChildDepthChain( float depth, bool inwards )
	{
		TwoStateAnimator layerAnimator = null;

		if( layerAnimators != null )
		{
			for( int i=0; i<layerAnimators.Length; i++ )
			{
				layerAnimator = layerAnimators[i];
				if( layerAnimator != null )
				{
					layerAnimator.StartDepthChain( depth, inwards );
					break;
				}
			}
		}
	}
}
