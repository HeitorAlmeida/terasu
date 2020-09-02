using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : ReactionController
{
	public const int EFFECT_CODE_INVALID = -1;
	public const int EFFECT_CODE_HP_DAMAGE = 0;
	public const int EFFECT_CODE_HP_RECOVERY = 1;
	
	protected BaseAgent triggerSourceComponent;
	protected BaseAgent[] triggerComponents;
	protected BaseAgent[] triggeringComponents;

	private Collider2D projectileCollider;
	private bool active;
	private bool over;
	private Vector2 direction;
	private float speed;
	private Vector2 vectorSpeed;
	private float acceleration;
	private float maxSpeed;
	private int effectCode;
	private float effectIntensity;
	private int destructionLevel;
	private float activeDuration;
	private float activeElapsed;
	private ShortLifeAnimator projectileAnimator;
	private float accumulatedEffect;

	protected override void Awake()
	{
		base.Awake();
		triggerSourceComponent = null;
		triggerComponents = null;
		triggeringComponents = null;
		projectileCollider = GetComponent<Collider2D>();
		active = true;
		over = false;
		direction = Vector2.zero;
		speed = 0f;
		vectorSpeed = Vector2.zero;
		acceleration = 0f;
		maxSpeed = 0f;
		effectCode = EFFECT_CODE_INVALID;
		effectIntensity = 0f;
		destructionLevel = ObstacleController.CONSTRUCTION_LEVEL_INVALID;
		activeDuration = 0f;
		activeElapsed = 0f;
		projectileAnimator = GetComponent<ShortLifeAnimator>();
		accumulatedEffect = 0f;
	}

	public void SetOrientation( Vector2 newNormalizedDirection )
	{
		float angleWithRight = 0f;

		direction = newNormalizedDirection;
		angleWithRight = UsefulFunctions.GetDirectionAngle( direction );
		transform.localRotation = Quaternion.Euler( 0f, 0f, angleWithRight );
		vectorSpeed = direction * speed;
	}

	public void SetSpeed( float newSpeed, float newAcceleration, float newMaxSpeed )
	{
		speed = newSpeed;
		vectorSpeed = direction * speed;
		acceleration = newAcceleration;
		maxSpeed = (newMaxSpeed > 0f) ? newMaxSpeed : -newMaxSpeed;
	}

	public void SetDuration( float newDuration )
	{
		activeDuration = newDuration;
	}

	public void SetEffect( int newEffectCode, float newEffectIntensity, int newDestructionLevel )
	{
		effectCode = newEffectCode;
		effectIntensity = (newEffectIntensity < 0f) ? -newEffectIntensity : newEffectIntensity;
		destructionLevel = newDestructionLevel;
	}

	protected override bool SourceCompatible( GameObject candidateSource )
	{
		if( base.SourceCompatible( candidateSource ) )
		{
			triggerSourceComponent = triggerSourceObject.GetComponent<BaseAgent>();
			if( triggerSourceComponent != null )
			{
				return true;
			}
		}
		return false;
	}

	protected override void ExtractTriggers()
	{
		switch( effectCode )
		{
			case EFFECT_CODE_HP_DAMAGE:
				triggerObjects = triggerSourceComponent.GetAdversaryObjects();
				triggerComponents = triggerSourceComponent.GetAdversaryComponents();
				break;
			case EFFECT_CODE_HP_RECOVERY:
				/*halmeida - gotta implement a way to get the allies of an agent.*/
				triggerObjects = null;
				triggerComponents = null;
				break;
			default:
				triggerObjects = null;
				triggerComponents = null;
				break;
		}
	}

	/*halmeida - this function should only return an index if that index is valid across all relevant arrays,
	i.e., all objects and components related to that index are valid and not null.*/
	protected override int GetTriggerIndex( GameObject possibleTrigger )
	{
		int triggerIndex = -1;

		triggerIndex = base.GetTriggerIndex( possibleTrigger );
		if( (triggerIndex > -1) && (triggerComponents != null) )
		{
			if( triggerIndex < triggerComponents.Length )
			{
				if( triggerComponents[triggerIndex] != null )
				{
					return triggerIndex;
				}
			}
		}
		return -1;
	}

	/*halmeida - this function receives an index that is valid across all relevant arrays and denotes
	an element that is not null across all relevant arrays.*/
	protected override void Trigger( int triggerIndex )
	{
		BaseAgent agentComponent = null;

		agentComponent = triggerComponents[triggerIndex];
		if( !continuousEffect )
		{
			switch( effectCode )
			{
				case EFFECT_CODE_HP_DAMAGE:
					agentComponent.SufferHPDamage( (int)effectIntensity, gameObject, triggerSourceComponent );
					break;
				case EFFECT_CODE_HP_RECOVERY:
					agentComponent.AddToCurrentHP( (int)effectIntensity, false );
					break;
			}
			Deactivate();
		}
		base.Trigger( triggerIndex );
	}

	public void Deactivate()
	{
		if( active )
		{
			active = false;
			if( projectileCollider != null )
			{
				projectileCollider.enabled = false;
			}
			speed = 0f;
			vectorSpeed = Vector2.zero;
			if( projectileAnimator != null )
			{
				projectileAnimator.StartDying();
			}
		}
	}

	protected override void AddTriggeringObject( int triggerIndex )
	{
		base.AddTriggeringObject( triggerIndex );
		UsefulFunctions.IncreaseArray<BaseAgent>( ref triggeringComponents, triggerComponents[triggerIndex] );
	}

	protected override void RemoveTriggeringObject( int triggerIndex )
	{
		base.RemoveTriggeringObject( triggerIndex );
		UsefulFunctions.DecreaseArray<BaseAgent>( ref triggeringComponents, triggerIndex );
	}

	public override void Clear()
	{
		base.Clear();
		projectileCollider = null;
		if( projectileAnimator != null )
		{
			projectileAnimator.Clear();
			projectileAnimator = null;
		}
		accumulatedEffect = 0f;
	}

	protected override void ClearTriggers()
	{
		base.ClearTriggers();
		triggerSourceComponent = null;
		triggerComponents = null;
		triggeringComponents = null;
	}

	public bool IsOver()
	{
		return over;
	}

	public FakeLight GetCurrentFakeLight()
	{
		if( projectileAnimator != null )
		{
			return projectileAnimator.GetCurrentFakeLight();
		}
		return null;
	}

	public virtual void Progress( float timeStep )
	{
		if( !over )
		{
			if( active )
			{
				if( activeDuration > 0f )
				{
					activeElapsed += timeStep;
					if( activeElapsed > activeDuration )
					{
						Deactivate();
					}
				}
				if( active )
				{
					Accelerate( timeStep );
					Move( timeStep );
				}
			}
			if( triggered && continuousEffect )
			{
				if( triggeringComponents != null )
				{
					for( int i=0; i<triggeringComponents.Length; i++ )
					{
						ApplyContinuousEffect( triggeringComponents[i], timeStep );
					}
				}
			}
			if( projectileAnimator != null )
			{
				projectileAnimator.Progress( timeStep );
				if( projectileAnimator.IsDead() )
				{
					/*halmeida - the projectile animation may start dying before we actually ask it to die.
					This may be due to the projectile not having a properly configured set of animations.
					In this case, we have to deactivate it now, since it does no longer exist visually.*/
					if( active )
					{
						Deactivate();
					}
					/*halmeida - wether we asked it to die or it died by itself, at this point the projectile
					does no longer exist.*/
					over = true;
				}
			}
		}
	}

	private void Accelerate( float timeStep )
	{
		speed += acceleration * timeStep;
		if( speed > 0f )
		{
			if( speed > maxSpeed )
			{
				speed = maxSpeed;
			}
		}
		else
		{
			if( speed < -maxSpeed )
			{
				speed = -maxSpeed;
			}
		}
		vectorSpeed = direction * speed;
	}

	private void Move( float timeStep )
	{
		Vector3 position = Vector3.zero;
		Vector2 offset = Vector2.zero;

		if( speed != 0f )
		{
			offset = vectorSpeed * timeStep;
			position = transform.localPosition;
			position += new Vector3( offset.x, offset.y, 0f );
			transform.localPosition = position;
		}
	}

	protected virtual void ApplyContinuousEffect( BaseAgent triggeringComponent, float timeStep )
	{
		int applicableEffect = 0;

		if( triggeringComponent != null )
		{
			switch( effectCode )
			{
				case EFFECT_CODE_HP_DAMAGE:
					accumulatedEffect += timeStep * effectIntensity;
					applicableEffect = (int)accumulatedEffect;
					if( applicableEffect > 0 )
					{
						accumulatedEffect -= applicableEffect;
						triggeringComponent.SufferHPDamage( applicableEffect, gameObject, triggerSourceComponent );
					}
					break;
				case EFFECT_CODE_HP_RECOVERY:
					accumulatedEffect += timeStep * effectIntensity;
					applicableEffect = (int)accumulatedEffect;
					if( applicableEffect > 0 )
					{
						accumulatedEffect -= applicableEffect;
						triggeringComponent.AddToCurrentHP( applicableEffect, false );
					}
					break;
			}
		}
	}

	public int GetEffectCode()
	{
		return effectCode;
	}

	public float GetEffectIntensity()
	{
		return effectIntensity;
	}

	public int GetDestructionLevel()
	{
		return destructionLevel;
	}

	public virtual void CorrectDepthChain()
	{
		if( projectileAnimator != null )
		{
			projectileAnimator.StartDepthChain( gameObject.transform.position.z, true );
		}
	}

	protected virtual void OnTriggerEnter2D( Collider2D otherCollider )
	{
		TryToTrigger( otherCollider.gameObject );
	}

	protected virtual void OnTriggerExit2D( Collider2D otherCollider )
	{
		TryToUntrigger( otherCollider.gameObject );
	}
}
