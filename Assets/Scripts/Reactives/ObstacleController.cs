using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	public const int CONSTRUCTION_LEVEL_INVALID = -1;
	public const int CONSTRUCTION_LEVEL_FRAIL = 0;
	public const int CONSTRUCTION_LEVEL_MEDIUM = 1;
	public const int CONSTRUCTION_LEVEL_STURDY = 2;

	public const int SOUND_FX_CATEGORY_DAMAGE_IN = 0;
	public const int SOUND_FX_CATEGORY_DESTRUCTION = 1;

	public int obstacleID;
	public int constructionLevel;
	public float obstacleHP;
	public float damageWaveLength;
	public GameObject debrisModel;
	public float debrisDensityDamage;
	public float debrisDensityDestruction;
	public Vector4 debrisColorToAdd;
	public float extraDebrisDelay;
	public string[] damageInSoundPaths;
	public string[] destructionSoundPaths;

	private Collider2D obstacleCollider;
	private ShortLifeAnimator obstacleAnimator;
	private float maxHP;
	private float currentHP;
	private bool destroyed;
	private bool over;
	private Color waveColorDamage;
	private Color waveColorRecovery;
	private float waveFrontLimit;
	private Vector4 waveLengths;
	private Vector4 waveAlphas;
	private float waveSpeed;
	private float debrisSlotDamage;
	private float debrisSlotDestruction;
	private GameObject[] debrisObjects;
	private SimpleAnimator[] debrisAnimators;
	private float debrisDepth;
	private float colliderWidth;
	private float colliderHeight;
	private float debrisExtraDelay;
	private float debrisExtraElapsed;
	private VFXParticleController particleController;

	private AudioSource audioSourceDamage;
	private AudioClip[] damageInSounds;
	private AudioClip[] destructionSounds;
	private int[] damageInSoundIDs;
	private int[] destructionSoundIDs;

	void Awake()
	{
		obstacleCollider = GetComponent<Collider2D>();
		obstacleAnimator = GetComponent<ShortLifeAnimator>();
		maxHP = (obstacleHP > 0f) ? obstacleHP : 1f;
		currentHP = maxHP;
		destroyed = false;
		over = false;
		damageWaveLength = (damageWaveLength < 0f) ? 0f : damageWaveLength;
		debrisDensityDamage = (debrisDensityDamage < 0f) ? 0f : debrisDensityDamage;
		debrisDensityDestruction = (debrisDensityDestruction < 0f) ? 0f : debrisDensityDestruction;
		waveColorDamage = Color.white;
		waveColorRecovery = new Color( 0.8f, 1f, 0.8f, 1f );
		waveFrontLimit = 3f * damageWaveLength;
		waveLengths = new Vector4( damageWaveLength, damageWaveLength/4f, damageWaveLength/8f, damageWaveLength/12f );
		waveAlphas = new Vector4( 1f, 0.7f, 0.45f, 0.2f );
		waveSpeed = 10f * damageWaveLength;
		debrisSlotDamage = (debrisDensityDamage > 0f) ? (1f / debrisDensityDamage) : 0f;
		debrisSlotDestruction = (debrisDensityDestruction > 0f) ? (1f / debrisDensityDestruction) : 0f;
		debrisObjects = null;
		debrisAnimators = null;
		debrisDepth = DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / BaseAnimator.MAX_ANIMATORS_WITHIN_LAYER;
		colliderWidth = 0f;
		colliderHeight = 0f;
		if( obstacleCollider != null )
		{
			colliderWidth = obstacleCollider.bounds.size.x;
			colliderHeight = obstacleCollider.bounds.size.y;
		}
		debrisExtraDelay = (extraDebrisDelay > 0f) ? extraDebrisDelay : 0f;
		debrisExtraElapsed = 0f;
		particleController = GetComponent<VFXParticleController>();

		audioSourceDamage = null;
		damageInSounds = null;
		damageInSoundIDs = null;
		destructionSounds = null;
		destructionSoundIDs = null;
		/*halmeida - the AudioCenter is a plugin for reducing latency of sounds in Android devices. It also supports the sound
		playback outside of the Android environment, but that support is really poor, with a single AudioSource. Because
		of that, I don't wanna use it unless we are within the android environment.*/
		#if !UNITY_ANDROID || UNITY_EDITOR
			AddAudioSources();
			LoadAllSoundsFromResources();
		#else
			LoadAllSoundsIntoAudioCenter();
		#endif
	}

	public void Progress( float timeStep )
	{
		SimpleAnimator debrisAnimator = null;

		if( !over )
		{
			if( debrisAnimators != null )
			{
				for( int i=0; i<debrisAnimators.Length; i++ )
				{
					debrisAnimator = debrisAnimators[i];
					debrisAnimator.Progress( timeStep );
					if( debrisAnimator.IsOver() )
					{
						RemoveDebris( i );
						if( debrisAnimators == null )
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
			if( destroyed )
			{
				if( debrisExtraDelay > 0f )
				{
					debrisExtraElapsed += timeStep;
					if( debrisExtraElapsed > debrisExtraDelay )
					{
						CreateAllDebris( debrisSlotDestruction );
						debrisExtraDelay = 0f;
					}
				}
			}
			if( obstacleAnimator != null )
			{
				obstacleAnimator.Progress( timeStep );
				if( obstacleAnimator.IsDead() )
				{
					/*halmeida - If the animations of the animator are not propperly configured, the animator
					may die before we ask it to, which means it will die visually, but it will still be working
					normally in terms of purpose.*/ 
					if( !destroyed )
					{
						DestroyObstacle();
					}
					/*halmeida - only after making sure it's propperly disabled we end it.*/
					over = true;
				}
			}
		}
	}

	private void DamageObstacle( float damagePower, GameObject damageSource )
	{
		if( !destroyed && (damagePower != 0f) )
		{
			currentHP -= damagePower;
			currentHP = (currentHP < 0f) ? 0f : currentHP;
			currentHP = (currentHP > maxHP) ? maxHP : currentHP;
			if( currentHP > 0f )
			{
				if( obstacleAnimator != null )
				{
					obstacleAnimator.SetLifeStage( currentHP / maxHP );
					if( damageSource != null )
					{
						if( damagePower > 0f )
						{
							obstacleAnimator.StartWave( waveColorDamage, damageSource.transform.localPosition, waveLengths, waveAlphas,
								waveFrontLimit, waveSpeed, 0f, 0f, 0f );
						}
						else
						{
							obstacleAnimator.StartWave( waveColorRecovery, damageSource.transform.localPosition, waveLengths, waveAlphas,
								waveFrontLimit, waveSpeed, 0f, 0f, 0f );
						}
					}
					if( obstacleAnimator.LifeStageChangeScheduled() && (particleController != null) && (damagePower > 0f) )
					{
						particleController.RequestParticleCreation();
					}
				}
				if( damagePower > 0f )
				{
					CreateAllDebris( debrisSlotDamage );
				}
			}
			else
			{
				if( damagePower > 0f )
				{
					DestroyObstacle();
					if( obstacleAnimator != null )
					{
						if( damageSource != null )
						{
							obstacleAnimator.StartWave( waveColorDamage, damageSource.transform.localPosition, waveLengths, waveAlphas,
								waveFrontLimit, waveSpeed, 0f, 0f, 0f );
						}
						if( particleController != null )
						{
							particleController.RequestParticleCreation();
						}
					}
					CreateAllDebris( debrisSlotDestruction );
				}
			}
		}
	}

	private void DestroyObstacle()
	{
		if( obstacleCollider != null )
		{
			obstacleCollider.enabled = false;
		}
		if( obstacleAnimator != null )
		{
			obstacleAnimator.StartDying();
		}
		destroyed = true;
	}

	private void CreateAllDebris( float slotSize )
	{
		float widthLeft = 0f;
		float heightLeft = 0f;
		float widthTraveled = 0f;
		float heightTraveled = 0f;
		float halfSlotSize = 0f;
		float actualSlotWidth = 0f;
		float actualSlotHeight = 0f;
		float debrisOffsetX = 0f;
		float debrisOffsetY = 0f;

		Debug.Log("Debug : ObstacleController : creating debris with slot size = "+slotSize+".");
		if( (slotSize > 0f) && (obstacleCollider != null) && (debrisModel != null) )
		{
			/*halmeida - we will divide the obstacle's area into slots. Each slot will receive a debris piece at a random
			point within its area. The origin of the slots is at the top left of the obstacle's area.*/
			halfSlotSize = slotSize / 2f;
			widthLeft = colliderWidth;
			heightLeft = colliderHeight;
			Debug.Log("Debug : ObstacleController : collider width = "+widthLeft+", height = "+heightLeft+".");
			while( heightLeft > 0f )
			{
				if( heightLeft > halfSlotSize )
				{
					actualSlotHeight = (heightLeft > slotSize) ? slotSize : heightLeft;
					while( widthLeft > 0f )
					{
						if( widthLeft > halfSlotSize )
						{
							actualSlotWidth = (widthLeft > slotSize) ? slotSize : widthLeft;
							debrisOffsetX = widthTraveled + Random.Range( 0f, actualSlotWidth );
							debrisOffsetY = -1f * (heightTraveled + Random.Range( 0f, actualSlotHeight ));
							CreateDebris( debrisOffsetX, debrisOffsetY );
							widthTraveled += actualSlotWidth;
							widthLeft -= actualSlotWidth;
						}
						else
						{
							widthLeft = 0f;
						}
					}
					heightTraveled += actualSlotHeight;
					heightLeft -= actualSlotHeight;
					widthLeft = colliderWidth;
					widthTraveled = 0f;
				}
				else
				{
					heightLeft = 0f;
				}
			}
		}
	}

	private void CreateDebris( float offsetX, float offsetY )
	{
		GameObject newDebrisObject = null;
		SimpleAnimator newDebrisAnimator = null;
		float originX = 0f;
		float originY = 0f;

		newDebrisObject = Instantiate( debrisModel, Vector3.zero, Quaternion.identity ) as GameObject;
		if( newDebrisObject != null )
		{
			newDebrisAnimator = newDebrisObject.GetComponent<SimpleAnimator>();
			if( newDebrisAnimator != null )
			{
				newDebrisObject.transform.SetParent( transform, false );
				originX = -1f * (colliderWidth / 2f);
				originY = colliderHeight / 2f;
				newDebrisObject.transform.localPosition = new Vector3( originX + offsetX, originY + offsetY, -debrisDepth );
				UsefulFunctions.IncreaseArray<GameObject>( ref debrisObjects, newDebrisObject );
				UsefulFunctions.IncreaseArray<SimpleAnimator>( ref debrisAnimators, newDebrisAnimator );
				Debug.Log("Debug : ObstacleController : created debris with offsets ("+offsetX+", "+offsetY+").");
			}
			else
			{
				Destroy( newDebrisObject );
			}
		}
	}

	private void RemoveDebris( int debrisIndex )
	{
		if( debrisAnimators != null )
		{
			if( (debrisIndex > -1) && (debrisIndex < debrisAnimators.Length) )
			{
				/*halmeida - no need to check if animator is null cause it never will be.*/
				debrisAnimators[debrisIndex].Clear();
				debrisAnimators[debrisIndex] = null;
				/*halmeida - relying on the coherence of all the "debris" arrays.*/
				Destroy( debrisObjects[debrisIndex] );
				debrisObjects[debrisIndex] = null;
				UsefulFunctions.DecreaseArray<SimpleAnimator>( ref debrisAnimators, debrisIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref debrisObjects, debrisIndex );
			}
		}
	}

	private void OnTriggerEnter2D( Collider2D otherCollider )
	{
		ProjectileController projectile = null;
		int destructionLevel = CONSTRUCTION_LEVEL_INVALID;

		//Debug.Log("Debug : ObstacleController : on trigger enter.");
		if( otherCollider.gameObject != null )
		{
			projectile = otherCollider.gameObject.GetComponent<ProjectileController>();
			if( projectile != null )
			{
				destructionLevel = projectile.GetDestructionLevel();
				if( (destructionLevel != CONSTRUCTION_LEVEL_INVALID) && (destructionLevel >= constructionLevel) )
				{
					/*halmeida - in this case, the projectile may affect the obstacle.*/
					if( projectile.GetEffectCode() == ProjectileController.EFFECT_CODE_HP_DAMAGE )
					{
						DamageObstacle( projectile.GetEffectIntensity(), otherCollider.gameObject );
					}
					else if( projectile.GetEffectCode() == ProjectileController.EFFECT_CODE_HP_RECOVERY )
					{
						DamageObstacle( projectile.GetEffectIntensity() * -1f, otherCollider.gameObject );
					}
				}
				projectile.Deactivate();
			}
		}
	}

	public void Clear()
	{
		/*halmeida - no need to clear the particle controller because it will be done when clearing the
		animator of the obstacle. Instead we just point away from it.*/
		/*if( particleController != null )
		{
			particleController.Clear();
			particleController = null;
		}*/
		particleController = null;
		if( debrisAnimators != null )
		{
			for( int i=0; i<debrisAnimators.Length; i++ )
			{
				debrisAnimators[i].Clear();
				debrisAnimators[i] = null;
				Destroy( debrisObjects[i] );
				debrisObjects[i] = null;
			}
			debrisAnimators = null;
			debrisObjects = null;
		}
		obstacleCollider = null;
		if( obstacleAnimator != null )
		{
			obstacleAnimator.Clear();
			obstacleAnimator = null;
		}
		ClearAllSoundsFromResources();
		ClearAllSoundsFromAudioCenter();
	}

	public bool IsDestroyed()
	{
		return destroyed;
	}

	public bool IsOver()
	{
		return over;
	}

	private void AddAudioSources()
	{
		audioSourceDamage = gameObject.AddComponent<AudioSource>();
	}

	private void LoadAllSoundsFromResources()
	{
		UsefulFunctions.LoadSoundsFromResources( damageInSoundPaths, ref damageInSounds );
		UsefulFunctions.LoadSoundsFromResources( destructionSoundPaths, ref destructionSounds );
	}

	private void ClearAllSoundsFromResources()
	{
		UsefulFunctions.ClearSoundsFromResources( ref damageInSounds );
		UsefulFunctions.ClearSoundsFromResources( ref destructionSounds );
	}

	private void LoadAllSoundsIntoAudioCenter()
	{
		UsefulFunctions.LoadSoundsIntoAudioCenter( damageInSoundPaths, ref damageInSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( destructionSoundPaths, ref destructionSoundIDs );
	}

	private void ClearAllSoundsFromAudioCenter()
	{
		UsefulFunctions.ClearSoundsFromAudioCenter( ref damageInSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref destructionSoundIDs );
	}

	private void PlaySoundEffect( int soundFXCategory )
	{
		int soundIndex = -1;

		#if !UNITY_ANDROID || UNITY_EDITOR
			AudioClip[] availableSounds = null;
			AudioClip chosenSound = null;
			AudioSource propperSource = null;

			propperSource = audioSourceDamage;
			switch( soundFXCategory )
			{
				case SOUND_FX_CATEGORY_DAMAGE_IN:
					availableSounds = damageInSounds;
					break;
				case SOUND_FX_CATEGORY_DESTRUCTION:
					availableSounds = destructionSounds;
					break;
			}
			if( (propperSource != null) && (availableSounds != null) )
			{
				if( availableSounds.Length > 0 )
				{
					soundIndex = Random.Range( 0, availableSounds.Length );
					chosenSound = availableSounds[soundIndex];
					if( chosenSound != null )
					{
						propperSource.clip = chosenSound;
						propperSource.Play();
					}
				}
			}
		#else
			int[] availableSoundIDs = null;
			int soundID = -1;

			switch( soundFXCategory )
			{
				case SOUND_FX_CATEGORY_DAMAGE_IN:
					availableSoundIDs = damageInSoundIDs;
					break;
				case SOUND_FX_CATEGORY_DESTRUCTION:
					availableSoundIDs = destructionSoundIDs;
					break;
			}
			if( availableSoundIDs != null )
			{
				if( availableSoundIDs.Length > 0 )
				{
					soundIndex = Random.Range( 0, availableSoundIDs.Length );
					soundID = availableSoundIDs[soundIndex];
					if( soundID != -1 )
					{
						AudioCenter.playSound( soundID, 1f );
					}
				}
			}
		#endif
	}
}
