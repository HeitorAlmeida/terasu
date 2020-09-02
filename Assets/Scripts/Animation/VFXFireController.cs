using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXFireController : MonoBehaviour
{
	public const float DEFAULT_FLAME_CREATION_WINDOW = 0.3f;
	public const float DEFAULT_FLAME_CREATION_WIDTH = 1f;
	public const float DEFAULT_FLAME_CREATION_HEIGHT = 1f;
	public const float DEFAULT_FLAME_CHANCE = 1f;
	public const int DEFAULT_FLAMES_PER_WINDOW = 1;

	public const float AURA_SCALE_INCREASE = 1.5f;
	public const float AURA_ALPHA_REDUCTION = 0.25f;

	public GameObject[] flamePieceModels;
	public float[] flamePieceChances;
	public float flamePieceAlpha;
	public float creationTimeWindowInactive;
	public float creationTimeWindowActive;
	public int creationsPerWindowInactive;
	public int creationsPerWindowActive;
	public Vector2 fuelAreaCenter;
	public Vector2 fuelAreaSize;
	public Vector2 fuelAreaSpread;
	public Vector4 minimumColorAddition;
	public Vector4 maximumColorAddition;
	public GameObject[] backAuraModels;
	public int defaultMaxFlamePieces;
	public float additionalDepth;

	private GameObject[] flameModels;
	private float[] flameChances;
	private float flameChancesSum;
	private GameObject[] flameObjects;
	private SimpleAnimator[] flameAnimators;
	private Rect[] creationRects;
	private bool active;
	private float windowDuration;
	private float windowDurationInactive;
	private float windowDurationActive;
	private int windowFlames;
	private int windowFlamesInactive;
	private int windowFlamesActive;
	private float windowElapsed;
	private float[] windowTriggers;
	private bool[] flamesCreated;
	private float flameDepth;
	private GameObject[] auraObjects;
	private SimpleAnimator[] auraAnimators;
	private int maxSimultaneousFlames;
	private GameObject flamesParent;
	private GameObject aurasParent;

	void Awake()
	{
		ValidateFuelArea();
		defaultMaxFlamePieces = (defaultMaxFlamePieces < 0) ? -defaultMaxFlamePieces : defaultMaxFlamePieces;
		flameModels = null;
		flameChances = null;
		flameChancesSum = 0f;
		ValidateFlameModels();
		flameObjects = null;
		flameAnimators = null;
		creationRects = null;
		DefineCreationAreas();
		windowDuration = 0f;
		windowDurationInactive = creationTimeWindowInactive;
		windowDurationActive = creationTimeWindowActive;
		windowFlames = 0;
		windowFlamesInactive = creationsPerWindowInactive;
		windowFlamesActive = creationsPerWindowActive;
		SetActive( false, false );
		flameDepth = DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / BaseAnimator.MAX_ANIMATORS_WITHIN_LAYER;
		auraObjects = null;
		auraAnimators = null;
		maxSimultaneousFlames = defaultMaxFlamePieces;
		flamesParent = new GameObject("VFXFireFlamesParent");
		if( flamesParent != null )
		{
			flamesParent.transform.position = Vector3.zero;
			flamesParent.transform.rotation = Quaternion.identity;
			flamesParent.transform.localScale = Vector3.one;
		}
		aurasParent = new GameObject("VFXFireAurasParent");
		if( aurasParent != null )
		{
			aurasParent.transform.SetParent( transform, false );
			/*halmeida - since every flame piece will be at flameDepth, I double that depth to make
			sure the auras are behind them.*/
			aurasParent.transform.localPosition = new Vector3( fuelAreaCenter.x, fuelAreaCenter.y, additionalDepth + 2f * flameDepth );
			aurasParent.transform.localScale = Vector3.one;
		}
		CreateAuras();
	}

	private void ValidateFuelArea()
	{
		fuelAreaSize.x = (fuelAreaSize.x > 0f) ? fuelAreaSize.x : DEFAULT_FLAME_CREATION_WIDTH;
		fuelAreaSize.y = (fuelAreaSize.y > 0f) ? fuelAreaSize.y : DEFAULT_FLAME_CREATION_HEIGHT;
		fuelAreaSpread.x = (fuelAreaSpread.x < 0f) ? 0f : fuelAreaSpread.x;
		fuelAreaSpread.y = (fuelAreaSpread.y < 0f) ? 0f : fuelAreaSpread.y;
	}

	private void ValidateFlameModels()
	{
		GameObject model = null;
		SimpleAnimator modelAnimator = null;
		float modelChance = 0f;

		if( flamePieceModels != null )
		{
			for( int i=0; i<flamePieceModels.Length; i++ )
			{
				model = flamePieceModels[i];
				if( model != null )
				{
					modelAnimator = model.GetComponent<SimpleAnimator>();
					if( modelAnimator != null )
					{
						UsefulFunctions.IncreaseArray<GameObject>( ref flameModels, model );
						modelChance = DEFAULT_FLAME_CHANCE;
						if( flamePieceChances != null )
						{
							if( flamePieceChances.Length > i )
							{
								modelChance = flamePieceChances[i];
								modelChance = (modelChance > 0f) ? modelChance : DEFAULT_FLAME_CHANCE;
							}
						}
						UsefulFunctions.IncreaseArray<float>( ref flameChances, modelChance );
						flameChancesSum += modelChance;
					}
				}
			}
		}
	}

	private void DefineCreationAreas()
	{
		Rect creationRect = default( Rect );

		creationRects = new Rect[3];
		/*halmeida - all creation area rectangles are top-left anchored.*/
		/*halmeida - the first creation area is just the fuel area.*/
		creationRect.width = fuelAreaSize.x;
		creationRect.height = fuelAreaSize.y;
		creationRect.x = fuelAreaCenter.x - creationRect.width / 2f;
		creationRect.y = fuelAreaCenter.y + creationRect.height / 2f;
		creationRects[0] = creationRect;
		/*halmeida - the second creation area is larger horizontally.*/
		creationRect.width = fuelAreaSize.x + fuelAreaSpread.x;
		creationRect.height = fuelAreaSize.y;
		creationRect.x = fuelAreaCenter.x - creationRect.width / 2f;
		creationRect.y = fuelAreaCenter.y + creationRect.height / 2f;
		creationRects[1] = creationRect;
		/*halmeida - the third creation area is larger vertically.*/
		creationRect.width = fuelAreaSize.x;
		creationRect.height = fuelAreaSize.y + fuelAreaSpread.y;
		creationRect.x = fuelAreaCenter.x - creationRect.width / 2f;
		creationRect.y = fuelAreaCenter.y + creationRect.height / 2f;
		creationRects[2] = creationRect;
	}

	public void SetActive( bool newActiveStatus, bool evaluatePreviousStatus = true )
	{
		bool previouslyActive = false;

		previouslyActive = active;
		active = newActiveStatus;
		if( active )
		{
			if( !evaluatePreviousStatus || (evaluatePreviousStatus && !previouslyActive) )
			{
				SetWindowDuration( windowDurationActive, windowFlamesActive );
			}
		}
		else
		{
			if( !evaluatePreviousStatus || (evaluatePreviousStatus && previouslyActive) )
			{
				SetWindowDuration( windowDurationInactive, windowFlamesInactive );
			}
		}
	}

	public void SetWindowDuration( float newWindowDuration, int newWindowFlames )
	{
		windowDuration = (newWindowDuration > 0f) ? newWindowDuration : DEFAULT_FLAME_CREATION_WINDOW;
		windowFlames = (newWindowFlames > 0f) ? newWindowFlames : DEFAULT_FLAMES_PER_WINDOW;
		windowTriggers = null;
		flamesCreated = null;
		if( windowFlames > 0 )
		{
			windowTriggers = new float[windowFlames];
			flamesCreated = new bool[windowFlames];
		}
		ResetFlameCreation();
	}

	public float GetActualWindowDuration()
	{
		return windowDuration;
	}

	public int GetActualWindowFlameAmount()
	{
		return windowFlames;
	}

	private void ResetFlameCreation()
	{
		windowElapsed = 0;
		if( windowTriggers != null )
		{
			for( int i=0; i<windowTriggers.Length; i++ )
			{
				windowTriggers[i] = Random.Range( 0f, windowDuration );
				//windowTriggers[i] = i * windowDuration / windowTriggers.Length;
				flamesCreated[i] = false;
			}
		}
	}

	private void CreateAuras()
	{
		GameObject auraModel = null;
		GameObject auraObject = null;
		SimpleAnimator auraAnimator = null;

		if( (backAuraModels != null) && (auraObjects == null) )
		{
			for( int i=0; i<backAuraModels.Length; i++ )
			{
				auraModel = backAuraModels[i];
				if( auraModel != null )
				{
					auraAnimator = auraModel.GetComponent<SimpleAnimator>();
					if( auraAnimator != null )
					{
						auraObject = Instantiate( auraModel, Vector3.zero, Quaternion.identity ) as GameObject;
						if( auraObject != null )
						{
							auraAnimator = auraObject.GetComponent<SimpleAnimator>();
							UsefulFunctions.IncreaseArray<GameObject>( ref auraObjects, auraObject );
							UsefulFunctions.IncreaseArray<SimpleAnimator>( ref auraAnimators, auraAnimator );
							if( aurasParent != null )
							{
								auraObject.transform.SetParent( aurasParent.transform, false );
							}
							else
							{
								auraObject.transform.SetParent( transform, false );
							}
							auraObject.transform.localPosition = Vector3.zero;
							auraAnimator.SetOpaqueAlpha( 0.1f );
						}
					}
				}
			}
		}
	}

	public void Progress( float timeStep )
	{
		ProgressFlames( timeStep );
		ProgressFlameCreation( timeStep );
		ProgressAuras( timeStep );
	}

	private void ProgressFlames( float timeStep )
	{
		SimpleAnimator flameAnimator = null;

		if( flameAnimators != null )
		{
			for( int i=0; i<flameAnimators.Length; i++ )
			{
				flameAnimator = flameAnimators[i];
				flameAnimator.Progress( timeStep );
				if( flameAnimator.IsOver() )
				{
					RemoveFlame(i);
					if( flameAnimators == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
			}
		}
	}

	private void ProgressFlameCreation( float timeStep )
	{
		Vector3 flamePosition = Vector3.zero;
		Vector2 offsetWithinRect = Vector2.zero;
		float windowTrigger = 0f;
		bool flameCreated = false;
		int creationRectIndex = -1;
		Rect creationRect = default( Rect );

		if( flameModels != null )
		{
			windowElapsed += timeStep;
			if( windowTriggers != null )
			{
				for( int i=0; i<windowTriggers.Length; i++ )
				{
					windowTrigger = windowTriggers[i];
					flameCreated = flamesCreated[i];
					if( !flameCreated && (windowElapsed > windowTrigger) )
					{
						if( creationRects != null )
						{
							creationRectIndex = i % creationRects.Length;
							creationRect = creationRects[creationRectIndex];
							flamePosition = transform.position;
							flamePosition.x += creationRect.x;
							flamePosition.y += creationRect.y;
							offsetWithinRect.x = Random.Range( 0f, creationRect.width );
							offsetWithinRect.y = Random.Range( 0f, creationRect.height );
							flamePosition.x += offsetWithinRect.x;
							flamePosition.y -= offsetWithinRect.y;
							/*halmeida - increasing the depth of the particle to show it behind the object generating it.*/
							flamePosition.z += additionalDepth + flameDepth;
							CreateFlame( flamePosition );
							flamesCreated[i] = true;
						}
					}
				}
			}
			if( windowElapsed >= windowDuration )
			{
				ResetFlameCreation();
			}
		}
	}

	private void ProgressAuras( float timeStep )
	{
		int currentFlames = 0;
		float scale = 1f;
		float alpha = 0f;
		GameObject auraObject = null;
		SimpleAnimator auraAnimator = null;
		float scaleIncrease = 0f;
		float alphaReduction = 0f;

		if( auraAnimators != null )
		{
			if( flameObjects != null )
			{
				maxSimultaneousFlames = (flameObjects.Length > maxSimultaneousFlames) ? flameObjects.Length : maxSimultaneousFlames;
				currentFlames = flameObjects.Length;
			}
			if( maxSimultaneousFlames > 0 )
			{
				scaleIncrease = AURA_SCALE_INCREASE * (float)maxSimultaneousFlames;
				scale = ((float)currentFlames + scaleIncrease) / ((float)maxSimultaneousFlames + scaleIncrease);
				alphaReduction = AURA_ALPHA_REDUCTION * (float)maxSimultaneousFlames;
				alpha = ((float)currentFlames - alphaReduction) / ((float)maxSimultaneousFlames - alphaReduction);
			}
			for( int i=0; i<auraObjects.Length; i++ )
			{
				auraObject = auraObjects[i];
				auraObject.transform.localScale = new Vector3( scale, scale, 1f );
			}
			for( int i=0; i<auraAnimators.Length; i++ )
			{
				auraAnimator = auraAnimators[i];
				auraAnimator.SetOpaqueAlpha( alpha );
				auraAnimator.StartAlphaFading( 1f, false, false, false );
				auraAnimator.Progress( timeStep );
			}
		}
	}

	private void CreateFlame( Vector3 flamePosition )
	{
		float chanceValue = 0f;
		int modelIndex = -1;
		GameObject flameModel = null;
		GameObject flameObject = null;
		SimpleAnimator flameAnimator = null;

		if( flameModels != null )
		{
			chanceValue = Random.Range( 0f, flameChancesSum );
			for( int i=0; i<flameChances.Length; i++ )
			{
				if( chanceValue > flameChances[i] )
				{
					chanceValue -= flameChances[i];
				}
				else
				{
					modelIndex = i;
					break;
				}
			}
			flameModel = flameModels[modelIndex];
			flameObject = Instantiate( flameModel, Vector3.zero, Quaternion.identity ) as GameObject;
			if( flameObject != null )
			{
				if( flamesParent != null )
				{
					flameObject.transform.SetParent( flamesParent.transform, false );
				}
				flameObject.transform.localPosition = flamePosition;
				flameAnimator = flameObject.GetComponent<SimpleAnimator>();
				flameAnimator.SetOpaqueAlpha( flamePieceAlpha );
				flameAnimator.SetColorAddition( Vector4.Lerp( minimumColorAddition, maximumColorAddition, Random.Range(0f, 1f) ) );
				if( Random.Range( 0, 2 ) == 1 )
				{
					flameAnimator.RequestFlip();
				}
				UsefulFunctions.IncreaseArray<GameObject>( ref flameObjects, flameObject );
				UsefulFunctions.IncreaseArray<SimpleAnimator>( ref flameAnimators, flameAnimator );
			}
		}
	}

	private void RemoveFlame( int flameIndex )
	{
		if( flameAnimators != null )
		{
			if( (flameIndex > -1) && (flameIndex < flameAnimators.Length) )
			{
				/*halmeida - no need to check if animator is null cause it never will be.*/
				flameAnimators[flameIndex].Clear();
				flameAnimators[flameIndex] = null;
				/*halmeida - relying on the coherence of all the "particle" arrays.*/
				Destroy( flameObjects[flameIndex] );
				flameObjects[flameIndex] = null;
				UsefulFunctions.DecreaseArray<SimpleAnimator>( ref flameAnimators, flameIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref flameObjects, flameIndex );
			}
		}
	}

	public void Clear()
	{
		if( flameAnimators != null )
		{
			for( int i=0; i<flameAnimators.Length; i++ )
			{
				flameAnimators[i].Clear();
				flameAnimators[i] = null;
				Destroy( flameObjects[i] );
				flameObjects[i] = null;
			}
			flameAnimators = null;
			flameObjects = null;
		}
		flameModels = null;
		flameChances = null;
		creationRects = null;
		windowTriggers = null;
		flamesCreated = null;
		if( auraAnimators != null )
		{
			for( int i=0; i<auraAnimators.Length; i++ )
			{
				auraAnimators[i].Clear();
				auraAnimators[i] = null;
				Destroy( auraObjects[i] );
				auraObjects[i] = null;
			}
			auraAnimators = null;
			auraObjects = null;
		}
		maxSimultaneousFlames = 0;
		if( flamesParent != null )
		{
			Destroy( flamesParent );
			flamesParent = null;
		}
		if( aurasParent != null )
		{
			Destroy( aurasParent );
			aurasParent = null;
		}
	}
}
