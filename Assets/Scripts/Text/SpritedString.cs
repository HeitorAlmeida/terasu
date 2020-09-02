using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritedString : MonoBehaviour
{
	private SymbolDatabase spriteSource;
	private int symbolAmount;
	private GameObject[] symbolObjects;
	private SpriteRenderer[] symbolRenderers;
	private SMBlackToColor[] symbolSMs;
	private Transform[] symbolTransforms;
	private float[] symbolWidths;
	private string value;
	private float width;
	private float height;
	private Material fontModMaterial;

	void Awake()
	{
		spriteSource = null;
		symbolAmount = 0;
		symbolObjects = null;
		symbolRenderers = null;
		symbolSMs = null;
		symbolTransforms = null;
		symbolWidths = null;
		value = null;
		width = 0f;
		height = 0f;
		fontModMaterial = null;
	}

	public void SetSymbolSource( SymbolDatabase newSpriteSource )
	{
		spriteSource = newSpriteSource;
		if( spriteSource != null )
		{
			fontModMaterial = spriteSource.fontModMaterial;
		}
	}

	public void SetValue( string newValue )
	{
		value = newValue;
		UpdateSymbolAmount();
		UpdateSymbolLooks();
		UpdateSymbolPositions();
	}

	private void UpdateSymbolAmount()
	{
		int newSymbolAmount = 0;
		int additionalSymbols = 0;
		GameObject[] newSymbolObjects = null;
		SpriteRenderer[] newSymbolRenderers = null;
		SMBlackToColor[] newSymbolSMs = null;
		Transform[] newSymbolTransforms = null;
		float[] newSymbolWidths = null;
		GameObject newSymbolObject = null;
		SpriteRenderer newSymbolRenderer = null;
		SMBlackToColor newSymbolSM = null;
		Transform newSymbolTransform = null;
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
				newSymbolRenderers = new SpriteRenderer[additionalSymbols];
				newSymbolSMs = new SMBlackToColor[additionalSymbols];
				newSymbolTransforms = new Transform[additionalSymbols];
				newSymbolWidths = new float[additionalSymbols];
				for( int i=0; i<additionalSymbols; i++ )
				{
					newSymbolObject = new GameObject("CharacterSymbol"+(symbolAmount+i));
					newSymbolRenderer = newSymbolObject.AddComponent<SpriteRenderer>();
					/*halmeida - the mere assignment of a material to the material property of a renderer does not cause material
					instantiation. It's just a pointer assignment. If we were to assign a material from the material property of
					a renderer, however, a clone of the material would be created.*/
					if( fontModMaterial != null )
					{
						newSymbolRenderer.material = fontModMaterial;
					}
					//Debug.Log("Debug : SpritedString : Materials before shader = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
					newSymbolSM = newSymbolObject.AddComponent<SMBlackToColor>();
					//Debug.Log("Debug : SpritedString : Materials after shader = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
					/*halmeida - the awake of this component does a material assignment from the renderer's material property,
					causing a material clone to be created. That's why we need to call the clear function of the component later.*/
					newSymbolTransform = newSymbolObject.transform;
					if( newSymbolTransform != null )
					{
						newSymbolTransform.SetParent( gameObject.transform, false );
					}
					newSymbolObjects[i] = newSymbolObject;
					newSymbolRenderers[i] = newSymbolRenderer;
					newSymbolSMs[i] = newSymbolSM;
					newSymbolTransforms[i] = newSymbolTransform;
					newSymbolWidths[i] = newSymbolWidth;
				}
				UsefulFunctions.IncreaseArrayWithArray<GameObject>( ref symbolObjects, newSymbolObjects );
				UsefulFunctions.IncreaseArrayWithArray<SpriteRenderer>( ref symbolRenderers, newSymbolRenderers );
				UsefulFunctions.IncreaseArrayWithArray<SMBlackToColor>( ref symbolSMs, newSymbolSMs );
				UsefulFunctions.IncreaseArrayWithArray<Transform>( ref symbolTransforms, newSymbolTransforms );
				UsefulFunctions.IncreaseArrayWithArray<float>( ref symbolWidths, newSymbolWidths );
				newSymbolObjects = null;
				newSymbolRenderers = null;
				newSymbolSMs = null;
				newSymbolTransforms = null;
				newSymbolWidths = null;
			}
			else
			{
				if( (symbolObjects != null) && (symbolRenderers != null) && (symbolTransforms != null) && (symbolWidths != null)
					&& (symbolSMs != null) )
				{
					if( (symbolObjects.Length == symbolAmount) && (symbolRenderers.Length == symbolAmount) &&
						(symbolTransforms.Length == symbolAmount) && (symbolWidths.Length == symbolAmount) &&
						(symbolSMs.Length == symbolAmount) )
					{
						for( int i=newSymbolAmount; i<symbolAmount; i++ )
						{
							symbolTransforms[i] = null;
							newSymbolSM = symbolSMs[i];
							if( newSymbolSM != null )
							{
								newSymbolSM.Clear();
								symbolSMs[i] = null;
							}
							symbolRenderers[i] = null;
							newSymbolObject = symbolObjects[i];
							if( newSymbolObject != null )
							{
								Destroy( newSymbolObject );
								symbolObjects[i] = null;
							}
						}
						newSymbolObjects = new GameObject[newSymbolAmount];
						newSymbolRenderers = new SpriteRenderer[newSymbolAmount];
						newSymbolSMs = new SMBlackToColor[newSymbolAmount];
						newSymbolTransforms = new Transform[newSymbolAmount];
						newSymbolWidths = new float[newSymbolAmount];
						for( int i=0; i<newSymbolAmount; i++ )
						{
							newSymbolObjects[i] = symbolObjects[i];
							newSymbolRenderers[i] = symbolRenderers[i];
							newSymbolSMs[i] = symbolSMs[i];
							newSymbolTransforms[i] = symbolTransforms[i];
							newSymbolWidths[i] = symbolWidths[i];
						}
						symbolObjects = newSymbolObjects;
						symbolRenderers = newSymbolRenderers;
						symbolSMs = newSymbolSMs;
						symbolTransforms = newSymbolTransforms;
						symbolWidths = newSymbolWidths;
						newSymbolObjects = null;
						newSymbolRenderers = null;
						newSymbolSMs = null;
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
		SpriteRenderer symbolRenderer = null;
		Vector3 spriteSize = Vector3.zero;

		width = 0f;
		height = 0f;
		if( (spriteSource != null) && (symbolAmount > 0) )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				character = value[i];
				symbolSprite = spriteSource.GetSymbolSprite( character );
				symbolRenderer = symbolRenderers[i];
				if( symbolRenderer != null )
				{
					symbolRenderer.sprite = symbolSprite;
					if( symbolSprite != null )
					{
						spriteSize = symbolSprite.bounds.size;
						symbolWidths[i] = spriteSize.x;
						width += spriteSize.x;
						if( spriteSize.y > height )
						{
							height = spriteSize.y;
						}
					}
				}
			}
		}
	}

	private void UpdateSymbolPositions()
	{
		float currentOffsetX = 0f;
		Transform symbolTransform = null;
		float symbolWidth = 0f;

		currentOffsetX = -width / 2f;
		for( int i=0; i<symbolAmount; i++ )
		{
			symbolTransform = symbolTransforms[i];
			if( symbolTransform != null )
			{
				symbolWidth = symbolWidths[i];
				currentOffsetX += symbolWidth / 2f;
				symbolTransform.localPosition = new Vector3( currentOffsetX, 0f, 0f );
				currentOffsetX += symbolWidth / 2f;
			}
		}
	}

	public void SetColor( Color newColor )
	{
		SMBlackToColor symbolSM = null;

		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolSM = symbolSMs[i];
				if( symbolSM != null )
				{
					symbolSM.SetBlackReplacementColor( newColor );
				}
			}
		}
	}

	public void Clear()
	{
		GameObject symbolObject = null;
		SMBlackToColor symbolSM = null;

		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolTransforms[i] = null;
				symbolRenderers[i] = null;
				symbolSM = symbolSMs[i];
				if( symbolSM != null )
				{
					symbolSM.Clear();
					symbolSMs[i] = null;
				}
				symbolObject = symbolObjects[i];
				if( symbolObject != null )
				{
					Destroy( symbolObject );
					symbolObjects[i] = null;
				}
			}
			symbolWidths = null;
			symbolTransforms = null;
			symbolRenderers = null;
			symbolSMs = null;
			symbolObjects = null;
			symbolAmount = 0;
		}
		value = null;
	}

	public Vector2 GetWorldDimensions()
	{
		return new Vector2( width, height );
	}

	public void ToggleAllSymbolVisuals( bool enable )
	{
		SpriteRenderer symbolRenderer = null;

		if( symbolAmount > 0 )
		{
			for( int i=0; i<symbolAmount; i++ )
			{
				symbolRenderer = symbolRenderers[i];
				if( symbolRenderer != null )
				{
					symbolRenderer.enabled = enable;
				}
			}
		}
	}

	public void ToggleSymbolVisual( int symbolIndex, bool enable )
	{
		SpriteRenderer symbolRenderer = null;

		if( symbolRenderers != null )
		{
			if( (symbolIndex > -1) && (symbolIndex < symbolRenderers.Length) )
			{
				symbolRenderer = symbolRenderers[symbolIndex];
				if( symbolRenderer != null )
				{
					symbolRenderer.enabled = enable;
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
