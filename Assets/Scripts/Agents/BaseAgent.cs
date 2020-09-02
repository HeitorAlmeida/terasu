using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAgent : MonoBehaviour
{
	public const int SOUND_FX_CATEGORY_DAMAGE_IN = 0;
	public const int SOUND_FX_CATEGORY_DEATH = 1;
	public const int SOUND_FX_CATEGORY_COIN = 2;
	public const int SOUND_FX_CATEGORY_BREAK = 3;
	public const int SOUND_FX_CATEGORY_POSITIVE = 4;
	public const int SOUND_FX_CATEGORY_NEGATIVE = 5;
	public const int SOUND_FX_CATEGORY_ITEM = 6;
	public const int SOUND_FX_CATEGORY_FADE = 7;

	public const float DEFAULT_PUSHED_INTENSITY = 4f;

	public const int INVALID_AGENT_ID = -1;

	public const bool DIRECTION_PURPOSE_MOVEMENT = true;
	public const bool DIRECTION_PURPOSE_AIM = false;

	public string agentName;
	public int agentID;
	public Sprite agentPortrait;
	public bool preciseMaxSpeed;
	public float originalAccel;
	public float originalMaxSpeed;
	public int originalMaxHP;
	public float originalAttack;
	public float originalMaxFrequency;
	public float originalProjectileSpeed;
	public float originalProjectileAccel;
	public float originalProjectileMaxSpeed;
	public float originalProjectileDuration;
	public GameObject projectileAttackModel;
	public GameObject[] projectileLaunchEffectsFront;
	public GameObject[] projectileLaunchEffectsBack;
	public Collider2D mainCollider2D;
	public string[] damageInSoundPaths;
	public string[] deathSoundPaths;

	protected Rigidbody2D rbody;
	protected bool pressingUpMove;
	protected bool pressingUpAim;
	protected bool pressingLeftMove;
	protected bool pressingLeftAim;
	protected bool pressingDownMove;
	protected bool pressingDownAim;
	protected bool pressingRightMove;
	protected bool pressingRightAim;
	protected Vector2[] standardDirections;
	protected Vector2 directionMove;
	protected Vector2 directionAim;
	protected Vector2 directionPushed;
	protected float baseAccel;
	protected float baseMaxSpeed;
	protected float baseMaxHP;
	protected float baseAttack;
	protected float baseMaxFrequency;
	protected float finalAccel;
	protected float finalMaxSpeed;
	protected float finalMaxSpeedTemp;
	protected float finalMaxSpeedForced;
	protected float finalMaxSqrSpeed;
	protected float finalMaxSqrSpeedTemp;
	protected float finalMaxSqrSpeedForced;
	protected float finalMaxSpeedLerpSpeed;
	protected float finalMaxSpeedLerpTraveled;
	protected int finalMaxHP;
	protected float finalAttack;
	protected float finalMaxFrequency;
	protected int currentHP;
	protected float projectileInterval;
	protected float projectileElapsed;
	protected float projectileSpeed;
	protected float projectileAccel;
	protected float projectileMaxSpeed;
	protected float projectileDuration;
	protected int destructionLevel;
	protected bool dead;
	protected bool over;
	protected bool stopRequested;
	protected float pushedIntensity;
	protected GameObject[] adversaryObjects;
	protected BaseAgent[] adversaryComponents;
	protected GameObject[] projectileObjects;
	protected ProjectileController[] projectileComponents;
	protected GameObject[] projectileLaunchFront;
	protected GameObject[] projectileLaunchBack;
	protected GameObject[] visualEffectObjects;
	protected SimpleAnimator[] visualEffectAnimators;
	protected Collider2D mainCollider;
	protected float radius;
	protected float width;
	protected float height;
	protected bool paused;
	protected Vector2 pausedVelocity;
	protected bool shouldUnpause;
	protected string currentName;
	protected bool acceptsDamage;
	protected bool triggersTransactions;
	protected bool forcingFinalMaxSpeed;
	protected ChallengeStage challengeStage;
	protected CameraController cameraController;
	protected SymbolDatabase fontDamage;
	protected Color fontColorDamage;
	protected Color fontColorHeal;
	protected Vector4 fontGradientDamage;

	protected AudioSource audioSourceDamageIn;
	protected AudioSource audioSourceVoice;
	protected AudioClip[] damageInSounds;
	protected AudioClip[] deathSounds;
	protected int[] damageInSoundIDs;
	protected int[] deathSoundIDs;

	protected virtual void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		pressingUpMove = false;
		pressingUpAim = false;
		pressingLeftMove = false;
		pressingLeftAim = false;
		pressingDownMove = false;
		pressingDownAim = false;
		pressingRightMove = false;
		pressingRightAim = false;
		standardDirections = null;
		CreateStandardDirections();
		directionMove = Vector2.zero;
		directionAim = Vector2.zero;
		directionPushed = Vector2.zero;
		baseAccel = 0f;
		baseMaxSpeed = 0f;
		baseMaxHP = 0f;
		baseAttack = 0f;
		baseMaxFrequency = 0f;
		finalAccel = 0f;
		finalMaxSpeed = 0f;
		finalMaxSpeedTemp = 0f;
		finalMaxSpeedForced = 0f;
		finalMaxSqrSpeed = 0f;
		finalMaxSqrSpeedTemp = 0f;
		finalMaxSqrSpeedForced = 0f;
		finalMaxSpeedLerpSpeed = 0f;
		finalMaxSpeedLerpTraveled = 0f;
		finalMaxHP = 0;
		finalAttack = 0f;
		finalMaxFrequency = 0f;
		currentHP = 0;
		projectileInterval = 0f;
		projectileElapsed = 0f;
		projectileSpeed = originalProjectileSpeed;
		projectileAccel = originalProjectileAccel;
		projectileMaxSpeed = originalProjectileMaxSpeed;
		projectileDuration = originalProjectileDuration;
		destructionLevel = ObstacleController.CONSTRUCTION_LEVEL_INVALID;
		dead = true;
		over = false;
		SetBaseStatsToOriginals( true );

		stopRequested = false;
		pushedIntensity = 0f;
		adversaryObjects = null;
		adversaryComponents = null;
		projectileObjects = null;
		projectileComponents = null;
		projectileLaunchFront = null;
		projectileLaunchBack = null;
		visualEffectObjects = null;
		visualEffectAnimators = null;
		ValidateVisualEffectModels();
		mainCollider = mainCollider2D;
		radius = 0f;
		width = 0f;
		height = 0f;
		if( mainCollider == null )
		{
			mainCollider = GetComponent<Collider2D>();
		}
		if( mainCollider != null )
		{
			width = mainCollider.bounds.size.x;
			height = mainCollider.bounds.size.y;
			radius = (width > height) ? width / 2f : height / 2f;
		}
		paused = false;
		pausedVelocity = Vector2.zero;
		shouldUnpause = false;
		currentName = null;
		SetCurrentName( agentName );
		acceptsDamage = true;
		triggersTransactions = true;
		forcingFinalMaxSpeed = false;
		challengeStage = null;
		cameraController = null;
		fontDamage = null;
		fontColorDamage = new Color( 1f, 0.8f, 0.2f, 1f );
		fontColorHeal = new Color( 0.7f, 1f, 0.2f, 1f );
		fontGradientDamage = new Vector4( 0f, 5f, 0f, 0f );

		audioSourceDamageIn = null;
		audioSourceVoice = null;
		damageInSounds = null;
		damageInSoundIDs = null;
		deathSounds = null;
		deathSoundIDs = null;
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

	private void CreateStandardDirections()
	{
		if( standardDirections == null )
		{
			standardDirections = new Vector2[8];
			standardDirections[0] = new Vector2( 0f, 1f );
			standardDirections[1] = new Vector2( -1f, 1f ).normalized;
			standardDirections[2] = new Vector2( -1f, 0f );
			standardDirections[3] = new Vector2( -1f, -1f ).normalized;
			standardDirections[4] = new Vector2( 0f, -1f );
			standardDirections[5] = new Vector2( 1f, -1f ).normalized;
			standardDirections[6] = new Vector2( 1f, 0f );
			standardDirections[7] = new Vector2( 1f, 1f ).normalized;  
		}
	}

	protected virtual void SetBaseStatsToOriginals( bool updateFinal )
	{
		SetBaseAccel( originalAccel, false );
		SetBaseMaxSpeed( originalMaxSpeed, false );
		SetBaseMaxHP( originalMaxHP, false );
		SetBaseAttack( originalAttack, false );
		SetBaseMaxFrequency( originalMaxFrequency, false );
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected void SetBaseMaxSpeed( float newMaxSpeed, bool updateFinal )
	{
		baseMaxSpeed = (newMaxSpeed < 0f) ? -newMaxSpeed : newMaxSpeed;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected void SetBaseAccel( float newAccel, bool updateFinal )
	{
		baseAccel = (newAccel < 0f) ? -newAccel : newAccel;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected void SetBaseMaxHP( float newMaxHP, bool updateFinal )
	{
		baseMaxHP = (newMaxHP < 0f) ? -newMaxHP : newMaxHP;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected void SetBaseAttack( float newAttack, bool updateFinal )
	{
		baseAttack = (newAttack < 0f) ? -newAttack : newAttack;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected void SetBaseMaxFrequency( float newMaxFrequency, bool updateFinal )
	{
		baseMaxFrequency = (newMaxFrequency < 0f) ? -newMaxFrequency : newMaxFrequency;
		if( updateFinal )
		{
			UpdateFinalStats();
		}
	}

	protected virtual void UpdateFinalStats()
	{
		float currentHPRate = 0f;

		finalAccel = baseAccel;
		finalMaxSpeed = baseMaxSpeed;
		finalMaxSqrSpeed = finalMaxSpeed * finalMaxSpeed;
		forcingFinalMaxSpeed = false;
		if( finalMaxHP > 0 )
		{
			currentHPRate = (float)currentHP / (float)finalMaxHP;
		}
		finalMaxHP = (int)baseMaxHP;
		if( currentHPRate > 0f )
		{
			SetCurrentHP( (int)(currentHPRate * finalMaxHP) );
		}
		else
		{
			SetCurrentHP( finalMaxHP );
		}
		finalAttack = baseAttack;
		finalMaxFrequency = baseMaxFrequency;
		if( finalMaxFrequency > 0f )
		{
			projectileInterval = 1f / finalMaxFrequency;
		}
		else
		{
			projectileInterval = 0f;
		}
	}

	protected virtual void SetCurrentHP( int newCurrentHP )
	{
		bool previouslyDead = false;

		currentHP = newCurrentHP;
		if( currentHP < 0 )
		{
			currentHP = 0;
		}
		else if( currentHP > finalMaxHP )
		{
			currentHP = finalMaxHP;
		}
		previouslyDead = dead;
		dead = ( currentHP == 0 );
		if( !dead )
		{
			if( previouslyDead )
			{
				ToggleColliders( true );
			}
			over = false;
		}
		else
		{
			if( !previouslyDead )
			{
				ToggleColliders( false );
			}
		}
	}

	public int GetCurrentHP()
	{
		return currentHP;
	}

	public void AddToCurrentHP( int hpAddition, bool revive )
	{
		if( revive || !dead )
		{
			SetCurrentHP( currentHP + hpAddition );
		}
	}

	public void FillHP( bool revive )
	{
		if( revive || !dead )
		{
			SetCurrentHP( finalMaxHP );
		}
	}

	public void SetStageAndCamera( ChallengeStage newChallengeStage, CameraController newCameraController )
	{
		challengeStage = newChallengeStage;
		cameraController = newCameraController;
	}

	public void SetDamageDisplayFont( SymbolDatabase newFontDamage )
	{
		fontDamage = newFontDamage;
	}

	public virtual bool SufferHPDamage( int damagePower, GameObject damageSourceObject, BaseAgent damageSourceAgent, bool displayDamage = true )
	{
		if( !dead && acceptsDamage )
		{
			SetCurrentHP( currentHP - damagePower );
			if( displayDamage )
			{
				DisplayDamageText( damagePower );
			}
			return true;
		}
		return false;
	}

	protected virtual void DisplayDamageText( int damageValue )
	{
		Vector2 worldPosition = Vector2.zero;

		if( cameraController != null )
		{
			worldPosition.x = transform.localPosition.x;
			worldPosition.y = transform.localPosition.y;
			cameraController.AddInterfaceCanvasText(""+damageValue, fontDamage, fontColorDamage, fontGradientDamage,
				CameraController.TEXT_EVOLUTION_ASCENT, null, worldPosition, false );
		}
	}

	public void ToggleDamageAcceptance( bool enable )
	{
		acceptsDamage = enable;
	}

	public void ToggleTransactionsTrigger( bool enable )
	{
		triggersTransactions = enable;
	}

	public bool GetTransactionsTrigger()
	{
		return triggersTransactions;
	}

	public virtual void ReceiveExperience( int experiencePoints, bool withVFX )
	{
		/*halmeida - reserved for subclasses.*/
	}

	public virtual int GetTotalExperiencePoints()
	{
		return 0;
	}

	public virtual void ResetExperience( bool resetStats )
	{
		/*halmeida - reserved for subclasses.*/
	}

	public void SetAdversaries( BaseAgent[] newAdversaryComponents )
	{
		int length = 0;
		BaseAgent adversaryComponent = null;
		GameObject correspondingObject = null;
		ProjectileController projectileComponent = null;

		adversaryObjects = null;
		adversaryComponents = newAdversaryComponents;
		if( adversaryComponents != null )
		{
			length = adversaryComponents.Length;
			if( length > 0 )
			{
				adversaryObjects = new GameObject[length];
				for( int i=0; i<length; i++ )
				{
					correspondingObject = null;
					adversaryComponent = adversaryComponents[i];
					if( adversaryComponent != null )
					{
						correspondingObject = adversaryComponent.gameObject;
					}
					adversaryObjects[i] = correspondingObject;
				}
			}
			else
			{
				adversaryComponents = null;
			}
		}
		if( projectileComponents != null )
		{
			for( int i=0; i<projectileComponents.Length; i++ )
			{
				projectileComponent = projectileComponents[i];
				projectileComponent.SetTriggerSource( gameObject );
			}
		}
	}

	public GameObject[] GetAdversaryObjects()
	{
		return adversaryObjects;
	}

	public BaseAgent[] GetAdversaryComponents()
	{
		return adversaryComponents;
	}

	protected virtual void ValidateVisualEffectModels()
	{
		GameObject effectModel = null;

		if( projectileLaunchEffectsFront != null )
		{
			for( int i=0; i<projectileLaunchEffectsFront.Length; i++ )
			{
				effectModel = projectileLaunchEffectsFront[i];
				if( effectModel != null )
				{
					if( effectModel.GetComponent<SimpleAnimator>() != null )
					{
						UsefulFunctions.IncreaseArray<GameObject>( ref projectileLaunchFront, effectModel );
					}
				}
			}
		}
		if( projectileLaunchEffectsBack != null )
		{
			for( int i=0; i<projectileLaunchEffectsBack.Length; i++ )
			{
				effectModel = projectileLaunchEffectsBack[i];
				if( effectModel != null )
				{
					if( effectModel.GetComponent<SimpleAnimator>() != null )
					{
						UsefulFunctions.IncreaseArray<GameObject>( ref projectileLaunchBack, effectModel );
					}
				}
			}
		}
	}

	protected virtual ProjectileController LaunchProjectile( GameObject projectileModel, Vector2 normalizedDirection, bool attacking,
		float newProjectileSpeed, float newProjectileAcceleration, float newProjectileMaxSpeed, float newProjectileDuration )
	{
		Vector3 projPosition = Vector3.zero;
		Vector2 projOriginOffset = Vector2.zero;
		GameObject projectileObject = null;
		ProjectileController projectileComponent = null;
		int effectCode = ProjectileController.EFFECT_CODE_INVALID;

		if( (projectileModel != null) && (normalizedDirection != Vector2.zero) )
		{
			projPosition = transform.position;
			projOriginOffset = normalizedDirection * radius;
			projPosition.x += projOriginOffset.x;
			projPosition.y += projOriginOffset.y;
			projectileObject = Instantiate( projectileModel, projPosition, Quaternion.identity ) as GameObject;
			projectileComponent = projectileObject.GetComponent<ProjectileController>();
			if( projectileComponent == null )
			{
				Destroy( projectileObject );
			}
			else
			{
				projectileComponent.SetOrientation( normalizedDirection );
				if( attacking )
				{
					effectCode = ProjectileController.EFFECT_CODE_HP_DAMAGE;
				}
				else
				{
					effectCode = ProjectileController.EFFECT_CODE_HP_RECOVERY;
				}
				projectileComponent.SetEffect( effectCode, finalAttack, destructionLevel );
				projectileComponent.SetTriggerSource( gameObject );
				projectileComponent.SetSpeed( newProjectileSpeed, newProjectileAcceleration, newProjectileMaxSpeed );
				projectileComponent.SetDuration( newProjectileDuration );
				UsefulFunctions.IncreaseArray<GameObject>( ref projectileObjects, projectileObject );
				UsefulFunctions.IncreaseArray<ProjectileController>( ref projectileComponents, projectileComponent );
				CreateProjectileLaunchEffects( projPosition, normalizedDirection );
				return projectileComponent;
			}
		}
		return null;
	}

	protected void RemoveProjectile( int projectileIndex )
	{
		GameObject projectileObject = null;
		ProjectileController projectileComponent = null;

		if( projectileObjects != null )
		{
			if( (projectileIndex > -1) && (projectileIndex < projectileObjects.Length) )
			{
				/*halmeida - relying on the coherence of all the "projectile" arrays.*/
				projectileComponent = projectileComponents[projectileIndex];
				RemoveProjectileInfluence( projectileComponent );
				projectileComponent.Clear();
				projectileObject = projectileObjects[projectileIndex];
				Destroy( projectileObject );
				UsefulFunctions.DecreaseArray<ProjectileController>( ref projectileComponents, projectileIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref projectileObjects, projectileIndex );
			}
		}
	}

	protected virtual void RemoveProjectileInfluence( ProjectileController projectile )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected void ClearProjectiles()
	{
		GameObject projectileObject = null;
		ProjectileController projectileComponent = null;

		if( projectileObjects != null )
		{
			for( int i=0; i<projectileObjects.Length; i++ )
			{
				projectileComponent = projectileComponents[i];
				RemoveProjectileInfluence( projectileComponent );
				projectileComponent.Clear();
				projectileComponents[i] = null;
				projectileObject = projectileObjects[i];
				Destroy( projectileObject );
				projectileObjects[i] = null;
			}
			projectileComponents = null;
			projectileObjects = null;
		}
	}

	protected virtual void CreateProjectileLaunchEffects( Vector3 projectilePosition, Vector2 normalizedDirection )
	{
		float angleWithRight = 0f;
		int totalEffects = 0;
		GameObject effectModel = null;
		GameObject effectObject = null;
		SimpleAnimator effectAnimator = null;
		Vector3 effectPosition = Vector3.zero;

		angleWithRight = UsefulFunctions.GetDirectionAngle( normalizedDirection );
		if( projectileLaunchFront != null )
		{
			totalEffects = projectileLaunchFront.Length;
			for( int i=0; i<totalEffects; i++ )
			{
				effectModel = projectileLaunchFront[i];
				effectObject = Instantiate( effectModel, Vector3.zero, Quaternion.identity ) as GameObject;
				effectObject.transform.localRotation = Quaternion.Euler( 0f, 0f, angleWithRight );
				effectAnimator = effectObject.GetComponent<SimpleAnimator>();
				/*halmeida - since the effects were validated in Awake(), both object and component are necessarily not null.*/
				effectPosition = projectilePosition;
				effectPosition.z -= (totalEffects - i) * effectAnimator.GetChildSeparation();
				effectObject.transform.position = effectPosition;
				UsefulFunctions.IncreaseArray<GameObject>( ref visualEffectObjects, effectObject );
				UsefulFunctions.IncreaseArray<SimpleAnimator>( ref visualEffectAnimators, effectAnimator );
			}
		}
		if( projectileLaunchBack != null )
		{
			totalEffects = projectileLaunchBack.Length;
			for( int i=0; i<totalEffects; i++ )
			{
				effectModel = projectileLaunchBack[i];
				effectObject = Instantiate( effectModel, Vector3.zero, Quaternion.identity ) as GameObject;
				effectObject.transform.localRotation = Quaternion.Euler( 0f, 0f, angleWithRight );
				effectAnimator = effectObject.GetComponent<SimpleAnimator>();
				/*halmeida - since the effects were validated in Awake(), both object and component are necessarily not null.*/
				effectPosition = projectilePosition;
				effectPosition.z += (i + 1) * effectAnimator.GetChildSeparation();
				effectObject.transform.position = effectPosition;
				UsefulFunctions.IncreaseArray<GameObject>( ref visualEffectObjects, effectObject );
				UsefulFunctions.IncreaseArray<SimpleAnimator>( ref visualEffectAnimators, effectAnimator );
			}
		}
	}

	protected void RemoveVisualEffect( int effectIndex )
	{
		GameObject effectObject = null;
		SimpleAnimator effectAnimator = null;

		if( visualEffectObjects != null )
		{
			if( (effectIndex > -1) && (effectIndex < visualEffectObjects.Length) )
			{
				/*halmeida - relying on the coherence of all the "visualEffect" arrays.*/
				effectAnimator = visualEffectAnimators[effectIndex];
				effectAnimator.Clear();
				effectObject = visualEffectObjects[effectIndex];
				Destroy( effectObject );
				UsefulFunctions.DecreaseArray<SimpleAnimator>( ref visualEffectAnimators, effectIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref visualEffectObjects, effectIndex );
			}
		}
	}

	protected void ClearVisualEffects( bool clearModels )
	{
		GameObject effectObject = null;
		SimpleAnimator effectAnimator = null;

		if( visualEffectObjects != null )
		{
			for( int i=0; i<visualEffectObjects.Length; i++ )
			{
				effectAnimator = visualEffectAnimators[i];
				effectAnimator.Clear();
				visualEffectAnimators[i] = null;
				effectObject = visualEffectObjects[i];
				Destroy( effectObject );
				visualEffectObjects[i] = null;
			}
			visualEffectAnimators = null;
			visualEffectObjects = null;
		}
		if( clearModels )
		{
			projectileLaunchFront = null;
			projectileLaunchBack = null;
		}
	}

	public virtual void Clear()
	{
		standardDirections = null;
		adversaryComponents = null;
		adversaryObjects = null;
		ClearVisualEffects( true );
		ClearProjectiles();
		ClearAnimation();
		ClearAllSoundsFromResources();
		ClearAllSoundsFromAudioCenter();
	}

	public void Progress( float timeStep )
	{
		Vector2 animationDirection = Vector2.zero;
		int directionIndex = -1;

		if( !over )
		{
			if( !paused )
			{
				ProgressAutoRecovery( timeStep );
				ProgressProjectiles( timeStep );
				ProgressAI( timeStep );
				animationDirection = directionMove;
				if( (directionMove.x == 0f) && (directionMove.y == 0f) )
				{
					directionIndex = GetStandardDirectionIndex( DIRECTION_PURPOSE_MOVEMENT );
					if( directionIndex > -1 )
					{
						animationDirection = standardDirections[directionIndex];
					}
				}
				ProvideAnimationDirectionMove( animationDirection );
				if( projectileInterval > 0f )
				{
					if( projectileElapsed > -1f )
					{
						projectileElapsed += timeStep;
						if( projectileElapsed > projectileInterval )
						{
							projectileElapsed = -1f;
						}
					}
				}
				animationDirection = directionAim;
				if( (directionAim.x == 0f) && (directionAim.y == 0f) )
				{
					directionIndex = GetStandardDirectionIndex( DIRECTION_PURPOSE_AIM );
					if( directionIndex > -1 )
					{
						animationDirection = standardDirections[directionIndex];
					}
				}
				if( animationDirection != Vector2.zero )
				{
					ProvideAnimationDirectionAim( animationDirection );
					ReactToValidAim( animationDirection );
				}
				ProgressAnimation( timeStep );
				ProgressVisualEffects( timeStep );
			}
			else
			{
				/*halmeida - if the animator was properly paused, it will only progress certain effects.*/
				ProgressAnimation( timeStep );
			}
		}
	}

	protected virtual void ProgressAutoRecovery( float timeStep )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ProgressProjectiles( float timeStep )
	{
		ProjectileController projectileComponent = null;

		if( projectileComponents != null )
		{
			for( int i=0; i<projectileComponents.Length; i++ )
			{
				projectileComponent = projectileComponents[i];
				projectileComponent.Progress( timeStep );
				if( projectileComponent.IsOver() )
				{
					RemoveProjectile( i );
					if( projectileComponents == null )
					{
						return;
					}
					else
					{
						i--;
					}
				}
			}
		}
	}

	protected virtual void ProgressAI( float timeStep )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ProvideAnimationDirectionMove( Vector2 newAnimationDirection )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ProvideAnimationDirectionAim( Vector2 newAnimationDirection )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ProvideAnimationRequestFade( bool fadeToTransparency, bool immediately, float fadeSpeed )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ProgressAnimation( float timeStep )
	{
		if( dead )
		{
			over = true;
		}
	}

	protected virtual void ProgressVisualEffects( float timeStep )
	{
		SimpleAnimator effectAnimator = null;

		if( visualEffectAnimators != null )
		{
			for( int i=0; i<visualEffectAnimators.Length; i++ )
			{
				effectAnimator = visualEffectAnimators[i];
				effectAnimator.Progress( timeStep );
				if( effectAnimator.IsOver() )
				{
					RemoveVisualEffect( i );
					if( visualEffectAnimators == null )
					{
						return;
					}
					else
					{
						i--;
					}
				}
			}
		}
	}

	protected virtual void ResetAnimation( bool sideToFace )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ToggleAnimationPause( bool pause )
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void ClearAnimation()
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual bool ReactToValidAim( Vector2 normalizedDirection )
	{
		if( (projectileAttackModel != null) && (projectileElapsed == -1f) )
		{
			LaunchProjectile( projectileAttackModel, normalizedDirection, true, projectileSpeed, projectileAccel,
				projectileMaxSpeed, projectileDuration );
			projectileElapsed = 0f;
			return true;
		}
		return false;
	}

	public void FixedProgress( float timeStep )
	{
		if( !paused )
		{
			AdjustTemporaryMaxSpeed( timeStep );
			SelfAccelerate( timeStep );
		}
	}

	private void AdjustTemporaryMaxSpeed( float timeStep )
	{
		if( forcingFinalMaxSpeed && (finalMaxSpeedLerpTraveled < 1f) )
		{
			finalMaxSpeedLerpTraveled += finalMaxSpeedLerpSpeed * timeStep;
			if( finalMaxSpeedLerpTraveled > 1f )
			{
				finalMaxSpeedLerpTraveled = 1f;
			}
			finalMaxSpeedTemp = Mathf.Lerp( finalMaxSpeed, finalMaxSpeedForced, finalMaxSpeedLerpTraveled );
			finalMaxSqrSpeedTemp = finalMaxSpeedTemp * finalMaxSpeedTemp;
		}
	}

	private void SelfAccelerate( float timeStep )
	{
		int directionIndex = -1;
		float currentSqrSpeed = 0f;
		float maxSpeed = 0f;
		float maxSqrSpeed = 0f;

		if( rbody != null )
		{
			if( stopRequested )
			{
				rbody.velocity = Vector2.zero;
				stopRequested = false;
			}
			else
			{
				if( pushedIntensity > 0f )
				{
					rbody.velocity = directionPushed * pushedIntensity;
					pushedIntensity = 0f;
				}
				else
				{
					currentSqrSpeed = rbody.velocity.sqrMagnitude;
					if( forcingFinalMaxSpeed )
					{
						maxSpeed = finalMaxSpeedTemp;
						maxSqrSpeed = finalMaxSqrSpeedTemp;
					}
					else
					{
						maxSpeed = finalMaxSpeed;
						maxSqrSpeed = finalMaxSqrSpeed;
					}
					if( currentSqrSpeed < maxSqrSpeed )
					{
						if( (directionMove.x != 0f) || (directionMove.y != 0f) )
						{
							/*halmeida - using touch input.*/
							rbody.AddForce( directionMove * finalAccel * timeStep, ForceMode2D.Impulse );
							if( preciseMaxSpeed )
							{
								rbody.velocity = Vector2.ClampMagnitude( rbody.velocity, maxSpeed );
							}
						}
						else
						{
							/*halmeida - using keyboard input.*/
							directionIndex = GetStandardDirectionIndex( DIRECTION_PURPOSE_MOVEMENT );
							if( directionIndex > -1 )
							{
								rbody.AddForce( standardDirections[directionIndex] * finalAccel * timeStep, ForceMode2D.Impulse );
								if( preciseMaxSpeed )
								{
									rbody.velocity = Vector2.ClampMagnitude( rbody.velocity, maxSpeed );
								}
							}
						}
					}
				}
			}
		}
	}

	public void SetMovementDirection( Vector2 newPressedDirection )
	{
		directionMove = newPressedDirection;
	}

	public void SetAimDirection( Vector2 newPressedDirection )
	{
		directionAim = newPressedDirection;
	}

	public void SetUpDirectionMove( bool pressed )
	{
		pressingUpMove = pressed;
	}

	public void SetUpDirectionAim( bool pressed )
	{
		pressingUpAim = pressed;
	}

	public void SetLeftDirectionMove( bool pressed )
	{
		pressingLeftMove = pressed;
	}

	public void SetLeftDirectionAim( bool pressed )
	{
		pressingLeftAim = pressed;
	}

	public void SetDownDirectionMove( bool pressed )
	{
		pressingDownMove = pressed;
	}

	public void SetDownDirectionAim( bool pressed )
	{
		pressingDownAim = pressed;
	}

	public void SetRightDirectionMove( bool pressed )
	{
		pressingRightMove = pressed;
	}

	public void SetRightDirectionAim( bool pressed )
	{
		pressingRightAim = pressed;
	}

	public void CancelCommands()
	{
		directionMove = Vector2.zero;
		directionAim = Vector2.zero;
		SetUpDirectionMove( false );
		SetUpDirectionAim( false );
		SetLeftDirectionMove( false );
		SetLeftDirectionAim( false );
		SetDownDirectionMove( false );
		SetDownDirectionAim( false );
		SetRightDirectionMove( false );
		SetRightDirectionAim( false );
	}

	public void StopMovement()
	{
		stopRequested = true;
	}

	public void SufferPush( Vector2 pushOrigin, float pushPower )
	{
		/*halmeida - for now, we will ignore the push power and make the intensity constant.*/
		pushedIntensity = DEFAULT_PUSHED_INTENSITY;
		if( pushedIntensity > 0f )
		{
			directionPushed = (Vector2)transform.position - pushOrigin;
			directionPushed.Normalize();
		}
	}

	public bool IsDead()
	{
		return dead;
	}

	public bool IsOver()
	{
		return over;
	}

	public bool IsPaused()
	{
		return paused;
	}

	public bool ShouldUnpause()
	{
		return shouldUnpause;
	}

	public void FadeToTransparency()
	{
		ProvideAnimationRequestFade( true, true, 1f );
	}

	public void FadeToOpaque()
	{
		ProvideAnimationRequestFade( false, true, 1f );
	}

	public void StartFadingToTransparency( float fadeSpeed )
	{
		ProvideAnimationRequestFade( true, false, fadeSpeed );
	}

	public void StartFadingToOpaque( float fadeSpeed )
	{
		ProvideAnimationRequestFade( false, false, fadeSpeed );
	}

	public virtual bool IsOpaque()
	{
		return false;
	}

	public virtual bool IsTransparent()
	{
		return true;
	}

	public virtual bool GetFacingSide()
	{
		return BaseAnimator.SIDE_RIGHT;
	}

	public void SetFacingSide( bool newFacingSide )
	{
		ResetAnimation( newFacingSide );
	}

	public virtual void PrepareForRetry( bool sideToFace )
	{
		ClearProjectiles();
		CancelCommands();
		StopMovement();
		FillHP( true );
		ResetAnimation( sideToFace );
	}

	public virtual void TogglePause( bool pause )
	{
		if( pause )
		{
			if( !paused )
			{
				paused = true;
				if( rbody != null )
				{
					pausedVelocity = rbody.velocity;
					rbody.velocity = Vector2.zero;
				}
				shouldUnpause = false;
				ToggleAnimationPause( true );
			}
		}
		else
		{
			if( paused )
			{
				paused = false;
				if( rbody != null )
				{
					rbody.velocity = pausedVelocity;
				}
				ToggleAnimationPause( false );
			}
		}
	}

	public void ShowDialogue( int dialogueID )
	{
		string text = null;
		string speakerName = null;
		bool tryAbove = false;
		bool autoClose = false;
		GameTextDatabase gameTextDatabase = null;
		TextBoxManager textBoxManager = null;

		gameTextDatabase = GameTextDatabase.Instance;
		textBoxManager = TextBoxManager.Instance;
		if( (gameTextDatabase != null) && (textBoxManager != null) )
		{
			text = gameTextDatabase.GetDialogueText( dialogueID, ref speakerName, ref tryAbove, ref autoClose );
			textBoxManager.ShowDialogue( speakerName, text, tryAbove, autoClose );
		}
	}

	public Vector2 GetVelocity()
	{
		if( rbody != null )
		{
			return rbody.velocity;
		}
		return Vector2.zero;
	}

	public void ForceFinalMaxSpeedTo( float newMaxSpeed, float newLerpSpeed = 1f )
	{
		forcingFinalMaxSpeed = true;
		finalMaxSpeedForced = newMaxSpeed;
		finalMaxSqrSpeedForced = finalMaxSpeedForced * finalMaxSpeedForced;
		if( newLerpSpeed <= 0f )
		{
			finalMaxSpeedLerpSpeed = 0f;
			finalMaxSpeedLerpTraveled = 1f;
			finalMaxSpeedTemp = finalMaxSpeedForced;
			finalMaxSqrSpeedTemp = finalMaxSqrSpeedForced;
		}
		else
		{
			finalMaxSpeedLerpSpeed = newLerpSpeed;
			finalMaxSpeedLerpTraveled = 0f;
			finalMaxSpeedTemp = finalMaxSpeed;
			finalMaxSqrSpeedTemp = finalMaxSqrSpeed;
		}
	}

	public void CorrectFinalStats()
	{
		UpdateFinalStats();
	}

	private int GetStandardDirectionIndex( bool directionPurpose )
	{
		int bitwiseOr = 0;

		if( directionPurpose == DIRECTION_PURPOSE_MOVEMENT )
		{
			if( pressingUpMove )
			{
				bitwiseOr = bitwiseOr | 1;
			}
			if( pressingLeftMove )
			{
				bitwiseOr = bitwiseOr | 2;
			}
			if( pressingDownMove )
			{
				bitwiseOr = bitwiseOr | 4;
			}
			if( pressingRightMove )
			{
				bitwiseOr = bitwiseOr | 8;
			}
		}
		else
		{
			if( pressingUpAim )
			{
				bitwiseOr = bitwiseOr | 1;
			}
			if( pressingLeftAim )
			{
				bitwiseOr = bitwiseOr | 2;
			}
			if( pressingDownAim )
			{
				bitwiseOr = bitwiseOr | 4;
			}
			if( pressingRightAim )
			{
				bitwiseOr = bitwiseOr | 8;
			}
		}
		switch( bitwiseOr )
		{
			case 1:
				//Debug.Log("Debug : PlayerController : pressing U.");
				return 0;
			case 2:
				//Debug.Log("Debug : PlayerController : pressing L.");
				return 2;
			case 3:
				//Debug.Log("Debug : PlayerController : pressing UL.");
				return 1;
			case 4:
				//Debug.Log("Debug : PlayerController : pressing D.");
				return 4;
			case 5:
				//Debug.Log("Debug : PlayerController : pressing U and D.");
				break;
			case 6:
				//Debug.Log("Debug : PlayerController : pressing LD.");
				return 3;
			case 7:
				//Debug.Log("Debug : PlayerController : pressing U, L and D.");
				break;
			case 8:
				//Debug.Log("Debug : PlayerController : pressing R.");
				return 6;
			case 9:
				//Debug.Log("Debug : PlayerController : pressing UR.");
				return 7;
			case 10:
				//Debug.Log("Debug : PlayerController : pressing L and R.");
				break;
			case 11:
				//Debug.Log("Debug : PlayerController : pressing U, L and R.");
				break;
			case 12:
				//Debug.Log("Debug : PlayerController : pressing DR.");
				return 5;
		}
		return -1;
	}

	public float GetWidth()
	{
		return width;
	}

	public float GetHeight()
	{
		return height;
	}

	public void SetCurrentName( string newCurrentName )
	{
		currentName = newCurrentName;
		if( string.IsNullOrEmpty( currentName ) )
		{
			currentName = "???";
		}
	}

	public string GetCurrentName()
	{
		return currentName;
	}

	public void ToggleColliders( bool enable )
	{
		if( mainCollider != null )
		{
			mainCollider.enabled = enable;
		}
	}

	protected virtual void LoadAllSoundsFromResources()
	{
		UsefulFunctions.LoadSoundsFromResources( damageInSoundPaths, ref damageInSounds );
		UsefulFunctions.LoadSoundsFromResources( deathSoundPaths, ref deathSounds );
	}

	protected virtual void ClearAllSoundsFromResources()
	{
		UsefulFunctions.ClearSoundsFromResources( ref damageInSounds );
		UsefulFunctions.ClearSoundsFromResources( ref deathSounds );
	}

	protected virtual void LoadAllSoundsIntoAudioCenter()
	{
		UsefulFunctions.LoadSoundsIntoAudioCenter( damageInSoundPaths, ref damageInSoundIDs );
		UsefulFunctions.LoadSoundsIntoAudioCenter( deathSoundPaths, ref deathSoundIDs );
	}

	protected virtual void ClearAllSoundsFromAudioCenter()
	{
		UsefulFunctions.ClearSoundsFromAudioCenter( ref damageInSoundIDs );
		UsefulFunctions.ClearSoundsFromAudioCenter( ref deathSoundIDs );
	}

	protected virtual void AddAudioSources()
	{
		audioSourceDamageIn = gameObject.AddComponent<AudioSource>();
		audioSourceVoice = gameObject.AddComponent<AudioSource>();
	}

	protected virtual void PlaySoundEffect( int soundFXCategory )
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
