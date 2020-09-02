using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStateAnimator : BaseAnimator
{
	public const int STATUS_IDLE = 0;
	public const int STATUS_ACTIVE = 1;

	public GameObject sequenceIdle;
	public GameObject sequenceActive;

	private SpriteRenderer spriteRenderer;
	private bool active;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		active = false;
	}

	protected override void ClearRenderingComponent()
	{
		spriteRenderer = null;
	}

	protected override bool ExtractRenderingComponent()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		return (spriteRenderer != null);
	}

	protected override void FeedRenderingComponent( Sprite newSprite, Sprite newSpriteNormal, bool newSpriteFlip )
	{
		if( spriteRenderer != null )
		{
			spriteRenderer.sprite = newSprite;
			spriteRenderer.flipX = newSpriteFlip;
		}
	}

	protected override Color ExtractRenderingColor()
	{
		if( spriteRenderer != null )
		{
			return spriteRenderer.color;
		}
		return Color.white;
	}

	protected override void FeedRenderingColor( Color newColor )
	{
		if( spriteRenderer != null )
		{
			spriteRenderer.color = newColor;
		}
	}

	public virtual void SetActive( bool newActive )
	{
		active = newActive;
	}

	protected override void FillSequencesArray()
	{
		totalSequences = 2;
		sequenceObjects = new GameObject[totalSequences];
		sequenceObjects[0] = sequenceIdle;
		sequenceObjects[1] = sequenceActive;
	}

	protected override void UpdateStatus()
	{
		if( status == INVALID_STATUS )
		{
			if( !over )
			{
				status = STATUS_IDLE;
			}
		}
		else
		{
			if( active )
			{
				status = STATUS_ACTIVE;
			}
			else
			{
				status = STATUS_IDLE;
			}
		}
	}

	protected override void AdvanceToNextStatus()
	{
		status = INVALID_STATUS;
	}

	protected override void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		newSpriteFlip = false;
		switch( statusValue )
		{
			case STATUS_IDLE:
				newSequenceIndex = 0;
				break;
			case STATUS_ACTIVE:
				newSequenceIndex = 1;
				break;
		}
	}

	public bool IsActive()
	{
		return active;
	}
}
