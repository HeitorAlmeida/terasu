using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXAppearanceController : MonoBehaviour
{
	protected Sprite sprite;
	protected bool spriteFlip;
	protected bool fixedSprite;
	protected float duration;
	protected float elapsed;
	protected bool started;
	protected bool over;
	protected Vector3 scaleTarget;
	protected Vector3[] scaleTargets;
	protected float scaleDuration;
	protected float[] scaleDurations;
	protected Vector3 scaleSpeed;
	protected Vector3[] scaleSpeeds;
	protected int scaleTargetIndex;
	protected bool scaleTargetReached;
	protected bool allScalingsOver;
	protected float alphaCurrent;
	protected float alphaTarget;
	protected float[] alphaTargets;
	protected float fadeDuration;
	protected float[] fadeDurations;
	protected float fadeSpeed;
	protected float[] fadeSpeeds;
	protected int alphaTargetIndex;
	protected bool alphaTargetReached;
	protected bool allFadesOver;

	protected virtual void Awake()
	{
		Clear( false );
		AddRenderingComponent();
	}

	public virtual void Clear( bool renderingComponentInitialized = true )
	{
		if( renderingComponentInitialized )
		{
			FeedRenderingComponent( null, false );
		}
		ClearRenderingComponent();
		ClearRenderingMaterial();
		sprite = null;
		spriteFlip = false;
		fixedSprite = false;
		duration = 0f;
		elapsed = 0f;
		started = false;
		over = false;
		ClearScaleEvolution();
		alphaCurrent = 1f;
		ClearAlphaEvolution();
	}

	protected virtual void ClearRenderingComponent()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer = null;
	}

	protected virtual void ClearRenderingMaterial()
	{
	}

	private void ClearScaleEvolution()
	{
		scaleTarget = Vector3.one;
		scaleTargets = null;
		scaleDuration = 0f;
		scaleDurations = null;
		scaleSpeed = Vector3.zero;
		scaleSpeeds = null;
		scaleTargetIndex = -1;
		scaleTargetReached = true;
		allScalingsOver = true;
	}  

	private void ClearAlphaEvolution()
	{
		alphaTarget = 1f;
		alphaTargets = null;
		fadeDuration = 0f;
		fadeDurations = null;
		fadeSpeed = 0;
		fadeSpeeds = null;
		alphaTargetIndex = -1;
		alphaTargetReached = true;
		allFadesOver = true;
	}

	protected virtual void AddRenderingComponent()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: AddComponent<SpriteRenderer>();
	}

	public virtual void ChangeRenderingMaterial( Material newMaterial )
	{
	}

	protected virtual void FeedRenderingComponent( Sprite newSprite, bool newSpriteFlip )
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer.sprite = newSprite;
	}

	protected virtual void FeedRenderingMaterial()
	{
	}

	protected virtual Color ExtractRendererColor()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: return spriteRenderer.color;
		return Color.white;
	}

	protected virtual void FeedRendererColor( Color newColor )
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer.color = newColor;
	}

	public void SetSprite( Sprite newSprite, bool newSpriteFlip, bool fixSprite )
	{
		if( !fixedSprite )
		{
			sprite = newSprite;
			spriteFlip = newSpriteFlip;
			FeedRenderingComponent( sprite, spriteFlip );
			fixedSprite = fixSprite;
		}
	}

	public void ConfigureScaleEvolution( Vector3 newScaleOrigin, Vector3[] newScaleTargets, float[] newScaleDurations )
	{
		int targets = 0;
		float duration = 0f;
		Vector3 difference = Vector3.zero;

		if( scaleTargets != null )
		{
			ClearScaleEvolution();
		}
		/*halmeida - first I prepare the variables so that the update brings the scale to the requested origin scale.*/
		allScalingsOver = false;
		scaleTarget = newScaleOrigin;
		scaleDuration = 0f;
		scaleTargetReached = false;
		/*halmeida - then I prepare the arrays so that the next target may be assigned properly after the update.*/
		if( (newScaleTargets != null) && (newScaleDurations != null) )
		{
			targets = newScaleTargets.Length;
			if( newScaleDurations.Length >= targets )
			{
				scaleTargets = newScaleTargets;
				scaleDurations = newScaleDurations;
				scaleSpeeds = new Vector3[targets];
				for( int i=0; i<targets; i++ )
				{
					duration = scaleDurations[i];
					if( duration > 0f )
					{
						difference = (i == 0) ? (scaleTargets[0] - newScaleOrigin) : (scaleTargets[i] - scaleTargets[i-1]);
						scaleSpeeds[i] = difference / duration;
					}
					else
					{
						scaleSpeeds[i] = Vector3.zero;
					}
				}
			}
		}
		scaleTargetIndex = -1;
		/*halmeida - call the update.*/
		UpdateTransformScale( 0f );
	}

	public void ConfigureAlphaEvolution( float newAlphaOrigin, float[] newAlphaTargets, float[] newFadeDurations )
	{
		int targets = 0;
		float duration = 0f;
		float difference = 0f;

		if( alphaTargets != null )
		{
			ClearAlphaEvolution();
		}
		/*halmeida - first I prepare the variables so that the update brings the alpha to the requested origin alpha.*/
		allFadesOver = false;
		alphaTarget = newAlphaOrigin;
		fadeDuration = 0f;
		alphaTargetReached = false;
		/*halmeida - then I prepare the arrays so that the next target may be assigned properly after the update.*/
		if( (newAlphaTargets != null) && (newFadeDurations != null) )
		{
			targets = newAlphaTargets.Length;
			if( newFadeDurations.Length >= targets )
			{
				alphaTargets = newAlphaTargets;
				fadeDurations = newFadeDurations;
				fadeSpeeds = new float[targets];
				for( int i=0; i<targets; i++ )
				{
					duration = fadeDurations[i];
					if( duration > 0f )
					{
						difference = (i == 0) ? (alphaTargets[0] - newAlphaOrigin) : (alphaTargets[i] - alphaTargets[i-1]);
						fadeSpeeds[i] = difference / duration;
					}
					else
					{
						fadeSpeeds[i] = 0f;
					}
				}
			}
		}
		alphaTargetIndex = -1;
		/*halmeida - call the update.*/
		UpdateRendererColor( 0f );
	}

	public virtual void ConfigureOutlineEvolution( Color[] newOutlineColors, float newOutlineWidth, float newOutlineColorInterval,
		float newOutAlphaOrigin, float[] newOutAlphaTargets, float[] newOutFadeDurations )
	{
	}

	public virtual void ConfigureColorAddition( Vector3 newColorAddition )
	{
	}

	public void StartEvolutions( float totalDuration )
	{
		if( !started && !over )
		{
			if( totalDuration > 0f )
			{
				duration = totalDuration;
				elapsed = 0f;
				started = true;
			}
		}
	}

	public void Progress( float timeStep )
	{
		if( started && !over )
		{
			UpdateTransform( timeStep );
			UpdateRenderer( timeStep );
			UpdateMaterial( timeStep );
			elapsed += timeStep;
			if( elapsed > duration )
			{
				over = true;
			}
		}
	}

	protected virtual void UpdateTransform( float timeStep )
	{
		UpdateTransformScale( timeStep );
	}

	protected virtual void UpdateRenderer( float timeStep )
	{
		UpdateRendererColor( timeStep );
	}

	protected virtual void UpdateMaterial( float timeStep )
	{
	}

	protected virtual void UpdateTransformScale( float timeStep )
	{
		Vector3 currentScale = Vector3.zero;

		if( !allScalingsOver )
		{
			if( !scaleTargetReached )
			{
				if( scaleDuration > 0f )
				{
					if( scaleSpeed == Vector3.zero )
					{
						transform.localScale = scaleTarget;
						scaleTargetReached = true;
					}
					else
					{
						currentScale = transform.localScale;
						if( scaleSpeed.x != 0f )
						{
							currentScale.x += scaleSpeed.x * timeStep;
							if( scaleSpeed.x > 0f )
							{
								if( currentScale.x > scaleTarget.x )
								{
									currentScale.x = scaleTarget.x;
									scaleTargetReached = true;
								}
							}
							else
							{
								if( currentScale.x < scaleTarget.x )
								{
									currentScale.x = scaleTarget.x;
									scaleTargetReached = true;
								}
							}
						}
						if( scaleSpeed.y != 0f )
						{
							currentScale.y += scaleSpeed.y * timeStep;
							if( scaleSpeed.y > 0f )
							{
								if( currentScale.y > scaleTarget.y )
								{
									currentScale.y = scaleTarget.y;
									scaleTargetReached = true;
								}
							}
							else
							{
								if( currentScale.y < scaleTarget.y )
								{
									currentScale.y = scaleTarget.y;
									scaleTargetReached = true;
								}
							}
						}
						if( scaleSpeed.z != 0f )
						{
							currentScale.z += scaleSpeed.z * timeStep;
							if( scaleSpeed.z > 0f )
							{
								if( currentScale.z > scaleTarget.z )
								{
									currentScale.z = scaleTarget.z;
									scaleTargetReached = true;
								}
							}
							else
							{
								if( currentScale.z < scaleTarget.z )
								{
									currentScale.z = scaleTarget.z;
									scaleTargetReached = true;
								}
							}
						}
						transform.localScale = currentScale;
					}
				}
				else
				{
					transform.localScale = scaleTarget;
					scaleTargetReached = true;
				}
			}
			if( scaleTargetReached )
			{
				AdvanceScaleTarget();
			}
		}
	}

	private void AdvanceScaleTarget()
	{
		if( !allScalingsOver && scaleTargetReached )
		{
			if( scaleTargets != null )
			{
				scaleTargetIndex = (scaleTargetIndex < -1) ? -1 : scaleTargetIndex;
				scaleTargetIndex++;
				if( scaleTargetIndex < scaleTargets.Length )
				{
					scaleTarget = scaleTargets[scaleTargetIndex];
					scaleDuration = scaleDurations[scaleTargetIndex];
					scaleSpeed = scaleSpeeds[scaleTargetIndex];
					scaleTargetReached = false;
				}
			}
			allScalingsOver = scaleTargetReached;
		}
	}

	protected virtual void UpdateRendererColor( float timeStep )
	{
		Color rendererColor = Color.white;

		rendererColor = ExtractRendererColor();
		if( !allFadesOver )
		{
			alphaCurrent = rendererColor.a;
			UpdateAlphaFading( timeStep );
			rendererColor.a = alphaCurrent;
		}
		FeedRendererColor( rendererColor );
	}

	private void UpdateAlphaFading( float timeStep )
	{
		if( !alphaTargetReached )
		{
			if( fadeDuration > 0f )
			{
				if( fadeSpeed != 0f )
				{
					alphaCurrent += fadeSpeed * timeStep;;
					if( fadeSpeed > 0f )
					{
						if( alphaCurrent > alphaTarget )
						{
							alphaCurrent = alphaTarget;
							alphaTargetReached = true;
						}
					}
					else
					{
						if( alphaCurrent < alphaTarget )
						{
							alphaCurrent = alphaTarget;
							alphaTargetReached = true;
						}
					}
				}
				else
				{
					alphaCurrent = alphaTarget;
					alphaTargetReached = true;
				}
			}
			else
			{
				alphaCurrent = alphaTarget;
				alphaTargetReached = true;
			}
		}
		if( alphaTargetReached )
		{
			AdvanceAlphaTarget();
		}
	}

	private void AdvanceAlphaTarget()
	{
		if( !allFadesOver && alphaTargetReached )
		{
			if( alphaTargets != null )
			{
				alphaTargetIndex = (alphaTargetIndex < -1) ? -1 : alphaTargetIndex;
				alphaTargetIndex++;
				if( alphaTargetIndex < alphaTargets.Length )
				{
					alphaTarget = alphaTargets[alphaTargetIndex];
					fadeDuration = fadeDurations[alphaTargetIndex];
					fadeSpeed = fadeSpeeds[alphaTargetIndex];
					alphaTargetReached = false;
				}
			}
			allFadesOver = alphaTargetReached;
		}
	}

	public bool IsOver()
	{
		return over;
	}
}
