using UnityEngine;
using System.Collections;

public class SimpleAnimator : BaseAnimator
{
	public const int STATUS_RUNNING = 0;

	public GameObject sequenceObject;

	private SpriteRenderer spriteRenderer;
	private Material material;
	private int waveColorID;
	private int waveSourceID;
	private int waveLengthsID;
	private int waveAlphasID;
	private int waveScalarsID;
	private int waveNoiseTexID;
	private int waveNoiseScaleID;
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
	private bool lastWave;
	private Sprite waveNoiseSprite;
	private float waveNoiseScale;
	private float waveDelayDuration;
	private float waveDelayElapsed;
	private int colorToAddID;
	private Vector4 colorToAdd;
	private bool flipRequested;

	protected override void Awake()
	{
		material = null;
		base.Awake();
	}

	public override void Clear( bool renderingComponentInitialized = true )
	{
		base.Clear( renderingComponentInitialized );
		waveColorID = -1;
		waveSourceID = -1;
		waveLengthsID = -1;
		waveAlphasID = -1;
		waveScalarsID = -1;
		waveNoiseTexID = -1;
		waveNoiseScaleID = -1;
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
		lastWave = false;
		waveNoiseSprite = null;
		waveNoiseScale = 0f;
		waveDelayDuration = 0f;
		waveDelayElapsed = 0f;
		colorToAddID = -1;
		colorToAdd = Vector4.zero;
		flipRequested = false;
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
			//Debug.Log("Debug : SimpleAnimator : Materials before extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			material = spriteRenderer.material;
			//Debug.Log("Debug : SimpleAnimator : Materials after extraction = "+Resources.FindObjectsOfTypeAll(typeof(Material)).Length+".");
			waveColorID = Shader.PropertyToID("_WaveColor");
			waveSourceID = Shader.PropertyToID("_WaveSource");
			waveLengthsID = Shader.PropertyToID("_WaveLengths");
			waveAlphasID = Shader.PropertyToID("_WaveAlphas");
			waveScalarsID = Shader.PropertyToID("_WaveScalars");
			waveNoiseTexID = Shader.PropertyToID("_NoiseTex");
			waveNoiseScaleID = Shader.PropertyToID("_NoiseScale");
			colorToAddID = Shader.PropertyToID("_ColorToAdd");
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

	protected override void FillSequencesArray()
	{
		totalSequences = 1;
		sequenceObjects = new GameObject[1];
		sequenceObjects[0] = sequenceObject;
	}

	protected override void UpdateStatus()
	{
		if( (status == INVALID_STATUS) && !over )
		{
			status = STATUS_RUNNING;
		}
	}

	public void StartWave( Color newWaveColor, Vector3 newWaveSource, Vector4 newWaveLengths, Vector4 newWaveAlphas, float newWaveFrontLimit,
		float newWaveSpeed, float newWaveLengthVariation, float solidifyWave, float fadeWithWave, Sprite newNoiseSprite, float newNoiseScale,
		float newWaveDelay )
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
			waveNoiseSprite = newNoiseSprite;
			if( waveNoiseSprite != null )
			{
				material.SetTexture( waveNoiseTexID, waveNoiseSprite.texture );
			}
			else
			{
				material.SetTexture( waveNoiseTexID, null );
			}
			waveNoiseScale = newNoiseScale;
			material.SetFloat( waveNoiseScaleID, waveNoiseScale );
			waveDelayDuration = newWaveDelay;
			waveDelayElapsed = 0f;
			lastWave = (fadeWithWave == 1f);
		}
		else
		{
			over = (fadeWithWave == 1f);
		}
	}

	public void SetColorAddition( Vector4 newColorAddition )
	{
		if( material != null )
		{
			colorToAdd = newColorAddition;
			material.SetVector( colorToAddID, colorToAdd );
		}
	}

	public void RequestFlip()
	{
		flipRequested = true;
	}

	protected override void UpdateMaterial( float timeStep )
	{
		float lengthVariation = 0f;

		base.UpdateMaterial( timeStep );
		if( (material != null) && waveRunning )
		{
			if( (waveDelayDuration > 0f) && (waveDelayElapsed < waveDelayDuration) )
			{
				waveDelayElapsed += timeStep;
			}
			else
			{
				waveFront += waveSpeed * timeStep;
				if( waveFront > waveFrontLimit )
				{
					waveFront = 0f;
					waveRunning = false;
					if( lastWave )
					{
						over = true;
					}
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
	}

	protected override void AdvanceToNextStatus()
	{
		status = INVALID_STATUS;
	}

	protected override bool RequiresNewSequence()
	{
		return ( (previousStatus != status) || flipRequested );
	}

	protected override void GetSequenceIndexForStatus( int statusValue, ref int newSequenceIndex, ref bool newSpriteFlip )
	{
		if( statusValue == STATUS_RUNNING )
		{
			newSequenceIndex = statusValue;
			newSpriteFlip = spriteFlip;
			if( flipRequested )
			{
				newSpriteFlip = !newSpriteFlip;
				flipRequested = false;
			}
			return;
		}
		newSequenceIndex = -1;
		newSpriteFlip = false;
	}

	public bool HasRunningWave()
	{
		return waveRunning;
	}
}
