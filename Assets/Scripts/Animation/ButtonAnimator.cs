using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System; //for the String.IsNullOrEmpty.

public class ButtonAnimator : BaseAnimator
{
	public const int SMALLEST_STATUS = 0;
	public const int STATUS_APPEARING = 0;
	public const int STATUS_IDLE = 1;
	public const int STATUS_ACTIVATING = 2;
	public const int STATUS_ACTIVATED = 3;
	public const int BIGGEST_STATUS = 3;

	public const float GLOW_SPEED = 1.7f;
	public const float GLOW_COLOR_R = 0.4f;
	public const float GLOW_COLOR_G = 0.4f;
	public const float GLOW_COLOR_B = 0.4f;
	public const float GLOW_COLOR_A = 1f;
	public const float NO_GLOW_DURATION = 0.6f;

	public GameObject sequenceAppearing;
	public GameObject sequenceIdle;
	public GameObject sequenceActivating;
	public GameObject sequenceActivated;
	public bool glow;
	public string appearingSoundPath;
	public string activatingSoundPath;

	private Image buttonImage;
	private bool pressed;
	private Color originalColor;
	private Color glowColor;
	private Color differenceColor;
	private float transitionTime;
	private bool toGlow;
	private bool toOriginal;
	private float permanenceTime;
	private float elapsedTime;
	private AudioSource audioSource;
	private AudioClip appearingSound;
	private AudioClip activatingSound;
	private int appearingSoundID;
	private int activatingSoundID;

	protected override void Awake()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		appearingSound = null;
		activatingSound = null;
		appearingSoundID = -1;
		activatingSoundID = -1;
		#if !UNITY_ANDROID || UNITY_EDITOR
			LoadAllSoundsFromResources();
		#else
			LoadAllSoundsIntoAudioCenter();
		#endif
		/*halmeida - gotta get the audio component before the awake of the base class because
		the awake already runs Progress() and that method may require sound.*/
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		pressed = false;
		originalColor = Color.white;
		glowColor = Color.white;
		differenceColor = Color.white;
		transitionTime = 0f;
		toGlow = false;
		toOriginal = false;
		permanenceTime = 0f;
		elapsedTime = 0f;
		if( renderingComponentInitialized )
		{
			/*halmeida - the BaseAnimator calls Clear() within its Awake(), to set the variables to
			a zero state. However, in that specific call to Clear, it uses the argument false. So,
			since we don't want to clear the sounds that we just loaded in this subclass' Awake,
			we only clear the sounds when the argument is true.*/
			ClearSoundsFromResources();
			ClearSoundsFromAudioCenter();
		}
	}

	protected override void ClearRenderingComponent()
	{
		buttonImage = null;
	}

	protected override bool ExtractRenderingComponent()
	{
		buttonImage = GetComponent<Image>();
		if( buttonImage != null)
		{
			originalColor = buttonImage.color;
			glowColor = new Color( GLOW_COLOR_R, GLOW_COLOR_G, GLOW_COLOR_B, GLOW_COLOR_A );
			differenceColor = originalColor - glowColor;
			permanenceTime = NO_GLOW_DURATION;
			return true;
		}
		return false;
	}

	protected override void FeedRenderingComponent( Sprite newSprite, Sprite newSpriteNormal, bool newSpriteFlip )
	{
		if( buttonImage != null )
		{
			buttonImage.sprite = newSprite;
		}
	}

	protected override Color ExtractRenderingColor()
	{
		if( buttonImage != null )
		{
			return buttonImage.color;
		}
		return Color.white;
	}

	protected override void FeedRenderingColor( Color newColor )
	{
		if( buttonImage != null )
		{
			buttonImage.color = newColor;
		}
	}

	protected override void FillSequencesArray()
	{
		totalSequences = 4;
		sequenceObjects = new GameObject[totalSequences];
		for( int i=0; i<totalSequences; i++ )
		{
			switch( i )
			{
				case 0:
					sequenceObjects[0] = sequenceAppearing;
					break;
				case 1:
					sequenceObjects[1] = sequenceIdle;
					break;
				case 2:
					sequenceObjects[2] = sequenceActivating;
					break;
				case 3:
					sequenceObjects[3] = sequenceActivated;
					break;
			}
		}
	}

	protected override void UpdateStatus()
	{
		if( status == INVALID_STATUS )
		{
			if( !over )
			{
				status = STATUS_APPEARING;
				#if !UNITY_ANDROID || UNITY_EDITOR
					if( (audioSource != null) && (appearingSound != null) )
					{
						audioSource.clip = appearingSound;
						audioSource.Play();
					}
				#else
					if( appearingSoundID != -1 )
					{
						AudioCenter.playSound( appearingSoundID, 1f );
					}
				#endif
			}
		}
		else
		{
			if( pressed )
			{
				if( (status != STATUS_ACTIVATING) && (status != STATUS_ACTIVATED) )
				{
					status = STATUS_ACTIVATING;
					#if !UNITY_ANDROID || UNITY_EDITOR
						if( (audioSource != null) && (activatingSound != null) )
						{
							audioSource.clip = activatingSound;
							audioSource.Play();
						}
					#else
						if( activatingSoundID != -1 )
						{
							AudioCenter.playSound( activatingSoundID, 1f );
						}
					#endif
				}
			}
			else
			{
				if( (status != STATUS_APPEARING) && (status != STATUS_IDLE) )
				{
					status = STATUS_IDLE;
				}
			}
		}
	}

	protected override void UpdateMaterial( float timeStep )
	{
		float transitionQuantity = 0f;
		Color newColor = Color.black;

		if( !pressed && glow )
		{
			if( elapsedTime < permanenceTime )
			{
				elapsedTime += timeStep;
				if( elapsedTime > permanenceTime )
				{
					elapsedTime = permanenceTime;
					toGlow = true;
					toOriginal = false;
				}
			}
			else
			{
				if( toGlow )
				{
					transitionTime += timeStep;
					transitionQuantity = transitionTime * GLOW_SPEED;
					if( transitionQuantity > 1f )
					{
						transitionQuantity = 1f;
						toGlow = false;
						toOriginal = true;
					}
					if( transitionQuantity < 0f )
					{
						transitionQuantity = 0f;
					}
				}
				if( toOriginal )
				{
					transitionTime -= timeStep;
					transitionQuantity = transitionTime * GLOW_SPEED;
					if( transitionQuantity < 0f )
					{
						transitionQuantity = 0f;
						toGlow = false;
						toOriginal = false;
						elapsedTime = 0f;
					}
					if( transitionQuantity > 1f )
					{
						transitionQuantity = 1f;
					}
				}
				if( buttonImage != null )
				{
					newColor = originalColor - (transitionQuantity * differenceColor);
					buttonImage.color = newColor;
				}
			}
		}
		base.UpdateMaterial( timeStep );
	}

	protected override void AdvanceToNextStatus()
	{
		switch( status )
		{
			case STATUS_APPEARING:
				status = STATUS_IDLE;
				break;
			case STATUS_IDLE:
				status = INVALID_STATUS;
				break;
			case STATUS_ACTIVATING:
				status = STATUS_ACTIVATED;
				break;
			case STATUS_ACTIVATED:
				status = INVALID_STATUS;
				break;
			default:
				status = INVALID_STATUS;
				break;
		}
	}

	protected override void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		newSpriteFlip = false;
		switch( statusValue )
		{
			case STATUS_APPEARING:
				newSequenceIndex = 0;
				break;
			case STATUS_IDLE:
				newSequenceIndex = 1;
				break;
			case STATUS_ACTIVATING:
				newSequenceIndex = 2;
				break;
			case STATUS_ACTIVATED:
				newSequenceIndex = 3;
				break;
		}
	}

	public void SetPressed( bool newPressed )
	{
		pressed = newPressed;
		if( buttonImage != null )
		{
			buttonImage.color = originalColor;
		}
	}

	public bool IsPressed()
	{
		return pressed;
	}

	private void LoadAllSoundsFromResources()
	{
		if( !String.IsNullOrEmpty( appearingSoundPath.Trim() ) && (appearingSound == null) )
		{
			appearingSound = Resources.Load<AudioClip>( "Sounds/"+(appearingSoundPath.Trim()) );
		}
		if( !String.IsNullOrEmpty( activatingSoundPath.Trim() ) && (activatingSound == null) )
		{
			activatingSound = Resources.Load<AudioClip>( "Sounds/"+(activatingSoundPath.Trim()) );
		}
	}

	private void ClearSoundsFromResources()
	{
		if( appearingSound != null )
		{
			Resources.UnloadAsset( appearingSound );
			appearingSound = null;
		}
		if( activatingSound != null )
		{
			Resources.UnloadAsset( activatingSound );
			activatingSound = null;
		}
	}

	private void LoadAllSoundsIntoAudioCenter()
	{
		if( !String.IsNullOrEmpty( appearingSoundPath.Trim() ) && (appearingSoundID == -1) )
		{
			appearingSoundID = AudioCenter.loadSound( appearingSoundPath.Trim() );
		}
		if( !String.IsNullOrEmpty( activatingSoundPath.Trim() ) && (activatingSoundID == -1) )
		{
			activatingSoundID = AudioCenter.loadSound( activatingSoundPath.Trim() );
		}
	}

	private void ClearSoundsFromAudioCenter()
	{
		if( appearingSoundID != -1 )
		{
			AudioCenter.unloadSound( appearingSoundID );
			appearingSoundID = -1;
		}
		if( activatingSoundID != -1 )
		{
			AudioCenter.unloadSound( activatingSoundID );
			activatingSoundID = -1;
		}
	}
}
