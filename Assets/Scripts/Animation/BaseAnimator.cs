using UnityEngine;
using System.Collections;

public class BaseAnimator : MonoBehaviour
{
	public const int INVALID_STATUS = -1;

	public const int MAX_ANIMATORS_WITHIN_LAYER = 100;

	public const bool SIDE_RIGHT = true;
	public const bool SIDE_LEFT = false;

	protected GameObject[] sequenceObjects;
	protected int totalSequences;
	protected int sequenceIndex;
	protected Sprite[] sprites;
	protected Sprite sprite;
	protected Sprite[] spriteNormals;
	protected Sprite spriteNormal;
	protected Vector2[] focusOffsetsOne;
	protected Vector2[] focusOffsetsTwo;
	protected Vector2 focusOffsetOne;
	protected Vector2 focusOffsetTwo;
	protected bool[] focusOneValidities;
	protected bool[] focusTwoValidities;
	protected bool focusOneValidity;
	protected bool focusTwoValidity;
	protected int spriteIndex;
	protected float spriteElapsed;
	protected bool spriteFlip;
	protected float spriteWidth;
	protected int[] firstIndexPerSequence;
	protected int[] lastIndexPerSequence;
	protected float[] speedPerSequence;
	protected bool[] loopPerSequence;
	protected FakeLight[] fakeLightPerSequence;
	protected int firstIndex;
	protected int lastIndex;
	protected float spriteDuration;
	protected bool loop;
	protected FakeLight fakeLight;
	protected bool keepSequenceProgress;
	protected int status;
	protected int previousStatus;
	protected bool over;
	protected VFXParticleController particleController;
	protected GameObject childObjectBack;
	protected GameObject childObjectFront;
	protected Transform childTransformBack;
	protected Transform childTransformFront;
	protected BaseAnimator childAnimatorBack;
	protected BaseAnimator childAnimatorFront;
	protected float childSeparation;
	protected bool forceSpriteFeeding;
	protected float fadeSpeed;
	protected bool fadeImmediately;
	protected bool fadedToTransparent;
	protected bool fadedToOpaque;
	protected float opaqueAlpha;
	protected float currentAlpha;
	protected GameObject[] shadowObjects;
	protected VFXAppearanceController[] shadows;
	protected bool paused;
	protected GameObject[] pausedShadowObjects;
	protected VFXAppearanceController[] pausedShadows;

	protected virtual void Awake()
	{
		particleController = null;
		childAnimatorBack = null;
		childAnimatorFront = null;
		shadowObjects = null;
		shadows = null;
		pausedShadowObjects = null;
		pausedShadows = null;
		Clear( false );
		if( ExtractRenderingComponent() )
		{
			opaqueAlpha = ExtractRenderingColor().a;
			currentAlpha = opaqueAlpha;
			fadedToOpaque = true;
			FillSequencesArray();
			if( (sequenceObjects != null) && (totalSequences > 0) )
			{
				firstIndexPerSequence = new int[totalSequences];
				lastIndexPerSequence = new int[totalSequences];
				speedPerSequence = new float[totalSequences];
				loopPerSequence = new bool[totalSequences];
				fakeLightPerSequence = new FakeLight[totalSequences];
				ExtractSpritesAndLights();
				Progress( 0f );
			}
		}
		/*halmeida - I wil extract the associated particle controller but I will not yet set this animator
		as that particle controller's focus provider, cause the awake on that particle controller may still
		be called causing it to lose the reference to this script.*/
		particleController = gameObject.GetComponent<VFXParticleController>();
		/*halmeida - Every class derived from this BaseAnimator will inherit the childAnimator field as a
		BaseAnimator instance which is severely limiting. To allow subclasses to have more complex child
		animators, we allow the extraction and operations over the child to be customized.*/
		/*halmeida - if a certain child of the object should show in front of it or behind it is unknown.
		To standardize, we  will assume the first animated child to be behind and ignore the rest.*/
		RecognizeBackChildAnimator();
	}

	void Start()
	{
		if( particleController != null )
		{
			particleController.SetFocusProvider( this );
		}
	}

	public virtual void Clear( bool renderingComponentInitialized = true )
	{
		FakeLight fakeLight = null;

		if( renderingComponentInitialized )
		{
			FeedRenderingComponent( null, null, false );
		}
		ClearRenderingComponent();
		sequenceObjects = null;
		totalSequences = 0;
		sequenceIndex = -1;
		sprites = null;
		sprite = null;
		spriteNormals = null;
		spriteNormal = null;
		focusOffsetsOne = null;
		focusOffsetsTwo = null;
		focusOffsetOne = Vector2.zero;
		focusOffsetTwo = Vector2.zero;
		focusOneValidities = null;
		focusTwoValidities = null;
		focusOneValidity = false;
		focusTwoValidity = false;
		spriteIndex = -1;
		spriteElapsed = 0f;
		spriteFlip = false;
		spriteWidth = 0f;
		firstIndexPerSequence = null;
		lastIndexPerSequence = null;
		speedPerSequence = null;
		loopPerSequence = null;
		if( fakeLightPerSequence != null )
		{
			for( int i=0; i<fakeLightPerSequence.Length; i++ )
			{
				fakeLight = fakeLightPerSequence[i];
				if( fakeLight != null )
				{
					fakeLight.Clear();
					fakeLightPerSequence[i] = null;
				}
			}
		}
		fakeLightPerSequence = null;
		firstIndex = -1;
		lastIndex = -1;
		spriteDuration = 0f;
		loop = false;
		keepSequenceProgress = false;
		status = INVALID_STATUS;
		previousStatus = INVALID_STATUS;
		over = false;
		forceSpriteFeeding = false;
		fadeSpeed = 0f;
		fadeImmediately = false;
		fadedToTransparent = false;
		fadedToOpaque = false;
		opaqueAlpha = 0f;
		currentAlpha = 0f;
		paused = false;
		ClearShadows();
		if( particleController != null )
		{
			particleController.Clear();
			particleController = null;
		}
		childSeparation = DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / MAX_ANIMATORS_WITHIN_LAYER;
		childObjectBack = null;
		childObjectFront = null;
		childTransformBack = null;
		childTransformFront = null;
		ClearChildAnimators();
	}

	protected virtual void ClearRenderingComponent()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer = null;
	}

	protected virtual bool ExtractRenderingComponent()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: GetComponent<SpriteRenderer>();
		return false;
	}

	protected virtual void FeedRenderingComponent( Sprite newSprite, Sprite newSpriteNormal, bool newSpriteFlip )
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer.sprite = newSprite;
	}

	protected virtual Color ExtractRenderingColor()
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: return spriteRenderer.color;
		return Color.white;
	}

	protected virtual void FeedRenderingColor( Color newColor )
	{
		/*halmeida - the subclasses will determine their own rendering components. It can be a SpriteRenderer or
		a canvas Image, for example.*/
		//Ex.: spriteRenderer.color = newColor;
	}

	protected virtual void RecognizeBackChildAnimator()
	{
		for( int i=0; i<gameObject.transform.childCount; i++ )
		{
			childTransformBack = gameObject.transform.GetChild( i );
			if( childTransformBack != null )
			{
				childObjectBack = childTransformBack.gameObject;
				childAnimatorBack = ExtractChildAnimatorComponent( childObjectBack );
				if( childAnimatorBack != null )
				{
					return;
				}
				else
				{
					childObjectBack = null;
					childTransformBack = null;
				}
			}
		}
	}

	protected virtual BaseAnimator ExtractChildAnimatorComponent( GameObject childObject )
	{
		if( childObject != null )
		{
			return childObject.GetComponent<BaseAnimator>();
		}
		return null;
	}

	public virtual void SetChildAnimator( GameObject newChildObject, bool behindParent )
	{
		Transform newChildTransform = null;
		BaseAnimator newChildAnimator = null;

		newChildAnimator = ExtractChildAnimatorComponent( newChildObject );
		if( newChildAnimator != null )
		{
			newChildTransform = newChildObject.transform;
		}
		else
		{
			newChildObject = null;
		}
		if( behindParent )
		{
			childAnimatorBack = newChildAnimator;
			childObjectBack = newChildObject;
			childTransformBack = newChildTransform;
		}
		else
		{
			childAnimatorFront = newChildAnimator;
			childObjectFront = newChildObject;
			childTransformFront = newChildTransform;
		}
	}

	protected virtual void StartAlphaFadingChildAnimators( float newFadeSpeed, bool immediately, bool behindParent )
	{
		if( behindParent )
		{
			if( childAnimatorBack != null )
			{
				childAnimatorBack.StartAlphaFading( newFadeSpeed, immediately, true, false );
			}
		}
		else
		{
			if( childAnimatorFront != null )
			{
				childAnimatorFront.StartAlphaFading( newFadeSpeed, immediately, false, true );
			}
		}
	}

	protected virtual void ClearChildAnimators()
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.Clear();
			childAnimatorBack = null;
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.Clear();
			childAnimatorFront = null;
		}
	}

	protected virtual void FillSequencesArray()
	{
		/*halmeida - the subclasses are supposed to place their particular sequence objects into this array.*/
		sequenceObjects = null;
		totalSequences = 0;
	}

	public void Progress( float timeStep )
	{
		int oldSpriteIndex = -1;
		bool oldSpriteFlip = false;

		if( !paused )
		{
			if( !over )
			{
				oldSpriteIndex = spriteIndex;
				oldSpriteFlip = spriteFlip;
				UpdateAnimation( timeStep );
				if( !over )
				{
					if( (spriteIndex != oldSpriteIndex) || (spriteFlip != oldSpriteFlip) || forceSpriteFeeding )
					{
						FeedRenderingComponent( sprite, spriteNormal, spriteFlip );
						forceSpriteFeeding = false;
					}
					if( particleController != null )
					{
						/*halmeida - when a particle controller creates particles automatically, it creates with basis on
						sprite provided focus points. If there's no sprite, there should be no particles.*/
						if( particleController.CreatesParticlesAutomatically() )
						{
							if( sprite != null )
							{
								particleController.Progress( timeStep );
							}
						}
						else
						{
							particleController.Progress( timeStep );
						}
					}
					RecordChangeVerifiers();
				}
				else
				{
					sprite = null;
					spriteNormal = null;
					FeedRenderingComponent( sprite, spriteNormal, spriteFlip );
				}
			}
		}
		ProgressChildAnimators( timeStep );
		ProgressShadows( timeStep );
	}

	protected virtual void ProgressChildAnimators( float timeStep )
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.Progress( timeStep );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.Progress( timeStep );
		}
	}

	protected virtual void ProgressShadows( float timeStep )
	{
		VFXAppearanceController shadow = null;
		VFXAppearanceController[] properShadows = null;

		properShadows = (paused ? pausedShadows : shadows);
		if( properShadows != null )
		{
			for( int i=0; i<properShadows.Length; i++ )
			{
				shadow = properShadows[i];
				if( shadow != null )
				{
					shadow.Progress( timeStep );
					if( shadow.IsOver() )
					{
						RemoveShadow( i, paused );
						/*halmeida - when we remove a shadow, the smaller array may be realocated somewhere else in memory.
						In this case, the porperShadows pointer will be pointing to the wrong place and has to be once again
						made to point to the proper memory place where the smaller array is.*/
						properShadows = (paused ? pausedShadows : shadows);
						if( properShadows == null )
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
	}

	private void UpdateAnimation( float timeStep )
	{
		int oldSequenceIndex = -1;
		int newSequenceIndex = -1;
		bool oldSpriteFlip = false;
		bool newSpriteFlip = false;
		int newSpriteIndex = -1;

		UpdateStatus();
		if( status == INVALID_STATUS )
		{
			over = true;
			return;
		}
		UpdateTransform( timeStep );
		UpdateMaterial( timeStep );
		oldSequenceIndex = sequenceIndex;
		oldSpriteFlip = spriteFlip;
		if( RequiresNewSequence() )
		{
			GetSequenceIndexForStatus( status, ref newSequenceIndex, ref newSpriteFlip );
			UpdateSequence( newSequenceIndex, newSpriteFlip );
		}
		if( (oldSequenceIndex == sequenceIndex) && (oldSpriteFlip == spriteFlip) ) 
		{
			/*halmeida - we didn't proceed to a new animation sequence, so we must progress the current one.*/
			if( (status != INVALID_STATUS) && (sequenceIndex != -1) && (spriteIndex != -1) )
			{
				spriteElapsed += timeStep;
				if( spriteElapsed > spriteDuration )
				{
					newSpriteIndex = spriteIndex;
					newSpriteIndex++;
					if( newSpriteIndex > lastIndex )
					{
						if( loop )
						{
							newSpriteIndex = firstIndex;
						}
						else
						{
							/*halmeida - first I will bring the sprite index back to a valid value, before reevaluating the status, because
							if the status reevaluation does not change the index and make it valid, we will still have a good index to use.*/
							newSpriteIndex = lastIndex;
							/*halmeida - now I reevaluate the status.*/ 
							AdvanceToNextStatus();
							if( status != INVALID_STATUS )
							{
								if( RequiresNewSequence() )
								{
									GetSequenceIndexForStatus( status, ref newSequenceIndex, ref newSpriteFlip );
									UpdateSequence( newSequenceIndex, newSpriteFlip );
									return;
								}
							}
							else
							{
								over = true;
								return;
							}
						}
					}
					UpdateSprite( newSpriteIndex, 0f );
				}
			}
		}
	}

	protected virtual void UpdateStatus()
	{
		status = INVALID_STATUS;
		keepSequenceProgress = false;
	}

	protected virtual void UpdateTransform( float timeStep )
	{
		return;
	}

	protected virtual void UpdateMaterial( float timeStep )
	{
		Color renderingColor = Color.white;

		renderingColor = ExtractRenderingColor();
		currentAlpha = renderingColor.a;
		UpdateAlphaFading( timeStep );
		renderingColor.a = currentAlpha;
		FeedRenderingColor( renderingColor );
		if( fakeLight != null )
		{
			fakeLight.Progress( timeStep );
		}
	}

	private void UpdateAlphaFading( float timeStep )
	{
		float fadeAmount = 0f;

		if( fadeSpeed != 0f )
		{
			fadeAmount = fadeSpeed * timeStep;
			currentAlpha += fadeAmount;
			if( fadeSpeed > 0f )
			{
				fadedToTransparent = false;
				if( (currentAlpha > opaqueAlpha) || fadeImmediately )
				{
					currentAlpha = opaqueAlpha;
					fadeSpeed = 0f;
					fadeImmediately = false;
					fadedToOpaque = true;
				}
			}
			else
			{
				fadedToOpaque = false;
				if( (currentAlpha < 0f) || fadeImmediately )
				{
					currentAlpha = 0f;
					fadeSpeed = 0f;
					fadeImmediately = false;
					fadedToTransparent = true;
				}
			}
		}
	}

	protected virtual void AdvanceToNextStatus()
	{
		status = INVALID_STATUS;
		keepSequenceProgress = false;
	} 

	protected virtual bool RequiresNewSequence()
	{
		return (previousStatus != status);
	}

	private void UpdateSequence( int newSequenceIndex, bool newSpriteFlip )
	{
		int progressIndex = 0;
		float progressTime = 0f;
		int newSpriteIndex = -1;
		float newSpriteElapsed = 0f;

		if( (newSequenceIndex > -1) && (newSequenceIndex < totalSequences) && (firstIndexPerSequence != null) &&
		(lastIndexPerSequence != null) && (speedPerSequence != null) && (loopPerSequence != null) && (fakeLightPerSequence != null) )
		{
			if( keepSequenceProgress && (spriteIndex > -1) && (firstIndex > -1) )
			{
				progressIndex = spriteIndex - firstIndex;
				progressTime = spriteElapsed;
				keepSequenceProgress = false;
			}
			sequenceIndex = newSequenceIndex;
			firstIndex = firstIndexPerSequence[sequenceIndex];
			lastIndex = lastIndexPerSequence[sequenceIndex];
			spriteDuration = speedPerSequence[sequenceIndex];
			loop = loopPerSequence[sequenceIndex];
			fakeLight = fakeLightPerSequence[sequenceIndex];
			spriteFlip = newSpriteFlip;
			newSpriteIndex = firstIndex + progressIndex;
			newSpriteElapsed = progressTime;
			UpdateSprite( newSpriteIndex, newSpriteElapsed );
		}
		else
		{
			/*halmeida - somebody is trying an UpdateAnimation after a Clear().*/
			over = true;
		}
	}

	protected virtual void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		return;
	}

	protected virtual void UpdateSprite( int newSpriteIndex, float newSpriteElapsed )
	{
		VFXAppearanceController shadow = null;

		spriteIndex = newSpriteIndex;
		spriteElapsed = newSpriteElapsed;
		sprite = null;
		spriteNormal = null;
		if( sprites != null )
		{
			if( (spriteIndex > -1) && (sprites.Length > spriteIndex) )
			{
				sprite = sprites[spriteIndex];
				if( spriteNormals != null )
				{
					/*halmeida - if spriteNormals exists, it necessarily has the same length as sprites.*/
					spriteNormal = spriteNormals[spriteIndex];
				}
			}
		}
		if( sprite == null )
		{
			spriteWidth = 0f;
			focusOneValidity = false;
			focusTwoValidity = false;
		}
		else
		{
			spriteWidth = sprite.bounds.size.x;
			focusOffsetOne = focusOffsetsOne[spriteIndex];
			focusOffsetTwo = focusOffsetsTwo[spriteIndex];
			if( spriteFlip )
			{
				focusOffsetOne = MirrorOffsetWithinSprite( focusOffsetOne );
				focusOffsetTwo = MirrorOffsetWithinSprite( focusOffsetTwo );
			}
			focusOneValidity = focusOneValidities[spriteIndex];
			focusTwoValidity = focusTwoValidities[spriteIndex];
			if( shadows != null )
			{
				for( int i=0; i<shadows.Length; i++ )
				{
					shadow = shadows[i];
					if( shadow != null )
					{
						/*halmeida - when updating the sprite of a shadow, I never lock the sprite. If a shadow
						has its sprite fixed at some point, that point is its creation.*/
						shadow.SetSprite( sprite, spriteFlip, false );
					}
				}
			}
		}
	}

	private Vector2 MirrorOffsetWithinSprite( Vector2 originalOffset )
	{
		originalOffset.x = spriteWidth - originalOffset.x;
		return originalOffset;
	}

	protected virtual void RecordChangeVerifiers()
	{
		previousStatus = status;
	}

	private void ExtractSpritesAndLights()
	{
		GameObject sequenceObject = null;
		AnimationSequence sequence = null;
		Sprite[] sequenceSprites = null;
		Sprite[] sequenceSpriteNormals = null;
		Vector2[] sequencePixelOffsetsOne = null;
		Vector2[] sequencePixelOffsetsTwo = null;
		int length = 0;
		int oldLength = 0;
		Sprite[] newSprites = null;
		Sprite[] newSpriteNormals = null;
		Vector2[] newFocusOffsetsOne = null;
		Vector2[] newFocusOffsetsTwo = null;
		bool[] newFocusOneValidities = null;
		bool[] newFocusTwoValidities = null;
		bool sequenceExtracted = false;
		Sprite newSprite = null;
		Sprite newSpriteNormal = null;
		float newPixelsPerUnit = 0f;
		Vector2 newPixelOffset = Vector2.zero;
		bool validFocusOffsetOne = false;
		bool validFocusOffsetTwo = false;
		float framesPerSecond = 0f;
		FakeLightParameters fakeLightSpecs = null;
		FakeLight fakeLight = null;

		sprites = null;
		spriteNormals = null;
		focusOffsetsOne = null;
		focusOffsetsTwo = null;
		focusOneValidities = null;
		focusTwoValidities = null;
		for( int i=0; i<totalSequences; i++ )
		{
			sequenceExtracted = false;
			sequenceObject = sequenceObjects[i];
			if( sequenceObject != null )
			{
				sequence = sequenceObject.GetComponent<AnimationSequence>();
				if( sequence != null )
				{
					sequenceSprites = sequence.sprites;
					sequenceSpriteNormals = sequence.spriteNormals;
					sequencePixelOffsetsOne = sequence.pixelOffsetsOne;
					sequencePixelOffsetsTwo = sequence.pixelOffsetsTwo;
					if( sequenceSprites != null )
					{
						length = sequenceSprites.Length;
						if( length > 0 )
						{
							if( sprites == null )
							{
								oldLength = 0;
							}
							else
							{
								oldLength = sprites.Length;
							}
							if( sequenceSpriteNormals != null )
							{
								if( sequenceSpriteNormals.Length != length )
								{
									sequenceSpriteNormals = null;
								}
							}
							length = oldLength + length;
							newSprites = new Sprite[length];
							newSpriteNormals = new Sprite[length];
							newFocusOffsetsOne = new Vector2[length];
							newFocusOffsetsTwo = new Vector2[length];
							newFocusOneValidities = new bool[length];
							newFocusTwoValidities = new bool[length];
							for( int j=0; j<oldLength; j++ )
							{
								newSprites[j] = sprites[j];
								newSpriteNormals[j] = spriteNormals[j];
								newFocusOffsetsOne[j] = focusOffsetsOne[j];
								newFocusOffsetsTwo[j] = focusOffsetsTwo[j];
								newFocusOneValidities[j] = focusOneValidities[j];
								newFocusTwoValidities[j] = focusTwoValidities[j];
							}
							for( int j=oldLength; j<length; j++ )
							{
								newSprite = sequenceSprites[j-oldLength];
								newSprites[j] = newSprite;
								newSpriteNormal = null;
								if( sequenceSpriteNormals != null )
								{
									newSpriteNormal = sequenceSpriteNormals[j-oldLength];
								}
								newSpriteNormals[j] = newSpriteNormal;
								validFocusOffsetOne = false;
								validFocusOffsetTwo = false;
								if( newSprite != null )
								{
									newPixelsPerUnit = newSprite.pixelsPerUnit;
									if( newPixelsPerUnit != 0f )
									{
										if( sequencePixelOffsetsOne != null )
										{
											if( sequencePixelOffsetsOne.Length > (j-oldLength) )
											{
												newPixelOffset = sequencePixelOffsetsOne[j-oldLength];
												newPixelOffset.x = newPixelOffset.x / newPixelsPerUnit;
												newPixelOffset.y = newPixelOffset.y / newPixelsPerUnit;
												newFocusOffsetsOne[j] = newPixelOffset;
												validFocusOffsetOne = true;
											}
										}
										if( sequencePixelOffsetsTwo != null )
										{
											if( sequencePixelOffsetsTwo.Length > (j-oldLength) )
											{
												newPixelOffset = sequencePixelOffsetsTwo[j-oldLength];
												newPixelOffset.x = newPixelOffset.x / newPixelsPerUnit;
												newPixelOffset.y = newPixelOffset.y / newPixelsPerUnit;
												newFocusOffsetsTwo[j] = newPixelOffset;
												validFocusOffsetTwo = true;
											}
										}
									}
								}
								if( !validFocusOffsetOne )
								{
									newFocusOffsetsOne[j] = Vector2.zero;
								}
								if( !validFocusOffsetTwo )
								{
									newFocusOffsetsTwo[j] = Vector2.zero;
								}
								newFocusOneValidities[j] = validFocusOffsetOne;
								newFocusTwoValidities[j] = validFocusOffsetTwo;
							}
							sprites = newSprites;
							spriteNormals = newSpriteNormals;
							focusOffsetsOne = newFocusOffsetsOne;
							focusOffsetsTwo = newFocusOffsetsTwo;
							focusOneValidities = newFocusOneValidities;
							focusTwoValidities = newFocusTwoValidities;
							newSprites = null;
							newSpriteNormals = null;
							newFocusOffsetsOne = null;
							newFocusOffsetsTwo = null;
							newFocusOneValidities = null;
							newFocusTwoValidities = null;
							firstIndexPerSequence[i] = oldLength;
							lastIndexPerSequence[i] = length-1;
							framesPerSecond = (sequence.framesPerSecond < 1) ? 1 : sequence.framesPerSecond;
							speedPerSequence[i] = 1f / framesPerSecond;
							loopPerSequence[i] = sequence.loop;
							fakeLightPerSequence[i] = null;
							if( sequence.hasFakeLight )
							{
								fakeLightSpecs = sequenceObject.GetComponent<FakeLightParameters>();
								if( fakeLightSpecs != null )
								{
									fakeLight = new FakeLight( fakeLightSpecs.color, fakeLightSpecs.startingRadius, fakeLightSpecs.startingIntensity );
									fakeLight.ConfigureStretches( fakeLightSpecs.radiusSpeedPerStretch, fakeLightSpecs.intensitySpeedPerStretch,
										fakeLightSpecs.durationPerStretch );
									fakeLightPerSequence[i] = fakeLight;
								}
							}
							sequenceExtracted = true;
						}
					}
				}
			}
			if( !sequenceExtracted )
			{
				firstIndexPerSequence[i] = -1;
				lastIndexPerSequence[i] = -1;
				speedPerSequence[i] = 0f;
				loopPerSequence[i] = false;
				fakeLightPerSequence[i] = null;
			}
		}
	}

	public int GetCurrentStatus()
	{
		return status;
	}

	public Vector2 GetCurrentFocusOffsetOne( ref bool focusValid )
	{
		focusValid = focusOneValidity;
		return focusOffsetOne;
	}

	public Vector2 GetCurrentFocusOffsetTwo( ref bool focusValid )
	{
		focusValid = focusTwoValidity;
		return focusOffsetTwo;
	}

	public FakeLight GetCurrentFakeLight()
	{
		return fakeLight;
	}

	public bool IsOver()
	{
		return over;
	}

	public void SetOpaqueAlpha( float newOpaqueAlpha )
	{
		opaqueAlpha = (newOpaqueAlpha > 1f) ? 1f : newOpaqueAlpha;
		opaqueAlpha = (opaqueAlpha < 0f) ? 0f : opaqueAlpha;
		if( currentAlpha >= opaqueAlpha )
		{
			StartAlphaFading( 1f, true, false, false );
		}
	}

	public float GetOpaqueAlpha()
	{
		return opaqueAlpha;
	}

	public virtual void StartAlphaFading( float newFadeSpeed, bool immediately, bool affectChildBehind, bool affectChildAhead )
	{
		fadeSpeed = newFadeSpeed;
		if( immediately )
		{
			fadeImmediately = true;
			UpdateMaterial( 0f );
		}
		if( affectChildBehind )
		{
			StartAlphaFadingChildAnimators( newFadeSpeed, immediately, true );
		}
		if( affectChildAhead )
		{
			StartAlphaFadingChildAnimators( newFadeSpeed, immediately, false );
		}
	}

	public virtual bool IsOpaque()
	{
		return fadedToOpaque;
	}

	public virtual bool IsTransparent()
	{
		return fadedToTransparent;
	}

	public virtual void SetSide( bool newSide )
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.SetSide( newSide );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.SetSide( newSide );
		}
	}

	public virtual bool GetSide()
	{
		return SIDE_RIGHT;
	}

	public virtual void SetSpeedAngleWithRight( bool isPushing, float newSpeedAngle, bool under90Transformed )
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.SetSpeedAngleWithRight( isPushing, newSpeedAngle, under90Transformed );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.SetSpeedAngleWithRight( isPushing, newSpeedAngle, under90Transformed );
		}
	}

	public virtual void SetSpeedAngleBody( bool pushingDirection, float angleWithRight )
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.SetSpeedAngleBody( pushingDirection, angleWithRight );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.SetSpeedAngleBody( pushingDirection, angleWithRight );
		}
	}

	public virtual void Stabilize()
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.Stabilize();
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.Stabilize();
		}
	}

	public virtual void SetLifeStage( float lifeRate )
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.SetLifeStage( lifeRate );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.SetLifeStage( lifeRate );
		}
	}

	public virtual void StartDying()
	{
		if( childAnimatorBack != null )
		{
			childAnimatorBack.StartDying();
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.StartDying();
		}
	}

	public virtual void TogglePause( bool pause )
	{
		paused = pause;
		if( childAnimatorBack != null )
		{
			childAnimatorBack.TogglePause( pause );
		}
		if( childAnimatorFront != null )
		{
			childAnimatorFront.TogglePause( pause );
		}
	}

	public bool IsPaused()
	{
		return paused;
	}

	public virtual int GetPausedShadowsAmount()
	{
		if( pausedShadows != null )
		{
			return pausedShadows.Length;
		}
		return 0;
	}

	public VFXParticleController GetParticleController()
	{
		return particleController;
	}

	public virtual void StartDepthChain( float depth, bool inwards )
	{
		Vector3 position = Vector3.zero;

		position = gameObject.transform.position;
		position.z = depth;
		gameObject.transform.position = position;
		if( inwards )
		{
			StartChildDepthChain( depth + childSeparation, true );
			CorrectShadowDepths( depth );
		}
		else
		{
			StartChildDepthChain( depth - childSeparation, false );
		}
	}



	protected virtual void StartChildDepthChain( float depth, bool inwards )
	{
		if( inwards )
		{
			if( childAnimatorBack != null )
			{
				childAnimatorBack.StartDepthChain( depth, inwards );
			}
		}
		else
		{
			if( childAnimatorFront != null )
			{
				childAnimatorFront.StartDepthChain( depth, inwards );
			}
		}
	}

	protected virtual void CorrectShadowDepths( float depth )
	{
		GameObject shadowObject = null;
		Vector3 position = Vector3.zero;
		float biggestPausedDepth = 0f;

		/*halmeida - the paused shadows have display priority over the normal shadows, so they take smaller depth values.*/
		biggestPausedDepth = depth;
		if( pausedShadowObjects != null )
		{
			depth += childSeparation;
			for( int i=0; i<pausedShadowObjects.Length; i++ )
			{
				shadowObject = pausedShadowObjects[i];
				if( shadowObject != null )
				{
					/*halmeida - the shadow with biggest index is the one that was most recently created. Therefore
					it should be on top of all others. It should be given the smallest depth.*/
					position = shadowObject.transform.position;
					position.z = depth + ((pausedShadowObjects.Length-1-i) * childSeparation);
					shadowObject.transform.position = position;
				}
			}
			biggestPausedDepth = depth + (pausedShadowObjects.Length-1) * childSeparation;
		}
		if( shadowObjects != null )
		{
			depth = biggestPausedDepth + childSeparation;
			for( int i=0; i<shadowObjects.Length; i++ )
			{
				shadowObject = shadowObjects[i];
				if( shadowObject != null )
				{
					position = shadowObject.transform.position;
					position.z = depth + ((shadowObjects.Length-1-i) * childSeparation);
					shadowObject.transform.position = position;
				}
			}
		}
	}

	public float GetChildSeparation()
	{
		return childSeparation;
	}

	public virtual void CreateShadow( ShadowConfiguration shadowConfig )
	{
		GameObject shadowObject = null;
		VFXAppearanceController shadow = null;
		int shadowIndex = 0;
		string properShadowName = null;

		if( shadowConfig != null )
		{
			if( !paused )
			{
				shadowIndex = (shadowObjects != null) ? shadowObjects.Length : 0;
				properShadowName = "ShadowObject";
			}
			else
			{
				shadowIndex = (pausedShadowObjects != null) ? pausedShadowObjects.Length : 0;
				properShadowName = "PausedShadowObject";
			}
			shadowObject = new GameObject(properShadowName+shadowIndex);
			if( shadowObject != null )
			{
				shadow = shadowObject.AddComponent<VFXSpriteController>();
				if( shadow != null )
				{
					if( !paused )
					{
						UsefulFunctions.IncreaseArray<GameObject>( ref shadowObjects, shadowObject );
						UsefulFunctions.IncreaseArray<VFXAppearanceController>( ref shadows, shadow );
					}
					else
					{
						UsefulFunctions.IncreaseArray<GameObject>( ref pausedShadowObjects, shadowObject );
						UsefulFunctions.IncreaseArray<VFXAppearanceController>( ref pausedShadows, shadow );
					}
					if( shadowConfig.fixedPosition )
					{
						shadowObject.transform.position = transform.position;
						shadowObject.transform.rotation = transform.rotation;
					}
					else
					{
						shadowObject.transform.SetParent( transform, false );
						shadowObject.transform.localPosition = Vector3.zero;
						shadowObject.transform.localRotation = Quaternion.identity;
					}
					CorrectShadowDepths( transform.position.z );
					shadow.ChangeRenderingMaterial( shadowConfig.renderingMaterial );
					shadow.SetSprite( sprite, spriteFlip, shadowConfig.fixedSprite );
					shadow.ConfigureScaleEvolution( shadowConfig.scaleOrigin, shadowConfig.scaleTargets, shadowConfig.scalingDurations );
					shadow.ConfigureAlphaEvolution( shadowConfig.alphaOrigin, shadowConfig.alphaTargets, shadowConfig.fadeDurations );
					shadow.ConfigureOutlineEvolution( shadowConfig.outlineColors, shadowConfig.outlineWidth, shadowConfig.outlineColorInterval,
						shadowConfig.outlineAlphaOrigin, shadowConfig.outlineAlphaTargets, shadowConfig.outlineFadeDurations );
					shadow.ConfigureColorAddition( shadowConfig.contentColorAddition );
					shadow.StartEvolutions( shadowConfig.totalDuration );
				}
				else
				{
					Destroy( shadowObject );
				}
			}
		}
	}

	protected void RemoveShadow( int shadowIndex, bool fromPausedShadows )
	{
		VFXAppearanceController shadow = null;
		GameObject shadowObject = null;
		VFXAppearanceController[] properShadows = null;
		GameObject[] properShadowObjects = null;

		if( fromPausedShadows )
		{
			properShadowObjects = pausedShadowObjects;
			properShadows = pausedShadows;
		}
		else
		{
			properShadowObjects = shadowObjects;
			properShadows = shadows;
		}
		if( properShadows != null )
		{
			if( (shadowIndex > -1) && (shadowIndex < properShadows.Length) )
			{
				shadow = properShadows[shadowIndex];
				if( shadow != null )
				{
					shadow.Clear();
					properShadows[shadowIndex] = null;
				}
				shadowObject = properShadowObjects[shadowIndex];
				if( shadowObject != null )
				{
					Destroy( shadowObject );
					properShadowObjects[shadowIndex] = null;
				}
				if( fromPausedShadows )
				{
					UsefulFunctions.DecreaseArray<VFXAppearanceController>( ref pausedShadows, shadowIndex );
					UsefulFunctions.DecreaseArray<GameObject>( ref pausedShadowObjects, shadowIndex );
				}
				else
				{
					UsefulFunctions.DecreaseArray<VFXAppearanceController>( ref shadows, shadowIndex );
					UsefulFunctions.DecreaseArray<GameObject>( ref shadowObjects, shadowIndex );
				}
			}
		}
	}

	public virtual void ClearShadows()
	{
		VFXAppearanceController shadow = null;
		GameObject shadowObject = null;

		if( pausedShadows != null )
		{
			for( int i=0; i<pausedShadows.Length; i++ )
			{
				shadow = pausedShadows[i];
				if( shadow != null )
				{
					shadow.Clear();
					pausedShadows[i] = null;
				}
				shadowObject = pausedShadowObjects[i];
				if( shadowObject != null )
				{
					Destroy( shadowObject );
					pausedShadowObjects[i] = null;
				}
			}
			pausedShadows = null;
			pausedShadowObjects = null;
		}
		if( shadows != null )
		{
			for( int i=0; i<shadows.Length; i++ )
			{
				shadow = shadows[i];
				if( shadow != null )
				{
					shadow.Clear();
					shadows[i] = null;
				}
				shadowObject = shadowObjects[i];
				if( shadowObject != null )
				{
					Destroy( shadowObject );
					shadowObjects[i] = null;
				}
			}
			shadows = null;
			shadowObjects = null;
		}
	}
}
