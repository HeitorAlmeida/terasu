using System.Collections;
using UnityEngine;
using UnityEngine.UI; //to use Image component.

public class SpritedStringUI : MonoBehaviour
{
	private SymbolDatabase spriteSource;
	private int symbolAmount;
	private GameObject[] symbolObjects;
	private Image[] symbolImages;
	private RectTransform[] symbolTransforms;
	private float[] symbolWidths;
	private Material fontModMaterial;
	private RectTransform ownTransform;
	private string value;
	private float width;
	private float height;
	private bool raycastTarget;
	private Color currentGeneralColor;
	private Color currentColor;
	private Vector4 currentColorGradient;
	private int colorID;
	private int colorGradientID;
	private int vertexToCameraID;

	void Awake()
	{
		spriteSource = null;
		symbolAmount = 0;
		symbolObjects = null;
		symbolImages = null;
		symbolTransforms = null;
		symbolWidths = null;
		fontModMaterial = null;
		ownTransform = gameObject.GetComponent<RectTransform>();
		value = null;
		width = 0f;
		height = 0f;
		raycastTarget = true;
		currentGeneralColor = Color.white;
		currentColor = Color.white;
		currentColorGradient = Vector4.zero;
		colorID = Shader.PropertyToID("_NewColor");
		colorGradientID = Shader.PropertyToID("_Gradient");
		vertexToCameraID = Shader.PropertyToID("_VertexToCamera");
	}

	public void SetSymbolSource( SymbolDatabase newSpriteSource )
	{
		Material fontModifier = null;

		spriteSource = newSpriteSource;
		if( spriteSource != null )
		{
			fontModifier = spriteSource.fontModMaterial;
			if( fontModifier != null )
			{
				fontModMaterial = new Material( fontModifier );
			}
		}
	}

	public void ToggleRaycastTargeting( bool enable )
	{
		Image symbolImage = null;

		raycastTarget = enable;
		if( symbolImages != null )
		{
			for( int i=0; i<symbolImages.Length; i++ )
			{
				symbolImage = symbolImages[i];
				if( symbolImage != null )
				{
					symbolImage.raycastTarget = raycastTarget;
				}
			}
		}
	}

	public void SetValue( string newValue )
	{
		value = newValue;
		UpdateSymbolAmount();
		UpdateSymbolLooks();
		UpdateSymbolPositions();
		SetGeneralColor( currentGeneralColor );
		SetColor( currentColor, currentColorGradient );
	}

	private void UpdateSymbolAmount()
	{
		int newSymbolAmount = 0;
		int additionalSymbols = 0;
		GameObject[] newSymbolObjects = null;
		Image[] newSymbolImages = null;
		RectTransform[] newSymbolTransforms = null;
		float[] newSymbolWidths = null;
		GameObject newSymbolObject = null;
		Image newSymbolImage = null;
		RectTransform newSymbolTransform = null;
		float newSymbolWidth = 0f;

		if( value != null )
		{
			newSymbolAmount = value.Length;
		}
		if( symbolAmount != newSymbolAmount )
		{
			if( symbolAmount < newSymbolAmount )
			{
				additionalSymbols = newSymbolAmount - symbolAmount;
				newSymbolObjects = new GameObject[additionalSymbols];
				newSymbolImages = new Image[additionalSymbols];
				newSymbolTransforms = new RectTransform[additionalSymbols];
				newSymbolWidths = new float[additionalSymbols];
				for( int i=0; i<additionalSymbols; i++ )
				{
					newSymbolObject = new GameObject("CharacterSymbol"+(symbolAmount+i), typeof(RectTransform));
					newSymbolImage = newSymbolObject.AddComponent<Image>();
					newSymbolImage.raycastTarget = raycastTarget;
					if( fontModMaterial != null )
					{
						newSymbolImage.material = fontModMaterial;
					}
					newSymbolTransform = newSymbolObject.GetComponent<RectTransform>();
					newSymbolTransform.SetParent( ownTransform, false );
					newSymbolObjects[i] = newSymbolObject;
					newSymbolImages[i] = newSymbolImage;
					newSymbolTransforms[i] = newSymbolTransform;
					newSymbolWidths[i] = newSymbolWidth;
				}
				UsefulFunctions.IncreaseArrayWithArray<GameObject>( ref symbolObjects, newSymbolObjects );
				UsefulFunctions.IncreaseArrayWithArray<Image>( ref symbolImages, newSymbolImages );
				UsefulFunctions.IncreaseArrayWithArray<RectTransform>( ref symbolTransforms, newSymbolTransforms );
				UsefulFunctions.IncreaseArrayWithArray<float>( ref symbolWidths, newSymbolWidths );
				newSymbolObjects = null;
				newSymbolImages = null;
				newSymbolTransforms = null;
				newSymbolWidths = null;
			}
			else
			{
				if( (symbolObjects != null) && (symbolImages != null) && (symbolTransforms != null) && (symbolWidths != null) )
				{
					if( (symbolObjects.Length == symbolAmount) && (symbolImages.Length == symbolAmount) &&
						(symbolTransforms.Length == symbolAmount) && (symbolWidths.Length == symbolAmount) )
					{
						for( int i=newSymbolAmount; i<symbolAmount; i++ )
						{
							symbolTransforms[i] = null;
							symbolImages[i] = null;
							Destroy( symbolObjects[i] );
							symbolObjects[i] = null;
						}
						newSymbolObjects = new GameObject[newSymbolAmount];
						newSymbolImages = new Image[newSymbolAmount];
						newSymbolTransforms = new RectTransform[newSymbolAmount];
						newSymbolWidths = new float[newSymbolAmount];
						for( int i=0; i<newSymbolAmount; i++ )
						{
							newSymbolObjects[i] = symbolObjects[i];
							newSymbolImages[i] = symbolImages[i];
							newSymbolTransforms[i] = symbolTransforms[i];
							newSymbolWidths[i] = symbolWidths[i];
						}
						symbolObjects = newSymbolObjects;
						symbolImages = newSymbolImages;
						symbolTransforms = newSymbolTransforms;
						symbolWidths = newSymbolWidths;
						newSymbolObjects = null;
						newSymbolImages = null;
						newSymbolTransforms = null;
						newSymbolWidths = null;
					}
				}
			}
			symbolAmount = newSymbolAmount;
		}
	}

	private void UpdateSymbolLooks()
	{
		char character = '?';
		Sprite symbolSprite = null;
		Image symbolImage = null;
		RectTransform symbolTransform = null;
		float symbolWidth = 0f;
		float symbolHeight = 0f;

		width = 0f;
		height = 0f;
		if( (spriteSource != null) && (symbolAmount > 0) )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				character = value[i];
				symbolSprite = spriteSource.GetSymbolSprite( character );
				if( symbolSprite != null )
				{
					symbolImage = symbolImages[i];
					if( symbolImage != null )
					{
						symbolImage.sprite = symbolSprite;
						symbolImage.SetNativeSize();
						symbolTransform = symbolTransforms[i];
						symbolWidth = symbolTransform.rect.width * symbolTransform.localScale.x;
						symbolWidths[i] = symbolWidth;
						width += symbolWidth;
						symbolHeight = symbolTransform.rect.height * symbolTransform.localScale.y;
						if( symbolHeight > height )
						{
							height = symbolHeight;
						}
					}
				}
			}
		}
		if( ownTransform != null )
		{
			ownTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, width );
			ownTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, height );
		}
	}

	private void UpdateSymbolPositions()
	{
		RectTransform rectTrans = null;
		float currentOffsetX = 0f;
		float symbolWidth = 0f;

		currentOffsetX = -width / 2f;
		for( int i=0; i<symbolAmount; i++ )
		{
			rectTrans = symbolTransforms[i];
			if( rectTrans != null )
			{
				symbolWidth = symbolWidths[i];
				currentOffsetX += symbolWidth / 2f;
				rectTrans.anchoredPosition = new Vector2( currentOffsetX, 0f );
				currentOffsetX += symbolWidth / 2f;
			}
		}
	}

	public void SetGeneralColor( Color newColor )
	{
		Image symbolImage = null;

		currentGeneralColor = newColor;
		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolImage = symbolImages[i];
				if( symbolImage != null )
				{
					symbolImage.color = currentGeneralColor;
				}
			}
		}
	}

	public void SetColor( Color newColor, Vector4 newColorGradient )
	{
		currentColor = newColor;
		currentColorGradient = newColorGradient;
		if( fontModMaterial != null )
		{
			fontModMaterial.SetColor(colorID, currentColor);
			fontModMaterial.SetVector(colorGradientID, currentColorGradient); 
		}
	}

	public void FeedPositionToMaterial()
	{
		Vector4 vertexToCamera = Vector4.zero;

		if( fontModMaterial != null )
		{
			vertexToCamera = new Vector4( -ownTransform.anchoredPosition.x, -ownTransform.anchoredPosition.y, 0f, 0f );
			fontModMaterial.SetVector(vertexToCameraID, vertexToCamera );
		}
	}

	public Color GetGeneralColor()
	{
		return currentGeneralColor;
	}

	public void Clear()
	{
		GameObject symbolObject = null;

		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolTransforms[i] = null;
				symbolImages[i] = null;
				symbolObject = symbolObjects[i];
				if( symbolObject != null )
				{
					Destroy( symbolObject );
					symbolObjects[i] = null;
				}
			}
			symbolWidths = null;
			symbolTransforms = null;
			symbolImages = null;
			symbolObjects = null;
			symbolAmount = 0;
		}
		if( fontModMaterial != null )
		{
			Destroy(fontModMaterial);
			fontModMaterial = null;
		}
		value = null;
	}

	public Vector2 GetUIDimensions()
	{
		return new Vector2( width, height );
	}

	public void ToggleAllSymbolVisuals( bool enable )
	{
		Image symbolImage = null;

		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolImage = symbolImages[i];
				if( symbolImage != null )
				{
					symbolImage.enabled = enable;
				}
			}
		}
	}

	public void ToggleSymbolVisual( int symbolIndex, bool enable )
	{
		Image symbolImage = null;

		if( symbolImages != null )
		{
			if( (symbolIndex > -1) && (symbolIndex < symbolImages.Length) )
			{
				symbolImage = symbolImages[symbolIndex];
				if( symbolImage != null )
				{
					symbolImage.enabled = enable;
				}
			}
		}
	}

	public int GetTotalSymbols()
	{
		return symbolAmount;
	}

	public string GetValue()
	{
		return value;
	}
}
