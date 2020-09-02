using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : RoutedAgent
{
	public const float FACTOR_LOW_HP = 0.3f;
	public const float FACTOR_HEAL_WITH_VFX = 0.01f;
	public const float DEFAULT_PROJECTILE_COST = 13.8f;
	public const float DEFAULT_MP_RECOVERY_RATE = 0.7f;
	public const float DEFAULT_MP_RECOVERY_WAIT = 1f;

	public enum EquipAttribute { Accel, MaxSpeed, MaxHP, MaxMP, Attack, MaxFrequency }

	public enum EquipBodyPart { Head, Neck, Torso, Arms, Legs }

	public GameObject gaugeHPModel;
	public int originalMaxMP;
	public GameObject gaugeMPModel;
	public GameObject[] shadowConfigPerSkill;
	public int originalLevelUpXP;
	public float levelUpXPFactor;
	public float levelUpAccelFactor;
	public float levelUpSpeedFactor;
	public float levelUpHPFactor;
	public float levelUpMPFactor;
	public float levelUpAttackFactor;
	public float levelUpFrequencyFactor;
	public int levelCap;
	public GameObject[] waveEmitterModels;
	public float[] waveEmitterExtraDepths;
	public int[] levelUpWaveIndexes;
	public GameObject[] equipmentModels;
	public int[] equipmentItemIDs;
	public int[] equipmentDisplayPriorities;
	public string[] coinSoundPaths;
	public string[] breakSoundPaths;
	public string[] positiveSoundPaths;
	public string[] negativeSoundPaths;
	public string[] itemSoundPaths;
	public string[] fadeSoundPaths;

	private float baseMaxMP;
	private float finalMaxMP;
	private float currentMP;
	private float projectileMPCost;
	private float autoMPRecoveryRate;
	private float autoMPRecoveryWait;
	private float autoMPRecoveryWaited;
	private bool lowHP;
	private GameObject gaugeHPObject;
	private GameObject gaugeMPObject;
	private UIGauge gaugeHP;
	private UIGauge gaugeMP;

	private KamiAnimator kamiAnim;
	private int animStatus;
	private ProjectileController fakeLightSource;
	private GameObject waveObject;
	private SimpleAnimator waveAnimator;
	private WaveParameters waveParams;
	private GameObject[] oneWaveObjects;
	private SimpleAnimator[] oneWaveAnimators;

	private int coins;
	private int ownedItemTypes;
	private int ownedItemTotal;
	private int[] ownedItemIDs;
	private int[] ownedItemUnits;
	private GameObject equipmentsParent;
	private int equippedTotal;
	private int[] equippedItemIDs;
	private EquipBodyPart[] equippedBodyParts;
	private int[] equippedListIndexes;
	private GameObject[] equippedObjects;
	private KamiAnimator[] equippedAnimators;
	private int[] equippedDisplayPriorities;
	private float unequippedDepth;
	private ItemDatabase itemDatabase;
	private float[] equippedAccelFactors;
	private float[] equippedMaxSpeedFactors;
	private float[] equippedMaxHPFactors;
	private float[] equippedMaxMPFactors;
	private float[] equippedAttackFactors;
	private float[] equippedMaxFrequencyFactors;
	private float equippedAccelTotal;
	private float equippedMaxSpeedTotal;
	private float equippedMaxHPTotal;
	private float equippedMaxMPTotal;
	private float equippedAttackTotal;
	private float equippedMaxFrequencyTotal;
	private bool skillShoot;
	private int skillBreak;

	private int maxLevel;
	private int currentLevel;
	private float levelUpXPRequirement;
	private int levelUpXPLeft;
	private int totalXP;
	private SymbolDatabase fontLevelUp;
	private Color fontColorLevelUp;
	private Vector4 fontGradientLevelUp;

	private AudioSource audioSourceItem;
	private AudioClip[] coinSounds;
	private AudioClip[] breakSounds;
	private AudioClip[] positiveSounds;
	private AudioClip[] negativeSounds;
	private AudioClip[] itemSounds;
	private AudioClip[] fadeSounds;
	private int[] coinSoundIDs;
	private int[] breakSoundIDs;
	private int[] positiveSoundIDs;
	private int[] negativeSoundIDs;
	private int[] itemSoundIDs;
	private int[] fadeSoundIDs;

	protected override void Awake()
	{
		baseMaxMP = 0f;
		finalMaxMP = 0f;
		currentMP = 0f;
		projectileMPCost = DEFAULT_PROJECTILE_COST;
		autoMPRecoveryRate = DEFAULT_MP_RECOVERY_RATE;
		autoMPRecoveryWait = DEFAULT_MP_RECOVERY_WAIT;
		autoMPRecoveryWaited = 0f;
		lowHP = true;
		gaugeHPObject = null;
		gaugeMPObject = null;
		gaugeHP = null;
		gaugeMP = null;

		kamiAnim = GetComponent<KamiAnimator>();
		animStatus = KamiAnimator.INVALID_STATUS;
		fakeLightSource = null;
		waveObject = null;
		waveAnimator = null;
		waveParams = null;
		ExtractWaveAnimator( 0 );
		oneWaveObjects = null;
		oneWaveAnimators = null;

		coins = 0;
		ownedItemTypes = 0;
		ownedItemTotal = 0;
		ownedItemIDs = null;
		ownedItemUnits = null;
		equipmentsParent = null;
		equipmentsParent = new GameObject("EquipmentsParent");
		equipmentsParent.transform.SetParent( transform, false );
		equipmentsParent.transform.localPosition = Vector3.zero;
		equippedTotal = 0;
		equippedItemIDs = null;
		equippedBodyParts = null;
		equippedListIndexes = null;
		equippedObjects = null;
		equippedAnimators = null;
		equippedDisplayPriorities = null;
		unequippedDepth = gameObject.transform.position.z;
		itemDatabase = null;
		equippedAccelFactors = null;
		equippedMaxSpeedFactors = null;
		equippedMaxHPFactors = null;
		equippedMaxMPFactors = null;
		equippedAttackFactors = null;
		equippedMaxFrequencyFactors = null;
		equippedAccelTotal = 0f;
		equippedMaxSpeedTotal = 0f;
		equippedMaxHPTotal = 0f;
		equippedMaxMPTotal = 0f;
		equippedAttackTotal = 0f;
		equippedMaxFrequencyTotal = 0f;
		skillShoot = false;
		skillBreak = ObstacleController.CONSTRUCTION_LEVEL_INVALID;

		currentLevel = 1;
		maxLevel = (levelCap < currentLevel) ? currentLevel : levelCap;
		levelUpXPRequirement = originalLevelUpXP;
		levelUpXPLeft = originalLevelUpXP;
		totalXP = 0;
		fontLevelUp = null;
		fontColorLevelUp = new Color( 1f, 0.9f, 0.7f, 1f );
		fontGradientLevelUp = new Vector4( 0f, 5f, 0f, 0f );

		audioSourceItem = null;
		coinSounds = null;
		breakSounds = null;
		positiveSounds = null;
		negativeSounds = null;
		itemSounds = null;
		fadeSounds = null;
		coinSoundIDs = null;
		breakSoundIDs = null;
		positiveSoundIDs = null;
		negativeSoundIDs = null;
		itemSoundIDs = null;
		fadeSoundIDs = null;
		base.Awake();
	}

	protected virtual void ExtractWaveAnimator( int modelIndex )
	{
		Vector3 waveEmitterLocalPosition = Vector3.zero;
		GameObject waveEmitterModel = null;

		ClearWaves( true, false );
		if( waveEmitterModels != null )
		{
			if( waveEmitterModels.Length > modelIndex )
			{
				waveEmitterModel = waveEmitterModels[modelIndex];
				if( waveEmitterModel != null )
				{
					waveObject = Instantiate( waveEmitterModel, Vector3.zero, Quaternion.identity ) as GameObject;
					if( waveObject != null )
					{
						waveAnimator = waveObject.GetComponent<SimpleAnimator>();
						if( waveAnimator != null )
						{
							waveParams = waveObject.GetComponent<WaveParameters>();
							if( waveParams != null )
							{
								waveObject.transform.SetParent( transform, false );
								if( waveEmitterExtraDepths != null )
								{
									if( waveEmitterExtraDepths.Length > modelIndex )
									{
										waveEmitterLocalPosition.z = waveEmitterExtraDepths[modelIndex];
									}
								}
								waveObject.transform.localPosition = waveEmitterLocalPosition;
							}
						}
						if( waveParams == null )
						{
							ClearWaves( true, false );
						}
					}
				}
			}
		}
	}

	private void SetBaseMaxMP( float newMaxMP, bool updateFinal )
	{
		baseMaxMP = (newMaxMP < 0f) ? -newMaxMP : newMaxMP;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected override void SetBaseStatsToOriginals( bool updateFinal )
	{
		SetBaseMaxMP( originalMaxMP, false );
		base.SetBaseStatsToOriginals( updateFinal );
	}

	protected override void UpdateFinalStats()
	{
		float currentFilledRate = 0f;

		finalAccel = baseAccel + (equippedAccelTotal * originalAccel);
		finalMaxSpeed = baseMaxSpeed + (equippedMaxSpeedTotal * originalMaxSpeed);
		finalMaxSqrSpeed = finalMaxSpeed * finalMaxSpeed;
		forcingFinalMaxSpeed = false;
		if( finalMaxHP > 0 )
		{
			currentFilledRate = (float)currentHP / (float)finalMaxHP;
		}
		finalMaxHP = (int)(baseMaxHP + (equippedMaxHPTotal * originalMaxHP));
		if( currentFilledRate > 0f )
		{
			SetCurrentHP( (int)(currentFilledRate * finalMaxHP) );
		}
		else
		{
			SetCurrentHP( finalMaxHP );
		}
		currentFilledRate = 0f;
		if( finalMaxMP > 0f )
		{
			currentFilledRate = currentMP / finalMaxMP;
		}
		finalMaxMP = baseMaxMP + (equippedMaxMPTotal * originalMaxMP);
		if( currentFilledRate > 0f )
		{
			SetCurrentMP( currentFilledRate * finalMaxMP );
		}
		else
		{
			SetCurrentMP( finalMaxMP );
		}
		finalAttack = baseAttack + (equippedAttackTotal * originalAttack);
		finalMaxFrequency = baseMaxFrequency + (equippedMaxFrequencyTotal * originalMaxFrequency);
		if( finalMaxFrequency > 0f )
		{
			projectileInterval = 1f / finalMaxFrequency;
		}
		else
		{
			projectileInterval = 0f;
		}
	}

	protected override void SetCurrentHP( int newCurrentHP )
	{
		int previousHP = 0;
		float recoveredFraction = 0f;

		previousHP = currentHP;
		base.SetCurrentHP( newCurrentHP );
		lowHP = ( currentHP < FACTOR_LOW_HP * finalMaxHP );
		if( gaugeHP != null )
		{
			gaugeHP.SetCurrentValue( currentHP );
		}
		if( (currentHP == finalMaxHP) && !paused && (kamiAnim != null) )
		{
			recoveredFraction = (float)(finalMaxHP - previousHP) / (float)finalMaxHP;
			if( recoveredFraction > FACTOR_HEAL_WITH_VFX )
			{
				kamiAnim.CreateFullHealShadow();
			}
		}
	}

	protected override void DisplayDamageText( int damageValue )
	{
		if( cameraController != null )
		{
			cameraController.AddInterfaceCanvasText( ""+damageValue, fontDamage, fontColorDamage, fontGradientDamage,
				CameraController.TEXT_EVOLUTION_ASCENT, transform, new Vector2( 0f, 0.45f ), false );
		}
	}

	private void SetCurrentMP( float newCurrentMP )
	{
		currentMP = newCurrentMP;
		if( currentMP < 0f )
		{
			currentMP = 0f;
		}
		if( currentMP > finalMaxMP )
		{
			currentMP = finalMaxMP;
		}
		if( gaugeMP != null )
		{
			gaugeMP.SetCurrentValue( currentMP );
		}
	}

	public override void ReceiveExperience( int experiencePoints, bool withVFX )
	{
		int previousLevel = 0;

		//Debug.Log("Debug : PlayerAgent : receiving "+experiencePoints+" experience points.");
		previousLevel = currentLevel;
		while( experiencePoints > 0 )
		{
			if( experiencePoints >= levelUpXPLeft )
			{
				experiencePoints -= levelUpXPLeft;
				totalXP += levelUpXPLeft;
				levelUpXPLeft = 0;
				if( currentLevel < maxLevel )
				{
					LevelUp( withVFX );
				}
				else
				{
					experiencePoints = 0;
				}
			}
			else
			{
				levelUpXPLeft -= experiencePoints;
				totalXP += experiencePoints;
				experiencePoints = 0;
			}
		}
		//Debug.Log("Debug : PlayerAgent : total experience = "+totalXP+".");
		//Debug.Log("Debug : PlayerAgent : experience left to level up = "+levelUpXPLeft+".");
		if( withVFX && (currentLevel > previousLevel) && (challengeStage != null) )
		{
			TogglePause( true );
			if( kamiAnim != null )
			{
				kamiAnim.CreateLevelUpShadows();
			}
			challengeStage.AddPausingAgent( this );
		}
	}

	private void LevelUp( bool withVFX )
	{
		if( currentLevel < maxLevel )
		{
			SetBaseAccel( baseAccel * levelUpAccelFactor, false );
			SetBaseMaxSpeed( baseMaxSpeed * levelUpSpeedFactor, false );
			SetBaseMaxHP( baseMaxHP * levelUpHPFactor, false );
			SetBaseMaxMP( baseMaxMP * levelUpMPFactor, false );
			SetBaseAttack( baseAttack * levelUpAttackFactor, false );
			SetBaseMaxFrequency( baseMaxFrequency * levelUpFrequencyFactor, false );
			UpdateFinalStats();
			currentLevel++;
			levelUpXPRequirement *= levelUpXPFactor;
			levelUpXPLeft = (int) levelUpXPRequirement;
			if( withVFX )
			{
				CreateLevelUpWaves();
				DisplayLevelUpText();
			}
			//PrintStats();
		}
	}

	private void CreateLevelUpWaves()
	{
		int levelUpWaveIndex = -1;
		GameObject waveEmitterModel = null;
		GameObject oneWaveObject = null;
		SimpleAnimator oneWaveAnimator = null;
		WaveParameters oneWaveParams = null;
		Vector3 waveEmitterLocalPosition = Vector3.zero;

		if( (waveEmitterModels != null) && (levelUpWaveIndexes != null) )
		{
			for( int i=0; i<levelUpWaveIndexes.Length; i++ )
			{
				levelUpWaveIndex = levelUpWaveIndexes[i];
				if( (levelUpWaveIndex > -1) && (levelUpWaveIndex < waveEmitterModels.Length) )
				{
					waveEmitterModel = waveEmitterModels[levelUpWaveIndex];
					if( waveEmitterModel != null )
					{
						oneWaveObject = Instantiate( waveEmitterModel, Vector3.zero, Quaternion.identity ) as GameObject;
						if( oneWaveObject != null )
						{
							oneWaveParams = null;
							oneWaveAnimator = oneWaveObject.GetComponent<SimpleAnimator>();
							if( oneWaveAnimator != null )
							{
								oneWaveParams = oneWaveObject.GetComponent<WaveParameters>();
								if( oneWaveParams != null )
								{
									oneWaveObject.transform.SetParent( transform, false );
									if( waveEmitterExtraDepths != null )
									{
										if( waveEmitterExtraDepths.Length > levelUpWaveIndex )
										{
											waveEmitterLocalPosition.z = waveEmitterExtraDepths[levelUpWaveIndex];
										}
									}
									oneWaveObject.transform.localPosition = waveEmitterLocalPosition;
									oneWaveAnimator.StartWave( oneWaveParams.waveColor, oneWaveObject.transform.position, oneWaveParams.waveLengths,
										oneWaveParams.waveAlphas, oneWaveParams.waveFrontLimit, oneWaveParams.waveSpeed,
										oneWaveParams.waveLengthVariation, 1f, 0f, oneWaveParams.waveNoise, oneWaveParams.waveNoiseScale,
										oneWaveParams.waveDelay );
									UsefulFunctions.IncreaseArray<GameObject>( ref oneWaveObjects, oneWaveObject );
									UsefulFunctions.IncreaseArray<SimpleAnimator>( ref oneWaveAnimators, oneWaveAnimator );
								}
							}
							if( oneWaveParams == null )
							{
								if( oneWaveAnimator != null )
								{
									oneWaveAnimator.Clear();
								}
								Destroy( oneWaveObject );
							}
						}
					}
				}
			}
		}
	}

	public void SetLevelUpDisplayFont( SymbolDatabase newFontLevelUp )
	{
		fontLevelUp = newFontLevelUp;
	}

	private void DisplayLevelUpText()
	{
		if( cameraController != null )
		{
			cameraController.AddInterfaceCanvasText( "LEVEL UP", fontLevelUp, fontColorLevelUp, fontGradientLevelUp,
				CameraController.TEXT_EVOLUTION_STAY, transform, new Vector2( 0f, 0.75f ), true );
		}
	}

	public void DisplayStats()
	{
		Vector2 worldPosition = Vector2.zero;

		if( cameraController != null )
		{
			worldPosition.x = cameraController.GetCameraObjectPosition().x - 2f;
			worldPosition.y = cameraController.GetCameraObjectPosition().y + 0.5f;
			cameraController.AddInterfaceCanvasText( "level "+currentLevel+".", fontLevelUp, fontColorLevelUp, fontGradientLevelUp,
				CameraController.TEXT_EVOLUTION_NONE_WAIT, worldPosition, true );
			worldPosition.y -= 0.5f;
			cameraController.AddInterfaceCanvasText( "accel---- "+UsefulFunctions.GetPercentage( baseAccel, originalAccel )+"%",
				fontLevelUp, fontColorLevelUp, fontGradientLevelUp, CameraController.TEXT_EVOLUTION_NONE_WAIT, worldPosition, true );
			worldPosition.y -= 0.5f;
			cameraController.AddInterfaceCanvasText( "max speed "+UsefulFunctions.GetPercentage( baseMaxSpeed, originalMaxSpeed )+"%",
				fontLevelUp, fontColorLevelUp, fontGradientLevelUp, CameraController.TEXT_EVOLUTION_NONE_WAIT, worldPosition, true );
			worldPosition.y -= 0.5f;
			cameraController.AddInterfaceCanvasText( "max hp--- "+UsefulFunctions.GetPercentage( baseMaxHP, originalMaxHP )+"%",
				fontLevelUp, fontColorLevelUp, fontGradientLevelUp, CameraController.TEXT_EVOLUTION_NONE_WAIT, worldPosition, true );
			worldPosition.y -= 0.5f;
			cameraController.AddInterfaceCanvasText( "attack--- "+UsefulFunctions.GetPercentage( baseAttack, originalAttack )+"%",
				fontLevelUp, fontColorLevelUp, fontGradientLevelUp, CameraController.TEXT_EVOLUTION_NONE_WAIT, worldPosition, true );
		}
	}

	public void CloseStatsDisplay()
	{
		if( cameraController != null )
		{
			cameraController.AllowWaitingTextsToEnd();
		}
	}

	private void PrintStats()
	{
		Debug.Log("Debug : PlayerAgent : level "+currentLevel+".");
		Debug.Log("Debug : PlayerAgent : accel---- = "+UsefulFunctions.GetPercentage( baseAccel, originalAccel )+"%");
		Debug.Log("Debug : PlayerAgent : max speed = "+UsefulFunctions.GetPercentage( baseMaxSpeed, originalMaxSpeed )+"%");
		Debug.Log("Debug : PlayerAgent : max hp--- = "+UsefulFunctions.GetPercentage( baseMaxHP, originalMaxHP )+"%");
		Debug.Log("Debug : PlayerAgent : max mp--- = "+UsefulFunctions.GetPercentage( baseMaxMP, originalMaxMP )+"%");
		Debug.Log("Debug : PlayerAgent : attack--- = "+UsefulFunctions.GetPercentage( baseAttack, originalAttack )+"%");
		Debug.Log("Debug : PlayerAgent : frequency = "+UsefulFunctions.GetPercentage( baseMaxFrequency, originalMaxFrequency )+"%");
		Debug.Log("Debug : PlayerAgent : XP to level up = "+levelUpXPLeft+".");
	}

	public void GetStatValueSegments( EquipAttribute attribute, ref float originalValue, ref float baseValue, ref float equippedFactorSum )
	{
		switch( attribute )
		{
			case EquipAttribute.Accel:
				originalValue = originalAccel;
				baseValue = baseAccel;
				equippedFactorSum = equippedAccelTotal;
				break;
			case EquipAttribute.MaxSpeed:
				originalValue = originalMaxSpeed;
				baseValue = baseMaxSpeed;
				equippedFactorSum = equippedMaxSpeedTotal;
				break;
			case EquipAttribute.MaxHP:
				originalValue = originalMaxHP;
				baseValue = baseMaxHP;
				equippedFactorSum = equippedMaxHPTotal;
				break;
			case EquipAttribute.MaxMP:
				originalValue = originalMaxMP;
				baseValue = baseMaxMP;
				equippedFactorSum = equippedMaxMPTotal;
				break;
			case EquipAttribute.Attack:
				originalValue = originalAttack;
				baseValue = baseAttack;
				equippedFactorSum = equippedAttackTotal;
				break;
			case EquipAttribute.MaxFrequency:
				break;
		}
	}

	public override int GetTotalExperiencePoints() 
	{
		return totalXP;
	}

	public override void ResetExperience( bool resetStats )
	{
		currentLevel = 1;
		levelUpXPRequirement = originalLevelUpXP;
		levelUpXPLeft = originalLevelUpXP;
		totalXP = 0;
		if( resetStats )
		{
			SetBaseStatsToOriginals( true );
		}
	}

	protected override ProjectileController LaunchProjectile( GameObject projectileModel, Vector2 normalizedDirection, bool attacking,
		float newProjectileSpeed, float newProjectileAcceleration, float newProjectileMaxSpeed, float newProjectileDuration )
	{
		ProjectileController launchedProjectile = null;

		launchedProjectile = base.LaunchProjectile( projectileModel, normalizedDirection, attacking, newProjectileSpeed,
			newProjectileAcceleration, newProjectileMaxSpeed, newProjectileDuration );
		if( launchedProjectile != null )
		{
			fakeLightSource = launchedProjectile;
		}
		return launchedProjectile;
	}

	protected override void RemoveProjectileInfluence( ProjectileController projectile )
	{
		if( (projectile != null) && (fakeLightSource != null) )
		{
			if( projectile.gameObject == fakeLightSource.gameObject )
			{
				if( kamiAnim != null )
				{
					kamiAnim.RemoveFakeLight();
				}
				fakeLightSource = null;
			}
		}
	}

	protected override void ProgressVisualEffects (float timeStep)
	{
		Vector3 fakeLightOffset = Vector3.zero;
		FakeLight fakeLight = null;

		base.ProgressVisualEffects (timeStep);
		if( (fakeLightSource != null) && (kamiAnim != null) )
		{
			fakeLightOffset = fakeLightSource.gameObject.transform.localPosition - transform.localPosition;
			fakeLight = fakeLightSource.GetCurrentFakeLight();
			if( fakeLight != null )
			{
				kamiAnim.SetFakeLight( fakeLight.color, new Vector2(fakeLightOffset.x, fakeLightOffset.y), fakeLight.GetRadius(),
					fakeLight.GetIntensity() );
			}
			else
			{
				kamiAnim.RemoveFakeLight();
			}
		}
	}

	public void CreateGauges()
	{
		CreateHPGauge();
		if( skillShoot )
		{
			CreateMPGauge();
		}
	}

	private void CreateHPGauge()
	{
		if( (gaugeHPModel != null) && (gaugeHPObject == null) && (cameraController != null) )
		{
			gaugeHPObject = Instantiate( gaugeHPModel ) as GameObject;
			if( gaugeHPObject != null )
			{
				gaugeHP = gaugeHPObject.GetComponent<UIGauge>();
				if( gaugeHP == null )
				{
					Destroy( gaugeHPObject );
					gaugeHPObject = null;
				}
				else
				{
					if( cameraController.AddCanvasGauge( gaugeHP, finalMaxHP, 0f ) )
					{
						gaugeHP.SetCurrentValue( currentHP );
					}
					else
					{
						gaugeHP.Clear();
						gaugeHP = null;
						Destroy( gaugeHPObject );
						gaugeHPObject = null;
					}
				}
			}
		}
	}

	private void CreateMPGauge()
	{
		if( (gaugeMPModel != null) && (gaugeMPObject == null) && (cameraController != null) )
		{
			gaugeMPObject = Instantiate( gaugeMPModel ) as GameObject;
			if( gaugeMPObject != null )
			{
				gaugeMP = gaugeMPObject.GetComponent<UIGauge>();
				if( gaugeMP == null )
				{
					Destroy( gaugeMPObject );
					gaugeMPObject = null;
				}
				else
				{
					if( cameraController.AddCanvasGauge( gaugeMP, finalMaxMP, 0f ) )
					{
						gaugeMP.SetCurrentValue( currentMP );
					}
					else
					{
						gaugeMP.Clear();
						gaugeMP = null;
						Destroy( gaugeMPObject );
						gaugeMPObject = null;
					}
				}
			}
		}
	}

	public void HideGauges()
	{
		if( gaugeHP != null )
		{
			gaugeHP.ToggleVisibility( false );
		}
		if( gaugeMP != null )
		{
			gaugeMP.ToggleVisibility( false );
		}
	}

	public void ShowGauges()
	{
		if( gaugeHP != null )
		{
			gaugeHP.ToggleVisibility( true );
		}
		if( gaugeMP != null )
		{
			gaugeMP.ToggleVisibility( true );
		}
	}

	public void ClearGauges()
	{
		ClearHPGauge();
		ClearMPGauge();
	}

	private void ClearHPGauge()
	{
		if( gaugeHP != null )
		{
			gaugeHP.Clear();
			gaugeHP = null;
		}
		if( gaugeHPObject != null )
		{
			Destroy( gaugeHPObject );
			gaugeHPObject = null;
		}
	}

	private void ClearMPGauge()
	{
		if( gaugeMP != null )
		{
			gaugeMP.Clear();
			gaugeMP = null;
		}
		if( gaugeMPObject != null )
		{
			Destroy( gaugeMPObject );
			gaugeMPObject = null;
		}
	}

	protected override void ProgressAutoRecovery( float timeStep )
	{
		if( autoMPRecoveryWaited > -1f )
		{
			autoMPRecoveryWaited += timeStep;
			if( autoMPRecoveryWaited > autoMPRecoveryWait )
			{
				autoMPRecoveryWaited = -1f;
			}
		}
		else
		{
			if( currentMP < finalMaxMP )
			{
				SetCurrentMP( currentMP + timeStep * autoMPRecoveryRate * finalMaxMP );
			}
		}
	}

	protected override void ProvideAnimationDirectionMove( Vector2 newAnimationDirection )
	{
		if( kamiAnim != null )
		{
			kamiAnim.InterpretMovementDirection( newAnimationDirection );
		}
	}

	protected override void ProvideAnimationDirectionAim( Vector2 newAnimationDirection )
	{
		/*halmeida - do nothing for now.*/
	}

	protected override void ProvideAnimationRequestFade( bool fadeToTransparency, bool immediately, float fadeSpeed )
	{
		/*halmeida - first we correct the fade speed to serve the correct purpose.*/
		if( fadeSpeed > 0f )
		{
			if( fadeToTransparency )
			{
				fadeSpeed *= -1f;
			}
		}
		else
		{
			if( !fadeToTransparency )
			{
				fadeSpeed *= -1f;
			}
		}
		if( kamiAnim != null )
		{
			kamiAnim.StartAlphaFading( fadeSpeed, immediately, true, true );
		}
	}

	public override bool IsOpaque()
	{
		if( kamiAnim != null )
		{
			return kamiAnim.IsOpaque();
		}
		return false;
	}

	public override bool IsTransparent()
	{
		if( kamiAnim != null )
		{
			return kamiAnim.IsTransparent();
		}
		return true;
	}

	public override bool GetFacingSide()
	{
		if( kamiAnim != null )
		{
			return kamiAnim.GetSide();
		}
		return BaseAnimator.SIDE_RIGHT;
	}

	protected override void ProgressAnimation( float timeStep )
	{
		SimpleAnimator oneWaveAnimator = null;

		if( kamiAnim != null )
		{
			kamiAnim.Progress( timeStep );
			if( paused )
			{
				if( kamiAnim.IsPaused() && (kamiAnim.GetPausedShadowsAmount() < 1) )
				{
					shouldUnpause = true;
					/*halmeida - why not unpause the agent immediately? Cause an agent might have been paused
					because another one had something that needed to be done during a pause. If an agent has
					nothing to do during a pause it does not mean it should be unpaused. We just rise a flag
					and let the responsible class take a look and unpause the agent at the proper moment.*/
				}
			}
		}
		if( !paused )
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
		if( oneWaveAnimators != null )
		{
			for( int i=0; i<oneWaveAnimators.Length; i++ )
			{
				oneWaveAnimator = oneWaveAnimators[i];
				oneWaveAnimator.Progress( timeStep );
				if( !oneWaveAnimator.HasRunningWave() )
				{
					RemoveOneTimeWave( i );
					if( oneWaveAnimators == null )
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

	protected override void ResetAnimation( bool sideToFace )
	{
		Vector2 directionToFace = Vector2.zero;

		if( kamiAnim != null )
		{
			/*halmeida - make the animation face the side we need.*/
			directionToFace = ( sideToFace ? Vector2.right : Vector2.left );
			kamiAnim.FaceDirection( directionToFace );
			/*halmeida - ensure the animation is not going towards any direction.*/
			kamiAnim.InterpretMovementDirection( Vector2.zero );
			/*halmeida - ensure the animation sits correctly (i.e., at the right rotation for example).*/
			kamiAnim.Stabilize();
			kamiAnim.Progress( 0f );
		}
	}

	protected override void ToggleAnimationPause( bool pause )
	{
		if( kamiAnim != null )
		{
			kamiAnim.TogglePause( pause );
		}
	}

	protected override void ClearAnimation()
	{
		if( kamiAnim != null )
		{
			kamiAnim.Clear();
		}
		ClearWaves( true, true );
	}

	private void ClearWaves( bool standardWaves, bool oneTimeWaves )
	{
		SimpleAnimator oneWaveAnimator = null;
		GameObject oneWaveObject = null;

		if( standardWaves )
		{
			waveParams = null;
			if( waveAnimator != null )
			{
				waveAnimator.Clear();
				waveAnimator = null;
			}
			if( waveObject != null )
			{
				Destroy( waveObject );
				waveObject = null;
			}
		}
		if( oneTimeWaves )
		{
			if( oneWaveAnimators != null )
			{
				for( int i=0; i<oneWaveAnimators.Length; i++ )
				{
					oneWaveAnimator = oneWaveAnimators[i];
					if( oneWaveAnimator != null )
					{
						oneWaveAnimator.Clear();
						oneWaveAnimators[i] = null;
					}
				}
				oneWaveAnimators = null;
			}
			if( oneWaveObjects != null )
			{
				for( int i=0; i<oneWaveObjects.Length; i++ )
				{
					oneWaveObject = oneWaveObjects[i];
					if( oneWaveObject != null )
					{
						Destroy( oneWaveObject );
						oneWaveObjects[i] = null;
					}
				}
				oneWaveObjects = null;
			}
		}
	}

	private void RemoveOneTimeWave( int oneTimeWaveIndex )
	{
		SimpleAnimator oneWaveAnimator = null;
		GameObject oneWaveObject = null;

		if( oneWaveObjects != null )
		{
			if( (oneTimeWaveIndex > -1) && (oneTimeWaveIndex < oneWaveObjects.Length) )
			{
				oneWaveAnimator = oneWaveAnimators[oneTimeWaveIndex];
				if( oneWaveAnimator != null )
				{
					oneWaveAnimator.Clear();
					oneWaveAnimators[oneTimeWaveIndex] = null;
				}
				oneWaveObject = oneWaveObjects[oneTimeWaveIndex];
				if( oneWaveObject != null )
				{
					Destroy( oneWaveObject );
					oneWaveObjects[oneTimeWaveIndex] = null;
				}
				UsefulFunctions.DecreaseArray<SimpleAnimator>( ref oneWaveAnimators, oneTimeWaveIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref oneWaveObjects, oneTimeWaveIndex );
			}
		}
	}

	public override void PrepareForRetry( bool sideToFace )
	{
		base.PrepareForRetry( sideToFace );
		SetCurrentMP( finalMaxMP );
	}

	protected override bool ReactToValidAim( Vector2 normalizedDirection )
	{
		if( skillShoot && (currentMP >= projectileMPCost) )
		{
			if( base.ReactToValidAim( normalizedDirection ) )
			{
				SetCurrentMP( currentMP - projectileMPCost );
				autoMPRecoveryWaited = 0f;
				return true;
			}
		}
		return false;
	}

	public void SetItemDatabase( ItemDatabase newItemDatabase )
	{
		itemDatabase = newItemDatabase;
	}

	public void ObtainItem( int itemID, bool withSound = true )
	{
		ItemData itemData = null;

		if( itemDatabase != null )
		{
			itemData = itemDatabase.GetItemData( itemID );
			ObtainItem( itemData, withSound );
		}
	}

	public void ObtainItem( ItemData itemData, bool withSound = true )
	{
		if( itemData != null )
		{
			if( itemData.obtainable )
			{
				//Debug.Log("Debug : PlayerAgent : got item with ID = "+itemData.itemID+".");
				if( !itemData.inventoryKept )
				{
					UseItem( itemData );
				}
				else
				{
					AddItemToInventory( itemData.itemID );
					if( withSound && (itemDatabase != null) )
					{
						if( itemDatabase.DisplayItemInfo( ItemDatabase.ItemDisplayContext.Field, itemData ) && (challengeStage != null) )
						{
							challengeStage.PauseGameForUIItemInfo();
						}
					}
				}
				if( withSound )
				{
					if( itemData.effectType == ItemData.EffectType.Money )
					{
						PlaySoundEffect( SOUND_FX_CATEGORY_COIN );
					}
					else
					{
						PlaySoundEffect( SOUND_FX_CATEGORY_ITEM );
					}
				}

			}
		}
	}

	public void UseItem( ItemData itemData )
	{
		int skillValue = -1;

		if( itemData != null )
		{
			switch( itemData.effectType )
			{
				case ItemData.EffectType.Money:
					coins += (int)itemData.effectValue;
					break;
				case ItemData.EffectType.SkillUnlock:
					skillValue = (int)itemData.effectValue;
					if( skillValue > -1 )
					{
						if( skillValue == 0 )
						{
							SetCurrentSkills( true, skillBreak );
						}
						else
						{
							skillValue--;
							SetCurrentSkills( skillShoot, skillValue );
						}
					}
					break;
			}
		}
	}

	private void AddItemToInventory( int itemID )
	{
		if( itemID != ItemData.INVALID_ITEM_ID )
		{
			if( ownedItemIDs != null )
			{
				for( int i=0; i<ownedItemIDs.Length; i++ )
				{
					if( ownedItemIDs[i] == itemID )
					{
						ownedItemUnits[i]++;
						ownedItemTotal++;
						return;
					}
				}
			}
			/*halmeida - gonna create a new entry in the inventory for the item.*/
			UsefulFunctions.IncreaseArray<int>( ref ownedItemIDs, itemID );
			UsefulFunctions.IncreaseArray<int>( ref ownedItemUnits, 1 );
			ownedItemTypes++;
			ownedItemTotal++;
		}
	}

	public bool EquipItem( int itemID )
	{
		ItemData itemData = null;

		/*halmeida - if the item is already equipped, change nothing and return false.*/
		if( equippedItemIDs != null )
		{
			for( int i=0; i<equippedTotal; i++ )
			{
				if( equippedItemIDs[i] == itemID )
				{
					return false;
				}
			}
		}
		if( ownedItemIDs != null )
		{
			for( int i=0; i<ownedItemIDs.Length; i++ )
			{
				if( ownedItemIDs[i] == itemID )
				{
					if( itemDatabase != null )
					{
						itemData = itemDatabase.GetItemData( itemID );
						return EquipItem( itemData );
					}
				}
			}
		}
		return false;
	}

	public bool EquipItem( ItemData itemData )
	{
		bool equippable = false;
		float toEquipAccelFactor = 0f;
		float toEquipMaxSpeedFactor = 0f;
		float toEquipMaxHPFactor = 0f;
		float toEquipMaxMPFactor = 0f;
		float toEquipAttackFactor = 0f;
		float toEquipMaxFrequencyFactor = 0f;
		EquipBodyPart toEquipPartCode = EquipBodyPart.Head;
		int equipmentListIndex = -1;

		ExtractEquipmentFactors( itemData, ref equippable, ref toEquipAccelFactor, ref toEquipMaxSpeedFactor, ref toEquipMaxHPFactor,
			ref toEquipMaxMPFactor, ref toEquipAttackFactor, ref toEquipMaxFrequencyFactor, ref toEquipPartCode );
		if( equippable )
		{
			/*halmeida - gotta check if there is already an item equipped at the body part that will
			receive this equipment. If there is, we remove that equipment before equipping the new one.*/
			if( equippedBodyParts != null )
			{
				for( int i=0; i<equippedBodyParts.Length; i++ )
				{
					if( equippedBodyParts[i] == toEquipPartCode )
					{
						UnequipItem( ItemData.INVALID_ITEM_ID, i );
						/*halmeida - since each body part only receives one equipment, once I found the one I can stop looking.*/
						break;
					}
				}
			}
			if( equipmentItemIDs != null )
			{
				for( int i=0; i<equipmentItemIDs.Length; i++ )
				{
					if( equipmentItemIDs[i] == itemData.itemID )
					{
						equipmentListIndex = i;
						break;
					}
				}
			}
			equippedTotal++;
			UsefulFunctions.IncreaseArray<int>( ref equippedItemIDs, itemData.itemID );
			UsefulFunctions.IncreaseArray<EquipBodyPart>( ref equippedBodyParts, toEquipPartCode );
			UsefulFunctions.IncreaseArray<int>( ref equippedListIndexes, equipmentListIndex );
			UsefulFunctions.IncreaseArray<GameObject>( ref equippedObjects, null );
			UsefulFunctions.IncreaseArray<KamiAnimator>( ref equippedAnimators, null );
			UsefulFunctions.IncreaseArray<int>( ref equippedDisplayPriorities, -1 );
			UsefulFunctions.IncreaseArray<float>( ref equippedAccelFactors, toEquipAccelFactor );
			UsefulFunctions.IncreaseArray<float>( ref equippedMaxSpeedFactors, toEquipMaxSpeedFactor );
			UsefulFunctions.IncreaseArray<float>( ref equippedMaxHPFactors, toEquipMaxHPFactor );
			UsefulFunctions.IncreaseArray<float>( ref equippedMaxMPFactors, toEquipMaxMPFactor );
			UsefulFunctions.IncreaseArray<float>( ref equippedAttackFactors, toEquipAttackFactor );
			UsefulFunctions.IncreaseArray<float>( ref equippedMaxFrequencyFactors, toEquipMaxFrequencyFactor );
			UpdateEquipmentFactors();
			UpdateFinalStats();
			/*halmeida - we will create a visual representation of the last equipped item, making all
			the array order changes that are necessary to respect display priorities.*/
			EquipVisualRepresentation();
			return true;
		}
		return false;
	}

	private void EquipVisualRepresentation()
	{
		int equipmentListIndex = -1;
		int toEquipDisplayPriority = 0;
		GameObject toEquipModel = null;
		GameObject toEquipObject = null;
		KamiAnimator toEquipAnimator = null;
		GameObject parentObject = null;
		GameObject childObject = null;
		KamiAnimator parentAnimator = null;
		int newPriorityIndex = 0;
		int itemID = ItemData.INVALID_ITEM_ID;
		EquipBodyPart partCode = EquipBodyPart.Head;
		float accelFactor = 0f;
		float maxSpeedFactor = 0f;
		float maxHPFactor = 0f;
		float maxMPFactor = 0f;
		float attackFactor = 0f;
		float maxFrequencyFactor = 0f;

		if( (equippedTotal > 0) && (equipmentsParent != null) )
		{
			/*halmeida - only proceed if there is no object already associated with the last equipped item.*/
			/*halmeida - all equipped data arrays should no longer be null and should have length equippedTotal.*/
			if( equippedObjects[equippedTotal-1] == null )
			{
				equipmentListIndex = equippedListIndexes[equippedTotal-1];
			}
		}
		if( (equipmentListIndex > -1) && (equipmentModels != null) && (equipmentDisplayPriorities != null) && (kamiAnim != null) )
		{
			if( (equipmentModels.Length > equipmentListIndex) && (equipmentDisplayPriorities.Length > equipmentListIndex) )
			{
				toEquipDisplayPriority = equipmentDisplayPriorities[equipmentListIndex];
				toEquipModel = equipmentModels[equipmentListIndex];
				if( toEquipModel != null )
				{
					toEquipObject = Instantiate( toEquipModel, Vector3.zero, Quaternion.identity ) as GameObject;
					toEquipAnimator = toEquipObject.GetComponent<KamiAnimator>();
					if( toEquipAnimator == null )
					{
						Destroy( toEquipObject );
						toEquipObject = null;
					}
				}
			}
		}
		if( toEquipObject != null )
		{
			/*halmeida - now that I have the object and its display priority, I gotta add it as a
			child of the player object, but respecting the priorities of the children already there.*/
			parentObject = equipmentsParent;
			parentAnimator = kamiAnim;
			childObject = null;
			newPriorityIndex = -1;
			/*halmeida - the priorities are ordered from smallest, at the smallest index, to biggest, at the largest index.*/
			/*halmeida - we don't check the last element because it is the last equipped item that we are trying to figure
			where to put.*/ 
			for( int i=0; i<(equippedTotal-1); i++ )
			{
				if( equippedDisplayPriorities[i] > toEquipDisplayPriority )
				{
					if( i > 0 )
					{
						parentObject = equippedObjects[i-1];
						parentAnimator = equippedAnimators[i-1];
					}
					childObject = equippedObjects[i];
					newPriorityIndex = i;
					break;
				}
			}
			if( newPriorityIndex == -1 )
			{
				if( equippedTotal > 1 )
				{
					parentObject = equippedObjects[equippedTotal-2];
					parentAnimator = equippedAnimators[equippedTotal-2];
				}
				newPriorityIndex = equippedTotal-1;
			}
			if( childObject != null )
			{
				childObject.transform.SetParent( toEquipObject.transform );
				childObject.transform.localPosition = Vector3.zero;
				toEquipAnimator.SetChildAnimator( childObject, false );
			}
			toEquipObject.transform.SetParent( parentObject.transform );
			toEquipObject.transform.localPosition = Vector3.zero;
			parentAnimator.SetChildAnimator( toEquipObject, false );

			/*halmeida - though the arrays, at this point, are complete, their elements aren't ordered.
			They have to be ordered at the same order that the children are added to the main object (this.gameObject).*/
			itemID = equippedItemIDs[equippedTotal-1];
			partCode = equippedBodyParts[equippedTotal-1];
			equipmentListIndex = equippedListIndexes[equippedTotal-1];
			accelFactor = equippedAccelFactors[equippedTotal-1];
			maxSpeedFactor = equippedMaxSpeedFactors[equippedTotal-1];
			maxHPFactor = equippedMaxHPFactors[equippedTotal-1];
			maxMPFactor = equippedMaxMPFactors[equippedTotal-1];
			attackFactor = equippedAttackFactors[equippedTotal-1];
			maxFrequencyFactor = equippedMaxFrequencyFactors[equippedTotal-1];
			for( int i=(equippedTotal-1); i>newPriorityIndex; i-- )
			{
				equippedItemIDs[i] = equippedItemIDs[i-1];
				equippedBodyParts[i] = equippedBodyParts[i-1];
				equippedListIndexes[i] = equippedListIndexes[i-1];
				equippedObjects[i] = equippedObjects[i-1];
				equippedAnimators[i] = equippedAnimators[i-1];
				equippedDisplayPriorities[i] = equippedDisplayPriorities[i-1];
				equippedAccelFactors[i] = equippedAccelFactors[i-1];
				equippedMaxSpeedFactors[i] = equippedMaxSpeedFactors[i-1];
				equippedMaxHPFactors[i] = equippedMaxHPFactors[i-1];
				equippedMaxMPFactors[i] = equippedMaxMPFactors[i-1];
				equippedAttackFactors[i] = equippedAttackFactors[i-1];
				equippedMaxFrequencyFactors[i] = equippedMaxFrequencyFactors[i-1];
			}
			equippedItemIDs[newPriorityIndex] = itemID;
			equippedBodyParts[newPriorityIndex] = partCode;
			equippedListIndexes[newPriorityIndex] = equipmentListIndex;
			equippedObjects[newPriorityIndex] = toEquipObject;
			equippedAnimators[newPriorityIndex] = toEquipAnimator;
			equippedDisplayPriorities[newPriorityIndex] = toEquipDisplayPriority;
			equippedAccelFactors[newPriorityIndex] = accelFactor;
			equippedMaxSpeedFactors[newPriorityIndex] = maxSpeedFactor;
			equippedMaxHPFactors[newPriorityIndex] = maxHPFactor;
			equippedMaxMPFactors[newPriorityIndex] = maxMPFactor;
			equippedAttackFactors[newPriorityIndex] = attackFactor;
			equippedMaxFrequencyFactors[newPriorityIndex] = maxFrequencyFactor;
			/*halmeida - now the chain of parenting and order of the elements in the arrays is correct.
			To actually make the display order of the equipments correct, I gotta give them propper depths.
			This is done through a method of the CharacterAnimator, but it changes the depth of this object
			to put it behind all the equipment, so we need to use the saved unequipped depth as a sort of
			anchor point.*/
			CorrectDepthChain( false );
			/*halmeida - with the display order correct, we synchronize the player animation and the
			animations of his equipments.*/
			kamiAnim.SynchChildAnimator();
		}
	}

	public bool UnequipItem( int itemID, int equippedIndex = -1 )
	{
		/*halmeida - if the index wasn't given, we find it through the item ID.*/
		if( equippedIndex < 0 )
		{
			if( equippedItemIDs != null )
			{
				for( int i=0; i<equippedTotal; i++ )
				{
					if( equippedItemIDs[i] == itemID )
					{
						equippedIndex = i;
						break;
					}
				}
			}
		}
		/*halmeida - if the index was given, it still needs to be valid for us to use.*/
		if( (equippedIndex > -1) && (equippedTotal > equippedIndex) )
		{
			UnequipVisualRepresentation( equippedIndex );
			UsefulFunctions.DecreaseArray<int>( ref equippedItemIDs, equippedIndex );
			UsefulFunctions.DecreaseArray<EquipBodyPart>( ref equippedBodyParts, equippedIndex );
			UsefulFunctions.DecreaseArray<int>( ref equippedListIndexes, equippedIndex );
			UsefulFunctions.DecreaseArray<GameObject>( ref equippedObjects, equippedIndex );
			UsefulFunctions.DecreaseArray<KamiAnimator>( ref equippedAnimators, equippedIndex );
			UsefulFunctions.DecreaseArray<int>( ref equippedDisplayPriorities, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedAccelFactors, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedMaxSpeedFactors, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedMaxHPFactors, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedMaxMPFactors, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedAttackFactors, equippedIndex );
			UsefulFunctions.DecreaseArray<float>( ref equippedMaxFrequencyFactors, equippedIndex );
			equippedTotal--;
			CorrectDepthChain( false );
			UpdateEquipmentFactors();
			UpdateFinalStats();
			return true;
		}
		return false;
	}

	private void UnequipVisualRepresentation( int equippedIndex )
	{
		GameObject parentObject = null;
		GameObject childObject = null;
		KamiAnimator parentAnimator = null;
		GameObject toRemoveObject = null;
		KamiAnimator toRemoveAnimator = null;

		if( (equippedObjects != null) && (equippedAnimators != null) )
		{
			toRemoveObject = equippedObjects[equippedIndex];
			toRemoveAnimator = equippedAnimators[equippedIndex];
			if( toRemoveAnimator != null )
			{
				toRemoveAnimator.SetChildAnimator( null, false );
				toRemoveAnimator.Clear();
				toRemoveAnimator = null;
				equippedAnimators[equippedIndex] = null;
			}
			if( equippedIndex > 0 )
			{
				parentObject = equippedObjects[equippedIndex-1];
				parentAnimator = equippedAnimators[equippedIndex-1];
			}
			else
			{
				parentObject = this.gameObject;
				parentAnimator = kamiAnim;
			}
			if( equippedTotal > (equippedIndex+1) )
			{
				childObject = equippedObjects[equippedIndex+1];
			}
			if( (parentObject != null) && (childObject != null) )
			{
				childObject.transform.SetParent( parentObject.transform );
			}
			if( parentAnimator != null )
			{
				parentAnimator.SetChildAnimator( childObject, false );
			}
			if( toRemoveObject != null )
			{
				Destroy( toRemoveObject );
				toRemoveObject = null;
				equippedObjects[equippedIndex] = null;
			}
		}
	}

	private void ExtractEquipmentFactors( ItemData itemData, ref bool equippable, ref float accelFactor, ref float maxSpeedFactor,
		ref float maxHPFactor, ref float maxMPFactor, ref float attackFactor, ref float maxFrequencyFactor, ref EquipBodyPart bodyPart )
	{
		EquipmentData equipment = null;
		EquipAttribute[] effectCodes = null;
		int[] effectPercentages = null;

		if( itemData != null )
		{
			equipment = itemData.equipment;
			equippable = (equipment != null);
			if( equippable )
			{
				effectCodes = equipment.attributes;
				if( effectCodes != null )
				{
					effectPercentages = equipment.attributePercentages;
					if( effectPercentages != null )
					{
						for( int i=0; i<effectCodes.Length; i++ )
						{
							switch( effectCodes[i] )
							{
								case EquipAttribute.Accel:
									if( effectPercentages.Length > i )
									{
										accelFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								case EquipAttribute.MaxSpeed:
									if( effectPercentages.Length > i )
									{
										maxSpeedFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								case EquipAttribute.MaxHP:
									if( effectPercentages.Length > i )
									{
										maxHPFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								case EquipAttribute.MaxMP:
									if( effectPercentages.Length > i )
									{
										maxMPFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								case EquipAttribute.Attack:
									if( effectPercentages.Length > i )
									{
										attackFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								case EquipAttribute.MaxFrequency:
									if( effectPercentages.Length > i )
									{
										maxFrequencyFactor = (float)effectPercentages[i] / 100f;
									}
									break;
								default:
									break;
							}
						}
					}
				}
				bodyPart = equipment.bodyPart;
			}
		}
	}

	private void UpdateEquipmentFactors()
	{
		equippedAccelTotal = 0f;
		if( equippedAccelFactors != null )
		{
			for( int i=0; i<equippedAccelFactors.Length; i++ )
			{
				equippedAccelTotal += equippedAccelFactors[i];
			}
		}
		equippedMaxSpeedTotal = 0f;
		if( equippedMaxSpeedFactors != null )
		{
			for( int i=0; i<equippedMaxSpeedFactors.Length; i++ )
			{
				equippedMaxSpeedTotal += equippedMaxSpeedFactors[i];
			}
		}
		equippedMaxHPTotal = 0f;
		if( equippedMaxHPFactors != null )
		{
			for( int i=0; i<equippedMaxHPFactors.Length; i++ )
			{
				equippedMaxHPTotal += equippedMaxHPFactors[i];
			}
		}
		equippedMaxMPTotal = 0f;
		if( equippedMaxMPFactors != null )
		{
			for( int i=0; i<equippedMaxMPFactors.Length; i++ )
			{
				equippedMaxMPTotal += equippedMaxMPFactors[i];
			}
		}
		equippedAttackTotal = 0f;
		if( equippedAttackFactors != null )
		{
			for( int i=0; i<equippedAttackFactors.Length; i++ )
			{
				equippedAttackTotal += equippedAttackFactors[i];
			}
		}
		equippedMaxFrequencyTotal = 0f;
		if( equippedMaxFrequencyFactors != null )
		{
			for( int i=0; i<equippedMaxFrequencyFactors.Length; i++ )
			{
				equippedMaxFrequencyTotal += equippedMaxFrequencyFactors[i];
			}
		}
	}

	public void ClearInventory()
	{
		ClearEquippedItems();
		ownedItemIDs = null;
		ownedItemUnits = null;
		ownedItemTypes = 0;
		ownedItemTotal = 0;
	}

	public void ClearEquippedItems()
	{
		KamiAnimator equippedAnimator = null;
		GameObject equippedObject = null;

		if( equippedTotal > 0 )
		{
			/*halmeida - the first equipment animator is supposed to already clear all others.*/
			equippedAnimator = equippedAnimators[0];
			if( equippedAnimator != null )
			{
				equippedAnimator.Clear();
			}
			if( kamiAnim != null )
			{
				kamiAnim.SetChildAnimator( null, false );
			}
			for( int i=(equippedTotal-1); i>-1; i-- )
			{
				equippedAnimators[i] = null;
				equippedObject = equippedObjects[i];
				if( equippedObject != null )
				{
					Destroy( equippedObject );
					equippedObject = null;
					equippedObjects[i] = null;
				}
			}
			equippedAnimators = null;
			equippedObjects = null;
			equippedDisplayPriorities = null;
			equippedListIndexes = null;
			equippedItemIDs = null;
			equippedBodyParts = null;
			equippedAccelFactors = null;
			equippedMaxSpeedFactors = null;
			equippedMaxHPFactors = null;
			equippedMaxMPFactors = null;
			equippedAttackFactors = null;
			equippedMaxFrequencyFactors = null;
			UpdateEquipmentFactors();
			UpdateFinalStats();
			equippedTotal = 0;
		}
	}

	public bool HasEquipment( int itemID )
	{
		if( ownedItemIDs != null )
		{
			for( int i=0; i<ownedItemTotal; i++ )
			{
				if( ownedItemIDs[i] == itemID )
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsEquippedWith( int itemID )
	{
		if( equippedItemIDs != null )
		{
			for( int i=0; i<equippedTotal; i++ )
			{
				if( equippedItemIDs[i] == itemID )
				{
					return true;
				}
			}
		}
		return false;
	}

	public int[] GetInventoryItemIDsCopy()
	{
		int[] ownedCopy = null;

		if( ownedItemIDs != null )
		{
			ownedCopy = new int[ownedItemIDs.Length];
			for( int i=0; i<ownedItemIDs.Length; i++ )
			{
				ownedCopy[i] = ownedItemIDs[i];
			}
		}
		return ownedCopy;
	}

	public int[] GetInventoryItemUnitsCopy()
	{
		int[] ownedCopy = null;

		if( ownedItemUnits != null )
		{
			ownedCopy = new int[ownedItemUnits.Length];
			for( int i=0; i<ownedItemUnits.Length; i++ )
			{
				ownedCopy[i] = ownedItemUnits[i];
			}
		}
		return ownedCopy;
	}

	public int[] GetEquippedItemIDsCopy()
	{
		int[] equippedCopy = null;

		if( equippedItemIDs != null )
		{
			equippedCopy = new int[equippedItemIDs.Length];
			for( int i=0; i<equippedItemIDs.Length; i++ )
			{
				equippedCopy[i] = equippedItemIDs[i];
			}
		}
		return equippedCopy;
	}

	public bool GetHypotheticalEquipResults( ItemData hypotheticalEquipment, EquipAttribute[] attributes, ref float[] results )
	{
		if( (hypotheticalEquipment == null) || (attributes == null) || (results == null) )
		{
			return false;
		}
		if( attributes.Length != results.Length )
		{
			return false;
		}
		bool newEquippable = false;
		float newAccel = 0f;
		float newMaxSpeed = 0f;
		float newMaxHP = 0f;
		float newMaxMP = 0f;
		float newAttack = 0f;
		float newMaxFrequency = 0f;
		EquipBodyPart newBodyPart = EquipBodyPart.Head;
		ExtractEquipmentFactors( hypotheticalEquipment, ref newEquippable, ref newAccel, ref newMaxSpeed, ref newMaxHP, ref newMaxMP,
			ref newAttack, ref newMaxFrequency, ref newBodyPart );
		if( newEquippable )
		{
			float newAccelTotal = equippedAccelTotal;
			float newMaxSpeedTotal = equippedMaxSpeedTotal;
			float newMaxHPTotal = equippedMaxHPTotal;
			float newMaxMPTotal = equippedMaxMPTotal;
			float newAttackTotal = equippedAttackTotal;
			float newMaxFrequencyTotal = equippedMaxFrequencyTotal;
			bool currentlyEquipped = false;
			if( equippedBodyParts != null )
			{
				for( int i=0; i<equippedBodyParts.Length; i++ )
				{
					if( equippedBodyParts[i] == newBodyPart )
					{
						newAccelTotal -= equippedAccelFactors[i];
						newMaxSpeedTotal -= equippedMaxSpeedFactors[i];
						newMaxHPTotal -= equippedMaxHPFactors[i];
						newMaxMPTotal -= equippedMaxMPFactors[i];
						newAttackTotal -= equippedAttackFactors[i];
						newMaxFrequencyTotal -= equippedMaxFrequencyFactors[i];
						if( equippedItemIDs[i] == hypotheticalEquipment.itemID )
						{
							currentlyEquipped = true;
						}
					}
				}
			}
			if( !currentlyEquipped )
			{
				newAccelTotal += newAccel;
				newMaxSpeedTotal += newMaxSpeed;
				newMaxHPTotal += newMaxHP;
				newMaxMPTotal += newMaxMP;
				newAttackTotal += newAttack;
				newMaxFrequencyTotal += newMaxFrequency;
			}
			for( int i=0; i<attributes.Length; i++ )
			{
				switch( attributes[i] )
				{
					case EquipAttribute.Accel:
						results[i] = newAccelTotal;
						break;
					case EquipAttribute.MaxSpeed:
						results[i] = newMaxSpeedTotal;
						break;
					case EquipAttribute.MaxHP:
						results[i] = newMaxHPTotal;
						break;
					case EquipAttribute.MaxMP:
						results[i] = newMaxMPTotal;
						break;
					case EquipAttribute.Attack:
						results[i] = newAttackTotal;
						break;
					case EquipAttribute.MaxFrequency:
						results[i] = newMaxFrequencyTotal;
						break;
				}
			}
			return true;
		}
		return false;
	} 

	public int GetCurrentCoins()
	{
		return coins;
	}

	public void GetCurrentSkills( ref bool shootingSkill, ref int breakingSkill )
	{
		shootingSkill = skillShoot;
		breakingSkill = skillBreak;
	}

	public int GetCurrentLevel()
	{
		return currentLevel;
	}

	public void SetCurrentCoins( int newCoins )
	{
		coins = newCoins;
	}

	public void SetCurrentSkills( bool shootingSkill, int breakingSkill )
	{
		int totalShadowConfigs = 0;
		GameObject shadowConfigObject = null;
		ShadowConfiguration shadowConfig = null;

		skillShoot = shootingSkill;
		skillBreak = breakingSkill;
		if( skillShoot )
		{
			if( gaugeMP == null )
			{
				CreateMPGauge();
			}
		}
		else
		{
			ClearMPGauge();
		}
		destructionLevel = skillBreak;
		if( (kamiAnim != null) && (shadowConfigPerSkill != null) )
		{
			/*halmeida - from the most advanced skill to the most simple one, we check for
			one that is active and apply only its shadow reconfiguration.*/
			totalShadowConfigs = shadowConfigPerSkill.Length;
			if( (skillBreak >= ObstacleController.CONSTRUCTION_LEVEL_STURDY) && (totalShadowConfigs > 4) )
			{
				shadowConfigObject = shadowConfigPerSkill[4];
			}
			if( shadowConfigObject == null )
			{
				if( (skillBreak >= ObstacleController.CONSTRUCTION_LEVEL_MEDIUM) && (totalShadowConfigs > 3) )
				{
					shadowConfigObject = shadowConfigPerSkill[3];
				}
			}
			if( shadowConfigObject == null )
			{
				if( (skillBreak >= ObstacleController.CONSTRUCTION_LEVEL_FRAIL) && (totalShadowConfigs > 2) )
				{
					shadowConfigObject = shadowConfigPerSkill[2];
				}
			}
			if( shadowConfigObject == null )
			{
				if( skillShoot && (totalShadowConfigs > 1) )
				{
					shadowConfigObject = shadowConfigPerSkill[1];
				}
			}
			if( shadowConfigObject == null )
			{
				if( totalShadowConfigs > 0 )
				{
					shadowConfigObject = shadowConfigPerSkill[0];
				}
			}
			if( shadowConfigObject != null )
			{
				shadowConfig = shadowConfigObject.GetComponent<ShadowConfiguration>();
				kamiAnim.ReconfigurePeriodicShadow( shadowConfig );
			}
		}
	}

	public void CorrectDepthChain( bool atBaseDepth )
	{
		if( atBaseDepth )
		{
			unequippedDepth = gameObject.transform.position.z;
		}
		if( kamiAnim != null )
		{
			kamiAnim.StartDepthChain( unequippedDepth, true );
			kamiAnim.StartDepthChain( unequippedDepth, false );
		}
	}

	public override void Clear()
	{
		ClearGauges();
		ClearInventory();
		base.Clear();
	}

	protected override void LoadAllSoundsFromResources()
	{
		base.LoadAllSoundsFromResources();
		UsefulFunctions.LoadSoundsFromResources( coinSoundPaths, ref coinSounds );
		UsefulFunctions.LoadSoundsFromResources( breakSoundPaths, ref breakSounds );
		UsefulFunctions.LoadSoundsFromResources( positiveSoundPaths, ref positiveSounds );
		UsefulFunctions.LoadSoundsFromResources( negativeSoundPaths, ref negativeSounds );
		UsefulFunctions.LoadSoundsFromResources( itemSoundPaths, ref itemSounds );
		UsefulFunctions.LoadSoundsFromResources( fadeSoundPaths, ref fadeSounds );
	}

	protected override void ClearAllSoundsFromResources()
	{
		base.ClearAllSoundsFromResources();
		UsefulFunctions.ClearSoundsFromResources( ref coinSounds );
		UsefulFunctions.ClearSoundsFromResources( ref breakSounds );
		UsefulFunctions.ClearSoundsFromResources( ref positiveSounds );
		UsefulFunctions.ClearSoundsFromResources( ref negativeSounds );
		UsefulFunctions.ClearSoundsFromResources( ref itemSounds );
		UsefulFunctions.ClearSoundsFromResources( ref fadeSounds );
	}

	protected override void LoadAllSoundsIntoAudioCenter()
	{
		base.LoadAllSoundsIntoAudioCenter();
		UsefulFunctions.LoadSoundsIntoAudioCenter( coinSoundPaths, ref coinSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( breakSoundPaths, ref breakSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( positiveSoundPaths, ref positiveSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( negativeSoundPaths, ref negativeSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( itemSoundPaths, ref itemSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( fadeSoundPaths, ref fadeSoundIDs );
	}

	protected override void ClearAllSoundsFromAudioCenter()
	{
		base.ClearAllSoundsFromAudioCenter();
		UsefulFunctions.ClearSoundsFromAudioCenter( ref coinSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref breakSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref positiveSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref negativeSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref itemSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref fadeSoundIDs );
	}

	protected override void AddAudioSources()
	{
		base.AddAudioSources();
		audioSourceItem = gameObject.AddComponent<AudioSource>();
	}

	protected override void PlaySoundEffect( int soundFXCategory )
	{
		int soundIndex = -1;

		#if !UNITY_ANDROID || UNITY_EDITOR
			AudioClip[] availableSounds = null;
			AudioClip chosenSound = null;
			AudioSource propperSource = null;

			switch( soundFXCategory )
			{
				case SOUND_FX_CATEGORY_DAMAGE_IN:
					propperSource = audioSourceDamageIn;
					availableSounds = damageInSounds;
					break;
				case SOUND_FX_CATEGORY_DEATH:
					propperSource = audioSourceVoice;
					availableSounds = deathSounds;
					break;
				case SOUND_FX_CATEGORY_COIN:
					propperSource = audioSourceItem;
					availableSounds = coinSounds;
					break;
				case SOUND_FX_CATEGORY_BREAK:
					propperSource = audioSourceVoice;
					availableSounds = breakSounds;
					break;
				case SOUND_FX_CATEGORY_POSITIVE:
					propperSource = audioSourceVoice;
					availableSounds = positiveSounds;
					break;
				case SOUND_FX_CATEGORY_NEGATIVE:
					propperSource = audioSourceVoice;
					availableSounds = negativeSounds;
					break;
				case SOUND_FX_CATEGORY_ITEM:
					propperSource = audioSourceItem;
					availableSounds = itemSounds;
					break;
				case SOUND_FX_CATEGORY_FADE:
					propperSource = audioSourceItem;
					availableSounds = fadeSounds;
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
				case SOUND_FX_CATEGORY_DEATH:
					availableSoundIDs = deathSoundIDs;
					break;
				case SOUND_FX_CATEGORY_COIN:
					availableSoundIDs = coinSoundIDs;
					break;
				case SOUND_FX_CATEGORY_BREAK:
					availableSoundIDs = breakSoundIDs;
					break;
				case SOUND_FX_CATEGORY_POSITIVE:
					availableSoundIDs = positiveSoundIDs;
					break;
				case SOUND_FX_CATEGORY_NEGATIVE:
					availableSoundIDs = negativeSoundIDs;
					break;
				case SOUND_FX_CATEGORY_ITEM:
					availableSoundIDs = itemSoundIDs;
					break;
				case SOUND_FX_CATEGORY_FADE:
					availableSoundIDs = fadeSoundIDs;
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
