using UnityEngine;
using System.Collections;

public class ItemController : ReactionController
{
	public const float SHRINK_SPEED = 15f;
	public const float SHRINK_ACCEL = 85f;       //128.348f; for the longest shrink.   //85f; for a good fast shrink.

	public const float SCALE_CHANGE_DURATION = 0.2f;
	public const int APPEARANCE_LEVEL_FIRST = 0;
	public const int APPEARANCE_LEVEL_SECOND = 1;
	public const int APPEARANCE_LEVEL_LAST = 2;

	public ItemData itemData;
	public bool grounded;
	public float groundPenetration;
	public GameObject[] waveEmitterModels;
	public float waveEmitterExtraDepth;

	protected ChallengeStage triggerSourceComponent;
	protected PlayerAgent[] triggerComponents;
	protected PlayerAgent[] triggeringComponents;
	protected int appearanceLevel;
	protected VFXFireController fireController;
	protected int waveEmitterIndex;
	protected GameObject waveEmitterObject;
	protected SimpleAnimator waveAnimator;
	protected WaveParameters waveParams;

	private Collider2D itemCollider;
	private TwoStateAnimator itemAnimator;
	private Vector3 originalScale;
	private float shrinkSpeed;
	private float shrinkAccel;
	private bool over;
	private bool obtainable;
	private ItemData.EffectType effectType;
	private float effectSpeed;
	private float scaleChangeDuration;
	private float scaleChangeElapsed;
	private Vector3 startScale;
	private Vector3 targetScale;
	private float accumulatedEffect;

	protected override void Awake()
	{
		itemAnimator = null;
		fireController = null;
		waveEmitterIndex = -1;
		waveEmitterObject = null;
		waveParams = null;
		waveAnimator = null;
		/*halmeida - set the animator to null because the Awake of the base class will call Clear(), and
		the redefinition of Clear() in this class asks if the animator is null.*/
		base.Awake();
		triggerSourceComponent = null;
		triggerComponents = null;
		triggeringComponents = null;
		appearanceLevel = APPEARANCE_LEVEL_FIRST;
		itemCollider = GetComponent<Collider2D>();
		ExtractAnimators();
		originalScale = gameObject.transform.localScale;
		shrinkSpeed = SHRINK_SPEED;
		shrinkAccel = SHRINK_ACCEL;
		over = false;
		obtainable = false;
		effectType = ItemData.EffectType.Money;
		effectSpeed = 0f;
		if( itemData != null )
		{
			obtainable = itemData.obtainable;
			effectType = itemData.effectType;
			effectSpeed = itemData.effectValue;
		}
		scaleChangeDuration = 0f;
		scaleChangeElapsed = 0f;
		startScale = originalScale;
		targetScale = originalScale;
		accumulatedEffect = 0f;
	}

	protected virtual void ExtractAnimators()
	{
		ExtractItemAnimator();
		ExtractFireController();
		ExtractWaveAnimator( 0 );
	}

	protected virtual void ExtractItemAnimator()
	{
		itemAnimator = GetComponent<TwoStateAnimator>();
	}

	protected virtual void ExtractFireController()
	{
		fireController = GetComponent<VFXFireController>();
	}

	protected virtual void ExtractWaveAnimator( int modelIndex )
	{
		Vector3 waveEmitterLocalPosition = Vector3.zero;
		GameObject waveEmitterModel = null;

		ClearWaveAnimator();
		if( waveEmitterModels != null )
		{
			if( waveEmitterModels.Length > modelIndex )
			{
				waveEmitterModel = waveEmitterModels[modelIndex];
				if( waveEmitterModel != null )
				{
					waveEmitterObject = Instantiate( waveEmitterModel, Vector3.zero, Quaternion.identity ) as GameObject;
					if( waveEmitterObject != null )
					{
						waveAnimator = waveEmitterObject.GetComponent<SimpleAnimator>();
						if( waveAnimator != null )
						{
							waveParams = waveEmitterObject.GetComponent<WaveParameters>();
							if( waveParams != null )
							{
								waveEmitterObject.transform.SetParent( transform, false );
								waveEmitterLocalPosition.z = waveEmitterExtraDepth;
								waveEmitterObject.transform.localPosition = waveEmitterLocalPosition;
								waveEmitterIndex = modelIndex;
							}
						}
						if( waveParams == null )
						{
							ClearWaveAnimator();
						}
					}
				}
			}
		}
	}

	protected override bool SourceCompatible( GameObject candidateSource )
	{
		if( base.SourceCompatible( candidateSource ) )
		{
			triggerSourceComponent = triggerSourceObject.GetComponent<ChallengeStage>();
			if( triggerSourceComponent != null )
			{
				return true;
			}
		}
		return false;
	}

	protected override void ExtractTriggers()
	{
		PlayerAgent playerComponent = null;
		GameObject correspondingObject = null;
		int triggers = 0;

		triggerComponents = triggerSourceComponent.GetAllPlayerComponents();
		if( triggerComponents != null )
		{
			triggers = triggerComponents.Length;
			if( triggers > 0 )
			{
				triggerObjects = new GameObject[triggers];
				for( int i=0; i<triggers; i++ )
				{
					correspondingObject = null;
					playerComponent = triggerComponents[i];
					if( playerComponent != null )
					{
						correspondingObject = playerComponent.gameObject;
					}
					triggerObjects[i] = correspondingObject;
				}
			}
			else
			{
				triggerComponents = null;
			}
		}
	}

	/*halmeida - this function should only return an index if that index is valid across all relevant arrays,
	i.e., all objects and components related to that index are valid and not null.*/
	protected override int GetTriggerIndex( GameObject possibleTrigger )
	{
		int triggerIndex = -1;
		PlayerAgent triggerComponent = null;

		triggerIndex = base.GetTriggerIndex( possibleTrigger );
		if( (triggerIndex > -1) && (triggerComponents != null) )
		{
			if( triggerIndex < triggerComponents.Length )
			{
				triggerComponent = triggerComponents[triggerIndex];
				if( triggerComponent != null )
				{
					if( !triggerComponent.IsDead() )
					{
						return triggerIndex;
					}
				}
			}
		}
		return -1;
	}

	/*halmeida - this function receives an index that is valid across all relevant arrays and denotes
	an element that is not null across all relevant arrays.*/
	protected override void Trigger( int triggerIndex )
	{
		PlayerAgent playerComponent = null;

		playerComponent = triggerComponents[triggerIndex];
		if( !continuousEffect )
		{
			if( obtainable )
			{
				playerComponent.ObtainItem( itemData );
			}
			if( itemCollider != null )
			{
				itemCollider.enabled = false;
			}
			/*halmeida - to make the item disappear, we make its scale change. This change occurs from the
			original scale into a scale that makes the object hard to see. If the object is already going
			through a scale change, we immediately end that change to make it possible for the disappearance
			change to happen correctly.*/
			if( scaleChangeDuration != 0f )
			{
				scaleChangeElapsed = 0f;
				scaleChangeDuration = 0f;
				startScale = targetScale;
				gameObject.transform.localScale = startScale;
			}
		}
		else
		{
			ActivateAnimators();
		}
		base.Trigger( triggerIndex );
	}

	protected virtual void ActivateAnimators()
	{
		ActivateItemAnimator();
		ActivateFireController();
		ExtractWaveAnimator( 1 );
	}

	protected virtual void ActivateItemAnimator()
	{
		if( itemAnimator != null )
		{
			itemAnimator.SetActive( true );
		}
	}

	protected virtual void ActivateFireController()
	{
		if( fireController != null )
		{
			fireController.SetActive( true );
		}
	}

	protected override void AddTriggeringObject( int triggerIndex )
	{
		base.AddTriggeringObject( triggerIndex );
		UsefulFunctions.IncreaseArray<PlayerAgent>( ref triggeringComponents, triggerComponents[triggerIndex] );
	}

	protected override void RemoveTriggeringObject( int triggeringIndex )
	{
		UsefulFunctions.DecreaseArray<PlayerAgent>( ref triggeringComponents, triggeringIndex );
		base.RemoveTriggeringObject( triggeringIndex );
	}

	protected override void Untrigger()
	{
		DeactivateAnimators();
		base.Untrigger();
	}

	protected virtual void DeactivateAnimators()
	{
		DeactivateItemAnimator();
		DeactivateFireController();
		ExtractWaveAnimator( 0 );
	}

	protected virtual void DeactivateItemAnimator()
	{
		if( itemAnimator != null )
		{
			itemAnimator.SetActive( false );
		}
	}

	protected virtual void DeactivateFireController()
	{
		if( fireController != null )
		{
			fireController.SetActive( false );
		}
	}

	public override void Clear()
	{
		itemCollider = null;
		accumulatedEffect = 0f;
		ClearAnimators();
		base.Clear();
	}

	protected virtual void ClearAnimators()
	{
		ClearItemAnimator();
		ClearFireController();
		ClearWaveAnimator();
	}

	protected virtual void ClearItemAnimator()
	{
		if( itemAnimator != null )
		{
			itemAnimator.Clear();
			itemAnimator = null;
		}
	}

	protected virtual void ClearFireController()
	{
		if( fireController != null )
		{
			fireController.Clear();
			fireController = null;
		}
	}

	protected virtual void ClearWaveAnimator()
	{
		waveParams = null;
		if( waveAnimator != null )
		{
			waveAnimator.Clear();
			waveAnimator = null;
		}
		if( waveEmitterObject != null )
		{
			Destroy( waveEmitterObject );
			waveEmitterObject = null;
		}
		waveEmitterIndex = -1;
	}

	protected override void ClearTriggers()
	{
		triggerSourceComponent = null;
		triggerComponents = null;
		triggeringComponents = null;
		base.ClearTriggers();
	}

	public bool IsOver()
	{
		return over;
	}

	public void RequestScaleChange( Vector3 scaleChangeFactors, bool fromOriginal )
	{
		if( continuousEffect || !triggered )
		{
			/*halmeida - we only accept scale change requests if the item is of continuous effect or if
			it hasn't yet been triggered, because when an item isn't of continuous effect, it goes through
			a particular scale change when it is triggered, and that scale change should not be messed with.*/
			if( scaleChangeDuration != 0f )
			{
				/*halmeida - if a scale change was already happening, we end it immediately to
				allow the requested scale change to proceed correctly.*/
				startScale = targetScale;
				gameObject.transform.localScale = startScale;
			}
			if( fromOriginal )
			{
				targetScale = Vector3.Scale( originalScale, scaleChangeFactors );
			}
			else
			{
				targetScale = Vector3.Scale( startScale, scaleChangeFactors );
			}
			scaleChangeDuration = SCALE_CHANGE_DURATION;
			scaleChangeElapsed = 0f;
		}
	}

	public virtual bool EvolveAppearance( float changeFactor )
	{
		if( appearanceLevel < APPEARANCE_LEVEL_LAST )
		{
			appearanceLevel++;
			return true;
		}
		return false;
	}

	public virtual void Progress( float timeStep )
	{
		Vector3 scale = Vector3.one;

		ProgressAnimators( timeStep );
		if( triggered )
		{
			if( !continuousEffect )
			{
				if( !over )
				{
					scale = gameObject.transform.localScale;
					scale.x += startScale.x * shrinkSpeed * timeStep;
					scale.y -= startScale.y * shrinkSpeed * timeStep;
					if( scale.y <= 0f )
					{
						over = true;
					}
					else
					{
						gameObject.transform.localScale = scale;
						/*halmeida - the shrink process should start fast and end slow.*/
						shrinkSpeed -= shrinkAccel * timeStep;
						if( shrinkSpeed <= 0f )
						{
							over = true;
						}
					}
				}
			}
			else
			{
				if( triggeringComponents != null )
				{
					for( int i=0; i<triggeringComponents.Length; i++ )
					{
						ApplyContinuousEffect( triggeringComponents[i], timeStep );
					}
				}
			}
		}
		if( continuousEffect || !triggered )
		{
			if( scaleChangeDuration != 0f )
			{
				scaleChangeElapsed += timeStep;
				if( scaleChangeElapsed > scaleChangeDuration )
				{
					scaleChangeElapsed = scaleChangeDuration;
				}
				scale = Vector3.Lerp( startScale, targetScale, scaleChangeElapsed / scaleChangeDuration );
				gameObject.transform.localScale = scale;
				if( scaleChangeElapsed == scaleChangeDuration )
				{
					scaleChangeElapsed = 0f;
					scaleChangeDuration = 0f;
					startScale = targetScale;
				}
			}
		}
	}

	protected virtual void ProgressAnimators( float timeStep )
	{
		Vector3 waveSource = Vector3.zero;

		ProgressItemAnimator( timeStep );
		ProgressFireController( timeStep );
		ProgressWaveAnimator( timeStep );
	}

	protected virtual void ProgressItemAnimator( float timeStep )
	{
		if( itemAnimator != null )
		{
			itemAnimator.Progress( timeStep );
		}
	}

	protected virtual void ProgressFireController( float timeStep )
	{
		if( fireController != null )
		{
			fireController.Progress( timeStep );
		}
	}

	protected virtual void ProgressWaveAnimator( float timeStep )
	{
		if( (waveAnimator != null) && (waveParams != null) )
		{
			if( !waveAnimator.HasRunningWave() )
			{
				waveAnimator.StartWave( waveParams.waveColor, waveAnimator.gameObject.transform.position, waveParams.waveLengths,
					waveParams.waveAlphas, waveParams.waveFrontLimit, waveParams.waveSpeed, waveParams.waveLengthVariation, 1f, 0f,
					waveParams.waveNoise, waveParams.waveNoiseScale, waveParams.waveDelay );
			}
			waveAnimator.Progress( timeStep );
		}
	}

	protected virtual void ApplyContinuousEffect( PlayerAgent triggeringComponent, float timeStep )
	{
		int applicableEffect = 0;

		if( triggeringComponent != null )
		{
			switch( effectType )
			{
				case ItemData.EffectType.HP:
					accumulatedEffect += timeStep * effectSpeed;
					applicableEffect = (int)accumulatedEffect;
					if( applicableEffect != 0 )
					{
						accumulatedEffect -= applicableEffect;
						triggeringComponent.AddToCurrentHP( applicableEffect, false );
					}
					break;
			}
		}
	}

	public virtual void CorrectDepthChain()
	{
		if( itemAnimator != null )
		{
			itemAnimator.StartDepthChain( gameObject.transform.position.z, true );
		}
	}

	protected virtual void OnTriggerEnter2D( Collider2D otherCollider )
	{
		//Debug.Log("Debug : ItemController : on trigger enter.");
		TryToTrigger( otherCollider.gameObject );
	}

	protected virtual void OnTriggerExit2D( Collider2D otherCollider )
	{
		TryToUntrigger( otherCollider.gameObject );
	}

	public virtual Rect GetLocalCollisionRect()
	{
		Rect collisionRect = new Rect( 0f, 0f, 0f, 0f );
		SpriteRenderer renderer = null;
		Sprite currentSprite = null;
		Bounds spriteBounds = default( Bounds );

		renderer = GetComponent<SpriteRenderer>();
		if( renderer != null )
		{
			currentSprite = renderer.sprite;
			if( currentSprite != null )
			{
				spriteBounds = currentSprite.bounds;
				/*halmeida - the rectangle will be bottom-left anchored and centered at (0, 0).*/
				collisionRect.x = -spriteBounds.extents.x;
				collisionRect.y = -spriteBounds.extents.y;
				collisionRect.width = spriteBounds.size.x;
				collisionRect.height = spriteBounds.size.y;
			}
		}
		return collisionRect;
	}
}
