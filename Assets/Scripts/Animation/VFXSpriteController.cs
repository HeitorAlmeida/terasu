using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpriteController : VFXAppearanceController
{
	protected SpriteRenderer spriteRenderer;
	protected Material material;
	protected int outlineColorID;
	protected int outlineWidthID;
	protected int colorToAddID;
	protected Color[] outlineColors;
	protected int outlineColorIndex;
	protected Color outlineColor;
	protected float outlineColorInterval;
	protected float outlineColorElapsed;
	protected float outlineWidth;
	protected float outAlphaCurrent;
	protected float outAlphaTarget;
	protected float[] outAlphaTargets;
	protected float outFadeDuration;
	protected float[] outFadeDurations;
	protected float outFadeSpeed;
	protected float[] outFadeSpeeds;
	protected int outAlphaTargetIndex;
	protected bool outAlphaTargetReached;
	protected bool outAllFadesOver;
	protected Vector4 colorToAdd;

	protected override void Awake()
	{
		material = null;
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		outlineColorID = -1;
		outlineWidthID = -1;
		colorToAddID = -1;
		ClearOutlineColorEvolution();
		outlineWidth = 0f;
		outAlphaCurrent = outlineColor.a;
		ClearOutlineAlphaEvolution();
		colorToAdd = Vector4.zero;
	}

	protected override void ClearRenderingComponent()
	{
		spriteRenderer = null;
	}

	protected override void ClearRenderingMaterial()
	{
		if( material != null )
		{
			Destroy( material );
			material = null;
		}
	}

	private void ClearOutlineColorEvolution()
	{
		outlineColors = null;
		outlineColorIndex = -1;
		outlineColor = Color.white;
		outlineColorInterval = 0f;
		outlineColorElapsed = 0f;
	}

	private void ClearOutlineAlphaEvolution()
	{
		outAlphaTarget = outAlphaCurrent;
		outAlphaTargets = null;
		outFadeDuration = 0f;
		outFadeDurations = null;
		outFadeSpeed = 0f;
		outFadeSpeeds = null;
		outAlphaTargetIndex = -1;
		outAlphaTargetReached = true;
		outAllFadesOver = true;
	}

	protected override void AddRenderingComponent()
	{
		spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		if( spriteRenderer != null )
		{
			material = spriteRenderer.material;
			outlineColorID = Shader.PropertyToID("_OutlineColor");
			outlineWidthID = Shader.PropertyToID("_OutlineWidth");
			colorToAddID = Shader.PropertyToID("_ColorToAdd");
			/*halmeida - the original material that comes with the SpriteRenderer does not support outline
			configuration, so it is kinda pointless to call FeedRenderingMaterial at this point.*/
		}
	}

	public override void ChangeRenderingMaterial( Material newMaterial )
	{
		if( newMaterial != null )
		{
			ClearRenderingMaterial();
			if( spriteRenderer != null )
			{
				/*halmeida - Notice that I do not set this class's material as the newMaterial, because that would not work.
				First I set the newMaterial into the renderer, then I get the material from the renderer. As I get the material
				from the renderer, the asset material that this method is most likely to receive gets cloned and I can operate
				over it without making a mess of the original material asset.*/
				spriteRenderer.material = newMaterial;
				material = spriteRenderer.material;
				FeedRenderingMaterial();
			}
		}
	}

	protected override void FeedRenderingComponent( Sprite newSprite, bool newSpriteFlip )
	{
		if( spriteRenderer != null )
		{
			spriteRenderer.sprite = newSprite;
			spriteRenderer.enabled = (newSprite != null);
			spriteRenderer.flipX = newSpriteFlip;
		}
	}

	protected override void FeedRenderingMaterial()
	{
		Color finalOutlineColor = Color.white;

		if( material != null )
		{
			finalOutlineColor = outlineColor;
			finalOutlineColor.a = outAlphaCurrent;
			material.SetColor( outlineColorID, finalOutlineColor );
			material.SetFloat( outlineWidthID, outlineWidth );
			material.SetVector( colorToAddID, colorToAdd );
		}
	}

	protected override Color ExtractRendererColor()
	{
		if( spriteRenderer != null )
		{
			return spriteRenderer.color;
		}
		return base.ExtractRendererColor();
	}

	protected override void FeedRendererColor( Color newColor )
	{
		if( spriteRenderer != null )
		{
			spriteRenderer.color = newColor;
		}
	}

	public override void ConfigureOutlineEvolution( Color[] newOutlineColors, float newOutlineWidth, float newOutlineColorInterval,
		float newOutAlphaOrigin, float[] newOutAlphaTargets, float[] newOutFadeDurations )
	{
		int targets = 0;
		float duration = 0f;
		float difference = 0f;

		if( outlineColors != null )
		{
			ClearOutlineColorEvolution();
		}
		/*halmeida - first I prepare the colors.*/
		if( newOutlineColors != null )
		{
			if( newOutlineColors.Length > 0 )
			{
				outlineColors = (Color[]) newOutlineColors.Clone();
				outlineColorIndex = 0;
				outlineColor = outlineColors[outlineColorIndex];
				outlineWidth = newOutlineWidth;
				outlineColorInterval = (newOutlineColorInterval > 0f) ? newOutlineColorInterval : 0f;
				outlineColorElapsed = 0f;
			}
		}
		if( outAlphaTargets != null )
		{
			ClearOutlineAlphaEvolution();
		}
		/*halmeida - then I prepare the variables so that the update brings the alpha to the requested origin alpha.*/
		outAllFadesOver = false;
		outAlphaTarget = newOutAlphaOrigin;
		outFadeDuration = 0f;
		outAlphaTargetReached = false;
		/*halmeida - then I prepare the arrays so that the next target may be assigned properly after the update.*/
		if( (newOutAlphaTargets != null) && (newOutFadeDurations != null) )
		{
			targets = newOutAlphaTargets.Length;
			if( newOutFadeDurations.Length >= targets )
			{
				outAlphaTargets = newOutAlphaTargets;
				outFadeDurations = newOutFadeDurations;
				outFadeSpeeds = new float[targets];
				for( int i=0; i<targets; i++ )
				{
					duration = outFadeDurations[i];
					if( duration > 0f )
					{
						difference = (i == 0) ? (outAlphaTargets[0] - newOutAlphaOrigin) : (outAlphaTargets[i] - outAlphaTargets[i-1]);
						outFadeSpeeds[i] = difference / duration;
					}
					else
					{
						outFadeSpeeds[i] = 0f;
					}
				}
			}
		}
		outAlphaTargetIndex = -1;
		/*halmeida - call the update.*/
		UpdateOutlineAlpha( 0f );
		FeedRenderingMaterial();
	}

	public override void ConfigureColorAddition( Vector3 newColorAddition )
	{
		colorToAdd = new Vector4( newColorAddition.x, newColorAddition.y, newColorAddition.z, 0f );
	}

	protected override void UpdateMaterial( float timeStep )
	{
		if( !outAllFadesOver )
		{
			UpdateOutlineAlpha( timeStep );
		}
		UpdateOutlineColor( timeStep );
		FeedRenderingMaterial();
	}

	private void UpdateOutlineAlpha( float timeStep )
	{
		if( !outAlphaTargetReached )
		{
			if( outFadeDuration > 0f )
			{
				if( outFadeSpeed != 0f )
				{
					outAlphaCurrent += outFadeSpeed * timeStep;
					if( outFadeSpeed > 0f )
					{
						if( outAlphaCurrent > outAlphaTarget )
						{
							outAlphaCurrent = outAlphaTarget;
							outAlphaTargetReached = true;
						}
					}
					else
					{
						if( outAlphaCurrent < outAlphaTarget )
						{
							outAlphaCurrent = outAlphaTarget;
							outAlphaTargetReached = true;
						}
					}
				}
				else
				{
					outAlphaCurrent = outAlphaTarget;
					outAlphaTargetReached = true;
				}
			}
			else
			{
				outAlphaCurrent = outAlphaTarget;
				outAlphaTargetReached = true;
			}
		}
		if( outAlphaTargetReached )
		{
			AdvanceOutlineAlphaTarget();
		}
	}

	private void AdvanceOutlineAlphaTarget()
	{
		if( !outAllFadesOver && outAlphaTargetReached )
		{
			if( outAlphaTargets != null )
			{
				outAlphaTargetIndex = (outAlphaTargetIndex < -1) ? -1 : outAlphaTargetIndex;
				outAlphaTargetIndex++;
				if( outAlphaTargetIndex < outAlphaTargets.Length )
				{
					outAlphaTarget = outAlphaTargets[outAlphaTargetIndex];
					outFadeDuration = outFadeDurations[outAlphaTargetIndex];
					outFadeSpeed = outFadeSpeeds[outAlphaTargetIndex];
					outAlphaTargetReached = false;
				}
			}
			outAllFadesOver = outAlphaTargetReached;
		}
	}

	private void UpdateOutlineColor( float timeStep )
	{
		if( (outlineColors != null) && (outlineColorInterval > 0f) )
		{
			outlineColorElapsed += timeStep;
			if( outlineColorElapsed > outlineColorInterval )
			{
				outlineColorIndex++;
				if( outlineColorIndex >= outlineColors.Length )
				{
					outlineColorIndex = 0;
				}
				outlineColor = outlineColors[outlineColorIndex];
			}
		}
	}
}
