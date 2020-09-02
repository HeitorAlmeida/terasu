using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgent : RoutedAgent
{
	public int rewardXP;
	public float damageWaveLength;

	private SimpleAnimator enemyAnim;
	private Color waveColor;
	private float waveFrontLimit;
	private Vector4 waveLengths;
	private Vector4 waveLengthsLast;
	private Vector4 waveAlphas;
	private Vector4 waveAlphasLast;
	private float waveSpeed;
	private float waveSpeedLast;

	protected override void Awake()
	{
		rewardXP = (rewardXP < 0) ? 0 : rewardXP;
		damageWaveLength = (damageWaveLength < 0f) ? 0f : damageWaveLength;
		enemyAnim = GetComponent<SimpleAnimator>();
		waveColor = Color.white;
		waveFrontLimit = 3f * damageWaveLength;
		waveLengths = new Vector4( damageWaveLength, damageWaveLength/4f, damageWaveLength/8f, damageWaveLength/12f );
		waveLengthsLast = waveLengths / 2f;
		waveLengthsLast.x /= 3f;
		waveAlphas = new Vector4( 1f, 0.7f, 0.45f, 0.2f );
		waveAlphasLast = waveAlphas;
		waveSpeed = 10f * damageWaveLength;
		waveSpeedLast = waveSpeed / 6f;
		base.Awake();
	}

	public override bool SufferHPDamage( int damagePower, GameObject damageSourceObject, BaseAgent damageSourceAgent, bool displayDamage = true )
	{
		if( base.SufferHPDamage( damagePower, damageSourceObject, damageSourceAgent, displayDamage ) )
		{
			if( damageSourceObject != null )
			{
				if( enemyAnim != null )
				{
					if( !dead )
					{
						enemyAnim.StartWave( waveColor, damageSourceObject.transform.localPosition, waveLengths, waveAlphas, waveFrontLimit,
							waveSpeed, 0f, 0f, 0f, null, 0f, 0f );
					}
					else
					{
						enemyAnim.StartWave( waveColor, damageSourceObject.transform.localPosition, waveLengthsLast, waveAlphasLast, waveFrontLimit,
							waveSpeedLast, 0f, 0f, 1f, null, 0f, 0f );
					}
				}
			}
			if( (damageSourceAgent != null) && dead )
			{
				damageSourceAgent.ReceiveExperience( rewardXP, true );
			}
			return true;
		}
		return false;
	}

	protected override void ProgressAnimation( float timeStep )
	{
		if( enemyAnim != null )
		{
			enemyAnim.Progress( timeStep );
			if( enemyAnim.IsOver() )
			{
				over = true;
			}
		}
		else
		{
			if( dead )
			{
				over = true;
			}
		}
	}

	protected override void ToggleAnimationPause( bool pause )
	{
		if( enemyAnim != null )
		{
			enemyAnim.TogglePause( pause );
		}
	}

	protected override void ClearAnimation()
	{
		if( enemyAnim != null )
		{
			enemyAnim.Clear();
		}
	}

	protected virtual void OnCollisionEnter2D( Collision2D collision )
	{
		GameObject adversaryObject = null;
		BaseAgent adversaryComponent = null;

		if( (collision != null) && (adversaryObjects != null) && (adversaryComponents != null) )
		{
			if( collision.gameObject != null )
			{
				/*halmeida - this is the invader's object.*/
				for( int i=0; i<adversaryObjects.Length; i++ )
				{
					adversaryObject = adversaryObjects[i];
					if( adversaryObject == collision.gameObject )
					{
						adversaryComponent = adversaryComponents[i];
						if( adversaryComponent != null )
						{
							adversaryComponent.SufferHPDamage( (int)finalAttack, gameObject, this );
							adversaryComponent.SufferPush( (Vector2)transform.position, finalAttack );
						}
						break;
					}
				}
			}
		}
	}
}
