using System.Collections;
using UnityEngine;
using UnityEngine.UI; //to use Image component.

public class SpritedNumberUI : MonoBehaviour
{
	private SymbolDatabase spriteSource;
	private int digitAmount;
	private GameObject[] digitObjects;
	private Image[] digitImages;
	private RectTransform[] digitTransforms;
	private RectTransform ownTransform;
	private int innerValue;
	private int outerValue;
	private bool suddenChange;
	private int framesPerValue;
	private int elapsedFrames;
	private bool initialized;
	private bool raycastTarget;

	void Awake()
	{
		spriteSource = null;
		digitAmount = 0;
		digitObjects = null;
		digitImages = null;
		digitTransforms = null;
		ownTransform = this.gameObject.GetComponent<RectTransform>();
		innerValue = 0;
		outerValue = 0;
		suddenChange = false;
		framesPerValue = 1;
		elapsedFrames = 0;
		initialized = false;
		raycastTarget = true;
	}

	public void SetSymbolSource( SymbolDatabase newSpriteSource )
	{
		spriteSource = newSpriteSource;
	}

	public void ToggleRaycastTargeting( bool enable )
	{
		Image digitImage = null;

		raycastTarget = enable;
		if( digitImages != null )
		{
			for( int i=0; i<digitImages.Length; i++ )
			{
				digitImage = digitImages[i];
				if( digitImage != null )
				{
					digitImage.raycastTarget = raycastTarget;
				}
			}
		}
	}

	public void SetValue( int newValue )
	{
		innerValue = newValue;
		if( suddenChange || !initialized )
		{
			outerValue = innerValue;
			UpdateDigitAmount();
			UpdateDigitLooks();
			UpdateDigitPositions();
		}
		initialized = true;
	}

	private void UpdateDigitAmount()
	{
		int newDigitAmount = 0;
		int additionalDigits = 0;
		GameObject[] newDigitObjects = null;
		Image[] newDigitImages = null;
		RectTransform[] newDigitTransforms = null;
		GameObject newDigitObject = null;
		Image newDigitImage = null;
		RectTransform newDigitTransform = null;

		newDigitAmount = UsefulFunctions.GetDigitAmount( outerValue );
		if( digitAmount != newDigitAmount )
		{
			if( digitAmount < newDigitAmount )
			{
				additionalDigits = newDigitAmount - digitAmount;
				newDigitObjects = new GameObject[additionalDigits];
				newDigitImages = new Image[additionalDigits];
				newDigitTransforms = new RectTransform[additionalDigits];
				for( int i=0; i<additionalDigits; i++ )
				{
					newDigitObject = new GameObject("Digit"+(digitAmount+i), typeof(RectTransform));
					newDigitImage = newDigitObject.AddComponent<Image>();
					newDigitImage.raycastTarget = raycastTarget;
					newDigitTransform = newDigitObject.GetComponent<RectTransform>();
					if( newDigitTransform != null )
					{
						newDigitTransform.SetParent( this.gameObject.transform, false );
					}
					newDigitObjects[i] = newDigitObject;
					newDigitImages[i] = newDigitImage;
					newDigitTransforms[i] = newDigitTransform;
				}
				UsefulFunctions.IncreaseArrayWithArray<GameObject>( ref digitObjects, newDigitObjects );
				UsefulFunctions.IncreaseArrayWithArray<Image>( ref digitImages, newDigitImages );
				UsefulFunctions.IncreaseArrayWithArray<RectTransform>( ref digitTransforms, newDigitTransforms );
				newDigitObjects = null;
				newDigitImages = null;
				newDigitTransforms = null;
			}
			else
			{
				if( (digitObjects != null) && (digitImages != null) && (digitTransforms != null) )
				{
					if( (digitObjects.Length == digitAmount) && (digitImages.Length == digitAmount) && (digitTransforms.Length == digitAmount) )
					{
						for( int i=newDigitAmount; i<digitAmount; i++ )
						{
							digitTransforms[i] = null;
							digitImages[i] = null;
							Destroy( digitObjects[i] );
							digitObjects[i] = null;
						}
						newDigitObjects = new GameObject[newDigitAmount];
						newDigitImages = new Image[newDigitAmount];
						newDigitTransforms = new RectTransform[newDigitAmount];
						for( int i=0; i<newDigitAmount; i++ )
						{
							newDigitObjects[i] = digitObjects[i];
							newDigitImages[i] = digitImages[i];
							newDigitTransforms[i] = digitTransforms[i];
						}
						digitObjects = newDigitObjects;
						digitImages = newDigitImages;
						digitTransforms = newDigitTransforms;
						newDigitObjects = null;
						newDigitImages = null;
						newDigitTransforms = null;
					}
				}
			}
			digitAmount = newDigitAmount;
		}
	}

	private void UpdateDigitLooks()
	{
		int digit = 0;
		int dividedValue = 0;
		Sprite digitSprite = null;
		Image digitImage = null;

		if( (spriteSource != null) && (digitAmount > 0) )
		{
			dividedValue = outerValue;
			for( int i=0; i<digitAmount; i++ )
			{
				digit = dividedValue % 10;
				dividedValue = dividedValue / 10;
				digitSprite = spriteSource.GetDigitSprite( digit );
				digitImage = digitImages[i];
				if( digitImage != null )
				{
					digitImage.sprite = digitSprite;
					digitImage.SetNativeSize();
				}
			}
		}
	}

	private void UpdateDigitPositions()
	{
		float totalWidth = 0f;
		RectTransform rectTrans = null;
		float currentOffsetX = 0f;
		float numberHeight = 0f;

		for( int i=0; i<digitAmount; i++ )
		{
			rectTrans = digitTransforms[i];
			if( rectTrans != null )
			{
				totalWidth += rectTrans.rect.width;
				if( numberHeight == 0f )
				{
					numberHeight = rectTrans.rect.height;
				}
			}
		}
		currentOffsetX = totalWidth / 2f;
		for( int i=0; i<digitAmount; i++ )
		{
			rectTrans = digitTransforms[i];
			if( rectTrans != null )
			{
				currentOffsetX -= rectTrans.rect.width / 2f;
				rectTrans.anchoredPosition = new Vector2( currentOffsetX, 0f );
				currentOffsetX -= rectTrans.rect.width / 2f;
			}
		}
		if( ownTransform != null )
		{
			ownTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, totalWidth );
			ownTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, numberHeight );
		}
	}

	public void Progress()
	{
		if( outerValue != innerValue )
		{
			if( !suddenChange )
			{
				elapsedFrames++;
				if( elapsedFrames >= framesPerValue )
				{
					elapsedFrames = 0;
					if( outerValue < innerValue )
					{
						outerValue++;
					}
					else
					{
						outerValue--;
					}
					UpdateDigitAmount();
					UpdateDigitLooks();
					UpdateDigitPositions();
				}
			}
			else
			{
				outerValue = innerValue;
				UpdateDigitAmount();
				UpdateDigitLooks();
				UpdateDigitPositions();
			}
		}
	}

	public void ToggleSuddenChange( bool sudden )
	{
		suddenChange = sudden;
	}

	public void SetColor( Color newColor )
	{
		Image digitImage = null;

		if( digitAmount > 0 )
		{
			for( int i=0; i<digitAmount; i++ )
			{
				digitImage = digitImages[i];
				if( digitImage != null )
				{
					digitImage.color = newColor;
				}
			}
		}
	}

	public void Clear()
	{
		GameObject digitObject = null;

		if( digitAmount > 0 )
		{
			for( int i=0; i<digitAmount; i++ )
			{
				digitTransforms[i] = null;
				digitImages[i] = null;
				digitObject = digitObjects[i];
				if( digitObject != null )
				{
					Destroy( digitObject );
					digitObject = null;
					digitObjects[i] = null;
				}
			}
			digitTransforms = null;
			digitImages = null;
			digitObjects = null;
			digitAmount = 0;
		}
	}
}
