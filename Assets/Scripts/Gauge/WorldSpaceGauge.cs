using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceGauge : MonoBehaviour
{
	public Color backgroundColor;
	public Vector2 backgroundSize;
	public Color foregroundColor;
	public Vector2 foregroundSize;
	public bool reverseSink;

	private GameObject bgObject;
	private Texture2D bgTexture;
	private Sprite bgSprite;
	private SpriteRenderer bgSpriteRenderer;
	private GameObject fgObject;
	private Texture2D fgTexture;
	private Sprite fgSprite;
	private SpriteRenderer fgSpriteRenderer;
	private float fgMaxXScale;
	private float fgMaxYScale;
	private float maxValue;
	private float minValue;
	private float currentValue;
	private bool horizontal;
	private float bgToFgDepth;
	private bool built;

	void Awake()
	{
		bgObject = null;
		bgTexture = null;
		bgSprite = null;
		bgSpriteRenderer = null;
		fgObject = null;
		fgTexture = null;
		fgSprite = null;
		fgSpriteRenderer = null;
		fgMaxXScale = 0f;
		fgMaxYScale = 0f;
		maxValue = 0f;
		minValue = 0f;
		currentValue = 0f;
		horizontal = false;
		bgToFgDepth = -DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / 100f;
		built = false;
	}

	public void Build( float maxDisplayableValue, float minDisplayableValue )
	{
		if( !built && (maxDisplayableValue > minDisplayableValue) && (bgObject == null) && (fgObject == null) )
		{
			CreateBackground();
			CreateForeground();
			maxValue = maxDisplayableValue;
			minValue = minDisplayableValue;
			currentValue = maxValue;
			built = true;
		}
	}

	private void CreateBackground()
	{
		Rect textureRect = default( Rect );

		bgTexture = new Texture2D( 4, 4 );
		for( int i=0; i<4; i++ )
		{
			for( int j=0; j<4; j++ )
			{
				bgTexture.SetPixel( i, j, backgroundColor );
			}
		}
		bgTexture.Apply();
		textureRect = new Rect( 0f, 0f, 4f, 4f );
		bgSprite = Sprite.Create( bgTexture, textureRect, new Vector2( 0.5f, 0.5f ), 4f );
		bgObject = new GameObject("WSGaugeBackground");
		bgSpriteRenderer = bgObject.AddComponent<SpriteRenderer>();
		bgSpriteRenderer.sprite = bgSprite;
		bgObject.transform.SetParent( transform, false );
		bgObject.transform.localPosition = Vector3.zero;
		bgObject.transform.localScale = new Vector3( backgroundSize.x, backgroundSize.y, 1f );
	}

	private void CreateForeground()
	{
		Rect textureRect = default( Rect );

		if( bgObject == null )
		{
			return;
		}
		fgTexture = new Texture2D( 4, 4 );
		for( int i=0; i<4; i++ )
		{
			for( int j=0; j<4; j++ )
			{
				fgTexture.SetPixel( i, j, foregroundColor );
			}
		}
		fgTexture.Apply();
		textureRect = new Rect( 0f, 0f, 4f, 4f );
		fgSprite = Sprite.Create( fgTexture, textureRect, new Vector2( 0.5f, 0.5f ), 4f );
		fgObject = new GameObject("WSGaugeForeground");
		fgSpriteRenderer = fgObject.AddComponent<SpriteRenderer>();
		fgSpriteRenderer.sprite = fgSprite;
		fgObject.transform.SetParent( transform, false );
		fgObject.transform.localPosition = new Vector3( 0f, 0f, bgToFgDepth );
		fgMaxXScale = foregroundSize.x;
		fgMaxYScale = foregroundSize.y;
		fgObject.transform.localScale = new Vector3( fgMaxXScale, fgMaxYScale, 1f );
		horizontal = (fgMaxXScale >= fgMaxYScale);
	}

	public float GetCurrentValue()
	{
		return currentValue;
	}

	public void SetCurrentValue( float newValue )
	{
		if( built )
		{
			currentValue = newValue;
			if( currentValue > maxValue )
			{
				currentValue = maxValue;
			}
			if( currentValue < minValue )
			{
				currentValue = minValue;
			}
			UpdateGaugeForeground();
		}
	}

	private void UpdateGaugeForeground()
	{
		float currentRate = 0f;
		float currentDimension = 0f;
		Vector3 gaugeScale = Vector3.zero;
		Vector3 gaugePosition = Vector3.zero;
		float offsetToSink = 0f;

		if( fgObject != null )
		{
			currentRate = currentValue / maxValue;
			gaugeScale = fgObject.transform.localScale;
			gaugePosition = fgObject.transform.localPosition;
			if( horizontal )
			{
				currentDimension = currentRate * fgMaxXScale;
				gaugeScale.x = currentDimension;
				offsetToSink = (fgMaxXScale - currentDimension) / 2f;
				if( reverseSink )
				{
					offsetToSink *= -1;
				}
				gaugePosition.x = -offsetToSink;
			}
			else
			{
				currentDimension = currentRate * fgMaxYScale;
				gaugeScale.y = currentDimension;
				offsetToSink = (fgMaxYScale - currentDimension) / 2f;
				if( reverseSink )
				{
					offsetToSink *= -1;
				}
				gaugePosition.y = -offsetToSink;
			}
			fgObject.transform.localScale = gaugeScale;
			fgObject.transform.localPosition = gaugePosition;
		}
	}

	public void Clear()
	{
		if( fgObject != null )
		{
			fgSpriteRenderer = null;
			Destroy( fgTexture );
			fgTexture = null;
			Destroy( fgSprite );
			fgSprite = null;
			Destroy( fgObject );
			fgObject = null;
		}
		if( bgObject != null )
		{
			bgSpriteRenderer = null;
			Destroy( bgTexture );
			bgTexture = null;
			Destroy( bgSprite );
			bgSprite = null;
			Destroy( bgObject );
			bgObject = null;
		}
		built = false;
	}

	public void ToggleVisibility( bool enable )
	{
		if( fgSpriteRenderer != null )
		{
			fgSpriteRenderer.enabled = enable;
		}
		if( bgSpriteRenderer != null )
		{
			bgSpriteRenderer.enabled = enable;
		}
	}
}
