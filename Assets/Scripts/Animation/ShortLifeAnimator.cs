using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortLifeAnimator : BaseAnimator
{
	public const int STATUS_APPEARING = 0;
	public const int STATUS_ALIVE = 1;
	public const int STATUS_DYING = 2;
	public const int STATUS_DEAD = 3;

	public GameObject sequenceAppearing;
	public GameObject[] sequencesAlive;
	public GameObject sequenceDying;
	public GameObject sequenceDead;

	private SpriteRenderer spriteRenderer;
	private bool deathRequested;
	private bool dead;
	private int totalLifeStages;
	private float lifeStageRate;
	private int lifeStageIndex;
	private bool lifeStageIndexChanged;
	private int lifeStageIndexRequested;
	private Material material;
	private int waveColorID;
	private int waveSourceID;
	private int waveLengthsID;
	private int waveAlphasID;
	private int waveScalarsID;
	private bool waveRunning;
	private Color waveColor;
	private Vector3 waveSource;
	private float waveFront;
	private float waveFrontLimit;
	private Vector4 waveLengths;
	private Vector4 waveAlphas;
	private Vector4 waveScalars;
	private float waveSpeed;
	private float waveLengthVariation;

	protected override void Awake()
	{
		material = null;
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		deathRequested = false;
		dead = false;
		totalLifeStages = 0;
		lifeStageRate = 0f;
		lifeStageIndex = -1;
		lifeStageIndexChanged = false;
		lifeStageIndexRequested = -1;
		waveColorID = -1;
		waveSourceID = -1;
		waveLengthsID = -1;
		waveAlphasID = -1;
		waveScalarsID = -1;
		waveRunning = false;
		waveColor = Color.white;
		waveSource = Vector3.zero;
		waveFront = 0f;
		waveFrontLimit = 0f;
		waveLengths = Vector4.zero;
		waveAlphas = Vector4.zero;
		waveScalars = Vector4.zero;
		waveSpeed = 0f;
		waveLengthVariation = 0f;
	}

	protected override void ClearRenderingComponent()
	{
		spriteRenderer = null;
		if( material != null )
		{
			Destroy( material );
			material = null;
		}
	}

	protected override bool ExtractRenderingComponent()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if( spriteRenderer != null )
		{
			//Debug.Log("Debug : ShortLifeAnimator : Materials before extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			material = spriteRenderer.material;
			//Debug.Log("Debug : ShortLifeAnimator : Materials after extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			waveColorID = Shader.PropertyToID("_WaveColor");
			waveSourceID = Shader.PropertyToID("_WaveSource");
			waveLengthsID = Shader.PropertyToID("_WaveLengths");
			waveAlphasID = Shader.PropertyToID("_WaveAlphas");
			waveScalarsID = Shader.PropertyToID("_WaveScalars");
			if( material != null )
			{
				material.SetVector( waveScalarsID, waveScalars );
				material.SetVector( waveLengthsID, waveLengths );
			}
		}
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

	protected override BaseAnimator ExtractChildAnimatorComponent( GameObject childObject )
	{
		if( childObject != null )
		{
			return childObject.GetComponent<ShortLifeAnimator>();
		}
		return null;
	}

	public override void SetLifeStage( float lifeRate )
	{
		float stageLimit = 1f;

		if((lifeRate <= 0f) || (lifeStageRate <= 0f))
		{
			StartDying();
		}
		else
		{
			lifeStageIndexRequested = -1;
			for( int i=0; i<totalLifeStages; i++ )
			{
				stageLimit -= lifeStageRate;
				if( lifeRate > stageLimit )
				{
					lifeStageIndexRequested = i;
					break;
				}
			}
			/*halmeida - even though we subtracted all parts of the whole, there exists an imprecision in float operations.
			The successive subtractions might have left a very small stageLimit that is still bigger than zero. If the user
			provides a lifeRate small enough to be inside this left over, it would not be detected as belonging to any of
			the stages. If this is the case, we force it to belong to the last life stage.*/ 
			if( lifeStageIndexRequested == -1 )
			{
				lifeStageIndexRequested = totalLifeStages - 1;
			}
		}
		base.SetLifeStage( lifeRate );
	}

	public bool LifeStageChangeScheduled()
	{
		return ( (lifeStageIndexRequested > -1) && (lifeStageIndexRequested != lifeStageIndex) );
	}

	public override void StartDying()
	{
		if( !dead && !deathRequested )
		{
			deathRequested = true;
			lifeStageIndex = -1;
			lifeStageIndexRequested = -1;
		}
		base.StartDying();
	}

	public bool IsDead()
	{
		return dead;
	}

	protected override void FillSequencesArray()
	{
		if( sequencesAlive != null )
		{
			totalLifeStages = sequencesAlive.Length;
			lifeStageRate = 1f / totalLifeStages;
		}
		totalSequences = 3 + totalLifeStages;
		sequenceObjects = new GameObject[totalSequences];
		sequenceObjects[0] = sequenceAppearing;
		for( int i=0; i<totalLifeStages; i++ )
		{
			sequenceObjects[1+i] = sequencesAlive[i];
		}
		sequenceObjects[1+totalLifeStages] = sequenceDying;
		sequenceObjects[2+totalLifeStages] = sequenceDead;
	}

	protected override void UpdateStatus()
	{
		if( status == INVALID_STATUS )
		{
			if( !over )
			{
				status = STATUS_APPEARING;
			}
		}
		else
		{
			if( deathRequested && ((status == STATUS_ALIVE) || (status == STATUS_APPEARING)) )
			{
				status = STATUS_DYING;
			}
			else
			{
				if( status == STATUS_ALIVE )
				{
					if( lifeStageIndexRequested > -1 )
					{
						if( lifeStageIndexRequested < totalLifeStages )
						{
							if( lifeStageIndexRequested != lifeStageIndex )
							{
								lifeStageIndex = lifeStageIndexRequested;
								lifeStageIndexRequested = -1;
								lifeStageIndexChanged = true;
							}
						}
						else
						{
							StartDying();
						}
					}
				}
			}
		}
	}

	public void StartWave( Color newWaveColor, Vector3 newWaveSource, Vector4 newWaveLengths, Vector4 newWaveAlphas, float newWaveFrontLimit,
		float newWaveSpeed, float newWaveLengthVariation, float solidifyWave, float fadeWithWave )
	{
		if( material != null )
		{
			waveColor = newWaveColor;
			waveSource = newWaveSource - transform.position;
			waveLengths = newWaveLengths;
			waveAlphas = newWaveAlphas;
			waveFront = 0f;
			waveFrontLimit = newWaveFrontLimit;
			waveSpeed = newWaveSpeed;
			waveLengthVariation = newWaveLengthVariation;
			waveRunning = true;
			waveScalars = new Vector4( waveFront, solidifyWave, fadeWithWave, 0f );
			material.SetColor( waveColorID, waveColor );
			material.SetVector( waveSourceID, new Vector4( waveSource.x, waveSource.y, 0f, 0f ) );
			material.SetVector( waveLengthsID, waveLengths );
			material.SetVector( waveAlphasID, waveAlphas );
			material.SetVector( waveScalarsID, waveScalars );
		}
	}

	protected override void UpdateMaterial( float timeStep )
	{
		float lengthVariation = 0f;

		base.UpdateMaterial( timeStep );
		if( (material != null) && waveRunning )
		{
			waveFront += waveSpeed * timeStep;
			if( waveFront > waveFrontLimit )
			{
				waveFront = 0f;
				waveRunning = false;
			}
			waveScalars.x = waveFront;
			material.SetVector( waveScalarsID, waveScalars );
			lengthVariation = waveLengthVariation * timeStep;
			if( lengthVariation != 0f )
			{
				waveLengths.x += lengthVariation;
				waveLengths.x = (waveLengths.x < 0f) ? 0f : waveLengths.x;
				waveLengths.y += lengthVariation;
				waveLengths.y = (waveLengths.y < 0f) ? 0f : waveLengths.y;
				waveLengths.z += lengthVariation;
				waveLengths.z = (waveLengths.z < 0f) ? 0f : waveLengths.z;
				waveLengths.w += lengthVariation;
				waveLengths.w = (waveLengths.w < 0f) ? 0f : waveLengths.w;
				material.SetVector( waveLengthsID, waveLengths );
			}
		}
	}

	protected override void AdvanceToNextStatus()
	{
		switch( status )
		{
			case STATUS_APPEARING:
				if( totalLifeStages > 0 )
				{
					status = STATUS_ALIVE;
					lifeStageIndex = 0;
					lifeStageIndexChanged = true;
				}
				else
				{
					StartDying();
				}
				break;
			case STATUS_ALIVE:
				/*halmeida - since the STATUS_ALIVE animation should be a looping animation, this case
				should never happen.*/
				status = INVALID_STATUS;
				break;
			case STATUS_DYING:
				dead = true;
				status = STATUS_DEAD;
				break;
			case STATUS_DEAD:
				status = INVALID_STATUS;
				break;
			default:
				status = INVALID_STATUS;
				break;
		}
	}

	protected override bool RequiresNewSequence()
	{
		if( lifeStageIndexChanged )
		{
			lifeStageIndexChanged = false;
			return true;
		}
		return base.RequiresNewSequence();
	}

	protected override void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		newSpriteFlip = false;
		switch( statusValue )
		{
			case STATUS_APPEARING:
				newSequenceIndex = 0;
				break;
			case STATUS_ALIVE:
				newSequenceIndex = 1+lifeStageIndex;
				break;
			case STATUS_DYING:
				newSequenceIndex = 1+totalLifeStages;
				break;
			case STATUS_DEAD:
				newSequenceIndex = 2+totalLifeStages;
				break;
			default:
				newSequenceIndex = -1;
				break;
		}
	}
}
